using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.protocol;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests for discriminator handling in generated schema types.
    /// </summary>
    [TestClass]
    public class DiscriminatorSchemaTests
    {
        [TestMethod]
        public void ContentBlock_Discriminator_RoundTrip()
        {
            ContentBlock content = new TextContent
            {
                Text = "hello"
            };

            string json = JsonConvert.SerializeObject(content);
            Assert.Contains("\"type\":\"text\"", json, $"Expected discriminator, got: {json}");

            var deserialized = JsonConvert.DeserializeObject<ContentBlock>(json);
            Assert.IsNotNull(deserialized);
            Assert.IsInstanceOfType(deserialized, typeof(TextContent));
            Assert.AreEqual("text", deserialized.Type);
        }

        [TestMethod]
        public void SessionUpdate_UserMessageChunk_DeserializesVariant()
        {
            string json = "{\"sessionUpdate\":\"user_message_chunk\",\"content\":{\"type\":\"text\",\"text\":\"hi\"}}";

            var update = JsonConvert.DeserializeObject<SessionUpdate>(json);
            Assert.IsNotNull(update);
            Assert.IsInstanceOfType(update, typeof(SessionUpdateUserMessageChunk));

            var chunk = (SessionUpdateUserMessageChunk)update;
            Assert.IsNotNull(chunk.Content);
            Assert.IsInstanceOfType(chunk.Content, typeof(TextContent));
        }

        [TestMethod]
        public void RequestPermissionOutcome_Cancelled_DeserializesVariant()
        {
            string json = "{\"outcome\":\"cancelled\"}";

            var outcome = JsonConvert.DeserializeObject<RequestPermissionOutcome>(json);
            Assert.IsNotNull(outcome);
            Assert.IsInstanceOfType(outcome, typeof(RequestPermissionOutcomeCancelled));
            Assert.AreEqual("cancelled", outcome.Outcome);
        }
    }
}
