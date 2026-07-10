using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests real RPC communication from agent to client using the stable API.
    /// Uses a real agent Connection sending to a real client Connection backed by a mock IAcpClient.
    /// Verifies request serialization and response deserialization through the actual JSON-RPC pipeline.
    /// </summary>
    [TestClass]
    public class AgentToClientRpcTests
    {
        [TestMethod]
        public async Task ReadTextFileAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.ReadTextFileResponseToReturn = new ReadTextFileResponse
                {
                    Meta = new Dictionary<string, object> { { "source", "mock-client" } },
                    Content = "line1\nline2\nline3"
                };

                var request = new ReadTextFileRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "rtf-1" } },
                    SessionId = "session-1",
                    Path = "/home/user/file.txt",
                    Line = 1,
                    Limit = 100
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.ReadTextFileAsync(request);

                Assert.IsNotNull(pair.Client.LastReadTextFileRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastReadTextFileRequest!.SessionId);
                Assert.AreEqual("/home/user/file.txt", pair.Client.LastReadTextFileRequest.Path);
                Assert.AreEqual((uint)1, pair.Client.LastReadTextFileRequest.Line);
                Assert.AreEqual((uint)100, pair.Client.LastReadTextFileRequest.Limit);
                Assert.IsNotNull(pair.Client.LastReadTextFileRequest.Meta);
                Assert.AreEqual("rtf-1", pair.Client.LastReadTextFileRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.AreEqual("line1\nline2\nline3", response.Content);
                Assert.IsNotNull(response.Meta);
                Assert.AreEqual("mock-client", response.Meta["source"].ToString());
            }
        }

        [TestMethod]
        public async Task ReadTextFileNonExistentAsync()
        {
            using var pair = ConnectionPair.Create();

            var request = new ReadTextFileRequest
            {
                Meta = new Dictionary<string, object> { { "testNonExistent", true } },
                SessionId = "session-1",
                Path = "/home/user/file.txt",
            };

            var agentConn = pair.Agent.ReceivedConnection!;

            var ex = await Assert.ThrowsAsync<RemoteInvocationException>(
                async () => await agentConn.ReadTextFileAsync(request));

            Assert.IsNotNull(ex);
            Assert.AreEqual("File not found", ex.Message);
            Assert.AreEqual((int)ErrorCode.ResourceNotFound, ex.ErrorCode);
            Assert.IsNull(ex.ErrorData);
        }

        [TestMethod]
        public async Task WriteTextFileAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.WriteTextFileResponseToReturn = new WriteTextFileResponse
                {
                    Meta = new Dictionary<string, object>
                    {
                        { "written", true },
                        { "bytes", 24L }
                    }
                };

                var request = new WriteTextFileRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "wtf-1" } },
                    SessionId = "session-1",
                    Path = "/home/user/output.txt",
                    Content = "Hello World\nSecond line"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.WriteTextFileAsync(request);

                Assert.IsNotNull(pair.Client.LastWriteTextFileRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastWriteTextFileRequest!.SessionId);
                Assert.AreEqual("/home/user/output.txt", pair.Client.LastWriteTextFileRequest.Path);
                Assert.AreEqual("Hello World\nSecond line", pair.Client.LastWriteTextFileRequest.Content);
                Assert.AreEqual("wtf-1", pair.Client.LastWriteTextFileRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
                Assert.AreEqual("True", response.Meta["written"].ToString());
                Assert.AreEqual("24", response.Meta["bytes"].ToString());
            }
        }

        [TestMethod]
        public async Task RequestPermissionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.RequestPermissionResponseToReturn = new RequestPermissionResponse
                {
                    Meta = new Dictionary<string, object> { { "decisionAt", "now" } },
                    Outcome = new SelectedPermissionOutcome
                    {
                        Meta = new Dictionary<string, object> { { "source", "ui" } },
                        OptionId = "allow-once-id"
                    }
                };

                var request = new RequestPermissionRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "perm-1" } },
                    SessionId = "session-1",
                    Options = new[]
                    {
                        new PermissionOption
                        {
                            Meta = new Dictionary<string, object> { { "risk", "low" } },
                            OptionId = "allow-once-id",
                            Name = "Allow Once",
                            Kind = PermissionOptionKind.AllowOnce
                        },
                        new PermissionOption
                        {
                            Meta = new Dictionary<string, object> { { "risk", "high" } },
                            OptionId = "reject-id",
                            Name = "Reject",
                            Kind = PermissionOptionKind.RejectOnce
                        }
                    },
                    ToolCall = new ToolCallUpdate
                    {
                        Meta = new Dictionary<string, object> { { "updateSource", "agent" } },
                        ToolCallId = "tc-1",
                        Title = "Execute command",
                        Status = ToolCallStatus.Pending,
                        Kind = ToolKind.Execute,
                        RawInput = new Dictionary<string, object> { { "command", "ls -la" } },
                        RawOutput = new Dictionary<string, object> { { "started", true } },
                        Locations = new[]
                        {
                            new ToolCallLocation
                            {
                                Meta = new Dictionary<string, object> { { "kind", "cwd" } },
                                Path = "/home/user/project",
                                Line = 12
                            }
                        },
                        Content = new ToolCallContent[]
                        {
                            new Content
                            {
                                Meta = new Dictionary<string, object> { { "seq", 1L } },
                                ContentValue = new TextContent
                                {
                                    Meta = new Dictionary<string, object> { { "format", "markdown" } },
                                    Annotations = new Annotations
                                    {
                                        Meta = new Dictionary<string, object> { { "anno", true } },
                                        Audience = new[] { Role.User },
                                        LastModified = "2026-03-03T00:00:00Z",
                                        Priority = 0.7
                                    },
                                    Text = "Preparing command"
                                }
                            },
                            new Diff
                            {
                                Meta = new Dictionary<string, object> { { "seq", 2L } },
                                Path = "/home/user/project/file.txt",
                                OldText = "old",
                                NewText = "new"
                            }
                        }
                    }
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.RequestPermissionAsync(request);

                Assert.IsNotNull(pair.Client.LastRequestPermissionRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastRequestPermissionRequest!.SessionId);
                Assert.AreEqual("perm-1", pair.Client.LastRequestPermissionRequest.Meta["traceId"].ToString());
                Assert.HasCount(2, pair.Client.LastRequestPermissionRequest.Options);
                Assert.AreEqual("allow-once-id", (string)pair.Client.LastRequestPermissionRequest.Options[0].OptionId);
                Assert.AreEqual("Allow Once", pair.Client.LastRequestPermissionRequest.Options[0].Name);
                Assert.AreEqual(PermissionOptionKind.AllowOnce, pair.Client.LastRequestPermissionRequest.Options[0].Kind);
                Assert.AreEqual("low", pair.Client.LastRequestPermissionRequest.Options[0].Meta["risk"].ToString());
                Assert.AreEqual("reject-id", (string)pair.Client.LastRequestPermissionRequest.Options[1].OptionId);
                Assert.AreEqual(PermissionOptionKind.RejectOnce, pair.Client.LastRequestPermissionRequest.Options[1].Kind);
                Assert.AreEqual("tc-1", (string)pair.Client.LastRequestPermissionRequest.ToolCall.ToolCallId);
                Assert.AreEqual("Execute command", pair.Client.LastRequestPermissionRequest.ToolCall.Title);
                Assert.AreEqual(ToolCallStatus.Pending, pair.Client.LastRequestPermissionRequest.ToolCall.Status);
                Assert.AreEqual(ToolKind.Execute, pair.Client.LastRequestPermissionRequest.ToolCall.Kind);
                Assert.HasCount(1, pair.Client.LastRequestPermissionRequest.ToolCall.Locations);
                Assert.AreEqual("/home/user/project", pair.Client.LastRequestPermissionRequest.ToolCall.Locations[0].Path);
                Assert.AreEqual((uint)12, pair.Client.LastRequestPermissionRequest.ToolCall.Locations[0].Line);
                Assert.HasCount(2, pair.Client.LastRequestPermissionRequest.ToolCall.Content);

                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response.Outcome, typeof(SelectedPermissionOutcome));
                Assert.AreEqual("now", response.Meta["decisionAt"].ToString());
                var selected = (SelectedPermissionOutcome)response.Outcome;
                Assert.AreEqual("allow-once-id", (string)selected.OptionId);
                Assert.AreEqual("ui", selected.Meta["source"].ToString());
            }
        }

        [TestMethod]
        public async Task RequestPermissionAsync_CancelledOutcome()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.RequestPermissionResponseToReturn = new RequestPermissionResponse
                {
                    Meta = new Dictionary<string, object> { { "decisionAt", "cancelled" } },
                    Outcome = new RequestPermissionOutcomeCancelled()
                };

                var request = new RequestPermissionRequest
                {
                    SessionId = "session-1",
                    Options = new[]
                    {
                        new PermissionOption
                        {
                            OptionId = "cancel-id",
                            Name = "Cancel",
                            Kind = PermissionOptionKind.RejectOnce
                        }
                    },
                    ToolCall = new ToolCallUpdate
                    {
                        ToolCallId = "tc-cancel-1",
                        Title = "Needs approval",
                        Status = ToolCallStatus.Pending
                    }
                };

                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.RequestPermissionAsync(request);

                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response.Outcome, typeof(RequestPermissionOutcomeCancelled));
                Assert.AreEqual("cancelled", response.Outcome.Outcome);
            }
        }

        [TestMethod]
        public async Task SessionUpdateAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                var notification = new SessionNotification
                {
                    Meta = new Dictionary<string, object> { { "eventId", "sess-upd-1" } },
                    SessionId = "session-1",
                    Update = new SessionUpdateAgentMessageChunk
                    {
                        Meta = new Dictionary<string, object> { { "chunk", 1L } },
                        Content = new TextContent
                        {
                            Meta = new Dictionary<string, object> { { "render", "plain" } },
                            Annotations = new Annotations
                            {
                                Meta = new Dictionary<string, object> { { "anno", "yes" } },
                                Audience = new[] { Role.User },
                                LastModified = "2026-03-03T00:00:01Z",
                                Priority = 0.5
                            },
                            Text = "Thinking..."
                        }
                    }
                };

                var agentConn = pair.Agent.ReceivedConnection!;
                await agentConn.SessionUpdateAsync(notification);

                var received = await Task.WhenAny(
                    pair.Client.SessionUpdateReceivedSignal.Task,
                    Task.Delay(5000));
                Assert.AreEqual(pair.Client.SessionUpdateReceivedSignal.Task, received,
                    "Session update notification was not received within timeout");

                Assert.IsNotNull(pair.Client.LastSessionNotification);
                Assert.AreEqual("session-1", (string)pair.Client.LastSessionNotification!.SessionId);
                Assert.AreEqual("sess-upd-1", pair.Client.LastSessionNotification.Meta["eventId"].ToString());
                Assert.IsInstanceOfType(pair.Client.LastSessionNotification.Update, typeof(SessionUpdateAgentMessageChunk));
                var chunk = (SessionUpdateAgentMessageChunk)pair.Client.LastSessionNotification.Update;
                Assert.IsInstanceOfType(chunk.Content, typeof(TextContent));
                Assert.AreEqual("Thinking...", ((TextContent)chunk.Content).Text);
                Assert.IsTrue(((TextContent)chunk.Content).Meta.ContainsKey("render"));
            }
        }

        [TestMethod]
        public async Task SessionUpdateAsync_VariantDiscriminators()
        {
            async Task SendAndAssertAsync(SessionUpdate update, string expectedSessionUpdate)
            {
                using (var pair = ConnectionPair.Create())
                {
                    var notification = new SessionNotification
                    {
                        SessionId = "session-variants",
                        Update = update
                    };

                    var agentConn = pair.Agent.ReceivedConnection!;
                    await agentConn.SessionUpdateAsync(notification);

                    var received = await Task.WhenAny(
                        pair.Client.SessionUpdateReceivedSignal.Task,
                        Task.Delay(5000));

                    Assert.AreEqual(pair.Client.SessionUpdateReceivedSignal.Task, received,
                        $"Session update '{expectedSessionUpdate}' was not received within timeout");
                    Assert.IsNotNull(pair.Client.LastSessionNotification);
                    Assert.AreEqual("session-variants", (string)pair.Client.LastSessionNotification!.SessionId);
                    Assert.AreEqual(expectedSessionUpdate, pair.Client.LastSessionNotification.Update.SessionUpdateValue);
                }
            }

            await SendAndAssertAsync(
                new SessionUpdateAgentThoughtChunk
                {
                    Meta = new Dictionary<string, object> { { "kind", "thought" } },
                    Content = new TextContent { Text = "Analyzing" }
                },
                "agent_thought_chunk");

            await SendAndAssertAsync(
                new SessionUpdateUserMessageChunk
                {
                    Meta = new Dictionary<string, object> { { "kind", "user" } },
                    Content = new TextContent { Text = "User chunk" }
                },
                "user_message_chunk");

            await SendAndAssertAsync(
                new Plan
                {
                    Meta = new Dictionary<string, object> { { "kind", "plan" } },
                    Entries = new[]
                    {
                        new PlanEntry
                        {
                            Content = "Inspect workspace",
                            Priority = PlanEntryPriority.Medium,
                            Status = PlanEntryStatus.InProgress
                        }
                    }
                },
                "plan");

            await SendAndAssertAsync(
                new CurrentModeUpdate
                {
                    Meta = new Dictionary<string, object> { { "kind", "mode" } },
                    CurrentModeId = "plan-mode"
                },
                "current_mode_update");

            await SendAndAssertAsync(
                new SessionInfoUpdate
                {
                    Meta = new Dictionary<string, object> { { "kind", "session-info" } },
                    Title = "Updated Session",
                    UpdatedAt = "2026-03-03T00:00:00Z"
                },
                "session_info_update");

            await SendAndAssertAsync(
                new ConfigOptionUpdate
                {
                    Meta = new Dictionary<string, object> { { "kind", "config" } },
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
                    }
                },
                "config_option_update");

            await SendAndAssertAsync(
                new ToolCall
                {
                    Meta = new Dictionary<string, object> { { "kind", "tool-call" } },
                    ToolCallId = "tool-1",
                    Title = "Read file",
                    Status = ToolCallStatus.InProgress,
                    Kind = ToolKind.Read,
                    RawInput = new Dictionary<string, object> { { "path", "/tmp/a.txt" } },
                    RawOutput = new Dictionary<string, object> { { "ok", true } },
                    Locations = new[] { new ToolCallLocation { Path = "/tmp/a.txt", Line = 1 } },
                    Content = new ToolCallContent[]
                    {
                        new Content { ContentValue = new TextContent { Text = "Reading..." } }
                    }
                },
                "tool_call");

            await SendAndAssertAsync(
                new SessionUpdateToolCallUpdate
                {
                    Meta = new Dictionary<string, object> { { "kind", "tool-call-update" } },
                    ToolCallId = "tool-1",
                    Title = "Read done",
                    Status = ToolCallStatus.Completed,
                    Kind = ToolKind.Read,
                    RawInput = new Dictionary<string, object> { { "path", "/tmp/a.txt" } },
                    RawOutput = new Dictionary<string, object> { { "bytes", 12L } },
                    Locations = new[] { new ToolCallLocation { Path = "/tmp/a.txt", Line = 1 } },
                    Content = new ToolCallContent[]
                    {
                        new Content { ContentValue = new TextContent { Text = "Done" } }
                    }
                },
                "tool_call_update");
        }

        [TestMethod]
        public async Task CreateTerminalAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.CreateTerminalResponseToReturn = new CreateTerminalResponse
                {
                    Meta = new Dictionary<string, object> { { "created", true } },
                    TerminalId = "term-abc-123"
                };

                var request = new CreateTerminalRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "term-create-1" } },
                    SessionId = "session-1",
                    Command = "bash",
                    Args = new[] { "-c", "echo hello" },
                    Cwd = "/home/user",
                    Env = new[]
                    {
                        new EnvVariable
                        {
                            Meta = new Dictionary<string, object> { { "scope", "process" } },
                            Name = "PATH",
                            Value = "/usr/bin"
                        }
                    },
                    OutputByteLimit = 1024
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.CreateTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastCreateTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastCreateTerminalRequest!.SessionId);
                Assert.AreEqual("bash", pair.Client.LastCreateTerminalRequest.Command);
                Assert.AreEqual("term-create-1", pair.Client.LastCreateTerminalRequest.Meta["traceId"].ToString());
                Assert.HasCount(2, pair.Client.LastCreateTerminalRequest.Args);
                Assert.AreEqual("-c", pair.Client.LastCreateTerminalRequest.Args[0]);
                Assert.AreEqual("echo hello", pair.Client.LastCreateTerminalRequest.Args[1]);
                Assert.AreEqual("/home/user", pair.Client.LastCreateTerminalRequest.Cwd);
                Assert.HasCount(1, pair.Client.LastCreateTerminalRequest.Env);
                Assert.AreEqual("PATH", pair.Client.LastCreateTerminalRequest.Env[0].Name);
                Assert.AreEqual("/usr/bin", pair.Client.LastCreateTerminalRequest.Env[0].Value);
                Assert.AreEqual((ulong)1024, pair.Client.LastCreateTerminalRequest.OutputByteLimit);

                Assert.IsNotNull(response);
                Assert.AreEqual("term-abc-123", response.TerminalId.ToString());
                Assert.AreEqual("True", response.Meta["created"].ToString());
            }
        }

        [TestMethod]
        public async Task KillTerminalAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.KillTerminalResponseToReturn = new KillTerminalResponse
                {
                    Meta = new Dictionary<string, object> { { "killed", true } }
                };

                var request = new KillTerminalRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "kill-1" } },
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.KillTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastKillTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastKillTerminalRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastKillTerminalRequest.TerminalId.ToString());
                Assert.AreEqual("kill-1", pair.Client.LastKillTerminalRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
            }
        }

        [TestMethod]
        public async Task TerminalOutputAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.TerminalOutputResponseToReturn = new TerminalOutputResponse
                {
                    Meta = new Dictionary<string, object> { { "collected", true } },
                    Output = "hello world\n",
                    Truncated = false,
                    ExitStatus = new TerminalExitStatus { ExitCode = 0, Signal = null }
                };

                var request = new TerminalOutputRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "term-out-1" } },
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.TerminalOutputAsync(request);

                Assert.IsNotNull(pair.Client.LastTerminalOutputRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastTerminalOutputRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastTerminalOutputRequest.TerminalId.ToString());
                Assert.AreEqual("term-out-1", pair.Client.LastTerminalOutputRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.AreEqual("hello world\n", response.Output);
                Assert.IsFalse(response.Truncated);
                Assert.IsNotNull(response.ExitStatus);
                Assert.AreEqual((uint)0, response.ExitStatus.ExitCode);
                Assert.AreEqual("True", response.Meta["collected"].ToString());
            }
        }

        [TestMethod]
        public async Task ReleaseTerminalAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.ReleaseTerminalResponseToReturn = new ReleaseTerminalResponse();

                var request = new ReleaseTerminalRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "release-1" } },
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.ReleaseTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastReleaseTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastReleaseTerminalRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastReleaseTerminalRequest.TerminalId.ToString());
                Assert.AreEqual("release-1", pair.Client.LastReleaseTerminalRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task WaitForTerminalExitAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.WaitForTerminalExitResponseToReturn = new WaitForTerminalExitResponse
                {
                    Meta = new Dictionary<string, object> { { "waited", true } },
                    ExitCode = 42,
                    Signal = "SIGTERM"
                };

                var request = new WaitForTerminalExitRequest
                {
                    Meta = new Dictionary<string, object> { { "traceId", "wait-exit-1" } },
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.WaitForTerminalExitAsync(request);

                Assert.IsNotNull(pair.Client.LastWaitForTerminalExitRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastWaitForTerminalExitRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastWaitForTerminalExitRequest.TerminalId.ToString());
                Assert.AreEqual("wait-exit-1", pair.Client.LastWaitForTerminalExitRequest.Meta["traceId"].ToString());

                Assert.IsNotNull(response);
                Assert.AreEqual((uint)42, response.ExitCode);
                Assert.AreEqual("SIGTERM", response.Signal);
                Assert.AreEqual("True", response.Meta["waited"].ToString());
            }
        }

        [TestMethod]
        public async Task ExtMethodAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.ExtMethodResponseToReturn = new Dictionary<string, object>
                {
                    { "resultKey", "resultValue" }
                };

                var extRequest = new Dictionary<string, object>
                {
                    { "input", "data" }
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.ExtMethodAsync("client_custom", extRequest);

                Assert.IsNotNull(pair.Client.LastExtMethodName);
                Assert.AreEqual("client_custom", pair.Client.LastExtMethodName);
                Assert.IsNotNull(pair.Client.LastExtMethodRequest);

                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task ExtMethodNoArgAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.ExtMethodResponseToReturn = new Dictionary<string, object>
                {
                    { "resultKey", "resultValue" }
                };

                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.ExtMethodAsync("client_custom", null!);

                Assert.IsNotNull(pair.Client.LastExtMethodName);
                Assert.AreEqual("client_custom", pair.Client.LastExtMethodName);
                Assert.IsNotNull(pair.Client.LastExtMethodRequest);

                Assert.IsNotNull(response);
                Assert.IsTrue(response is JObject);

                var dict = ((JObject)response).ToObject<Dictionary<string, string>>();
                Assert.IsNotNull(dict);
                Assert.IsTrue(dict.ContainsKey("resultKey"));
                Assert.AreEqual("resultValue", dict["resultKey"]);
            }
        }

        [TestMethod]
        public async Task ExtNotificationAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                var notificationPayload = new Dictionary<string, object>
                {
                    { "status", "connected" }
                };

                var agentConn = pair.Agent.ReceivedConnection!;
                await agentConn.ExtNotificationAsync("client_event", notificationPayload);

                var received = await Task.WhenAny(
                    pair.Client.ExtNotificationReceivedSignal.Task,
                    Task.Delay(5000));
                Assert.AreEqual(pair.Client.ExtNotificationReceivedSignal.Task, received,
                    "Extension notification was not received within timeout");

                Assert.AreEqual("client_event", pair.Client.LastExtNotificationName);
                Assert.IsNotNull(pair.Client.LastExtNotificationPayload);
            }
        }
    }
}
