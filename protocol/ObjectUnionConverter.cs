using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace dotacp.protocol
{
    /// <summary>
    /// JSON converter for object unions (abstract base + derived types) that have no discriminator in JSON.
    /// Tries to deserialize to each variant type; prefers types whose required JSON keys are present.
    /// Contrast with <see cref="UnionTypeConverter{TUnion}"/>, which handles value/struct unions (e.g. int | string | null).
    /// </summary>
    /// <typeparam name="TBase">The abstract base type for the union.</typeparam>
    internal sealed class ObjectUnionConverter<TBase> : JsonConverter where TBase : class
    {
        private static readonly Lazy<Type[]> _variantTypes = new Lazy<Type[]>(LoadVariantTypes);

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => typeof(TBase).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var variantTypes = _variantTypes.Value;

            if (variantTypes == null || variantTypes.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Union variant types not found on {typeof(TBase).Name}.");
            }

            // Prefer types whose UnionVariantRequiredJsonKeys are all present in the JSON (disambiguates without discriminator)
            var ordered = variantTypes
                .Select(t => new { Type = t, Keys = GetRequiredJsonKeys(t) })
                .OrderByDescending(x => x.Keys != null && x.Keys.Length > 0 && x.Keys.All(k => jsonObject[k] != null))
                .ThenBy(x => x.Keys == null || x.Keys.Length == 0 ? 1 : 0)
                .Select(x => x.Type)
                .ToArray();

            Exception lastException = null;
            foreach (var targetType in ordered)
            {
                try
                {
                    var target = Activator.CreateInstance(targetType);
                    using (var jsonReader = jsonObject.CreateReader())
                    {
                        serializer.Populate(jsonReader, target);
                    }
                    return target;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            throw new JsonSerializationException(
                $"Could not deserialize to any union variant of {typeof(TBase).Name}. Tried: {string.Join(", ", Array.ConvertAll(variantTypes, t => t.Name))}.",
                lastException);
        }

        private static string[] GetRequiredJsonKeys(Type type)
        {
            var field = type.GetField("UnionVariantRequiredJsonKeys", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as string[];
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("WriteJson is not used when CanWrite is false.");
        }

        private static Type[] LoadVariantTypes()
        {
            var baseType = typeof(TBase);
            var field = baseType.GetField("UnionVariantTypes", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (field == null)
            {
                throw new InvalidOperationException(
                    $"UnionVariantTypes field not found on {baseType.Name}.");
            }

            var value = field.GetValue(null) as Type[];
            if (value == null || value.Length == 0)
            {
                throw new InvalidOperationException(
                    $"UnionVariantTypes is null or empty on {baseType.Name}.");
            }

            return value;
        }
    }
}
