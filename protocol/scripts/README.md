# ACP Code Generation Scripts

This directory contains PowerShell scripts to generate C# code from the [Agent Client Protocol (ACP)](https://agentclientprotocol.com/) JSON schema.

## Overview

The **Agent Client Protocol** is a standardized protocol that enables communication between code editors (like Zed) and AI coding agents. This code generation system automatically generates strongly-typed C# models from the ACP specification schema.

### What is ACP?

ACP allows:
- **Editors** (clients) to connect to **AI Agents** via a standardized protocol
- Structured communication for code editing operations (read, write, search, etc.)
- Tool invocation and capabilities negotiation
- Session management and streaming responses

The protocol is defined in JSON Schema format and maintained at https://github.com/agentclientprotocol/agent-client-protocol.

## Scripts

### `gen_all.ps1`
Main orchestration script that runs the complete code generation pipeline.

**Usage:**
```powershell
# Generate using local schema files
./gen_all.ps1

# Download and generate specific schema version
./gen_all.ps1 -Version "v0.10.8"

# Generate from main branch
./gen_all.ps1 -Version main

# Force re-download
./gen_all.ps1 -Version "v0.10.8" -Force

# Use environment variables
$env:ACP_SCHEMA_VERSION = "v0.10.8"
./gen_all.ps1
```

**Environment Variables:**
- `ACP_SCHEMA_VERSION`: Git ref (tag/branch) to fetch schema from
- `ACP_SCHEMA_REPO`: Repository containing schema (default: `agentclientprotocol/agent-client-protocol`)
- `ACP_SCHEMA_DOWNLOAD`: Force download (set to "1", "true", or "yes")

### `gen_schema.ps1`
Generates C# model classes from `schema/schema.json`.

**Produces:** `protocol/Schema.cs`

**Features:**
- Converts JSON Schema definitions to C# classes
- Snake_case property names → PascalCase
- Handles type conversions (JSON types → C# types)
- Supports schema references ($ref) and nested types
- Handles discriminator-based unions as abstract base classes with concrete variants
- Generates XML documentation comments
- Handles nullable types and defaults
- Uses Newtonsoft.Json attributes and converters for serialization

**Type Mapping:**
| JSON Schema | C# |
|------------|-----|
| `string` | `string` |
| `integer` | `int` |
| `number` | `double` |
| `boolean` | `bool` |
| `object` | `object` |
| `array` | `List<T>` |
| `null` | `object` |

### `gen_meta.ps1`
Generates protocol metadata from `schema/meta.json`.

**Produces:** `protocol/Meta.cs`

**Contains:**
- `ProtocolVersion`: ACP protocol version number
- `AgentMethods`: Dictionary of methods agents handle
  - `authentication`
  - `initialize`
  - `session/*` operations
  - etc.
- `ClientMethods`: Dictionary of methods clients handle
  - `fs/*` operations (file system)
  - `terminal/*` operations
  - `session/request_permission`
  - etc.

## Schema Files

### `schema/schema.json`
Complete JSON Schema definition of ACP protocol messages, including:
- Message types (InitializeRequest, PromptRequest, etc.)
- Content blocks (text, image, audio, embedded resources)
- Tool definitions and permissions
- Session configuration
- Capabilities and error types

### `schema/meta.json`
Metadata about the protocol:
- Agent vs Client method mappings
- Protocol version

### `schema/VERSION`
Git ref used to fetch the current schema (auto-generated)

## Workflow

### Quick Start

```powershell
cd protocol/scripts
./gen_all.ps1
```

This will:
1. Use existing local schema files (or download if missing)
2. Parse schema.json and generate Schema.cs
3. Parse meta.json and generate Meta.cs
4. Output generated files to `protocol/`

### Update to Latest Schema

```powershell
./gen_all.ps1 -Version main
```

Downloads latest schema from upstream and regenerates code.

### Update to Specific Release

```powershell
./gen_all.ps1 -Version "v0.10.8"
```

### CI/CD Integration

Add to your build or setup process:

```powershell
# In your build script or GitHub Actions workflow
cd protocol/scripts
./gen_all.ps1
```

## Generated Files

After running `gen_all.ps1`, the following files are generated:

- **`protocol/Schema.cs`** - C# records for all ACP types (~100+ types)
- **`protocol/Meta.cs`** - Protocol metadata and method mappings

## Understanding the ACP Protocol

### Key Concepts

1. **Messages**: Request-response pairs between editor and agent
   - `InitializeRequest` / `InitializeResponse`
   - `PromptRequest` / `PromptResponse`
   - `SessionUpdateNotification`

2. **Content Blocks**: Different types of content (text, images, audio, resources)
  - `TextContent`
  - `ImageContent`
  - `AudioContent`
  - `EmbeddedResource`

3. **Tools**: Actions agents can perform
   - File operations (read, write)
   - Terminal commands
   - Search operations
   - Custom tools

4. **Capabilities**: Negotiated between editor and agent
   - MCP (Model Context Protocol) support
   - Prompt types supported
   - Session features supported

### Protocol Flow

```
Editor                          Agent
  |                              |
  |--- InitializeRequest ------->|
  |                              |
  |<---- InitializeResponse -----|
  |                              |
  |--- PromptRequest ----------->|
  |                              |
  |<---- SessionUpdateNotification (chunks)
  |                              |
  |<---- PromptResponse ----------|
  |                              |
```

## Advanced Usage

### Custom Schema Post-Processing

To add custom transformations to generated classes:

1. Edit the post-processing functions in `gen_schema.ps1`
2. Add new naming conventions or type mappings
3. Re-run `gen_all.ps1`

### Integration with Build System

Add to your `.csproj`:

```xml
<Target Name="GenerateAcpSchema" BeforeTargets="Build">
  <Exec Command="pwsh -File protocol/scripts/gen_all.ps1" />
</Target>
```

Or create a setup target:

```xml
<Target Name="SetupAcp">
  <Exec Command="pwsh -File protocol/scripts/gen_all.ps1" />
</Target>
```

Then run: `dotnet build -t SetupAcp`

## Troubleshooting

### Scripts won't run: "cannot be loaded because running scripts is disabled"

Enable script execution:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Schema download fails

Check network connectivity and repository URL:
```powershell
./gen_all.ps1 -Version main -Verbose
```

### Missing schema files

Download explicitly:
```powershell
./gen_all.ps1 -Version main
```

## References

- [Agent Client Protocol](https://agentclientprotocol.com/)
- [ACP Specification](https://github.com/agentclientprotocol/agent-client-protocol)
- [Python SDK Implementation](https://github.com/agentclientprotocol/python-sdk)
- [TypeScript SDK Implementation](https://github.com/agentclientprotocol/typescript-sdk)

## Contributing

To improve the code generation:

1. Update type mappings or generation logic in the `.ps1` scripts
2. Test with `./gen_all.ps1`
3. Review generated code in `protocol/Schema.cs` and `protocol/Meta.cs`
4. Submit issues or improvements to the repository
