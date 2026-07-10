using dotacp.agent.unstable;
using dotacp.protocol.unstable;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Mock IAcpAgent (unstable) that captures received requests and returns configured responses.
    /// Implements the full unstable protocol surface including session management.
    /// </summary>
    internal sealed class MockAgent_Unstable : IAcpAgent
    {
        public Connection? ReceivedConnection { get; private set; }

        // Captured requests
        public AuthenticateRequest? LastAuthenticateRequest { get; private set; }
        public DidChangeDocumentNotification? LastDidChangeDocumentNotification { get; private set; }
        public DidCloseDocumentNotification? LastDidCloseDocumentNotification { get; private set; }
        public DidFocusDocumentNotification? LastDidFocusDocumentNotification { get; private set; }
        public DidOpenDocumentNotification? LastDidOpenDocumentNotification { get; private set; }
        public DidSaveDocumentNotification? LastDidSaveDocumentNotification { get; private set; }
        public InitializeRequest? LastInitializeRequest { get; private set; }
        public LogoutRequest? LastLogoutRequest { get; private set; }
        public AcceptNesNotification? LastAcceptNesNotification { get; private set; }
        public CloseNesRequest? LastCloseNesRequest { get; private set; }
        public RejectNesNotification? LastRejectNesNotification { get; private set; }
        public StartNesRequest? LastStartNesRequest { get; private set; }
        public SuggestNesRequest? LastSuggestNesRequest { get; private set; }
        public DisableProviderRequest? LastDisableProviderRequest { get; private set; }
        public ListProvidersRequest? LastListProvidersRequest { get; private set; }
        public SetProviderRequest? LastSetProviderRequest { get; private set; }
        public CancelNotification? LastCancelNotification { get; private set; }
        public ForkSessionRequest? LastForkSessionRequest { get; private set; }
        public ListSessionsRequest? LastListSessionsRequest { get; private set; }
        public LoadSessionRequest? LastLoadSessionRequest { get; private set; }
        public NewSessionRequest? LastNewSessionRequest { get; private set; }
        public PromptRequest? LastPromptRequest { get; private set; }
        public ResumeSessionRequest? LastResumeSessionRequest { get; private set; }
        public SetSessionConfigOptionRequest? LastSetSessionConfigOptionRequest { get; private set; }
        public SetSessionModeRequest? LastSetSessionModeRequest { get; private set; }
        public CloseSessionRequest? LastCloseSessionRequest { get; private set; }
        public string? LastExtMethodName { get; private set; }
        public object? LastExtMethodRequest { get; private set; }
        public string? LastExtNotificationName { get; private set; }
        public object? LastExtNotificationPayload { get; private set; }

        // Responses to return
        public AuthenticateResponse AuthenticateResponseToReturn { get; set; } = new AuthenticateResponse();
        public InitializeResponse InitializeResponseToReturn { get; set; } = new InitializeResponse();
        public LogoutResponse LogoutResponseToReturn { get; set; } = new LogoutResponse();
        public CloseNesResponse CloseNesResponseToReturn { get; set; } = new CloseNesResponse();
        public StartNesResponse StartNesResponseToReturn { get; set; } = new StartNesResponse();
        public SuggestNesResponse SuggestNesResponseToReturn { get; set; } = new SuggestNesResponse();
        public DisableProviderResponse DisableProviderResponseToReturn { get; set; } = new DisableProviderResponse();
        public ListProvidersResponse ListProvidersResponseToReturn { get; set; } = new ListProvidersResponse();
        public SetProviderResponse SetProviderResponseToReturn { get; set; } = new SetProviderResponse();
        public ForkSessionResponse ForkSessionResponseToReturn { get; set; } = new ForkSessionResponse();
        public ListSessionsResponse ListSessionsResponseToReturn { get; set; } = new ListSessionsResponse { Sessions = new SessionInfo[0] };
        public LoadSessionResponse LoadSessionResponseToReturn { get; set; } = new LoadSessionResponse();
        public NewSessionResponse NewSessionResponseToReturn { get; set; } = new NewSessionResponse();
        public PromptResponse PromptResponseToReturn { get; set; } = new PromptResponse();
        public ResumeSessionResponse ResumeSessionResponseToReturn { get; set; } = new ResumeSessionResponse();
        public SetSessionConfigOptionResponse SetSessionConfigOptionResponseToReturn { get; set; } = new SetSessionConfigOptionResponse { ConfigOptions = new SessionConfigOption[0] };
        public SetSessionModeResponse SetSessionModeResponseToReturn { get; set; } = new SetSessionModeResponse();
        public CloseSessionResponse CloseSessionResponseToReturn { get; set; } = new CloseSessionResponse();
        public object ExtMethodResponseToReturn { get; set; } = new object();

        // Notification signal
        public TaskCompletionSource<bool> CancelReceivedSignal { get; } = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> ExtNotificationReceivedSignal { get; } = new TaskCompletionSource<bool>();

        public void OnClientConnected(Connection connection)
        {
            ReceivedConnection = connection;
        }

        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default)
        {
            LastAuthenticateRequest = request;
            return Task.FromResult(AuthenticateResponseToReturn);
        }

        public Task DidchangeAsync(DidChangeDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            LastDidChangeDocumentNotification = notification;
            return Task.CompletedTask;
        }

        public Task DidcloseAsync(DidCloseDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            LastDidCloseDocumentNotification = notification;
            return Task.CompletedTask;
        }

        public Task DidfocusAsync(DidFocusDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            LastDidFocusDocumentNotification = notification;
            return Task.CompletedTask;
        }

        public Task DidopenAsync(DidOpenDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            LastDidOpenDocumentNotification = notification;
            return Task.CompletedTask;
        }

        public Task DidsaveAsync(DidSaveDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            LastDidSaveDocumentNotification = notification;
            return Task.CompletedTask;
        }

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default)
        {
            LastInitializeRequest = request;
            return Task.FromResult(InitializeResponseToReturn);
        }

        public Task<LogoutResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
        {
            LastLogoutRequest = request;
            return Task.FromResult(LogoutResponseToReturn);
        }

        public Task AcceptAsync(AcceptNesNotification notification, CancellationToken cancellationToken = default)
        {
            LastAcceptNesNotification = notification;
            return Task.CompletedTask;
        }

        public Task<CloseNesResponse> CloseAsync(CloseNesRequest request, CancellationToken cancellationToken = default)
        {
            LastCloseNesRequest = request;
            return Task.FromResult(CloseNesResponseToReturn);
        }

        public Task RejectAsync(RejectNesNotification notification, CancellationToken cancellationToken = default)
        {
            LastRejectNesNotification = notification;
            return Task.CompletedTask;
        }

        public Task<StartNesResponse> StartAsync(StartNesRequest request, CancellationToken cancellationToken = default)
        {
            LastStartNesRequest = request;
            return Task.FromResult(StartNesResponseToReturn);
        }

        public Task<SuggestNesResponse> SuggestAsync(SuggestNesRequest request, CancellationToken cancellationToken = default)
        {
            LastSuggestNesRequest = request;
            return Task.FromResult(SuggestNesResponseToReturn);
        }

        public Task<DisableProviderResponse> DisableAsync(DisableProviderRequest request, CancellationToken cancellationToken = default)
        {
            LastDisableProviderRequest = request;
            return Task.FromResult(DisableProviderResponseToReturn);
        }

        public Task<ListProvidersResponse> ListAsync(ListProvidersRequest request, CancellationToken cancellationToken = default)
        {
            LastListProvidersRequest = request;
            return Task.FromResult(ListProvidersResponseToReturn);
        }

        public Task<SetProviderResponse> SetAsync(SetProviderRequest request, CancellationToken cancellationToken = default)
        {
            LastSetProviderRequest = request;
            return Task.FromResult(SetProviderResponseToReturn);
        }

        public Task CancelAsync(CancelNotification notification, CancellationToken cancellationToken = default)
        {
            LastCancelNotification = notification;
            CancelReceivedSignal.TrySetResult(true);
            return Task.CompletedTask;
        }

        public Task<ForkSessionResponse> ForkSessionAsync(ForkSessionRequest request, CancellationToken cancellationToken = default)
        {
            LastForkSessionRequest = request;
            return Task.FromResult(ForkSessionResponseToReturn);
        }

        public Task<ListSessionsResponse> ListSessionsAsync(ListSessionsRequest request, CancellationToken cancellationToken = default)
        {
            LastListSessionsRequest = request;
            return Task.FromResult(ListSessionsResponseToReturn);
        }

        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request, CancellationToken cancellationToken = default)
        {
            LastLoadSessionRequest = request;
            return Task.FromResult(LoadSessionResponseToReturn);
        }

        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request, CancellationToken cancellationToken = default)
        {
            LastNewSessionRequest = request;
            return Task.FromResult(NewSessionResponseToReturn);
        }

        public async Task<PromptResponse> PromptAsync(PromptRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Meta != null && request.Meta.TryGetValue("testCancel", out var testCancel)
                && (bool)testCancel)
            {
                await CancelReceivedSignal.Task;
                return new PromptResponse
                {
                    // According to the schema, it must return a PromptResponse with StopReason.Cancelled when the session is cancelled
                    StopReason = StopReason.Cancelled
                };
            }

            LastPromptRequest = request;
            return PromptResponseToReturn;
        }

        public Task<ResumeSessionResponse> ResumeSessionAsync(ResumeSessionRequest request, CancellationToken cancellationToken = default)
        {
            LastResumeSessionRequest = request;
            return Task.FromResult(ResumeSessionResponseToReturn);
        }

        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(SetSessionConfigOptionRequest request, CancellationToken cancellationToken = default)
        {
            LastSetSessionConfigOptionRequest = request;
            return Task.FromResult(SetSessionConfigOptionResponseToReturn);
        }

        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request, CancellationToken cancellationToken = default)
        {
            LastSetSessionModeRequest = request;
            return Task.FromResult(SetSessionModeResponseToReturn);
        }

        public Task<CloseSessionResponse> CloseAsync(CloseSessionRequest request, CancellationToken cancellationToken = default)
        {
            LastCloseSessionRequest = request;
            return Task.FromResult(CloseSessionResponseToReturn);
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
        }

        public Task<DeleteSessionResponse> DeleteAsync(DeleteSessionRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
