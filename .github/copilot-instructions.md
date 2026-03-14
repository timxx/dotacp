# dotacp instructions for AI coding agents

- `dotacp` is a .NET SDK for ACP, not a single app. Most work falls into three layers: generated protocol surface (`protocol/`, generated files in `agent/` and `client/`), handwritten transport/runtime behavior (`shared/`, converter helpers), and schema/code generation (`generator/`).
- Do not hand-edit files marked with `Generated from schema/meta.json and schema/schema.json. Do not edit by hand.` (for example `protocol/Meta.cs`, `protocol/Schema.cs`, `agent/Connection.cs`, `client/Connection.cs`, `agent/IAcpAgent.cs`, `client/IAcpClient.cs`).
- If protocol shape changes, update `protocol/schema/` and regenerate instead of patching generated APIs directly.

## Big picture architecture

- `protocol/` contains ACP DTOs/method constants plus handwritten JSON conversion helpers (`UnionTypeConverter.cs`, `ObjectUnionConverter.cs`, `TypeAliasConverter.cs`, `DiscriminatorConverter.cs`).
- `agent/Connection.cs` and `client/Connection.cs` are mirrored wrappers over `StreamJsonRpc` (`NewLineDelimitedMessageHandler` + `JsonMessageFormatter`), each adding its generated RPC target and starting listeners.
- `shared/ExtensionMethodRoutingMessageHandler.cs` is compiled into both `agent` and `client` via `<Compile Include="..\shared\*.cs" />`; changes here affect both directions.
- Extension transport rule is repo-specific: outbound `ExtMethodAsync("foo", ...)` sends `_foo`; routing rewrites inbound `_foo` to `__acp_ext_method__`/`__acp_ext_notification__` with payload `{ method: "foo", arguments: ... }`.
- `agentcli/` and `clientcli/` are reference integrations for stdio wiring and session lifecycle; consult them before changing connection bootstrap behavior.
- Integration tests use in-memory duplex streams (`Nerdbank.Streams.FullDuplexStream.CreatePair()`) in `unittest/ConnectionPair.cs` rather than process-based tests.

## Project conventions that matter

- Language level is pinned to C# 8.0 in `Directory.Build.props`; avoid newer syntax/features.
- Libraries target `netstandard2.0;net472`; CI can append `net10.0;net9.0;net8.0` when `EnableCiTargetFrameworks=true` (`agent.csproj`, `client.csproj`). Keep shared code `netstandard2.0`-compatible.
- Nullable is enabled in `agent/`, `client/`, CLI projects, and tests; preserve nullability annotations and contracts.
- `Connection.RunAgent(...)` and `Connection.RunClient(...)` intentionally return `null` for invalid args (verified by `unittest/ConnectionFactoryTests.cs`).
- Public APIs follow async protocol handlers (`Task`/`Task<T>`) with optional `CancellationToken` and namespaces `dotacp.<project>`.

## Developer workflows

- Build all: `dotnet build dotacp.sln`
- Run tests: `dotnet test unittest/unittest.csproj`
- CI-like local pass: `pwsh ./scripts/build-ci.ps1 -Configuration Release -EnableModernTargetFrameworks:$true`
- Full regeneration (preferred): `pwsh ./scripts/gen-schema.ps1`
- Manual regeneration (when isolating failures):
	- `dotnet run --project generator -- schema --schema-dir protocol/schema --output-dir protocol`
	- `dotnet run --project generator -- meta --schema-dir protocol/schema --output-dir protocol`
	- `dotnet run --project generator -- interfaces --schema-dir protocol/schema --output-dir .`

## Editing guidance

- For any schema/protocol behavior change, inspect all three layers together: `protocol/schema/` + `generator/` + generated `agent`/`client` outputs.
- When message routing, extension behavior, or stream wiring changes, update/add tests in `unittest/AgentToClientRpcTests.cs`, `unittest/ClientToAgentRpcTests.cs`, and `unittest/ConnectionFactoryTests.cs`.
- Use `agent/README.md`, `client/README.md`, and `protocol/README.md` as canonical usage examples; keep public API behavior aligned with those docs.
