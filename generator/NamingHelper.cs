using System;
using System.Text;

namespace dotacp.generator
{
    /// <summary>
    /// Helper methods for converting naming conventions (snake_case to PascalCase, etc.)
    /// </summary>
    public static class NamingHelper
    {
        /// <summary>
        /// Convert snake_case property name to PascalCase
        /// Handles special case: _meta -> Meta
        /// Also handles multi-word titles with spaces
        /// </summary>
        public static string ConvertPropertyName(string name)
        {
            if (name == "_meta")
            {
                return "Meta";
            }

            var parts = name.Split('_');
            var sb = new StringBuilder();
            
            foreach (var part in parts)
            {
                if (part.Length > 0)
                {
                    // Keep existing casing for parts, just capitalize first letter
                    sb.Append(char.ToUpper(part[0]));
                    if (part.Length > 1)
                    {
                        sb.Append(part.Substring(1));
                    }
                }
            }

            var result = sb.ToString();

            // Handle multi-word titles with spaces (e.g., "Parse error" -> "ParseError")
            if (result.Contains(" "))
            {
                var words = result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                sb.Clear();
                foreach (var word in words)
                {
                    if (word.Length > 0)
                    {
                        sb.Append(char.ToUpper(word[0]));
                        if (word.Length > 1)
                        {
                            sb.Append(word.Substring(1).ToLower());
                        }
                    }
                }
                result = sb.ToString();
            }

            // Remove any remaining invalid characters
            result = System.Text.RegularExpressions.Regex.Replace(result, "[^a-zA-Z0-9_]", "");

            // Ensure it starts with a letter or underscore
            if (result.Length > 0 && char.IsDigit(result[0]))
            {
                result = "_" + result;
            }

            return result;
        }

        /// <summary>
        /// Convert name to class name (ensure PascalCase)
        /// </summary>
        public static string ConvertNameToClass(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            return char.ToUpper(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// Convert snake_case to PascalCase (used for Meta.cs generation)
        /// </summary>
        public static string ConvertToPascalCase(string name)
        {
            var parts = name.Split('_');
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                if (part.Length > 0)
                {
                    sb.Append(char.ToUpper(part[0]));
                    if (part.Length > 1)
                    {
                        sb.Append(part.Substring(1).ToLower());
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get property info for discriminator handling (handles naming conflicts)
        /// </summary>
        public static (string CsName, string JsonName) GetDiscriminatorPropertyInfo(string className, string propertyName)
        {
            var csPropName = ConvertPropertyName(propertyName);

            // Handle naming conflict: if property name matches class name, add suffix
            if (csPropName == className)
            {
                csPropName = $"{csPropName}Value";
            }

            return (csPropName, propertyName);
        }
    }
}
