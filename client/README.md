# dotacp.client

.NET library for implementing [Agent Client Protocol (ACP)](https://agentclientprotocol.com/) clients (editors/IDEs).

## What is dotacp.client?

This package provides the tools needed to integrate ACP agent support into editors, IDEs, and other client applications. It includes:

- **IAcpClient Interface**: Core interface defining all client-side protocol handlers
- **Connection Management**: Handles JSON-RPC communication with ACP agents
- **RPC Message Routing**: Automatic dispatch of inbound agent requests

## Dependencies

- **.NET Standard 2.0** or higher
- `dotacp.protocol` - Protocol type definitions
- `StreamJsonRpc` (v2.7.76+) - JSON-RPC communication

## Installation

```bash
dotnet add package dotacp.client
```

Or reference the project directly:

```xml
<ItemGroup>
    <ProjectReference Include="path/to/client/client.csproj" />
</ItemGroup>
```

## Quick Start

### 1. Implement the IAcpClient Interface

```csharp
using dotacp.client;
using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

public class MyAcpClient : IAcpClient
{
    public async Task<RequestPermissionResponse> RequestPermissionAsync(
        RequestPermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Show permission dialog to user using request.Options and request.ToolCall details
        var userOption = await ShowPermissionDialogAsync(
            request.Options,
            request.ToolCall);

        return new RequestPermissionResponse
        {
            Outcome = userOption != null 
                ? (RequestPermissionOutcome)new SelectedPermissionOutcome { OptionId = userOption.OptionId }
                : new RequestPermissionOutcomeCancelled()
        };
    }

    public async Task SessionUpdateAsync(
        SessionNotification notification,
        CancellationToken cancellationToken = default)
    {
        // Handle session state updates from agent
        // Update UI, logs, etc.
        Console.WriteLine($"Session {notification.SessionId} updated");
    }

    public async Task<WriteTextFileResponse> WriteTextFileAsync(
        WriteTextFileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Write file to disk using the client's file system
            // request.SessionId, request.Path, and request.Content are available
            await File.WriteAllTextAsync(
                request.Path,
                request.Content,
                cancellationToken);

            return new WriteTextFileResponse();
        }
        catch (Exception ex)
        {
            // Throw exception to indicate error to the agent
            throw new InvalidOperationException($"Failed to write file {request.Path}: {ex.Message}");
        }
    }

    public async Task<ReadTextFileResponse> ReadTextFileAsync(
        ReadTextFileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Read file from disk (request.Path available)
            var content = await File.ReadAllTextAsync(
                request.Path,
                cancellationToken);

            return new ReadTextFileResponse
            {
                Content = content
            };
        }
        catch (Exception ex)
        {
            // Throw exception to indicate error to the agent
            throw new InvalidOperationException($"Failed to read file {request.Path}: {ex.Message}");
        }
    }

    public async Task<CreateTerminalResponse> CreateTerminalAsync(
        CreateTerminalRequest request,
        CancellationToken cancellationToken = default)
    {
        // Create a new terminal/shell session
        // request.SessionId, request.Command, request.Args, request.Cwd, request.Env available
        var terminalId = Guid.NewGuid().ToString();
        
        // Start the terminal process
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = request.Command,
                Arguments = string.Join(" ", request.Args ?? Array.Empty<string>()),
                WorkingDirectory = request.Cwd,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();

        return new CreateTerminalResponse
        {
            TerminalId = terminalId
        };
    }

    public async Task<TerminalOutputResponse> TerminalOutputAsync(
        TerminalOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        // Get output from the terminal (request.SessionId, request.TerminalId available)
        // Return the captured output and exit status if available
        return new TerminalOutputResponse
        {
            Output = "Terminal output here",
            Truncated = false,
            ExitStatus = null  // null if still running
        };
    }

    public async Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
        ReleaseTerminalRequest request,
        CancellationToken cancellationToken = default)
    {
        // Release/close a terminal
        // Clean up the terminal process if needed
        return new ReleaseTerminalResponse { };
    }

    public async Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
        WaitForTerminalExitRequest request,
        CancellationToken cancellationToken = default)
    {
        // Wait for terminal process to exit
        // Return exit code
        return new WaitForTerminalExitResponse { ExitCode = 0 };
    }

    public async Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
        KillTerminalCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        // Kill/terminate a terminal process
        return new KillTerminalCommandResponse { };
    }

    public async Task<object> ExtMethodAsync(
        string method,
        object request,
        CancellationToken cancellationToken = default)
    {
        // Handle custom extension methods specific to your client
        throw new NotImplementedException($"Unknown extension method: {method}");
    }

    public async Task ExtNotificationAsync(
        string method,
        object notification,
        CancellationToken cancellationToken = default)
    {
        // Handle custom extension notifications
        Console.WriteLine($"Extension notification: {method}");
    }

    private async Task<PermissionOption> ShowPermissionDialogAsync(
        PermissionOption[] options,
        ToolCallUpdate toolCall)
    {
        // Implementation depends on your UI framework
        // Display toolCall.Title and options to user
        // Return the selected option or null to deny
        return options.FirstOrDefault();
    }
}
```

### 2. Connect to an Agent

```csharp
using System.Diagnostics;

// Start the agent process
var agentProcess = Process.Start(new ProcessStartInfo
{
    FileName = "my-agent.exe",
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true
});

// Create client implementation
var client = new MyAcpClient();

// Connect to the agent
var connection = Connection.ConnectToAgent(
    client,
    agentProcess.StandardInput.BaseStream,
    agentProcess.StandardOutput.BaseStream);

if (connection == null)
{
    Console.WriteLine("Failed to connect to agent");
    return;
}

// Initialize the protocol
var initResponse = await connection.InitializeAsync(
    new InitializeRequest
    {
        ProtocolVersion = ProtocolMeta.Version,
        ClientCapabilities = new ClientCapabilities
        {
            Fs = new FileSystemCapability
            {
                ReadTextFile = true,
                WriteTextFile = true
            },
            Terminal = true
        }
    });

Console.WriteLine($"Agent initialized with version {initResponse.ProtocolVersion}");

// Authenticate
var authResponse = await connection.AuthenticateAsync(
    new AuthenticateRequest
    {
        MethodId = "auth-method-1"  // Use method ID advertised by agent
    });

// Create a new session
var sessionResponse = await connection.NewSessionAsync(
    new NewSessionRequest
    {
        Cwd = Directory.GetCurrentDirectory(),
        McpServers = Array.Empty<McpServer>()
    });

var sessionId = sessionResponse.SessionId;

// Send prompts to the agent
var promptResponse = await connection.PromptAsync(
    new PromptRequest
    {
        SessionId = sessionId,
        Prompt = new ContentBlock[]
        {
            new TextContent { Text = "Help me refactor this function" }
        }
    });

Console.WriteLine($"Agent processing stopped: {promptResponse.StopReason}");
```

## Protocol Request Handlers

Your client implementation must handle the following agent requests:

### File Operations
- **`fs/read_text_file`** - Agent requests to read file content
- **`fs/write_text_file`** - Agent requests to write files

### Permission Requests
- **`session/request_permission`** - Agent asks for permission to perform actions
  - Can request file modifications, terminal creation, etc.

### Session Updates
- **`session/update`** - Agent sends session state updates and notifications

### Terminal Management
- **`terminal/create`** - Create a new terminal/shell
- **`terminal/output`** - Send output to a terminal
- **`terminal/release`** - Close/release a terminal
- **`terminal/wait_for_exit`** - Wait for terminal process to exit
- **`terminal/kill`** - Kill/terminate a terminal

## Best Practices

1. **Error Handling**: Always catch exceptions and return appropriate error responses
2. **User Permissions**: Prompt users for sensitive operations (file writes, terminal creation)
3. **File Access**: Restrict agent file access to appropriate directories
4. **Cancellation**: Respect `CancellationToken` for responsive UI
5. **Logging**: Log all agent interactions for debugging
6. **Security**: Validate agent requests before execution
7. **Resource Management**: Properly clean up processes and file handles

## License

Licensed under the Apache License 2.0. See the [LICENSE](../LICENSE) file for details.

## See Also

- [dotacp.protocol](../protocol/README.md) - Protocol definitions
- [dotacp.agent](../agent/README.md) - Agent implementation guide
- [Agent Client Protocol Specification](https://agentclientprotocol.com/)
