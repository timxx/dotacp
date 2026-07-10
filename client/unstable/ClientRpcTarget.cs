// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/schema-v1.19.0

using dotacp.protocol.unstable;
using dotacp.shared;
using StreamJsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client.unstable
{
    internal sealed class ClientRpcTarget
    {
        private readonly IAcpClient _client;

        public ClientRpcTarget(IAcpClient client)
        {
            _client = client;
        }

        [JsonRpcMethod(ClientMethods.ElicitationComplete, UseSingleObjectParameterDeserialization = true)]
        public Task CompleteAsync(
            CompleteElicitationNotification notification,
            CancellationToken cancellationToken = default)
        {
            return _client.CompleteAsync(notification, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.ElicitationCreate, UseSingleObjectParameterDeserialization = true)]
        public Task<CreateElicitationResponse> CreateAsync(
            CreateElicitationRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.CreateAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.FsReadTextFile, UseSingleObjectParameterDeserialization = true)]
        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ReadTextFileAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.FsWriteTextFile, UseSingleObjectParameterDeserialization = true)]
        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.WriteTextFileAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.McpConnect, UseSingleObjectParameterDeserialization = true)]
        public Task<ConnectMcpResponse> ConnectAsync(
            ConnectMcpRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ConnectAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.McpDisconnect, UseSingleObjectParameterDeserialization = true)]
        public Task<DisconnectMcpResponse> DisconnectAsync(
            DisconnectMcpRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.DisconnectAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.SessionRequestPermission, UseSingleObjectParameterDeserialization = true)]
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.RequestPermissionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.SessionUpdate, UseSingleObjectParameterDeserialization = true)]
        public Task SessionUpdateAsync(
            SessionNotification notification,
            CancellationToken cancellationToken = default)
        {
            return _client.SessionUpdateAsync(notification, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalCreate, UseSingleObjectParameterDeserialization = true)]
        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.CreateTerminalAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalKill, UseSingleObjectParameterDeserialization = true)]
        public Task<KillTerminalResponse> KillTerminalAsync(
            KillTerminalRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.KillTerminalAsync(request, cancellationToken);
        }

        [JsonRpcMethod(ClientMethods.TerminalOutput, UseSingleObjectParameterDeserialization = true)]
        public Task<TerminalOutputResponse> TerminalOutputAsync(
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

        [JsonRpcMethod("__acp_ext_method__", UseSingleObjectParameterDeserialization = true)]
        public Task<object> HandleExtensionMethodAsync(
            ExtensionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ExtMethodAsync(request.Method, request.Arguments, cancellationToken);
        }

        [JsonRpcMethod("__acp_ext_notification__", UseSingleObjectParameterDeserialization = true)]
        public Task HandleExtensionNotificationAsync(
            ExtensionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _client.ExtNotificationAsync(request.Method, request.Arguments, cancellationToken);
        }
    }
}
