using System;
using System.Reflection;
using Newtonsoft.Json;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for type alias structs.
    /// Serializes/deserializes the underlying value type directly.
    /// </summary>
    /// <typeparam name="TAlias">The type alias struct type</typeparam>
    /// <typeparam name="TValue">The underlying value type</typeparam>
    public class TypeAliasConverter<TAlias, TValue> : JsonConverter
    {
        private static readonly FieldInfo _valueField = typeof(TAlias).GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);

        public override bool CanConvert(Type objectType) => objectType == typeof(TAlias);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<TValue>(reader);
            return Activator.CreateInstance(typeof(TAlias), value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var underlyingValue = (TValue)_valueField.GetValue(value);
            serializer.Serialize(writer, underlyingValue);
        }
    }
}
