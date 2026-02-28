using dotacp.protocol;
using StreamJsonRpc;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dotacp.client
{
    public class Connection
    {
        private JsonRpc _rpc;
        private IAcpClient _client;

        public static Connection ConnectToAgent(IAcpClient client,
            Stream inputStream, Stream outputStream,
            TraceSource traceSource = null)
        {
            if (client == null || inputStream == null || outputStream == null)
                return null;

            return new Connection(client, inputStream, outputStream, traceSource);
        }

        private Connection(IAcpClient client, Stream inputStream, Stream outputStream,
            TraceSource traceSource = null)
        {
            _client = client;

            var handler = new NewLineDelimitedMessageHandler(
                inputStream, outputStream, new JsonMessageFormatter());
            _rpc = new JsonRpc(handler);
            if (traceSource != null)
                _rpc.TraceSource = traceSource;

            _rpc.StartListening();
        }

        public Task<InitializeResponse> InitializeAsync(InitializeRequest request,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<InitializeRequest, InitializeResponse>(
                AgentMethods.Initialize, request, cancellationToken);
        }

        private Task<TResponse> SendRequestAsync<TRequest, TResponse>(
            string method, TRequest request, CancellationToken cancellationToken)
        {
            return _rpc.InvokeWithParameterObjectAsync<TResponse>(
                method, request, cancellationToken);
        }
    }
}
