// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.11.2

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
        /// Handles the protocol <c>authenticate</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
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
        /// Handles the protocol <c>session/set_model</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<SetSessionModelResponse> SetSessionModelAsync(SetSessionModelRequest request,
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
