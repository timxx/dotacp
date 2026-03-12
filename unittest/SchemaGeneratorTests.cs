using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace dotacp.unittest
{
    [TestClass]
    public class SchemaGeneratorTests
    {
        [TestMethod]
        public void Generate_SkipsDocsIgnoredDefinitions()
        {
            var schemaJson = @"{
  '$defs': {
    'IgnoredType': {
      'type': 'object',
      'x-docs-ignore': true,
      'properties': {
        'value': { 'type': 'string' }
      },
      'required': ['value']
    },
    'KeptType': {
      'type': 'object',
      'properties': {
        'name': { 'type': 'string' }
      },
      'required': ['name']
    }
  }
}";

            var schemaPath = Path.GetTempFileName();
            var versionPath = Path.GetTempFileName();

            File.WriteAllText(schemaPath, schemaJson);
            File.WriteAllText(versionPath, string.Empty);

            var generator = new SchemaGenerator();
            var result = generator.Generate(schemaPath, versionPath);

            Assert.DoesNotContain("class IgnoredType", result, "IgnoredType should not be generated");
            Assert.Contains("class KeptType", result, "KeptType should be generated");

            File.Delete(schemaPath);
            File.Delete(versionPath);
        }

        [TestMethod]
        public void Generate_WithCustomNamespace_UsesNamespaceAndBaseProtocolImports()
        {
            var schemaJson = @"{
  '$defs': {
    'KeptType': {
      'type': 'object',
      'properties': {
        'name': { 'type': 'string' }
      }
    }
  }
}";

            var schemaPath = Path.GetTempFileName();
            var versionPath = Path.GetTempFileName();

            File.WriteAllText(schemaPath, schemaJson);
            File.WriteAllText(versionPath, string.Empty);

            var generator = new SchemaGenerator();
            var result = generator.Generate(schemaPath, versionPath, "dotacp.protocol.unstable");

            Assert.Contains("namespace dotacp.protocol.unstable", result);
            Assert.Contains("using dotacp.protocol;", result);

            File.Delete(schemaPath);
            File.Delete(versionPath);
        }
    }
}
