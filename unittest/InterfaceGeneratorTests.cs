using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace dotacp.unittest
{
    [TestClass]
    public class InterfaceGeneratorTests
    {
        [TestMethod]
        public void Generate_CreatesAllRequiredFiles()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""InitializeRequest"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent"",
                            ""description"": ""Initialize the agent""
                        },
                        ""InitializeResponse"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        },
                        ""ReadTextFileRequest"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        },
                        ""ReadTextFileResponse"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                Assert.IsTrue(File.Exists(Path.Combine(agentDir, "IAcpAgent.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(agentDir, "AgentRpcTarget.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(agentDir, "Connection.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(clientDir, "IAcpClient.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(clientDir, "ClientRpcTarget.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(clientDir, "Connection.cs")));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_IAcpAgent_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize"",
                        ""new_session"": ""session/new""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""InitializeRequest"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent"",
                            ""description"": ""Initialize the agent""
                        },
                        ""InitializeResponse"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        },
                        ""NewSessionRequest"": {
                            ""x-method"": ""session/new"",
                            ""x-side"": ""agent"",
                            ""description"": ""Create new session""
                        },
                        ""NewSessionResponse"": {
                            ""x-method"": ""session/new"",
                            ""x-side"": ""agent""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                Assert.Contains("public interface IAcpAgent", agentInterface);
                Assert.Contains("void OnClientConnected(Connection connection);", agentInterface);
                Assert.Contains("Task<InitializeResponse> InitializeAsync(InitializeRequest request,", agentInterface);
                Assert.Contains("Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,", agentInterface);
                Assert.Contains("Task<object> ExtMethodAsync(string method, object request,", agentInterface);
                Assert.Contains("Task ExtNotificationAsync(string method, object notification,", agentInterface);
                Assert.Contains("// Schema ref: v0.10.8", agentInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_AgentRpcTarget_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
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

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentRpcTarget = File.ReadAllText(Path.Combine(agentDir, "AgentRpcTarget.cs"));
                Assert.Contains("internal sealed class AgentRpcTarget", agentRpcTarget);
                Assert.Contains("private readonly IAcpAgent _agent;", agentRpcTarget);
                Assert.Contains("[JsonRpcMethod(AgentMethods.Initialize, UseSingleObjectParameterDeserialization = true)]", agentRpcTarget);
                Assert.Contains("public Task<InitializeResponse> InitializeAsync(", agentRpcTarget);
                Assert.Contains("return _agent.InitializeAsync(request, cancellationToken);", agentRpcTarget);
                Assert.Contains("[JsonRpcMethod(\"__acp_ext_method__\", UseSingleObjectParameterDeserialization = true)]", agentRpcTarget);
                Assert.Contains("public Task<object> HandleExtensionMethodAsync(", agentRpcTarget);
                Assert.Contains("[JsonRpcMethod(\"__acp_ext_notification__\", UseSingleObjectParameterDeserialization = true)]", agentRpcTarget);
                Assert.Contains("public Task HandleExtensionNotificationAsync(", agentRpcTarget);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithCustomNamespaces_UsesCustomAgentClientAndProtocolNamespaces()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent", "unstable");
            var clientDir = Path.Combine(tempDir, "client", "unstable");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""InitializeRequest"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        },
                        ""InitializeResponse"": {
                            ""x-method"": ""initialize"",
                            ""x-side"": ""agent""
                        },
                        ""ReadTextFileRequest"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        },
                        ""ReadTextFileResponse"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                generator.Generate(
                    metaJsonPath,
                    schemaJsonPath,
                    versionFilePath,
                    agentDir,
                    clientDir,
                    "dotacp.protocol.unstable",
                    "dotacp.agent.unstable",
                    "dotacp.client.unstable");

                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                var clientInterface = File.ReadAllText(Path.Combine(clientDir, "IAcpClient.cs"));
                var agentConnection = File.ReadAllText(Path.Combine(agentDir, "Connection.cs"));
                var clientConnection = File.ReadAllText(Path.Combine(clientDir, "Connection.cs"));

                Assert.Contains("namespace dotacp.agent.unstable", agentInterface);
                Assert.Contains("using dotacp.protocol.unstable;", agentInterface);
                Assert.Contains("namespace dotacp.client.unstable", clientInterface);
                Assert.Contains("using dotacp.protocol.unstable;", clientInterface);
                Assert.Contains("namespace dotacp.agent.unstable", agentConnection);
                Assert.Contains("namespace dotacp.client.unstable", clientConnection);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_AgentConnection_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""ReadTextFileRequest"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        },
                        ""ReadTextFileResponse"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentConnection = File.ReadAllText(Path.Combine(agentDir, "Connection.cs"));
                Assert.Contains("public class Connection", agentConnection);
                Assert.Contains("public Task Completion => _rpc.Completion;", agentConnection);
                Assert.Contains("public static Connection? RunAgent(IAcpAgent agent,", agentConnection);
                Assert.Contains("public Task<ReadTextFileResponse> ReadTextFileAsync(", agentConnection);
                Assert.Contains("return SendRequestAsync<ReadTextFileRequest, ReadTextFileResponse>(", agentConnection);
                Assert.Contains("public Task<object> ExtMethodAsync(string method, object request,", agentConnection);
                Assert.Contains("public Task ExtNotificationAsync(string method, object notification,", agentConnection);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_IAcpClient_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file"",
                        ""write_text_file"": ""fs/write_text_file""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""ReadTextFileRequest"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client"",
                            ""description"": ""Read a text file""
                        },
                        ""ReadTextFileResponse"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        },
                        ""WriteTextFileRequest"": {
                            ""x-method"": ""fs/write_text_file"",
                            ""x-side"": ""client""
                        },
                        ""WriteTextFileResponse"": {
                            ""x-method"": ""fs/write_text_file"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v0.10.8");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var clientInterface = File.ReadAllText(Path.Combine(clientDir, "IAcpClient.cs"));
                Assert.Contains("public interface IAcpClient", clientInterface);
                Assert.Contains("Task<ReadTextFileResponse> ReadTextFileAsync(ReadTextFileRequest request,", clientInterface);
                Assert.Contains("Task<WriteTextFileResponse> WriteTextFileAsync(WriteTextFileRequest request,", clientInterface);
                Assert.Contains("Task<object> ExtMethodAsync(string method, object request,", clientInterface);
                Assert.Contains("Task ExtNotificationAsync(string method, object notification,", clientInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_ClientRpcTarget_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""ReadTextFileRequest"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        },
                        ""ReadTextFileResponse"": {
                            ""x-method"": ""fs/read_text_file"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var clientRpcTarget = File.ReadAllText(Path.Combine(clientDir, "ClientRpcTarget.cs"));
                Assert.Contains("internal sealed class ClientRpcTarget", clientRpcTarget);
                Assert.Contains("private readonly IAcpClient _client;", clientRpcTarget);
                Assert.Contains("[JsonRpcMethod(ClientMethods.ReadTextFile, UseSingleObjectParameterDeserialization = true)]", clientRpcTarget);
                Assert.Contains("public Task<ReadTextFileResponse> ReadTextFileAsync(", clientRpcTarget);
                Assert.Contains("return _client.ReadTextFileAsync(request, cancellationToken);", clientRpcTarget);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_ClientConnection_ContainsExpectedMethods()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
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

                File.WriteAllText(versionFilePath, "");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var clientConnection = File.ReadAllText(Path.Combine(clientDir, "Connection.cs"));
                Assert.Contains("public class Connection", clientConnection);
                Assert.Contains("public static Connection? RunClient(IAcpClient client,", clientConnection);
                Assert.Contains("public Task<InitializeResponse> InitializeAsync(", clientConnection);
                Assert.Contains("return SendRequestAsync<InitializeRequest, InitializeResponse>(", clientConnection);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithNotifications_GeneratesCorrectly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""session_update"": ""session/update""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""SessionUpdateNotification"": {
                            ""x-method"": ""session/update"",
                            ""x-side"": ""agent"",
                            ""description"": ""Session update notification""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                Assert.Contains("Task SessionUpdateAsync(SessionUpdateNotification notification,", agentInterface);

                var agentRpcTarget = File.ReadAllText(Path.Combine(agentDir, "AgentRpcTarget.cs"));
                Assert.Contains("public Task SessionUpdateAsync(", agentRpcTarget);
                Assert.Contains("SessionUpdateNotification notification,", agentRpcTarget);
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
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION_NONEXISTENT");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
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

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                Assert.Contains("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.", agentInterface);
                Assert.DoesNotContain("// Schema ref:", agentInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithSessionMethods_UsesCorrectMethodNames()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""new_session"": ""session/new"",
                        ""cancel"": ""session/cancel"",
                        ""load_session"": ""session/load"",
                        ""fork_session"": ""session/fork"",
                        ""list_sessions"": ""session/list"",
                        ""resume_session"": ""session/resume"",
                        ""prompt"": ""session/prompt"",
                        ""session_update"": ""session/update"",
                        ""request_permission"": ""session/request_permission"",
                        ""set_session_config_option"": ""session/set_config_option"",
                        ""set_session_mode"": ""session/set_mode"",
                        ""set_session_model"": ""session/set_model""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""NewSessionRequest"": {""x-method"": ""session/new"", ""x-side"": ""agent""},
                        ""NewSessionResponse"": {""x-method"": ""session/new"", ""x-side"": ""agent""},
                        ""CancelRequest"": {""x-method"": ""session/cancel"", ""x-side"": ""agent""},
                        ""CancelResponse"": {""x-method"": ""session/cancel"", ""x-side"": ""agent""},
                        ""LoadSessionRequest"": {""x-method"": ""session/load"", ""x-side"": ""agent""},
                        ""LoadSessionResponse"": {""x-method"": ""session/load"", ""x-side"": ""agent""},
                        ""ForkSessionRequest"": {""x-method"": ""session/fork"", ""x-side"": ""agent""},
                        ""ForkSessionResponse"": {""x-method"": ""session/fork"", ""x-side"": ""agent""},
                        ""ListSessionsRequest"": {""x-method"": ""session/list"", ""x-side"": ""agent""},
                        ""ListSessionsResponse"": {""x-method"": ""session/list"", ""x-side"": ""agent""},
                        ""ResumeSessionRequest"": {""x-method"": ""session/resume"", ""x-side"": ""agent""},
                        ""ResumeSessionResponse"": {""x-method"": ""session/resume"", ""x-side"": ""agent""},
                        ""PromptRequest"": {""x-method"": ""session/prompt"", ""x-side"": ""agent""},
                        ""PromptResponse"": {""x-method"": ""session/prompt"", ""x-side"": ""agent""},
                        ""SessionUpdateNotification"": {""x-method"": ""session/update"", ""x-side"": ""agent""},
                        ""RequestPermissionRequest"": {""x-method"": ""session/request_permission"", ""x-side"": ""agent""},
                        ""RequestPermissionResponse"": {""x-method"": ""session/request_permission"", ""x-side"": ""agent""},
                        ""SetSessionConfigOptionRequest"": {""x-method"": ""session/set_config_option"", ""x-side"": ""agent""},
                        ""SetSessionConfigOptionResponse"": {""x-method"": ""session/set_config_option"", ""x-side"": ""agent""},
                        ""SetSessionModeRequest"": {""x-method"": ""session/set_mode"", ""x-side"": ""agent""},
                        ""SetSessionModeResponse"": {""x-method"": ""session/set_mode"", ""x-side"": ""agent""},
                        ""SetSessionModelRequest"": {""x-method"": ""session/set_model"", ""x-side"": ""agent""},
                        ""SetSessionModelResponse"": {""x-method"": ""session/set_model"", ""x-side"": ""agent""}
                    }
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                Assert.Contains("Task<NewSessionResponse> NewSessionAsync", agentInterface);
                Assert.Contains("Task<CancelResponse> CancelAsync", agentInterface);
                Assert.Contains("Task<LoadSessionResponse> LoadSessionAsync", agentInterface);
                Assert.Contains("Task<ForkSessionResponse> ForkSessionAsync", agentInterface);
                Assert.Contains("Task<ListSessionsResponse> ListSessionsAsync", agentInterface);
                Assert.Contains("Task<ResumeSessionResponse> ResumeSessionAsync", agentInterface);
                Assert.Contains("Task<PromptResponse> PromptAsync", agentInterface);
                Assert.Contains("Task SessionUpdateAsync", agentInterface);
                Assert.Contains("Task<RequestPermissionResponse> RequestPermissionAsync", agentInterface);
                Assert.Contains("Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync", agentInterface);
                Assert.Contains("Task<SetSessionModeResponse> SetSessionModeAsync", agentInterface);
                Assert.Contains("Task<SetSessionModelResponse> SetSessionModelAsync", agentInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithFsMethods_UsesCorrectMethodNames()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""read_text_file"": ""fs/read_text_file"",
                        ""search"": ""fs/search""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""ReadTextFileRequest"": {""x-method"": ""fs/read_text_file"", ""x-side"": ""client""},
                        ""ReadTextFileResponse"": {""x-method"": ""fs/read_text_file"", ""x-side"": ""client""},
                        ""SearchRequest"": {""x-method"": ""fs/search"", ""x-side"": ""client""},
                        ""SearchResponse"": {""x-method"": ""fs/search"", ""x-side"": ""client""}
                    }
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var clientInterface = File.ReadAllText(Path.Combine(clientDir, "IAcpClient.cs"));
                Assert.Contains("Task<ReadTextFileResponse> ReadTextFileAsync", clientInterface);
                Assert.Contains("Task<SearchResponse> SearchAsync", clientInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithTerminalMethods_UsesCorrectMethodNames()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""terminal_output"": ""terminal/output"",
                        ""wait_for_terminal_exit"": ""terminal/wait_for_exit"",
                        ""run_terminal"": ""terminal/run""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""TerminalOutputNotification"": {""x-method"": ""terminal/output"", ""x-side"": ""client""},
                        ""WaitForTerminalExitRequest"": {""x-method"": ""terminal/wait_for_exit"", ""x-side"": ""client""},
                        ""WaitForTerminalExitResponse"": {""x-method"": ""terminal/wait_for_exit"", ""x-side"": ""client""},
                        ""RunTerminalRequest"": {""x-method"": ""terminal/run"", ""x-side"": ""client""},
                        ""RunTerminalResponse"": {""x-method"": ""terminal/run"", ""x-side"": ""client""}
                    }
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var clientInterface = File.ReadAllText(Path.Combine(clientDir, "IAcpClient.cs"));
                Assert.Contains("Task TerminalOutputAsync", clientInterface);
                Assert.Contains("Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync", clientInterface);
                Assert.Contains("Task<RunTerminalResponse> RunTerminalAsync", clientInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_WithClientNotification_SendsNotificationInConnection()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {},
                    ""clientMethods"": {
                        ""terminal_output"": ""terminal/output""
                    }
                }");

                File.WriteAllText(schemaJsonPath, @"{
                    ""$defs"": {
                        ""TerminalOutputNotification"": {
                            ""x-method"": ""terminal/output"",
                            ""x-side"": ""client""
                        }
                    }
                }");

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert - Check that agent connection has notification send
                var agentConnection = File.ReadAllText(Path.Combine(agentDir, "Connection.cs"));
                Assert.Contains("public Task TerminalOutputAsync(", agentConnection);
                Assert.Contains("return SendNotificationAsync(ClientMethods.TerminalOutput, notification, cancellationToken);", agentConnection);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void Generate_SkipsMethodsWithoutRequestOrResponse()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var schemaDir = Path.Combine(tempDir, "schema");
            var agentDir = Path.Combine(tempDir, "agent");
            var clientDir = Path.Combine(tempDir, "client");

            Directory.CreateDirectory(schemaDir);
            Directory.CreateDirectory(agentDir);
            Directory.CreateDirectory(clientDir);

            try
            {
                var metaJsonPath = Path.Combine(schemaDir, "meta.json");
                var schemaJsonPath = Path.Combine(schemaDir, "schema.json");
                var versionFilePath = Path.Combine(schemaDir, "VERSION");

                File.WriteAllText(metaJsonPath, @"{
                    ""agentMethods"": {
                        ""initialize"": ""initialize"",
                        ""incomplete"": ""incomplete""
                    },
                    ""clientMethods"": {}
                }");

                File.WriteAllText(schemaJsonPath, @"{
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

                File.WriteAllText(versionFilePath, "v1.0.0");

                var generator = new InterfaceGenerator();

                // Act
                generator.Generate(metaJsonPath, schemaJsonPath, versionFilePath, agentDir, clientDir);

                // Assert
                var agentInterface = File.ReadAllText(Path.Combine(agentDir, "IAcpAgent.cs"));
                Assert.Contains("Task<InitializeResponse> InitializeAsync", agentInterface);
                Assert.DoesNotContain("IncompleteAsync", agentInterface);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }
    }
}
