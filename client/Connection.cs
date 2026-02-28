using dotacp.protocol;
using StreamJsonRpc;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    public class Connection
    {
        private JsonRpc _rpc;
        private IAcpClient _client;

        private Connection(IAcpClient client, Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            _client = client;

            var handler = new NewLineDelimitedMessageHandler(
                inputStream, outputStream, new JsonMessageFormatter());
            _rpc = new JsonRpc(handler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            _rpc.StartListening();
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
        /// Create a Connection to an ACP agent over the given streams.
        /// </summary>
        public static Connection? ConnectToAgent(IAcpClient client,
            Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            if (client == null || inputStream == null || outputStream == null)
                return null;

            return new Connection(client, inputStream, outputStream, traceSource);
        }

        /// <summary>
        /// Initializes the agent using the specified initialization parameters.
        /// </summary>
        public Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<InitializeRequest, InitializeResponse>(
                AgentMethods.Initialize, request, cancellationToken);
        }

        /// <summary>
        /// Authenticates a user using the specified authentication request.
        /// </summary>
        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<AuthenticateRequest, AuthenticateResponse>(
                AgentMethods.Authenticate, request, cancellationToken);
        }

        /// <summary>
        /// Creates a new session using the specified session parameters.
        /// </summary>
        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<NewSessionRequest, NewSessionResponse>(
                AgentMethods.SessionNew, request, cancellationToken);
        }

        /// <summary>
        /// Loads an existing session using the specified session parameters.
        /// </summary>
        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<LoadSessionRequest, LoadSessionResponse>(
                AgentMethods.SessionLoad, request, cancellationToken);
        }

        /// <summary>
        /// Sends a user prompt to the agent for the specified session.
        /// </summary>
        public Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<PromptRequest, PromptResponse>(
                AgentMethods.SessionPrompt, request, cancellationToken);
        }

        /// <summary>
        /// Sets a session configuration option for the specified session.
        /// </summary>
        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionConfigOptionRequest, SetSessionConfigOptionResponse>(
                AgentMethods.SessionSetConfigOption, request, cancellationToken);
        }

        /// <summary>
        /// Sets the session mode for the specified session.
        /// </summary>
        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionModeRequest, SetSessionModeResponse>(
                AgentMethods.SessionSetMode, request, cancellationToken);
        }

        /// <summary>
        /// Notifies the agent to cancel ongoing operations for a session.
        /// </summary>
        public Task CancelAsync(CancelNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.SessionCancel, notification, cancellationToken);
        }

        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<object, object>(method, request, cancellationToken);
        }

        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(method, notification, cancellationToken);
        }
    }
}
