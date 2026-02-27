using StreamJsonRpc;
using System.IO;

namespace dotacp.client
{
    public class Connection
    {
        private JsonRpc _rpc;
        private IAcpClient _client;

        public static Connection ConnectToAgent(IAcpClient client,
            Stream inputStream, Stream outputStream)
        {
            if (client == null || inputStream == null || outputStream == null)
                return null;

            return new Connection(client, inputStream, outputStream);
        }

        private Connection(IAcpClient client, Stream inputStream, Stream outputStream)
        {
            _client = client;

            _rpc = new JsonRpc(inputStream, outputStream);
            _rpc.StartListening();
        }
    }
}
