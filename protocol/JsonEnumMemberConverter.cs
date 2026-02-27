using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace dotacp.protocol
{
    /// <summary>
    /// Generic JSON converter for enums that uses JsonEnumValueAttribute to map between
    /// enum members and their JSON string representations.
    /// Falls back to the enum member name if no attribute is present.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to convert.</typeparam>
    public class JsonEnumMemberConverter<TEnum> : JsonConverter where TEnum : struct, Enum
    {
        private readonly Dictionary<TEnum, string> _enumToString;
        private readonly Dictionary<string, TEnum> _stringToEnum;

        public JsonEnumMemberConverter()
        {
            _enumToString = new Dictionary<TEnum, string>();
            _stringToEnum = new Dictionary<string, TEnum>(StringComparer.Ordinal);

            var enumType = typeof(TEnum);
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var enumValue = (TEnum)field.GetValue(null);
                var attribute = field.GetCustomAttribute<JsonEnumValueAttribute>();
                var stringValue = attribute != null ? attribute.Value : field.Name;

                _enumToString[enumValue] = stringValue;
                _stringToEnum[stringValue] = enumValue;
            }
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(TEnum);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            if (value == null)
            {
                throw new JsonSerializationException($"Cannot convert null to {typeof(TEnum).Name}");
            }

            if (_stringToEnum.TryGetValue(value, out var enumValue))
            {
                return enumValue;
            }

            throw new JsonSerializationException($"Unknown value '{value}' for enum {typeof(TEnum).Name}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is TEnum enumValue && _enumToString.TryGetValue(enumValue, out var stringValue))
            {
                writer.WriteValue(stringValue);
            }
            else
            {
                throw new JsonSerializationException($"Unknown enum value '{value}' for {typeof(TEnum).Name}");
            }
        }
    }
}
