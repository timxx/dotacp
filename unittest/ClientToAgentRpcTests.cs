using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests real RPC communication from client to agent using the stable API surface only.
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

                var response = await pair.ClientConn.AuthenticateAsync(new AuthenticateRequest
                {
                    MethodId = "oauth2"
                });

                Assert.IsNotNull(pair.Agent.LastAuthenticateRequest);
                Assert.AreEqual("oauth2", pair.Agent.LastAuthenticateRequest!.MethodId.ToString());
                Assert.IsNotNull(response);
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
                    AgentInfo = new Implementation
                    {
                        Name = "test-agent",
                        Version = "1.0.0"
                    },
                    AgentCapabilities = new AgentCapabilities
                    {
                        LoadSession = true,
                        McpCapabilities = new McpCapabilities { Http = true },
                        PromptCapabilities = new PromptCapabilities { Image = true },
                        SessionCapabilities = new SessionCapabilities
                        {
                            List = new SessionListCapabilities()
                        }
                    },
                    AuthMethods = new AuthMethod[0]
                };

                var response = await pair.ClientConn.InitializeAsync(new InitializeRequest
                {
                    ProtocolVersion = ProtocolMeta.Version,
                    ClientInfo = new Implementation { Name = "test-client", Version = "2.0.0" },
                    ClientCapabilities = new ClientCapabilities
                    {
                        Fs = new FileSystemCapabilities { ReadTextFile = true, WriteTextFile = true },
                        Terminal = true
                    }
                });

                Assert.IsNotNull(pair.Agent.LastInitializeRequest);
                Assert.AreEqual("test-client", pair.Agent.LastInitializeRequest!.ClientInfo.Name);
                Assert.IsNotNull(response);
                Assert.AreEqual("test-agent", response.AgentInfo.Name);
                Assert.IsTrue(response.AgentCapabilities.LoadSession);
                Assert.HasCount(0, response.AuthMethods);
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
                        new SessionInfo { SessionId = "session-1", Cwd = "/tmp", Title = "Test Session" }
                    },
                    NextCursor = "cursor-abc"
                };

                var response = await pair.ClientConn.ListSessionsAsync(new ListSessionsRequest
                {
                    Cwd = "/tmp"
                });

                Assert.IsNotNull(pair.Agent.LastListSessionsRequest);
                Assert.AreEqual("/tmp", pair.Agent.LastListSessionsRequest!.Cwd);
                Assert.IsNotNull(response);
                Assert.HasCount(1, response.Sessions);
                Assert.AreEqual("session-1", (string)response.Sessions[0].SessionId);
            }
        }

        [TestMethod]
        public async Task LoadSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.LoadSessionResponseToReturn = new LoadSessionResponse
                {
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Id = "model",
                            Name = "Model",
                            CurrentValue = "gpt-5",
                            Options = new SessionConfigSelectOption[]
                            {
                                new SessionConfigSelectOption { Name = "GPT-5", Value = "gpt-5" }
                            }
                        }
                    },
                    Modes = new SessionModeState
                    {
                        CurrentModeId = "plan",
                        AvailableModes = new[]
                        {
                            new SessionMode { Id = "plan", Name = "Plan" }
                        }
                    }
                };

                var response = await pair.ClientConn.LoadSessionAsync(new LoadSessionRequest
                {
                    SessionId = "session-1",
                    Cwd = "/workspace",
                    McpServers = new McpServer[0]
                });

                Assert.IsNotNull(pair.Agent.LastLoadSessionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastLoadSessionRequest!.SessionId);
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.ConfigOptions);
                Assert.IsNotNull(response.Modes);
            }
        }

        [TestMethod]
        public async Task NewSessionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.NewSessionResponseToReturn = new NewSessionResponse
                {
                    SessionId = "new-session-1",
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Id = "mode",
                            Name = "Mode",
                            CurrentValue = "plan",
                            Options = new SessionConfigSelectOption[]
                            {
                                new SessionConfigSelectOption { Name = "Plan", Value = "plan" }
                            }
                        }
                    },
                    Modes = new SessionModeState
                    {
                        CurrentModeId = "plan",
                        AvailableModes = new[] { new SessionMode { Id = "plan", Name = "Plan" } }
                    }
                };

                var response = await pair.ClientConn.NewSessionAsync(new NewSessionRequest
                {
                    Cwd = "/workspace",
                    McpServers = new McpServer[0]
                });

                Assert.IsNotNull(pair.Agent.LastNewSessionRequest);
                Assert.AreEqual("/workspace", pair.Agent.LastNewSessionRequest!.Cwd);
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
                    StopReason = StopReason.EndTurn
                };

                var response = await pair.ClientConn.PromptAsync(new PromptRequest
                {
                    SessionId = "session-1",
                    Prompt = new ContentBlock[] { new TextContent { Text = "Hello" } }
                });

                Assert.IsNotNull(pair.Agent.LastPromptRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastPromptRequest!.SessionId);
                Assert.IsNotNull(response);
                Assert.AreEqual(StopReason.EndTurn, response.StopReason);
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

                var response = await pair.ClientConn.SetSessionConfigOptionAsync(new SetSessionConfigOptionRequest
                {
                    SessionId = "session-1",
                    ConfigId = "mode",
                    Value = (SessionConfigValueId)"plan"
                });

                Assert.IsNotNull(pair.Agent.LastSetSessionConfigOptionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionConfigOptionRequest!.SessionId);
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task SetSessionModeAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Agent.SetSessionModeResponseToReturn = new SetSessionModeResponse();

                var response = await pair.ClientConn.SetSessionModeAsync(new SetSessionModeRequest
                {
                    SessionId = "session-1",
                    ModeId = "plan"
                });

                Assert.IsNotNull(pair.Agent.LastSetSessionModeRequest);
                Assert.AreEqual("plan", (string)pair.Agent.LastSetSessionModeRequest!.ModeId);
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

                var response = await pair.ClientConn.ExtMethodAsync("custom_method", new Dictionary<string, object>
                {
                    { "param1", "value1" }
                });

                Assert.IsNotNull(pair.Agent.LastExtMethodName);
                Assert.AreEqual("custom_method", pair.Agent.LastExtMethodName);

                Assert.IsNotNull(pair.Agent.LastExtMethodRequest);
                Assert.IsTrue(pair.Agent.LastExtMethodRequest is Dictionary<string, object>);
                Assert.AreEqual("value1", ((Dictionary<string, object>)pair.Agent.LastExtMethodRequest)["param1"].ToString());

                Assert.IsNotNull(response);
                Assert.IsTrue(response is JObject);
                var dict = ((JObject)response).ToObject<Dictionary<string, string>>();
                Assert.IsNotNull(dict);
                Assert.AreEqual("customValue", dict["customField"]);
            }
        }

        [TestMethod]
        public async Task ExtNotificationAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                await pair.ClientConn.ExtNotificationAsync("custom_event", new Dictionary<string, object>
                {
                    { "event", "status_change" }
                });

                var received = await Task.WhenAny(pair.Agent.ExtNotificationReceivedSignal.Task, Task.Delay(5000));
                Assert.AreEqual(pair.Agent.ExtNotificationReceivedSignal.Task, received);
                Assert.AreEqual("custom_event", pair.Agent.LastExtNotificationName);
                Assert.IsNotNull(pair.Agent.LastExtNotificationPayload);
                Assert.IsTrue(pair.Agent.LastExtNotificationPayload is Dictionary<string, object>);
                Assert.AreEqual("status_change", ((Dictionary<string, object>)pair.Agent.LastExtNotificationPayload)["event"].ToString());
            }
        }

        [TestMethod]
        public async Task TestCancelAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                var promptTask = pair.ClientConn.PromptAsync(new PromptRequest
                {
                    Meta = new Dictionary<string, object> { { "testCancel", true } },
                    SessionId = "session-1",
                    Prompt = new ContentBlock[] { new TextContent { Text = "Hello" } }
                });

                await pair.ClientConn.CancelAsync(new CancelNotification
                {
                    SessionId = "session-1"
                });

                var promptResponse = await promptTask;
                Assert.IsNotNull(pair.Agent.LastCancelNotification);
                Assert.AreEqual(StopReason.Cancelled, promptResponse.StopReason);
            }
        }
    }
}
