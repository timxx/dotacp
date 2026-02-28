using dotacp.client;
using dotacp.protocol;
using System;
using System.Collections.Generic;
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

            var connection = Connection.ConnectToAgent(
                client,
                process.StandardInput.BaseStream,
                process.StandardOutput.BaseStream,
                new TraceSource("JsonRpc", SourceLevels.Verbose));

            if (connection == null)
            {
                Console.WriteLine("Failed to connect to agent.");
                return;
            }

            if (!await InitiliazeAsync(connection))
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
                    McpServers = new List<McpServer>()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create session: {ex.ToString()}");
                process.Close();
                return;
            }

            Console.WriteLine($"Session: {session.SessionId}");
            // FIXME: models is in unstable schema
            // Console.WriteLine("Available models:");

            Console.WriteLine("Available modes:");
            foreach (var mode in session.Modes.AvailableModes)
            {
                Console.WriteLine($"  {mode.Id}: {mode.Name} - {mode.Description}");
            }
            Console.WriteLine($"Current mode: {session.Modes.CurrentModeId}");

            while (true)
            {
                Console.WriteLine("Press Enter to send a request, or type '/exit' to quit.");
                var input = Console.ReadLine();
                if (input == "/exit")
                    break;

                try
                {
                    var promptResp = await connection.PromptAsync(new PromptRequest()
                    {
                        SessionId = session.SessionId,
                        // FIXME: update the script to generate TextContent based on ContentBlock
                        Prompt = new List<ContentBlock>(),
                    });
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

        static async Task<bool> InitiliazeAsync(Connection connection)
        {
            try
            {
                var response = await connection.InitializeAsync(new InitializeRequest()
                {
                    ClientCapabilities = new ClientCapabilities()
                    {
                        Fs = new FileSystemCapability()
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
                        try
                        {
                            var authResp = await connection.AuthenticateAsync(
                                new AuthenticateRequest()
                                {
                                    MethodId = method.Id,
                                });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Auth with method `{method.Name}` failed: {ex.Message}");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
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
