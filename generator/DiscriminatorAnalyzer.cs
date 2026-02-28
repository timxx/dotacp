using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace dotacp.generator
{
    /// <summary>
    /// Analyzes JSON schema definitions for discriminated unions
    /// </summary>
    public class DiscriminatorAnalyzer
    {
        private readonly JObject definitions;
        private readonly Dictionary<string, int> refCounts;

        public Dictionary<string, DiscriminatorBaseInfo> BaseInfo { get; } = new Dictionary<string, DiscriminatorBaseInfo>();
        public Dictionary<string, DiscriminatorDerivedInfo> DerivedInfo { get; } = new Dictionary<string, DiscriminatorDerivedInfo>();
        public Dictionary<string, List<DiscriminatorVariant>> VariantClasses { get; } = new Dictionary<string, List<DiscriminatorVariant>>();
        public HashSet<string> AbstractBases { get; } = new HashSet<string>();
        public Dictionary<string, string> ChildToAbstractBase { get; } = new Dictionary<string, string>();

        public DiscriminatorAnalyzer(JObject definitions)
        {
            this.definitions = definitions;
            this.refCounts = GetDefinitionRefCounts(definitions);
            AnalyzeDiscriminators();
            AnalyzeDiscriminatorsInAnyOfVariants();
            AnalyzeAbstractBases();
        }

        private void AnalyzeDiscriminators()
        {
            foreach (var defProp in definitions.Properties().OrderBy(p => p.Name))
            {
                var defName = defProp.Name;
                var def = defProp.Value as JObject;
                if (def == null)
                    continue;

                // Check if this has a discriminator and oneOf
                var discriminator = def["discriminator"] as JObject;
                var oneOf = def["oneOf"] as JArray;

                if (discriminator == null || oneOf == null)
                    continue;

                var propertyName = discriminator["propertyName"]?.ToString();
                if (string.IsNullOrEmpty(propertyName))
                    continue;

                var baseClassName = NamingHelper.ConvertNameToClass(defName);
                var discInfo = NamingHelper.GetDiscriminatorPropertyInfo(baseClassName, propertyName);

                // Collect variants
                var variants = new List<(string RefName, string ConstValue, JObject Item)>();

                foreach (var item in oneOf)
                {
                    var itemObj = item as JObject;
                    if (itemObj == null)
                        continue;

                    string refName = null;

                    // Check for $ref
                    var itemRef = itemObj["$ref"]?.ToString();
                    if (!string.IsNullOrEmpty(itemRef))
                    {
                        refName = itemRef.Split('/').Last();
                    }
                    else
                    {
                        // Check allOf for $ref
                        var allOf = itemObj["allOf"] as JArray;
                        if (allOf != null)
                        {
                            foreach (var allOfItem in allOf)
                            {
                                var allOfRef = allOfItem["$ref"]?.ToString();
                                if (!string.IsNullOrEmpty(allOfRef))
                                {
                                    refName = allOfRef.Split('/').Last();
                                    break;
                                }
                            }
                        }
                    }

                    // Get const value
                    string constValue = null;
                    var properties = itemObj["properties"] as JObject;
                    if (properties != null && properties[propertyName] is JObject discProp)
                    {
                        constValue = discProp["const"]?.ToString();
                    }

                    if (constValue == null)
                        continue;

                    variants.Add((refName, constValue, itemObj));
                }

                if (variants.Count == 0)
                    continue;

                // Count refs in this base
                var refCountsInBase = new Dictionary<string, int>();
                foreach (var variant in variants)
                {
                    if (!string.IsNullOrEmpty(variant.RefName))
                    {
                        if (!refCountsInBase.ContainsKey(variant.RefName))
                        {
                            refCountsInBase[variant.RefName] = 0;
                        }
                        refCountsInBase[variant.RefName]++;
                    }
                }

                var mapping = new Dictionary<string, string>();

                foreach (var variant in variants)
                {
                    var refName = variant.RefName;
                    var constValue = variant.ConstValue ?? "";
                    var variantItem = variant.Item;

                    var globalRefCount = 0;
                    var localRefCount = 0;

                    if (!string.IsNullOrEmpty(refName))
                    {
                        if (refCounts.ContainsKey(refName))
                        {
                            globalRefCount = refCounts[refName];
                        }
                        if (refCountsInBase.ContainsKey(refName))
                        {
                            localRefCount = refCountsInBase[refName];
                        }
                    }

                    // Use direct inheritance if ref is only used once globally and once locally
                    var useDirectInheritance = !string.IsNullOrEmpty(refName) &&
                                               globalRefCount == 1 &&
                                               localRefCount == 1;

                    if (useDirectInheritance)
                    {
                        DerivedInfo[refName] = new DiscriminatorDerivedInfo
                        {
                            BaseName = defName,
                            PropertyName = propertyName,
                            PropertyCsName = discInfo.CsName,
                            PropertyJsonName = discInfo.JsonName,
                            DiscriminatorValue = constValue,
                            IsAbstract = false
                        };

                        mapping[constValue] = NamingHelper.ConvertNameToClass(refName);
                    }
                    else
                    {
                        // Create wrapper variant class
                        var variantClassName = NamingHelper.ConvertNameToClass(defName) +
                                             NamingHelper.ConvertPropertyName(constValue);

                        if (!VariantClasses.ContainsKey(defName))
                        {
                            VariantClasses[defName] = new List<DiscriminatorVariant>();
                        }

                        // Get variant definition
                        var variantDefinition = variantItem;
                        if (!string.IsNullOrEmpty(refName) && definitions[refName] is JObject referencedDef)
                        {
                            variantDefinition = referencedDef;
                        }

                        VariantClasses[defName].Add(new DiscriminatorVariant
                        {
                            ClassName = variantClassName,
                            BaseClassName = NamingHelper.ConvertNameToClass(defName),
                            DiscriminatorPropertyName = propertyName,
                            DiscriminatorPropertyCsName = discInfo.CsName,
                            DiscriminatorPropertyJsonName = discInfo.JsonName,
                            DiscriminatorValue = constValue,
                            Definition = variantDefinition,
                            Description = variantItem["description"]?.ToString()
                        });

                        mapping[constValue] = variantClassName;
                    }
                }

                BaseInfo[defName] = new DiscriminatorBaseInfo
                {
                    PropertyName = propertyName,
                    PropertyCsName = discInfo.CsName,
                    PropertyJsonName = discInfo.JsonName,
                    Mapping = mapping
                };
            }
        }

        private void AnalyzeDiscriminatorsInAnyOfVariants()
        {
            foreach (var defProp in definitions.Properties().OrderBy(p => p.Name))
            {
                var defName = defProp.Name;
                var def = defProp.Value as JObject;
                if (def == null)
                    continue;

                // Skip if already has explicit discriminator
                if (def["discriminator"] != null || def["oneOf"] != null)
                    continue;

                // Check for anyOf with discriminator in variants
                var anyOf = def["anyOf"] as JArray;
                if (anyOf == null || anyOf.Count == 0)
                    continue;

                // Only process anyOf that has complex types (allOf + $ref), not primitive unions
                // Primitive unions like ErrorCode (anyOf with const integer values) and 
                // RequestId (anyOf with null/integer/string types) should not be treated as discriminated unions
                bool hasComplexVariant = false;
                foreach (var item in anyOf)
                {
                    var itemObj = item as JObject;
                    if (itemObj != null)
                    {
                        var allOf = itemObj["allOf"] as JArray;
                        if (allOf != null)
                        {
                            foreach (var allOfItem in allOf)
                            {
                                var allOfRef = allOfItem["$ref"]?.ToString();
                                if (!string.IsNullOrEmpty(allOfRef))
                                {
                                    hasComplexVariant = true;
                                    break;
                                }
                            }
                            if (hasComplexVariant) break;
                        }
                    }
                }

                // Skip primitive union types
                if (!hasComplexVariant)
                    continue;

                // Try to detect discriminator property from first variant
                string discriminatorProperty = null;
                var variants = new List<(string RefName, string ConstValue, string Title, JObject Item)>();

                foreach (var item in anyOf)
                {
                    var itemObj = item as JObject;
                    if (itemObj == null)
                        continue;

                    string refName = null;
                    string constValue = null;
                    string title = itemObj["title"]?.ToString();

                    // Get $ref from allOf
                    var allOf = itemObj["allOf"] as JArray;
                    if (allOf != null)
                    {
                        foreach (var allOfItem in allOf)
                        {
                            var allOfRef = allOfItem["$ref"]?.ToString();
                            if (!string.IsNullOrEmpty(allOfRef))
                            {
                                refName = allOfRef.Split('/').Last();
                                break;
                            }
                        }
                    }

                    // Look for const value in properties
                    var properties = itemObj["properties"] as JObject;
                    if (properties != null)
                    {
                        foreach (var prop in properties.Properties())
                        {
                            var propObj = prop.Value as JObject;
                            if (propObj != null && propObj["const"] != null)
                            {
                                discriminatorProperty = prop.Name;
                                constValue = propObj["const"]?.ToString();
                                break;
                            }
                        }
                    }

                    // If no const value but we have a title, use title as the value
                    if (constValue == null && !string.IsNullOrEmpty(title))
                    {
                        if (discriminatorProperty == null)
                            discriminatorProperty = "type";
                        constValue = title;
                    }

                    if (!string.IsNullOrEmpty(constValue))
                    {
                        variants.Add((refName, constValue, title, itemObj));
                    }
                }

                // Need at least 2 variants with discriminator info
                if (variants.Count < 2 || discriminatorProperty == null)
                    continue;

                // Skip if already in BaseInfo
                if (BaseInfo.ContainsKey(defName))
                    continue;

                var baseClassName = NamingHelper.ConvertNameToClass(defName);
                var discInfo = NamingHelper.GetDiscriminatorPropertyInfo(baseClassName, discriminatorProperty);

                var mapping = new Dictionary<string, string>();
                var refCountsInBase = new Dictionary<string, int>();

                foreach (var variant in variants)
                {
                    if (!string.IsNullOrEmpty(variant.RefName))
                    {
                        if (!refCountsInBase.ContainsKey(variant.RefName))
                            refCountsInBase[variant.RefName] = 0;
                        refCountsInBase[variant.RefName]++;
                    }
                }

                foreach (var variant in variants)
                {
                    var refName = variant.RefName;
                    var constValue = variant.ConstValue ?? "";

                    var globalRefCount = 0;
                    var localRefCount = 0;

                    if (!string.IsNullOrEmpty(refName))
                    {
                        if (refCounts.ContainsKey(refName))
                            globalRefCount = refCounts[refName];
                        if (refCountsInBase.ContainsKey(refName))
                            localRefCount = refCountsInBase[refName];
                    }

                    // Use direct inheritance if ref is only used once globally and once locally
                    var useDirectInheritance = !string.IsNullOrEmpty(refName) &&
                                               globalRefCount == 1 &&
                                               localRefCount == 1;

                    if (useDirectInheritance)
                    {
                        DerivedInfo[refName] = new DiscriminatorDerivedInfo
                        {
                            BaseName = defName,
                            PropertyName = discriminatorProperty,
                            PropertyCsName = discInfo.CsName,
                            PropertyJsonName = discInfo.JsonName,
                            DiscriminatorValue = constValue,
                            IsAbstract = false
                        };

                        mapping[constValue] = NamingHelper.ConvertNameToClass(refName);
                    }
                    else
                    {
                        // Create wrapper variant class
                        var variantClassName = baseClassName + NamingHelper.ConvertPropertyName(constValue);

                        if (!VariantClasses.ContainsKey(defName))
                        {
                            VariantClasses[defName] = new List<DiscriminatorVariant>();
                        }

                        // Get variant definition
                        var variantDefinition = variant.Item;
                        if (!string.IsNullOrEmpty(refName) && definitions[refName] is JObject referencedDef)
                        {
                            variantDefinition = referencedDef;
                        }

                        VariantClasses[defName].Add(new DiscriminatorVariant
                        {
                            ClassName = variantClassName,
                            BaseClassName = baseClassName,
                            DiscriminatorPropertyName = discriminatorProperty,
                            DiscriminatorPropertyCsName = discInfo.CsName,
                            DiscriminatorPropertyJsonName = discInfo.JsonName,
                            DiscriminatorValue = constValue,
                            Definition = variantDefinition,
                            Description = variant.Item["description"]?.ToString()
                        });

                        mapping[constValue] = variantClassName;
                    }
                }

                BaseInfo[defName] = new DiscriminatorBaseInfo
                {
                    PropertyName = discriminatorProperty,
                    PropertyCsName = discInfo.CsName,
                    PropertyJsonName = discInfo.JsonName,
                    Mapping = mapping
                };
            }
        }

        private Dictionary<string, int> GetDefinitionRefCounts(JObject defs)
        {
            var counts = new Dictionary<string, int>();

            foreach (var defProp in defs.Properties())
            {
                var def = defProp.Value;
                CountRefs(def, counts);
            }

            return counts;
        }

        private void CountRefs(JToken node, Dictionary<string, int> counts)
        {
            if (node == null) return;

            if (node is JObject obj)
            {
                foreach (var prop in obj.Properties())
                {
                    if (prop.Name == "$ref" && prop.Value != null)
                    {
                        var refName = prop.Value.ToString().Split('/').Last();
                        if (!counts.ContainsKey(refName))
                        {
                            counts[refName] = 0;
                        }
                        counts[refName]++;
                    }
                    else
                    {
                        CountRefs(prop.Value, counts);
                    }
                }
            }
            else if (node is JArray arr)
            {
                foreach (var item in arr)
                {
                    CountRefs(item, counts);
                }
            }
        }

        private void AnalyzeAbstractBases()
        {
            foreach (var defProp in definitions.Properties().OrderBy(p => p.Name))
            {
                var defName = defProp.Name;
                var def = defProp.Value as JObject;
                if (def == null)
                    continue;

                // Skip if it already has a discriminator (handled by AnalyzeDiscriminators)
                if (def["discriminator"] != null)
                    continue;

                // Check for anyOf with allOf refs
                var anyOf = def["anyOf"] as JArray;
                if (anyOf == null)
                    continue;

                // Check if all items have allOf with $ref
                var childRefs = new List<string>();
                foreach (var item in anyOf)
                {
                    var itemObj = item as JObject;
                    if (itemObj == null)
                        continue;

                    var allOf = itemObj["allOf"] as JArray;
                    if (allOf == null)
                    {
                        childRefs.Clear();
                        break;
                    }

                    // Look for $ref in allOf
                    string refName = null;
                    foreach (var allOfItem in allOf)
                    {
                        var allOfRef = allOfItem["$ref"]?.ToString();
                        if (!string.IsNullOrEmpty(allOfRef))
                        {
                            refName = allOfRef.Split('/').Last();
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(refName))
                    {
                        childRefs.Clear();
                        break;
                    }

                    childRefs.Add(refName);
                }

                // If we found valid child refs, mark this as an abstract base
                if (childRefs.Count > 0)
                {
                    AbstractBases.Add(defName);
                    foreach (var childRef in childRefs)
                    {
                        ChildToAbstractBase[childRef] = defName;
                    }
                }
            }
        }
    }
}
