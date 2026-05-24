using dotacp.agent.unstable;
using dotacp.protocol.unstable;
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

        private string[] _codeBlocks = new string[]
        {
            "Compile", " and", " run", ":\n\n",
            "```", "bash", "\n",
            "g", "++", " -", "std", "=c", "++", "17", " demo", ".cpp", " -", "o", " demo", "\n",
            "./", "demo", "\n",
            "``", "`\n\n",
            "If", " you", " want",
        };

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
                    PromptCapabilities = new PromptCapabilities
                    {
                        Audio = true,
                        EmbeddedContext = true,
                        Image = true,
                    }
                },
            });
        }

        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AuthenticateResponse());
        }

        public async Task<NewSessionResponse> NewSessionAsync(NewSessionRequest request,
            CancellationToken cancellationToken = default)
        {
            var sessionId = Guid.NewGuid().ToString();
            var session = new Session { SessionId = sessionId, Cwd = request.Cwd };
            _sessions[sessionId] = session;

            var response = new NewSessionResponse()
            {
                SessionId = sessionId,
            };

            _ = Task.Run(async () =>
            {
                await SendAvailableCommandsAsync(sessionId, cancellationToken);
            });

            return response;
        }

        private async Task SendAvailableCommandsAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            await _connection!.SessionUpdateAsync(new SessionNotification()
            {
                SessionId = sessionId,
                Update = new AvailableCommandsUpdate()
                {
                    AvailableCommands = new AvailableCommand[]
                    {
                        new AvailableCommand()
                        {
                            Name = "codeblock",
                            Description = "Show a streaming code block example",
                        },
                        new AvailableCommand()
                        {
                            Name = "permission",
                            Description = "Request user permission with options",
                        },
                        new AvailableCommand()
                        {
                            Name = "plan",
                            Description = "Create and display a plan",
                        },
                        new AvailableCommand()
                        {
                            Name = "usage",
                            Description = "Update usage based on input",
                        },
                    },
                },
            }, cancellationToken);
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
                if (!(block is TextContent textContent))
                {
                    await Console.Error.WriteLineAsync($"Received unsupported content block of type {block.Type}");
                    continue;
                }

                string[] blocksToResponse;
                switch (textContent.Text.Trim())
                {
                    case "/codeblock":
                        blocksToResponse = _codeBlocks;
                        break;
                    case "/permission":
                        var permissionResponse = await RequstPermissionAsync(request.SessionId);
                        switch (permissionResponse.Outcome)
                        {
                            case RequestPermissionOutcomeCancelled cancelled:
                                blocksToResponse = new string[] { "User cancelled the permission request." };
                                break;
                            case SelectedPermissionOutcome selected:
                                blocksToResponse = new string[] { $"User selected permission option: {selected.OptionId}" };
                                break;
                            default:
                                blocksToResponse = Array.Empty<string>();
                                break;
                        }
                        break;
                    case "/plan":
                        await CreatePlanAsync(request.SessionId);
                        // Simulate some delay for plan execution
                        await Task.Delay(5000);
                        blocksToResponse = new string[]
                        {
                            "Plan is created\n",
                        };
                        break;
                    case var cmd when cmd.StartsWith("/usage"):
                        blocksToResponse = await HandleUsageAsync(request.SessionId, textContent.Text.Trim());
                        break;
                    default:
                        blocksToResponse = new string[] { textContent.Text };
                        break;
                }

                foreach (var text in blocksToResponse)
                {
                    await _connection!.SessionUpdateAsync(new SessionNotification()
                    {
                        SessionId = request.SessionId,
                        Update = new SessionUpdateAgentMessageChunk()
                        {
                            Content = new TextContent()
                            {
                                Text = text
                            },
                        },
                    }, cancellationToken);
                }
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

        public async Task<CloseSessionResponse> CloseAsync(CloseSessionRequest request, CancellationToken cancellationToken = default)
        {
            await Console.Error.WriteLineAsync($"Close operation received for session {request.SessionId}");
            return new CloseSessionResponse();
        }

        private Task<RequestPermissionResponse> RequstPermissionAsync(string sessionId)
        {
            return _connection!.RequestPermissionAsync(new RequestPermissionRequest()
            {
                SessionId = sessionId,
                Options = new PermissionOption[]
                {
                    new PermissionOption()
                    {
                        Name = "Allow",
                        Kind = PermissionOptionKind.AllowOnce,
                        OptionId = "allow_once",
                    },
                    new PermissionOption()
                    {
                        Name = "Allow Always",
                        Kind = PermissionOptionKind.AllowAlways,
                        OptionId = "allow_always",
                    },
                    new PermissionOption()
                    {
                        Name = "Reject",
                        Kind = PermissionOptionKind.RejectOnce,
                        OptionId = "reject_once",
                    },
                },
                ToolCall = new ToolCallUpdate()
                {
                    Title = "Permission Request Example",
                    Kind = ToolKind.Execute,
                    Content = new ToolCallContent[]
                    {
                        new Content { ContentValue = new TextContent { Text = "Execute action" } }
                    }
                }
            });
        }

        private Task CreatePlanAsync(string sessionId)
        {
            return _connection!.SessionUpdateAsync(new SessionNotification()
            {
                SessionId = sessionId,
                Update = new Plan()
                {
                    Entries = new PlanEntry[]
                    {
                        new PlanEntry()
                        {
                            Status = PlanEntryStatus.Completed,
                            Content = "Step 1: Do something"
                        },
                        new PlanEntry()
                        {
                            Status = PlanEntryStatus.InProgress,
                            Content = "Step 2: Do something else"
                        },
                        new PlanEntry()
                        {
                            Status = PlanEntryStatus.Pending,
                            Content = "Step 3: Do another thing"
                        }
                    },
                },
            });
        }

        private async Task<string[]> HandleUsageAsync(string sessionId, string input)
        {
            // Parse percentage from input if provided
            double percentage;
            var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length > 1 && double.TryParse(parts[1], out var parsedPercentage) && parsedPercentage >= 0 && parsedPercentage <= 100)
            {
                percentage = parsedPercentage;
            }
            else
            {
                // Random percentage between 0 and 100
                var random = new Random();
                percentage = random.NextDouble() * 100;
            }

            // Calculate used based on percentage (assuming size is 100000 tokens)
            const ulong size = 100000;
            var used = (ulong)(size * percentage / 100.0);

            await _connection!.SessionUpdateAsync(new SessionNotification()
            {
                SessionId = sessionId,
                Update = new UsageUpdate()
                {
                    Size = size,
                    Used = used,
                    Cost = new Cost()
                    {
                        Currency = "USD",
                        Amount = percentage / 100.0 * 0.5, // Max cost $0.50
                    },
                },
            });

            return new string[] { $"Usage updated: {percentage:F1}% ({used} tokens)" };
        }

        public void OnDisconnected(Connection connection)
        {
        }

        public Task DidchangeAsync(DidChangeDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DidcloseAsync(DidCloseDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DidfocusAsync(DidFocusDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DidopenAsync(DidOpenDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DidsaveAsync(DidSaveDocumentNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<LogoutResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AcceptAsync(AcceptNesNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CloseNesResponse> CloseAsync(CloseNesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RejectAsync(RejectNesNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<StartNesResponse> StartAsync(StartNesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SuggestNesResponse> SuggestAsync(SuggestNesRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DisableProviderResponse> DisableAsync(DisableProviderRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ListProvidersResponse> ListAsync(ListProvidersRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SetProviderResponse> SetAsync(SetProviderRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteSessionResponse> DeleteAsync(DeleteSessionRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private class Session
        {
            public string SessionId { get; set; } = null!;
            public string Cwd { get; set; } = null!;
        }
    }
}
