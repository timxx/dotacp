using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotacp.generator
{
    /// <summary>
    /// Builds individual model class definitions from JSON schema
    /// </summary>
    public class ModelClassBuilder
    {
        private readonly string name;
        private JObject definition;
        private readonly JObject allDefinitions;
        private readonly PropertyTypeResolver typeResolver;
        private readonly DiscriminatorAnalyzer discriminatorAnalyzer;
        private readonly Dictionary<string, PropertyUnionInfo> propertyUnionInfo = new Dictionary<string, PropertyUnionInfo>(StringComparer.Ordinal);

        private sealed class PropertyUnionInfo
        {
            public PropertyUnionInfo(string typeName, List<string> unionTypes)
            {
                TypeName = typeName;
                UnionTypes = unionTypes;
            }

            public string TypeName { get; }
            public List<string> UnionTypes { get; }
        }

        public ModelClassBuilder(
            string name,
            JObject definition,
            JObject allDefinitions,
            PropertyTypeResolver typeResolver,
            DiscriminatorAnalyzer discriminatorAnalyzer)
        {
            this.name = name;
            this.definition = definition;
            this.allDefinitions = allDefinitions;
            this.typeResolver = typeResolver;
            this.discriminatorAnalyzer = discriminatorAnalyzer;
        }

        public string Generate()
        {
            // Check if this is a simple type alias
            if (IsSimpleTypeAlias())
            {
                var targetType = GetTypeAliasTarget();
                return GenerateTypeAliasStruct(targetType);
            }

            // Handle simple enum definitions
            var enumValue = definition["enum"] as JArray;
            if (enumValue != null)
            {
                return GenerateSimpleEnum();
            }

            // Handle discriminated unions with discriminator (at least one variant has const in JSON)
            if (discriminatorAnalyzer.BaseInfo.ContainsKey(name))
            {
                return GenerateDiscriminatorBaseClass();
            }

            // Handle unions without discriminator (no variant has const; like Python A | B)
            if (discriminatorAnalyzer.UnionWithoutDiscriminator.TryGetValue(name, out var unionVariantTypes))
            {
                return GenerateUnionWithoutDiscriminatorBaseClass(unionVariantTypes);
            }

            // Handle abstract base classes (anyOf with allOf refs)
            if (discriminatorAnalyzer.AbstractBases.Contains(name))
            {
                return GenerateAbstractBaseClass();
            }

            // Handle oneOf/anyOf at root level
            var oneOf = definition["oneOf"] as JArray;
            var anyOf = definition["anyOf"] as JArray;

            if (oneOf != null || anyOf != null)
            {
                var items = (oneOf ?? anyOf)!;

                // Open-ended string anyOf with known const values maps better to a string-backed alias.
                if (IsOpenStringEnumLikePattern(items))
                {
                    return GenerateOpenStringEnumLikeStruct(items);
                }

                // Check if this is an enum-like pattern
                if (IsEnumLikePattern(items, out var enumType))
                {
                    return GenerateEnumFromOneOf(items, enumType);
                }

                // Check if this is a union type
                if (IsUnionType(items, out var unionTypes, out var hasNullType))
                {
                    return GenerateUnionTypeStruct(unionTypes, hasNullType);
                }

                // Check if it's a discriminated union with properties
                if (HasProperties(items))
                {
                    // Merge properties from union items
                    definition = MergeUnionProperties(items);
                }
            }

            // Generate regular class
            return GenerateRegularClass();
        }

        private bool IsSimpleTypeAlias()
        {
            var typeToken = definition["type"];
            if (typeToken == null || typeToken.Type == JTokenType.Array)
            {
                return false;
            }

            // Must not have any complex schema properties
            var complexProperties = new[] { "properties", "allOf", "oneOf", "anyOf", "enum", "items", "additionalProperties", "required", "discriminator" };
            return !complexProperties.Any(prop => definition[prop] != null);
        }

        private string GetTypeAliasTarget()
        {
            var baseType = TypeMapper.GetTypeName(definition["type"]!.ToString());

            // Check for format hints on integer types
            if (definition["type"]!.ToString() == "integer" && definition["format"] != null)
            {
                var format = definition["format"]!.ToString();
                return format switch
                {
                    "uint16" => "ushort",
                    "uint32" => "uint",
                    "uint64" => "ulong",
                    "int16" => "short",
                    "int32" => "int",
                    "int64" => "long",
                    _ => "int"
                };
            }

            return baseType;
        }

        private string GenerateTypeAliasStruct(string underlyingType)
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            // Generate struct
            sb.AppendLineLf($"[JsonConverter(typeof(TypeAliasConverter<{className}, {underlyingType}>))]");
            sb.AppendLineLf($"public readonly struct {className} : IEquatable<{className}>");
            sb.AppendLineLf("{");
            sb.AppendLineLf($"    private readonly {underlyingType} _value;");
            sb.AppendLineLf();
            sb.AppendLineLf($"    public {className}({underlyingType} value)");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        _value = value;");
            sb.AppendLineLf("    }");
            sb.AppendLineLf();
            sb.AppendLineLf($"    public static implicit operator {className}({underlyingType} value) => new {className}(value);");
            sb.AppendLineLf($"    public static implicit operator {underlyingType}({className} alias) => alias._value;");
            sb.AppendLineLf();

            var isValueType = TypeMapper.IsValueType(underlyingType);

            if (isValueType)
            {
                sb.AppendLineLf($"    public bool Equals({className} other) => _value == other._value;");
                sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {className} other && Equals(other);");
                sb.AppendLineLf("    public override int GetHashCode() => _value.GetHashCode();");
                sb.AppendLineLf("    public override string ToString() => _value.ToString();");
            }
            else
            {
                sb.AppendLineLf($"    public bool Equals({className} other) => _value == other._value;");
                sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {className} other && Equals(other);");
                sb.AppendLineLf("    public override int GetHashCode() => _value?.GetHashCode() ?? 0;");
                sb.AppendLineLf("    public override string ToString() => _value?.ToString() ?? string.Empty;");
            }

            sb.Append("}");

            return sb.ToString();
        }

        private string GenerateSimpleEnum()
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            var enumArray = definition["enum"] as JArray;
            var typeToken = definition["type"];

            string enumType = null;
            if (typeToken != null)
            {
                if (typeToken.Type == JTokenType.Array)
                {
                    var typeArray = typeToken as JArray;
                    enumType = typeArray!.FirstOrDefault(t => t.ToString() != "null")?.ToString();
                }
                else
                {
                    enumType = typeToken.ToString();
                }
            }

            if (enumType == "integer")
            {
                // Integer enum
                var backingType = "int";
                var format = definition["format"]?.ToString();
                if (format != null)
                {
                    backingType = format switch
                    {
                        "int64" => "long",
                        "uint16" => "ushort",
                        "uint32" => "uint",
                        "uint64" => "ulong",
                        "int16" => "short",
                        "int32" => "int",
                        _ => "int"
                    };
                }

                sb.AppendLineLf($"public enum {className} : {backingType}");
                sb.AppendLineLf("{");

                var values = new List<string>();
                foreach (var item in enumArray!)
                {
                    var enumName = NamingHelper.ConvertPropertyName(item.ToString());
                    values.Add($"    {enumName} = {item}");
                }

                sb.Append(string.Join(",\n\n", values));
                sb.AppendLineLf();
                sb.Append("}");
            }
            else
            {
                // String enum
                sb.AppendLineLf($"[JsonConverter(typeof(JsonEnumMemberConverter<{className}>))]");
                sb.AppendLineLf($"public enum {className}");
                sb.AppendLineLf("{");

                var values = new List<string>();
                foreach (var item in enumArray!)
                {
                    var enumName = NamingHelper.ConvertPropertyName(item.ToString());
                    values.Add($"    [JsonEnumValue(\"{item}\")]\n    {enumName}");
                }

                sb.Append(string.Join(",\n\n", values));
                sb.AppendLineLf();
                sb.Append("}");
            }

            return sb.ToString();
        }

        private string GenerateDiscriminatorBaseClass()
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var baseInfo = discriminatorAnalyzer.BaseInfo[name];
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            sb.AppendLineLf($"[JsonConverter(typeof(DiscriminatorConverter<{className}>))]");
            sb.AppendLineLf($"public abstract class {className}");
            sb.AppendLineLf("{");

            // Add discriminator mapping
            sb.AppendLineLf($"    internal const string DiscriminatorPropertyName = \"{baseInfo.PropertyName}\";");
            sb.AppendLineLf("    internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)");
            sb.AppendLineLf("    {");

            var mappingLines = baseInfo.Mapping.OrderBy(kv => kv.Key).Select(kv =>
                $"        {{ \"{kv.Key}\", typeof({kv.Value}) }}");
            sb.Append(string.Join(",\n", mappingLines));
            sb.AppendLineLf();

            sb.AppendLineLf("    };");

            if (!string.IsNullOrEmpty(baseInfo.DefaultTypeWhenDiscriminatorMissing))
            {
                sb.AppendLineLf();
                sb.AppendLineLf($"    /// <summary>When the discriminator property is missing in JSON, deserialize as this type.</summary>");
                sb.AppendLineLf($"    internal static readonly Type DefaultTypeWhenDiscriminatorMissing = typeof({baseInfo.DefaultTypeWhenDiscriminatorMissing});");
            }

            sb.AppendLineLf();

            // Add discriminator property
            sb.AppendLineLf($"    [JsonProperty(\"{baseInfo.PropertyJsonName}\")]");
            sb.AppendLineLf($"    public abstract string {baseInfo.PropertyCsName} {{ get; }}");

            // Add other properties
            var properties = GetPropertyLines(definition, className, new[] { baseInfo.PropertyName });
            if (properties.Count > 0)
            {
                sb.AppendLineLf();
                sb.Append(string.Join("\n\n", properties));
                sb.AppendLineLf();
            }

            sb.Append("}");

            // Add variant classes
            if (discriminatorAnalyzer.VariantClasses.ContainsKey(name))
            {
                foreach (var variant in discriminatorAnalyzer.VariantClasses[name])
                {
                    sb.AppendLineLf();
                    sb.AppendLineLf();
                    sb.Append(GenerateVariantClass(variant));
                }
            }

            return sb.ToString();
        }

        private string GenerateUnionWithoutDiscriminatorBaseClass(List<string> variantClassNames)
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            AppendXmlDocs(sb, definition["description"]?.ToString());

            sb.AppendLineLf($"[JsonConverter(typeof(ObjectUnionConverter<{className}>))]");
            sb.AppendLineLf($"public abstract class {className}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>");
            sb.AppendLineLf("    internal static readonly Type[] UnionVariantTypes = new Type[]");
            sb.AppendLineLf("    {");
            foreach (var variantName in variantClassNames)
            {
                sb.AppendLineLf($"        typeof({variantName}),");
            }
            sb.AppendLineLf("    };");
            sb.AppendLineLf("}");

            return sb.ToString();
        }

        private string GenerateVariantClass(DiscriminatorVariant variant)
        {
            var sb = new StringBuilder();

            // XML documentation
            var description = variant.Description ?? variant.Definition["description"]?.ToString();
            AppendXmlDocs(sb, description);

            sb.AppendLineLf($"public class {variant.ClassName} : {variant.BaseClassName}");
            sb.AppendLineLf("{");

            // Add discriminator property override
            sb.AppendLineLf($"    [JsonProperty(\"{variant.DiscriminatorPropertyJsonName}\")]");
            sb.AppendLineLf($"    public override string {variant.DiscriminatorPropertyCsName} => \"{variant.DiscriminatorValue}\";");

            // Add other properties
            var properties = GetPropertyLines(variant.Definition, variant.ClassName, new[] { variant.DiscriminatorPropertyName });
            if (properties.Count > 0)
            {
                sb.AppendLineLf();
                sb.Append(string.Join("\n\n", properties));
                sb.AppendLineLf();
            }

            sb.Append("}");

            return sb.ToString();
        }

        private bool IsEnumLikePattern(JArray items, out string enumType)
        {
            enumType = "";

            var allHaveConstOrTitle = true;
            var allSameType = true;
            var allHaveConst = true;
            string firstType = null;

            foreach (var item in items)
            {
                var itemObj = item as JObject;
                if (itemObj == null) return false;

                var itemTypeToken = itemObj["type"];
                string itemType = null;

                if (itemTypeToken != null)
                {
                    if (itemTypeToken.Type == JTokenType.Array)
                    {
                        var typeArray = itemTypeToken as JArray;
                        itemType = typeArray!.FirstOrDefault(t => t.ToString() != "null")?.ToString();
                    }
                    else
                    {
                        itemType = itemTypeToken.ToString();
                    }
                }

                if (firstType == null)
                {
                    firstType = itemType;
                }
                else if (firstType != itemType)
                {
                    allSameType = false;
                    break;
                }

                if (itemObj["const"] == null && itemObj["title"] == null)
                {
                    allHaveConstOrTitle = false;
                    break;
                }

                if (itemObj["const"] == null)
                {
                    allHaveConst = false;
                }
            }

            if (!allHaveConstOrTitle || !allSameType)
            {
                return false;
            }

            if (firstType == "string" && allHaveConst)
            {
                enumType = firstType;
                return true;
            }

            if (firstType == "integer")
            {
                enumType = firstType;
                return true;
            }

            return false;
        }

        private bool IsOpenStringEnumLikePattern(JArray items)
        {
            var hasConst = false;
            var hasPlainStringFallback = false;
            var allowedFallbackKeys = new HashSet<string>(StringComparer.Ordinal)
            {
                "type",
                "title",
                "description"
            };

            foreach (var item in items)
            {
                var itemObj = item as JObject;
                if (itemObj == null)
                {
                    return false;
                }

                var itemTypeToken = itemObj["type"];
                string itemType = null;

                if (itemTypeToken != null)
                {
                    if (itemTypeToken.Type == JTokenType.Array)
                    {
                        var typeArray = itemTypeToken as JArray;
                        itemType = typeArray!.FirstOrDefault(t => t.ToString() != "null")?.ToString();
                    }
                    else
                    {
                        itemType = itemTypeToken.ToString();
                    }
                }

                if (itemType != "string")
                {
                    return false;
                }

                if (itemObj["const"] != null)
                {
                    hasConst = true;
                    continue;
                }

                if (itemObj.Properties().Any(property => !allowedFallbackKeys.Contains(property.Name)))
                {
                    return false;
                }

                hasPlainStringFallback = true;
            }

            return hasConst && hasPlainStringFallback;
        }

        private string GenerateOpenStringEnumLikeStruct(JArray items)
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            AppendXmlDocs(sb, definition["description"]?.ToString());

            sb.AppendLineLf($"[JsonConverter(typeof(TypeAliasConverter<{className}, string>))]");
            sb.AppendLineLf($"public readonly struct {className} : IEquatable<{className}>");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    private readonly string _value;");
            sb.AppendLineLf();
            sb.AppendLineLf($"    public {className}(string value)");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        _value = value;");
            sb.AppendLineLf("    }");
            sb.AppendLineLf();
            sb.AppendLineLf($"    public static implicit operator {className}(string value) => new {className}(value);");
            sb.AppendLineLf($"    public static implicit operator string({className} alias) => alias._value;");

            foreach (var item in items.OfType<JObject>().Where(item => item["const"] != null))
            {
                var constValue = item["const"]!.ToString();
                var memberName = !string.IsNullOrEmpty(item["title"]?.ToString())
                    ? NamingHelper.ConvertPropertyName(item["title"]!.ToString())
                    : NamingHelper.ConvertPropertyName(constValue);
                var description = item["description"]?.ToString();

                sb.AppendLineLf();
                if (!string.IsNullOrEmpty(description))
                {
                    AppendXmlDocs(sb, description, "    ");
                }

                sb.AppendLineLf($"    public static {className} {memberName} => new {className}(\"{constValue}\");");
            }

            sb.AppendLineLf();
            sb.AppendLineLf($"    public bool Equals({className} other) => _value == other._value;");
            sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {className} other && Equals(other);");
            sb.AppendLineLf("    public override int GetHashCode() => _value?.GetHashCode() ?? 0;");
            sb.AppendLineLf("    public override string ToString() => _value?.ToString() ?? string.Empty;");
            sb.Append("}");

            return sb.ToString();
        }

        private string GenerateEnumFromOneOf(JArray items, string enumType)
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            if (enumType == "integer")
            {
                // Integer enum
                var backingType = "int";
                var firstItem = items[0] as JObject;
                var format = firstItem?["format"]?.ToString();
                if (format != null)
                {
                    backingType = format switch
                    {
                        "int64" => "long",
                        "uint16" => "ushort",
                        "uint32" => "uint",
                        "uint64" => "ulong",
                        "int16" => "short",
                        "int32" => "int",
                        _ => "int"
                    };
                }

                sb.AppendLineLf($"public enum {className} : {backingType}");
                sb.AppendLineLf("{");

                var enumValues = new List<string>();
                foreach (var item in items)
                {
                    var itemObj = item as JObject;
                    if (itemObj == null)
                        continue;

                    var constValue = itemObj["const"];
                    var title = itemObj["title"]?.ToString();
                    var description = itemObj["description"]?.ToString();

                    string enumName;
                    if (!string.IsNullOrEmpty(title))
                    {
                        enumName = NamingHelper.ConvertPropertyName(title);
                    }
                    else if (constValue != null)
                    {
                        enumName = NamingHelper.ConvertPropertyName(constValue.ToString());
                    }
                    else
                    {
                        continue;
                    }

                    var enumEntry = new StringBuilder();
                    if (!string.IsNullOrEmpty(description))
                    {
                        AppendXmlDocs(enumEntry, description, "    ");
                    }

                    if (constValue != null)
                    {
                        enumEntry.Append($"    {enumName} = {constValue}");
                        enumValues.Add(enumEntry.ToString());
                    }
                }

                sb.Append(string.Join(",\n\n", enumValues));
                sb.AppendLineLf();
                sb.Append("}");
            }
            else
            {
                // String enum
                sb.AppendLineLf($"[JsonConverter(typeof(JsonEnumMemberConverter<{className}>))]");
                sb.AppendLineLf($"public enum {className}");
                sb.AppendLineLf("{");

                var enumValues = new List<string>();
                foreach (var item in items)
                {
                    var itemObj = item as JObject;
                    if (itemObj == null)
                        continue;

                    var constValue = itemObj["const"]?.ToString();
                    var title = itemObj["title"]?.ToString();
                    var description = itemObj["description"]?.ToString();

                    string enumName;
                    if (!string.IsNullOrEmpty(title))
                    {
                        enumName = NamingHelper.ConvertPropertyName(title);
                    }
                    else if (!string.IsNullOrEmpty(constValue))
                    {
                        enumName = NamingHelper.ConvertPropertyName(constValue);
                    }
                    else
                    {
                        continue;
                    }

                    var enumEntry = new StringBuilder();
                    if (!string.IsNullOrEmpty(description))
                    {
                        AppendXmlDocs(enumEntry, description, "    ");
                    }

                    var actualValue = constValue ?? title;
                    enumEntry.Append($"    [JsonEnumValue(\"{actualValue}\")]\n    {enumName}");

                    enumValues.Add(enumEntry.ToString());
                }

                sb.Append(string.Join(",\n\n", enumValues));
                sb.AppendLineLf();
                sb.Append("}");
            }

            return sb.ToString();
        }

        private string GenerateAbstractBaseClass()
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            // Find all child types that inherit from this abstract base
            var childClassNames = discriminatorAnalyzer.ChildToAbstractBase
                .Where(kv => kv.Value == name)
                .Select(kv => NamingHelper.ConvertNameToClass(kv.Key))
                .ToList();

            // Emit ObjectUnionConverter so the abstract base can be deserialized from any variant
            if (childClassNames.Count > 0)
            {
                sb.AppendLineLf($"[JsonConverter(typeof(ObjectUnionConverter<{className}>))]");
            }

            // Generate abstract base class
            sb.AppendLineLf($"public abstract class {className}");
            sb.AppendLineLf("{");

            if (childClassNames.Count > 0)
            {
                sb.AppendLineLf("    /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>");
                sb.AppendLineLf("    internal static readonly Type[] UnionVariantTypes = new Type[]");
                sb.AppendLineLf("    {");
                foreach (var child in childClassNames)
                {
                    sb.AppendLineLf($"        typeof({child}),");
                }
                sb.AppendLineLf("    };");
            }

            sb.Append("}");

            return sb.ToString();
        }

        private bool IsUnionType(JArray items, out List<string> unionTypes, out bool hasNullType)
        {
            unionTypes = new List<string>();
            hasNullType = false;
            var hasProperties = false;

            foreach (var item in items)
            {
                var itemObj = item as JObject;
                if (itemObj == null)
                    continue;

                if (itemObj["properties"] != null)
                {
                    hasProperties = true;
                    break;
                }

                var itemRef = itemObj["$ref"]?.ToString();
                if (!string.IsNullOrEmpty(itemRef))
                {
                    var refName = itemRef.Split('/').Last();
                    unionTypes.Add(NamingHelper.ConvertNameToClass(refName));
                    continue;
                }

                var allOf = itemObj["allOf"] as JArray;
                if (allOf != null)
                {
                    var allOfRef = allOf
                        .OfType<JObject>()
                        .Select(o => o["$ref"]?.ToString())
                        .FirstOrDefault(r => !string.IsNullOrEmpty(r));

                    if (!string.IsNullOrEmpty(allOfRef))
                    {
                        var refName = allOfRef.Split('/').Last();
                        unionTypes.Add(NamingHelper.ConvertNameToClass(refName));
                        continue;
                    }
                }

                var itemTypeToken = itemObj["type"];
                if (itemTypeToken != null)
                {
                    if (itemTypeToken.Type == JTokenType.Array)
                    {
                        var typeArray = itemTypeToken as JArray;
                        if (typeArray!.Any(t => t.ToString() == "null"))
                        {
                            hasNullType = true;
                        }

                        var nonNullTypes = typeArray.Where(t => t.ToString() != "null").ToList();
                        if (nonNullTypes.Count > 0)
                        {
                            var csType = GetCSharpTypeForJsonType(nonNullTypes[0].ToString(), itemObj);
                            if (!string.IsNullOrEmpty(csType))
                            {
                                unionTypes.Add(csType);
                            }
                        }
                    }
                    else if (itemTypeToken.ToString() == "null")
                    {
                        hasNullType = true;
                    }
                    else
                    {
                        var csType = GetCSharpTypeForJsonType(itemTypeToken.ToString(), itemObj);
                        if (!string.IsNullOrEmpty(csType))
                        {
                            unionTypes.Add(csType);
                        }
                    }
                }
            }

            // Don't deduplicate yet - check count before deduplication
            // Deduplication happens in GenerateUnionTypeStruct
            return unionTypes.Count > 1 && !hasProperties;
        }

        private string GetCSharpTypeForJsonType(string jsonType, JObject item)
        {
            if (jsonType == "array")
            {
                return typeResolver.GetPropertyType(item);
            }

            if (jsonType == "integer" && item["format"] != null)
            {
                var format = item["format"]!.ToString();
                return format switch
                {
                    "uint16" => "ushort",
                    "uint32" => "uint",
                    "uint64" => "ulong",
                    "int16" => "short",
                    "int32" => "int",
                    "int64" => "long",
                    _ => "int"
                };
            }

            return TypeMapper.GetTypeName(jsonType);
        }

        private string GenerateUnionTypeStruct(List<string> unionTypes, bool hasNullType)
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // Remove duplicate types (keep unique types only)
            var uniqueUnionTypes = unionTypes.Distinct().ToList();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            sb.AppendLineLf($"[JsonConverter(typeof(UnionTypeConverter<{className}>))]");
            sb.AppendLineLf($"public readonly struct {className} : IEquatable<{className}>");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    private readonly object _value;");
            sb.AppendLineLf("    private readonly int _typeIndex;");

            if (hasNullType)
            {
                sb.AppendLineLf("    private readonly bool _isNull;");
            }

            sb.AppendLineLf();

            // Generate constructors for each unique type
            for (int i = 0; i < uniqueUnionTypes.Count; i++)
            {
                var unionType = uniqueUnionTypes[i];
                sb.AppendLineLf($"    public {className}({unionType} value)");
                sb.AppendLineLf("    {");
                sb.AppendLineLf("        _value = value;");
                sb.AppendLineLf($"        _typeIndex = {i};");
                if (hasNullType)
                {
                    sb.AppendLineLf("        _isNull = false;");
                }
                sb.AppendLineLf("    }");
                sb.AppendLineLf();
            }

            // Add null constructor if needed
            if (hasNullType)
            {
                sb.AppendLineLf($"    private {className}(bool isNull)");
                sb.AppendLineLf("    {");
                sb.AppendLineLf("        _value = null;");
                sb.AppendLineLf("        _typeIndex = -1;");
                sb.AppendLineLf("        _isNull = isNull;");
                sb.AppendLineLf("    }");
                sb.AppendLineLf();
                sb.AppendLineLf($"    public static {className} Null => new {className}(true);");
                sb.AppendLineLf();
            }

            // Generate implicit conversions
            foreach (var unionType in uniqueUnionTypes)
            {
                sb.AppendLineLf($"    public static implicit operator {className}({unionType} value) => new {className}(value);");
            }
            sb.AppendLineLf();

            // Add null check property if needed
            if (hasNullType)
            {
                sb.AppendLineLf("    public bool IsNull => _isNull;");
                sb.AppendLineLf();
            }

            // Generate TryGet methods
            foreach (var unionType in uniqueUnionTypes)
            {
                var cleanTypeName = unionType.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                var methodName = "TryGet" + char.ToUpper(cleanTypeName[0]) + cleanTypeName.Substring(1);

                sb.AppendLineLf($"    public bool {methodName}(out {unionType} value)");
                sb.AppendLineLf("    {");
                if (hasNullType)
                {
                    sb.AppendLineLf("        if (_isNull)");
                    sb.AppendLineLf("        {");
                    sb.AppendLineLf("            value = default;");
                    sb.AppendLineLf("            return false;");
                    sb.AppendLineLf("        }");
                }
                sb.AppendLineLf($"        if (_value is {unionType} v)");
                sb.AppendLineLf("        {");
                sb.AppendLineLf("            value = v;");
                sb.AppendLineLf("            return true;");
                sb.AppendLineLf("        }");
                sb.AppendLineLf("        value = default;");
                sb.AppendLineLf("        return false;");
                sb.AppendLineLf("    }");
                sb.AppendLineLf();
            }

            // Generate Equals, GetHashCode, ToString
            if (hasNullType)
            {
                sb.AppendLineLf($"    public bool Equals({className} other) => _isNull == other._isNull && (_isNull || (Equals(_value, other._value) && _typeIndex == other._typeIndex));");
                sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {className} other && Equals(other);");
                sb.AppendLineLf("    public override int GetHashCode()");
                sb.AppendLineLf("    {");
                sb.AppendLineLf("        if (_isNull) return 0;");
                sb.AppendLineLf("        unchecked");
                sb.AppendLineLf("        {");
                sb.AppendLineLf("            int hash = 17;");
                sb.AppendLineLf("            hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);");
                sb.AppendLineLf("            hash = hash * 31 + _typeIndex;");
                sb.AppendLineLf("            return hash;");
                sb.AppendLineLf("        }");
                sb.AppendLineLf("    }");
                sb.AppendLineLf("    public override string ToString() => _isNull ? string.Empty : (_value?.ToString() ?? string.Empty);");
            }
            else
            {
                sb.AppendLineLf($"    public bool Equals({className} other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;");
                sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {className} other && Equals(other);");
                sb.AppendLineLf("    public override int GetHashCode()");
                sb.AppendLineLf("    {");
                sb.AppendLineLf("        unchecked");
                sb.AppendLineLf("        {");
                sb.AppendLineLf("            int hash = 17;");
                sb.AppendLineLf("            hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);");
                sb.AppendLineLf("            hash = hash * 31 + _typeIndex;");
                sb.AppendLineLf("            return hash;");
                sb.AppendLineLf("        }");
                sb.AppendLineLf("    }");
                sb.AppendLineLf("    public override string ToString() => _value?.ToString() ?? string.Empty;");
            }

            sb.Append("}");

            return sb.ToString();
        }

        private bool HasProperties(JArray items)
        {
            return items.Any(item => (item as JObject)?["properties"] != null);
        }

        private JObject MergeUnionProperties(JArray items)
        {
            var mergedProperties = new JObject();
            var variantPropertyDefinitions = new Dictionary<string, List<JObject>>(StringComparer.Ordinal);

            var baseProperties = definition["properties"] as JObject;
            if (baseProperties != null)
            {
                foreach (var prop in baseProperties.Properties())
                {
                    mergedProperties[prop.Name] = prop.Value;
                }
            }

            foreach (var item in items)
            {
                var itemObj = item as JObject;
                if (itemObj == null)
                    continue;

                var properties = itemObj["properties"] as JObject;
                if (properties != null)
                {
                    foreach (var prop in properties.Properties())
                    {
                        if (prop.Value is JObject propObj)
                        {
                            if (!variantPropertyDefinitions.TryGetValue(prop.Name, out var defs))
                            {
                                defs = new List<JObject>();
                                variantPropertyDefinitions[prop.Name] = defs;
                            }
                            defs.Add(propObj);
                        }

                        if (mergedProperties[prop.Name] == null)
                        {
                            mergedProperties[prop.Name] = prop.Value;
                        }
                    }
                }
            }

            CollectPropertyUnionInfo(variantPropertyDefinitions);

            if (mergedProperties.Count > 0)
            {
                var newDef = new JObject(definition);
                newDef["properties"] = mergedProperties;
                MergeRequiredProperties(newDef, items);
                return newDef;
            }

            return definition;
        }

        private void CollectPropertyUnionInfo(Dictionary<string, List<JObject>> variantPropertyDefinitions)
        {
            foreach (var entry in variantPropertyDefinitions)
            {
                if (entry.Value.Count < 2)
                {
                    continue;
                }

                var unionTypes = new List<string>();
                foreach (var propDef in entry.Value)
                {
                    var csType = typeResolver.GetPropertyType(propDef, entry.Key);
                    if (!string.IsNullOrEmpty(csType))
                    {
                        unionTypes.Add(csType);
                    }
                }

                var distinctTypes = unionTypes.Distinct().ToList();
                if (distinctTypes.Count < 2 || distinctTypes.Contains("object"))
                {
                    continue;
                }

                var unionTypeName = NamingHelper.ConvertNameToClass(name) + NamingHelper.ConvertPropertyName(entry.Key);
                propertyUnionInfo[entry.Key] = new PropertyUnionInfo(unionTypeName, distinctTypes);
            }
        }

        private void MergeRequiredProperties(JObject newDef, JArray items)
        {
            var baseRequired = definition["required"] as JArray;
            var requiredList = new List<string>();
            var requiredSet = new HashSet<string>(StringComparer.Ordinal);

            if (baseRequired != null)
            {
                foreach (var item in baseRequired)
                {
                    var value = item.ToString();
                    if (requiredSet.Add(value))
                    {
                        requiredList.Add(value);
                    }
                }
            }

            var variantRequiredSets = new List<HashSet<string>>();
            foreach (var item in items)
            {
                var itemObj = item as JObject;
                if (itemObj == null)
                {
                    continue;
                }

                var required = itemObj["required"] as JArray;
                if (required == null)
                {
                    variantRequiredSets.Add(new HashSet<string>(StringComparer.Ordinal));
                    continue;
                }

                var set = new HashSet<string>(StringComparer.Ordinal);
                foreach (var req in required)
                {
                    set.Add(req.ToString());
                }
                variantRequiredSets.Add(set);
            }

            if (variantRequiredSets.Count > 0)
            {
                var intersection = new HashSet<string>(variantRequiredSets[0], StringComparer.Ordinal);
                for (var i = 1; i < variantRequiredSets.Count; i++)
                {
                    intersection.IntersectWith(variantRequiredSets[i]);
                }

                if (intersection.Count > 0)
                {
                    var firstRequired = (items[0] as JObject)?["required"] as JArray;
                    if (firstRequired != null)
                    {
                        foreach (var req in firstRequired)
                        {
                            var value = req.ToString();
                            if (intersection.Contains(value) && requiredSet.Add(value))
                            {
                                requiredList.Add(value);
                            }
                        }
                    }
                }
            }

            if (requiredList.Count > 0)
            {
                newDef["required"] = new JArray(requiredList);
            }
        }

        private string GenerateRegularClass()
        {
            var className = NamingHelper.ConvertNameToClass(name);
            var sb = new StringBuilder();

            // XML documentation
            AppendXmlDocs(sb, definition["description"]?.ToString());

            if (propertyUnionInfo.Count > 0)
            {
                foreach (var unionInfo in propertyUnionInfo.Values.OrderBy(info => info.TypeName))
                {
                    sb.Append(GeneratePropertyUnionTypeStruct(unionInfo.TypeName, unionInfo.UnionTypes));
                    sb.AppendLineLf();
                    sb.AppendLineLf();
                }
            }

            // Determine class type
            var classDeclaration = $"public class {className}";

            // Check if this inherits from an abstract base
            string baseNameForUnion = null;
            if (discriminatorAnalyzer.ChildToAbstractBase.ContainsKey(name))
            {
                var baseName = discriminatorAnalyzer.ChildToAbstractBase[name];
                var baseClassName = NamingHelper.ConvertNameToClass(baseName);
                classDeclaration = $"public class {className} : {baseClassName}";
                if (discriminatorAnalyzer.UnionWithoutDiscriminator.ContainsKey(baseName))
                    baseNameForUnion = baseName;
            }
            else if (discriminatorAnalyzer.DerivedInfo.ContainsKey(name))
            {
                var derivedInfo = discriminatorAnalyzer.DerivedInfo[name];
                var baseClassName = NamingHelper.ConvertNameToClass(derivedInfo.BaseName);

                if (derivedInfo.IsAbstract)
                {
                    classDeclaration = $"public abstract class {className} : {baseClassName}";
                }
                else
                {
                    classDeclaration = $"public class {className} : {baseClassName}";
                }
            }

            var properties = new List<string>();

            // For union-without-discriminator variants, add required JSON keys so the converter can try the right type first
            if (baseNameForUnion != null)
            {
                var required = definition["required"] as JArray;
                if (required != null && required.Count > 0)
                {
                    var jsonKeys = required.Select(r => "\"" + r.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"").ToList();
                    properties.Add("    /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>");
                    properties.Add("    internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { " + string.Join(", ", jsonKeys) + " };");
                }
            }

            // Add discriminator override if needed
            if (discriminatorAnalyzer.DerivedInfo.ContainsKey(name) &&
                !discriminatorAnalyzer.DerivedInfo[name].IsAbstract)
            {
                var derivedInfo = discriminatorAnalyzer.DerivedInfo[name];
                properties.Add($"    [JsonProperty(\"{derivedInfo.PropertyJsonName}\")]\n    public override string {derivedInfo.PropertyCsName} => \"{derivedInfo.DiscriminatorValue}\";");
            }

            properties.AddRange(GetPropertyLines(definition, className, Array.Empty<string>()));

            if (properties.Count > 0)
            {
                sb.AppendLineLf(classDeclaration);
                sb.AppendLineLf("{");
                sb.Append(string.Join("\n\n", properties));
                sb.AppendLineLf();
                sb.Append("}");
            }
            else
            {
                sb.AppendLineLf(classDeclaration);
                sb.AppendLineLf("{");
                sb.Append("}");
            }

            return sb.ToString();
        }

        private List<string> GetPropertyLines(JObject definition, string className, string[] skipProperties)
        {
            var properties = new List<string>();
            var props = definition["properties"] as JObject;

            if (props == null || !props.Properties().Any())
            {
                return properties;
            }

            var required = definition["required"] as JArray;
            var requiredProps = new HashSet<string>();
            if (required != null)
            {
                foreach (var r in required)
                {
                    requiredProps.Add(r.ToString());
                }
            }

            foreach (var propName in props.Properties().OrderBy(p => p.Name).Select(p => p.Name))
            {
                if (skipProperties.Contains(propName))
                {
                    continue;
                }

                var prop = props[propName] as JObject;
                if (prop == null)
                    continue;

                var csType = propertyUnionInfo.TryGetValue(propName, out var unionInfo)
                    ? unionInfo.TypeName
                    : typeResolver.GetPropertyType(prop, propName);
                var csPropName = NamingHelper.ConvertPropertyName(propName);
                var propIsRequired = requiredProps.Contains(propName);
                var needsJsonPropertyName = false;

                // Handle naming conflicts
                if (csPropName == className)
                {
                    csPropName = $"{csPropName}Value";
                    needsJsonPropertyName = true;
                }

                // If not required and not nullable, make it nullable (only for value types)
                if (!propIsRequired && !csType.EndsWith("?") && prop["default"] == null)
                {
                    if (TypeMapper.IsValueType(csType))
                    {
                        csType = $"{csType}?";
                    }
                }

                var propLine = new StringBuilder();

                // Add XML documentation
                var propDescription = prop["description"]?.ToString();
                if (!string.IsNullOrEmpty(propDescription))
                {
                    AppendXmlDocs(propLine, propDescription, "    ");
                }

                // Add JsonPropertyName attribute if needed
                if (!needsJsonPropertyName && propName != NamingHelper.ConvertPropertyName(propName))
                {
                    needsJsonPropertyName = true;
                }

                if (needsJsonPropertyName)
                {
                    propLine.AppendLineLf($"    [JsonProperty(\"{propName}\")]");
                }

                // Build property declaration
                var propDeclaration = $"    public {csType} {csPropName} {{ get; set; }}";

                if (propIsRequired && !csType.EndsWith("?"))
                {
                    var isUnionValueType = propertyUnionInfo.ContainsKey(propName);
                    var isReferenceType = !isUnionValueType && TypeMapper.IsReferenceType(csType);

                    if (isReferenceType)
                    {
                        propDeclaration += " = null!;";
                    }
                }

                propLine.Append(propDeclaration);

                // Add default if specified
                var defaultValue = prop["default"];
                if (defaultValue != null)
                {
                    var defaultCSharp = TypeMapper.ConvertDefaultValue(defaultValue, csType);
                    if (!string.IsNullOrEmpty(defaultCSharp))
                    {
                        propLine.Append($" = {defaultCSharp};");
                    }
                }

                properties.Add(propLine.ToString());
            }

            return properties;
        }

        private string GeneratePropertyUnionTypeStruct(string unionTypeName, List<string> unionTypes)
        {
            var sb = new StringBuilder();

            sb.AppendLineLf($"[JsonConverter(typeof(UnionTypeConverter<{unionTypeName}>))]");
            sb.AppendLineLf($"public readonly struct {unionTypeName} : IEquatable<{unionTypeName}>");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    private readonly object _value;");
            sb.AppendLineLf("    private readonly int _typeIndex;");
            sb.AppendLineLf();

            for (int i = 0; i < unionTypes.Count; i++)
            {
                var unionType = unionTypes[i];
                sb.AppendLineLf($"    public {unionTypeName}({unionType} value)");
                sb.AppendLineLf("    {");
                sb.AppendLineLf("        _value = value;");
                sb.AppendLineLf($"        _typeIndex = {i};");
                sb.AppendLineLf("    }");
                sb.AppendLineLf();
            }

            foreach (var unionType in unionTypes)
            {
                sb.AppendLineLf($"    public static implicit operator {unionTypeName}({unionType} value) => new {unionTypeName}(value);");
            }
            sb.AppendLineLf();

            foreach (var unionType in unionTypes)
            {
                var cleanTypeName = unionType.Replace("<", "").Replace(">", "").Replace(",", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                var methodName = "TryGet" + char.ToUpper(cleanTypeName[0]) + cleanTypeName.Substring(1);

                sb.AppendLineLf($"    public bool {methodName}(out {unionType} value)");
                sb.AppendLineLf("    {");
                sb.AppendLineLf($"        if (_value is {unionType} v)");
                sb.AppendLineLf("        {");
                sb.AppendLineLf("            value = v;");
                sb.AppendLineLf("            return true;");
                sb.AppendLineLf("        }");
                sb.AppendLineLf("        value = default;");
                sb.AppendLineLf("        return false;");
                sb.AppendLineLf("    }");
                sb.AppendLineLf();
            }

            sb.AppendLineLf($"    public bool Equals({unionTypeName} other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;");
            sb.AppendLineLf($"    public override bool Equals(object obj) => obj is {unionTypeName} other && Equals(other);");
            sb.AppendLineLf("    public override int GetHashCode()");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        unchecked");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            int hash = 17;");
            sb.AppendLineLf("            hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);");
            sb.AppendLineLf("            hash = hash * 31 + _typeIndex;");
            sb.AppendLineLf("            return hash;");
            sb.AppendLineLf("        }");
            sb.AppendLineLf("    }");
            sb.AppendLineLf("    public override string ToString() => _value?.ToString() ?? string.Empty;");

            sb.Append("}");

            return sb.ToString();
        }

        private void AppendXmlDocs(StringBuilder sb, string description, string indent = "")
        {
            if (string.IsNullOrEmpty(description)) return;

            sb.AppendLineLf($"{indent}/// <summary>");

            var descLines = description.Replace("\r\n", "\n").Split('\n');
            foreach (var descLine in descLines)
            {
                var trimmed = descLine.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    sb.AppendLineLf($"{indent}/// {trimmed}");
                }
                else
                {
                    sb.AppendLineLf($"{indent}///");
                }
            }

            sb.AppendLineLf($"{indent}/// </summary>");
        }
    }
}
