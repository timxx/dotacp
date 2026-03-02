using dotacp.protocol;
using StreamJsonRpc;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent
{
    /// <summary>
    /// Manages a JSON-RPC connection between an ACP agent and an ACP client.
    /// The agent can use this connection to communicate with the Client so it behaves like a Client.
    /// </summary>
    public class Connection
    {
        private JsonRpc _rpc;

        /// <summary>
        /// Gets a task that completes when the underlying RPC channel is closed.
        /// </summary>
        public Task Completion => _rpc.Completion;

        private Connection(IAcpAgent agent, Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            var handler = new NewLineDelimitedMessageHandler(
                inputStream, outputStream, new JsonMessageFormatter());
            _rpc = new JsonRpc(handler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            _rpc.AddLocalRpcTarget(new AgentRpcTarget(agent));
            _rpc.StartListening();

            agent.OnClientConnected(this);
        }

        private Task<TResponse> SendRequestAsync<TRequest, TResponse>(
            string method, TRequest request, CancellationToken cancellationToken)
        {
            return _rpc.InvokeWithParameterObjectAsync<TResponse>(
                method, request, cancellationToken);
        }

        private Task SendNotificationAsync<TNotification>(
            string method, TNotification notification, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _rpc.NotifyWithParameterObjectAsync(method, notification);
        }

        /// <summary>
        /// Create a Connection to an ACP client over the given streams.
        /// </summary>
        /// <param name="agent">The agent implementation that handles incoming RPC calls.</param>
        /// <param name="inputStream">The (client) input stream to write to.</param>
        /// <param name="outputStream">The (client) output stream to read from.</param>
        /// <param name="traceSource">Optional trace source used for StreamJsonRpc diagnostics.</param>
        /// <returns>
        /// A running <see cref="Connection"/> instance, or <see langword="null"/> when a required argument is <see langword="null"/>.
        /// </returns>
        public static Connection? RunAgent(IAcpAgent agent,
            Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            if (agent == null || inputStream == null || outputStream == null)
                return null;

            return new Connection(agent, inputStream, outputStream, traceSource);
        }

        /// <summary>
        /// Requests permission from the client to perform an action.
        /// </summary>
        /// <param name="request">The permission request payload.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The client's permission decision.</returns>
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<RequestPermissionRequest, RequestPermissionResponse>(
                ClientMethods.SessionRequestPermission, request, cancellationToken);
        }

        /// <summary>
        /// Sends a session update notification to the client.
        /// </summary>
        /// <param name="notification">The update payload to send.</param>
        /// <param name="cancellationToken">A token that cancels the send operation before dispatch.</param>
        /// <returns>A task that completes after the notification is queued for transport.</returns>
        public Task SessionUpdateAsync(SessionNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(ClientMethods.SessionUpdate, notification, cancellationToken);
        }

        /// <summary>
        /// Requests the client to write text to a file.
        /// </summary>
        /// <param name="request">The file write request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The write result returned by the client.</returns>
        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<WriteTextFileRequest, WriteTextFileResponse>(
                ClientMethods.FsWriteTextFile, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to read text from a file.
        /// </summary>
        /// <param name="request">The file read request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The file content response returned by the client.</returns>
        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ReadTextFileRequest, ReadTextFileResponse>(
                ClientMethods.FsReadTextFile, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to create a terminal.
        /// </summary>
        /// <param name="request">The terminal creation request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The terminal creation response.</returns>
        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<CreateTerminalRequest, CreateTerminalResponse>(
                ClientMethods.TerminalCreate, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to return terminal output.
        /// </summary>
        /// <param name="request">The terminal output request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The terminal output response.</returns>
        public Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TerminalOutputRequest, TerminalOutputRequest>(
                ClientMethods.TerminalOutput, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to release a terminal.
        /// </summary>
        /// <param name="request">The terminal release request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The terminal release response.</returns>
        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ReleaseTerminalRequest, ReleaseTerminalResponse>(
                ClientMethods.TerminalRelease, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to wait for terminal exit.
        /// </summary>
        /// <param name="request">The wait-for-exit request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The terminal exit status response.</returns>
        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<WaitForTerminalExitRequest, WaitForTerminalExitResponse>(
                ClientMethods.TerminalWaitForExit, request, cancellationToken);
        }

        /// <summary>
        /// Requests the client to kill a terminal command.
        /// </summary>
        /// <param name="request">The terminal kill request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The terminal kill response.</returns>
        public Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<KillTerminalCommandRequest, KillTerminalCommandResponse>(
                ClientMethods.TerminalKill, request, cancellationToken);
        }

        /// <summary>
        /// Calls an extended method on the client.
        /// </summary>
        /// <param name="method">The extension method name.</param>
        /// <param name="request">The request object for the extension method.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The response object returned by the client extension method.</returns>
        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<object, object>(method, request, cancellationToken);
        }

        /// <summary>
        /// Sends an extended notification to the client.
        /// </summary>
        /// <param name="method">The extension notification name.</param>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels dispatch before send.</param>
        /// <returns>A task that completes after the notification is queued for transport.</returns>
        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(method, notification, cancellationToken);
        }
    }
}
