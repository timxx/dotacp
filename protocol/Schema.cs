// Generated from schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.10.8

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace dotacp.protocol
{
    // Type aliases

    /// <summary>
    /// Unique identifier for a permission option.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<PermissionOptionId, string>))]
    public readonly struct PermissionOptionId : IEquatable<PermissionOptionId>
    {
        private readonly string _value;

        public PermissionOptionId(string value)
        {
            _value = value;
        }

        public static implicit operator PermissionOptionId(string value) => new PermissionOptionId(value);
        public static implicit operator string(PermissionOptionId alias) => alias._value;

        public bool Equals(PermissionOptionId other) => _value == other._value;
        public override bool Equals(object obj) => obj is PermissionOptionId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Protocol version identifier.
    ///
    /// This version is only bumped for breaking changes.
    /// Non-breaking changes should be introduced via capabilities.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<ProtocolVersion, ushort>))]
    public readonly struct ProtocolVersion : IEquatable<ProtocolVersion>
    {
        private readonly ushort _value;

        public ProtocolVersion(ushort value)
        {
            _value = value;
        }

        public static implicit operator ProtocolVersion(ushort value) => new ProtocolVersion(value);
        public static implicit operator ushort(ProtocolVersion alias) => alias._value;

        public bool Equals(ProtocolVersion other) => _value == other._value;
        public override bool Equals(object obj) => obj is ProtocolVersion other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();
    }

    /// <summary>
    /// JSON RPC Request Id
    ///
    /// An identifier established by the Client that MUST contain a String, Number, or NULL value if included. If it is not included it is assumed to be a notification. The value SHOULD normally not be Null [1] and Numbers SHOULD NOT contain fractional parts [2]
    ///
    /// The Server MUST reply with the same value in the Response object if included. This member is used to correlate the context between the two objects.
    ///
    /// [1] The use of Null as a value for the id member in a Request object is discouraged, because this specification uses a value of Null for Responses with an unknown id. Also, because JSON-RPC 1.0 uses an id value of Null for Notifications this could cause confusion in handling.
    ///
    /// [2] Fractional parts may be problematic, since many decimal fractions cannot be represented exactly as binary fractions.
    /// </summary>
    [JsonConverter(typeof(UnionTypeConverter<RequestId>))]
    public readonly struct RequestId : IEquatable<RequestId>
    {
        private readonly object _value;
        private readonly int _typeIndex;
        private readonly bool _isNull;

        public RequestId(long value)
        {
            _value = value;
            _typeIndex = 0;
            _isNull = false;
        }

        public RequestId(string value)
        {
            _value = value;
            _typeIndex = 1;
            _isNull = false;
        }

        private RequestId(bool isNull)
        {
            _value = null;
            _typeIndex = -1;
            _isNull = isNull;
        }

        public static RequestId Null => new RequestId(true);

        public static implicit operator RequestId(long value) => new RequestId(value);
        public static implicit operator RequestId(string value) => new RequestId(value);

        public bool IsNull => _isNull;

        public bool TryGetLong(out long value)
        {
            if (_isNull)
            {
                value = default;
                return false;
            }
            if (_value is long v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetString(out string value)
        {
            if (_isNull)
            {
                value = default;
                return false;
            }
            if (_value is string v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool Equals(RequestId other) => _isNull == other._isNull && (_isNull || (Equals(_value, other._value) && _typeIndex == other._typeIndex));
        public override bool Equals(object obj) => obj is RequestId other && Equals(other);
        public override int GetHashCode()
        {
            if (_isNull) return 0;
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);
                hash = hash * 31 + _typeIndex;
                return hash;
            }
        }
        public override string ToString() => _isNull ? string.Empty : (_value?.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Unique identifier for a session configuration option value group.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<SessionConfigGroupId, string>))]
    public readonly struct SessionConfigGroupId : IEquatable<SessionConfigGroupId>
    {
        private readonly string _value;

        public SessionConfigGroupId(string value)
        {
            _value = value;
        }

        public static implicit operator SessionConfigGroupId(string value) => new SessionConfigGroupId(value);
        public static implicit operator string(SessionConfigGroupId alias) => alias._value;

        public bool Equals(SessionConfigGroupId other) => _value == other._value;
        public override bool Equals(object obj) => obj is SessionConfigGroupId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Unique identifier for a session configuration option.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<SessionConfigId, string>))]
    public readonly struct SessionConfigId : IEquatable<SessionConfigId>
    {
        private readonly string _value;

        public SessionConfigId(string value)
        {
            _value = value;
        }

        public static implicit operator SessionConfigId(string value) => new SessionConfigId(value);
        public static implicit operator string(SessionConfigId alias) => alias._value;

        public bool Equals(SessionConfigId other) => _value == other._value;
        public override bool Equals(object obj) => obj is SessionConfigId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Possible values for a session configuration option.
    /// </summary>
    [JsonConverter(typeof(UnionTypeConverter<SessionConfigSelectOptions>))]
    public readonly struct SessionConfigSelectOptions : IEquatable<SessionConfigSelectOptions>
    {
        private readonly object _value;
        private readonly int _typeIndex;

        public SessionConfigSelectOptions(object[] value)
        {
            _value = value;
            _typeIndex = 0;
        }

        public static implicit operator SessionConfigSelectOptions(object[] value) => new SessionConfigSelectOptions(value);

        public bool TryGetObject(out object[] value)
        {
            if (_value is object[] v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool Equals(SessionConfigSelectOptions other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;
        public override bool Equals(object obj) => obj is SessionConfigSelectOptions other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);
                hash = hash * 31 + _typeIndex;
                return hash;
            }
        }
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Unique identifier for a session configuration option value.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<SessionConfigValueId, string>))]
    public readonly struct SessionConfigValueId : IEquatable<SessionConfigValueId>
    {
        private readonly string _value;

        public SessionConfigValueId(string value)
        {
            _value = value;
        }

        public static implicit operator SessionConfigValueId(string value) => new SessionConfigValueId(value);
        public static implicit operator string(SessionConfigValueId alias) => alias._value;

        public bool Equals(SessionConfigValueId other) => _value == other._value;
        public override bool Equals(object obj) => obj is SessionConfigValueId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// A unique identifier for a conversation session between a client and agent.
    ///
    /// Sessions maintain their own context, conversation history, and state,
    /// allowing multiple independent interactions with the same agent.
    ///
    /// See protocol docs: [Session ID](https://agentclientprotocol.com/protocol/session-setup#session-id)
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<SessionId, string>))]
    public readonly struct SessionId : IEquatable<SessionId>
    {
        private readonly string _value;

        public SessionId(string value)
        {
            _value = value;
        }

        public static implicit operator SessionId(string value) => new SessionId(value);
        public static implicit operator string(SessionId alias) => alias._value;

        public bool Equals(SessionId other) => _value == other._value;
        public override bool Equals(object obj) => obj is SessionId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Unique identifier for a Session Mode.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<SessionModeId, string>))]
    public readonly struct SessionModeId : IEquatable<SessionModeId>
    {
        private readonly string _value;

        public SessionModeId(string value)
        {
            _value = value;
        }

        public static implicit operator SessionModeId(string value) => new SessionModeId(value);
        public static implicit operator string(SessionModeId alias) => alias._value;

        public bool Equals(SessionModeId other) => _value == other._value;
        public override bool Equals(object obj) => obj is SessionModeId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Unique identifier for a tool call within a session.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<ToolCallId, string>))]
    public readonly struct ToolCallId : IEquatable<ToolCallId>
    {
        private readonly string _value;

        public ToolCallId(string value)
        {
            _value = value;
        }

        public static implicit operator ToolCallId(string value) => new ToolCallId(value);
        public static implicit operator string(ToolCallId alias) => alias._value;

        public bool Equals(ToolCallId other) => _value == other._value;
        public override bool Equals(object obj) => obj is ToolCallId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    // Enums for string-based enum-like types

    /// <summary>
    /// Predefined error codes for common JSON-RPC and ACP-specific errors.
    ///
    /// These codes follow the JSON-RPC 2.0 specification for standard errors
    /// and use the reserved range (-32000 to -32099) for protocol-specific errors.
    /// </summary>
    public enum ErrorCode : int
    {
        /// <summary>
        /// **Parse error**: Invalid JSON was received by the server.
        /// An error occurred on the server while parsing the JSON text.
        /// </summary>
        ParseError = -32700,

        /// <summary>
        /// **Invalid request**: The JSON sent is not a valid Request object.
        /// </summary>
        InvalidRequest = -32600,

        /// <summary>
        /// **Method not found**: The method does not exist or is not available.
        /// </summary>
        MethodNotFound = -32601,

        /// <summary>
        /// **Invalid params**: Invalid method parameter(s).
        /// </summary>
        InvalidParams = -32602,

        /// <summary>
        /// **Internal error**: Internal JSON-RPC error.
        /// Reserved for implementation-defined server errors.
        /// </summary>
        InternalError = -32603,

        /// <summary>
        /// **Authentication required**: Authentication is required before this operation can be performed.
        /// </summary>
        AuthenticationRequired = -32000,

        /// <summary>
        /// **Resource not found**: A given resource, such as a file, was not found.
        /// </summary>
        ResourceNotFound = -32002,

        /// <summary>
        /// Other undefined error code.
        /// </summary>
        Other = 0
    }

    /// <summary>
    /// The type of permission option being presented to the user.
    ///
    /// Helps clients choose appropriate icons and UI treatment.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<PermissionOptionKind>))]
    public enum PermissionOptionKind
    {
        /// <summary>
        /// Allow this operation only this time.
        /// </summary>
        [JsonEnumValue("allow_once")]
        AllowOnce,

        /// <summary>
        /// Allow this operation and remember the choice.
        /// </summary>
        [JsonEnumValue("allow_always")]
        AllowAlways,

        /// <summary>
        /// Reject this operation only this time.
        /// </summary>
        [JsonEnumValue("reject_once")]
        RejectOnce,

        /// <summary>
        /// Reject this operation and remember the choice.
        /// </summary>
        [JsonEnumValue("reject_always")]
        RejectAlways
    }

    /// <summary>
    /// Priority levels for plan entries.
    ///
    /// Used to indicate the relative importance or urgency of different
    /// tasks in the execution plan.
    /// See protocol docs: [Plan Entries](https://agentclientprotocol.com/protocol/agent-plan#plan-entries)
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<PlanEntryPriority>))]
    public enum PlanEntryPriority
    {
        /// <summary>
        /// High priority task - critical to the overall goal.
        /// </summary>
        [JsonEnumValue("high")]
        High,

        /// <summary>
        /// Medium priority task - important but not critical.
        /// </summary>
        [JsonEnumValue("medium")]
        Medium,

        /// <summary>
        /// Low priority task - nice to have but not essential.
        /// </summary>
        [JsonEnumValue("low")]
        Low
    }

    /// <summary>
    /// Status of a plan entry in the execution flow.
    ///
    /// Tracks the lifecycle of each task from planning through completion.
    /// See protocol docs: [Plan Entries](https://agentclientprotocol.com/protocol/agent-plan#plan-entries)
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<PlanEntryStatus>))]
    public enum PlanEntryStatus
    {
        /// <summary>
        /// The task has not started yet.
        /// </summary>
        [JsonEnumValue("pending")]
        Pending,

        /// <summary>
        /// The task is currently being worked on.
        /// </summary>
        [JsonEnumValue("in_progress")]
        InProgress,

        /// <summary>
        /// The task has been successfully completed.
        /// </summary>
        [JsonEnumValue("completed")]
        Completed
    }

    /// <summary>
    /// The sender or recipient of messages and data in a conversation.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<Role>))]
    public enum Role
    {
        [JsonEnumValue("assistant")]
        Assistant,

        [JsonEnumValue("user")]
        User
    }

    /// <summary>
    /// Semantic category for a session configuration option.
    ///
    /// This is intended to help Clients distinguish broadly common selectors (e.g. model selector vs
    /// session mode selector vs thought/reasoning level) for UX purposes (keyboard shortcuts, icons,
    /// placement). It MUST NOT be required for correctness. Clients MUST handle missing or unknown
    /// categories gracefully.
    ///
    /// Category names beginning with `_` are free for custom use, like other ACP extension methods.
    /// Category names that do not begin with `_` are reserved for the ACP spec.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<SessionConfigOptionCategory>))]
    public enum SessionConfigOptionCategory
    {
        /// <summary>
        /// Session mode selector.
        /// </summary>
        [JsonEnumValue("mode")]
        Mode,

        /// <summary>
        /// Model selector.
        /// </summary>
        [JsonEnumValue("model")]
        Model,

        /// <summary>
        /// Thought/reasoning level selector.
        /// </summary>
        [JsonEnumValue("thought_level")]
        ThoughtLevel,

        /// <summary>
        /// Unknown / uncategorized selector.
        /// </summary>
        [JsonEnumValue("other")]
        Other
    }

    /// <summary>
    /// Reasons why an agent stops processing a prompt turn.
    ///
    /// See protocol docs: [Stop Reasons](https://agentclientprotocol.com/protocol/prompt-turn#stop-reasons)
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<StopReason>))]
    public enum StopReason
    {
        /// <summary>
        /// The turn ended successfully.
        /// </summary>
        [JsonEnumValue("end_turn")]
        EndTurn,

        /// <summary>
        /// The turn ended because the agent reached the maximum number of tokens.
        /// </summary>
        [JsonEnumValue("max_tokens")]
        MaxTokens,

        /// <summary>
        /// The turn ended because the agent reached the maximum number of allowed
        /// agent requests between user turns.
        /// </summary>
        [JsonEnumValue("max_turn_requests")]
        MaxTurnRequests,

        /// <summary>
        /// The turn ended because the agent refused to continue. The user prompt
        /// and everything that comes after it won't be included in the next
        /// prompt, so this should be reflected in the UI.
        /// </summary>
        [JsonEnumValue("refusal")]
        Refusal,

        /// <summary>
        /// The turn was cancelled by the client via `session/cancel`.
        ///
        /// This stop reason MUST be returned when the client sends a `session/cancel`
        /// notification, even if the cancellation causes exceptions in underlying operations.
        /// Agents should catch these exceptions and return this semantically meaningful
        /// response to confirm successful cancellation.
        /// </summary>
        [JsonEnumValue("cancelled")]
        Cancelled
    }

    /// <summary>
    /// Execution status of a tool call.
    ///
    /// Tool calls progress through different statuses during their lifecycle.
    ///
    /// See protocol docs: [Status](https://agentclientprotocol.com/protocol/tool-calls#status)
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<ToolCallStatus>))]
    public enum ToolCallStatus
    {
        /// <summary>
        /// The tool call hasn't started running yet because the input is either
        /// streaming or we're awaiting approval.
        /// </summary>
        [JsonEnumValue("pending")]
        Pending,

        /// <summary>
        /// The tool call is currently running.
        /// </summary>
        [JsonEnumValue("in_progress")]
        InProgress,

        /// <summary>
        /// The tool call completed successfully.
        /// </summary>
        [JsonEnumValue("completed")]
        Completed,

        /// <summary>
        /// The tool call failed with an error.
        /// </summary>
        [JsonEnumValue("failed")]
        Failed
    }

    /// <summary>
    /// Categories of tools that can be invoked.
    ///
    /// Tool kinds help clients choose appropriate icons and optimize how they
    /// display tool execution progress.
    ///
    /// See protocol docs: [Creating](https://agentclientprotocol.com/protocol/tool-calls#creating)
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<ToolKind>))]
    public enum ToolKind
    {
        /// <summary>
        /// Reading files or data.
        /// </summary>
        [JsonEnumValue("read")]
        Read,

        /// <summary>
        /// Modifying files or content.
        /// </summary>
        [JsonEnumValue("edit")]
        Edit,

        /// <summary>
        /// Removing files or data.
        /// </summary>
        [JsonEnumValue("delete")]
        Delete,

        /// <summary>
        /// Moving or renaming files.
        /// </summary>
        [JsonEnumValue("move")]
        Move,

        /// <summary>
        /// Searching for information.
        /// </summary>
        [JsonEnumValue("search")]
        Search,

        /// <summary>
        /// Running commands or code.
        /// </summary>
        [JsonEnumValue("execute")]
        Execute,

        /// <summary>
        /// Internal reasoning or planning.
        /// </summary>
        [JsonEnumValue("think")]
        Think,

        /// <summary>
        /// Retrieving external data.
        /// </summary>
        [JsonEnumValue("fetch")]
        Fetch,

        /// <summary>
        /// Switching the current session mode.
        /// </summary>
        [JsonEnumValue("switch_mode")]
        SwitchMode,

        /// <summary>
        /// Other tool types (default).
        /// </summary>
        [JsonEnumValue("other")]
        Other
    }

    // Generated model classes from ACP schema

    /// <summary>
    /// Capabilities supported by the agent.
    ///
    /// Advertised during initialization to inform the client about
    /// available features and content types.
    ///
    /// See protocol docs: [Agent Capabilities](https://agentclientprotocol.com/protocol/initialization#agent-capabilities)
    /// </summary>
    public class AgentCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Whether the agent supports `session/load`.
        /// </summary>
        [JsonProperty("loadSession")]
        public bool LoadSession { get; set; } = false;

        /// <summary>
        /// MCP capabilities supported by the agent.
        /// </summary>
        [JsonProperty("mcpCapabilities")]
        public McpCapabilities McpCapabilities { get; set; }

        /// <summary>
        /// Prompt capabilities supported by the agent.
        /// </summary>
        [JsonProperty("promptCapabilities")]
        public PromptCapabilities PromptCapabilities { get; set; }

        [JsonProperty("sessionCapabilities")]
        public SessionCapabilities SessionCapabilities { get; set; }
    }

    public class AgentNotification
    {
        [JsonProperty("method")]
        public string Method { get; set; } = null!;

        [JsonProperty("params")]
        public object Params { get; set; }
    }

    public class AgentRequest
    {
        [JsonProperty("id")]
        public RequestId Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; } = null!;

        [JsonProperty("params")]
        public object Params { get; set; }
    }

    public class AgentResponse
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("id")]
        public RequestId Id { get; set; }

        /// <summary>
        /// All possible responses that an agent can send to a client.
        ///
        /// This enum is used internally for routing RPC responses. You typically won't need
        /// to use this directly - the responses are handled automatically by the connection.
        ///
        /// These are responses to the corresponding `ClientRequest` variants.
        /// </summary>
        [JsonProperty("result")]
        public object Result { get; set; }
    }

    /// <summary>
    /// Optional annotations for the client. The client can use annotations to inform how objects are used or displayed
    /// </summary>
    public class Annotations
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("audience")]
        public Role[] Audience { get; set; }

        [JsonProperty("lastModified")]
        public string LastModified { get; set; }

        [JsonProperty("priority")]
        public double? Priority { get; set; }
    }

    /// <summary>
    /// Audio provided to or from an LLM.
    /// </summary>
    public class AudioContent : ContentBlock
    {
        [JsonProperty("type")]
        public override string Type => "audio";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; } = null!;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = null!;
    }

    /// <summary>
    /// Request parameters for the authenticate method.
    ///
    /// Specifies which authentication method to use.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the authentication method to use.
        /// Must be one of the methods advertised in the initialize response.
        /// </summary>
        [JsonProperty("methodId")]
        public string MethodId { get; set; } = null!;
    }

    /// <summary>
    /// Response to the `authenticate` method.
    /// </summary>
    public class AuthenticateResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

    /// <summary>
    /// Describes an available authentication method.
    /// </summary>
    public class AuthMethod
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Optional description providing more details about this authentication method.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Unique identifier for this authentication method.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Human-readable name of the authentication method.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// Information about a command.
    /// </summary>
    public class AvailableCommand
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Human-readable description of what the command does.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Input for the command if required
        /// </summary>
        [JsonProperty("input")]
        public AvailableCommandInput Input { get; set; }

        /// <summary>
        /// Command name (e.g., `create_plan`, `research_codebase`).
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// The input specification for a command.
    /// </summary>
    public class AvailableCommandInput
    {
    }

    /// <summary>
    /// Available commands are ready or have changed
    /// </summary>
    public class AvailableCommandsUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "available_commands_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Commands the agent can execute
        /// </summary>
        [JsonProperty("availableCommands")]
        public AvailableCommand[] AvailableCommands { get; set; } = null!;
    }

    /// <summary>
    /// Binary resource contents.
    /// </summary>
    public class BlobResourceContents
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("blob")]
        public string Blob { get; set; } = null!;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Notification to cancel ongoing operations for a session.
    ///
    /// See protocol docs: [Cancellation](https://agentclientprotocol.com/protocol/prompt-turn#cancellation)
    /// </summary>
    public class CancelNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the session to cancel operations for.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Capabilities supported by the client.
    ///
    /// Advertised during initialization to inform the agent about
    /// available features and methods.
    ///
    /// See protocol docs: [Client Capabilities](https://agentclientprotocol.com/protocol/initialization#client-capabilities)
    /// </summary>
    public class ClientCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// File system capabilities supported by the client.
        /// Determines which file operations the agent can request.
        /// </summary>
        [JsonProperty("fs")]
        public FileSystemCapability Fs { get; set; }

        /// <summary>
        /// Whether the Client support all `terminal/*` methods.
        /// </summary>
        [JsonProperty("terminal")]
        public bool Terminal { get; set; } = false;
    }

    public class ClientNotification
    {
        [JsonProperty("method")]
        public string Method { get; set; } = null!;

        [JsonProperty("params")]
        public object Params { get; set; }
    }

    public class ClientRequest
    {
        [JsonProperty("id")]
        public RequestId Id { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; } = null!;

        [JsonProperty("params")]
        public object Params { get; set; }
    }

    public class ClientResponse
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("id")]
        public RequestId Id { get; set; }

        /// <summary>
        /// All possible responses that a client can send to an agent.
        ///
        /// This enum is used internally for routing RPC responses. You typically won't need
        /// to use this directly - the responses are handled automatically by the connection.
        ///
        /// These are responses to the corresponding `AgentRequest` variants.
        /// </summary>
        [JsonProperty("result")]
        public object Result { get; set; }
    }

    /// <summary>
    /// Session configuration options have been updated.
    /// </summary>
    public class ConfigOptionUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "config_option_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The full set of configuration options and their current values.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; } = null!;
    }

    /// <summary>
    /// Standard content block (text, images, resources).
    /// </summary>
    public class Content : ToolCallContent
    {
        [JsonProperty("type")]
        public override string Type => "content";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The actual content block.
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock ContentValue { get; set; } = null!;
    }

    /// <summary>
    /// Content blocks represent displayable information in the Agent Client Protocol.
    ///
    /// They provide a structured way to handle various types of user-facing content—whether
    /// it's text from language models, images for analysis, or embedded resources for context.
    ///
    /// Content blocks appear in:
    /// - User prompts sent via `session/prompt`
    /// - Language model output streamed through `session/update` notifications
    /// - Progress updates and results from tool calls
    ///
    /// This structure is compatible with the Model Context Protocol (MCP), enabling
    /// agents to seamlessly forward content from MCP tool outputs without transformation.
    ///
    /// See protocol docs: [Content](https://agentclientprotocol.com/protocol/content)
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<ContentBlock>))]
    public abstract class ContentBlock
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "audio", typeof(AudioContent) },
            { "image", typeof(ImageContent) },
            { "resource", typeof(EmbeddedResource) },
            { "resource_link", typeof(ResourceLink) },
            { "text", typeof(TextContent) }
        };

        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// A streamed item of content
    /// </summary>
    public class ContentChunk
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;
    }

    /// <summary>
    /// Request to create a new terminal and execute a command.
    /// </summary>
    public class CreateTerminalRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Array of command arguments.
        /// </summary>
        [JsonProperty("args")]
        public string[] Args { get; set; }

        /// <summary>
        /// The command to execute.
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; } = null!;

        /// <summary>
        /// Working directory for the command (absolute path).
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; }

        /// <summary>
        /// Environment variables for the command.
        /// </summary>
        [JsonProperty("env")]
        public EnvVariable[] Env { get; set; }

        /// <summary>
        /// Maximum number of output bytes to retain.
        ///
        /// When the limit is exceeded, the Client truncates from the beginning of the output
        /// to stay within the limit.
        ///
        /// The Client MUST ensure truncation happens at a character boundary to maintain valid
        /// string output, even if this means the retained output is slightly less than the
        /// specified limit.
        /// </summary>
        [JsonProperty("outputByteLimit")]
        public ulong? OutputByteLimit { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response containing the ID of the created terminal.
    /// </summary>
    public class CreateTerminalResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The unique identifier for the created terminal.
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// The current mode of the session has changed
    ///
    /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
    /// </summary>
    public class CurrentModeUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "current_mode_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the current mode
        /// </summary>
        [JsonProperty("currentModeId")]
        public SessionModeId CurrentModeId { get; set; }
    }

    /// <summary>
    /// A diff representing file modifications.
    ///
    /// Shows changes to files in a format suitable for display in the client UI.
    ///
    /// See protocol docs: [Content](https://agentclientprotocol.com/protocol/tool-calls#content)
    /// </summary>
    public class Diff : ToolCallContent
    {
        [JsonProperty("type")]
        public override string Type => "diff";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The new content after modification.
        /// </summary>
        [JsonProperty("newText")]
        public string NewText { get; set; } = null!;

        /// <summary>
        /// The original content (None for new files).
        /// </summary>
        [JsonProperty("oldText")]
        public string OldText { get; set; }

        /// <summary>
        /// The file path being modified.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; } = null!;
    }

    /// <summary>
    /// The contents of a resource, embedded into a prompt or tool call result.
    /// </summary>
    public class EmbeddedResource : ContentBlock
    {
        [JsonProperty("type")]
        public override string Type => "resource";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("resource")]
        public EmbeddedResourceResource Resource { get; set; } = null!;
    }

    /// <summary>
    /// Resource content that can be embedded in a message.
    /// </summary>
    public class EmbeddedResourceResource
    {
    }

    /// <summary>
    /// An environment variable to set when launching an MCP server.
    /// </summary>
    public class EnvVariable
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The name of the environment variable.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The value to set for the environment variable.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; } = null!;
    }

    /// <summary>
    /// JSON-RPC error object.
    ///
    /// Represents an error that occurred during method execution, following the
    /// JSON-RPC 2.0 error object specification with optional additional data.
    ///
    /// See protocol docs: [JSON-RPC Error Object](https://www.jsonrpc.org/specification#error_object)
    /// </summary>
    public class Error
    {
        /// <summary>
        /// A number indicating the error type that occurred.
        /// This must be an integer as defined in the JSON-RPC specification.
        /// </summary>
        [JsonProperty("code")]
        public ErrorCode Code { get; set; }

        /// <summary>
        /// Optional primitive or structured value that contains additional information about the error.
        /// This may include debugging information or context-specific details.
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// A string providing a short description of the error.
        /// The message should be limited to a concise single sentence.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = null!;
    }

    /// <summary>
    /// Allows the Agent to send an arbitrary notification that is not part of the ACP spec.
    /// Extension notifications provide a way to send one-way messages for custom functionality
    /// while maintaining protocol compatibility.
    ///
    /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
    /// </summary>
    public class ExtNotification
    {
    }

    /// <summary>
    /// Allows for sending an arbitrary request that is not part of the ACP spec.
    /// Extension methods provide a way to add custom functionality while maintaining
    /// protocol compatibility.
    ///
    /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
    /// </summary>
    public class ExtRequest
    {
    }

    /// <summary>
    /// Allows for sending an arbitrary response to an [`ExtRequest`] that is not part of the ACP spec.
    /// Extension methods provide a way to add custom functionality while maintaining
    /// protocol compatibility.
    ///
    /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
    /// </summary>
    public class ExtResponse
    {
    }

    /// <summary>
    /// Filesystem capabilities supported by the client.
    /// File system capabilities that a client may support.
    ///
    /// See protocol docs: [FileSystem](https://agentclientprotocol.com/protocol/initialization#filesystem)
    /// </summary>
    public class FileSystemCapability
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Whether the Client supports `fs/read_text_file` requests.
        /// </summary>
        [JsonProperty("readTextFile")]
        public bool ReadTextFile { get; set; } = false;

        /// <summary>
        /// Whether the Client supports `fs/write_text_file` requests.
        /// </summary>
        [JsonProperty("writeTextFile")]
        public bool WriteTextFile { get; set; } = false;
    }

    /// <summary>
    /// An HTTP header to set when making requests to the MCP server.
    /// </summary>
    public class HttpHeader
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The name of the HTTP header.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The value to set for the HTTP header.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; } = null!;
    }

    /// <summary>
    /// An image provided to or from an LLM.
    /// </summary>
    public class ImageContent : ContentBlock
    {
        [JsonProperty("type")]
        public override string Type => "image";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; } = null!;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = null!;

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }

    /// <summary>
    /// Metadata about the implementation of the client or agent.
    /// Describes the name and version of an MCP implementation, with an optional
    /// title for UI representation.
    /// </summary>
    public class Implementation
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Intended for programmatic or logical use, but can be used as a display
        /// name fallback if title isn’t present.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Intended for UI and end-user contexts — optimized to be human-readable
        /// and easily understood.
        ///
        /// If not provided, the name should be used for display.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Version of the implementation. Can be displayed to the user or used
        /// for debugging or metrics purposes. (e.g. "1.0.0").
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = null!;
    }

    /// <summary>
    /// Request parameters for the initialize method.
    ///
    /// Sent by the client to establish connection and negotiate capabilities.
    ///
    /// See protocol docs: [Initialization](https://agentclientprotocol.com/protocol/initialization)
    /// </summary>
    public class InitializeRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Capabilities supported by the client.
        /// </summary>
        [JsonProperty("clientCapabilities")]
        public ClientCapabilities ClientCapabilities { get; set; }

        /// <summary>
        /// Information about the Client name and version sent to the Agent.
        ///
        /// Note: in future versions of the protocol, this will be required.
        /// </summary>
        [JsonProperty("clientInfo")]
        public Implementation ClientInfo { get; set; }

        /// <summary>
        /// The latest protocol version supported by the client.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public ProtocolVersion ProtocolVersion { get; set; }
    }

    /// <summary>
    /// Response to the `initialize` method.
    ///
    /// Contains the negotiated protocol version and agent capabilities.
    ///
    /// See protocol docs: [Initialization](https://agentclientprotocol.com/protocol/initialization)
    /// </summary>
    public class InitializeResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Capabilities supported by the agent.
        /// </summary>
        [JsonProperty("agentCapabilities")]
        public AgentCapabilities AgentCapabilities { get; set; }

        /// <summary>
        /// Information about the Agent name and version sent to the Client.
        ///
        /// Note: in future versions of the protocol, this will be required.
        /// </summary>
        [JsonProperty("agentInfo")]
        public Implementation AgentInfo { get; set; }

        /// <summary>
        /// Authentication methods supported by the agent.
        /// </summary>
        [JsonProperty("authMethods")]
        public AuthMethod[] AuthMethods { get; set; } = new AuthMethod[0];

        /// <summary>
        /// The protocol version the client specified if supported by the agent,
        /// or the latest protocol version supported by the agent.
        ///
        /// The client should disconnect, if it doesn't support this version.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public ProtocolVersion ProtocolVersion { get; set; }
    }

    /// <summary>
    /// Request to kill a terminal command without releasing the terminal.
    /// </summary>
    public class KillTerminalCommandRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The ID of the terminal to kill.
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// Response to terminal/kill command method
    /// </summary>
    public class KillTerminalCommandResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

    /// <summary>
    /// Request parameters for loading an existing session.
    ///
    /// Only available if the Agent supports the `loadSession` capability.
    ///
    /// See protocol docs: [Loading Sessions](https://agentclientprotocol.com/protocol/session-setup#loading-sessions)
    /// </summary>
    public class LoadSessionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The working directory for this session.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; } = null!;

        /// <summary>
        /// List of MCP servers to connect to for this session.
        /// </summary>
        [JsonProperty("mcpServers")]
        public McpServer[] McpServers { get; set; } = null!;

        /// <summary>
        /// The ID of the session to load.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response from loading an existing session.
    /// </summary>
    public class LoadSessionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// Initial mode state if supported by the Agent
        ///
        /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
        /// </summary>
        [JsonProperty("modes")]
        public SessionModeState Modes { get; set; }
    }

    /// <summary>
    /// MCP capabilities supported by the agent
    /// </summary>
    public class McpCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Agent supports [`McpServer::Http`].
        /// </summary>
        [JsonProperty("http")]
        public bool Http { get; set; } = false;

        /// <summary>
        /// Agent supports [`McpServer::Sse`].
        /// </summary>
        [JsonProperty("sse")]
        public bool Sse { get; set; } = false;
    }

    /// <summary>
    /// Configuration for connecting to an MCP (Model Context Protocol) server.
    ///
    /// MCP servers provide tools and context that the agent can use when
    /// processing prompts.
    ///
    /// See protocol docs: [MCP Servers](https://agentclientprotocol.com/protocol/session-setup#mcp-servers)
    /// </summary>
    public class McpServer
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// HTTP transport configuration for MCP.
    /// </summary>
    public class McpServerHttp
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// HTTP headers to set when making requests to the MCP server.
        /// </summary>
        [JsonProperty("headers")]
        public HttpHeader[] Headers { get; set; } = null!;

        /// <summary>
        /// Human-readable name identifying this MCP server.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// URL to the MCP server.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = null!;
    }

    /// <summary>
    /// SSE transport configuration for MCP.
    /// </summary>
    public class McpServerSse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// HTTP headers to set when making requests to the MCP server.
        /// </summary>
        [JsonProperty("headers")]
        public HttpHeader[] Headers { get; set; } = null!;

        /// <summary>
        /// Human-readable name identifying this MCP server.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// URL to the MCP server.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; } = null!;
    }

    /// <summary>
    /// Stdio transport configuration for MCP.
    /// </summary>
    public class McpServerStdio
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Command-line arguments to pass to the MCP server.
        /// </summary>
        [JsonProperty("args")]
        public string[] Args { get; set; } = null!;

        /// <summary>
        /// Path to the MCP server executable.
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; } = null!;

        /// <summary>
        /// Environment variables to set when launching the MCP server.
        /// </summary>
        [JsonProperty("env")]
        public EnvVariable[] Env { get; set; } = null!;

        /// <summary>
        /// Human-readable name identifying this MCP server.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// Request parameters for creating a new session.
    ///
    /// See protocol docs: [Creating a Session](https://agentclientprotocol.com/protocol/session-setup#creating-a-session)
    /// </summary>
    public class NewSessionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The working directory for this session. Must be an absolute path.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; } = null!;

        /// <summary>
        /// List of MCP (Model Context Protocol) servers the agent should connect to.
        /// </summary>
        [JsonProperty("mcpServers")]
        public McpServer[] McpServers { get; set; } = null!;
    }

    /// <summary>
    /// Response from creating a new session.
    ///
    /// See protocol docs: [Creating a Session](https://agentclientprotocol.com/protocol/session-setup#creating-a-session)
    /// </summary>
    public class NewSessionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// Initial mode state if supported by the Agent
        ///
        /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
        /// </summary>
        [JsonProperty("modes")]
        public SessionModeState Modes { get; set; }

        /// <summary>
        /// Unique identifier for the created session.
        ///
        /// Used in all subsequent requests for this conversation.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// An option presented to the user when requesting permission.
    /// </summary>
    public class PermissionOption
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Hint about the nature of this permission option.
        /// </summary>
        [JsonProperty("kind")]
        public PermissionOptionKind Kind { get; set; }

        /// <summary>
        /// Human-readable label to display to the user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Unique identifier for this permission option.
        /// </summary>
        [JsonProperty("optionId")]
        public PermissionOptionId OptionId { get; set; }
    }

    /// <summary>
    /// An execution plan for accomplishing complex tasks.
    ///
    /// Plans consist of multiple entries representing individual tasks or goals.
    /// Agents report plans to clients to provide visibility into their execution strategy.
    /// Plans can evolve during execution as the agent discovers new requirements or completes tasks.
    ///
    /// See protocol docs: [Agent Plan](https://agentclientprotocol.com/protocol/agent-plan)
    /// </summary>
    public class Plan : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "plan";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The list of tasks to be accomplished.
        ///
        /// When updating a plan, the agent must send a complete list of all entries
        /// with their current status. The client replaces the entire plan with each update.
        /// </summary>
        [JsonProperty("entries")]
        public PlanEntry[] Entries { get; set; } = null!;
    }

    /// <summary>
    /// A single entry in the execution plan.
    ///
    /// Represents a task or goal that the assistant intends to accomplish
    /// as part of fulfilling the user's request.
    /// See protocol docs: [Plan Entries](https://agentclientprotocol.com/protocol/agent-plan#plan-entries)
    /// </summary>
    public class PlanEntry
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Human-readable description of what this task aims to accomplish.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = null!;

        /// <summary>
        /// The relative importance of this task.
        /// Used to indicate which tasks are most critical to the overall goal.
        /// </summary>
        [JsonProperty("priority")]
        public PlanEntryPriority Priority { get; set; }

        /// <summary>
        /// Current execution status of this task.
        /// </summary>
        [JsonProperty("status")]
        public PlanEntryStatus Status { get; set; }
    }

    /// <summary>
    /// Prompt capabilities supported by the agent in `session/prompt` requests.
    ///
    /// Baseline agent functionality requires support for [`ContentBlock::Text`]
    /// and [`ContentBlock::ResourceLink`] in prompt requests.
    ///
    /// Other variants must be explicitly opted in to.
    /// Capabilities for different types of content in prompt requests.
    ///
    /// Indicates which content types beyond the baseline (text and resource links)
    /// the agent can process.
    ///
    /// See protocol docs: [Prompt Capabilities](https://agentclientprotocol.com/protocol/initialization#prompt-capabilities)
    /// </summary>
    public class PromptCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Agent supports [`ContentBlock::Audio`].
        /// </summary>
        [JsonProperty("audio")]
        public bool Audio { get; set; } = false;

        /// <summary>
        /// Agent supports embedded context in `session/prompt` requests.
        ///
        /// When enabled, the Client is allowed to include [`ContentBlock::Resource`]
        /// in prompt requests for pieces of context that are referenced in the message.
        /// </summary>
        [JsonProperty("embeddedContext")]
        public bool EmbeddedContext { get; set; } = false;

        /// <summary>
        /// Agent supports [`ContentBlock::Image`].
        /// </summary>
        [JsonProperty("image")]
        public bool Image { get; set; } = false;
    }

    /// <summary>
    /// Request parameters for sending a user prompt to the agent.
    ///
    /// Contains the user's message and any additional context.
    ///
    /// See protocol docs: [User Message](https://agentclientprotocol.com/protocol/prompt-turn#1-user-message)
    /// </summary>
    public class PromptRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The blocks of content that compose the user's message.
        ///
        /// As a baseline, the Agent MUST support [`ContentBlock::Text`] and [`ContentBlock::ResourceLink`],
        /// while other variants are optionally enabled via [`PromptCapabilities`].
        ///
        /// The Client MUST adapt its interface according to [`PromptCapabilities`].
        ///
        /// The client MAY include referenced pieces of context as either
        /// [`ContentBlock::Resource`] or [`ContentBlock::ResourceLink`].
        ///
        /// When available, [`ContentBlock::Resource`] is preferred
        /// as it avoids extra round-trips and allows the message to include
        /// pieces of context from sources the agent may not have access to.
        /// </summary>
        [JsonProperty("prompt")]
        public ContentBlock[] Prompt { get; set; } = null!;

        /// <summary>
        /// The ID of the session to send this user message to
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response from processing a user prompt.
    ///
    /// See protocol docs: [Check for Completion](https://agentclientprotocol.com/protocol/prompt-turn#4-check-for-completion)
    /// </summary>
    public class PromptResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Indicates why the agent stopped processing the turn.
        /// </summary>
        [JsonProperty("stopReason")]
        public StopReason StopReason { get; set; }
    }

    /// <summary>
    /// Request to read content from a text file.
    ///
    /// Only available if the client supports the `fs.readTextFile` capability.
    /// </summary>
    public class ReadTextFileRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Maximum number of lines to read.
        /// </summary>
        [JsonProperty("limit")]
        public uint? Limit { get; set; }

        /// <summary>
        /// Line number to start reading from (1-based).
        /// </summary>
        [JsonProperty("line")]
        public uint? Line { get; set; }

        /// <summary>
        /// Absolute path to the file to read.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; } = null!;

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response containing the contents of a text file.
    /// </summary>
    public class ReadTextFileResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; } = null!;
    }

    /// <summary>
    /// Request to release a terminal and free its resources.
    /// </summary>
    public class ReleaseTerminalRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The ID of the terminal to release.
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// Response to terminal/release method
    /// </summary>
    public class ReleaseTerminalResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

    /// <summary>
    /// The outcome of a permission request.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<RequestPermissionOutcome>))]
    public abstract class RequestPermissionOutcome
    {
        internal const string DiscriminatorPropertyName = "outcome";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "cancelled", typeof(RequestPermissionOutcomeCancelled) },
            { "selected", typeof(SelectedPermissionOutcome) }
        };

        [JsonProperty("outcome")]
        public abstract string Outcome { get; }
    }

    /// <summary>
    /// The prompt turn was cancelled before the user responded.
    ///
    /// When a client sends a `session/cancel` notification to cancel an ongoing
    /// prompt turn, it MUST respond to all pending `session/request_permission`
    /// requests with this `Cancelled` outcome.
    ///
    /// See protocol docs: [Cancellation](https://agentclientprotocol.com/protocol/prompt-turn#cancellation)
    /// </summary>
    public class RequestPermissionOutcomeCancelled : RequestPermissionOutcome
    {
        [JsonProperty("outcome")]
        public override string Outcome => "cancelled";
    }

    /// <summary>
    /// Request for user permission to execute a tool call.
    ///
    /// Sent when the agent needs authorization before performing a sensitive operation.
    ///
    /// See protocol docs: [Requesting Permission](https://agentclientprotocol.com/protocol/tool-calls#requesting-permission)
    /// </summary>
    public class RequestPermissionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Available permission options for the user to choose from.
        /// </summary>
        [JsonProperty("options")]
        public PermissionOption[] Options { get; set; } = null!;

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// Details about the tool call requiring permission.
        /// </summary>
        [JsonProperty("toolCall")]
        public ToolCallUpdate ToolCall { get; set; } = null!;
    }

    /// <summary>
    /// Response to a permission request.
    /// </summary>
    public class RequestPermissionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The user's decision on the permission request.
        /// </summary>
        [JsonProperty("outcome")]
        public RequestPermissionOutcome Outcome { get; set; } = null!;
    }

    /// <summary>
    /// A resource that the server is capable of reading, included in a prompt or tool call result.
    /// </summary>
    public class ResourceLink : ContentBlock
    {
        [JsonProperty("type")]
        public override string Type => "resource_link";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("size")]
        public long? Size { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// The user selected one of the provided options.
    /// </summary>
    public class SelectedPermissionOutcome : RequestPermissionOutcome
    {
        [JsonProperty("outcome")]
        public override string Outcome => "selected";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the option the user selected.
        /// </summary>
        [JsonProperty("optionId")]
        public PermissionOptionId OptionId { get; set; }
    }

    /// <summary>
    /// Session capabilities supported by the agent.
    ///
    /// As a baseline, all Agents **MUST** support `session/new`, `session/prompt`, `session/cancel`, and `session/update`.
    ///
    /// Optionally, they **MAY** support other session methods and notifications by specifying additional capabilities.
    ///
    /// Note: `session/load` is still handled by the top-level `load_session` capability. This will be unified in future versions of the protocol.
    ///
    /// See protocol docs: [Session Capabilities](https://agentclientprotocol.com/protocol/initialization#session-capabilities)
    /// </summary>
    public class SessionCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

    /// <summary>
    /// A session configuration option selector and its current state.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<SessionConfigOption>))]
    public abstract class SessionConfigOption
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "select", typeof(SessionConfigSelect) }
        };

        [JsonProperty("type")]
        public abstract string Type { get; }

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Optional semantic category for this option (UX only).
        /// </summary>
        [JsonProperty("category")]
        public SessionConfigOptionCategory Category { get; set; }

        /// <summary>
        /// Optional description for the Client to display to the user.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Unique identifier for the configuration option.
        /// </summary>
        [JsonProperty("id")]
        public SessionConfigId Id { get; set; }

        /// <summary>
        /// Human-readable label for the option.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// A single-value selector (dropdown) session configuration option payload.
    /// </summary>
    public class SessionConfigSelect : SessionConfigOption
    {
        [JsonProperty("type")]
        public override string Type => "select";

        /// <summary>
        /// The currently selected value.
        /// </summary>
        [JsonProperty("currentValue")]
        public SessionConfigValueId CurrentValue { get; set; }

        /// <summary>
        /// The set of selectable options.
        /// </summary>
        [JsonProperty("options")]
        public SessionConfigSelectOptions Options { get; set; }
    }

    /// <summary>
    /// A group of possible values for a session configuration option.
    /// </summary>
    public class SessionConfigSelectGroup
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Unique identifier for this group.
        /// </summary>
        [JsonProperty("group")]
        public SessionConfigGroupId Group { get; set; }

        /// <summary>
        /// Human-readable label for this group.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The set of option values in this group.
        /// </summary>
        [JsonProperty("options")]
        public SessionConfigSelectOption[] Options { get; set; } = null!;
    }

    /// <summary>
    /// A possible value for a session configuration option.
    /// </summary>
    public class SessionConfigSelectOption
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Optional description for this option value.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Human-readable label for this option value.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Unique identifier for this option value.
        /// </summary>
        [JsonProperty("value")]
        public SessionConfigValueId Value { get; set; }
    }

    /// <summary>
    /// A mode the agent can operate in.
    ///
    /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
    /// </summary>
    public class SessionMode
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public SessionModeId Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// The set of modes and the one currently active.
    /// </summary>
    public class SessionModeState
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The set of modes that the Agent can operate in
        /// </summary>
        [JsonProperty("availableModes")]
        public SessionMode[] AvailableModes { get; set; } = null!;

        /// <summary>
        /// The current mode the Agent is in.
        /// </summary>
        [JsonProperty("currentModeId")]
        public SessionModeId CurrentModeId { get; set; }
    }

    /// <summary>
    /// Notification containing a session update from the agent.
    ///
    /// Used to stream real-time progress and results during prompt processing.
    ///
    /// See protocol docs: [Agent Reports Output](https://agentclientprotocol.com/protocol/prompt-turn#3-agent-reports-output)
    /// </summary>
    public class SessionNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the session this update pertains to.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The actual update content.
        /// </summary>
        [JsonProperty("update")]
        public SessionUpdate Update { get; set; } = null!;
    }

    /// <summary>
    /// Different types of updates that can be sent during session processing.
    ///
    /// These updates provide real-time feedback about the agent's progress.
    ///
    /// See protocol docs: [Agent Reports Output](https://agentclientprotocol.com/protocol/prompt-turn#3-agent-reports-output)
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<SessionUpdate>))]
    public abstract class SessionUpdate
    {
        internal const string DiscriminatorPropertyName = "sessionUpdate";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "agent_message_chunk", typeof(SessionUpdateAgentMessageChunk) },
            { "agent_thought_chunk", typeof(SessionUpdateAgentThoughtChunk) },
            { "available_commands_update", typeof(AvailableCommandsUpdate) },
            { "config_option_update", typeof(ConfigOptionUpdate) },
            { "current_mode_update", typeof(CurrentModeUpdate) },
            { "plan", typeof(Plan) },
            { "tool_call", typeof(ToolCall) },
            { "tool_call_update", typeof(SessionUpdateToolCallUpdate) },
            { "user_message_chunk", typeof(SessionUpdateUserMessageChunk) }
        };

        [JsonProperty("sessionUpdate")]
        public abstract string SessionUpdateValue { get; }
    }

    /// <summary>
    /// A chunk of the user's message being streamed.
    /// </summary>
    public class SessionUpdateUserMessageChunk : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "user_message_chunk";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;
    }

    /// <summary>
    /// A chunk of the agent's response being streamed.
    /// </summary>
    public class SessionUpdateAgentMessageChunk : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "agent_message_chunk";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;
    }

    /// <summary>
    /// A chunk of the agent's internal reasoning being streamed.
    /// </summary>
    public class SessionUpdateAgentThoughtChunk : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "agent_thought_chunk";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;
    }

    /// <summary>
    /// Update on the status or results of a tool call.
    /// </summary>
    public class SessionUpdateToolCallUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "tool_call_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Replace the content collection.
        /// </summary>
        [JsonProperty("content")]
        public ToolCallContent[] Content { get; set; }

        /// <summary>
        /// Update the tool kind.
        /// </summary>
        [JsonProperty("kind")]
        public ToolKind Kind { get; set; }

        /// <summary>
        /// Replace the locations collection.
        /// </summary>
        [JsonProperty("locations")]
        public ToolCallLocation[] Locations { get; set; }

        /// <summary>
        /// Update the raw input.
        /// </summary>
        [JsonProperty("rawInput")]
        public object RawInput { get; set; }

        /// <summary>
        /// Update the raw output.
        /// </summary>
        [JsonProperty("rawOutput")]
        public object RawOutput { get; set; }

        /// <summary>
        /// Update the execution status.
        /// </summary>
        [JsonProperty("status")]
        public ToolCallStatus Status { get; set; }

        /// <summary>
        /// Update the human-readable title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The ID of the tool call being updated.
        /// </summary>
        [JsonProperty("toolCallId")]
        public ToolCallId ToolCallId { get; set; }
    }

    /// <summary>
    /// Request parameters for setting a session configuration option.
    /// </summary>
    public class SetSessionConfigOptionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the configuration option to set.
        /// </summary>
        [JsonProperty("configId")]
        public SessionConfigId ConfigId { get; set; }

        /// <summary>
        /// The ID of the session to set the configuration option for.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The ID of the configuration option value to set.
        /// </summary>
        [JsonProperty("value")]
        public SessionConfigValueId Value { get; set; }
    }

    /// <summary>
    /// Response to `session/set_config_option` method.
    /// </summary>
    public class SetSessionConfigOptionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The full set of configuration options and their current values.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; } = null!;
    }

    /// <summary>
    /// Request parameters for setting a session mode.
    /// </summary>
    public class SetSessionModeRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The ID of the mode to set.
        /// </summary>
        [JsonProperty("modeId")]
        public SessionModeId ModeId { get; set; }

        /// <summary>
        /// The ID of the session to set the mode for.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response to `session/set_mode` method.
    /// </summary>
    public class SetSessionModeResponse
    {
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

    /// <summary>
    /// Embed a terminal created with `terminal/create` by its id.
    ///
    /// The terminal must be added before calling `terminal/release`.
    ///
    /// See protocol docs: [Terminal](https://agentclientprotocol.com/protocol/terminals)
    /// </summary>
    public class Terminal : ToolCallContent
    {
        [JsonProperty("type")]
        public override string Type => "terminal";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// Exit status of a terminal command.
    /// </summary>
    public class TerminalExitStatus
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The process exit code (may be null if terminated by signal).
        /// </summary>
        [JsonProperty("exitCode")]
        public uint? ExitCode { get; set; }

        /// <summary>
        /// The signal that terminated the process (may be null if exited normally).
        /// </summary>
        [JsonProperty("signal")]
        public string Signal { get; set; }
    }

    /// <summary>
    /// Request to get the current output and status of a terminal.
    /// </summary>
    public class TerminalOutputRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The ID of the terminal to get output from.
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// Response containing the terminal output and exit status.
    /// </summary>
    public class TerminalOutputResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Exit status if the command has completed.
        /// </summary>
        [JsonProperty("exitStatus")]
        public TerminalExitStatus ExitStatus { get; set; }

        /// <summary>
        /// The terminal output captured so far.
        /// </summary>
        [JsonProperty("output")]
        public string Output { get; set; } = null!;

        /// <summary>
        /// Whether the output was truncated due to byte limits.
        /// </summary>
        [JsonProperty("truncated")]
        public bool Truncated { get; set; }
    }

    /// <summary>
    /// Text provided to or from an LLM.
    /// </summary>
    public class TextContent : ContentBlock
    {
        [JsonProperty("type")]
        public override string Type => "text";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;
    }

    /// <summary>
    /// Text-based resource contents.
    /// </summary>
    public class TextResourceContents
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Represents a tool call that the language model has requested.
    ///
    /// Tool calls are actions that the agent executes on behalf of the language model,
    /// such as reading files, executing code, or fetching data from external sources.
    ///
    /// See protocol docs: [Tool Calls](https://agentclientprotocol.com/protocol/tool-calls)
    /// </summary>
    public class ToolCall : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "tool_call";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Content produced by the tool call.
        /// </summary>
        [JsonProperty("content")]
        public ToolCallContent[] Content { get; set; }

        /// <summary>
        /// The category of tool being invoked.
        /// Helps clients choose appropriate icons and UI treatment.
        /// </summary>
        [JsonProperty("kind")]
        public ToolKind Kind { get; set; }

        /// <summary>
        /// File locations affected by this tool call.
        /// Enables "follow-along" features in clients.
        /// </summary>
        [JsonProperty("locations")]
        public ToolCallLocation[] Locations { get; set; }

        /// <summary>
        /// Raw input parameters sent to the tool.
        /// </summary>
        [JsonProperty("rawInput")]
        public object RawInput { get; set; }

        /// <summary>
        /// Raw output returned by the tool.
        /// </summary>
        [JsonProperty("rawOutput")]
        public object RawOutput { get; set; }

        /// <summary>
        /// Current execution status of the tool call.
        /// </summary>
        [JsonProperty("status")]
        public ToolCallStatus Status { get; set; }

        /// <summary>
        /// Human-readable title describing what the tool is doing.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Unique identifier for this tool call within the session.
        /// </summary>
        [JsonProperty("toolCallId")]
        public ToolCallId ToolCallId { get; set; }
    }

    /// <summary>
    /// Content produced by a tool call.
    ///
    /// Tool calls can produce different types of content including
    /// standard content blocks (text, images) or file diffs.
    ///
    /// See protocol docs: [Content](https://agentclientprotocol.com/protocol/tool-calls#content)
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<ToolCallContent>))]
    public abstract class ToolCallContent
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "content", typeof(Content) },
            { "diff", typeof(Diff) },
            { "terminal", typeof(Terminal) }
        };

        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// A file location being accessed or modified by a tool.
    ///
    /// Enables clients to implement "follow-along" features that track
    /// which files the agent is working with in real-time.
    ///
    /// See protocol docs: [Following the Agent](https://agentclientprotocol.com/protocol/tool-calls#following-the-agent)
    /// </summary>
    public class ToolCallLocation
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Optional line number within the file.
        /// </summary>
        [JsonProperty("line")]
        public uint? Line { get; set; }

        /// <summary>
        /// The file path being accessed or modified.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; } = null!;
    }

    /// <summary>
    /// An update to an existing tool call.
    ///
    /// Used to report progress and results as tools execute. All fields except
    /// the tool call ID are optional - only changed fields need to be included.
    ///
    /// See protocol docs: [Updating](https://agentclientprotocol.com/protocol/tool-calls#updating)
    /// </summary>
    public class ToolCallUpdate
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Replace the content collection.
        /// </summary>
        [JsonProperty("content")]
        public ToolCallContent[] Content { get; set; }

        /// <summary>
        /// Update the tool kind.
        /// </summary>
        [JsonProperty("kind")]
        public ToolKind Kind { get; set; }

        /// <summary>
        /// Replace the locations collection.
        /// </summary>
        [JsonProperty("locations")]
        public ToolCallLocation[] Locations { get; set; }

        /// <summary>
        /// Update the raw input.
        /// </summary>
        [JsonProperty("rawInput")]
        public object RawInput { get; set; }

        /// <summary>
        /// Update the raw output.
        /// </summary>
        [JsonProperty("rawOutput")]
        public object RawOutput { get; set; }

        /// <summary>
        /// Update the execution status.
        /// </summary>
        [JsonProperty("status")]
        public ToolCallStatus Status { get; set; }

        /// <summary>
        /// Update the human-readable title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The ID of the tool call being updated.
        /// </summary>
        [JsonProperty("toolCallId")]
        public ToolCallId ToolCallId { get; set; }
    }

    /// <summary>
    /// All text that was typed after the command name is provided as input.
    /// </summary>
    public class UnstructuredCommandInput
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// A hint to display when the input hasn't been provided yet
        /// </summary>
        [JsonProperty("hint")]
        public string Hint { get; set; } = null!;
    }

    /// <summary>
    /// Request to wait for a terminal command to exit.
    /// </summary>
    public class WaitForTerminalExitRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The ID of the terminal to wait for.
        /// </summary>
        [JsonProperty("terminalId")]
        public string TerminalId { get; set; } = null!;
    }

    /// <summary>
    /// Response containing the exit status of a terminal command.
    /// </summary>
    public class WaitForTerminalExitResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The process exit code (may be null if terminated by signal).
        /// </summary>
        [JsonProperty("exitCode")]
        public uint? ExitCode { get; set; }

        /// <summary>
        /// The signal that terminated the process (may be null if exited normally).
        /// </summary>
        [JsonProperty("signal")]
        public string Signal { get; set; }
    }

    /// <summary>
    /// Request to write content to a text file.
    ///
    /// Only available if the client supports the `fs.writeTextFile` capability.
    /// </summary>
    public class WriteTextFileRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }

        /// <summary>
        /// The text content to write to the file.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = null!;

        /// <summary>
        /// Absolute path to the file to write.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; } = null!;

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response to `fs/write_text_file`
    /// </summary>
    public class WriteTextFileResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public object Meta { get; set; }
    }

}
