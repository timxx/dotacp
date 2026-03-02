using dotacp.agent;
using dotacp.protocol;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace agentcli
{
    internal class Agent : IAcpAgent
    {
        private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        private Connection? _connection;

        public void OnClientConnected(Connection connection)
        {
            _connection = connection;
        }

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new InitializeResponse()
            {
                ProtocolVersion = ProtocolMeta.Version,
                AgentInfo = new Implementation()
                {
                    Name = "agentcli",
                    Version = "0.1.0",
                    Title = "Simple ACP Agent Demo",
                },
                AgentCapabilities = new AgentCapabilities()
                {
                    LoadSession = false,
                },
            });
        }

        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AuthenticateResponse());
        }

        public Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            var sessionId = Guid.NewGuid().ToString();
            var session = new Session { SessionId = sessionId, Cwd = request.Cwd };
            _sessions[sessionId] = session;

            return Task.FromResult(new NewSessionResponse()
            {
                SessionId = sessionId,
            });
        }

        public Task<LoadSessionResponse> LoadSessionAsync(LoadSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new LoadSessionResponse());
        }

        public async Task<PromptResponse> PromptAsync(PromptRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            foreach (var block in request.Prompt)
            {
                if (!(block is TextContent))
                {
                    await Console.Error.WriteLineAsync($"Received unsupported content block of type {block.Type}");
                    continue;
                }

                await _connection!.SessionUpdateAsync(new SessionNotification()
                {
                    SessionId = request.SessionId,
                    Update = new SessionUpdateAgentMessageChunk()
                    {
                        Content = new TextContent()
                        {
                            Text = $"Received: {(block as TextContent)!.Text}"
                        },
                    },
                }, cancellationToken);
            }

            var response = new PromptResponse()
            {
                StopReason = StopReason.EndTurn,
            };

            return response;
        }

        public Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
            SetSessionConfigOptionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            return Task.FromResult(new SetSessionConfigOptionResponse());
        }

        public Task<SetSessionModeResponse> SetSessionModeAsync(SetSessionModeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            return Task.FromResult(new SetSessionModeResponse());
        }

        public async Task CancelAsync(CancelNotification notification,
            CancellationToken cancellationToken = default)
        {
            await Console.Error.WriteLineAsync($"Cancel operation received for session {notification.SessionId}");
        }

        public Task<ForkSessionResponse> ForkSessionAsync(ForkSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            var newSessionId = Guid.NewGuid().ToString();
            var newSession = new Session { SessionId = newSessionId, Cwd = request.Cwd };
            _sessions[newSessionId] = newSession;

            return Task.FromResult(new ForkSessionResponse()
            {
                SessionId = newSessionId,
            });
        }

        public Task<ListSessionsResponse> ListSessionsAsync(ListSessionsRequest request,
            CancellationToken cancellationToken = default)
        {
            var sessions = new List<SessionInfo>();
            foreach (var session in _sessions.Values)
            {
                sessions.Add(new SessionInfo()
                {
                    SessionId = session.SessionId,
                });
            }

            return Task.FromResult(new ListSessionsResponse()
            {
                Sessions = sessions.ToArray(),
            });
        }

        public Task<ResumeSessionResponse> ResumeSessionAsync(ResumeSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            return Task.FromResult(new ResumeSessionResponse());
        }

        public Task<SetSessionModelResponse> SetSessionModelAsync(SetSessionModelRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_sessions.TryGetValue(request.SessionId, out var session))
            {
                throw new InvalidOperationException($"Session {request.SessionId} not found.");
            }

            return Task.FromResult(new SetSessionModelResponse());
        }

        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            if (method != "test_extmethod")
                throw new Exception($"Method not found: {method}");

            return Task.FromResult<object>(new Dictionary<string, string>()
            {
                {"example", "response" }
            });
        }

        public async Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            await Console.Error.WriteLineAsync($"Extended notification '{method}' received.");

            if (method != "test_extnotification")
                throw new Exception($"Notification method not found: {method}");
        }

        private class Session
        {
            public string SessionId { get; set; } = null!;
            public string Cwd { get; set; } = null!;
        }
    }
}
