using dotacp.client.unstable;
using dotacp.protocol.unstable;
using StreamJsonRpc;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace clientcli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: clientcli AGENT_PROGRAM [args...]");
                return;
            }

            var program = args[0];
            string agentArgs = string.Empty;
            for (int i = 1; i < args.Length; i++)
            {
                if (i != 1)
                    agentArgs += " ";
                agentArgs += QuoteArgument(args[i]);
            }

            var process = CreateAgent(program, agentArgs);
            if (process == null)
            {
                Console.WriteLine("Failed to start agent process.");
                return;
            }

            var client = new Client();

            using var connection = Connection.RunClient(
                client,
                process.StandardInput.BaseStream,
                process.StandardOutput.BaseStream,
                new TraceSource("JsonRpc", SourceLevels.Verbose));

            if (connection == null)
            {
                Console.WriteLine("Failed to connect to agent.");
                return;
            }

            var capabilities = await InitiliazeAsync(connection);
            if (capabilities == null)
            {
                Console.WriteLine("Failed to initialize connection.");
                process.Close();
                return;
            }

            NewSessionResponse? session = null;
            try
            {
                session = await connection.NewSessionAsync(new NewSessionRequest()
                {
                    Cwd = Environment.CurrentDirectory,
                    McpServers = new McpServer[0],
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create session: {ex.ToString()}");
                process.Close();
                return;
            }

            Console.WriteLine($"Session: {session.SessionId}");

            bool hasModels = false;
            bool hasModes = false;

            if (session.ConfigOptions?.Length > 0)
            {
                foreach (var config in session.ConfigOptions)
                {
                    if (config.Category == SessionConfigOptionCategory.Model)
                    {
                        if (config is SessionConfigSelect configSelect)
                        {
                            if (configSelect.Options.TryGetSessionConfigSelectOption(out SessionConfigSelectOption[] options))
                            {
                                Console.WriteLine("Available models:");
                                foreach (var model in options)
                                    Console.WriteLine($"  - {model.Value}: {model.Name} - {model.Description}");
                                hasModels = options.Length > 0;
                                Console.WriteLine($"Current model: {configSelect.CurrentValue}");
                            }
                            else
                            {
                                Console.WriteLine($"Unhandled SessionConfigSelectGroup");
                            }
                        }

                        continue;
                    }

                    if (config.Category == SessionConfigOptionCategory.Mode)
                    {
                        if (config is SessionConfigSelect configSelect)
                        {
                            if (configSelect.Options.TryGetSessionConfigSelectOption(out SessionConfigSelectOption[] options))
                            {
                                Console.WriteLine("Available modes:");
                                foreach (var mode in options)
                                    Console.WriteLine($"  - {mode.Value}: {mode.Name} - {mode.Description}");
                                hasModes = options.Length > 0;
                                Console.WriteLine($"Current mode: {configSelect.CurrentValue}");
                            }
                            else
                            {
                                Console.WriteLine($"Unhandled SessionConfigSelectGroup");
                            }
                        }

                        continue;
                    }

                    Console.WriteLine($"Unknown config category: {config.Category}");
                }
            }

            try
            {
                var result = await connection.ExtMethodAsync(
                    "test_extmethod",
                    new { text = "hello from client" });

                await connection.ExtNotificationAsync(
                    "test_extnotification",
                    new object[] { 1, 2, 3 });
            }
            catch (RemoteMethodNotFoundException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Extension method/notification call failed: {ex}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Available commands:");
            Console.WriteLine("  /exit - Exit the client");
            if (hasModes)
                Console.WriteLine("  /switchmode <modeId> - Switch mode");
            if (hasModels)
                Console.WriteLine("  /switchmodel <modelId> - Switch model");
            Console.WriteLine("  /newsession [workingDir] - Start a new session");
            if (capabilities.LoadSession)
                Console.WriteLine("  /loadsession <sessionId> - Load another session");
            if (capabilities.SessionCapabilities != null)
            {
                if (capabilities.SessionCapabilities.List != null)
                    Console.WriteLine("  /listsessions - List available sessions to load");
                if (capabilities.SessionCapabilities.Fork != null)
                    Console.WriteLine("  /forksession - Fork the current session");
                if (capabilities.SessionCapabilities.Resume != null)
                    Console.WriteLine("  /resumesession <sessionId> - Resume a suspended session");
            }
            Console.ResetColor();

            while (true)
            {
                Console.WriteLine("Press Enter to send a request");
                Console.ForegroundColor = ConsoleColor.Green;
                var input = Console.ReadLine();
                Console.ResetColor();
                if (input == null)
                    break;
                if (input.Length == 0)
                    continue;

                input = input.Trim();
                if (input == "/exit")
                    break;

                if (hasModes && input.StartsWith("/switchmode "))
                {
                    var modeId = input.Split(' ')[1].Trim();
                    await connection.SetSessionModeAsync(new SetSessionModeRequest()
                    {
                        SessionId = session.SessionId,
                        ModeId = modeId
                    });
                    continue;
                }

                if (hasModels && input.StartsWith("/switchmodel "))
                {
                    var modelId = input.Split(' ')[1].Trim();
                    await connection.SetSessionConfigOptionAsync(new SetSessionConfigOptionRequest()
                    {
                        SessionId = session.SessionId,
                        ConfigId = (string)SessionConfigOptionCategory.Model,
                        Value = new SetSessionConfigOptionRequestValue(modelId)
                    });
                    continue;
                }

                if (capabilities.LoadSession && input.StartsWith("/loadsession "))
                {
                    var sessionId = input.Split(' ')[1].Trim();
                    var loadResp = await connection.LoadSessionAsync(new LoadSessionRequest()
                    {
                        SessionId = sessionId,
                        Cwd = Environment.CurrentDirectory,
                        McpServers = Array.Empty<McpServer>()
                    });
                    continue;
                }

                if (input.StartsWith("/newsession"))
                {
                    var pos = input.IndexOf(' ');
                    string? workingDir = null;
                    if (pos >= 0)
                        workingDir = input.Substring(pos + 1).Trim();

                    var newSessionResp = await connection.NewSessionAsync(new NewSessionRequest()
                    {
                        Cwd = workingDir ?? Environment.CurrentDirectory,
                        McpServers = Array.Empty<McpServer>()
                    });
                    session = newSessionResp;
                    Console.WriteLine($"New session: {session.SessionId}");
                    continue;
                }

                if (capabilities.SessionCapabilities != null)
                {
                    if (capabilities.SessionCapabilities.List != null && input == "/listsessions")
                    {
                        var listResp = await connection.ListSessionsAsync(new ListSessionsRequest());
                        Console.WriteLine("Available sessions:");
                        foreach (var s in listResp.Sessions)
                            Console.WriteLine($"  - {s.SessionId}: {s.Title}");
                        continue;
                    }

                    if (capabilities.SessionCapabilities.Fork != null && input == "/forksession")
                    {
                        var forkResp = await connection.ForkSessionAsync(new ForkSessionRequest()
                        {
                            SessionId = session.SessionId,
                            Cwd = Environment.CurrentDirectory,
                        });
                        continue;
                    }

                    if (capabilities.SessionCapabilities.Resume != null && input.StartsWith("/resumesession "))
                    {
                        var sessionId = input.Split(' ')[1].Trim();
                        var resumeResp = await connection.ResumeSessionAsync(new ResumeSessionRequest()
                        {
                            SessionId = sessionId,
                            Cwd = Environment.CurrentDirectory,
                        });
                        continue;
                    }
                }

                try
                {
                    var promptResp = await connection.PromptAsync(new PromptRequest()
                    {
                        SessionId = session.SessionId,
                        Prompt = new ContentBlock[1]
                        {
                            new TextContent()
                            {
                                Text = input,
                            }
                        }
                    });

                    client.EndTurn(promptResp.StopReason);
                    Console.WriteLine($"\nStop reason: {promptResp.StopReason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            process.Close();
        }

        private static string QuoteArgument(string v)
        {
            if (string.IsNullOrEmpty(v))
                return v;

            if (v.Contains(" ") && v[0] != '"')

                return "\"" + v + "\"";

            return v;
        }

        static Process? CreateAgent(string program, string args)
        {
            var process = Process.Start(new ProcessStartInfo(program, args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,
            });

            if (process == null)
                return null;

            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    Console.WriteLine("[AGENT ERROR] " + e.Data);
            };

            process.BeginErrorReadLine();

            return process;
        }

        static async Task<AgentCapabilities?> InitiliazeAsync(Connection connection)
        {
            try
            {
                var response = await connection.InitializeAsync(new InitializeRequest()
                {
                    ClientCapabilities = new ClientCapabilities()
                    {
                        Fs = new FileSystemCapabilities()
                        {
                            ReadTextFile = true,
                            WriteTextFile = true,
                        },
                    },
                    ClientInfo = new Implementation()
                    {
                        Name = "dotacp client",
                        Version = "1.0.0"
                    },
                    ProtocolVersion = ProtocolMeta.Version,
                });

                PrintAgentCapabilities(response.AgentCapabilities);
                PrintAgentInfo(response.AgentInfo);
                Console.WriteLine($"Agent Protocol Version: {response.ProtocolVersion}");

                if (response.AuthMethods != null)
                {
                    foreach (var method in response.AuthMethods)
                    {
                        var (id, name) = GetAuthMethodIdAndName(method);
                        if (id == null)
                            continue;
                        try
                        {
                            var authResp = await connection.AuthenticateAsync(
                                new AuthenticateRequest()
                                {
                                    MethodId = id,
                                });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Auth with method `{name ?? id}` failed: {ex.Message}");
                        }
                    }
                }

                return response.AgentCapabilities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        static (string? id, string? name) GetAuthMethodIdAndName(AuthMethod method)
        {
            switch (method)
            {
                case AuthMethodAgent a: return (a.Id, a.Name);
                case AuthMethodEnvVar e: return (e.Id, e.Name);
                case AuthMethodTerminal t: return (t.Id, t.Name);
                default: return (null, null);
            }
        }

        static void PrintAgentCapabilities(AgentCapabilities caps)
        {
            Console.WriteLine("Agent Capabilities:");
            Console.WriteLine($"  LoadSession: {caps.LoadSession}");
        }

        static void PrintAgentInfo(Implementation info)
        {
            Console.WriteLine($"Agent: {info.Name} {info.Version} ({info.Title})");
        }
    }
}
