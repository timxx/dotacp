using dotacp.protocol;
using StreamJsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    internal sealed class ClientRpcTarget
    {
        private readonly IAcpClient _client;

        public ClientRpcTarget(IAcpClient client)
        {
            _client = client;
        }

        [JsonRpcMethod(ClientMethods.SessionRequestPermission, UseSingleObjectParameterDeserialization = true)]
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.RequestPermissionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.SessionUpdate, UseSingleObjectParameterDeserialization = true)]
        public Task SessionUpdateAsync(SessionNotification notification,
            CancellationToken cancellationToken = default)
        {
            return _client.SessionUpdateAsync(notification, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.FsWriteTextFile, UseSingleObjectParameterDeserialization = true)]
        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.WriteTextFileAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.FsReadTextFile, UseSingleObjectParameterDeserialization = true)]
        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ReadTextFileAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalCreate, UseSingleObjectParameterDeserialization = true)]
        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.CreateTerminalAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalOutput, UseSingleObjectParameterDeserialization = true)]
        public Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.TerminalOutputAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalRelease, UseSingleObjectParameterDeserialization = true)]
        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ReleaseTerminalAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalWaitForExit, UseSingleObjectParameterDeserialization = true)]
        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.WaitForTerminalExitAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalKill, UseSingleObjectParameterDeserialization = true)]
        public Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.KillTerminalCommandAsync(request, cancellationToken);
        }
    }
}
