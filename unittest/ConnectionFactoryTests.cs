using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nerdbank.Streams;
using System.Diagnostics;
using AgentConnection = dotacp.agent.unstable.Connection;
using ClientConnection = dotacp.client.unstable.Connection;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests Connection factory method null-argument handling.
    /// </summary>
    [TestClass]
    public class ConnectionFactoryTests
    {
        [TestMethod]
        public void RunAgent_NullAgent_ReturnsNull()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = AgentConnection.RunAgent(null!, streams.Item1, streams.Item1);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunAgent_NullInputStream_ReturnsNull()
        {
            var agent = new MockAgent();
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = AgentConnection.RunAgent(agent, null!, streams.Item1);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunAgent_NullOutputStream_ReturnsNull()
        {
            var agent = new MockAgent();
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = AgentConnection.RunAgent(agent, streams.Item1, null!);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunAgent_TraceSource_ReturnsConnection()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var agent = new MockAgent();
                var traceSource = new TraceSource("JsonRpc", SourceLevels.Verbose);
                var result = AgentConnection.RunAgent(agent, streams.Item1, streams.Item1, traceSource);
                Assert.IsNotNull(result);
                Assert.IsNotNull(agent.ReceivedConnection);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunClient_NullClient_ReturnsNull()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = ClientConnection.RunClient(null!, streams.Item1, streams.Item1);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunClient_NullInputStream_ReturnsNull()
        {
            var client = new MockClient();
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = ClientConnection.RunClient(client, null!, streams.Item1);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunClient_NullOutputStream_ReturnsNull()
        {
            var client = new MockClient();
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var result = ClientConnection.RunClient(client, streams.Item1, null!);
                Assert.IsNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunAgent_ValidArgs_ReturnsConnection()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var agent = new MockAgent();
                var result = AgentConnection.RunAgent(agent, streams.Item1, streams.Item1);
                Assert.IsNotNull(result);
                Assert.IsNotNull(agent.ReceivedConnection);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunClient_ValidArgs_ReturnsConnection()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var client = new MockClient();
                var result = ClientConnection.RunClient(client, streams.Item1, streams.Item1);
                Assert.IsNotNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void RunClient_TraceSource_ReturnsConnection()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var client = new MockClient();
                var traceSource = new TraceSource("JsonRpc", SourceLevels.Verbose);
                var result = ClientConnection.RunClient(client, streams.Item1, streams.Item1, traceSource);
                Assert.IsNotNull(result);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void TestClientCompletion()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var client = new MockClient();
                var result = ClientConnection.RunClient(client, streams.Item1, streams.Item1);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Completion);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }

        [TestMethod]
        public void TestAgentCompletion()
        {
            var streams = FullDuplexStream.CreatePair();
            try
            {
                var agent = new MockAgent();
                var result = AgentConnection.RunAgent(agent, streams.Item1, streams.Item1);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Completion);
            }
            finally
            {
                streams.Item1.Dispose();
                streams.Item2.Dispose();
            }
        }
    }
}
