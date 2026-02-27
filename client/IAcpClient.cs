using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    public interface IAcpClient
    {
        Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default);

        Task SessionUpdateAsync(
            SessionNotification notification,
            CancellationToken cancellationToken = default);

        Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default);

        Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default);

        Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default);

        Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request,
            CancellationToken cancellationToken = default);

        Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default);

        Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request,
            CancellationToken cancellationToken = default);

        Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request,
            CancellationToken cancellationToken = default);

        Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default);

        Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default);
    }
}
