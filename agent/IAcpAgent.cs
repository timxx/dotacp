using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent
{
    public interface IAcpAgent
    {
        void OnClientConnected(Connection connection);

        Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default);

        Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default);

        Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default);

        Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default);

        Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default);

        Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default);

        Task CancelAsync(CancelNotification notification,
            CancellationToken cancellationToken = default);

        Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default);

        Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default);
    }
}
