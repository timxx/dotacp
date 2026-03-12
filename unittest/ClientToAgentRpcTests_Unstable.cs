using dotacp.protocol.unstable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    [TestClass]
    public class ClientToAgentRpcTests_Unstable
    {
        [TestMethod]
        public async Task ForkSessionAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.ForkSessionResponseToReturn = new ForkSessionResponse { SessionId = "forked-1" };

                var response = await pair.ClientConn.ForkSessionAsync(new ForkSessionRequest
                {
                    SessionId = "session-1",
                    Cwd = "/workspace",
                    McpServers = new McpServer[0]
                });

                Assert.IsNotNull(pair.Agent.LastForkSessionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastForkSessionRequest!.SessionId);
                Assert.IsNotNull(response);
                Assert.AreEqual("forked-1", (string)response.SessionId);
            }
        }

        [TestMethod]
        public async Task NewSessionAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.NewSessionResponseToReturn = new NewSessionResponse
                {
                    SessionId = "new-1",
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
                Assert.AreEqual("new-1", (string)response.SessionId);
                Assert.IsNotNull(response.ConfigOptions);
                Assert.IsNotNull(response.Models);
                Assert.AreEqual("gpt-5", (string)response.Models.CurrentModelId);
                Assert.IsNotNull(response.Modes);
            }
        }

        [TestMethod]
        public async Task LoadSessionAsync_WithModelsAndModes()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.LoadSessionResponseToReturn = new LoadSessionResponse
                {
                    ConfigOptions = new SessionConfigOption[]
                    {
                        new SessionConfigSelect
                        {
                            Id = "model-selector",
                            Name = "Model",
                            Description = "Choose the active model",
                            Category = SessionConfigOptionCategory.Model,
                            CurrentValue = "gpt-5",
                            Options = new SessionConfigSelectOption[]
                            {
                                new SessionConfigSelectOption { Name = "GPT-5", Description = "Balanced", Value = "gpt-5" }
                            }
                        }
                    },
                    Models = new SessionModelState
                    {
                        CurrentModelId = "gpt-5",
                        AvailableModels = new[]
                        {
                            new ModelInfo { ModelId = "gpt-5", Name = "GPT-5", Description = "Primary model" }
                        }
                    },
                    Modes = new SessionModeState
                    {
                        CurrentModeId = "plan-mode",
                        AvailableModes = new[]
                        {
                            new SessionMode { Id = "plan-mode", Name = "Plan", Description = "Plan-first responses" }
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
                Assert.IsNotNull(response.Models);
                Assert.AreEqual("gpt-5", (string)response.Models.CurrentModelId);
                Assert.IsNotNull(response.Modes);
                Assert.AreEqual("plan-mode", (string)response.Modes.CurrentModeId);
            }
        }

        [TestMethod]
        public async Task ResumeSessionAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.ResumeSessionResponseToReturn = new ResumeSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "resumed", true } }
                };

                var response = await pair.ClientConn.ResumeSessionAsync(new ResumeSessionRequest
                {
                    SessionId = "session-1",
                    Cwd = "/workspace",
                    McpServers = new McpServer[0]
                });

                Assert.IsNotNull(pair.Agent.LastResumeSessionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastResumeSessionRequest!.SessionId);
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task SetSessionConfigOptionAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
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
        public async Task SetSessionConfigOptionAsync_WithBooleanValue()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.SetSessionConfigOptionResponseToReturn = new SetSessionConfigOptionResponse
                {
                    ConfigOptions = new SessionConfigOption[0]
                };

                var response = await pair.ClientConn.SetSessionConfigOptionAsync(new SetSessionConfigOptionRequest
                {
                    SessionId = "session-1",
                    ConfigId = "feature-flag",
                    Type = "boolean",
                    Value = true
                });

                Assert.IsNotNull(pair.Agent.LastSetSessionConfigOptionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastSetSessionConfigOptionRequest!.SessionId);
                Assert.IsTrue(pair.Agent.LastSetSessionConfigOptionRequest.Value.TryGetBool(out var boolValue));
                Assert.IsTrue(boolValue);
                Assert.IsFalse(pair.Agent.LastSetSessionConfigOptionRequest.Value.TryGetSessionConfigValueId(out _));
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task PromptAsync_WithUsage()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.PromptResponseToReturn = new PromptResponse
                {
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

                var response = await pair.ClientConn.PromptAsync(new PromptRequest
                {
                    SessionId = "session-1",
                    Prompt = new ContentBlock[] { new TextContent { Text = "Hello, agent!" } }
                });

                Assert.IsNotNull(pair.Agent.LastPromptRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastPromptRequest!.SessionId);
                Assert.IsNotNull(response);
                Assert.AreEqual(StopReason.EndTurn, response.StopReason);
                Assert.IsNotNull(response.Usage);
                Assert.AreEqual((ulong)150, response.Usage.TotalTokens);
            }
        }

        [TestMethod]
        public async Task SetSessionModeAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
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
        public async Task SetSessionModelAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                var response = await pair.ClientConn.SetSessionModelAsync(new SetSessionModelRequest
                {
                    SessionId = "session-1",
                    ModelId = "gpt-5"
                });

                Assert.IsNotNull(pair.Agent.LastSetSessionModelRequest);
                Assert.AreEqual("gpt-5", (string)pair.Agent.LastSetSessionModelRequest!.ModelId);
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task CloseAsync()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                pair.Agent.CloseSessionResponseToReturn = new CloseSessionResponse
                {
                    Meta = new Dictionary<string, object> { { "closed", true } }
                };

                var response = await pair.ClientConn.CloseAsync(new CloseSessionRequest
                {
                    SessionId = "session-1"
                });

                Assert.IsNotNull(pair.Agent.LastCloseSessionRequest);
                Assert.AreEqual("session-1", (string)pair.Agent.LastCloseSessionRequest!.SessionId);
                Assert.IsNotNull(response);
            }
        }
    }
}
