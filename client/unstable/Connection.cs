// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/schema-v1.19.0

using dotacp.protocol.unstable;
using dotacp.shared;
using StreamJsonRpc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client.unstable
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
            _rpc = new JsonRpcEx(routingHandler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            _rpc.AddLocalRpcTarget(new ClientRpcTarget(client));
            _rpc.StartListening();

            _rpc.Disconnected += (sender, e) => client.OnDisconnected(this);
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
        /// Sends the agent <c>document/didChange</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task DidchangeAsync(
            DidChangeDocumentNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.DocumentDidChange, notification, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>document/didClose</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task DidcloseAsync(
            DidCloseDocumentNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.DocumentDidClose, notification, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>document/didFocus</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task DidfocusAsync(
            DidFocusDocumentNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.DocumentDidFocus, notification, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>document/didOpen</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task DidopenAsync(
            DidOpenDocumentNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.DocumentDidOpen, notification, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>document/didSave</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task DidsaveAsync(
            DidSaveDocumentNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.DocumentDidSave, notification, cancellationToken);
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
        /// Calls the agent <c>logout</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<LogoutResponse> LogoutAsync(
            LogoutRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<LogoutRequest, LogoutResponse>(
                AgentMethods.Logout, request, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>nes/accept</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task AcceptAsync(
            AcceptNesNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.NesAccept, notification, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>nes/close</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<CloseNesResponse> CloseAsync(
            CloseNesRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<CloseNesRequest, CloseNesResponse>(
                AgentMethods.NesClose, request, cancellationToken);
        }

        /// <summary>
        /// Sends the agent <c>nes/reject</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>A task that completes when the notification is sent.</returns>
        public Task RejectAsync(
            RejectNesNotification notification,
            CancellationToken cancellationToken = default)
        {
            return SendNotificationAsync(AgentMethods.NesReject, notification, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>nes/start</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<StartNesResponse> StartAsync(
            StartNesRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<StartNesRequest, StartNesResponse>(
                AgentMethods.NesStart, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>nes/suggest</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<SuggestNesResponse> SuggestAsync(
            SuggestNesRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SuggestNesRequest, SuggestNesResponse>(
                AgentMethods.NesSuggest, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>providers/disable</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<DisableProviderResponse> DisableAsync(
            DisableProviderRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<DisableProviderRequest, DisableProviderResponse>(
                AgentMethods.ProvidersDisable, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>providers/list</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<ListProvidersResponse> ListAsync(
            ListProvidersRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<ListProvidersRequest, ListProvidersResponse>(
                AgentMethods.ProvidersList, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>providers/set</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<SetProviderResponse> SetAsync(
            SetProviderRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<SetProviderRequest, SetProviderResponse>(
                AgentMethods.ProvidersSet, request, cancellationToken);
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
        /// Calls the agent <c>session/close</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<CloseSessionResponse> CloseAsync(
            CloseSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<CloseSessionRequest, CloseSessionResponse>(
                AgentMethods.SessionClose, request, cancellationToken);
        }

        /// <summary>
        /// Calls the agent <c>session/delete</c> method.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels the operation.</param>
        /// <returns>The response.</returns>
        public Task<DeleteSessionResponse> DeleteAsync(
            DeleteSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<DeleteSessionRequest, DeleteSessionResponse>(
                AgentMethods.SessionDelete, request, cancellationToken);
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
