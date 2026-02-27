using System;
using System.Reflection;
using Newtonsoft.Json;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for union type structs.
    /// Handles serialization/deserialization of types that can be one of several different types.
    /// </summary>
    /// <typeparam name="TUnion">The union type struct</typeparam>
    public class UnionTypeConverter<TUnion> : JsonConverter
    {
        private static readonly FieldInfo _valueField = typeof(TUnion).GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo _isNullField = typeof(TUnion).GetField("_isNull", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo _nullProperty = typeof(TUnion).GetProperty("Null", BindingFlags.Public | BindingFlags.Static);

        public override bool CanConvert(Type objectType) => objectType == typeof(TUnion);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = null;
            bool isNull = false;

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    if (_nullProperty != null)
                        return _nullProperty.GetValue(null);
                    isNull = true;
                    break;

                case JsonToken.String:
                    value = (string)reader.Value;
                    break;

                case JsonToken.Integer:
                    // Newtonsoft reads integers as long by default
                    value = (long)reader.Value;
                    break;

                case JsonToken.Float:
                    value = (double)reader.Value;
                    break;

                case JsonToken.Boolean:
                    value = (bool)reader.Value;
                    break;

                default:
                    throw new JsonSerializationException(
                        $"Unexpected token type {reader.TokenType} for union type {typeof(TUnion).Name}");
            }

            if (isNull)
                throw new JsonSerializationException(
                    $"Union type {typeof(TUnion).Name} does not support null values");

            // Find a constructor that matches the value type
            foreach (var ctor in typeof(TUnion).GetConstructors())
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 1)
                {
                    var paramType = parameters[0].ParameterType;

                    if (value == null)
                    {
                        if (!paramType.IsValueType || Nullable.GetUnderlyingType(paramType) != null)
                            return ctor.Invoke(new object[] { null });
                    }
                    else if (paramType.IsAssignableFrom(value.GetType()))
                    {
                        return ctor.Invoke(new object[] { value });
                    }
                    else if (paramType == typeof(long) && value is int intValue)
                    {
                        return ctor.Invoke(new object[] { (long)intValue });
                    }
                    else if (paramType == typeof(int) && value is long longValue
                        && longValue >= int.MinValue && longValue <= int.MaxValue)
                    {
                        return ctor.Invoke(new object[] { (int)longValue });
                    }
                }
            }

            throw new JsonSerializationException(
                $"No suitable constructor found for union type {typeof(TUnion).Name} " +
                $"with value type {value?.GetType().Name ?? "null"}");
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
