using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for union type structs.
    /// Handles serialization/deserialization of types that can be one of several different types.
    /// </summary>
    /// <typeparam name="TUnion">The union type struct</typeparam>
    internal class UnionTypeConverter<TUnion> : JsonConverter
    {
        private static readonly FieldInfo _valueField = typeof(TUnion).GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo _isNullField = typeof(TUnion).GetField("_isNull", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo _nullProperty = typeof(TUnion).GetProperty("Null", BindingFlags.Public | BindingFlags.Static);

        public override bool CanConvert(Type objectType) => objectType == typeof(TUnion);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.ReadFrom(reader);

            if (token.Type == JTokenType.Null)
            {
                if (_nullProperty != null)
                {
                    return _nullProperty.GetValue(null);
                }

                throw new JsonSerializationException(
                    $"Union type {typeof(TUnion).Name} does not support null values");
            }

            // Find a constructor that matches the value type
            var candidates = new List<(object Value, int Score, ConstructorInfo Ctor)>();

            foreach (var ctor in typeof(TUnion).GetConstructors())
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 1)
                {
                    var paramType = parameters[0].ParameterType;

                    try
                    {
                        var converted = token.ToObject(paramType, serializer);
                        if (converted != null || !paramType.IsValueType || Nullable.GetUnderlyingType(paramType) != null)
                        {
                            var score = GetMatchScore(token, paramType);
                            candidates.Add((converted, score, ctor));
                        }
                    }
                    catch (Exception)
                    {
                        // Try next constructor type
                    }
                }
            }

            if (candidates.Count > 0)
            {
                var best = candidates
                    .OrderByDescending(c => c.Score)
                    .ThenBy(c => c.Ctor.GetParameters()[0].ParameterType.FullName, StringComparer.Ordinal)
                    .First();

                return best.Ctor.Invoke(new[] { best.Value });
            }

            throw new JsonSerializationException(
                $"No suitable constructor found for union type {typeof(TUnion).Name} " +
                $"for JSON token type {token.Type}");
        }

        private static int GetMatchScore(JToken token, Type paramType)
        {
            if (token.Type == JTokenType.Array && paramType.IsArray)
            {
                var elementType = paramType.GetElementType();
                var tokenArray = (JArray)token;
                if (elementType != null && tokenArray.Count > 0 && tokenArray[0] is JObject firstObject)
                {
                    return 100 + GetObjectPropertyMatchScore(firstObject, elementType);
                }

                return 100;
            }

            if (token.Type == JTokenType.Object)
            {
                if (token is JObject tokenObject)
                {
                    return 100 + GetObjectPropertyMatchScore(tokenObject, paramType);
                }

                return 100;
            }

            return 10;
        }

        private static int GetObjectPropertyMatchScore(JObject tokenObject, Type targetType)
        {
            var targetNames = new HashSet<string>(
                targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? ToCamelCase(p.Name)),
                StringComparer.Ordinal);

            var score = 0;
            foreach (var prop in tokenObject.Properties())
            {
                if (targetNames.Contains(prop.Name))
                {
                    score += 5;
                }
            }

            return score;
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            {
                return value;
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (_isNullField != null)
            {
                var isNull = (bool)_isNullField.GetValue(value);
                if (isNull)
                {
                    writer.WriteNull();
                    return;
                }
            }

            var underlyingValue = _valueField.GetValue(value);
            if (underlyingValue == null)
                writer.WriteNull();
            else
                serializer.Serialize(writer, underlyingValue);
        }
    }
}
