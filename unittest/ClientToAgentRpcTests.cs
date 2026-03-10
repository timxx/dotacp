using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests real RPC communication from client to agent.
    /// Uses a real client Connection sending to a real agent Connection backed by a mock IAcpAgent.
    /// Verifies request serialization and response deserialization through the actual JSON-RPC pipeline.
    /// </summary>
    [TestClass]
    public class ClientToAgentRpcTests
    {
        [TestMethod]
        public async Task AuthenticateAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.AuthenticateResponseToReturn = new AuthenticateResponse
                {
                    Meta = new Dictionary<string, object>
                    {
                        { "token", "abc123" },
                        { "expiresIn", 3600L }
                    }
                };

                var request = new AuthenticateRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "auth-1" } },
                    MethodId = "oauth2"
                };
                var response = await pair.ClientConn.AuthenticateAsync(request);

                Assert.IsNotNull(pair.Agent.LastAuthenticateRequest);
                Assert.AreEqual("oauth2", pair.Agent.LastAuthenticateRequest!.MethodId);
                Assert.AreEqual("auth-1", pair.Agent.LastAuthenticateRequest.Meta["traceId"].ToString());
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
                Assert.AreEqual("abc123", response.Meta["token"].ToString());
                Assert.AreEqual("3600", response.Meta["expiresIn"].ToString());
            }
        }

        [TestMethod]
        public async Task InitializeAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.InitializeResponseToReturn = new InitializeResponse
                {
                    Meta = new Dictionary<string, object> { { "serverRegion", "us-east" } },
                    ProtocolVersion = ProtocolMeta.Version,
                    AgentInfo = new Implementation
                    {
                        Meta = new Dictionary<string, object> { { "tier", "dev" } },
                        Name = "test-agent",
                        Title = "Test Agent",
                        Version = "1.0.0"
                    },
                    AgentCapabilities = new AgentCapabilities
                    {
                        Meta = new Dictionary<string, object> { { "caps", "v1" } },
                        LoadSession = true,
                        McpCapabilities = new McpCapabilities
                        {
                            Meta = new Dictionary<string, object> { { "mcp", true } },
                            Http = true,
                            Sse = true
                        },
                        PromptCapabilities = new PromptCapabilities
                        {
                            Meta = new Dictionary<string, object> { { "prompt", "full" } },
                            Audio = true,
                            EmbeddedContext = true,
                            Image = true
                        },
                        SessionCapabilities = new SessionCapabilities
                        {
                            Meta = new Dictionary<string, object> { { "session", "extended" } },
                            Fork = new SessionForkCapabilities
                            {
                                Meta = new Dictionary<string, object> { { "supported", true } }
                            },
                            List = new SessionListCapabilities
                            {
                                Meta = new Dictionary<string, object> { { "supported", true } }
                            },
                            Resume = new SessionResumeCapabilities
                            {
                                Meta = new Dictionary<string, object> { { "supported", true } }
                            }
                        }
                    },
                    AuthMethods = new AuthMethod[]
                    {
                        new AuthMethodAgent
                        {
                            Meta = new Dictionary<string, object> { { "priority", 1L } },
                            Id = "oauth",
                            Name = "OAuth 2.0",
                            Description = "Browser-based OAuth flow"
                        }
                    }
                };

                var request = new InitializeRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "init-1" } },
                    ProtocolVersion = ProtocolMeta.Version,
                    ClientInfo = new Implementation
                    {
                        Meta = new Dictionary<string, object> { { "platform", "vscode" } },
                        Name = "test-client",
                        Title = "Test Client",
                        Version = "2.0.0"
                    },
                    ClientCapabilities = new ClientCapabilities
                    {
                        Meta = new Dictionary<string, object> { { "clientCaps", true } },
                        Fs = new FileSystemCapabilities
                        {
                            Meta = new Dictionary<string, object> { { "fs", "enabled" } },
                            ReadTextFile = true,
                            WriteTextFile = true
                        },
                        Terminal = true
                    }
                };
                var response = await pair.ClientConn.InitializeAsync(request);

                Assert.IsNotNull(pair.Agent.LastInitializeRequest);
                Assert.AreEqual(ProtocolMeta.Version, (ushort)pair.Agent.LastInitializeRequest!.ProtocolVersion);
                Assert.AreEqual("test-client", pair.Agent.LastInitializeRequest.ClientInfo.Name);
                Assert.AreEqual("2.0.0", pair.Agent.LastInitializeRequest.ClientInfo.Version);
                Assert.AreEqual("Test Client", pair.Agent.LastInitializeRequest.ClientInfo.Title);
                Assert.IsTrue(pair.Agent.LastInitializeRequest.ClientCapabilities.Terminal);
                Assert.IsTrue(pair.Agent.LastInitializeRequest.ClientCapabilities.Fs.ReadTextFile);

                Assert.IsNotNull(response);
                Assert.AreEqual(ProtocolMeta.Version, (ushort)response.ProtocolVersion);
                Assert.AreEqual("test-agent", response.AgentInfo.Name);
                Assert.AreEqual("1.0.0", response.AgentInfo.Version);
                Assert.AreEqual("Test Agent", response.AgentInfo.Title);
                Assert.IsTrue(response.AgentCapabilities.LoadSession);
                Assert.IsTrue(response.AgentCapabilities.McpCapabilities.Http);
                Assert.IsTrue(response.AgentCapabilities.PromptCapabilities.Image);
                Assert.HasCount(1, response.AuthMethods);
                var authMethod = response.AuthMethods[0] as AuthMethodAgent;
                Assert.IsNotNull(authMethod);
                Assert.AreEqual("oauth", authMethod!.Id);
                Assert.AreEqual("OAuth 2.0", authMethod.Name);
                Assert.AreEqual("Browser-based OAuth flow", authMethod.Description);
            }
        }

        [TestMethod]
        public async Task ForkSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ForkSessionResponseToReturn = new ForkSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "forked", true } },
                    SessionId = "forked-session-1"
                };

                var request = new ForkSessionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "fork-1" } },
                    SessionId = "original-session",
                    Cwd = "/home/user/project",
                    McpServers = new McpServer[]
                    {
                        new McpServerHttp
                        {
                            Meta = new Dictionary<string, object> { { "origin", "client" } },
                            Name = "http-server",
                            Url = "https://example.test/mcp",
                            Headers = new[] { new HttpHeader { Name = "Authorization", Value = "Bearer token" } }
                        }
                    }
                };
                var response = await pair.ClientConn.ForkSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastForkSessionRequest);
                Assert.AreEqual("original-session", (string)pair.Agent.LastForkSessionRequest!.SessionId);
                Assert.AreEqual("/home/user/project", pair.Agent.LastForkSessionRequest.Cwd);
                Assert.AreEqual("fork-1", pair.Agent.LastForkSessionRequest.Meta["traceId"].ToString());
                Assert.HasCount(1, pair.Agent.LastForkSessionRequest.McpServers);
                Assert.IsInstanceOfType(pair.Agent.LastForkSessionRequest.McpServers[0], typeof(McpServerHttp));

                Assert.IsNotNull(response);
                Assert.AreEqual("forked-session-1", (string)response.SessionId);
                Assert.AreEqual("True", response.Meta["forked"].ToString());
            }
        }

        [TestMethod]
        public async Task ListSessionsAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ListSessionsResponseToReturn = new ListSessionsResponse
                {
                    Meta = new Dictionary<string, object> { { "pageSize", 1L } },
                    Sessions = new[]
                    {
                        new SessionInfo
                        {
                            Meta = new Dictionary<string, object> { { "status", "active" } },
                            SessionId = "session-1",
                            Cwd = "/tmp",
                            Title = "Test Session",
                            UpdatedAt = "2026-01-01T00:00:00Z"
                        }
                    },
                    NextCursor = "cursor-abc"
                };

                var request = new ListSessionsRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "list-1" } },
                    Cwd = "/tmp",
                    Cursor = "prev-cursor"
                };
                var response = await pair.ClientConn.ListSessionsAsync(request);

                Assert.IsNotNull(pair.Agent.LastListSessionsRequest);
                Assert.AreEqual("/tmp", pair.Agent.LastListSessionsRequest!.Cwd);
                Assert.AreEqual("prev-cursor", pair.Agent.LastListSessionsRequest.Cursor);
                Assert.AreEqual("list-1", pair.Agent.LastListSessionsRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.HasCount(1, response.Sessions);
                Assert.AreEqual("session-1", (string)response.Sessions[0].SessionId);
                Assert.AreEqual("/tmp", response.Sessions[0].Cwd);
                Assert.AreEqual("Test Session", response.Sessions[0].Title);
                Assert.AreEqual("2026-01-01T00:00:00Z", response.Sessions[0].UpdatedAt);
                Assert.AreEqual("cursor-abc", response.NextCursor);
                Assert.AreEqual("active", response.Sessions[0].Meta["status"].ToString());
            }
        }

        [TestMethod]
        public async Task LoadSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.LoadSessionResponseToReturn = new LoadSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "loaded", true } },
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Meta = new Dictionary<string, object> { { "source", "agent" } },
                            Id = "model-selector",
                            Name = "Model",
                            Description = "Choose the active model",
                            Category = SessionConfigOptionCategory.Model,
                            CurrentValue = "gpt-5",
                            Options = new SessionConfigSelectOption[]
                            {
                                new SessionConfigSelectOption
                                {
                                    Meta = new Dictionary<string, object> { { "default", true } },
                                    Name = "GPT-5",
                                    Description = "Balanced",
                                    Value = "gpt-5"
                                },
                                new SessionConfigSelectOption
                                {
                                    Name = "GPT-5 Mini",
                                    Description = "Fast",
                                    Value = "gpt-5-mini"
                                }
                            }
                        }
                    },
                    Models = new SessionModelState
                    {
                        Meta = new Dictionary<string, object> { { "models", 2L } },
                        CurrentModelId = "gpt-5",
                        AvailableModels = new[]
                        {
                            new ModelInfo
                            {
                                Meta = new Dictionary<string, object> { { "family", "gpt" } },
                                ModelId = "gpt-5",
                                Name = "GPT-5",
                                Description = "Primary model"
                            },
                            new ModelInfo
                            {
                                ModelId = "gpt-5-mini",
                                Name = "GPT-5 Mini",
                                Description = "Lower latency"
                            }
                        }
                    },
                    Modes = new SessionModeState
                    {
                        Meta = new Dictionary<string, object> { { "modes", 2L } },
                        CurrentModeId = "plan-mode",
                        AvailableModes = new[]
                        {
                            new SessionMode
                            {
                                Meta = new Dictionary<string, object> { { "scope", "default" } },
                                Id = "plan-mode",
                                Name = "Plan",
                                Description = "Plan-first responses"
                            },
                            new SessionMode
                            {
                                Id = "edit-mode",
                                Name = "Edit",
                                Description = "Direct edits"
                            }
                        }
                    }
                };

                var request = new LoadSessionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "load-1" } },
                    SessionId = "session-to-load",
                    Cwd = "/workspace",
                    McpServers = new McpServer[]
                    {
                        new McpServerStdio
                        {
                            Meta = new Dictionary<string, object> { { "transport", "stdio" } },
                            Name = "mcp-test",
                            Command = "/usr/bin/mcp",
                            Args = new[] { "--port", "3000" },
                            Env = new[] { new EnvVariable { Meta = new Dictionary<string, object> { { "masked", true } }, Name = "KEY", Value = "val" } }
                        },
                        new McpServerSse
                        {
                            Meta = new Dictionary<string, object> { { "transport", "sse" } },
                            Name = "mcp-sse",
                            Url = "https://example.test/sse",
                            Headers = new[] { new HttpHeader { Name = "X-Client", Value = "dotacp-tests" } }
                        }
                    }
                };
                var response = await pair.ClientConn.LoadSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastLoadSessionRequest);
                Assert.AreEqual("session-to-load", (string)pair.Agent.LastLoadSessionRequest!.SessionId);
                Assert.AreEqual("/workspace", pair.Agent.LastLoadSessionRequest.Cwd);
                Assert.AreEqual("load-1", pair.Agent.LastLoadSessionRequest.Meta["traceId"].ToString());
                Assert.HasCount(2, pair.Agent.LastLoadSessionRequest.McpServers);
                Assert.IsInstanceOfType(pair.Agent.LastLoadSessionRequest.McpServers[0], typeof(McpServerStdio));
                Assert.IsInstanceOfType(pair.Agent.LastLoadSessionRequest.McpServers[1], typeof(McpServerSse));

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
                Assert.IsNotNull(response.ConfigOptions);
                Assert.HasCount(1, response.ConfigOptions);
                Assert.IsInstanceOfType(response.ConfigOptions[0], typeof(SessionConfigSelect));
                var loadConfigSelect = (SessionConfigSelect)response.ConfigOptions[0];
                Assert.IsTrue(loadConfigSelect.Options.TryGetSessionConfigSelectOption(out var loadOptions));
                Assert.IsFalse(loadConfigSelect.Options.TryGetSessionConfigSelectGroup(out _));
                Assert.IsNotNull(loadOptions);
                Assert.HasCount(2, loadOptions);
                Assert.IsNotNull(response.Models);
                Assert.AreEqual("gpt-5", (string)response.Models.CurrentModelId);
                Assert.IsNotNull(response.Modes);
                Assert.AreEqual("plan-mode", (string)response.Modes.CurrentModeId);
            }
        }

        [TestMethod]
        public async Task NewSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.NewSessionResponseToReturn = new NewSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "created", true } },
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Id = "mode-selector",
                            Name = "Mode",
                            Description = "Choose session mode",
                            Category = SessionConfigOptionCategory.Mode,
                            CurrentValue = "plan-mode",
                            Options = new SessionConfigSelectGroup[]
                            {
                                new SessionConfigSelectGroup
                                {
                                    Group = "general",
                                    Name = "General",
                                    Options = new[]
                                    {
                                        new SessionConfigSelectOption { Name = "Plan", Value = "plan-mode", Description = "Structured" },
                                        new SessionConfigSelectOption { Name = "Edit", Value = "edit-mode", Description = "Direct" }
                                    }
                                }
                            }
                        }
                    },
                    Models = new SessionModelState
                    {
                        CurrentModelId = "gpt-5",
                        AvailableModels = new[]
                        {
                            new ModelInfo { ModelId = "gpt-5", Name = "GPT-5", Description = "Primary" }
                        }
                    },
                    Modes = new SessionModeState
                    {
                        CurrentModeId = "plan-mode",
                        AvailableModes = new[]
                        {
                            new SessionMode { Id = "plan-mode", Name = "Plan", Description = "Plan mode" }
                        }
                    },
                    SessionId = "new-session-1"
                };

                var request = new NewSessionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "new-1" } },
                    Cwd = "/home/user",
                    McpServers = new McpServer[]
                    {
                        new McpServerHttp
                        {
                            Name = "mcp-http",
                            Url = "https://example.test/http",
                            Headers = new[] { new HttpHeader { Name = "Authorization", Value = "Bearer token" } }
                        },
                        new McpServerSse
                        {
                            Name = "mcp-sse",
                            Url = "https://example.test/sse",
                            Headers = new[] { new HttpHeader { Name = "Accept", Value = "text/event-stream" } }
                        }
                    }
                };
                var response = await pair.ClientConn.NewSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastNewSessionRequest);
                Assert.AreEqual("/home/user", pair.Agent.LastNewSessionRequest!.Cwd);
                Assert.IsNotNull(pair.Agent.LastNewSessionRequest.McpServers);
                Assert.AreEqual("new-1", pair.Agent.LastNewSessionRequest.Meta["traceId"].ToString());
                Assert.HasCount(2, pair.Agent.LastNewSessionRequest.McpServers);

                Assert.IsNotNull(response);
                Assert.AreEqual("new-session-1", (string)response.SessionId);
                Assert.IsNotNull(response.ConfigOptions);
                var newConfigSelect = (SessionConfigSelect)response.ConfigOptions[0];
                Assert.IsFalse(newConfigSelect.Options.TryGetSessionConfigSelectOption(out _));
                Assert.IsTrue(newConfigSelect.Options.TryGetSessionConfigSelectGroup(out var newGroups));
                Assert.IsNotNull(newGroups);
                Assert.HasCount(1, newGroups);
                Assert.IsNotNull(response.Models);
                Assert.IsNotNull(response.Modes);
            }
        }

        [TestMethod]
        public async Task PromptAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.PromptResponseToReturn = new PromptResponse
                {
                    Meta = new Dictionary<string, object> { { "turn", 1L } },
                    StopReason = StopReason.EndTurn,
                    Usage = new Usage
                    {
                        CachedReadTokens = 30,
                        CachedWriteTokens = 20,
                        InputTokens = 100,
                        OutputTokens = 50,
                        ThoughtTokens = 25,
                        TotalTokens = 150
                    }
                };

                var request = new PromptRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "prompt-1" } },
                    SessionId = "session-1",
                    Prompt = new ContentBlock[]
                    {
                        new TextContent
                        {
                            Meta = new Dictionary<string, object> { { "lang", "en" } },
                            Annotations = new Annotations
                            {
                                Audience = new[] { Role.Assistant },
                                LastModified = "2026-03-03T00:00:00Z",
                                Priority = 1.0
                            },
                            Text = "Hello, agent!"
                        },
                        new ImageContent
                        {
                            Meta = new Dictionary<string, object> { { "source", "clipboard" } },
                            MimeType = "image/png",
                            Data = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAAB",
                            Uri = "file:///tmp/screenshot.png"
                        },
                        new ResourceLink
                        {
                            Name = "README",
                            Title = "Project README",
                            Description = "Reference docs",
                            MimeType = "text/markdown",
                            Size = 1234,
                            Uri = "file:///workspace/README.md"
                        }
                    }
                };
                var response = await pair.ClientConn.PromptAsync(request);

                Assert.IsNotNull(pair.Agent.LastPromptRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastPromptRequest!.SessionId);
                Assert.AreEqual("prompt-1", pair.Agent.LastPromptRequest.Meta["traceId"].ToString());
                Assert.HasCount(3, pair.Agent.LastPromptRequest.Prompt);
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[0], typeof(TextContent));
                Assert.AreEqual("Hello, agent!", ((TextContent)pair.Agent.LastPromptRequest.Prompt[0]).Text);
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[1], typeof(ImageContent));
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[2], typeof(ResourceLink));

                Assert.IsNotNull(response);
                Assert.AreEqual(StopReason.EndTurn, response.StopReason);
                Assert.IsNotNull(response.Usage);
                Assert.AreEqual((ulong)30, response.Usage.CachedReadTokens);
                Assert.AreEqual((ulong)20, response.Usage.CachedWriteTokens);
                Assert.AreEqual((ulong)100, response.Usage.InputTokens);
                Assert.AreEqual((ulong)50, response.Usage.OutputTokens);
                Assert.AreEqual((ulong)25, response.Usage.ThoughtTokens);
                Assert.AreEqual((ulong)150, response.Usage.TotalTokens);
            }
        }

        [TestMethod]
        public async Task PromptAsync_WithEmbeddedResources()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.PromptResponseToReturn = new PromptResponse
                {
                    StopReason = StopReason.EndTurn,
                    Usage = new Usage
                    {
                        InputTokens = 42,
                        OutputTokens = 7,
                        TotalTokens = 49
                    }
                };

                var request = new PromptRequest
                {
                    SessionId = "session-embedded",
                    Prompt = new ContentBlock[]
                    {
                        new EmbeddedResource
                        {
                            Meta = new Dictionary<string, object> { { "res", "text" } },
                            Resource = new TextResourceContents
                            {
                                Uri = "file:///workspace/README.md",
                                MimeType = "text/markdown",
                                Text = "# Title"
                            }
                        },
                        new EmbeddedResource
                        {
                            Meta = new Dictionary<string, object> { { "res", "blob" } },
                            Resource = new BlobResourceContents
                            {
                                Uri = "file:///workspace/logo.png",
                                MimeType = "image/png",
                                Blob = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAAB"
                            }
                        }
                    }
                };

                var response = await pair.ClientConn.PromptAsync(request);

                Assert.IsNotNull(pair.Agent.LastPromptRequest);
                Assert.AreEqual("session-embedded", (string)pair.Agent.LastPromptRequest!.SessionId);
                Assert.HasCount(2, pair.Agent.LastPromptRequest.Prompt);
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[0], typeof(EmbeddedResource));
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[1], typeof(EmbeddedResource));

                var textEmbedded = (EmbeddedResource)pair.Agent.LastPromptRequest.Prompt[0];
                var blobEmbedded = (EmbeddedResource)pair.Agent.LastPromptRequest.Prompt[1];
                Assert.IsInstanceOfType(textEmbedded.Resource, typeof(TextResourceContents));
                Assert.IsInstanceOfType(blobEmbedded.Resource, typeof(BlobResourceContents));

                Assert.IsNotNull(response);
                Assert.AreEqual(StopReason.EndTurn, response.StopReason);
                Assert.IsNotNull(response.Usage);
                Assert.AreEqual((ulong)49, response.Usage.TotalTokens);
            }
        }

        [TestMethod]
        public async Task ResumeSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ResumeSessionResponseToReturn = new ResumeSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "resumed", true } }
                };

                var request = new ResumeSessionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "resume-1" } },
                    SessionId = "session-to-resume",
                    Cwd = "/workspace",
                    McpServers = new McpServer[]
                    {
                        new McpServerSse
                        {
                            Name = "resume-sse",
                            Url = "https://example.test/resume",
                            Headers = new[] { new HttpHeader { Name = "X-Session", Value = "resume" } }
                        }
                    }
                };
                var response = await pair.ClientConn.ResumeSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastResumeSessionRequest);
                Assert.AreEqual("session-to-resume", (string)pair.Agent.LastResumeSessionRequest!.SessionId);
                Assert.AreEqual("/workspace", pair.Agent.LastResumeSessionRequest.Cwd);
                Assert.AreEqual("resume-1", pair.Agent.LastResumeSessionRequest.Meta["traceId"].ToString());
                Assert.HasCount(1, pair.Agent.LastResumeSessionRequest.McpServers);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
            }
        }

        [TestMethod]
        public async Task SetSessionConfigOptionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.SetSessionConfigOptionResponseToReturn = new SetSessionConfigOptionResponse
                {
                    Meta = new Dictionary<string, object> { { "updated", true } },
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Id = "config-opt-1",
                            Name = "Mode",
                            Description = "Execution mode",
                            Category = SessionConfigOptionCategory.Mode,
                            CurrentValue = "value-1",
                            Options = new SessionConfigSelectOption[]
                            {
                                new SessionConfigSelectOption { Name = "Value 1", Value = "value-1", Description = "Default" },
                                new SessionConfigSelectOption { Name = "Value 2", Value = "value-2", Description = "Alternative" }
                            }
                        }
                    }
                };

                var request = new SetSessionConfigOptionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "set-config-1" } },
                    SessionId = "session-1",
                    ConfigId = "config-opt-1",
                    Value = "value-1"
                };
                var response = await pair.ClientConn.SetSessionConfigOptionAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionConfigOptionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionConfigOptionRequest!.SessionId);
                Assert.AreEqual("config-opt-1", (string)pair.Agent.LastSetSessionConfigOptionRequest.ConfigId);
                Assert.AreEqual("value-1", (string)pair.Agent.LastSetSessionConfigOptionRequest.Value);
                Assert.AreEqual("set-config-1", pair.Agent.LastSetSessionConfigOptionRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.ConfigOptions);
                Assert.HasCount(1, response.ConfigOptions);
                Assert.IsInstanceOfType(response.ConfigOptions[0], typeof(SessionConfigSelect));
            }
        }

        [TestMethod]
        public async Task SetSessionModeAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.SetSessionModeResponseToReturn = new SetSessionModeResponse();

                var request = new SetSessionModeRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "set-mode-1" } },
                    SessionId = "session-1",
                    ModeId = "plan-mode"
                };
                var response = await pair.ClientConn.SetSessionModeAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionModeRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionModeRequest!.SessionId);
                Assert.AreEqual("plan-mode", (string)pair.Agent.LastSetSessionModeRequest.ModeId);
                Assert.AreEqual("set-mode-1", pair.Agent.LastSetSessionModeRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task SetSessionModelAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.SetSessionModelResponseToReturn = new SetSessionModelResponse();

                var request = new SetSessionModelRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "set-model-1" } },
                    SessionId = "session-1",
                    ModelId = "gpt-4"
                };
                var response = await pair.ClientConn.SetSessionModelAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionModelRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionModelRequest!.SessionId);
                Assert.AreEqual("gpt-4", (string)pair.Agent.LastSetSessionModelRequest.ModelId);
                Assert.AreEqual("set-model-1", pair.Agent.LastSetSessionModelRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task CloseAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.CloseSessionResponseToReturn = new CloseSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "stopped", true } }
                };

                var request = new CloseSessionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "stop-1" } },
                    SessionId = "session-1"
                };
                var response = await pair.ClientConn.CloseAsync(request);

                Assert.IsNotNull(pair.Agent.LastCloseSessionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastCloseSessionRequest!.SessionId);
                Assert.AreEqual("stop-1", pair.Agent.LastCloseSessionRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
                Assert.IsTrue((bool)response.Meta["stopped"]);
            }
        }

        [TestMethod]
        public async Task ExtMethodAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ExtMethodResponseToReturn = new Dictionary<string, object>
                {
                    { "customField", "customValue" }
                };

                var extRequest = new Dictionary<string, object>
                {
                    { "param1", "value1" },
                    { "param2", 42L }
                };
                var response = await pair.ClientConn.ExtMethodAsync("custom_method", extRequest);

                Assert.IsNotNull(pair.Agent.LastExtMethodName);
                Assert.AreEqual("custom_method", pair.Agent.LastExtMethodName);
                Assert.IsNotNull(pair.Agent.LastExtMethodRequest);

                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task ExtNotificationAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                var notificationPayload = new Dictionary<string, object>
                {
                    { "event", "status_change" },
                    { "detail", "ready" }
                };

                await pair.ClientConn.ExtNotificationAsync("custom_event", notificationPayload);

                var received = await Task.WhenAny(
                    pair.Agent.ExtNotificationReceivedSignal.Task,
                    Task.Delay(5000));
                Assert.AreEqual(pair.Agent.ExtNotificationReceivedSignal.Task, received,
                    "Extension notification was not received within timeout");

                Assert.AreEqual("custom_event", pair.Agent.LastExtNotificationName);
                Assert.IsNotNull(pair.Agent.LastExtNotificationPayload);
            }
        }

        [TestMethod]
        public async Task ExtNotificationNoArgAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                await pair.ClientConn.ExtNotificationAsync("custom_event", null!);

                var received = await Task.WhenAny(
                    pair.Agent.ExtNotificationReceivedSignal.Task,
                    Task.Delay(5000));
                Assert.AreEqual(pair.Agent.ExtNotificationReceivedSignal.Task, received,
                    "Extension notification was not received within timeout");

                Assert.AreEqual("custom_event", pair.Agent.LastExtNotificationName);
                Assert.IsNotNull(pair.Agent.LastExtNotificationPayload);
            }
        }

        [TestMethod]
        public async Task TestCancelAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.PromptResponseToReturn = new PromptResponse
                {
                    StopReason = StopReason.EndTurn,
                };

                var request = new PromptRequest
                {
                    Meta = new Dictionary<string, object> { { "testCancel", true } },
                    SessionId = "session-1",
                    Prompt = new ContentBlock[]
                    {
                        new TextContent { Text = "Hello, agent!" }
                    }
                };

                var promptTask = pair.ClientConn.PromptAsync(request);

                await pair.ClientConn.CancelAsync(new CancelNotification
                {
                    Meta = new Dictionary<string, object> { { "traceId", "cancel-1" } },
                    SessionId = "session-1"
                });

                var promptResponse = await promptTask;

                Assert.IsNotNull(pair.Agent.LastCancelNotification);
                Assert.AreEqual("session-1", (string)pair.Agent.LastCancelNotification!.SessionId);
                Assert.AreEqual("cancel-1", pair.Agent.LastCancelNotification.Meta["traceId"].ToString());

                Assert.IsNotNull(promptResponse);
                Assert.AreEqual(StopReason.Cancelled, promptResponse.StopReason);
            }
        }
    }
}
