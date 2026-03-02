using dotacp.protocol;
using StreamJsonRpc;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent
{
    public class Connection
    {
        private JsonRpc _rpc;

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
        public Task SessionUpdateAsync(SessionNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(ClientMethods.SessionUpdate, notification, cancellationToken);
        }

        /// <summary>
        /// Requests the client to write text to a file.
        /// </summary>
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
        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<object, object>(method, request, cancellationToken);
        }

        /// <summary>
        /// Sends an extended notification to the client.
        /// </summary>
        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(method, notification, cancellationToken);
        }
    }
}
