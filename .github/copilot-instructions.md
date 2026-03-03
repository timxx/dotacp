# dotacp Codebase Guide for AI Agents

## Overview

**dotacp** is a comprehensive .NET SDK implementation of the [Agent Client Protocol (ACP)](https://agentclientprotocol.com/), enabling communication between code editors/IDEs and AI coding agents via JSON-RPC.

## Architecture & Component Structure

The solution contains 7 core projects organized by responsibility:

### Core Protocol Layer
- **`protocol/`** - Type-safe .NET models auto-generated from ACP schema v0.10.8
  - Auto-generated files have comment: "Generated from schema/meta.json and schema/schema.json. Do not edit by hand."
  - Contains: `Meta.cs` (constants), protocol enums, and request/response DTOs
  - Never manually edit protocol layer files - regenerate via `generator` project if schema changes

### Integration Libraries
- **`agent/`** - Library for building ACP agents (AI service implementations)
  - Core interface: `IAcpAgent` with lifecycle method `OnClientConnected(Connection connection)`
  - Reference: [agent/README.md](agent/README.md) for implementation patterns
  - Connection manages JSON-RPC communication and handles inbound protocol calls

- **`client/`** - Library for building ACP clients (editor/IDE implementations)
  - Core interface: `IAcpClient` with request handlers (e.g., `ReadTextFileAsync`, `WriteTextFileAsync`)
  - Reference: [client/README.md](client/README.md) for usage examples
  - Connection routes inbound requests to appropriate interface methods

### Code Generation
- **`generator/`** - CLI tool that generates protocol types, interfaces, and connections
  - Subcommands: `schema`, `meta`, `interfaces` (invoked during build)
  - Key generators: `InterfaceGenerator.cs`, `SchemaGenerator.cs`, `MetaGenerator.cs`
  - Updates are triggered by schema changes in `protocol/schema/`

### Examples & Testing
- **`clientcli/` & `agentcli/`** - Reference CLI implementations demonstrating connection setup
- **`unittest/`** - MSTest suite testing protocol conformance and generator correctness
- **`shared/`** - `ExtensionMethodRoutingMessageHandler.cs` handles custom "_" prefixed extension methods

## Critical Concepts

### Protocol Extension Methods
- Methods starting with underscore (e.g., `_custom_method`) are extension methods
- `ExtensionMethodRoutingMessageHandler` intercepts these and routes to `__acp_ext_method__` / `__acp_ext_notification__`
- Allows protocol extensions beyond the core ACP specification

### JSON-RPC Message Flow
1. Client/Agent implements interface (e.g., `IAcpAgent`)
2. `Connection.RunAgent()` / `Connection.RunClient()` wraps implementation in JSON-RPC router
3. StreamJsonRpc handles bidirectional message serialization over streams (stdin/stdout)
4. Messages use `NewLineDelimitedMessageHandler` + `JsonMessageFormatter` (Newtonsoft.Json)

### Auto-Generated Code Pattern
Generated files are marked with header comment. Do NOT manually edit:
- `client/ClientRpcTarget.cs` - Routes inbound RPC calls to IAcpClient
- `agent/AgentRpcTarget.cs` - Routes inbound RPC calls to IAcpAgent
- `client/Connection.cs` & `agent/Connection.cs` - RPC method signatures for client→agent communication
- Protocol DTOs in `protocol/` - All request/response/notification types

## Development Workflows

### Building & Testing
```bash
# One-time setup (restore NuGet packages)
dotnet restore

# Build solution
dotnet build

# Run all unit tests (MSTest)
dotnet test

# CI build with modern target frameworks (run from PowerShell)
.\scripts\build-ci.ps1 -Configuration Release -EnableModernTargetFrameworks $true
```

### Regenerating Protocol Types
When `protocol/schema/schema.json` or `protocol/schema/meta.json` changes:
```bash
cd generator
dotnet run -- schema --schema-dir ../protocol/schema --output-dir ../protocol
dotnet run -- meta --schema-dir ../protocol/schema --output-dir ../protocol
dotnet run -- interfaces --schema-dir ../protocol/schema --output-dir ..
```

### Adding New Protocol Methods
1. Update `protocol/schema/schema.json` per ACP spec
2. Run generator commands above to regenerate types
3. Update `IAcpClient` or `IAcpAgent` interface implementations with new method handlers
4. Add test cases in `unittest/`

## Code Style & Conventions

- **Language version**: C# 8.0 (`LangVersion` in Directory.Build.props)
- **Nullable context**: Enabled globally - use `?` for nullable reference types
- **Implicit usings**: Disabled - all namespaces explicitly included
- **JSON serialization**: Newtonsoft.Json with custom converters for union types
- **Namespacing**: `dotacp.<ProjectName>` (e.g., `dotacp.protocol`, `dotacp.agent`)
- **Custom attributes**: `[JsonEnumMemberAttribute]` for enum mapping, `[TypeAliasConverter]` for type aliases

## Key Files Reference

| Purpose | Files |
|---------|-------|
| Protocol constants & methods | `protocol/Meta.cs`, `protocol/Schema.cs` |
| Client contract | `client/IAcpClient.cs`, `client/ClientRpcTarget.cs` |
| Agent contract | `agent/IAcpAgent.cs`, `agent/AgentRpcTarget.cs` |
| RPC communication primitives | `client/Connection.cs`, `agent/Connection.cs` |
| Extension method routing | `shared/ExtensionMethodRoutingMessageHandler.cs` |
| Generator orchestration | `generator/Program.cs`, `generator/InterfaceGenerator.cs` |
| CLI examples | `clientcli/Client.cs`, `agentcli/Agent.cs` |

## Important Patterns to Follow

1. **Async/await throughout** - All protocol methods return `Task<T>` and accept `CancellationToken`
2. **Null safety** - Null checks in Connection factory methods (return null on invalid args)
3. **Extension method naming** - Use `_` prefix for custom protocol methods
4. **Interface preservation** - Don't modify `IAcpClient`/`IAcpAgent` directly; use generators
5. **StreamJsonRpc integration** - Use provided `Connection` classes; don't create JsonRpc manually
6. **Test organization** - Unit tests follow `<Component><Tested>Tests.cs` naming pattern

## When Implementing New Features

- **Adding protocol handlers**: Implement method in `IAcpClient` or `IAcpAgent`, regenerate interfaces
- **Modifying protocol**: Update schema → regenerate all → update implementations
- **Custom serialization**: Create custom converter inheriting `JsonConverter` in `protocol/` (see `UnionTypeConverter.cs`)
- **Breaking changes**: Document in protocol version (currently v0.10.8)
