// Generated from schema/meta.json. Do not edit by hand.
// Schema ref: refs/tags/schema-v1.19.0

#pragma warning disable CS1591

namespace dotacp.protocol.unstable
{
    /// <summary>
    /// Protocol metadata
    /// </summary>
    public static class ProtocolMeta
    {
        /// <summary>
        /// ACP Protocol Version
        /// </summary>
        public const ushort Version = 1;
    }

    /// <summary>
    /// Methods that agents handle
    /// </summary>
    public static class AgentMethods
    {
        public const string Authenticate = "authenticate";
        public const string DocumentDidChange = "document/didChange";
        public const string DocumentDidClose = "document/didClose";
        public const string DocumentDidFocus = "document/didFocus";
        public const string DocumentDidOpen = "document/didOpen";
        public const string DocumentDidSave = "document/didSave";
        public const string Initialize = "initialize";
        public const string Logout = "logout";
        public const string McpMessage = "mcp/message";
        public const string NesAccept = "nes/accept";
        public const string NesClose = "nes/close";
        public const string NesReject = "nes/reject";
        public const string NesStart = "nes/start";
        public const string NesSuggest = "nes/suggest";
        public const string ProvidersDisable = "providers/disable";
        public const string ProvidersList = "providers/list";
        public const string ProvidersSet = "providers/set";
        public const string SessionCancel = "session/cancel";
        public const string SessionClose = "session/close";
        public const string SessionDelete = "session/delete";
        public const string SessionFork = "session/fork";
        public const string SessionList = "session/list";
        public const string SessionLoad = "session/load";
        public const string SessionNew = "session/new";
        public const string SessionPrompt = "session/prompt";
        public const string SessionResume = "session/resume";
        public const string SessionSetConfigOption = "session/set_config_option";
        public const string SessionSetMode = "session/set_mode";
    }

    /// <summary>
    /// Methods that clients handle
    /// </summary>
    public static class ClientMethods
    {
        public const string ElicitationComplete = "elicitation/complete";
        public const string ElicitationCreate = "elicitation/create";
        public const string FsReadTextFile = "fs/read_text_file";
        public const string FsWriteTextFile = "fs/write_text_file";
        public const string McpConnect = "mcp/connect";
        public const string McpDisconnect = "mcp/disconnect";
        public const string McpMessage = "mcp/message";
        public const string SessionRequestPermission = "session/request_permission";
        public const string SessionUpdate = "session/update";
        public const string TerminalCreate = "terminal/create";
        public const string TerminalKill = "terminal/kill";
        public const string TerminalOutput = "terminal/output";
        public const string TerminalRelease = "terminal/release";
        public const string TerminalWaitForExit = "terminal/wait_for_exit";
    }
}

#pragma warning restore CS1591
