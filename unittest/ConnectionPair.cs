using Nerdbank.Streams;
using System;
using System.IO;

using AgentConnection = dotacp.agent.Connection;
using ClientConnection = dotacp.client.Connection;

namespace dotacp.unittest
{
    /// <summary>
    /// Holds a connected pair of agent and client connections for testing.
    /// </summary>
    internal sealed class ConnectionPair : IDisposable
    {
        public MockAgent Agent { get; }
        public MockClient Client { get; }
        public ClientConnection ClientConn { get; }
        private readonly Stream _stream1;
        private readonly Stream _stream2;

        private ConnectionPair(MockAgent agent, MockClient client,
            ClientConnection clientConn,
            Stream stream1, Stream stream2)
        {
            Agent = agent;
            Client = client;
            ClientConn = clientConn;
            _stream1 = stream1;
            _stream2 = stream2;
        }

        public static ConnectionPair Create()
        {
            var streams = FullDuplexStream.CreatePair();
            var agent = new MockAgent();
            var client = new MockClient();

            AgentConnection.RunAgent(agent, streams.Item1, streams.Item1);
            var clientConn = ClientConnection.RunClient(client, streams.Item2, streams.Item2)!;

            return new ConnectionPair(agent, client, clientConn,
                streams.Item1, streams.Item2);
        }

        public void Dispose()
        {
            _stream1.Dispose();
            _stream2.Dispose();
        }
    }
}
