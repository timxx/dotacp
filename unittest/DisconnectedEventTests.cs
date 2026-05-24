using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nerdbank.Streams;
using System;
using System.Threading;
using System.Threading.Tasks;

using AgentConnection = dotacp.agent.Connection;
using ClientConnection = dotacp.client.Connection;
using IAcpAgent = dotacp.agent.IAcpAgent;
using IAcpClient = dotacp.client.IAcpClient;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests for the OnDisconnected event on both agent and client connections.
    /// </summary>
    [TestClass]
    public class DisconnectedEventTests
    {
        [TestMethod]
        public async Task Agent_OnDisconnected_CalledWhenClientConnectionDisposed()
        {
            var disconnectedTcs = new TaskCompletionSource<bool>();
            var mockAgent = new MockAgentWithDisconnectedCallback(() => disconnectedTcs.TrySetResult(true));

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(mockAgent, streams.Item1, streams.Item1);

            // Wait a bit to ensure connection is established
            await Task.Delay(50);

            // Dispose the client connection to trigger disconnect
            streams.Item2.Dispose();
            streams.Item1.Dispose();
            agentConnection!.Dispose();

            // Wait for the disconnected event
            var result = await Task.WhenAny(disconnectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(disconnectedTcs.Task, result, "OnDisconnected was not called within timeout");
        }

        [TestMethod]
        public async Task Client_OnDisconnected_CalledWhenAgentConnectionDisposed()
        {
            var disconnectedTcs = new TaskCompletionSource<bool>();
            var mockClient = new MockClientWithDisconnectedCallback(() => disconnectedTcs.TrySetResult(true));

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(new MockAgent(), streams.Item1, streams.Item1);
            var clientConnection = ClientConnection.RunClient(mockClient, streams.Item2, streams.Item2);

            // Wait a bit to ensure connection is established
            await Task.Delay(50);

            // Dispose the agent connection to trigger disconnect
            streams.Item1.Dispose();
            streams.Item2.Dispose();
            agentConnection!.Dispose();
            clientConnection!.Dispose();

            // Wait for the disconnected event
            var result = await Task.WhenAny(disconnectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(disconnectedTcs.Task, result, "OnDisconnected was not called within timeout");
        }

        [TestMethod]
        public async Task Agent_OnDisconnected_ConnectionParameterIsNotNull()
        {
            AgentConnection? disconnectedConnection = null;
            var mockAgent = new MockAgentWithDisconnectedCallback(conn => disconnectedConnection = conn);

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(mockAgent, streams.Item1, streams.Item1);

            // Wait a bit to ensure connection is established
            await Task.Delay(50);

            // Dispose to trigger disconnect
            streams.Item2.Dispose();
            streams.Item1.Dispose();
            agentConnection!.Dispose();

            // Wait for the disconnected event
            await Task.Delay(100);

            Assert.IsNotNull(disconnectedConnection, "Connection parameter should not be null");
            Assert.AreEqual(agentConnection, disconnectedConnection, "Connection parameter should be the same instance");
        }

        [TestMethod]
        public async Task Client_OnDisconnected_ConnectionParameterIsNotNull()
        {
            ClientConnection? disconnectedConnection = null;
            var mockClient = new MockClientWithDisconnectedCallback(conn => disconnectedConnection = conn);

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(new MockAgent(), streams.Item1, streams.Item1);
            var clientConnection = ClientConnection.RunClient(mockClient, streams.Item2, streams.Item2);

            // Wait a bit to ensure connection is established
            await Task.Delay(50);

            // Dispose to trigger disconnect
            streams.Item1.Dispose();
            streams.Item2.Dispose();
            agentConnection!.Dispose();
            clientConnection!.Dispose();

            // Wait for the disconnected event
            await Task.Delay(100);

            Assert.IsNotNull(disconnectedConnection, "Connection parameter should not be null");
            Assert.AreEqual(clientConnection, disconnectedConnection, "Connection parameter should be the same instance");
        }

        [TestMethod]
        public async Task Both_OnDisconnected_CalledOnConnectionPairDisposal()
        {
            var agentDisconnectedTcs = new TaskCompletionSource<bool>();
            var clientDisconnectedTcs = new TaskCompletionSource<bool>();

            var mockAgent = new MockAgentWithDisconnectedCallback(() => agentDisconnectedTcs.TrySetResult(true));
            var mockClient = new MockClientWithDisconnectedCallback(() => clientDisconnectedTcs.TrySetResult(true));

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(mockAgent, streams.Item1, streams.Item1);
            var clientConnection = ClientConnection.RunClient(mockClient, streams.Item2, streams.Item2);

            // Wait a bit to ensure connection is established
            await Task.Delay(50);

            // Dispose both sides
            streams.Item1.Dispose();
            streams.Item2.Dispose();
            agentConnection!.Dispose();
            clientConnection!.Dispose();

            // Wait for both disconnected events
            var agentResult = await Task.WhenAny(agentDisconnectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(agentDisconnectedTcs.Task, agentResult, "Agent OnDisconnected was not called within timeout");

            var clientResult = await Task.WhenAny(clientDisconnectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(clientDisconnectedTcs.Task, clientResult, "Client OnDisconnected was not called within timeout");
        }

        [TestMethod]
        public async Task OnDisconnected_CalledAfterOnClientConnected()
        {
            var connectedTcs = new TaskCompletionSource<bool>();
            var disconnectedTcs = new TaskCompletionSource<bool>();
            bool connectedCalledBeforeDisconnected = false;
            bool connectedCalled = false;

            var mockAgent = new MockAgentWithCallbacks(
                onConnected: () => { connectedCalled = true; connectedTcs.TrySetResult(true); },
                onDisconnected: () => { connectedCalledBeforeDisconnected = connectedCalled; disconnectedTcs.TrySetResult(true); }
            );

            var streams = FullDuplexStream.CreatePair();
            var agentConnection = AgentConnection.RunAgent(mockAgent, streams.Item1, streams.Item1);

            // Wait for connected
            var connectedResult = await Task.WhenAny(connectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(connectedTcs.Task, connectedResult, "OnClientConnected was not called within timeout");

            // Dispose to trigger disconnect
            streams.Item2.Dispose();
            streams.Item1.Dispose();
            agentConnection!.Dispose();

            // Wait for disconnected
            var disconnectedResult = await Task.WhenAny(disconnectedTcs.Task, Task.Delay(5000));
            Assert.AreEqual(disconnectedTcs.Task, disconnectedResult, "OnDisconnected was not called within timeout");

            Assert.IsTrue(connectedCalledBeforeDisconnected, "OnClientConnected should be called before OnDisconnected");
        }
    }

    /// <summary>
    /// Mock IAcpAgent that invokes a callback when OnDisconnected is called.
    /// </summary>
    internal sealed class MockAgentWithDisconnectedCallback : IAcpAgent
    {
        private readonly Action<AgentConnection>? _onDisconnectedWithConnection;
        private readonly Action? _onDisconnected;

        public MockAgentWithDisconnectedCallback(Action<AgentConnection> onDisconnectedWithConnection)
        {
            _onDisconnectedWithConnection = onDisconnectedWithConnection;
        }

        public MockAgentWithDisconnectedCallback(Action onDisconnected)
        {
            _onDisconnected = onDisconnected;
        }

        public void OnClientConnected(AgentConnection connection) { }

        public void OnDisconnected(AgentConnection connection)
        {
            _onDisconnectedWithConnection?.Invoke(connection);
            _onDisconnected?.Invoke();
        }

        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new AuthenticateResponse());

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new InitializeResponse());

        public Task CancelAsync(CancelNotification notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ListSessionsResponse> ListSessionsAsync(ListSessionsRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ListSessionsResponse { Sessions = Array.Empty<SessionInfo>() });

        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new LoadSessionResponse());

        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new NewSessionResponse());

        public Task<PromptResponse> PromptAsync(PromptRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PromptResponse());

        public Task<ResumeSessionResponse> ResumeSessionAsync(ResumeSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResumeSessionResponse());

        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(SetSessionConfigOptionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SetSessionConfigOptionResponse { ConfigOptions = Array.Empty<SessionConfigOption>() });

        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SetSessionModeResponse());

        public Task<CloseSessionResponse> CloseAsync(CloseSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new CloseSessionResponse());

        public Task<object> ExtMethodAsync(string method, object request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new object());

        public Task ExtNotificationAsync(string method, object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<LogoutResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Mock IAcpClient that invokes a callback when OnDisconnected is called.
    /// </summary>
    internal sealed class MockClientWithDisconnectedCallback : IAcpClient
    {
        private readonly Action<ClientConnection>? _onDisconnectedWithConnection;
        private readonly Action? _onDisconnected;

        public MockClientWithDisconnectedCallback(Action<ClientConnection> onDisconnectedWithConnection)
        {
            _onDisconnectedWithConnection = onDisconnectedWithConnection;
        }

        public MockClientWithDisconnectedCallback(Action onDisconnected)
        {
            _onDisconnected = onDisconnected;
        }

        public void OnDisconnected(ClientConnection connection)
        {
            _onDisconnectedWithConnection?.Invoke(connection);
            _onDisconnected?.Invoke();
        }

        public Task<ReadTextFileResponse> ReadTextFileAsync(ReadTextFileRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ReadTextFileResponse { Content = "" });

        public Task<WriteTextFileResponse> WriteTextFileAsync(WriteTextFileRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new WriteTextFileResponse());

        public Task<RequestPermissionResponse> RequestPermissionAsync(RequestPermissionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RequestPermissionResponse { Outcome = new RequestPermissionOutcomeCancelled() });

        public Task SessionUpdateAsync(SessionNotification notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<CreateTerminalResponse> CreateTerminalAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new CreateTerminalResponse { TerminalId = "" });

        public Task<KillTerminalResponse> KillTerminalAsync(KillTerminalRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new KillTerminalResponse());

        public Task<TerminalOutputResponse> TerminalOutputAsync(TerminalOutputRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new TerminalOutputResponse { Output = "" });

        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(ReleaseTerminalRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ReleaseTerminalResponse());

        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(WaitForTerminalExitRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new WaitForTerminalExitResponse());

        public Task<object> ExtMethodAsync(string method, object request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new object());

        public Task ExtNotificationAsync(string method, object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    /// <summary>
    /// Mock IAcpAgent with callbacks for both OnClientConnected and OnDisconnected.
    /// </summary>
    internal sealed class MockAgentWithCallbacks : IAcpAgent
    {
        private readonly Action? _onConnected;
        private readonly Action? _onDisconnected;

        public MockAgentWithCallbacks(Action? onConnected = null, Action? onDisconnected = null)
        {
            _onConnected = onConnected;
            _onDisconnected = onDisconnected;
        }

        public void OnClientConnected(AgentConnection connection)
        {
            _onConnected!.Invoke();
        }

        public void OnDisconnected(AgentConnection connection)
        {
            _onDisconnected!.Invoke();
        }

        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new AuthenticateResponse());

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new InitializeResponse());

        public Task CancelAsync(CancelNotification notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<ListSessionsResponse> ListSessionsAsync(ListSessionsRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ListSessionsResponse { Sessions = Array.Empty<SessionInfo>() });

        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new LoadSessionResponse());

        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new NewSessionResponse());

        public Task<PromptResponse> PromptAsync(PromptRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PromptResponse());

        public Task<ResumeSessionResponse> ResumeSessionAsync(ResumeSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResumeSessionResponse());

        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(SetSessionConfigOptionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SetSessionConfigOptionResponse { ConfigOptions = Array.Empty<SessionConfigOption>() });

        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SetSessionModeResponse());

        public Task<CloseSessionResponse> CloseAsync(CloseSessionRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new CloseSessionResponse());

        public Task<object> ExtMethodAsync(string method, object request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new object());

        public Task ExtNotificationAsync(string method, object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<LogoutResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
