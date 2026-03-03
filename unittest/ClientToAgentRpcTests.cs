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
                    Meta = new Dictionary<string, object> { { "token", "abc123" } }
                };

                var request = new AuthenticateRequest { MethodId = "oauth2" };
                var response = await pair.ClientConn.AuthenticateAsync(request);

                Assert.IsNotNull(pair.Agent.LastAuthenticateRequest);
                Assert.AreEqual("oauth2", pair.Agent.LastAuthenticateRequest!.MethodId);
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
                Assert.AreEqual("abc123", response.Meta["token"].ToString());
            }
        }

        [TestMethod]
        public async Task InitializeAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.InitializeResponseToReturn = new InitializeResponse
                {
                    ProtocolVersion = ProtocolMeta.Version,
                    AgentInfo = new Implementation { Name = "test-agent", Version = "1.0.0" },
                    AuthMethods = new[]
                    {
                        new AuthMethod { Id = "oauth", Name = "OAuth 2.0" }
                    }
                };

                var request = new InitializeRequest
                {
                    ProtocolVersion = ProtocolMeta.Version,
                    ClientInfo = new Implementation { Name = "test-client", Version = "2.0.0" }
                };
                var response = await pair.ClientConn.InitializeAsync(request);

                Assert.IsNotNull(pair.Agent.LastInitializeRequest);
                Assert.AreEqual(ProtocolMeta.Version, (ushort)pair.Agent.LastInitializeRequest!.ProtocolVersion);
                Assert.AreEqual("test-client", pair.Agent.LastInitializeRequest.ClientInfo.Name);
                Assert.AreEqual("2.0.0", pair.Agent.LastInitializeRequest.ClientInfo.Version);

                Assert.IsNotNull(response);
                Assert.AreEqual(ProtocolMeta.Version, (ushort)response.ProtocolVersion);
                Assert.AreEqual("test-agent", response.AgentInfo.Name);
                Assert.AreEqual("1.0.0", response.AgentInfo.Version);
                Assert.HasCount(1, response.AuthMethods);
                Assert.AreEqual("oauth", response.AuthMethods[0].Id);
                Assert.AreEqual("OAuth 2.0", response.AuthMethods[0].Name);
            }
        }

        [TestMethod]
        public async Task ForkSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ForkSessionResponseToReturn = new ForkSessionResponse
                {
                    SessionId = "forked-session-1"
                };

                var request = new ForkSessionRequest
                {
                    SessionId = "original-session",
                    Cwd = "/home/user/project"
                };
                var response = await pair.ClientConn.ForkSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastForkSessionRequest);
                Assert.AreEqual("original-session", (string)pair.Agent.LastForkSessionRequest!.SessionId);
                Assert.AreEqual("/home/user/project", pair.Agent.LastForkSessionRequest.Cwd);

                Assert.IsNotNull(response);
                Assert.AreEqual("forked-session-1", (string)response.SessionId);
            }
        }

        [TestMethod]
        public async Task ListSessionsAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.ListSessionsResponseToReturn = new ListSessionsResponse
                {
                    Sessions = new[]
                    {
                        new SessionInfo
                        {
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
                    Cwd = "/tmp",
                    Cursor = "prev-cursor"
                };
                var response = await pair.ClientConn.ListSessionsAsync(request);

                Assert.IsNotNull(pair.Agent.LastListSessionsRequest);
                Assert.AreEqual("/tmp", pair.Agent.LastListSessionsRequest!.Cwd);
                Assert.AreEqual("prev-cursor", pair.Agent.LastListSessionsRequest.Cursor);

                Assert.IsNotNull(response);
                Assert.HasCount(1, response.Sessions);
                Assert.AreEqual("session-1", (string)response.Sessions[0].SessionId);
                Assert.AreEqual("/tmp", response.Sessions[0].Cwd);
                Assert.AreEqual("Test Session", response.Sessions[0].Title);
                Assert.AreEqual("2026-01-01T00:00:00Z", response.Sessions[0].UpdatedAt);
                Assert.AreEqual("cursor-abc", response.NextCursor);
            }
        }

        [TestMethod]
        public async Task LoadSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.LoadSessionResponseToReturn = new LoadSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "loaded", true } }
                };

                var request = new LoadSessionRequest
                {
                    SessionId = "session-to-load",
                    Cwd = "/workspace",
                    McpServers = new McpServer[]
                    {
                        new McpServerStdio
                        {
                            Name = "mcp-test",
                            Command = "/usr/bin/mcp",
                            Args = new[] { "--port", "3000" },
                            Env = new[] { new EnvVariable { Name = "KEY", Value = "val" } }
                        }
                    }
                };
                var response = await pair.ClientConn.LoadSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastLoadSessionRequest);
                Assert.AreEqual("session-to-load", (string)pair.Agent.LastLoadSessionRequest!.SessionId);
                Assert.AreEqual("/workspace", pair.Agent.LastLoadSessionRequest.Cwd);
                Assert.HasCount(1, pair.Agent.LastLoadSessionRequest.McpServers);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
            }
        }

        [TestMethod]
        public async Task NewSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.NewSessionResponseToReturn = new NewSessionResponse
                {
                    SessionId = "new-session-1"
                };

                var request = new NewSessionRequest
                {
                    Cwd = "/home/user",
                    McpServers = new McpServer[0]
                };
                var response = await pair.ClientConn.NewSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastNewSessionRequest);
                Assert.AreEqual("/home/user", pair.Agent.LastNewSessionRequest!.Cwd);
                Assert.IsNotNull(pair.Agent.LastNewSessionRequest.McpServers);

                Assert.IsNotNull(response);
                Assert.AreEqual("new-session-1", (string)response.SessionId);
            }
        }

        [TestMethod]
        public async Task PromptAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.PromptResponseToReturn = new PromptResponse
                {
                    StopReason = StopReason.EndTurn,
                    Usage = new Usage
                    {
                        InputTokens = 100,
                        OutputTokens = 50,
                        TotalTokens = 150
                    }
                };

                var request = new PromptRequest
                {
                    SessionId = "session-1",
                    Prompt = new ContentBlock[]
                    {
                        new TextContent { Text = "Hello, agent!" }
                    }
                };
                var response = await pair.ClientConn.PromptAsync(request);

                Assert.IsNotNull(pair.Agent.LastPromptRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastPromptRequest!.SessionId);
                Assert.HasCount(1, pair.Agent.LastPromptRequest.Prompt);
                Assert.IsInstanceOfType(pair.Agent.LastPromptRequest.Prompt[0], typeof(TextContent));
                Assert.AreEqual("Hello, agent!", ((TextContent)pair.Agent.LastPromptRequest.Prompt[0]).Text);

                Assert.IsNotNull(response);
                Assert.AreEqual(StopReason.EndTurn, response.StopReason);
                Assert.IsNotNull(response.Usage);
                Assert.AreEqual((ulong)100, response.Usage.InputTokens);
                Assert.AreEqual((ulong)50, response.Usage.OutputTokens);
                Assert.AreEqual((ulong)150, response.Usage.TotalTokens);
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
                    SessionId = "session-to-resume",
                    Cwd = "/workspace"
                };
                var response = await pair.ClientConn.ResumeSessionAsync(request);

                Assert.IsNotNull(pair.Agent.LastResumeSessionRequest);
                Assert.AreEqual("session-to-resume", (string)pair.Agent.LastResumeSessionRequest!.SessionId);
                Assert.AreEqual("/workspace", pair.Agent.LastResumeSessionRequest.Cwd);

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
                    ConfigOptions = new SessionConfigOption[0]
                };

                var request = new SetSessionConfigOptionRequest
                {
                    SessionId = "session-1",
                    ConfigId = "config-opt-1",
                    Value = "value-1"
                };
                var response = await pair.ClientConn.SetSessionConfigOptionAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionConfigOptionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionConfigOptionRequest!.SessionId);
                Assert.AreEqual("config-opt-1", (string)pair.Agent.LastSetSessionConfigOptionRequest.ConfigId);
                Assert.AreEqual("value-1", (string)pair.Agent.LastSetSessionConfigOptionRequest.Value);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.ConfigOptions);
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
                    SessionId = "session-1",
                    ModeId = "plan-mode"
                };
                var response = await pair.ClientConn.SetSessionModeAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionModeRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionModeRequest!.SessionId);
                Assert.AreEqual("plan-mode", (string)pair.Agent.LastSetSessionModeRequest.ModeId);

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
                    SessionId = "session-1",
                    ModelId = "gpt-4"
                };
                var response = await pair.ClientConn.SetSessionModelAsync(request);

                Assert.IsNotNull(pair.Agent.LastSetSessionModelRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionModelRequest!.SessionId);
                Assert.AreEqual("gpt-4", (string)pair.Agent.LastSetSessionModelRequest.ModelId);

                Assert.IsNotNull(response);
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
                    SessionId = "session-1"
                });

                var promptResponse = await promptTask;

                Assert.IsNotNull(pair.Agent.LastCancelNotification);
                Assert.AreEqual("session-1", (string)pair.Agent.LastCancelNotification!.SessionId);

                Assert.IsNotNull(promptResponse);
                Assert.AreEqual(StopReason.Cancelled, promptResponse.StopReason);
            }
        }
    }
}
