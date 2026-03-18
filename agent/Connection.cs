// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.11.2

using dotacp.protocol;
using dotacp.shared;
using StreamJsonRpc;
using System;
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
    public class Connection : IDisposable
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
            var routingHandler = new ExtensionMethodRoutingMessageHandler(handler);
            _rpc = new JsonRpcEx(routingHandler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            _rpc.AddLocalRpcTarget(new AgentRpcTarget(agent));
            _rpc.StartListening();

            _rpc.Disconnected += (sender, e) => agent.OnDisconnected(this);

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
        /// Calls the client <c>fs/read_text_file</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ReadTextFileRequest, ReadTextFileResponse>(
                ClientMethods.FsReadTextFile, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>fs/write_text_file</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<WriteTextFileRequest, WriteTextFileResponse>(
                ClientMethods.FsWriteTextFile, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>session/request_permission</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<RequestPermissionRequest, RequestPermissionResponse>(
                ClientMethods.SessionRequestPermission, request, cancellationToken);
        }

        /// <summary>
        /// Sends the client <c>session/update</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task SessionUpdateAsync(
            SessionNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(ClientMethods.SessionUpdate, notification, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>terminal/create</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<CreateTerminalRequest, CreateTerminalResponse>(
                ClientMethods.TerminalCreate, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>terminal/kill</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<KillTerminalResponse> KillTerminalAsync(
            KillTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<KillTerminalRequest, KillTerminalResponse>(
                ClientMethods.TerminalKill, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>terminal/output</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<TerminalOutputResponse> TerminalOutputAsync(
            TerminalOutputRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TerminalOutputRequest, TerminalOutputResponse>(
                ClientMethods.TerminalOutput, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>terminal/release</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ReleaseTerminalRequest, ReleaseTerminalResponse>(
                ClientMethods.TerminalRelease, request, cancellationToken);
        }

        /// <summary>
        /// Calls the client <c>terminal/wait_for_exit</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<WaitForTerminalExitRequest, WaitForTerminalExitResponse>(
                ClientMethods.TerminalWaitForExit, request, cancellationToken);
        }

        /// <summary>
        /// Calls a client extension method.
        /// </summary>
        /// <param name="method">The extension method name.</param>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response object.</returns>
        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<object, object>(
                "_" + method, request, cancellationToken);
        }

        /// <summary>
        /// Sends a client extension notification.
        /// </summary>
        /// <param name="method">The extension notification name.</param>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(
                "_" + method, notification, cancellationToken);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        public void Dispose()
        {
            _rpc.Dispose();
        }
    }
}
