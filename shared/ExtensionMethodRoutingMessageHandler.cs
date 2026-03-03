using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.shared
{
    /// <summary>
    /// Message handler that routes extension methods (starting with "_") to the appropriate handler.
    /// This wrapper intercepts JSON-RPC messages and transforms method names that start with "_"
    /// into calls to handler methods by wrapping the payload appropriately.
    /// </summary>
    internal class ExtensionMethodRoutingMessageHandler : IJsonRpcMessageHandler
    {
        private readonly IJsonRpcMessageHandler _innerHandler;

        public ExtensionMethodRoutingMessageHandler(IJsonRpcMessageHandler innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public IJsonRpcMessageFormatter Formatter => _innerHandler.Formatter;

        public bool CanRead => _innerHandler.CanRead;

        public bool CanWrite => _innerHandler.CanWrite;

        public async ValueTask<JsonRpcMessage?> ReadAsync(CancellationToken cancellationToken)
        {
            var message = await _innerHandler.ReadAsync(cancellationToken);

            // Only intercept extension method calls (those starting with "_")
            if (message is JsonRpcRequest request && !string.IsNullOrEmpty(request.Method)
                && request.Method!.StartsWith("_", StringComparison.Ordinal))
            {
                var wrappedRequest = new JsonRpcRequest()
                {
                    RequestId = request.RequestId,
                    Method = request.IsNotification
                        ? "__acp_ext_notification__"
                        : "__acp_ext_method__",
                    Arguments = new object[]
                    {
                        new ExtensionRequest
                        {
                            Method = request.Method.Substring(1),
                            Arguments = request.Arguments ?? new object()
                        }
                    }
                };
                return wrappedRequest;
            }

            return message;
        }

        public async ValueTask WriteAsync(JsonRpcMessage message, CancellationToken cancellationToken)
        {
            await _innerHandler.WriteAsync(message, cancellationToken);
        }
    }

    /// <summary>
    /// Represents an extension method/notification request payload.
    /// </summary>
    internal class ExtensionRequest
    {
        /// <summary>Gets or sets the extension method/notification name (without the "_" prefix).</summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>Gets or sets the request payload.</summary>
        public object Arguments { get; set; } = null!;
    }
}
