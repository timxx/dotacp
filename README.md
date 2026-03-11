# dotacp - Agent Client Protocol .NET SDK

[![GitHub](https://img.shields.io/badge/github-timxx/dotacp-blue)](https://github.com/timxx/dotacp)
[![License](https://img.shields.io/badge/license-Apache-green)](#license)
[![Build Status](https://github.com/timxx/dotacp/actions/workflows/build.yml/badge.svg)](https://github.com/timxx/dotacp/actions/workflows/build.yml)
[![Unit Testing](https://github.com/timxx/dotacp/actions/workflows/test.yml/badge.svg)](https://github.com/timxx/dotacp/actions/workflows/test.yml)
[![codecov](https://codecov.io/gh/timxx/dotacp/branch/main/graph/badge.svg)](https://codecov.io/gh/timxx/dotacp)
[![NuGet](https://img.shields.io/nuget/v/dotacp.protocol.svg)](https://www.nuget.org/packages/dotacp.protocol)

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

- **[`protocol/`](protocol/README.md)** - Protocol definitions and data models
  - Auto-generated schema from the ACP specification
  - Type-safe request/response types and enums
  - See [protocol README](protocol/README.md) for usage details

- **[`client/`](client/README.md)** - Client library for implementing ACP clients (editors/IDEs)
  - `IAcpClient.cs` - Core interface with all required protocol handlers
  - `Connection.cs` - Connection management and lifecycle
  - See [client README](client/README.md) for implementation guide and examples

- **[`agent/`](agent/README.md)** - Agent library for implementing ACP agents
  - `IAcpAgent.cs` - Core interface with all required protocol methods
  - `Connection.cs` - Connection management for agents
  - See [agent README](agent/README.md) for implementation guide and examples

- **`clientcli/`** - Example CLI client implementation
  - Reference implementation demonstrating how to use the client library

- **`agentcli/`** - Example CLI agent implementation
  - Reference implementation demonstrating how to use the agent library

- **`unittest/`** - Comprehensive test suite
  - Protocol conformance tests and integration tests

- **`generator/`** - Code generation utilities
  - Generates type-safe protocol models from ACP schema

## Getting Started

For detailed guidance on each component:

- **Building an Agent**: See [agent/README.md](agent/README.md)
- **Building a Client**: See [client/README.md](client/README.md)
- **Protocol Types**: See [protocol/README.md](protocol/README.md)

### Installation

```bash
dotnet add package dotacp.protocol
dotnet add package dotacp.agent
dotnet add package dotacp.client
```

Requirements:
- **.NET Standard**: 2.0 or higher
- **.NET Framework**: 4.7.2 or higher
- Dependencies: `Newtonsoft.Json` for JSON serialization

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
# Generate from current schema
pwsh ./scripts/gen-schema.ps1

# Update to latest schema
pwsh ./scripts/gen-schema.ps1 -Version main -Force
```

### Local CI-Style Build

For testing the CI build locally (with modern target frameworks), use the helper script:

```bash
# Build and test with modern TFMs enabled (default)
pwsh ./scripts/build-ci.ps1

# Debug build variant
pwsh ./scripts/build-ci.ps1 -Configuration Debug

# Disable modern TFMs (local dev mode)
pwsh ./scripts/build-ci.ps1 -EnableModernTargetFrameworks:$false
```

### Target Frameworks

- **Local/VS2019 default**: `netstandard2.0;net472` (unchanged, no new SDK required)
- **CI builds**: Adds `net10.0;net9.0;net8.0` for `client`, `agent` and `protocol` only
- **Activation**: Set `/p:EnableCiTargetFrameworks=true` in MSBuild or use the script above

## References

- [Agent Client Protocol Official Documentation](https://agentclientprotocol.com/)
- [Protocol Specification](https://agentclientprotocol.com/protocol/overview)
- [ACP GitHub Repository](https://github.com/agentclientprotocol/agent-client-protocol)
- [Model Context Protocol](https://modelcontextprotocol.io/)

## License

This project is licensed under the Apache License. See [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: https://agentclientprotocol.com/
- 🐛 **Issues**: https://github.com/timxx/dotacp/issues

---

**Made with ❤️ for the AI-assisted coding community**
