# dotacp.protocol

Type-safe .NET protocol definitions for the [Agent Client Protocol (ACP)](https://agentclientprotocol.com/).

## What is dotacp.protocol?

This package contains the core protocol definitions and data models for ACP, auto-generated from the official ACP schema. It provides:

- **Type-safe Protocol Models**: Complete C# representations of all ACP request/response types
- **Type Aliases & Enums**: Domain-specific types for protocol identifiers, modes, and options
- **JSON Serialization Support**: Built-in converters for complex types like union types and discriminated unions
- **Constants & Metadata**: Protocol version info and method name constants for both agents and clients

## Dependencies

- **.NET Standard 2.0** or higher
- `Newtonsoft.Json` (v13.0.4+) for JSON serialization

## Installation

```bash
dotnet add package dotacp.protocol
```

Or reference the project directly:

```xml
<ItemGroup>
    <ProjectReference Include="path/to/protocol/protocol.csproj" />
</ItemGroup>
```

## Usage

### Protocol Metadata

Access protocol version and method constants:

```csharp
using dotacp.protocol;

// Get protocol version
var version = ProtocolMeta.Version; // 1

// Agent method names
string initMethod = AgentMethods.Initialize;
string sessionPrompt = AgentMethods.SessionPrompt;

// Client method names
string fsRead = ClientMethods.FsReadTextFile;
string terminal = ClientMethods.TerminalCreate;
```

### Working with Protocol Messages

Use the auto-generated protocol models in your client or agent implementations:

```csharp
var initRequest = new InitializeRequest
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
};

// Serialize to JSON automatically handled by Newtonsoft.Json
var json = JsonConvert.SerializeObject(initRequest);

// Deserialize back
var deserialized = JsonConvert.DeserializeObject<InitializeRequest>(json);
```

### Type Aliases

The protocol uses type aliases for semantic clarity:

```csharp
// Permission option identifiers
var permId = new PermissionOptionId("com.example.action");

// Session identifiers
SessionId sessionId = "session-123";
```

### Union Types & Discriminated Unions

The protocol supports union types with automatic JSON conversion:

```csharp
// RequestId can be a string, number, or null
RequestId requestId = "request-1";
RequestId requestIdNum = 42L;
var requestIdNull = RequestId.Null;

// Automatically serialized correctly based on type
```

## Key Types

### Request/Response Types
- `InitializeRequest` / `InitializeResponse` - Protocol initialization
- `AuthenticateRequest` / `AuthenticateResponse` - Authentication
- `NewSessionRequest` / `NewSessionResponse` - Session creation
- `PromptRequest` / `PromptResponse` - Sending prompts to agents
- `ReadTextFileRequest` / `ReadTextFileResponse` - File operations
- `CreateTerminalRequest` / `CreateTerminalResponse` - Terminal management
- And many more...

### Notifications
- `SessionNotification` - Session updates from agents
- `CancelNotification` - Cancellation notifications
- Terminal output notifications

### Exceptions
- `AcpException` - Base exception for protocol errors

## Schema Generation

This package is auto-generated from the official ACP schema. To update it:

1. The schema source is in `schema/` directory
2. Execute the schema generation script: `./scripts/gen-schema.ps1`

The generated code includes:
- All protocol types and interfaces
- JSON converters for special types
- XML documentation comments

## Protocol Version

This package implements **ACP Protocol Version 1** based on the **ACP v0.11.2** specification.

## See Also
- [dotacp.client](../client/README.md) - Client implementation guide
- [dotacp.agent](../agent/README.md) - Agent implementation guide
- [Agent Client Protocol Specification](https://agentclientprotocol.com/)

## License

Licensed under the Apache License 2.0. See the [LICENSE](../LICENSE) file for details.
