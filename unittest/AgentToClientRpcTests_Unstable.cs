using dotacp.protocol.unstable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotacp.unittest
{
    /// <summary>
    /// Unstable-only agent->client RPC tests.
    /// Keeps unstable variant coverage out of stable test files.
    /// </summary>
    [TestClass]
    public class AgentToClientRpcTests_Unstable
    {
        [TestMethod]
        public async Task SessionUpdateAsync_UsageUpdateVariant()
        {
            using (var pair = ConnectionPair_Unstable.Create())
            {
                var notification = new SessionNotification
                {
                    SessionId = "session-unstable",
                    Update = new UsageUpdate
                    {
                        Meta = new Dictionary<string, object> { { "kind", "usage" } },
                        Cost = new Cost { Amount = 1.23, Currency = "USD" },
                        Size = 8192,
                        Used = 2048
                    }
                };

                var agentConn = pair.Agent.ReceivedConnection!;
                await agentConn.SessionUpdateAsync(notification);

                var received = await Task.WhenAny(
                    pair.Client.SessionUpdateReceivedSignal.Task,
                    Task.Delay(5000));

                Assert.AreEqual(pair.Client.SessionUpdateReceivedSignal.Task, received,
                    "Unstable usage_update notification was not received within timeout");
                Assert.IsNotNull(pair.Client.LastSessionNotification);
                Assert.AreEqual("session-unstable", (string)pair.Client.LastSessionNotification!.SessionId);
                Assert.AreEqual("usage_update", pair.Client.LastSessionNotification.Update.SessionUpdateValue);
            }
        }
    }
}
