using dotacp.client;
using dotacp.protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace clientcli
{
    internal class Client : IAcpClient
    {
        public Task<RequestPermissionResponse> RequestPermissionAsync(
            RequestPermissionRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task SessionUpdateAsync(SessionNotification notification, CancellationToken cancellationToken = default)
        {
            var update = notification.Update;
            if (update is AvailableCommandsUpdate commands)
            {
                Console.WriteLine($"Available commands:");
                foreach (var command in commands.AvailableCommands)
                {
                    Console.WriteLine($"  - {command.Name}: {command.Description}");
                }
            }
            else if (update is SessionUpdateAgentMessageChunk agentMessage)
            {
                Console.WriteLine($"Agent message: {ContentBlockText(agentMessage.Content)}");
            }
            else if (update is SessionUpdateAgentThoughtChunk agentThought)
            {
                Console.WriteLine($"Agent thought: {ContentBlockText(agentThought.Content)}");
            }
            else if (update is ConfigOptionUpdate configOption)
            {
                Console.WriteLine($"Config option update: {configOption.ConfigOptions.Count}");
            }
            else if (update is CurrentModeUpdate currentMode)
            {
                Console.WriteLine($"Current mode update: {currentMode.CurrentModeId}");
            }
            else if (update is Plan plan)
            {
                Console.WriteLine($"Plan update:");
                foreach (var entry in plan.Entries)
                {
                    Console.WriteLine($"  - {entry.Priority} {entry.Status} -> {entry.Content}");
                }
            }
            else if (update is ToolCall toolCall)
            {
                Console.WriteLine($"Tool call update: {toolCall.Title} {toolCall.ToolCallId}");
            }
            else if (update is SessionUpdateToolCallUpdate toolCallUpdate)
            {
                Console.WriteLine($"Tool call update: {toolCallUpdate.Title} {toolCallUpdate.ToolCallId}");
            }
            else if (update is SessionUpdateUserMessageChunk userMessage)
            {
                Console.WriteLine($"User message: {ContentBlockText(userMessage.Content)}");
            }
            else
            {
                Console.WriteLine($"Unhandled session update type: {update.GetType().Name}");
            }

            return Task.CompletedTask;
        }

        public Task<WriteTextFileResponse> WriteTextFileAsync(
            WriteTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReadTextFileResponse> ReadTextFileAsync(
            ReadTextFileRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<CreateTerminalResponse> CreateTerminalAsync(
            CreateTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<KillTerminalCommandResponse> KillTerminalCommandAsync(
            KillTerminalCommandRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReleaseTerminalResponse> ReleaseTerminalAsync(
            ReleaseTerminalRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<TerminalOutputRequest> TerminalOutputAsync(
            TerminalOutputRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<WaitForTerminalExitResponse> WaitForTerminalExitAsync(
            WaitForTerminalExitRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<object> ExtMethodAsync(string method, object request,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task ExtNotificationAsync(string method, object notification,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        private string ContentBlockText(ContentBlock block)
        {
            if (block is TextContent text)
                return text.Text;

            if (block is ImageContent image)
                return $"[Image: {image.MimeType}]";

            if (block is ResourceLink resource)
                return $"[Resource: {resource.Name}({resource.Uri})]";

            // TODO: fix EmbeddedResourceResource convert
            if (block is EmbeddedResource embeddedResource)
                return $"[Embedded resource: ]";

            if (block is AudioContent audioContent)
                return $"[Audio: {audioContent.MimeType}]";

            return $"[Unsupported content block type {block.GetType().Name}]";
        }
    }
}
