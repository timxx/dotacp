using dotacp.client;
using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

namespace clientcli
{
    internal class Client : IAcpClient
    {
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task SessionUpdateAsync(SessionNotification notification, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
