// Generated from schema/schema.json. Do not edit by hand.
// Schema ref: refs/tags/v0.12.2

#pragma warning disable CS1591

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace dotacp.protocol.unstable
{
    // Type aliases

    [JsonConverter(typeof(UnionTypeConverter<ElicitationContentValue>))]
    public readonly struct ElicitationContentValue : IEquatable<ElicitationContentValue>
    {
        private readonly object _value;
        private readonly int _typeIndex;

        public ElicitationContentValue(string value)
        {
            _value = value;
            _typeIndex = 0;
        }

        public ElicitationContentValue(long value)
        {
            _value = value;
            _typeIndex = 1;
        }

        public ElicitationContentValue(double value)
        {
            _value = value;
            _typeIndex = 2;
        }

        public ElicitationContentValue(bool value)
        {
            _value = value;
            _typeIndex = 3;
        }

        public ElicitationContentValue(string[] value)
        {
            _value = value;
            _typeIndex = 4;
        }

        public static implicit operator ElicitationContentValue(string value) => new ElicitationContentValue(value);
        public static implicit operator ElicitationContentValue(long value) => new ElicitationContentValue(value);
        public static implicit operator ElicitationContentValue(double value) => new ElicitationContentValue(value);
        public static implicit operator ElicitationContentValue(bool value) => new ElicitationContentValue(value);
        public static implicit operator ElicitationContentValue(string[] value) => new ElicitationContentValue(value);

        public bool TryGetString(out string value)
        {
            if (_value is string v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetLong(out long value)
        {
            if (_value is long v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetDouble(out double value)
        {
            if (_value is double v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetBool(out bool value)
        {
            if (_value is bool v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetString(out string[] value)
        {
            if (_value is string[] v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool Equals(ElicitationContentValue other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;
        public override bool Equals(object obj) => obj is ElicitationContentValue other && Equals(other);
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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Unique identifier for an elicitation.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<ElicitationId, string>))]
    public readonly struct ElicitationId : IEquatable<ElicitationId>
    {
        private readonly string _value;

        public ElicitationId(string value)
        {
            _value = value;
        }

        public static implicit operator ElicitationId(string value) => new ElicitationId(value);
        public static implicit operator string(ElicitationId alias) => alias._value;

        public bool Equals(ElicitationId other) => _value == other._value;
        public override bool Equals(object obj) => obj is ElicitationId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// A unique identifier for a model.
    /// </summary>
    [JsonConverter(typeof(TypeAliasConverter<ModelId, string>))]
    public readonly struct ModelId : IEquatable<ModelId>
    {
        private readonly string _value;

        public ModelId(string value)
        {
            _value = value;
        }

        public static implicit operator ModelId(string value) => new ModelId(value);
        public static implicit operator string(ModelId alias) => alias._value;

        public bool Equals(ModelId other) => _value == other._value;
        public override bool Equals(object obj) => obj is ModelId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }

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

        public SessionConfigSelectOptions(SessionConfigSelectOption[] value)
        {
            _value = value;
            _typeIndex = 0;
        }

        public SessionConfigSelectOptions(SessionConfigSelectGroup[] value)
        {
            _value = value;
            _typeIndex = 1;
        }

        public static implicit operator SessionConfigSelectOptions(SessionConfigSelectOption[] value) => new SessionConfigSelectOptions(value);
        public static implicit operator SessionConfigSelectOptions(SessionConfigSelectGroup[] value) => new SessionConfigSelectOptions(value);

        public bool TryGetSessionConfigSelectOption(out SessionConfigSelectOption[] value)
        {
            if (_value is SessionConfigSelectOption[] v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetSessionConfigSelectGroup(out SessionConfigSelectGroup[] value)
        {
            if (_value is SessionConfigSelectGroup[] v)
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
    /// Type discriminator for elicitation schemas.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<ElicitationSchemaType>))]
    public enum ElicitationSchemaType
    {
        /// <summary>
        /// Object schema type.
        /// </summary>
        [JsonEnumValue("object")]
        Object
    }

    /// <summary>
    /// Items definition for untitled multi-select enum properties.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<ElicitationStringType>))]
    public enum ElicitationStringType
    {
        /// <summary>
        /// String schema type.
        /// </summary>
        [JsonEnumValue("string")]
        String
    }

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
        /// **Request cancelled**: **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Execution of the method was aborted either due to a cancellation request from the caller or
        /// because of resource constraints or shutdown.
        /// </summary>
        RequestCancelled = -32800,

        /// <summary>
        /// **Authentication required**: Authentication is required before this operation can be performed.
        /// </summary>
        AuthenticationRequired = -32000,

        /// <summary>
        /// **Resource not found**: A given resource, such as a file, was not found.
        /// </summary>
        ResourceNotFound = -32002,

        /// <summary>
        /// **URL elicitation required**: **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// The agent requires user input via a URL-based elicitation before it can proceed.
        /// </summary>
        UrlElicitationRequired = -32042
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Well-known API protocol identifiers for LLM providers.
    ///
    /// Agents and clients MUST handle unknown protocol identifiers gracefully.
    ///
    /// Protocol names beginning with `_` are free for custom use, like other ACP extension methods.
    /// Protocol names that do not begin with `_` are reserved for the ACP spec.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<LlmProtocol>))]
    public enum LlmProtocol
    {
        /// <summary>
        /// Anthropic API protocol.
        /// </summary>
        [JsonEnumValue("anthropic")]
        Anthropic,

        /// <summary>
        /// OpenAI API protocol.
        /// </summary>
        [JsonEnumValue("openai")]
        Openai,

        /// <summary>
        /// Azure OpenAI API protocol.
        /// </summary>
        [JsonEnumValue("azure")]
        Azure,

        /// <summary>
        /// Google Vertex AI API protocol.
        /// </summary>
        [JsonEnumValue("vertex")]
        Vertex,

        /// <summary>
        /// AWS Bedrock API protocol.
        /// </summary>
        [JsonEnumValue("bedrock")]
        Bedrock,

        /// <summary>
        /// Unknown or custom protocol.
        /// </summary>
        [JsonEnumValue("other")]
        Other
    }

    /// <summary>
    /// Severity of a diagnostic.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<NesDiagnosticSeverity>))]
    public enum NesDiagnosticSeverity
    {
        /// <summary>
        /// An error.
        /// </summary>
        [JsonEnumValue("error")]
        Error,

        /// <summary>
        /// A warning.
        /// </summary>
        [JsonEnumValue("warning")]
        Warning,

        /// <summary>
        /// An informational message.
        /// </summary>
        [JsonEnumValue("information")]
        Information,

        /// <summary>
        /// A hint.
        /// </summary>
        [JsonEnumValue("hint")]
        Hint
    }

    /// <summary>
    /// The reason a suggestion was rejected.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<NesRejectReason>))]
    public enum NesRejectReason
    {
        /// <summary>
        /// The user explicitly dismissed the suggestion.
        /// </summary>
        [JsonEnumValue("rejected")]
        Rejected,

        /// <summary>
        /// The suggestion was shown but the user continued editing without interacting.
        /// </summary>
        [JsonEnumValue("ignored")]
        Ignored,

        /// <summary>
        /// The suggestion was superseded by a newer suggestion.
        /// </summary>
        [JsonEnumValue("replaced")]
        Replaced,

        /// <summary>
        /// The request was cancelled before the agent returned a response.
        /// </summary>
        [JsonEnumValue("cancelled")]
        Cancelled
    }

    /// <summary>
    /// What triggered the suggestion request.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<NesTriggerKind>))]
    public enum NesTriggerKind
    {
        /// <summary>
        /// Triggered by user typing or cursor movement.
        /// </summary>
        [JsonEnumValue("automatic")]
        Automatic,

        /// <summary>
        /// Triggered by a diagnostic appearing at or near the cursor.
        /// </summary>
        [JsonEnumValue("diagnostic")]
        Diagnostic,

        /// <summary>
        /// Triggered by an explicit user action (keyboard shortcut).
        /// </summary>
        [JsonEnumValue("manual")]
        Manual
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
    /// The encoding used for character offsets in positions.
    ///
    /// Follows the same conventions as LSP 3.17. The default is UTF-16.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<PositionEncodingKind>))]
    public enum PositionEncodingKind
    {
        /// <summary>
        /// Character offsets count UTF-16 code units. This is the default.
        /// </summary>
        [JsonEnumValue("utf-16")]
        Utf16,

        /// <summary>
        /// Character offsets count Unicode code points.
        /// </summary>
        [JsonEnumValue("utf-32")]
        Utf32,

        /// <summary>
        /// Character offsets count UTF-8 code units (bytes).
        /// </summary>
        [JsonEnumValue("utf-8")]
        Utf8
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
    /// String format types for string properties in elicitation schemas.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<StringFormat>))]
    public enum StringFormat
    {
        /// <summary>
        /// Email address format.
        /// </summary>
        [JsonEnumValue("email")]
        Email,

        /// <summary>
        /// URI format.
        /// </summary>
        [JsonEnumValue("uri")]
        Uri,

        /// <summary>
        /// Date format (YYYY-MM-DD).
        /// </summary>
        [JsonEnumValue("date")]
        Date,

        /// <summary>
        /// Date-time format (ISO 8601).
        /// </summary>
        [JsonEnumValue("date-time")]
        Datetime
    }

    /// <summary>
    /// How the agent wants document changes delivered.
    /// </summary>
    [JsonConverter(typeof(JsonEnumMemberConverter<TextDocumentSyncKind>))]
    public enum TextDocumentSyncKind
    {
        /// <summary>
        /// Client sends the entire file content on each change.
        /// </summary>
        [JsonEnumValue("full")]
        Full,

        /// <summary>
        /// Client sends only the changed ranges.
        /// </summary>
        [JsonEnumValue("incremental")]
        Incremental
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
    /// Notification sent when a suggestion is accepted.
    /// </summary>
    public class AcceptNesNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the accepted suggestion.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Authentication-related capabilities supported by the agent.
    /// </summary>
    public class AgentAuthCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the agent supports the logout method.
        ///
        /// By supplying `{}` it means that the agent supports the logout method.
        /// </summary>
        [JsonProperty("logout")]
        public LogoutCapabilities Logout { get; set; }
    }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Authentication-related capabilities supported by the agent.
        /// </summary>
        [JsonProperty("auth")]
        public AgentAuthCapabilities Auth { get; set; }

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
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// NES (Next Edit Suggestions) capabilities supported by the agent.
        /// </summary>
        [JsonProperty("nes")]
        public NesCapabilities Nes { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// The position encoding selected by the agent from the client's supported encodings.
        /// </summary>
        [JsonProperty("positionEncoding")]
        public PositionEncodingKind PositionEncoding { get; set; }

        /// <summary>
        /// Prompt capabilities supported by the agent.
        /// </summary>
        [JsonProperty("promptCapabilities")]
        public PromptCapabilities PromptCapabilities { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Provider configuration capabilities supported by the agent.
        ///
        /// By supplying `{}` it means that the agent supports provider configuration methods.
        /// </summary>
        [JsonProperty("providers")]
        public ProvidersCapabilities Providers { get; set; }

        [JsonProperty("sessionCapabilities")]
        public SessionCapabilities SessionCapabilities { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; } = null!;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Authentication capabilities supported by the client.
    ///
    /// Advertised during initialization to inform the agent which authentication
    /// method types the client can handle. This governs opt-in types that require
    /// additional client-side support.
    /// </summary>
    public class AuthCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the client supports `terminal` authentication methods.
        ///
        /// When `true`, the agent may include `terminal` entries in its authentication methods.
        /// </summary>
        [JsonProperty("terminal")]
        public bool Terminal { get; set; } = false;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Describes a single environment variable for an [`AuthMethodEnvVar`] authentication method.
    /// </summary>
    public class AuthEnvVar
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Human-readable label for this variable, displayed in client UI.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// The environment variable name (e.g. `"OPENAI_API_KEY"`).
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Whether this variable is optional.
        ///
        /// Defaults to `false`.
        /// </summary>
        [JsonProperty("optional")]
        public bool Optional { get; set; } = false;

        /// <summary>
        /// Whether this value is a secret (e.g. API key, token).
        /// Clients should use a password-style input for secret vars.
        ///
        /// Defaults to `true`.
        /// </summary>
        [JsonProperty("secret")]
        public bool Secret { get; set; } = true;
    }

    /// <summary>
    /// Describes an available authentication method.
    ///
    /// The `type` field acts as the discriminator in the serialized JSON form.
    /// When no `type` is present, the method is treated as `agent`.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<AuthMethod>))]
    public abstract class AuthMethod
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "agent", typeof(AuthMethodAgent) },
            { "env_var", typeof(AuthMethodEnvVar) },
            { "terminal", typeof(AuthMethodTerminal) }
        };

        /// <summary>When the discriminator property is missing in JSON, deserialize as this type.</summary>
        internal static readonly Type DefaultTypeWhenDiscriminatorMissing = typeof(AuthMethodAgent);

        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// Agent handles authentication itself.
    ///
    /// This is the default authentication method type.
    /// </summary>
    public class AuthMethodAgent : AuthMethod
    {
        [JsonProperty("type")]
        public override string Type => "agent";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Environment variable authentication method.
    ///
    /// The user provides credentials that the client passes to the agent as environment variables.
    /// </summary>
    public class AuthMethodEnvVar : AuthMethod
    {
        [JsonProperty("type")]
        public override string Type => "env_var";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
        /// Optional link to a page where the user can obtain their credentials.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// Human-readable name of the authentication method.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The environment variables the client should set.
        /// </summary>
        [JsonProperty("vars")]
        public AuthEnvVar[] Vars { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Terminal-based authentication method.
    ///
    /// The client runs an interactive terminal for the user to authenticate via a TUI.
    /// </summary>
    public class AuthMethodTerminal : AuthMethod
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Additional arguments to pass when running the agent binary for terminal auth.
        /// </summary>
        [JsonProperty("args")]
        public string[] Args { get; set; }

        /// <summary>
        /// Optional description providing more details about this authentication method.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Additional environment variables to set when running the agent binary for terminal auth.
        /// </summary>
        [JsonProperty("env")]
        public object Env { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    public abstract class AvailableCommandInput
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Commands the agent can execute
        /// </summary>
        [JsonProperty("availableCommands")]
        public AvailableCommand[] AvailableCommands { get; set; } = null!;
    }

    /// <summary>
    /// Binary resource contents.
    /// </summary>
    public class BlobResourceContents : EmbeddedResourceResource
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "blob", "uri" };

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("blob")]
        public string Blob { get; set; } = null!;

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Schema for boolean properties in an elicitation form.
    /// </summary>
    public class BooleanPropertySchema : ElicitationPropertySchema
    {
        [JsonProperty("type")]
        public override string Type => "boolean";

        /// <summary>
        /// Default value.
        /// </summary>
        [JsonProperty("default")]
        public bool? Default { get; set; }

        /// <summary>
        /// Human-readable description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Optional title for the property.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the session to cancel operations for.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Notification to cancel an ongoing request.
    ///
    /// See protocol docs: [Cancellation](https://agentclientprotocol.com/protocol/cancellation)
    /// </summary>
    public class CancelRequestNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the request to cancel.
        /// </summary>
        [JsonProperty("requestId")]
        public RequestId RequestId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Authentication capabilities supported by the client.
        /// Determines which authentication method types the agent may include
        /// in its `InitializeResponse`.
        /// </summary>
        [JsonProperty("auth")]
        public AuthCapabilities Auth { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Elicitation capabilities supported by the client.
        /// Determines which elicitation modes the agent may use.
        /// </summary>
        [JsonProperty("elicitation")]
        public ElicitationCapabilities Elicitation { get; set; }

        /// <summary>
        /// File system capabilities supported by the client.
        /// Determines which file operations the agent can request.
        /// </summary>
        [JsonProperty("fs")]
        public FileSystemCapabilities Fs { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// NES (Next Edit Suggestions) capabilities supported by the client.
        /// </summary>
        [JsonProperty("nes")]
        public ClientNesCapabilities Nes { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// The position encodings supported by the client, in order of preference.
        /// </summary>
        [JsonProperty("positionEncodings")]
        public PositionEncodingKind[] PositionEncodings { get; set; }

        /// <summary>
        /// Whether the Client support all `terminal/*` methods.
        /// </summary>
        [JsonProperty("terminal")]
        public bool Terminal { get; set; } = false;
    }

    /// <summary>
    /// NES capabilities advertised by the client during initialization.
    /// </summary>
    public class ClientNesCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the client supports the `jump` suggestion kind.
        /// </summary>
        [JsonProperty("jump")]
        public NesJumpCapabilities Jump { get; set; }

        /// <summary>
        /// Whether the client supports the `rename` suggestion kind.
        /// </summary>
        [JsonProperty("rename")]
        public NesRenameCapabilities Rename { get; set; }

        /// <summary>
        /// Whether the client supports the `searchAndReplace` suggestion kind.
        /// </summary>
        [JsonProperty("searchAndReplace")]
        public NesSearchAndReplaceCapabilities SearchAndReplace { get; set; }
    }

    /// <summary>
    /// Request to close an NES session.
    ///
    /// The agent **must** cancel any ongoing work related to the NES session
    /// and then free up any resources associated with the session.
    /// </summary>
    public class CloseNesRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the NES session to close.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response from closing an NES session.
    /// </summary>
    public class CloseNesResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Request parameters for closing an active session.
    ///
    /// If supported, the agent **must** cancel any ongoing work related to the session
    /// (treat it as if `session/cancel` was called) and then free up any resources
    /// associated with the session.
    ///
    /// Only available if the Agent supports the `sessionCapabilities.close` capability.
    /// </summary>
    public class CloseSessionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the session to close.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response from closing a session.
    /// </summary>
    public class CloseSessionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Notification sent by the agent when a URL-based elicitation is complete.
    /// </summary>
    public class CompleteElicitationNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the elicitation that completed.
        /// </summary>
        [JsonProperty("elicitationId")]
        public ElicitationId ElicitationId { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// A unique identifier for the message this chunk belongs to.
        ///
        /// All chunks belonging to the same message share the same `messageId`.
        /// A change in `messageId` indicates a new message has started.
        /// Both clients and agents MUST use UUID format for message IDs.
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Cost information for a session.
    /// </summary>
    public class Cost
    {
        /// <summary>
        /// Total cumulative cost for session.
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }

        /// <summary>
        /// ISO 4217 currency code (e.g., "USD", "EUR").
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request from the agent to elicit structured user input.
    ///
    /// The agent sends this to the client to request information from the user,
    /// either via a form or by directing them to a URL.
    /// Elicitations are tied to a session (optionally a tool call) or a request.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<CreateElicitationRequest>))]
    public abstract class CreateElicitationRequest
    {
        internal const string DiscriminatorPropertyName = "mode";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "form", typeof(ElicitationFormMode) },
            { "url", typeof(ElicitationUrlMode) }
        };

        [JsonProperty("mode")]
        public abstract string Mode { get; }

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A human-readable message describing what input is needed.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response from the client to an elicitation request.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<CreateElicitationResponse>))]
    public abstract class CreateElicitationResponse
    {
        internal const string DiscriminatorPropertyName = "action";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "accept", typeof(ElicitationAcceptAction) },
            { "cancel", typeof(CreateElicitationResponseCancel) },
            { "decline", typeof(CreateElicitationResponseDecline) }
        };

        [JsonProperty("action")]
        public abstract string Action { get; }

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// The user declined the elicitation.
    /// </summary>
    public class CreateElicitationResponseDecline : CreateElicitationResponse
    {
        [JsonProperty("action")]
        public override string Action => "decline";
    }

    /// <summary>
    /// The elicitation was cancelled.
    /// </summary>
    public class CreateElicitationResponseCancel : CreateElicitationResponse
    {
        [JsonProperty("action")]
        public override string Action => "cancel";
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the current mode
        /// </summary>
        [JsonProperty("currentModeId")]
        public SessionModeId CurrentModeId { get; set; }
    }

    /// <summary>
    /// Notification sent when a file is edited.
    /// </summary>
    public class DidChangeDocumentNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The content changes.
        /// </summary>
        [JsonProperty("contentChanges")]
        public TextDocumentContentChangeEvent[] ContentChanges { get; set; } = null!;

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The URI of the changed document.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        /// <summary>
        /// The new version number of the document.
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }
    }

    /// <summary>
    /// Notification sent when a file is closed.
    /// </summary>
    public class DidCloseDocumentNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The URI of the closed document.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Notification sent when a file becomes the active editor tab.
    /// </summary>
    public class DidFocusDocumentNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; } = null!;

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The URI of the focused document.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        /// <summary>
        /// The version number of the document.
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }

        /// <summary>
        /// The portion of the file currently visible in the editor viewport.
        /// </summary>
        [JsonProperty("visibleRange")]
        public Range VisibleRange { get; set; } = null!;
    }

    /// <summary>
    /// Notification sent when a file is opened in the editor.
    /// </summary>
    public class DidOpenDocumentNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The language identifier of the document (e.g., "rust", "python").
        /// </summary>
        [JsonProperty("languageId")]
        public string LanguageId { get; set; } = null!;

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The full text content of the document.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        /// <summary>
        /// The URI of the opened document.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        /// <summary>
        /// The version number of the document.
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }
    }

    /// <summary>
    /// Notification sent when a file is saved.
    /// </summary>
    public class DidSaveDocumentNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// The URI of the saved document.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for `providers/disable`.
    /// </summary>
    public class DisableProvidersRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Provider id to disable.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response to `providers/disable`.
    /// </summary>
    public class DisableProvidersResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// The user accepted the elicitation and provided content.
    /// </summary>
    public class ElicitationAcceptAction : CreateElicitationResponse
    {
        [JsonProperty("action")]
        public override string Action => "accept";

        /// <summary>
        /// The user-provided content, if any, as an object matching the requested schema.
        /// </summary>
        [JsonProperty("content")]
        public object Content { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Elicitation capabilities supported by the client.
    /// </summary>
    public class ElicitationCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the client supports form-based elicitation.
        /// </summary>
        [JsonProperty("form")]
        public ElicitationFormCapabilities Form { get; set; }

        /// <summary>
        /// Whether the client supports URL-based elicitation.
        /// </summary>
        [JsonProperty("url")]
        public ElicitationUrlCapabilities Url { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Form-based elicitation capabilities.
    /// </summary>
    public class ElicitationFormCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Form-based elicitation mode where the client renders a form from the provided schema.
    /// </summary>
    [JsonConverter(typeof(ObjectUnionConverter<ElicitationFormMode>))]
    public abstract class ElicitationFormMode
    {
        /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>
        internal static readonly Type[] UnionVariantTypes = new Type[]
        {
            typeof(ElicitationSessionScope),
            typeof(ElicitationRequestScope),
        };
    }


    /// <summary>
    /// Property schema for elicitation form fields.
    ///
    /// Each variant corresponds to a JSON Schema `"type"` value.
    /// Single-select enums use the `String` variant with `enum` or `oneOf` set.
    /// Multi-select enums use the `Array` variant.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<ElicitationPropertySchema>))]
    public abstract class ElicitationPropertySchema
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "array", typeof(MultiSelectPropertySchema) },
            { "boolean", typeof(BooleanPropertySchema) },
            { "integer", typeof(IntegerPropertySchema) },
            { "number", typeof(NumberPropertySchema) },
            { "string", typeof(StringPropertySchema) }
        };

        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request-scoped elicitation, tied to a specific JSON-RPC request outside of a session
    /// (e.g., during auth/configuration phases before any session is started).
    /// </summary>
    public class ElicitationRequestScope : ElicitationUrlMode
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "requestId" };

        /// <summary>
        /// The request this elicitation is tied to.
        /// </summary>
        [JsonProperty("requestId")]
        public RequestId RequestId { get; set; }
    }

    /// <summary>
    /// Type-safe elicitation schema for requesting structured user input.
    ///
    /// This represents a JSON Schema object with primitive-typed properties,
    /// as required by the elicitation specification.
    /// </summary>
    public class ElicitationSchema
    {
        /// <summary>
        /// Optional description of what this schema represents.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Property definitions (must be primitive types).
        /// </summary>
        [JsonProperty("properties")]
        public object Properties { get; set; }

        /// <summary>
        /// List of required property names.
        /// </summary>
        [JsonProperty("required")]
        public string[] Required { get; set; }

        /// <summary>
        /// Optional title for the schema.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Type discriminator. Always `"object"`.
        /// </summary>
        [JsonProperty("type")]
        public ElicitationSchemaType Type { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Session-scoped elicitation, optionally tied to a specific tool call.
    ///
    /// When `tool_call_id` is set, the elicitation is tied to a specific tool call.
    /// This is useful when an agent receives an elicitation from an MCP server
    /// during a tool call and needs to redirect it to the user.
    /// </summary>
    public class ElicitationSessionScope : ElicitationUrlMode
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "sessionId" };

        /// <summary>
        /// The session this elicitation is tied to.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// Optional tool call within the session.
        /// </summary>
        [JsonProperty("toolCallId")]
        public ToolCallId ToolCallId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// URL-based elicitation capabilities.
    /// </summary>
    public class ElicitationUrlCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// URL-based elicitation mode where the client directs the user to a URL.
    /// </summary>
    [JsonConverter(typeof(ObjectUnionConverter<ElicitationUrlMode>))]
    public abstract class ElicitationUrlMode
    {
        /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>
        internal static readonly Type[] UnionVariantTypes = new Type[]
        {
            typeof(ElicitationSessionScope),
            typeof(ElicitationRequestScope),
        };
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
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("resource")]
        public EmbeddedResourceResource Resource { get; set; } = null!;
    }

    /// <summary>
    /// Resource content that can be embedded in a message.
    /// </summary>
    [JsonConverter(typeof(ObjectUnionConverter<EmbeddedResourceResource>))]
    public abstract class EmbeddedResourceResource
    {
        /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>
        internal static readonly Type[] UnionVariantTypes = new Type[]
        {
            typeof(TextResourceContents),
            typeof(BlobResourceContents),
        };
    }


    /// <summary>
    /// A titled enum option with a const value and human-readable title.
    /// </summary>
    public class EnumOption
    {
        /// <summary>
        /// The constant value for this option.
        /// </summary>
        [JsonProperty("const")]
        public string Const { get; set; } = null!;

        /// <summary>
        /// Human-readable title for this option.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
    /// File system capabilities that a client may support.
    ///
    /// See protocol docs: [FileSystem](https://agentclientprotocol.com/protocol/initialization#filesystem)
    /// </summary>
    public class FileSystemCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for forking an existing session.
    ///
    /// Creates a new session based on the context of an existing one, allowing
    /// operations like generating summaries without affecting the original session's history.
    ///
    /// Only available if the Agent supports the `session.fork` capability.
    /// </summary>
    public class ForkSessionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Additional workspace roots to activate for this session. Each path must be absolute.
        ///
        /// When omitted or empty, no additional roots are activated. When non-empty,
        /// this is the complete resulting additional-root list for the forked
        /// session.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

        /// <summary>
        /// The working directory for this session.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; } = null!;

        /// <summary>
        /// List of MCP servers to connect to for this session.
        /// </summary>
        [JsonProperty("mcpServers")]
        public McpServer[] McpServers { get; set; }

        /// <summary>
        /// The ID of the session to fork.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response from forking an existing session.
    /// </summary>
    public class ForkSessionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Initial model state if supported by the Agent
        /// </summary>
        [JsonProperty("models")]
        public SessionModelState Models { get; set; }

        /// <summary>
        /// Initial mode state if supported by the Agent
        ///
        /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
        /// </summary>
        [JsonProperty("modes")]
        public SessionModeState Modes { get; set; }

        /// <summary>
        /// Unique identifier for the newly created forked session.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// Schema for integer properties in an elicitation form.
    /// </summary>
    public class IntegerPropertySchema : ElicitationPropertySchema
    {
        [JsonProperty("type")]
        public override string Type => "integer";

        /// <summary>
        /// Default value.
        /// </summary>
        [JsonProperty("default")]
        public long? Default { get; set; }

        /// <summary>
        /// Human-readable description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Maximum value (inclusive).
        /// </summary>
        [JsonProperty("maximum")]
        public long? Maximum { get; set; }

        /// <summary>
        /// Minimum value (inclusive).
        /// </summary>
        [JsonProperty("minimum")]
        public long? Minimum { get; set; }

        /// <summary>
        /// Optional title for the property.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    /// <summary>
    /// Request to kill a terminal without releasing it.
    /// </summary>
    public class KillTerminalRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    /// Response to `terminal/kill` method
    /// </summary>
    public class KillTerminalResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for `providers/list`.
    /// </summary>
    public class ListProvidersRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response to `providers/list`.
    /// </summary>
    public class ListProvidersResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Configurable providers with current routing info suitable for UI display.
        /// </summary>
        [JsonProperty("providers")]
        public ProviderInfo[] Providers { get; set; } = null!;
    }

    /// <summary>
    /// Request parameters for listing existing sessions.
    ///
    /// Only available if the Agent supports the `sessionCapabilities.list` capability.
    /// </summary>
    public class ListSessionsRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Filter sessions by the exact ordered additional workspace roots. Each path must be absolute.
        ///
        /// This filter applies only when the field is present and non-empty. When
        /// omitted or empty, no additional-root filter is applied.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

        /// <summary>
        /// Opaque cursor token from a previous response's nextCursor field for cursor-based pagination
        /// </summary>
        [JsonProperty("cursor")]
        public string Cursor { get; set; }

        /// <summary>
        /// Filter sessions by working directory. Must be an absolute path.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; }
    }

    /// <summary>
    /// Response from listing sessions.
    /// </summary>
    public class ListSessionsResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Opaque cursor token. If present, pass this in the next request's cursor parameter
        /// to fetch the next page. If absent, there are no more results.
        /// </summary>
        [JsonProperty("nextCursor")]
        public string NextCursor { get; set; }

        /// <summary>
        /// Array of session information objects
        /// </summary>
        [JsonProperty("sessions")]
        public SessionInfo[] Sessions { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Additional workspace roots to activate for this session. Each path must be absolute.
        ///
        /// When omitted or empty, no additional roots are activated. When non-empty,
        /// this is the complete resulting additional-root list for the loaded
        /// session.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Initial model state if supported by the Agent
        /// </summary>
        [JsonProperty("models")]
        public SessionModelState Models { get; set; }

        /// <summary>
        /// Initial mode state if supported by the Agent
        ///
        /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
        /// </summary>
        [JsonProperty("modes")]
        public SessionModeState Modes { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Logout capabilities supported by the agent.
    ///
    /// By supplying `{}` it means that the agent supports the logout method.
    /// </summary>
    public class LogoutCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for the logout method.
    ///
    /// Terminates the current authenticated session.
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response to the `logout` method.
    /// </summary>
    public class LogoutResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
    [JsonConverter(typeof(DiscriminatorConverter<McpServer>))]
    public abstract class McpServer
    {
        internal const string DiscriminatorPropertyName = "type";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "http", typeof(McpServerHttp) },
            { "sse", typeof(McpServerSse) },
            { "stdio", typeof(McpServerStdio) }
        };

        /// <summary>When the discriminator property is missing in JSON, deserialize as this type.</summary>
        internal static readonly Type DefaultTypeWhenDiscriminatorMissing = typeof(McpServerStdio);

        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    /// <summary>
    /// HTTP transport configuration for MCP.
    /// </summary>
    public class McpServerHttp : McpServer
    {
        [JsonProperty("type")]
        public override string Type => "http";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    public class McpServerSse : McpServer
    {
        [JsonProperty("type")]
        public override string Type => "sse";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    public class McpServerStdio : McpServer
    {
        [JsonProperty("type")]
        public override string Type => "stdio";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Information about a selectable model.
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Optional description of the model.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Unique identifier for the model.
        /// </summary>
        [JsonProperty("modelId")]
        public ModelId ModelId { get; set; } = null!;

        /// <summary>
        /// Human-readable name of the model.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// Items for a multi-select (array) property schema.
    /// </summary>
    [JsonConverter(typeof(ObjectUnionConverter<MultiSelectItems>))]
    public abstract class MultiSelectItems
    {
        /// <summary>Variant types for union deserialization (no discriminator in JSON).</summary>
        internal static readonly Type[] UnionVariantTypes = new Type[]
        {
            typeof(UntitledMultiSelectItems),
            typeof(TitledMultiSelectItems),
        };
    }


    /// <summary>
    /// Schema for multi-select (array) properties in an elicitation form.
    /// </summary>
    public class MultiSelectPropertySchema : ElicitationPropertySchema
    {
        [JsonProperty("type")]
        public override string Type => "array";

        /// <summary>
        /// Default selected values.
        /// </summary>
        [JsonProperty("default")]
        public string[] Default { get; set; }

        /// <summary>
        /// Human-readable description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The items definition describing allowed values.
        /// </summary>
        [JsonProperty("items")]
        public MultiSelectItems Items { get; set; } = null!;

        /// <summary>
        /// Maximum number of items to select.
        /// </summary>
        [JsonProperty("maxItems")]
        public ulong? MaxItems { get; set; }

        /// <summary>
        /// Minimum number of items to select.
        /// </summary>
        [JsonProperty("minItems")]
        public ulong? MinItems { get; set; }

        /// <summary>
        /// Optional title for the property.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    /// <summary>
    /// NES capabilities advertised by the agent during initialization.
    /// </summary>
    public class NesCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Context the agent wants attached to each suggestion request.
        /// </summary>
        [JsonProperty("context")]
        public NesContextCapabilities Context { get; set; }

        /// <summary>
        /// Events the agent wants to receive.
        /// </summary>
        [JsonProperty("events")]
        public NesEventCapabilities Events { get; set; }
    }

    /// <summary>
    /// Context capabilities the agent wants attached to each suggestion request.
    /// </summary>
    public class NesContextCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the agent wants diagnostics context.
        /// </summary>
        [JsonProperty("diagnostics")]
        public NesDiagnosticsCapabilities Diagnostics { get; set; }

        /// <summary>
        /// Whether the agent wants edit history context.
        /// </summary>
        [JsonProperty("editHistory")]
        public NesEditHistoryCapabilities EditHistory { get; set; }

        /// <summary>
        /// Whether the agent wants open files context.
        /// </summary>
        [JsonProperty("openFiles")]
        public NesOpenFilesCapabilities OpenFiles { get; set; }

        /// <summary>
        /// Whether the agent wants recent files context.
        /// </summary>
        [JsonProperty("recentFiles")]
        public NesRecentFilesCapabilities RecentFiles { get; set; }

        /// <summary>
        /// Whether the agent wants related snippets context.
        /// </summary>
        [JsonProperty("relatedSnippets")]
        public NesRelatedSnippetsCapabilities RelatedSnippets { get; set; }

        /// <summary>
        /// Whether the agent wants user actions context.
        /// </summary>
        [JsonProperty("userActions")]
        public NesUserActionsCapabilities UserActions { get; set; }
    }

    /// <summary>
    /// A diagnostic (error, warning, etc.).
    /// </summary>
    public class NesDiagnostic
    {
        /// <summary>
        /// The diagnostic message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = null!;

        /// <summary>
        /// The range of the diagnostic.
        /// </summary>
        [JsonProperty("range")]
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The severity of the diagnostic.
        /// </summary>
        [JsonProperty("severity")]
        public NesDiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// The URI of the file containing the diagnostic.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Capabilities for diagnostics context.
    /// </summary>
    public class NesDiagnosticsCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Capabilities for `document/didChange` events.
    /// </summary>
    public class NesDocumentDidChangeCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The sync kind the agent wants: `"full"` or `"incremental"`.
        /// </summary>
        [JsonProperty("syncKind")]
        public TextDocumentSyncKind SyncKind { get; set; }
    }

    /// <summary>
    /// Marker for `document/didClose` capability support.
    /// </summary>
    public class NesDocumentDidCloseCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Marker for `document/didFocus` capability support.
    /// </summary>
    public class NesDocumentDidFocusCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Marker for `document/didOpen` capability support.
    /// </summary>
    public class NesDocumentDidOpenCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Marker for `document/didSave` capability support.
    /// </summary>
    public class NesDocumentDidSaveCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Document event capabilities the agent wants to receive.
    /// </summary>
    public class NesDocumentEventCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Whether the agent wants `document/didChange` events, and the sync kind.
        /// </summary>
        [JsonProperty("didChange")]
        public NesDocumentDidChangeCapabilities DidChange { get; set; }

        /// <summary>
        /// Whether the agent wants `document/didClose` events.
        /// </summary>
        [JsonProperty("didClose")]
        public NesDocumentDidCloseCapabilities DidClose { get; set; }

        /// <summary>
        /// Whether the agent wants `document/didFocus` events.
        /// </summary>
        [JsonProperty("didFocus")]
        public NesDocumentDidFocusCapabilities DidFocus { get; set; }

        /// <summary>
        /// Whether the agent wants `document/didOpen` events.
        /// </summary>
        [JsonProperty("didOpen")]
        public NesDocumentDidOpenCapabilities DidOpen { get; set; }

        /// <summary>
        /// Whether the agent wants `document/didSave` events.
        /// </summary>
        [JsonProperty("didSave")]
        public NesDocumentDidSaveCapabilities DidSave { get; set; }
    }

    /// <summary>
    /// Capabilities for edit history context.
    /// </summary>
    public class NesEditHistoryCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Maximum number of edit history entries the agent can use.
        /// </summary>
        [JsonProperty("maxCount")]
        public uint? MaxCount { get; set; }
    }

    /// <summary>
    /// An entry in the edit history.
    /// </summary>
    public class NesEditHistoryEntry
    {
        /// <summary>
        /// A diff representing the edit.
        /// </summary>
        [JsonProperty("diff")]
        public string Diff { get; set; } = null!;

        /// <summary>
        /// The URI of the edited file.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// A text edit suggestion.
    /// </summary>
    public class NesEditSuggestion : NesSuggestion
    {
        [JsonProperty("kind")]
        public override string Kind => "edit";

        /// <summary>
        /// Optional suggested cursor position after applying edits.
        /// </summary>
        [JsonProperty("cursorPosition")]
        public Position CursorPosition { get; set; }

        /// <summary>
        /// The text edits to apply.
        /// </summary>
        [JsonProperty("edits")]
        public NesTextEdit[] Edits { get; set; } = null!;

        /// <summary>
        /// Unique identifier for accept/reject tracking.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The URI of the file to edit.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Event capabilities the agent can consume.
    /// </summary>
    public class NesEventCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Document event capabilities.
        /// </summary>
        [JsonProperty("document")]
        public NesDocumentEventCapabilities Document { get; set; }
    }

    /// <summary>
    /// A code excerpt from a file.
    /// </summary>
    public class NesExcerpt
    {
        /// <summary>
        /// The end line of the excerpt (zero-based).
        /// </summary>
        [JsonProperty("endLine")]
        public uint EndLine { get; set; }

        /// <summary>
        /// The start line of the excerpt (zero-based).
        /// </summary>
        [JsonProperty("startLine")]
        public uint StartLine { get; set; }

        /// <summary>
        /// The text content of the excerpt.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = null!;
    }

    /// <summary>
    /// Marker for jump suggestion support.
    /// </summary>
    public class NesJumpCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// A jump-to-location suggestion.
    /// </summary>
    public class NesJumpSuggestion : NesSuggestion
    {
        [JsonProperty("kind")]
        public override string Kind => "jump";

        /// <summary>
        /// Unique identifier for accept/reject tracking.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The target position within the file.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; } = null!;

        /// <summary>
        /// The file to navigate to.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// An open file in the editor.
    /// </summary>
    public class NesOpenFile
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        [JsonProperty("languageId")]
        public string LanguageId { get; set; } = null!;

        /// <summary>
        /// Timestamp in milliseconds since epoch of when the file was last focused.
        /// </summary>
        [JsonProperty("lastFocusedMs")]
        public ulong? LastFocusedMs { get; set; }

        /// <summary>
        /// The URI of the file.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        /// <summary>
        /// The visible range in the editor, if any.
        /// </summary>
        [JsonProperty("visibleRange")]
        public Range VisibleRange { get; set; }
    }

    /// <summary>
    /// Capabilities for open files context.
    /// </summary>
    public class NesOpenFilesCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// A recently accessed file.
    /// </summary>
    public class NesRecentFile
    {
        /// <summary>
        /// The language identifier.
        /// </summary>
        [JsonProperty("languageId")]
        public string LanguageId { get; set; } = null!;

        /// <summary>
        /// The full text content of the file.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        /// <summary>
        /// The URI of the file.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Capabilities for recent files context.
    /// </summary>
    public class NesRecentFilesCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Maximum number of recent files the agent can use.
        /// </summary>
        [JsonProperty("maxCount")]
        public uint? MaxCount { get; set; }
    }

    /// <summary>
    /// A related code snippet from a file.
    /// </summary>
    public class NesRelatedSnippet
    {
        /// <summary>
        /// The code excerpts.
        /// </summary>
        [JsonProperty("excerpts")]
        public NesExcerpt[] Excerpts { get; set; } = null!;

        /// <summary>
        /// The URI of the file containing the snippets.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Capabilities for related snippets context.
    /// </summary>
    public class NesRelatedSnippetsCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Marker for rename suggestion support.
    /// </summary>
    public class NesRenameCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// A rename symbol suggestion.
    /// </summary>
    public class NesRenameSuggestion : NesSuggestion
    {
        [JsonProperty("kind")]
        public override string Kind => "rename";

        /// <summary>
        /// Unique identifier for accept/reject tracking.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The new name for the symbol.
        /// </summary>
        [JsonProperty("newName")]
        public string NewName { get; set; } = null!;

        /// <summary>
        /// The position of the symbol to rename.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; } = null!;

        /// <summary>
        /// The file URI containing the symbol.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Repository metadata for an NES session.
    /// </summary>
    public class NesRepository
    {
        /// <summary>
        /// The repository name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The repository owner.
        /// </summary>
        [JsonProperty("owner")]
        public string Owner { get; set; } = null!;

        /// <summary>
        /// The remote URL of the repository.
        /// </summary>
        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; } = null!;
    }

    /// <summary>
    /// Marker for search and replace suggestion support.
    /// </summary>
    public class NesSearchAndReplaceCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// A search-and-replace suggestion.
    /// </summary>
    public class NesSearchAndReplaceSuggestion : NesSuggestion
    {
        [JsonProperty("kind")]
        public override string Kind => "searchAndReplace";

        /// <summary>
        /// Unique identifier for accept/reject tracking.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Whether `search` is a regular expression. Defaults to `false`.
        /// </summary>
        [JsonProperty("isRegex")]
        public bool? IsRegex { get; set; }

        /// <summary>
        /// The replacement text.
        /// </summary>
        [JsonProperty("replace")]
        public string Replace { get; set; } = null!;

        /// <summary>
        /// The text or pattern to find.
        /// </summary>
        [JsonProperty("search")]
        public string Search { get; set; } = null!;

        /// <summary>
        /// The file URI to search within.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Context attached to a suggestion request.
    /// </summary>
    public class NesSuggestContext
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Current diagnostics (errors, warnings).
        /// </summary>
        [JsonProperty("diagnostics")]
        public NesDiagnostic[] Diagnostics { get; set; }

        /// <summary>
        /// Recent edit history.
        /// </summary>
        [JsonProperty("editHistory")]
        public NesEditHistoryEntry[] EditHistory { get; set; }

        /// <summary>
        /// Currently open files in the editor.
        /// </summary>
        [JsonProperty("openFiles")]
        public NesOpenFile[] OpenFiles { get; set; }

        /// <summary>
        /// Recently accessed files.
        /// </summary>
        [JsonProperty("recentFiles")]
        public NesRecentFile[] RecentFiles { get; set; }

        /// <summary>
        /// Related code snippets.
        /// </summary>
        [JsonProperty("relatedSnippets")]
        public NesRelatedSnippet[] RelatedSnippets { get; set; }

        /// <summary>
        /// Recent user actions (typing, navigation, etc.).
        /// </summary>
        [JsonProperty("userActions")]
        public NesUserAction[] UserActions { get; set; }
    }

    /// <summary>
    /// A suggestion returned by the agent.
    /// </summary>
    [JsonConverter(typeof(DiscriminatorConverter<NesSuggestion>))]
    public abstract class NesSuggestion
    {
        internal const string DiscriminatorPropertyName = "kind";
        internal static readonly Dictionary<string, Type> DiscriminatorMapping = new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            { "edit", typeof(NesEditSuggestion) },
            { "jump", typeof(NesJumpSuggestion) },
            { "rename", typeof(NesRenameSuggestion) },
            { "searchAndReplace", typeof(NesSearchAndReplaceSuggestion) }
        };

        [JsonProperty("kind")]
        public abstract string Kind { get; }
    }

    /// <summary>
    /// A text edit within a suggestion.
    /// </summary>
    public class NesTextEdit
    {
        /// <summary>
        /// The replacement text.
        /// </summary>
        [JsonProperty("newText")]
        public string NewText { get; set; } = null!;

        /// <summary>
        /// The range to replace.
        /// </summary>
        [JsonProperty("range")]
        public Range Range { get; set; } = null!;
    }

    /// <summary>
    /// A user action (typing, cursor movement, etc.).
    /// </summary>
    public class NesUserAction
    {
        /// <summary>
        /// The kind of action (e.g., "insertChar", "cursorMovement").
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; } = null!;

        /// <summary>
        /// The position where the action occurred.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; } = null!;

        /// <summary>
        /// Timestamp in milliseconds since epoch.
        /// </summary>
        [JsonProperty("timestampMs")]
        public ulong TimestampMs { get; set; }

        /// <summary>
        /// The URI of the file where the action occurred.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Capabilities for user actions context.
    /// </summary>
    public class NesUserActionsCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Maximum number of user actions the agent can use.
        /// </summary>
        [JsonProperty("maxCount")]
        public uint? MaxCount { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Additional workspace roots for this session. Each path must be absolute.
        ///
        /// These expand the session's filesystem scope without changing `cwd`, which
        /// remains the base for relative paths. When omitted or empty, no
        /// additional roots are activated for the new session.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Initial model state if supported by the Agent
        /// </summary>
        [JsonProperty("models")]
        public SessionModelState Models { get; set; }

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
    /// Schema for number (floating-point) properties in an elicitation form.
    /// </summary>
    public class NumberPropertySchema : ElicitationPropertySchema
    {
        [JsonProperty("type")]
        public override string Type => "number";

        /// <summary>
        /// Default value.
        /// </summary>
        [JsonProperty("default")]
        public double? Default { get; set; }

        /// <summary>
        /// Human-readable description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Maximum value (inclusive).
        /// </summary>
        [JsonProperty("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// Minimum value (inclusive).
        /// </summary>
        [JsonProperty("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Optional title for the property.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// A zero-based position in a text document.
    ///
    /// The meaning of `character` depends on the negotiated position encoding.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Zero-based character offset (encoding-dependent).
        /// </summary>
        [JsonProperty("character")]
        public uint Character { get; set; }

        /// <summary>
        /// Zero-based line number.
        /// </summary>
        [JsonProperty("line")]
        public uint Line { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// A client-generated unique identifier for this user message.
        ///
        /// If provided, the Agent SHOULD echo this value as `userMessageId` in the
        /// [`PromptResponse`] to confirm it was recorded.
        /// Both clients and agents MUST use UUID format for message IDs.
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Indicates why the agent stopped processing the turn.
        /// </summary>
        [JsonProperty("stopReason")]
        public StopReason StopReason { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Token usage for this turn (optional).
        /// </summary>
        [JsonProperty("usage")]
        public Usage Usage { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// The acknowledged user message ID.
        ///
        /// If the client provided a `messageId` in the [`PromptRequest`], the agent echoes it here
        /// to confirm it was recorded. If the client did not provide one, the agent MAY assign one
        /// and return it here. Absence of this field indicates the agent did not record a message ID.
        /// </summary>
        [JsonProperty("userMessageId")]
        public string UserMessageId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Current effective non-secret routing configuration for a provider.
    /// </summary>
    public class ProviderCurrentConfig
    {
        /// <summary>
        /// Protocol currently used by this provider.
        /// </summary>
        [JsonProperty("apiType")]
        public LlmProtocol ApiType { get; set; }

        /// <summary>
        /// Base URL currently used by this provider.
        /// </summary>
        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Information about a configurable LLM provider.
    /// </summary>
    public class ProviderInfo
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Current effective non-secret routing config.
        /// Null or omitted means provider is disabled.
        /// </summary>
        [JsonProperty("current")]
        public ProviderCurrentConfig Current { get; set; }

        /// <summary>
        /// Provider identifier, for example "main" or "openai".
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Whether this provider is mandatory and cannot be disabled via `providers/disable`.
        /// If true, clients must not call `providers/disable` for this id.
        /// </summary>
        [JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Supported protocol types for this provider.
        /// </summary>
        [JsonProperty("supported")]
        public LlmProtocol[] Supported { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Provider configuration capabilities supported by the agent.
    ///
    /// By supplying `{}` it means that the agent supports provider configuration methods.
    /// </summary>
    public class ProvidersCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// A range in a text document, expressed as start and end positions.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// The end position (exclusive).
        /// </summary>
        [JsonProperty("end")]
        public Position End { get; set; } = null!;

        /// <summary>
        /// The start position (inclusive).
        /// </summary>
        [JsonProperty("start")]
        public Position Start { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; } = null!;
    }

    /// <summary>
    /// Notification sent when a suggestion is rejected.
    /// </summary>
    public class RejectNesNotification
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the rejected suggestion.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// The reason for rejection.
        /// </summary>
        [JsonProperty("reason")]
        public NesRejectReason Reason { get; set; }

        /// <summary>
        /// The session ID for this notification.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// Request parameters for resuming an existing session.
    ///
    /// Resumes an existing session without returning previous messages (unlike `session/load`).
    /// This is useful for agents that can resume sessions but don't implement full session loading.
    ///
    /// Only available if the Agent supports the `sessionCapabilities.resume` capability.
    /// </summary>
    public class ResumeSessionRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Additional workspace roots to activate for this session. Each path must be absolute.
        ///
        /// When omitted or empty, no additional roots are activated. When non-empty,
        /// this is the complete resulting additional-root list for the resumed
        /// session.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

        /// <summary>
        /// The working directory for this session.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; } = null!;

        /// <summary>
        /// List of MCP servers to connect to for this session.
        /// </summary>
        [JsonProperty("mcpServers")]
        public McpServer[] McpServers { get; set; }

        /// <summary>
        /// The ID of the session to resume.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Response from resuming an existing session.
    /// </summary>
    public class ResumeSessionResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Initial session configuration options if supported by the Agent.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Initial model state if supported by the Agent
        /// </summary>
        [JsonProperty("models")]
        public SessionModelState Models { get; set; }

        /// <summary>
        /// Initial mode state if supported by the Agent
        ///
        /// See protocol docs: [Session Modes](https://agentclientprotocol.com/protocol/session-modes)
        /// </summary>
        [JsonProperty("modes")]
        public SessionModeState Modes { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the option the user selected.
        /// </summary>
        [JsonProperty("optionId")]
        public PermissionOptionId OptionId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Capabilities for additional session directories support.
    ///
    /// By supplying `{}` it means that the agent supports the `additionalDirectories` field on
    /// supported session lifecycle requests and `session/list`.
    /// </summary>
    public class SessionAdditionalDirectoriesCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Whether the agent supports `additionalDirectories` on supported session lifecycle requests and `session/list`.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public SessionAdditionalDirectoriesCapabilities AdditionalDirectories { get; set; }

        /// <summary>
        /// Whether the agent supports `session/close`.
        /// </summary>
        [JsonProperty("close")]
        public SessionCloseCapabilities Close { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Whether the agent supports `session/fork`.
        /// </summary>
        [JsonProperty("fork")]
        public SessionForkCapabilities Fork { get; set; }

        /// <summary>
        /// Whether the agent supports `session/list`.
        /// </summary>
        [JsonProperty("list")]
        public SessionListCapabilities List { get; set; }

        /// <summary>
        /// Whether the agent supports `session/resume`.
        /// </summary>
        [JsonProperty("resume")]
        public SessionResumeCapabilities Resume { get; set; }
    }

    /// <summary>
    /// Capabilities for the `session/close` method.
    ///
    /// By supplying `{}` it means that the agent supports closing of sessions.
    /// </summary>
    public class SessionCloseCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// A boolean on/off toggle session configuration option payload.
    /// </summary>
    public class SessionConfigBoolean : SessionConfigOption
    {
        [JsonProperty("type")]
        public override string Type => "boolean";

        /// <summary>
        /// The current value of the boolean option.
        /// </summary>
        [JsonProperty("currentValue")]
        public bool CurrentValue { get; set; }
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
            { "boolean", typeof(SessionConfigBoolean) },
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Capabilities for the `session/fork` method.
    ///
    /// By supplying `{}` it means that the agent supports forking of sessions.
    /// </summary>
    public class SessionForkCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Information about a session returned by session/list
    /// </summary>
    public class SessionInfo
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// Authoritative ordered additional workspace roots for this session. Each path must be absolute.
        ///
        /// When omitted or empty, there are no additional roots for the session.
        /// </summary>
        [JsonProperty("additionalDirectories")]
        public string[] AdditionalDirectories { get; set; }

        /// <summary>
        /// The working directory for this session. Must be an absolute path.
        /// </summary>
        [JsonProperty("cwd")]
        public string Cwd { get; set; } = null!;

        /// <summary>
        /// Unique identifier for the session
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// Human-readable title for the session
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// ISO 8601 timestamp of last activity
        /// </summary>
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }

    /// <summary>
    /// Update to session metadata. All fields are optional to support partial updates.
    ///
    /// Agents send this notification to update session information like title or custom metadata.
    /// This allows clients to display dynamic session names and track session state changes.
    /// </summary>
    public class SessionInfoUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "session_info_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Human-readable title for the session. Set to null to clear.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// ISO 8601 timestamp of last activity. Set to null to clear.
        /// </summary>
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }

    /// <summary>
    /// Capabilities for the `session/list` method.
    ///
    /// By supplying `{}` it means that the agent supports listing of sessions.
    /// </summary>
    public class SessionListCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public SessionModeId Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// The set of models and the one currently active.
    /// </summary>
    public class SessionModelState
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The set of models that the Agent can use
        /// </summary>
        [JsonProperty("availableModels")]
        public ModelInfo[] AvailableModels { get; set; } = null!;

        /// <summary>
        /// The current model the Agent is in.
        /// </summary>
        [JsonProperty("currentModelId")]
        public ModelId CurrentModelId { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// Capabilities for the `session/resume` method.
    ///
    /// By supplying `{}` it means that the agent supports resuming of sessions.
    /// </summary>
    public class SessionResumeCapabilities
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
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
            { "session_info_update", typeof(SessionInfoUpdate) },
            { "tool_call", typeof(ToolCall) },
            { "tool_call_update", typeof(SessionUpdateToolCallUpdate) },
            { "usage_update", typeof(UsageUpdate) },
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// A unique identifier for the message this chunk belongs to.
        ///
        /// All chunks belonging to the same message share the same `messageId`.
        /// A change in `messageId` indicates a new message has started.
        /// Both clients and agents MUST use UUID format for message IDs.
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// A unique identifier for the message this chunk belongs to.
        ///
        /// All chunks belonging to the same message share the same `messageId`.
        /// A change in `messageId` indicates a new message has started.
        /// Both clients and agents MUST use UUID format for message IDs.
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A single item of content
        /// </summary>
        [JsonProperty("content")]
        public ContentBlock Content { get; set; } = null!;

        /// <summary>
        /// **UNSTABLE**
        ///
        /// This capability is not part of the spec yet, and may be removed or changed at any point.
        ///
        /// A unique identifier for the message this chunk belongs to.
        ///
        /// All chunks belonging to the same message share the same `messageId`.
        /// A change in `messageId` indicates a new message has started.
        /// Both clients and agents MUST use UUID format for message IDs.
        /// </summary>
        [JsonProperty("messageId")]
        public string MessageId { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for `providers/set`.
    ///
    /// Replaces the full configuration for one provider id.
    /// </summary>
    public class SetProvidersRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Protocol type for this provider.
        /// </summary>
        [JsonProperty("apiType")]
        public LlmProtocol ApiType { get; set; }

        /// <summary>
        /// Base URL for requests sent through this provider.
        /// </summary>
        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Full headers map for this provider.
        /// May include authorization, routing, or other integration-specific headers.
        /// </summary>
        [JsonProperty("headers")]
        public object Headers { get; set; }

        /// <summary>
        /// Provider id to configure.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response to `providers/set`.
    /// </summary>
    public class SetProvidersResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Request parameters for setting a session configuration option.
    /// </summary>
    [JsonConverter(typeof(UnionTypeConverter<SetSessionConfigOptionRequestValue>))]
    public readonly struct SetSessionConfigOptionRequestValue : IEquatable<SetSessionConfigOptionRequestValue>
    {
        private readonly object _value;
        private readonly int _typeIndex;

        public SetSessionConfigOptionRequestValue(bool value)
        {
            _value = value;
            _typeIndex = 0;
        }

        public SetSessionConfigOptionRequestValue(SessionConfigValueId value)
        {
            _value = value;
            _typeIndex = 1;
        }

        public static implicit operator SetSessionConfigOptionRequestValue(bool value) => new SetSessionConfigOptionRequestValue(value);
        public static implicit operator SetSessionConfigOptionRequestValue(SessionConfigValueId value) => new SetSessionConfigOptionRequestValue(value);

        public bool TryGetBool(out bool value)
        {
            if (_value is bool v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetSessionConfigValueId(out SessionConfigValueId value)
        {
            if (_value is SessionConfigValueId v)
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        public bool Equals(SetSessionConfigOptionRequestValue other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;
        public override bool Equals(object obj) => obj is SetSessionConfigOptionRequestValue other && Equals(other);
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
        public Dictionary<string, object> Meta { get; set; }

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

        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The boolean value.
        /// </summary>
        [JsonProperty("value")]
        public SetSessionConfigOptionRequestValue Value { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The full set of configuration options and their current values.
        /// </summary>
        [JsonProperty("configOptions")]
        public SessionConfigOption[] ConfigOptions { get; set; } = null!;
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Request parameters for setting a session model.
    /// </summary>
    public class SetSessionModelRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The ID of the model to set.
        /// </summary>
        [JsonProperty("modelId")]
        public ModelId ModelId { get; set; } = null!;

        /// <summary>
        /// The ID of the session to set the model for.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Response to `session/set_model` method.
    /// </summary>
    public class SetSessionModelResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }
    }

    /// <summary>
    /// Request to start an NES session.
    /// </summary>
    public class StartNesRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Repository metadata, if the workspace is a git repository.
        /// </summary>
        [JsonProperty("repository")]
        public NesRepository Repository { get; set; }

        /// <summary>
        /// The workspace folders.
        /// </summary>
        [JsonProperty("workspaceFolders")]
        public WorkspaceFolder[] WorkspaceFolders { get; set; }

        /// <summary>
        /// The root URI of the workspace.
        /// </summary>
        [JsonProperty("workspaceUri")]
        public string WorkspaceUri { get; set; }
    }

    /// <summary>
    /// Response to `nes/start`.
    /// </summary>
    public class StartNesResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The session ID for the newly started NES session.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }
    }

    /// <summary>
    /// Schema for string properties in an elicitation form.
    ///
    /// When `enum` or `oneOf` is set, this represents a single-select enum
    /// with `"type": "string"`.
    /// </summary>
    public class StringPropertySchema : ElicitationPropertySchema
    {
        [JsonProperty("type")]
        public override string Type => "string";

        /// <summary>
        /// Default value.
        /// </summary>
        [JsonProperty("default")]
        public string Default { get; set; }

        /// <summary>
        /// Human-readable description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Enum values for untitled single-select enums.
        /// </summary>
        [JsonProperty("enum")]
        public string[] Enum { get; set; }

        /// <summary>
        /// String format.
        /// </summary>
        [JsonProperty("format")]
        public StringFormat Format { get; set; }

        /// <summary>
        /// Maximum string length.
        /// </summary>
        [JsonProperty("maxLength")]
        public uint? MaxLength { get; set; }

        /// <summary>
        /// Minimum string length.
        /// </summary>
        [JsonProperty("minLength")]
        public uint? MinLength { get; set; }

        /// <summary>
        /// Titled enum options for titled single-select enums.
        /// </summary>
        [JsonProperty("oneOf")]
        public EnumOption[] OneOf { get; set; }

        /// <summary>
        /// Pattern the string must match.
        /// </summary>
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        /// <summary>
        /// Optional title for the property.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    /// <summary>
    /// Request for a code suggestion.
    /// </summary>
    public class SuggestNesRequest
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Context for the suggestion, included based on agent capabilities.
        /// </summary>
        [JsonProperty("context")]
        public NesSuggestContext Context { get; set; }

        /// <summary>
        /// The current cursor position.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; } = null!;

        /// <summary>
        /// The current text selection range, if any.
        /// </summary>
        [JsonProperty("selection")]
        public Range Selection { get; set; }

        /// <summary>
        /// The session ID for this request.
        /// </summary>
        [JsonProperty("sessionId")]
        public SessionId SessionId { get; set; }

        /// <summary>
        /// What triggered this suggestion request.
        /// </summary>
        [JsonProperty("triggerKind")]
        public NesTriggerKind TriggerKind { get; set; }

        /// <summary>
        /// The URI of the document to suggest for.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        /// <summary>
        /// The version number of the document.
        /// </summary>
        [JsonProperty("version")]
        public long Version { get; set; }
    }

    /// <summary>
    /// Response to `nes/suggest`.
    /// </summary>
    public class SuggestNesResponse
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// The list of suggestions.
        /// </summary>
        [JsonProperty("suggestions")]
        public NesSuggestion[] Suggestions { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;
    }

    /// <summary>
    /// A content change event for a document.
    ///
    /// When `range` is `None`, `text` is the full content of the document.
    /// When `range` is `Some`, `text` replaces the given range.
    /// </summary>
    public class TextDocumentContentChangeEvent
    {
        /// <summary>
        /// The range of the document that changed. If `None`, the entire content is replaced.
        /// </summary>
        [JsonProperty("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The new text for the range, or the full document content if `range` is `None`.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = null!;
    }

    /// <summary>
    /// Text-based resource contents.
    /// </summary>
    public class TextResourceContents : EmbeddedResourceResource
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "text", "uri" };

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
    }

    /// <summary>
    /// Items definition for titled multi-select enum properties.
    /// </summary>
    public class TitledMultiSelectItems : MultiSelectItems
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "anyOf" };

        /// <summary>
        /// Titled enum options.
        /// </summary>
        [JsonProperty("anyOf")]
        public EnumOption[] AnyOf { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    public class UnstructuredCommandInput : AvailableCommandInput
    {
        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// A hint to display when the input hasn't been provided yet
        /// </summary>
        [JsonProperty("hint")]
        public string Hint { get; set; } = null!;
    }

    /// <summary>
    /// Items definition for untitled multi-select enum properties.
    /// </summary>
    public class UntitledMultiSelectItems : MultiSelectItems
    {
        /// <summary>Required JSON keys for union variant matching (no discriminator).</summary>

        internal static readonly string[] UnionVariantRequiredJsonKeys = new string[] { "type", "enum" };

        /// <summary>
        /// Allowed enum values.
        /// </summary>
        [JsonProperty("enum")]
        public string[] Enum { get; set; } = null!;

        /// <summary>
        /// Item type discriminator. Must be `"string"`.
        /// </summary>
        [JsonProperty("type")]
        public ElicitationStringType Type { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Token usage information for a prompt turn.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Total cache read tokens.
        /// </summary>
        [JsonProperty("cachedReadTokens")]
        public ulong? CachedReadTokens { get; set; }

        /// <summary>
        /// Total cache write tokens.
        /// </summary>
        [JsonProperty("cachedWriteTokens")]
        public ulong? CachedWriteTokens { get; set; }

        /// <summary>
        /// Total input tokens across all turns.
        /// </summary>
        [JsonProperty("inputTokens")]
        public ulong InputTokens { get; set; }

        /// <summary>
        /// Total output tokens across all turns.
        /// </summary>
        [JsonProperty("outputTokens")]
        public ulong OutputTokens { get; set; }

        /// <summary>
        /// Total thought/reasoning tokens
        /// </summary>
        [JsonProperty("thoughtTokens")]
        public ulong? ThoughtTokens { get; set; }

        /// <summary>
        /// Sum of all token types across session.
        /// </summary>
        [JsonProperty("totalTokens")]
        public ulong TotalTokens { get; set; }
    }

    /// <summary>
    /// **UNSTABLE**
    ///
    /// This capability is not part of the spec yet, and may be removed or changed at any point.
    ///
    /// Context window and cost update for a session.
    /// </summary>
    public class UsageUpdate : SessionUpdate
    {
        [JsonProperty("sessionUpdate")]
        public override string SessionUpdateValue => "usage_update";

        /// <summary>
        /// The _meta property is reserved by ACP to allow clients and agents to attach additional
        /// metadata to their interactions. Implementations MUST NOT make assumptions about values at
        /// these keys.
        ///
        /// See protocol docs: [Extensibility](https://agentclientprotocol.com/protocol/extensibility)
        /// </summary>
        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Cumulative session cost (optional).
        /// </summary>
        [JsonProperty("cost")]
        public Cost Cost { get; set; }

        /// <summary>
        /// Total context window size in tokens.
        /// </summary>
        [JsonProperty("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Tokens currently in context.
        /// </summary>
        [JsonProperty("used")]
        public ulong Used { get; set; }
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }

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
    /// A workspace folder.
    /// </summary>
    public class WorkspaceFolder
    {
        /// <summary>
        /// The display name of the folder.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The URI of the folder.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;
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
        public Dictionary<string, object> Meta { get; set; }

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
        public Dictionary<string, object> Meta { get; set; }
    }

}

#pragma warning restore CS1591
