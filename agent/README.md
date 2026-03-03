# dotacp.agent

.NET library for implementing [Agent Client Protocol (ACP)](https://agentclientprotocol.com/) agents.

## What is dotacp.agent?

This package provides the tools needed to build ACP agent implementations in .NET. It includes:

- **IAcpAgent Interface**: Core interface with all required protocol methods
- **Connection Management**: Handles JSON-RPC communication with ACP clients
- **RPC Message Routing**: Automatic dispatch of inbound protocol calls

## Dependencies

- **.NET Standard 2.0** or higher
- `dotacp.protocol` - Protocol type definitions
- `StreamJsonRpc` (v2.7.76+) - JSON-RPC communication

## Installation

```bash
dotnet add package dotacp.agent
```

Or reference the project directly:

```xml
<ItemGroup>
    <ProjectReference Include="path/to/agent/agent.csproj" />
</ItemGroup>
```

## Quick Start

### 1. Implement the IAcpAgent Interface

```csharp
using dotacp.agent;
using dotacp.protocol;
using System.Threading;
using System.Threading.Tasks;

public class MyAcpAgent : IAcpAgent
{
    private Connection? _connection;

    public void OnClientConnected(Connection connection)
    {
        _connection = connection;
        // Agent is now connected to the client
    }

    public async Task<InitializeResponse> InitializeAsync(
        InitializeRequest request,
        CancellationToken cancellationToken = default)
    {
        return new InitializeResponse
        {
            ProtocolVersion = ProtocolMeta.Version,
            AgentCapabilities = new AgentCapabilities
            {
                LoadSession = true,
                PromptCapabilities = new PromptCapabilities
                {
                    Audio = false,
                    Image = true,
                    EmbeddedContext = true
                }
            }
        };
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default)
    {
        // Implement your authentication logic
        // Use request.MethodId to identify which auth method is being used
        return new AuthenticateResponse();
    }

    public async Task<NewSessionResponse> NewSessionAsync(
        NewSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        // request.Cwd contains the working directory
        // request.McpServers contains MCP server configurations
        var sessionId = new SessionId(Guid.NewGuid().ToString());
        return new NewSessionResponse
        {
            SessionId = sessionId,
            ConfigOptions = new SessionConfigOption[] { },
            Modes = new SessionModeState
            {
                AvailableModes = new SessionMode[] { },
                CurrentModeId = new SessionModeId("default")
            }
        };
    }

    public async Task<LoadSessionResponse> LoadSessionAsync(
        LoadSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Load and restore session state
        return new LoadSessionResponse { /* ... */ };
    }

    public async Task<PromptResponse> PromptAsync(
        PromptRequest request,
        CancellationToken cancellationToken = default)
    {
        // Process the prompt from request.Prompt (array of ContentBlock)
        // The session ID is in request.SessionId
        // Return the stop reason indicating why the agent stopped processing
        return new PromptResponse
        {
            StopReason = StopReason.EndTurn
        };
    }

    public async Task<SetSessionConfigOptionResponse> SetSessionConfigOptionAsync(
        SetSessionConfigOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Update session configuration
        return new SetSessionConfigOptionResponse { };
    }

    public async Task<SetSessionModeResponse> SetSessionModeAsync(
        SetSessionModeRequest request,
        CancellationToken cancellationToken = default)
    {
        // Handle session mode changes
        return new SetSessionModeResponse { };
    }

    public async Task CancelAsync(
        CancelNotification notification,
        CancellationToken cancellationToken = default)
    {
        // Handle cancellation requests
    }

    public async Task<object> ExtMethodAsync(
        string method,
        object request,
        CancellationToken cancellationToken = default)
    {
        // Handle custom extension methods
        throw new NotImplementedException($"Unknown extension method: {method}");
    }
}
```

### 2. Connect the Agent

```csharp
using System.Diagnostics;

// Start the agent process (in a host application)
var agent = new MyAcpAgent();
var process = Process.Start(/* your agent executable */);

// Create the connection
var connection = Connection.RunAgent(
    agent,
    process.StandardInput.BaseStream,
    process.StandardOutput.BaseStream);

// Wait for the connection to close
await connection.Completion;
```

## Protocol Methods

Your implementation must handle the following methods:

### Agent Methods (called by client)
- **`initialize`** - Handshake and capability negotiation
- **`authenticate`** - User authentication
- **`session/new`** - Create a new session
- **`session/load`** - Load an existing session
- **`session/prompt`** - Process a user prompt
- **`session/set_config_option`** - Update session settings
- **`session/set_mode`** - Change session mode
- **`session/cancel`** - Cancel an ongoing operation

### Using the Connection

After initialization, use the `Connection` object to call client methods:

```csharp
// Request permission for an action
var permissionResponse = await _connection!.RequestPermissionAsync(
    new RequestPermissionRequest
    {
        SessionId = sessionId,
        Options = new PermissionOption[]
        {
            new PermissionOption
            {
                OptionId = "approve",
                Name = "Approve",
                Kind = PermissionOptionKind.AllowOnce
            }
        },
        ToolCall = new ToolCallUpdate
        {
            Title = "Create file",
            Kind = ToolKind.Tool,
            Status = ToolCallStatus.InProgress
        }
    },
    cancellationToken);

// Write a file
await _connection!.WriteTextFileAsync(
    new WriteTextFileRequest
    {
        SessionId = sessionId,
        Path = "/tmp/output.txt",
        Content = "File content here"
    },
    cancellationToken);

// Create a terminal
var terminalResponse = await _connection!.CreateTerminalAsync(
    new CreateTerminalRequest
    {
        SessionId = sessionId,
        Label = "Build Terminal"
    },
    cancellationToken);

// Send terminal output
await _connection!.TerminalOutputAsync(
    new TerminalOutputRequest
    {
        TerminalId = terminalResponse.TerminalId,
        Output = "Build command output here"
    },
    cancellationToken);

// Send session updates
await _connection!.SessionUpdateAsync(
    new SessionNotification
    {
        SessionId = sessionId,
        State = new SessionState { /* ... */ }
    },
    cancellationToken);
```

## Error Handling

Implement proper error handling in your protocol method implementations:

```csharp
public async Task<PromptResponse> PromptAsync(
    PromptRequest request,
    CancellationToken cancellationToken = default)
{
    try
    {
        // Process the prompt
        return new PromptResponse { /* ... */ };
    }
    catch (OperationCanceledException)
    {
        // Handle cancellation
        throw;
    }
    catch (Exception ex)
    {
        // Log and handle errors appropriately
        // The RPC layer will marshal exceptions back to the client
        throw;
    }
}
```

## Extensions

Support custom extension methods not in the core protocol:

```csharp
public async Task<object> ExtMethodAsync(
    string method,
    object request,
    CancellationToken cancellationToken = default)
{
    switch (method)
    {
        case "custom.analyze":
            // Handle custom analyze method
            return new { result = "analyzed" };
        
        default:
            throw new NotImplementedException($"Unknown method: {method}");
    }
}
```

## Best Practices

1. **Cancellation Tokens**: Always respect `CancellationToken` in async methods
2. **Error Handling**: Properly handle and log exceptions
3. **Resource Management**: Clean up resources in `OnClientConnected` if needed
4. **Timeouts**: Consider implementing timeouts for long-running operations
5. **Logging**: Use structured logging to track protocol interactions

## License

Licensed under the Apache License 2.0. See the [LICENSE](../LICENSE) file for details.

## See Also

- [dotacp.protocol](../protocol/README.md) - Protocol definitions
- [dotacp.client](../client/README.md) - Client implementation guide
- [Agent Client Protocol Specification](https://agentclientprotocol.com/)
