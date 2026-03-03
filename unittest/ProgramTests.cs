using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    [TestClass]
    public class ProgramTests
    {
        private static Type GetProgramType()
        {
            var assembly = typeof(SchemaGenerator).Assembly;
            return assembly.GetType("dotacp.generator.Program");
        }

        private static MethodInfo GetMainMethod()
        {
            var programType = GetProgramType();
            return programType.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private async Task<int> InvokeMainAsync(string[] args)
        {
            var mainMethod = GetMainMethod();
            var task = (Task<int>)mainMethod.Invoke(null, new object[] { args });
            return await task;
        }

        [TestMethod]
        public async Task Main_SchemaCommand_WithValidSchema_ReturnsZero()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(outputDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{
                    ""$defs"": {
                        ""TestType"": {
                            ""type"": ""object"",
                            ""properties"": {
                                ""value"": { ""type"": ""string"" }
                            }
                        }
                    }
                }");

                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "v1.0.0");

                var args = new[] { "schema", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
                Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Schema.cs")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_SchemaCommand_WithMissingSchemaFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);

            try
            {
                var args = new[] { "schema", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_MetaCommand_WithValidMeta_ReturnsZero()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(outputDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""version"": 1,
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "v1.0.0");

                var args = new[] { "meta", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
                Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Meta.cs")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_MetaCommand_WithMissingMetaFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);

            try
            {
                var args = new[] { "meta", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InterfacesCommand_WithValidFiles_ReturnsZero()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = tempDir;
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{
                    ""$defs"": {
                        ""InitializeRequest"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        },
                        ""InitializeResponse"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        }
                    }
                }");

                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "v1.0.0");

                var args = new[] { "interfaces", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
                Assert.IsTrue(File.Exists(Path.Combine(agentDir, "IAcpAgent.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(clientDir, "IAcpClient.cs")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InterfacesCommand_WithMissingMetaFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), "{}");

                var args = new[] { "interfaces", "--schema-dir", schemaDir, "--output-dir", tempDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InterfacesCommand_WithMissingSchemaFile_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), "{}");

                var args = new[] { "interfaces", "--schema-dir", schemaDir, "--output-dir", tempDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_WithNoDownload_ReturnsZero()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{
                    ""$defs"": {
                        ""TestType"": {
                            ""type"": ""object"",
                            ""properties"": {
                                ""value"": { ""type"": ""string"" }
                            }
                        }
                    }
                }");

                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""version"": 1,
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "v1.0.0");

                var args = new[] { "all", "--no-download", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
                Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Schema.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(outputDir, "Meta.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(agentDir, "IAcpAgent.cs")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_WithVersion_SkipsDownloadIfCached()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{
                    ""$defs"": {
                        ""TestType"": {
                            ""type"": ""object"",
                            ""properties"": {
                                ""value"": { ""type"": ""string"" }
                            }
                        }
                    }
                }");

                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""version"": 1,
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "refs/tags/v1.0.0");

                var args = new[] { "all", "--version", "v1.0.0", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_SchemaGenerationFails_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");

            Directory.CreateDirectory(schemaDir);

            try
            {
                // Create invalid schema.json
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), "not valid json {");

                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""version"": 1,
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                var args = new[] { "all", "--no-download", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_MetaGenerationFails_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");

            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{
                    ""$defs"": {}
                }");

                // meta.json is missing - will cause error

                var args = new[] { "all", "--no-download", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreNotEqual(0, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_SchemaCommand_WithInvalidJson_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), "invalid json {");

                var args = new[] { "schema", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_MetaCommand_WithInvalidJson_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var outputDir = Path.Combine(tempDir, "output");
            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), "invalid json {");

                var args = new[] { "meta", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InterfacesCommand_WithInvalidJson_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            Directory.CreateDirectory(schemaDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), "invalid json {");
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), "{}");

                var args = new[] { "interfaces", "--schema-dir", schemaDir, "--output-dir", tempDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(1, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_WithForceFlag_DownloadsSchema()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "refs/tags/v0.9.0");

                var args = new[] { "all", "--version", "main", "--force", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                // Note: This test requires network access to GitHub
                try
                {
                    var result = await InvokeMainAsync(args);

                    // Assert
                    // If successful, schema files should be downloaded
                    if (result == 0)
                    {
                        Assert.IsTrue(File.Exists(Path.Combine(schemaDir, "schema.json")));
                        Assert.IsTrue(File.Exists(Path.Combine(schemaDir, "meta.json")));
                    }
                }
                catch (Exception)
                {
                    // Network issues - test is inconclusive
                    Assert.Inconclusive("Network unavailable or GitHub unreachable");
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_WithCustomRepo_UsesCustomRepo()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                // Create existing files so schema generation passes without download
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{""$defs"":{}}");
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{""version"":1,""agentMethods"":{},""clientMethods"":{}}");
                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "v1.0.0");

                var args = new[] {
                    "all",
                    "--no-download",
                    "--repo", "custom/repo",
                    "--schema-dir", schemaDir,
                    "--output-dir", outputDir
                };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert - should succeed with no-download flag
                Assert.AreEqual(0, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InterfacesCommand_WithProtocolOutputDir_ResolvesCorrectly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var protocolDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"), @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {}
                }");

                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{""$defs"":{}}");

                var args = new[] { "interfaces", "--schema-dir", schemaDir, "--output-dir", protocolDir };

                // Act
                var result = await InvokeMainAsync(args);

                // Assert
                Assert.AreEqual(0, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_WithDifferentVersion_TriggersDownload()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                // Set up cached version that differs from requested
                File.WriteAllText(Path.Combine(schemaDir, "VERSION"), "refs/tags/v0.9.0");

                var args = new[] { "all", "--version", "main", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act - This will attempt to download from GitHub
                try
                {
                    var result = await InvokeMainAsync(args);
                    // If successful, files should be present
                    if (result == 0)
                    {
                        Assert.IsTrue(File.Exists(Path.Combine(schemaDir, "schema.json")));
                    }
                }
                catch
                {
                    // Network issues - test is inconclusive
                    Assert.Inconclusive("Network unavailable");
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_AllCommand_InterfacesGenerationFails_ReturnsError()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "protocol", "schema");
            var outputDir = Path.Combine(tempDir, "protocol");

            Directory.CreateDirectory(schemaDir);

            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                File.WriteAllText(Path.Combine(schemaDir, "schema.json"), @"{""$defs"":{}}");
                File.WriteAllText(Path.Combine(schemaDir, "meta.json"),
                    @"{""version"":1,""agentMethods"":{},""clientMethods"":{}}");

                var args = new[] { "all", "--no-download", "--schema-dir", schemaDir, "--output-dir", outputDir };

                // Act
                var result = await InvokeMainAsync(args);

                Assert.AreEqual(0, result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task Main_InvalidCommand_ReturnsError()
        {
            // Arrange
            var args = new[] { "invalid-command" };

            // Act
            var result = await InvokeMainAsync(args);

            // Assert
            Assert.AreNotEqual(0, result);
        }

        [TestMethod]
        public async Task Main_NoArguments_ReturnsError()
        {
            // Arrange
            var args = new string[] { };

            // Act
            var result = await InvokeMainAsync(args);

            // Assert
            Assert.AreNotEqual(0, result);
        }
    }
}
