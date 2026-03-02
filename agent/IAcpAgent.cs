using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent
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
        /// Handles the protocol <c>initialize</c> request.
        /// </summary>
        /// <param name="request">The client initialization request containing version and capabilities.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The initialization response with the negotiated version and agent capabilities.</returns>
        Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>authenticate</c> request.
        /// </summary>
        /// <param name="request">The authentication request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The authentication result.</returns>
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/new</c> request.
        /// </summary>
        /// <param name="request">The session creation request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The new session response.</returns>
        Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/load</c> request.
        /// </summary>
        /// <param name="request">The session load request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The loaded session response.</returns>
        Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/prompt</c> request.
        /// </summary>
        /// <param name="request">The prompt request for an existing session.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The prompt response.</returns>
        Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/set_config_option</c> request.
        /// </summary>
        /// <param name="request">The config option update request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The update result.</returns>
        Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/set_mode</c> request.
        /// </summary>
        /// <param name="request">The session mode change request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The mode change result.</returns>
        Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/cancel</c> notification.
        /// </summary>
        /// <param name="notification">The cancellation notification.</param>
        /// <param name="cancellationToken">A token that cancels notification handling.</param>
        /// <returns>A task that completes when cancellation handling is finished.</returns>
        Task CancelAsync(CancelNotification notification,
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
