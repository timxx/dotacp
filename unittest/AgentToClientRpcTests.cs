using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests real RPC communication from agent to client.
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
                    Content = "line1\nline2\nline3"
                };

                var request = new ReadTextFileRequest
                {
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

                Assert.IsNotNull(response);
                Assert.AreEqual("line1\nline2\nline3", response.Content);
            }
        }

        [TestMethod]
        public async Task WriteTextFileAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.WriteTextFileResponseToReturn = new WriteTextFileResponse
                {
                    Meta = new Dictionary<string, object> { { "written", true } }
                };

                var request = new WriteTextFileRequest
                {
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

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Meta);
            }
        }

        [TestMethod]
        public async Task RequestPermissionAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.RequestPermissionResponseToReturn = new RequestPermissionResponse
                {
                    Outcome = new SelectedPermissionOutcome
                    {
                        OptionId = "allow-once-id"
                    }
                };

                var request = new RequestPermissionRequest
                {
                    SessionId = "session-1",
                    Options = new[]
                    {
                        new PermissionOption
                        {
                            OptionId = "allow-once-id",
                            Name = "Allow Once",
                            Kind = PermissionOptionKind.AllowOnce
                        },
                        new PermissionOption
                        {
                            OptionId = "reject-id",
                            Name = "Reject",
                            Kind = PermissionOptionKind.RejectOnce
                        }
                    },
                    ToolCall = new ToolCallUpdate
                    {
                        ToolCallId = "tc-1",
                        Title = "Execute command",
                        Status = ToolCallStatus.Pending
                    }
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.RequestPermissionAsync(request);

                Assert.IsNotNull(pair.Client.LastRequestPermissionRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastRequestPermissionRequest!.SessionId);
                Assert.HasCount(2, pair.Client.LastRequestPermissionRequest.Options);
                Assert.AreEqual("allow-once-id", (string)pair.Client.LastRequestPermissionRequest.Options[0].OptionId);
                Assert.AreEqual("Allow Once", pair.Client.LastRequestPermissionRequest.Options[0].Name);
                Assert.AreEqual(PermissionOptionKind.AllowOnce, pair.Client.LastRequestPermissionRequest.Options[0].Kind);
                Assert.AreEqual("reject-id", (string)pair.Client.LastRequestPermissionRequest.Options[1].OptionId);
                Assert.AreEqual(PermissionOptionKind.RejectOnce, pair.Client.LastRequestPermissionRequest.Options[1].Kind);
                Assert.AreEqual("tc-1", (string)pair.Client.LastRequestPermissionRequest.ToolCall.ToolCallId);
                Assert.AreEqual("Execute command", pair.Client.LastRequestPermissionRequest.ToolCall.Title);
                Assert.AreEqual(ToolCallStatus.Pending, pair.Client.LastRequestPermissionRequest.ToolCall.Status);

                Assert.IsNotNull(response);
                Assert.IsInstanceOfType(response.Outcome, typeof(SelectedPermissionOutcome));
                var selected = (SelectedPermissionOutcome)response.Outcome;
                Assert.AreEqual("allow-once-id", (string)selected.OptionId);
            }
        }

        [TestMethod]
        public async Task SessionUpdateAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                var notification = new SessionNotification
                {
                    SessionId = "session-1",
                    Update = new SessionUpdateAgentMessageChunk
                    {
                        Content = new TextContent { Text = "Thinking..." }
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
                Assert.IsInstanceOfType(pair.Client.LastSessionNotification.Update, typeof(SessionUpdateAgentMessageChunk));
                var chunk = (SessionUpdateAgentMessageChunk)pair.Client.LastSessionNotification.Update;
                Assert.IsInstanceOfType(chunk.Content, typeof(TextContent));
                Assert.AreEqual("Thinking...", ((TextContent)chunk.Content).Text);
            }
        }

        [TestMethod]
        public async Task CreateTerminalAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.CreateTerminalResponseToReturn = new CreateTerminalResponse
                {
                    TerminalId = "term-abc-123"
                };

                var request = new CreateTerminalRequest
                {
                    SessionId = "session-1",
                    Command = "bash",
                    Args = new[] { "-c", "echo hello" },
                    Cwd = "/home/user",
                    Env = new[] { new EnvVariable { Name = "PATH", Value = "/usr/bin" } },
                    OutputByteLimit = 1024
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.CreateTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastCreateTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastCreateTerminalRequest!.SessionId);
                Assert.AreEqual("bash", pair.Client.LastCreateTerminalRequest.Command);
                Assert.HasCount(2, pair.Client.LastCreateTerminalRequest.Args);
                Assert.AreEqual("-c", pair.Client.LastCreateTerminalRequest.Args[0]);
                Assert.AreEqual("echo hello", pair.Client.LastCreateTerminalRequest.Args[1]);
                Assert.AreEqual("/home/user", pair.Client.LastCreateTerminalRequest.Cwd);
                Assert.HasCount(1, pair.Client.LastCreateTerminalRequest.Env);
                Assert.AreEqual("PATH", pair.Client.LastCreateTerminalRequest.Env[0].Name);
                Assert.AreEqual("/usr/bin", pair.Client.LastCreateTerminalRequest.Env[0].Value);
                Assert.AreEqual((ulong)1024, pair.Client.LastCreateTerminalRequest.OutputByteLimit);

                Assert.IsNotNull(response);
                Assert.AreEqual("term-abc-123", response.TerminalId);
            }
        }

        [TestMethod]
        public async Task KillTerminalAsync()
        {
            using (var pair = ConnectionPair.Create())
            {
                pair.Client.KillTerminalResponseToReturn = new KillTerminalCommandResponse
                {
                    Meta = new Dictionary<string, object> { { "killed", true } }
                };

                var request = new KillTerminalCommandRequest
                {
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.KillTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastKillTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastKillTerminalRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastKillTerminalRequest.TerminalId);

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
                    Output = "hello world\n",
                    Truncated = false,
                    ExitStatus = new TerminalExitStatus { ExitCode = 0 }
                };

                var request = new TerminalOutputRequest
                {
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.TerminalOutputAsync(request);

                Assert.IsNotNull(pair.Client.LastTerminalOutputRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastTerminalOutputRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastTerminalOutputRequest.TerminalId);

                Assert.IsNotNull(response);
                Assert.AreEqual("hello world\n", response.Output);
                Assert.IsFalse(response.Truncated);
                Assert.IsNotNull(response.ExitStatus);
                Assert.AreEqual((uint)0, response.ExitStatus.ExitCode);
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
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.ReleaseTerminalAsync(request);

                Assert.IsNotNull(pair.Client.LastReleaseTerminalRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastReleaseTerminalRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastReleaseTerminalRequest.TerminalId);

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
                    ExitCode = 42,
                    Signal = "SIGTERM"
                };

                var request = new WaitForTerminalExitRequest
                {
                    SessionId = "session-1",
                    TerminalId = "term-1"
                };
                var agentConn = pair.Agent.ReceivedConnection!;
                var response = await agentConn.WaitForTerminalExitAsync(request);

                Assert.IsNotNull(pair.Client.LastWaitForTerminalExitRequest);
                Assert.AreEqual("session-1", (string)pair.Client.LastWaitForTerminalExitRequest!.SessionId);
                Assert.AreEqual("term-1", pair.Client.LastWaitForTerminalExitRequest.TerminalId);

                Assert.IsNotNull(response);
                Assert.AreEqual((uint)42, response.ExitCode);
                Assert.AreEqual("SIGTERM", response.Signal);
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
