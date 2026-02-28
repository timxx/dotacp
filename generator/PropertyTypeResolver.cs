using System.Linq;
using Newtonsoft.Json.Linq;

namespace dotacp.generator
{
    /// <summary>
    /// Resolves C# property types from JSON schema definitions
    /// </summary>
    public class PropertyTypeResolver
    {
        private readonly JObject allDefinitions;

        public PropertyTypeResolver(JObject allDefinitions)
        {
            this.allDefinitions = allDefinitions;
        }

        /// <summary>
        /// Get C# type for a JSON schema property
        /// </summary>
        public string GetPropertyType(JObject property)
        {
            // Handle $ref first
            var refValue = property["$ref"]?.ToString();
            if (!string.IsNullOrEmpty(refValue))
            {
                var refName = refValue.Split('/').Last();
                return NamingHelper.ConvertNameToClass(refName);
            }

            // Handle allOf with references
            var allOf = property["allOf"] as JArray;
            if (allOf != null)
            {
                foreach (var item in allOf)
                {
                    var itemRef = item["$ref"]?.ToString();
                    if (!string.IsNullOrEmpty(itemRef))
                    {
                        var refName = itemRef.Split('/').Last();
                        return NamingHelper.ConvertNameToClass(refName);
                    }
                }
            }

            // Handle type array (e.g., ["string", "null"], ["integer", "null"], ["array", "null"])
            var typeToken = property["type"];
            var isNullable = false;
            string typeString = null;

            if (typeToken != null)
            {
                if (typeToken.Type == JTokenType.Array)
                {
                    var typeArray = typeToken as JArray;
                    isNullable = typeArray!.Any(t => t.ToString() == "null");
                    var nonNullTypes = typeArray.Where(t => t.ToString() != "null").ToList();
                    
                    // Handle array type in type array (e.g., ["array", "null"])
                    if (nonNullTypes.Count > 0 && nonNullTypes[0].ToString() == "array")
                    {
                        var items = property["items"] as JObject;
                        var itemType = items != null ? GetPropertyType(items) : "object";
                        var result = $"{itemType}[]";
                        // For nullable array, don't add ? since arrays are reference types
                        return result;
                    }
                    
                    if (nonNullTypes.Count == 1)
                    {
                        typeString = nonNullTypes[0].ToString();
                    }
                }
                else if (typeToken.Type == JTokenType.String && typeToken.ToString() == "array")
                {
                    // Handle simple array type
                    var items = property["items"] as JObject;
                    var itemType = items != null ? GetPropertyType(items) : "object";
                    return $"{itemType}[]";
                }
                else if (typeToken.Type == JTokenType.String)
                {
                    typeString = typeToken.ToString();
                }
            }

            // Handle enum
            if (property["enum"] != null)
            {
                return "string";
            }

            // Handle typed values
            if (!string.IsNullOrEmpty(typeString))
            {
                string mappedType;

                // Check for format hints on integer types
                if (typeString == "integer" && property["format"] != null)
                {
                    var format = property["format"].ToString();
                    mappedType = format switch
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
                else
                {
                    mappedType = TypeMapper.GetTypeName(typeString);
                }

                if (isNullable && typeString == "object")
                {
                    return "object";
                }

                // Add ? for nullable value types only
                if (isNullable && TypeMapper.IsValueType(mappedType))
                {
                    return mappedType + "?";
                }

                return mappedType;
            }

            // Handle anyOf (union types)
            var anyOf = property["anyOf"] as JArray;
            if (anyOf != null)
            {
                // Check if this is just a nullable reference pattern: anyOf: [{ $ref }, { type: null }]
                if (anyOf.Count == 2)
                {
                    var hasRef = false;
                    var hasNull = false;
                    string refType = null;

                    foreach (var item in anyOf)
                    {
                        var itemObj = item as JObject;
                        if (itemObj == null)
                            continue;

                        var itemRef = itemObj["$ref"]?.ToString();
                        if (!string.IsNullOrEmpty(itemRef))
                        {
                            hasRef = true;
                            var refName = itemRef.Split('/').Last();
                            refType = NamingHelper.ConvertNameToClass(refName);
                        }
                        else if (itemObj["type"]?.ToString() == "null")
                        {
                            hasNull = true;
                        }
                    }

                    // If it's a ref + null pattern, return the nullable reference type
                    if (hasRef && hasNull && !string.IsNullOrEmpty(refType))
                    {
                        return refType;
                    }
                }

                // Otherwise, return object for complex union types
                return "object";
            }

            // Handle oneOf (discriminated unions) - return object without nullable annotation
            if (property["oneOf"] != null)
            {
                return "object";
            }

            // Default to object (no nullable annotation)
            return "object";
        }
    }
}
