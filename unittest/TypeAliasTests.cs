using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.protocol;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests for type alias structs generated from schema.json.
    /// </summary>
    [TestClass]
    public class TypeAliasTests
    {
#region SessionId Tests (string alias)

        [TestMethod]
        public void SessionId_IsStructNotClass()
        {
            var type = typeof(SessionId);
            Assert.IsTrue(type.IsValueType, "SessionId should be a struct (value type), not a class");
        }

        [TestMethod]
        public void SessionId_ImplicitConversionFromString()
        {
            string testValue = "test-session-id";
            SessionId sessionId = testValue;

            string result = sessionId;
            Assert.AreEqual(testValue, result);
        }

        [TestMethod]
        public void SessionId_ImplicitConversionToString()
        {
            SessionId sessionId = new SessionId("test-session-id");
            string result = sessionId;

            Assert.AreEqual("test-session-id", result);
        }

        [TestMethod]
        public void SessionId_SerializesAsString()
        {
            SessionId sessionId = new SessionId("test-session-id");
            string json = JsonSerializer.Serialize(sessionId);

            Assert.AreEqual("\"test-session-id\"", json);
        }

        [TestMethod]
        public void SessionId_DeserializesFromString()
        {
            string json = "\"test-session-id\"";
            SessionId sessionId = JsonSerializer.Deserialize<SessionId>(json);

            string result = sessionId;
            Assert.AreEqual("test-session-id", result);
        }

        [TestMethod]
        public void SessionId_RoundTripSerialization()
        {
            SessionId original = new SessionId("test-session-id");
            string json = JsonSerializer.Serialize(original);
            SessionId deserialized = JsonSerializer.Deserialize<SessionId>(json);

            Assert.AreEqual(original, deserialized);
        }

        [TestMethod]
        public void SessionId_EqualsWorks()
        {
            SessionId id1 = new SessionId("test-id");
            SessionId id2 = new SessionId("test-id");
            SessionId id3 = new SessionId("different-id");

            Assert.AreEqual(id1, id2);
            Assert.AreNotEqual(id1, id3);
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsFalse(id1.Equals(id3));
        }

        [TestMethod]
        public void SessionId_GetHashCodeConsistent()
        {
            SessionId id1 = new SessionId("test-id");
            SessionId id2 = new SessionId("test-id");

            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [TestMethod]
        public void SessionId_ToStringWorks()
        {
            SessionId sessionId = new SessionId("test-session-id");
            Assert.AreEqual("test-session-id", sessionId.ToString());
        }

#endregion

#region ProtocolVersion Tests (ushort alias)

        [TestMethod]
        public void ProtocolVersion_IsStructNotClass()
        {
            var type = typeof(ProtocolVersion);
            Assert.IsTrue(type.IsValueType, "ProtocolVersion should be a struct (value type), not a class");
        }

        [TestMethod]
        public void ProtocolVersion_ImplicitConversionFromUshort()
        {
            ushort testValue = 42;
            ProtocolVersion version = testValue;

            ushort result = version;
            Assert.AreEqual(testValue, result);
        }

        [TestMethod]
        public void ProtocolVersion_SerializesAsNumber()
        {
            ProtocolVersion version = new ProtocolVersion(42);
            string json = JsonSerializer.Serialize(version);

            Assert.AreEqual("42", json);
        }

        [TestMethod]
        public void ProtocolVersion_DeserializesFromNumber()
        {
            string json = "42";
            ProtocolVersion version = JsonSerializer.Deserialize<ProtocolVersion>(json);

            ushort result = version;
            Assert.AreEqual((ushort)42, result);
        }

        [TestMethod]
        public void ProtocolVersion_RoundTripSerialization()
        {
            ProtocolVersion original = new ProtocolVersion(42);
            string json = JsonSerializer.Serialize(original);
            ProtocolVersion deserialized = JsonSerializer.Deserialize<ProtocolVersion>(json);

            Assert.AreEqual(original, deserialized);
        }

        [TestMethod]
        public void ProtocolVersion_GetHashCodeWorks()
        {
            ProtocolVersion v1 = new ProtocolVersion(42);
            ProtocolVersion v2 = new ProtocolVersion(42);

            Assert.AreEqual(v1.GetHashCode(), v2.GetHashCode());
        }

#endregion

#region All String Type Aliases Tests

        [TestMethod]

        public void AllStringTypeAliases_CanSerializeAndDeserialize()
        {
            // Test all string-based type aliases
            var stringAliasTypes = new[]
            {
                typeof(SessionId),
                typeof(SessionConfigValueId),
                typeof(SessionConfigId),
                typeof(SessionConfigGroupId),
                typeof(SessionModeId),
                typeof(ToolCallId),
                typeof(PermissionOptionId)
            };

            foreach (var aliasType in stringAliasTypes)
            {
                // Create instance using reflection
                var testValue = $"test-{aliasType.Name.ToLower()}";
                var instance = Activator.CreateInstance(aliasType, testValue);

                // Serialize
                var json = JsonSerializer.Serialize(instance, aliasType);
                Assert.AreEqual($"\"{testValue}\"", json, $"{aliasType.Name} should serialize as string");

                // Deserialize
                var deserialized = JsonSerializer.Deserialize(json, aliasType);
                Assert.AreEqual(instance, deserialized, $"{aliasType.Name} round-trip should work");
            }
        }

        [TestMethod]
        public void AllStringTypeAliases_AreStructs()
        {
            var stringAliasTypes = new[]
            {
                typeof(SessionId),
                typeof(SessionConfigValueId),
                typeof(SessionConfigId),
                typeof(SessionConfigGroupId),
                typeof(SessionModeId),
                typeof(ToolCallId),
                typeof(PermissionOptionId)
            };

            foreach (var aliasType in stringAliasTypes)
            {
                Assert.IsTrue(aliasType.IsValueType, $"{aliasType.Name} should be a struct");
            }
        }

#endregion

#region Integration Tests
        [TestMethod]
        public void TypeAliasInComplexObject_WorksWithPromptRequest()
        {
            var sessionId = new SessionId("test-session-456");
            var request = new PromptRequest
            {
                SessionId = sessionId,
                Prompt = new List<ContentBlock>()
            };

            var json = JsonSerializer.Serialize(request);
            Assert.IsTrue(json.Contains("\"sessionId\":\"test-session-456\""),
                "SessionId should serialize as string in JSON");

            var deserialized = JsonSerializer.Deserialize<PromptRequest>(json);
            string deserializedId = deserialized.SessionId;
            Assert.AreEqual("test-session-456", (string)sessionId);
        }

#endregion
    }
}
