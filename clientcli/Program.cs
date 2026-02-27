using dotacp.client;
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

            var connection = Connection.ConnectToAgent(
                client,
                process.StandardInput.BaseStream,
                process.StandardOutput.BaseStream);

            if (connection == null)
            {
                Console.WriteLine("Failed to connect to agent.");
                return;
            }

            while (true)
            {
                Console.WriteLine("Press Enter to send a request, or type '/exit' to quit.");
                var input = Console.ReadLine();
                if (input == "/exit")
                    break;

                // TODO: ...
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

        static Process CreateAgent(string program, string args)
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
    }
}
