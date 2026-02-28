using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for discriminated union base classes.
    /// Uses a discriminator property to select the concrete derived type.
    /// </summary>
    /// <typeparam name="TBase">The base type for the discriminated union.</typeparam>
    public sealed class DiscriminatorConverter<TBase> : JsonConverter where TBase : class
    {
        private static readonly Lazy<DiscriminatorInfo> _info = new Lazy<DiscriminatorInfo>(BuildInfo);

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => typeof(TBase).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var info = _info.Value;

            if (!jsonObject.TryGetValue(info.PropertyName, StringComparison.Ordinal, out var discriminatorToken))
            {
                throw new JsonSerializationException(
                    $"Missing discriminator property '{info.PropertyName}' for {typeof(TBase).Name}.");
            }

            var discriminatorValue = discriminatorToken.Type == JTokenType.String
                ? discriminatorToken.Value<string>()
                : discriminatorToken.ToString();

            if (!info.Mapping.TryGetValue(discriminatorValue, out var targetType))
            {
                throw new JsonSerializationException(
                    $"Unknown discriminator value '{discriminatorValue}' for {typeof(TBase).Name}.");
            }

            var target = Activator.CreateInstance(targetType);
            using (var jsonReader = jsonObject.CreateReader())
            {
                serializer.Populate(jsonReader, target);
            }

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("WriteJson is not used when CanWrite is false.");
        }

        private static DiscriminatorInfo BuildInfo()
        {
            var baseType = typeof(TBase);
            var propertyField = baseType.GetField("DiscriminatorPropertyName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var mappingField = baseType.GetField("DiscriminatorMapping", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (propertyField == null || mappingField == null)
            {
                throw new InvalidOperationException(
                    $"Discriminator metadata fields not found on {baseType.Name}."
                );
            }

            var propertyName = propertyField.GetValue(null) as string;
            var mapping = mappingField.GetValue(null) as Dictionary<string, Type>;

            if (string.IsNullOrWhiteSpace(propertyName) || mapping == null)
            {
                throw new InvalidOperationException(
                    $"Discriminator metadata fields are invalid on {baseType.Name}."
                );
            }

            return new DiscriminatorInfo(propertyName, mapping);
        }

        private sealed class DiscriminatorInfo
        {
            public DiscriminatorInfo(string propertyName, Dictionary<string, Type> mapping)
            {
                PropertyName = propertyName;
                Mapping = mapping;
            }

            public string PropertyName { get; }
            public Dictionary<string, Type> Mapping { get; }
        }
    }
}
