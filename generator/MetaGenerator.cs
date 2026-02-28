using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace dotacp.generator
{
    /// <summary>
    /// Generates Meta.cs from meta.json
    /// </summary>
    public class MetaGenerator
    {
        /// <summary>
        /// Generate Meta.cs from meta.json
        /// </summary>
        public string Generate(string metaJsonPath, string versionFilePath)
        {
            var metaJson = File.ReadAllText(metaJsonPath);
            var meta = JObject.Parse(metaJson);

            var sb = new StringBuilder();

            // Generate header
            sb.AppendLineLf("// Generated from schema/meta.json. Do not edit by hand.");
            
            if (File.Exists(versionFilePath))
            {
                var gitRef = File.ReadAllText(versionFilePath).Trim();
                if (!string.IsNullOrEmpty(gitRef))
                {
                    sb.AppendLineLf($"// Schema ref: {gitRef}");
                }
            }

            sb.AppendLineLf();
            sb.AppendLineLf("namespace dotacp.protocol");
            sb.AppendLineLf("{");

            // Generate ProtocolMeta class
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Protocol metadata");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public static class ProtocolMeta");
            sb.AppendLineLf("    {");

            // Add protocol version
            var version = meta["version"]?.Value<ushort>() ?? 1;
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// ACP Protocol Version");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf($"        public const ushort Version = {version};");

            sb.AppendLineLf("    }");
            sb.AppendLineLf();

            // Generate AgentMethods class
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Methods that agents handle");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public static class AgentMethods");
            sb.AppendLineLf("    {");

            var agentMethods = meta["agentMethods"] as JObject;
            if (agentMethods != null)
            {
                foreach (var prop in agentMethods.Properties().OrderBy(p => p.Name))
                {
                    var constName = NamingHelper.ConvertToPascalCase(prop.Name);
                    var methodPath = prop.Value.ToString();
                    sb.AppendLineLf($"        public const string {constName} = \"{methodPath}\";");
                }
            }

            sb.AppendLineLf("    }");
            sb.AppendLineLf();

            // Generate ClientMethods class
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Methods that clients handle");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public static class ClientMethods");
            sb.AppendLineLf("    {");

            var clientMethods = meta["clientMethods"] as JObject;
            if (clientMethods != null)
            {
                foreach (var prop in clientMethods.Properties().OrderBy(p => p.Name))
                {
                    var constName = NamingHelper.ConvertToPascalCase(prop.Name);
                    var methodPath = prop.Value.ToString();
                    sb.AppendLineLf($"        public const string {constName} = \"{methodPath}\";");
                }
            }

            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            return sb.ToString();
        }
    }
}
