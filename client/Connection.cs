// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.11.0

using dotacp.protocol;
using dotacp.shared;
using StreamJsonRpc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    /// <summary>
    /// Manages a JSON-RPC connection between an ACP client and an ACP agent.
    /// The client can use this connection to communicate with the Agent.
    /// </summary>
    public class Connection : IDisposable
    {
        private JsonRpc _rpc;

        /// <summary>
        /// Gets a task that completes when the underlying RPC channel is closed.
        /// </summary>
        public Task Completion => _rpc.Completion;

        private Connection(IAcpClient client, Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            var handler = new NewLineDelimitedMessageHandler(
                inputStream, outputStream, new JsonMessageFormatter());
            var routingHandler = new ExtensionMethodRoutingMessageHandler(handler);
            _rpc = new JsonRpc(routingHandler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

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
        /// <param name="client">The client implementation that handles incoming RPC calls.</param>
        /// <param name="inputStream">The (agent) input stream to write to.</param>
        /// <param name="outputStream">The (agent) output stream to read from.</param>
        /// <param name="traceSource">Optional trace source used for StreamJsonRpc diagnostics.</param>
        /// <returns>
        /// A running <see cref="Connection"/> instance, or <see langword="null"/> when a required argument is <see langword="null"/>.
        /// </returns>
        public static Connection? RunClient(IAcpClient client,
            Stream inputStream, Stream outputStream,
            TraceSource? traceSource = null)
        {
            if (client == null || inputStream == null || outputStream == null)
                return null;

            return new Connection(client, inputStream, outputStream, traceSource);
        }

        /// <summary>
        /// Calls the agent <c>authenticate</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<AuthenticateResponse> AuthenticateAsync(
            AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<AuthenticateRequest, AuthenticateResponse>(
                AgentMethods.Authenticate, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>initialize</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<InitializeResponse> InitializeAsync(
            InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<InitializeRequest, InitializeResponse>(
                AgentMethods.Initialize, request, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>session/cancel</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task CancelAsync(
            CancelNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.SessionCancel, notification, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/fork</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ForkSessionResponse> ForkSessionAsync(
            ForkSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ForkSessionRequest, ForkSessionResponse>(
                AgentMethods.SessionFork, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/list</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ListSessionsResponse> ListSessionsAsync(
            ListSessionsRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ListSessionsRequest, ListSessionsResponse>(
                AgentMethods.SessionList, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/load</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<LoadSessionResponse> LoadSessionAsync(
            LoadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<LoadSessionRequest, LoadSessionResponse>(
                AgentMethods.SessionLoad, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/new</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<NewSessionResponse> NewSessionAsync(
            NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<NewSessionRequest, NewSessionResponse>(
                AgentMethods.SessionNew, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/prompt</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<PromptResponse> PromptAsync(
            PromptRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<PromptRequest, PromptResponse>(
                AgentMethods.SessionPrompt, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/resume</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ResumeSessionResponse> ResumeSessionAsync(
            ResumeSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ResumeSessionRequest, ResumeSessionResponse>(
                AgentMethods.SessionResume, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/set_config_option</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionConfigOptionRequest, SetSessionConfigOptionResponse>(
                AgentMethods.SessionSetConfigOption, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/set_mode</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<SetSessionModeResponse> SetSessionModeAsync(
            SetSessionModeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionModeRequest, SetSessionModeResponse>(
                AgentMethods.SessionSetMode, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/set_model</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<SetSessionModelResponse> SetSessionModelAsync(
            SetSessionModelRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetSessionModelRequest, SetSessionModelResponse>(
                AgentMethods.SessionSetModel, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/stop</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<StopSessionResponse> StopAsync(
            StopSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<StopSessionRequest, StopSessionResponse>(
                AgentMethods.SessionStop, request, cancellationToken);
        }

        /// <summary>
        /// Calls an agent extension method.
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
        /// Sends an agent extension notification.
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
