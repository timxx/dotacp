using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    /// <summary>
    /// Defines the methods an ACP client implementation must provide to handle protocol calls from an agent.
    /// </summary>
    public interface IAcpClient
    {
        /// <summary>
        /// Handles the protocol <c>session/request_permission</c> request.
        /// </summary>
        /// <param name="request">The permission request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The permission response.</returns>
        Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>session/update</c> notification.
        /// </summary>
        /// <param name="notification">The session update notification payload.</param>
        /// <param name="cancellationToken">A token that cancels notification handling.</param>
        /// <returns>A task that completes when handling is finished.</returns>
        Task SessionUpdateAsync(
            SessionNotification notification,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>fs/write_text_file</c> request.
        /// </summary>
        /// <param name="request">The write-text-file request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The write operation response.</returns>
        Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>fs/read_text_file</c> request.
        /// </summary>
        /// <param name="request">The read-text-file request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The read operation response.</returns>
        Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/create</c> request.
        /// </summary>
        /// <param name="request">The create-terminal request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The terminal creation response.</returns>
        Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/output</c> request.
        /// </summary>
        /// <param name="request">The terminal output request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The terminal output response.</returns>
        Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/release</c> request.
        /// </summary>
        /// <param name="request">The release-terminal request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The terminal release response.</returns>
        Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/wait_for_exit</c> request.
        /// </summary>
        /// <param name="request">The wait-for-exit request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The terminal exit response.</returns>
        Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles the protocol <c>terminal/kill</c> request.
        /// </summary>
        /// <param name="request">The kill-terminal request.</param>
        /// <param name="cancellationToken">A token that cancels request processing.</param>
        /// <returns>The kill operation response.</returns>
        Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request,
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
