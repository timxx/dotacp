// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.10.8

using dotacp.protocol;
using dotacp.shared;
using StreamJsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.agent
{
    internal sealed class AgentRpcTarget
    {
        private readonly IAcpAgent _agent;

        public AgentRpcTarget(IAcpAgent agent)
        {
            _agent = agent;
        }

        [JsonRpcMethod(AgentMethods.Authenticate, UseSingleObjectParameterDeserialization = true)]
        public Task<AuthenticateResponse> AuthenticateAsync(
            AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.AuthenticateAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.Initialize, UseSingleObjectParameterDeserialization = true)]
        public Task<InitializeResponse> InitializeAsync(
            InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.InitializeAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionCancel, UseSingleObjectParameterDeserialization = true)]
        public Task CancelAsync(
            CancelNotification notification,
            CancellationToken cancellationToken = default)
        {
            return _agent.CancelAsync(notification, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionFork, UseSingleObjectParameterDeserialization = true)]
        public Task<ForkSessionResponse> ForkSessionAsync(
            ForkSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.ForkSessionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionList, UseSingleObjectParameterDeserialization = true)]
        public Task<ListSessionsResponse> ListSessionsAsync(
            ListSessionsRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.ListSessionsAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionLoad, UseSingleObjectParameterDeserialization = true)]
        public Task<LoadSessionResponse> LoadSessionAsync(
            LoadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.LoadSessionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionNew, UseSingleObjectParameterDeserialization = true)]
        public Task<NewSessionResponse> NewSessionAsync(
            NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.NewSessionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionPrompt, UseSingleObjectParameterDeserialization = true)]
        public Task<PromptResponse> PromptAsync(
            PromptRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.PromptAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionResume, UseSingleObjectParameterDeserialization = true)]
        public Task<ResumeSessionResponse> ResumeSessionAsync(
            ResumeSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.ResumeSessionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionSetConfigOption, UseSingleObjectParameterDeserialization = true)]
        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.SetSessionConfigOptionAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionSetMode, UseSingleObjectParameterDeserialization = true)]
        public Task<SetSessionModeResponse> SetSessionModeAsync(
            SetSessionModeRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.SetSessionModeAsync(request, cancellationToken);
        }

        [JsonRpcMethod(AgentMethods.SessionSetModel, UseSingleObjectParameterDeserialization = true)]
        public Task<SetSessionModelResponse> SetSessionModelAsync(
            SetSessionModelRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.SetSessionModelAsync(request, cancellationToken);
        }

        [JsonRpcMethod("__acp_ext_method__", UseSingleObjectParameterDeserialization = true)]
        public Task<object> HandleExtensionMethodAsync(
            ExtensionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.ExtMethodAsync(request.Method, request.Arguments, cancellationToken);
        }

        [JsonRpcMethod("__acp_ext_notification__", UseSingleObjectParameterDeserialization = true)]
        public Task HandleExtensionNotificationAsync(
            ExtensionRequest request,
            CancellationToken cancellationToken = default)
        {
            return _agent.ExtNotificationAsync(request.Method, request.Arguments, cancellationToken);
        }
    }
}
