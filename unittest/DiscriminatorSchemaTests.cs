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

        [TestMethod]
        public void McpServer_Http_DeserializesWithConstDiscriminator()
        {
            // Arrange
            string json = @"{
                ""type"": ""http"",
                ""name"": ""TestHttp"",
                ""url"": ""http://localhost:3000"",
                ""headers"": [{""name"": ""X-API-Key"", ""value"": ""secret""}]
            }";

            // Act
            var server = JsonConvert.DeserializeObject<McpServer>(json);

            // Assert
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(McpServerHttp));
            Assert.AreEqual("http", server.Type);

            var httpServer = (McpServerHttp)server;
            Assert.AreEqual("TestHttp", httpServer.Name);
            Assert.AreEqual("http://localhost:3000", httpServer.Url);
            Assert.HasCount(1, httpServer.Headers);
            Assert.AreEqual("X-API-Key", httpServer.Headers[0].Name);
        }

        [TestMethod]
        public void McpServer_Sse_DeserializesWithConstDiscriminator()
        {
            // Arrange
            string json = @"{
                ""type"": ""sse"",
                ""name"": ""TestSSE"",
                ""url"": ""https://example.com/sse"",
                ""headers"": []
            }";

            // Act
            var server = JsonConvert.DeserializeObject<McpServer>(json);

            // Assert
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(McpServerSse));
            Assert.AreEqual("sse", server.Type);

            var sseServer = (McpServerSse)server;
            Assert.AreEqual("TestSSE", sseServer.Name);
            Assert.AreEqual("https://example.com/sse", sseServer.Url);
            Assert.IsEmpty(sseServer.Headers);
        }

        [TestMethod]
        public void McpServer_Stdio_DeserializesWithTitleBasedDiscriminator()
        {
            // Arrange
            string json = @"{
                ""type"": ""stdio"",
                ""name"": ""TestStdio"",
                ""command"": ""/usr/bin/mcp-server"",
                ""args"": [""--config"", ""config.json""],
                ""env"": [{""name"": ""DEBUG"", ""value"": ""true""}]
            }";

            // Act
            var server = JsonConvert.DeserializeObject<McpServer>(json);

            // Assert
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(McpServerStdio));
            Assert.AreEqual("stdio", server.Type);

            var stdioServer = (McpServerStdio)server;
            Assert.AreEqual("TestStdio", stdioServer.Name);
            Assert.AreEqual("/usr/bin/mcp-server", stdioServer.Command);
            Assert.HasCount(2, stdioServer.Args);
            Assert.AreEqual("--config", stdioServer.Args[0]);
            Assert.AreEqual("config.json", stdioServer.Args[1]);
            Assert.HasCount(1, stdioServer.Env);
            Assert.AreEqual("DEBUG", stdioServer.Env[0].Name);
        }

        [TestMethod]
        public void McpServer_Http_SerializesWithDiscriminator()
        {
            // Arrange
            var server = new McpServerHttp
            {
                Name = "MyHttp",
                Url = "http://localhost:8080",
                Headers = new[] { new HttpHeader { Name = "Auth", Value = "Bearer token" } }
            };

            // Act
            string json = JsonConvert.SerializeObject(server);
            var deserialized = JsonConvert.DeserializeObject<McpServer>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.IsInstanceOfType(deserialized, typeof(McpServerHttp));
            Assert.AreEqual("http", deserialized.Type);
            Assert.AreEqual("MyHttp", ((McpServerHttp)deserialized).Name);
        }

        [TestMethod]
        public void McpServer_MultipleVariants_DeserializeCorrectly()
        {
            // Arrange
            var jsonVariants = new[]
            {
                @"{""type"": ""http"", ""name"": ""Http"", ""url"": ""http://example.com"", ""headers"": []}",
                @"{""type"": ""sse"", ""name"": ""Sse"", ""url"": ""https://example.com"", ""headers"": []}",
                @"{""type"": ""stdio"", ""name"": ""Stdio"", ""command"": ""cmd"", ""args"": [], ""env"": []}"
            };

            // Act & Assert
            var httpServer = JsonConvert.DeserializeObject<McpServer>(jsonVariants[0]);
            Assert.IsInstanceOfType(httpServer, typeof(McpServerHttp));
            Assert.AreEqual("http", httpServer.Type);

            var sseServer = JsonConvert.DeserializeObject<McpServer>(jsonVariants[1]);
            Assert.IsInstanceOfType(sseServer, typeof(McpServerSse));
            Assert.AreEqual("sse", sseServer.Type);

            var stdioServer = JsonConvert.DeserializeObject<McpServer>(jsonVariants[2]);
            Assert.IsInstanceOfType(stdioServer, typeof(McpServerStdio));
            Assert.AreEqual("stdio", stdioServer.Type);
        }
    }
}
