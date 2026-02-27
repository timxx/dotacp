using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for union type structs.
    /// Handles serialization/deserialization of types that can be one of several different types.
    /// </summary>
    /// <typeparam name="TUnion">The union type struct</typeparam>
    public class UnionTypeConverter<TUnion> : JsonConverter<TUnion>
    {
        private static readonly FieldInfo _valueField = typeof(TUnion).GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo _isNullField = typeof(TUnion).GetField("_isNull", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo _nullProperty = typeof(TUnion).GetProperty("Null", BindingFlags.Public | BindingFlags.Static);

        public override TUnion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Try to determine the type from the JSON token
            object value = null;
            bool isNull = false;

            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    // Check if this union type supports null
                    if (_nullProperty != null)
                    {
                        return (TUnion)_nullProperty.GetValue(null);
                    }
                    isNull = true;
                    break;

                case JsonTokenType.String:
                    value = reader.GetString();
                    break;

                case JsonTokenType.Number:
                    // Try int first, then long, then double
                    if (reader.TryGetInt32(out int intVal))
                    {
                        value = intVal;
                    }
                    else if (reader.TryGetInt64(out long longVal))
                    {
                        value = longVal;
                    }
                    else
                    {
                        value = reader.GetDouble();
                    }
                    break;

                case JsonTokenType.True:
                    value = true;
                    break;

                case JsonTokenType.False:
                    value = false;
                    break;

                default:
                    throw new JsonException($"Unexpected token type {reader.TokenType} for union type {typeof(TUnion).Name}");
            }

            if (isNull)
            {
                throw new JsonException($"Union type {typeof(TUnion).Name} does not support null values");
            }

            // Find a constructor that matches the value type
            var ctors = typeof(TUnion).GetConstructors();
            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 1)
                {
                    var paramType = parameters[0].ParameterType;
                    
                    // Check if the value can be assigned to this parameter type
                    if (value == null)
                    {
                        // Null can only be assigned to reference types or Nullable<T>
                        if (!paramType.IsValueType || Nullable.GetUnderlyingType(paramType) != null)
                        {
                            return (TUnion)ctor.Invoke(new object[] { null });
                        }
                    }
                    else if (paramType.IsAssignableFrom(value.GetType()))
                    {
                        return (TUnion)ctor.Invoke(new object[] { value });
                    }
                    // Try conversion for numeric types
                    else if (paramType == typeof(long) && value is int intValue)
                    {
                        return (TUnion)ctor.Invoke(new object[] { (long)intValue });
                    }
                    else if (paramType == typeof(int) && value is long longValue && longValue >= int.MinValue && longValue <= int.MaxValue)
                    {
                        return (TUnion)ctor.Invoke(new object[] { (int)longValue });
                    }
                }
            }

            throw new JsonException($"No suitable constructor found for union type {typeof(TUnion).Name} with value type {value?.GetType().Name ?? "null"}");
        }

        public override void Write(Utf8JsonWriter writer, TUnion value, JsonSerializerOptions options)
        {
            // Check if this is a null value
            if (_isNullField != null)
            {
                var isNullValue = (bool)_isNullField.GetValue(value);
                if (isNullValue)
                {
                    writer.WriteNullValue();
                    return;
                }
            }

            // Extract the underlying value
            var underlyingValue = _valueField.GetValue(value);
            if (underlyingValue == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                // Serialize the underlying value directly
                JsonSerializer.Serialize(writer, underlyingValue, underlyingValue.GetType(), options);
            }
        }
    }
}
