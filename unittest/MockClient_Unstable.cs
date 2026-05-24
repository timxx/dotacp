using dotacp.client.unstable;
using dotacp.protocol.unstable;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Mock IAcpClient (unstable) that captures received requests and returns configured responses.
    /// </summary>
    internal sealed class MockClient_Unstable : IAcpClient
    {
        // Captured requests
        public CompleteElicitationNotification? LastCompleteElicitationNotification { get; private set; }
        public CreateElicitationRequest? LastCreateElicitationRequest { get; private set; }
        public ReadTextFileRequest? LastReadTextFileRequest { get; private set; }
        public WriteTextFileRequest? LastWriteTextFileRequest { get; private set; }
        public RequestPermissionRequest? LastRequestPermissionRequest { get; private set; }
        public SessionNotification? LastSessionNotification { get; private set; }
        public CreateTerminalRequest? LastCreateTerminalRequest { get; private set; }
        public KillTerminalRequest? LastKillTerminalRequest { get; private set; }
        public TerminalOutputRequest? LastTerminalOutputRequest { get; private set; }
        public ReleaseTerminalRequest? LastReleaseTerminalRequest { get; private set; }
        public WaitForTerminalExitRequest? LastWaitForTerminalExitRequest { get; private set; }
        public string? LastExtMethodName { get; private set; }
        public object? LastExtMethodRequest { get; private set; }
        public string? LastExtNotificationName { get; private set; }
        public object? LastExtNotificationPayload { get; private set; }

        // Responses to return
        public CreateElicitationResponse CreateElicitationResponseToReturn { get; set; } = new CreateElicitationResponseCancel();
        public ReadTextFileResponse ReadTextFileResponseToReturn { get; set; } = new ReadTextFileResponse { Content = "" };
        public WriteTextFileResponse WriteTextFileResponseToReturn { get; set; } = new WriteTextFileResponse();
        public RequestPermissionResponse RequestPermissionResponseToReturn { get; set; } = new RequestPermissionResponse { Outcome = new RequestPermissionOutcomeCancelled() };
        public CreateTerminalResponse CreateTerminalResponseToReturn { get; set; } = new CreateTerminalResponse { TerminalId = "" };
        public KillTerminalResponse KillTerminalResponseToReturn { get; set; } = new KillTerminalResponse();
        public TerminalOutputResponse TerminalOutputResponseToReturn { get; set; } = new TerminalOutputResponse { Output = "" };
        public ReleaseTerminalResponse ReleaseTerminalResponseToReturn { get; set; } = new ReleaseTerminalResponse();
        public WaitForTerminalExitResponse WaitForTerminalExitResponseToReturn { get; set; } = new WaitForTerminalExitResponse();
        public object ExtMethodResponseToReturn { get; set; } = new object();

        // Notification signals
        public TaskCompletionSource<bool> CompleteElicitationReceivedSignal { get; } = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> SessionUpdateReceivedSignal { get; } = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> ExtNotificationReceivedSignal { get; } = new TaskCompletionSource<bool>();

        public Task CompleteAsync(CompleteElicitationNotification notification, CancellationToken cancellationToken = default)
        {
            LastCompleteElicitationNotification = notification;
            CompleteElicitationReceivedSignal.TrySetResult(true);
            return Task.CompletedTask;
        }

        public Task<CreateElicitationResponse> CreateAsync(CreateElicitationRequest request, CancellationToken cancellationToken = default)
        {
            LastCreateElicitationRequest = request;
            return Task.FromResult(CreateElicitationResponseToReturn);
        }

        public Task<ReadTextFileResponse> ReadTextFileAsync(ReadTextFileRequest request, CancellationToken cancellationToken = default)
        {
            LastReadTextFileRequest = request;
            return Task.FromResult(ReadTextFileResponseToReturn);
        }

        public Task<WriteTextFileResponse> WriteTextFileAsync(WriteTextFileRequest request, CancellationToken cancellationToken = default)
        {
            LastWriteTextFileRequest = request;
            return Task.FromResult(WriteTextFileResponseToReturn);
        }

        public Task<RequestPermissionResponse> RequestPermissionAsync(RequestPermissionRequest request, CancellationToken cancellationToken = default)
        {
            LastRequestPermissionRequest = request;
            return Task.FromResult(RequestPermissionResponseToReturn);
        }

        public Task SessionUpdateAsync(SessionNotification notification, CancellationToken cancellationToken = default)
        {
            LastSessionNotification = notification;
            SessionUpdateReceivedSignal.TrySetResult(true);
            return Task.CompletedTask;
        }

        public Task<CreateTerminalResponse> CreateTerminalAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default)
        {
            LastCreateTerminalRequest = request;
            return Task.FromResult(CreateTerminalResponseToReturn);
        }

        public Task<KillTerminalResponse> KillTerminalAsync(KillTerminalRequest request, CancellationToken cancellationToken = default)
        {
            LastKillTerminalRequest = request;
            return Task.FromResult(KillTerminalResponseToReturn);
        }

        public Task<TerminalOutputResponse> TerminalOutputAsync(TerminalOutputRequest request, CancellationToken cancellationToken = default)
        {
            LastTerminalOutputRequest = request;
            return Task.FromResult(TerminalOutputResponseToReturn);
        }

        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(ReleaseTerminalRequest request, CancellationToken cancellationToken = default)
        {
            LastReleaseTerminalRequest = request;
            return Task.FromResult(ReleaseTerminalResponseToReturn);
        }

        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(WaitForTerminalExitRequest request, CancellationToken cancellationToken = default)
        {
            LastWaitForTerminalExitRequest = request;
            return Task.FromResult(WaitForTerminalExitResponseToReturn);
        }

        public Task<object> ExtMethodAsync(string method, object request, CancellationToken cancellationToken = default)
        {
            LastExtMethodName = method;
            LastExtMethodRequest = request;
            return Task.FromResult(ExtMethodResponseToReturn);
        }

        public Task ExtNotificationAsync(string method, object notification, CancellationToken cancellationToken = default)
        {
            LastExtNotificationName = method;
            LastExtNotificationPayload = notification;
            ExtNotificationReceivedSignal.TrySetResult(true);
            return Task.CompletedTask;
        }

        public void OnDisconnected(Connection connection)
        {
            // Mock implementation - no-op
        }

        public Task<ConnectMcpResponse> ConnectAsync(ConnectMcpRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<DisconnectMcpResponse> DisconnectAsync(DisconnectMcpRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
