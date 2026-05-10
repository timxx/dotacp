// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.12.2

using dotacp.protocol.unstable;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client.unstable
{
    /// <summary>
    /// Defines the methods an ACP client implementation must provide to handle protocol calls from an agent.
    /// </summary>
    public interface IAcpClient
    {
        /// <summary>
        /// Called when the RPC connection is disconnected.
        /// </summary>
        /// <param name="connection">The connection that was disconnected.</param>
        void OnDisconnected(Connection connection);

        /// <summary>
        /// Handles the protocol <c>elicitation/complete</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task CompleteAsync(CompleteElicitationNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>elicitation/create</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<CreateElicitationResponse> CreateAsync(CreateElicitationRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>fs/read_text_file</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ReadTextFileResponse> ReadTextFileAsync(ReadTextFileRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>fs/write_text_file</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<WriteTextFileResponse> WriteTextFileAsync(WriteTextFileRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/request_permission</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<RequestPermissionResponse> RequestPermissionAsync(RequestPermissionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/update</c> notification.
        /// </summary>
        /// <param name="notification">The notification payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task SessionUpdateAsync(SessionNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/create</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<CreateTerminalResponse> CreateTerminalAsync(CreateTerminalRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/kill</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<KillTerminalResponse> KillTerminalAsync(KillTerminalRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/output</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<TerminalOutputResponse> TerminalOutputAsync(TerminalOutputRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/release</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<ReleaseTerminalResponse> ReleaseTerminalAsync(ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/wait_for_exit</c> request.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The response.</returns>
        Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(WaitForTerminalExitRequest request,
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
        /// <param name="notification">The extension notification payload.</param>
        /// <param name="cancellationToken">A token that cancels notification handling.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default);
    }
}
