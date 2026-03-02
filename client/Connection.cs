using dotacp.protocol;
using StreamJsonRpc;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    /// <summary>
    /// Manages a JSON-RPC connection between an ACP client and an ACP agent.
    /// The client can use this connection to communicate with the Agent so it behaves like an Agent.
    /// </summary>
    public class Connection
    {
        private JsonRpc _rpc;

        private Connection(IAcpClient client, Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            var handler = new NewLineDelimitedMessageHandler(
                inputStream, outputStream, new JsonMessageFormatter());
            _rpc = new JsonRpc(handler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            // TODO: handle ext methods and notifications
            // don't known what acp agents will need yet
            _rpc.AddLocalRpcTarget(new ClientRpcTarget(client));
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
        /// <param name="client">The client implementation that handles inbound agent calls.</param>
        /// <param name="inputStream">The (agent) input stream to write to.</param>
        /// <param name="outputStream">The (agent) output stream to read from.</param>
        /// <param name="traceSource">Optional trace source used for StreamJsonRpc diagnostics.</param>
        /// <returns>
        /// A running <see cref="Connection"/> instance, or <see langword="null"/> when a required argument is <see langword="null"/>.
        /// </returns>
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
        /// <param name="request">The initialization request with protocol version and client capabilities.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The initialization response from the agent.</returns>
        public Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<InitializeRequest, InitializeResponse>(
                AgentMethods.Initialize, request, cancellationToken);
        }

        /// <summary>
        /// Authenticates a user using the specified authentication request.
        /// </summary>
        /// <param name="request">The authentication request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The authentication response from the agent.</returns>
        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<AuthenticateRequest, AuthenticateResponse>(
                AgentMethods.Authenticate, request, cancellationToken);
        }

        /// <summary>
        /// Creates a new session using the specified session parameters.
        /// </summary>
        /// <param name="request">The session creation request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The new session response from the agent.</returns>
        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<NewSessionRequest, NewSessionResponse>(
                AgentMethods.SessionNew, request, cancellationToken);
        }

        /// <summary>
        /// Loads an existing session using the specified session parameters.
        /// </summary>
        /// <param name="request">The session load request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The loaded session response from the agent.</returns>
        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<LoadSessionRequest, LoadSessionResponse>(
                AgentMethods.SessionLoad, request, cancellationToken);
        }

        /// <summary>
        /// Sends a user prompt to the agent for the specified session.
        /// </summary>
        /// <param name="request">The prompt request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The prompt response from the agent.</returns>
        public Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<PromptRequest, PromptResponse>(
                AgentMethods.SessionPrompt, request, cancellationToken);
        }

        /// <summary>
        /// Sets a session configuration option for the specified session.
        /// </summary>
        /// <param name="request">The config option update request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The update response from the agent.</returns>
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
        /// <param name="request">The session mode change request.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The mode change response from the agent.</returns>
        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionModeRequest, SetSessionModeResponse>(
                AgentMethods.SessionSetMode, request, cancellationToken);
        }

        /// <summary>
        /// Notifies the agent to cancel ongoing operations for a session.
        /// </summary>
        /// <param name="notification">The cancellation notification.</param>
        /// <param name="cancellationToken">A token that cancels dispatch before send.</param>
        /// <returns>A task that completes after the notification is queued for transport.</returns>
        public Task CancelAsync(CancelNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.SessionCancel, notification, cancellationToken);
        }

        /// <summary>
        /// Calls an extension method on the agent that is not part of the core protocol.
        /// </summary>
        /// <param name="method">The extension method name.</param>
        /// <param name="request">The request object for the extension method.</param>
        /// <param name="cancellationToken">A token that cancels the request.</param>
        /// <returns>The extension method response object.</returns>
        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<object, object>(method, request, cancellationToken);
        }

        /// <summary>
        /// Sends an extension notification to the agent that is not part of the core protocol.
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
