using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace dotacp.unittest
{
    [TestClass]
    public class MetaGeneratorTests
    {
        [TestMethod]
        public void Generate_WithValidMeta_ReturnsExpectedOutput()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""agentMethods"": {
                        ""initialize"": ""initialize"",
                        ""new_session"": ""session/new""
                    },
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file"",
                        ""write_text_file"": ""fs/write_text_file""
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.Contains("// Generated from schema/meta.json. Do not edit by hand.", result);
                Assert.Contains("// Schema ref: v0.10.8", result);
                Assert.Contains("public static class ProtocolMeta", result);
                Assert.Contains("public const ushort Version = 1;", result);
                Assert.Contains("public static class AgentMethods", result);
                Assert.Contains("public const string Initialize = \"initialize\";", result);
                Assert.Contains("public const string NewSession = \"session/new\";", result);
                Assert.Contains("public static class ClientMethods", result);
                Assert.Contains("public const string ReadTextFile = \"fs/read_text_file\";", result);
                Assert.Contains("public const string WriteTextFile = \"fs/write_text_file\";", result);
                Assert.Contains("#pragma warning disable CS1591", result);
                Assert.Contains("#pragma warning restore CS1591", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithoutVersionFile_GeneratesWithoutSchemaRef()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION_NONEXISTENT");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 2,
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.Contains("// Generated from schema/meta.json. Do not edit by hand.", result);
                Assert.DoesNotContain("// Schema ref:", result);
                Assert.Contains("public const ushort Version = 2;", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithEmptyVersionFile_GeneratesWithoutSchemaRef()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                File.WriteAllText(versionFilePath, "   ");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.DoesNotContain("// Schema ref:", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithNoVersionProperty_UsesDefaultVersion()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.Contains("public const ushort Version = 1;", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithCustomNamespace_UsesNamespace()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new MetaGenerator();
                var result = generator.Generate(metaJsonPath, versionFilePath, "dotacp.protocol.unstable");

                Assert.Contains("namespace dotacp.protocol.unstable", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithEmptyAgentMethods_GeneratesEmptyClass()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""test"": ""test/method""
                    }
                }");

                File.WriteAllText(versionFilePath, "test");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.Contains("public static class AgentMethods", result);
                Assert.Contains("public static class ClientMethods", result);
                Assert.Contains("public const string Test = \"test/method\";", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithNullAgentMethods_GeneratesEmptyClass()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""clientMethods"": {}
                }");

                File.WriteAllText(versionFilePath, "test");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                Assert.IsNotNull(result);
                Assert.Contains("public static class AgentMethods", result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_MethodsSortedAlphabetically()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var metaJsonPath = Path.Combine(tempDir, "meta.json");
                var versionFilePath = Path.Combine(tempDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""version"": 1,
                    ""agentMethods"": {
                        ""zzz"": ""zzz"",
                        ""aaa"": ""aaa"",
                        ""mmm"": ""mmm""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(versionFilePath, "test");

                var generator = new MetaGenerator();

                // Act
                var result = generator.Generate(metaJsonPath, versionFilePath);

                // Assert
                // Just verify the result contains the methods - order checking is implicit in generation
                Assert.Contains("public const string Aaa", result);
                Assert.Contains("public const string Mmm", result);
                Assert.Contains("public const string Zzz", result);

                // Verify they appear in alphabetical order
                int aaaPos = result.IndexOf("Aaa = ");
                Assert.IsGreaterThan(0, aaaPos, "Aaa should be found");

                int mmmPos = result.IndexOf("Mmm = ");
                Assert.IsGreaterThan(0, mmmPos, "Mmm should be found");

                int zzzPos = result.IndexOf("Zzz = ");
                Assert.IsGreaterThan(0, zzzPos, "Zzz should be found");

                Assert.IsLessThan(mmmPos, aaaPos, "Aaa should appear before Mmm");
                Assert.IsLessThan(zzzPos, mmmPos, "Mmm should appear before Zzz");
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }
    }
}
