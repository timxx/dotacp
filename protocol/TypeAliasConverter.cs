using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for type alias structs.
    /// Serializes/deserializes the underlying value type directly.
    /// </summary>
    /// <typeparam name="TAlias">The type alias struct type</typeparam>
    /// <typeparam name="TValue">The underlying value type</typeparam>
    public class TypeAliasConverter<TAlias, TValue> : JsonConverter<TAlias>
    {
        private static readonly FieldInfo _valueField = typeof(TAlias).GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);

        public override TAlias Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize as the underlying type
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            // Use Activator to create the struct with the value
            return (TAlias)Activator.CreateInstance(typeof(TAlias), value);
        }

        public override void Write(Utf8JsonWriter writer, TAlias value, JsonSerializerOptions options)
        {
            // Extract the underlying value using reflection
            var underlyingValue = (TValue)_valueField.GetValue(value);

            // Serialize as the underlying type
            JsonSerializer.Serialize(writer, underlyingValue, options);
        }
    }
}
