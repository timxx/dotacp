using Nerdbank.Streams;
using System;
using System.IO;

using AgentConnection = dotacp.agent.unstable.Connection;
using ClientConnection = dotacp.client.unstable.Connection;

namespace dotacp.unittest
{
    /// <summary>
    /// Holds a connected pair of agent and client connections for testing (unstable API).
    /// </summary>
    internal sealed class ConnectionPair_Unstable : IDisposable
    {
        public MockAgent_Unstable Agent { get; }
        public MockClient_Unstable Client { get; }
        public ClientConnection ClientConn { get; }
        private readonly Stream _stream1;
        private readonly Stream _stream2;

        private ConnectionPair_Unstable(MockAgent_Unstable agent, MockClient_Unstable client,
            ClientConnection clientConn,
            Stream stream1, Stream stream2)
        {
            Agent = agent;
            Client = client;
            ClientConn = clientConn;
            _stream1 = stream1;
            _stream2 = stream2;
        }

        public static ConnectionPair_Unstable Create()
        {
            var streams = FullDuplexStream.CreatePair();
            var agent = new MockAgent_Unstable();
            var client = new MockClient_Unstable();

            AgentConnection.RunAgent(agent, streams.Item1, streams.Item1);
            var clientConn = ClientConnection.RunClient(client, streams.Item2, streams.Item2)!;

            return new ConnectionPair_Unstable(agent, client, clientConn,
                streams.Item1, streams.Item2);
        }

        public void Dispose()
        {
            _stream1.Dispose();
            _stream2.Dispose();
            ClientConn.Dispose();
        }
    }
}
