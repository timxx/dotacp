using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace dotacp.generator
{
    /// <summary>
    /// Generates Schema.cs from schema.json
    /// </summary>
    public class SchemaGenerator
    {
        private JObject definitions = new JObject();
        private PropertyTypeResolver typeResolver;
        private DiscriminatorAnalyzer discriminatorAnalyzer;

        /// <summary>
        /// Generate Schema.cs from schema.json
        /// </summary>
        public string Generate(string schemaJsonPath, string versionFilePath)
        {
            var schemaJson = File.ReadAllText(schemaJsonPath);
            var schema = JObject.Parse(schemaJson);

            definitions = schema["$defs"] as JObject ?? new JObject();
            typeResolver = new PropertyTypeResolver(definitions);
            discriminatorAnalyzer = new DiscriminatorAnalyzer(definitions);

            var sb = new StringBuilder();

            // Generate header
            sb.AppendLineLf("// Generated from schema/schema.json. Do not edit by hand.");

            if (File.Exists(versionFilePath))
            {
                var gitRef = File.ReadAllText(versionFilePath).Trim();
                if (!string.IsNullOrEmpty(gitRef))
                {
                    sb.AppendLineLf($"// Schema ref: {gitRef}");
                }
            }

            sb.AppendLineLf();

            // Generate using statements
            sb.AppendLineLf("using System;");
            sb.AppendLineLf("using System.Collections.Generic;");
            sb.AppendLineLf("using Newtonsoft.Json;");
            sb.AppendLineLf();

            // Start namespace
            sb.AppendLineLf("namespace dotacp.protocol");
            sb.AppendLineLf("{");

            // Separate definitions by type
            var typeAliases = new List<string>();
            var enumDefinitions = new List<string>();
            var recordClasses = new List<string>();

            foreach (var defProp in definitions.Properties().OrderBy(p => p.Name))
            {
                var defName = defProp.Name;
                var def = defProp.Value as JObject;
                if (def == null)
                    continue;

                var classCode = GenerateModelClass(defName, def);

                // Check type of generated code
                if (classCode.Contains("IEquatable<"))
                {
                    typeAliases.Add(classCode);
                }
                else if (classCode.Contains("public enum ") || classCode.Contains("public abstract enum "))
                {
                    enumDefinitions.Add(classCode);
                }
                else
                {
                    recordClasses.Add(classCode);
                }
            }

            // Add type aliases first
            if (typeAliases.Count > 0)
            {
                sb.AppendLineLf("    // Type aliases");
                sb.AppendLineLf();
                foreach (var alias in typeAliases)
                {
                    sb.Append(IndentCode(alias, 1));
                    sb.AppendLineLf();
                }
            }

            // Add enums next
            if (enumDefinitions.Count > 0)
            {
                sb.AppendLineLf("    // Enums for string-based enum-like types");
                sb.AppendLineLf();
                foreach (var enumDef in enumDefinitions)
                {
                    sb.Append(IndentCode(enumDef, 1));
                    sb.AppendLineLf();
                }
            }

            // Then add class definitions
            sb.AppendLineLf("    // Generated model classes from ACP schema");
            sb.AppendLineLf();
            foreach (var recordClass in recordClasses)
            {
                sb.Append(IndentCode(recordClass, 1));
                sb.AppendLineLf();
            }

            // Close namespace
            sb.AppendLineLf("}");

            return sb.ToString();
        }

        private string GenerateModelClass(string name, JObject definition)
        {
            var modelBuilder = new ModelClassBuilder(
                name, 
                definition, 
                definitions, 
                typeResolver!, 
                discriminatorAnalyzer!
            );

            return modelBuilder.Generate();
        }

        private string IndentCode(string code, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4);
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    sb.Append(indent);
                }
                sb.AppendLineLf(line);
            }

            return sb.ToString();
        }
    }
}
