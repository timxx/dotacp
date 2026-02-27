using System;

namespace dotacp.protocol
{
    /// <summary>
    /// Specifies the JSON string value for an enum member.
    /// Used when the JSON representation differs from the enum member name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonEnumValueAttribute : Attribute
    {
        /// <summary>
        /// Gets the JSON string value for this enum member.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the JsonEnumValueAttribute class.
        /// </summary>
        /// <param name="value">The JSON string value for this enum member.</param>
        public JsonEnumValueAttribute(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
