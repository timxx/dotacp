using dotacp.client.unstable;
using dotacp.protocol.unstable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace clientcli
{
    internal enum MessageType
    {
        Unknown = 0,
        AvaiableCommands,
        AgentMessage,
        AgentThought,
        ConfigOption,
        CurrentMode,
        Plan,
        ToolCall,
        ToolCallUpdate,
        UserMessage,
        Usage,
        SessionInfo,
    }

    internal class Client : IAcpClient
    {
        private MessageType _lastMessage = MessageType.Unknown;

        public async Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request, CancellationToken cancellationToken = default)
        {
            await Console.Out.WriteLineAsync("Permission requested:");
            if (request.ToolCall != null)
                await Console.Out.WriteLineAsync($"  Tool call: {request.ToolCall.ToolCallId} `{request.ToolCall.Title}` {request.ToolCall.Kind} {request.ToolCall.Status}");

            foreach (var opt in request.Options)
            {
                await Console.Out.WriteLineAsync($"  Option: {opt.Name} ({opt.OptionId})");
            }

            return new RequestPermissionResponse
            {
                Outcome = new RequestPermissionOutcomeCancelled()
            };
        }

        public async Task SessionUpdateAsync(SessionNotification notification, CancellationToken cancellationToken = default)
        {
            var oldColor = Console.ForegroundColor;
            Console.ResetColor();

            var update = notification.Update;
            if (update is AvailableCommandsUpdate commands)
            {
                _lastMessage = MessageType.AvaiableCommands;
                Console.ForegroundColor = ConsoleColor.Cyan;
                await Console.Out.WriteLineAsync($"Available commands:");
                foreach (var command in commands.AvailableCommands)
                {
                    await Console.Out.WriteAsync($"  /{command.Name}");
                    if (command.Input is UnstructuredCommandInput input)
                        await Console.Out.WriteAsync($" <{input.Hint}>");
                    await Console.Out.WriteLineAsync($" - {command.Description}");
                }
                Console.ResetColor();
            }
            else if (update is SessionUpdateAgentMessageChunk agentMessage)
            {
                await EnsureNewLineAsync(MessageType.AgentMessage, "\nAgent message:");

                Console.ForegroundColor = ConsoleColor.Magenta;
                await Console.Out.WriteAsync(ContentBlockText(agentMessage.Content));
                Console.ResetColor();

                _lastMessage = MessageType.AgentMessage;
            }
            else if (update is SessionUpdateAgentThoughtChunk agentThought)
            {
                await EnsureNewLineAsync(MessageType.AgentThought, "\nAgent thought:");

                Console.ForegroundColor = ConsoleColor.Yellow;
                await Console.Out.WriteAsync(ContentBlockText(agentThought.Content));
                Console.ResetColor();

                _lastMessage = MessageType.AgentThought;
            }
            else if (update is ConfigOptionUpdate configOption)
            {
                _lastMessage = MessageType.ConfigOption;
                await Console.Out.WriteLineAsync($"Config option update: {configOption.ConfigOptions.Length}");
            }
            else if (update is CurrentModeUpdate currentMode)
            {
                _lastMessage = MessageType.CurrentMode;
                await Console.Out.WriteLineAsync($"Current mode update: {currentMode.CurrentModeId}");
            }
            else if (update is Plan plan)
            {
                await EnsureNewLineAsync(MessageType.Plan);
                _lastMessage = MessageType.Plan;
                await Console.Out.WriteLineAsync($"Plan update:");
                foreach (var entry in plan.Entries)
                {
                    await Console.Out.WriteLineAsync($"  - {entry.Priority} {entry.Status} -> {entry.Content}");
                }
            }
            else if (update is ToolCall toolCall)
            {
                await EnsureNewLineAsync(MessageType.ToolCall);
                await Console.Out.WriteLineAsync($"Tool call: {toolCall.ToolCallId} `{toolCall.Title}` {toolCall.Kind} {toolCall.Status}");
                _lastMessage = MessageType.ToolCall;
            }
            else if (update is SessionUpdateToolCallUpdate toolCallUpdate)
            {
                if (_lastMessage != MessageType.ToolCall)
                    await EnsureNewLineAsync(MessageType.ToolCallUpdate);
                _lastMessage = MessageType.ToolCallUpdate;
                await Console.Out.WriteLineAsync($"Tool call: {toolCallUpdate.ToolCallId} `{toolCallUpdate.Title}` {toolCallUpdate.Kind} {toolCallUpdate.Status}");
            }
            else if (update is SessionUpdateUserMessageChunk userMessage)
            {
                _lastMessage = MessageType.UserMessage;
                await Console.Out.WriteLineAsync($"User message: {ContentBlockText(userMessage.Content)}");
            }
            else if (update is UsageUpdate usage)
            {
                await EnsureNewLineAsync(MessageType.Usage);
                _lastMessage = MessageType.Usage;
                await Console.Out.WriteLineAsync($"Context: {usage.Size} | Used: {usage.Used}");
            }
            else if (update is SessionInfoUpdate sessionInfo)
            {
                await EnsureNewLineAsync(MessageType.SessionInfo);
                _lastMessage = MessageType.SessionInfo;
                await Console.Out.WriteLineAsync($"Session info: Title - {sessionInfo.Title}");
            }
            else
            {
                await Console.Out.WriteLineAsync($"Unhandled session update type: {update.GetType().Name}");
            }

            Console.ForegroundColor = oldColor;
        }

        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<KillTerminalResponse> KillTerminalAsync(
            KillTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TerminalOutputResponse> TerminalOutputAsync(
            TerminalOutputRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private string ContentBlockText(ContentBlock block)
        {
            if (block is TextContent text)
                return text.Text;

            if (block is ImageContent image)
                return $"[Image: {image.MimeType}]";

            if (block is ResourceLink resource)
                return $"[Resource: {resource.Name}({resource.Uri})]";

            if (block is EmbeddedResource embeddedResource)
            {
                if (embeddedResource.Resource is TextResourceContents textResource)
                    return $"[Embedded text resource: {textResource.Text}]";
                if (embeddedResource.Resource is BlobResourceContents blobResource)
                    return $"[Embedded blob resource: {blobResource.Uri}]";
                return $"[Unknown embedded resource: {embeddedResource.Resource.GetType().Name}]";
            }

            if (block is AudioContent audioContent)
                return $"[Audio: {audioContent.MimeType}]";

            return $"[Unsupported content block type {block.GetType().Name}]";
        }

        private async Task EnsureNewLineAsync(MessageType type, string message = "")
        {
            if (type != _lastMessage)
                await Console.Out.WriteLineAsync(message);
        }

        internal void EndTurn(StopReason reason)
        {
            _lastMessage = MessageType.Unknown;
        }

        public void OnDisconnected(Connection connection)
        {
        }

        public Task CompleteAsync(CompleteElicitationNotification notification, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CreateElicitationResponse> CreateAsync(CreateElicitationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
