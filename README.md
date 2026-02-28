# dotacp - Agent Client Protocol .NET SDK

[![GitHub](https://img.shields.io/badge/github-timxx/dotacp-blue)](https://github.com/timxx/dotacp)
[![License](https://img.shields.io/badge/license-MIT-green)](#license)

A comprehensive .NET implementation of the [Agent Client Protocol (ACP)](https://agentclientprotocol.com/), enabling seamless communication between code editors/IDEs and AI coding agents.

## What is ACP?

The **Agent Client Protocol** is a standardized protocol for communication between code editors/IDEs and AI coding agents, similar to how the [Language Server Protocol (LSP)](https://microsoft.github.io/language-server-protocol/) standardized language server integration.

ACP solves the interoperability problem in AI-assisted coding:
- **For Agents**: Implement once, work with any compatible editor
- **For Editors**: Support any ACP-compatible agent without custom integrations
- **For Developers**: Choose the best combination of tools for your workflow

### Key Benefits
- 🔌 **Protocol Standardization**: No more custom integrations per agent-editor pair
- 🏗️ **Decoupled Architecture**: Agents and editors innovate independently
- 🌐 **Local & Remote Support**: Works with local subprocess agents and cloud-hosted services
- 📦 **MCP Integration**: Compatible with [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) for enhanced tool capabilities

## Project Structure

This repository contains a complete .NET SDK implementation with the following components:

### Core Projects

- **`protocol/`** - Protocol definitions and data models
  - Auto-generated schema from the ACP specification (v0.10.8)
  - Type-safe representations of all protocol messages
  - Support for JSON-RPC communication patterns
  - Contains type aliases, enums, request/response models, and content types

- **`client/`** - Client library for implementing ACP clients
  - `IAcpClient.cs` - Core client interface definitions
  - `Connection.cs` - Connection management and lifecycle
  - `ClientRpcTarget.cs` - RPC message routing
  - Handles authentication, session management, and bidirectional communication

- **`clientcli/`** - Example CLI client implementation
  - Demonstrates how to use the SDK to build an ACP client
  - Reference implementation for developers

- **`unittest/`** - Comprehensive test suite
  - Protocol conformance tests
  - Integration tests
  - Examples and usage patterns

- **`generator/`** - Code generation utilities
  - Maintains type-safe protocol models
  - Supports schema updates and evolution

## Quick Start

### Prerequisites
- **.NET Framework**: 4.7.2 or higher
- **.NET Standard**: 2.0 compatible
- Dependencies: `Newtonsoft.Json` for JSON serialization

### Installation

Add the NuGet package to your project:
```bash
dotnet add package dotacp.protocol
dotnet add package dotacp.client
```

Or manually reference the project:
```xml
<ItemGroup>
    <ProjectReference Include="path/to/client/client.csproj" />
    <ProjectReference Include="path/to/protocol/protocol.csproj" />
</ItemGroup>
```

### Basic Usage

#### 1. Connect to Agent

```csharp
// start the agent process
var process = ...

// connect
var connection = Connection.ConnectToAgent(
    client,
    process.StandardInput.BaseStream,
    process.StandardOutput.BaseStream);
```

#### 2. Initialize a Session

```csharp
using dotacp.protocol;
using dotacp.client;

// Create a new session
var request = new NewSessionRequest
{
    Cwd = "/path/to/project",
    McpServers = new McpServer[] { }
};

var response = await client.NewSessionAsync(request);
var sessionId = response.SessionId;
```

#### 3. Send a User Prompt

```csharp
var promptRequest = new PromptRequest
{
    SessionId = sessionId,
    Prompt = new ContentBlock[]
    {
        new TextContent 
        { 
            Text = "Help me refactor this function to use async/await" 
        }
    }
};

await client.PromptAsync(promptRequest);
```

#### 3. Handle Session Updates

```csharp
// Session notify through IAcpClient.SessionUpdateAsync
{
    switch (notification.Update)
    {
        case Plan plan:
            Console.WriteLine($"Agent plan: {plan.Entries.Length} tasks");
            break;

        case ToolCall toolCall:
            Console.WriteLine($"Tool executing: {toolCall.Title}");
            Console.WriteLine($"Status: {toolCall.Status}");
            break;

        case SessionUpdateAgentMessageChunk chunk:
            Console.WriteLine($"Agent: {chunk.Content.Text}");
            break;

        // handle other update notification
    }
};
```

#### 4. Cancel Operations

```csharp
var cancelNotification = new CancelNotification
{
    SessionId = sessionId
};

await connection.CancelAsync(cancelNotification);
```

## Architecture

### Connection Lifecycle

```
┌─────────────────────────────────────────────────────────┐
│                    Client Application                   │
└──────────────────────────┬──────────────────────────────┘
                           │
                    ┌──────▼──────┐
                    │  IAcpClient │
                    └──────┬──────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
   ┌────▼────┐      ┌─────▼─────┐      ┌────▼────┐
   │Session  │      │Connection │      │   RPC   │
   │Mgmt     │      │Management │      │ Routing │
   └────┬────┘      └─────┬─────┘      └────┬────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
                ┌──────────▼───────────┐
                │  JSON-RPC Transport  │
                │(stdio/HTTP/WebSocket)│
                └──────────┬───────────┘
                           │
                    ┌──────▼──────┐
                    │  ACP Agent  │
                    └─────────────┘
```

### Message Flow

1. **Initialization**: Client sends `initialize` request, agent responds with capabilities
2. **Session Creation**: Client creates session with `session/new`
3. **User Input**: Client sends prompts via `session/prompt`
4. **Real-time Updates**: Agent streams `session/update` notifications
5. **Tool Execution**: Agent executes tools, client may request permissions
6. **Session Termination**: Client cancels or closes session

## Examples

### Example 1: Interactive Chat Client

See `clientcli/Client.cs` for a complete example of building an interactive ACP client.

## Contributing

Contributions are welcome! Please ensure:
- Code follows the existing style conventions
- All tests pass: `dotnet test`
- Changes maintain protocol compliance
- XML documentation is updated

## Development

### Build from Source

```bash
# Clone the repository
git clone https://github.com/timxx/dotacp.git
cd dotacp

# Build the solution
dotnet build

# Run tests
dotnet test

# Build release package
dotnet pack -c Release
```

### Schema Generation

The protocol types are generated from the official ACP schema. To update:

```bash
# Run the generator
pwsh protocol\scripts\gen_all.ps1 -no-download
```

## References

- [Agent Client Protocol Official Documentation](https://agentclientprotocol.com/)
- [Protocol Specification](https://agentclientprotocol.com/protocol/overview)
- [ACP GitHub Repository](https://github.com/agentclientprotocol/agent-client-protocol)
- [Model Context Protocol](https://modelcontextprotocol.io/)

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: https://agentclientprotocol.com/
- 🐛 **Issues**: https://github.com/timxx/dotacp/issues

---

**Made with ❤️ for the AI-assisted coding community**
