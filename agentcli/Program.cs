using agentcli;
using dotacp.agent;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var agent = new Agent();

            var inputStream = Console.OpenStandardInput();
            var outputStream = Console.OpenStandardOutput();

            using var connection = Connection.RunAgent(
                agent,
                outputStream,
                inputStream,
                new TraceSource("JsonRpc", SourceLevels.Verbose));

            if (connection == null)
            {
                await Console.Error.WriteLineAsync("Failed to create agent connection.");
                return;
            }

            await connection.Completion;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Error: {ex}");
        }
    }
}
