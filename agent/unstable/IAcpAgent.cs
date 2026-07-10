// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/schema-v1.19.0

using dotacp.protocol.unstable;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent.unstable
{
    /// <summary>
    /// Defines the methods an ACP agent implementation must provide to handle protocol requests.
    /// </summary>
    public interface IAcpAgent
    {
        /// <summary>
        /// Called after the RPC connection is established.
        /// </summary>
        /// <param name="connection">The active connection that can be used for outbound calls to the client.</param>
        void OnClientConnected(Connection connection);

        /// <summary>
        /// Called when the RPC connection is disconnected.
        /// </summary>
        /// <param name="connection">The connection that was disconnected.</param>
        void OnDisconnected(Connection connection);

        /// <summary>
        /// Handles the protocol <c>authenticate</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>document/didChange</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task DidchangeAsync(DidChangeDocumentNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>document/didClose</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task DidcloseAsync(DidCloseDocumentNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>document/didFocus</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task DidfocusAsync(DidFocusDocumentNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>document/didOpen</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task DidopenAsync(DidOpenDocumentNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>document/didSave</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task DidsaveAsync(DidSaveDocumentNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>initialize</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>logout</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<LogoutResponse> LogoutAsync(LogoutRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>nes/accept</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task AcceptAsync(AcceptNesNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>nes/close</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<CloseNesResponse> CloseAsync(CloseNesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>nes/reject</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task RejectAsync(RejectNesNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>nes/start</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<StartNesResponse> StartAsync(StartNesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>nes/suggest</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<SuggestNesResponse> SuggestAsync(SuggestNesRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>providers/disable</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<DisableProviderResponse> DisableAsync(DisableProviderRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>providers/list</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ListProvidersResponse> ListAsync(ListProvidersRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>providers/set</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<SetProviderResponse> SetAsync(SetProviderRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/cancel</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task CancelAsync(CancelNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/close</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<CloseSessionResponse> CloseAsync(CloseSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/delete</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<DeleteSessionResponse> DeleteAsync(DeleteSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/fork</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ForkSessionResponse> ForkSessionAsync(ForkSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/list</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ListSessionsResponse> ListSessionsAsync(ListSessionsRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/load</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/new</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/prompt</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/resume</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ResumeSessionResponse> ResumeSessionAsync(ResumeSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/set_config_option</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/set_mode</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles an extension method call that is not part of the core protocol.
        /// </summary>
        /// <param name="method">The extension method name.</param>
        /// <param name="request">The extension request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The extension method response object.</returns>
        Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles an extension notification that is not part of the core protocol.
        /// </summary>
        /// <param name="method">The extension notification name.</param>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels notification handling.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default);
    }
}
