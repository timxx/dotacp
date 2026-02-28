using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace dotacp.generator
{
    /// <summary>
    /// Information about a discriminator base type
    /// </summary>
    public class DiscriminatorBaseInfo
    {
        public string PropertyName { get; set; } = "";
        public string PropertyCsName { get; set; } = "";
        public string PropertyJsonName { get; set; } = "";
        public Dictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Information about a discriminator derived type
    /// </summary>
    public class DiscriminatorDerivedInfo
    {
        public string BaseName { get; set; } = "";
        public string PropertyName { get; set; } = "";
        public string PropertyCsName { get; set; } = "";
        public string PropertyJsonName { get; set; } = "";
        public string DiscriminatorValue { get; set; } = "";
        public bool IsAbstract { get; set; }
    }

    /// <summary>
    /// Information about a variant class for discriminated unions
    /// </summary>
    public class DiscriminatorVariant
    {
        public string ClassName { get; set; } = "";
        public string BaseClassName { get; set; } = "";
        public string DiscriminatorPropertyName { get; set; } = "";
        public string DiscriminatorPropertyCsName { get; set; } = "";
        public string DiscriminatorPropertyJsonName { get; set; } = "";
        public string DiscriminatorValue { get; set; } = "";
        public JObject Definition { get; set; } = new JObject();
        public string Description { get; set; }
    }
}
