// Generated from schema/meta.json. Do not edit by hand.
// Schema ref: refs/tags/v0.10.8

namespace dotacp.protocol
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
        public const string Initialize = "initialize";
        public const string SessionCancel = "session/cancel";
        public const string SessionLoad = "session/load";
        public const string SessionNew = "session/new";
        public const string SessionPrompt = "session/prompt";
        public const string SessionSetConfigOption = "session/set_config_option";
        public const string SessionSetMode = "session/set_mode";
    }

    /// <summary>
    /// Methods that clients handle
    /// </summary>
    public static class ClientMethods
    {
        public const string FsReadTextFile = "fs/read_text_file";
        public const string FsWriteTextFile = "fs/write_text_file";
        public const string SessionRequestPermission = "session/request_permission";
        public const string SessionUpdate = "session/update";
        public const string TerminalCreate = "terminal/create";
        public const string TerminalKill = "terminal/kill";
        public const string TerminalOutput = "terminal/output";
        public const string TerminalRelease = "terminal/release";
        public const string TerminalWaitForExit = "terminal/wait_for_exit";
    }
}

