using System.Collections.Generic;
using Newtonsoft.Json;
using dotacp.protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotacp.unittest
{
    /// <summary>
    /// Tests for anyOf handling in generated schema types.
    /// Verifies enum and union type behavior.
    /// </summary>
    [TestClass]
    public class AnyOfSchemaTests
    {
#region ErrorCode Enum Tests

        [TestMethod]
        public void ErrorCode_IntegerEnum_CanAssignValues()
        {
            // Arrange & Act
            ErrorCode parseError = ErrorCode.ParseError;
            ErrorCode invalidRequest = ErrorCode.InvalidRequest;
            ErrorCode other = ErrorCode.Other;

            // Assert
            Assert.AreEqual(-32700, (int)parseError);
            Assert.AreEqual(-32600, (int)invalidRequest);
            Assert.AreEqual(0, (int)other);
        }

        [TestMethod]
        public void ErrorCode_CanSerializeToJson()
        {
            // Arrange
            var error = new Error
            {
                Code = ErrorCode.InvalidRequest,
                Message = "Test error"
            };

            // Act
            string json = JsonConvert.SerializeObject(error);

            // Assert
            // Integer enums serialize as numbers, not strings
            Assert.Contains("-32600", json, $"Should serialize as integer value, got: {json}");
            Assert.Contains("Test error", json);
        }

        [TestMethod]
        public void ErrorCode_CanDeserializeFromJson()
        {
            // Arrange - integer enums can be deserialized from numbers
            string json = "{\"code\":-32601,\"message\":\"Method not found\"}";

            // Act
            var error = JsonConvert.DeserializeObject<Error>(json);

            // Assert
            Assert.IsNotNull(error);
            Assert.AreEqual(ErrorCode.MethodNotFound, error.Code);
            Assert.AreEqual("Method not found", error.Message);
        }

        [TestMethod]
        public void ErrorCode_CanConvertToAndFromInt()
        {
            // Arrange
            int errorValue = -32700;

            // Act
            ErrorCode code = (ErrorCode)errorValue;
            int backToInt = (int)code;

            // Assert
            Assert.AreEqual(ErrorCode.ParseError, code);
            Assert.AreEqual(errorValue, backToInt);
        }

#endregion

#region SessionConfigOptionCategory String Enum Tests

        [TestMethod]
        public void SessionConfigOptionCategory_SerializesToCorrectJsonValues()
        {
            // Arrange
            var categories = new[]
            {
                SessionConfigOptionCategory.Mode,
                SessionConfigOptionCategory.Model,
                SessionConfigOptionCategory.ThoughtLevel,
                SessionConfigOptionCategory.Other
            };

            var expectedValues = new[] { "mode", "model", "thought_level", "other" };

            // Act & Assert
            for (int i = 0; i < categories.Length; i++)
            {
                string json = JsonConvert.SerializeObject(categories[i]);
                Assert.Contains(expectedValues[i], json, $"Expected {expectedValues[i]} but got {json}");
            }
        }

        [TestMethod]
        public void SessionConfigOptionCategory_CanDeserializeFromJson()
        {
            // Arrange
            string json = "\"thought_level\"";

            // Act
            var category = JsonConvert.DeserializeObject<SessionConfigOptionCategory>(json);

            // Assert
            Assert.AreEqual(SessionConfigOptionCategory.ThoughtLevel, category);
        }

#endregion

#region RequestId Union Type Tests

        [TestMethod]
        public void RequestId_CanHoldLongValue()
        {
            // Arrange & Act
            RequestId id = 12345L;

            // Assert
            Assert.IsTrue(id.TryGetLong(out long value));
            Assert.AreEqual(12345L, value);
            Assert.IsFalse(id.IsNull);
        }

        [TestMethod]
        public void RequestId_CanHoldStringValue()
        {
            // Arrange & Act
            RequestId id = "test-request-123";

            // Assert
            Assert.IsTrue(id.TryGetString(out string value));
            Assert.AreEqual("test-request-123", value);
            Assert.IsFalse(id.IsNull);
        }

        [TestMethod]
        public void RequestId_CanBeNull()
        {
            // Arrange & Act
            RequestId id = RequestId.Null;

            // Assert
            Assert.IsTrue(id.IsNull);
            Assert.IsFalse(id.TryGetLong(out _));
            Assert.IsFalse(id.TryGetString(out _));
        }

        [TestMethod]
        public void RequestId_TryGetReturnsFalseForWrongType()
        {
            // Arrange
            RequestId id = "string-value";

            // Act & Assert
            Assert.IsFalse(id.TryGetLong(out long longValue));
            Assert.AreEqual(0L, longValue); // Should be default value

            Assert.IsTrue(id.TryGetString(out string stringValue));
            Assert.AreEqual("string-value", stringValue);
        }

        [TestMethod]
        public void RequestId_SerializeLongValue()
        {
            // Arrange
            var request = new ClientRequest
            {
                Id = 42L,
                Method = "test"
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"id\":42", json, $"Expected id:42 but got {json}");
        }

        [TestMethod]
        public void RequestId_SerializeStringValue()
        {
            // Arrange
            var request = new ClientRequest
            {
                Id = "request-123",
                Method = "test"
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"id\":\"request-123\"",
                json, $"Expected id:\"request-123\" but got {json}");
        }

        [TestMethod]
        public void RequestId_SerializeNullValue()
        {
            // Arrange
            var request = new ClientRequest
            {
                Id = RequestId.Null,
                Method = "test"
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"id\":null",
                json, $"Expected id:null but got {json}");
        }

        [TestMethod]
        public void RequestId_DeserializeLongValue()
        {
            // Arrange
            string json = "{\"id\":100,\"method\":\"test\"}";

            // Act
            var request = JsonConvert.DeserializeObject<ClientRequest>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.Id.TryGetLong(out long value));
            Assert.AreEqual(100L, value);
        }

        [TestMethod]
        public void RequestId_DeserializeStringValue()
        {
            // Arrange
            string json = "{\"id\":\"test-id\",\"method\":\"test\"}";

            // Act
            var request = JsonConvert.DeserializeObject<ClientRequest>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.Id.TryGetString(out string value));
            Assert.AreEqual("test-id", value);
        }

        [TestMethod]
        public void RequestId_DeserializeNullValue()
        {
            // Arrange
            string json = "{\"id\":null,\"method\":\"test\"}";

            // Act
            var request = JsonConvert.DeserializeObject<ClientRequest>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.Id.IsNull);
        }

        [TestMethod]
        public void RequestId_EqualsWorksCorrectly()
        {
            // Arrange
            RequestId id1 = 123L;
            RequestId id2 = 123L;
            RequestId id3 = 456L;
            RequestId id4 = "123";

            // Act & Assert
            Assert.AreEqual(id1, id2);
            Assert.AreNotEqual(id1, id3);
            Assert.AreNotEqual(id1, id4); // Different types
        }

        [TestMethod]
        public void RequestId_GetHashCodeWorksCorrectly()
        {
            // Arrange
            RequestId id1 = 123L;
            RequestId id2 = 123L;
            RequestId id3 = "test";

            // Act
            int hash1 = id1.GetHashCode();
            int hash2 = id2.GetHashCode();
            int hash3 = id3.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2, "Same values should have same hash code");
            Assert.AreNotEqual(hash1, hash3, "Different values should have different hash codes");
        }

        [TestMethod]
        public void RequestId_ToStringWorks()
        {
            // Arrange
            RequestId longId = 999L;
            RequestId stringId = "test-id";
            RequestId nullId = RequestId.Null;

            // Act & Assert
            Assert.AreEqual("999", longId.ToString());
            Assert.AreEqual("test-id", stringId.ToString());
            Assert.AreEqual(string.Empty, nullId.ToString());
        }

#endregion

#region Integration Tests

        [TestMethod]
        public void Error_WithErrorCodeEnum_RoundTripSerialization()
        {
            // Arrange
            var originalError = new Error
            {
                Code = ErrorCode.MethodNotFound,
                Message = "The method 'test' does not exist",
                Data = new { detail = "Additional info" }
            };

            // Act
            string json = JsonConvert.SerializeObject(originalError);
            var deserializedError = JsonConvert.DeserializeObject<Error>(json);

            // Assert
            Assert.IsNotNull(deserializedError);
            Assert.AreEqual(originalError.Code, deserializedError.Code);
            Assert.AreEqual(originalError.Message, deserializedError.Message);
        }

        [TestMethod]
        public void ClientRequest_WithRequestIdUnion_RoundTripSerialization()
        {
            // Arrange
            var requests = new[]
            {
                new ClientRequest { Id = 1L, Method = "method1" },
                new ClientRequest { Id = "string-id", Method = "method2" },
                new ClientRequest { Id = RequestId.Null, Method = "method3" }
            };

            foreach (var originalRequest in requests)
            {
                // Act
                string json = JsonConvert.SerializeObject(originalRequest);
                var deserializedRequest = JsonConvert.DeserializeObject<ClientRequest>(json);

                // Assert
                Assert.IsNotNull(deserializedRequest);
                Assert.AreEqual(originalRequest.Method, deserializedRequest.Method);

                if (originalRequest.Id.IsNull)
                {
                    Assert.IsTrue(deserializedRequest.Id.IsNull);
                }
                else if (originalRequest.Id.TryGetLong(out long longVal))
                {
                    Assert.IsTrue(deserializedRequest.Id.TryGetLong(out long deserializedLong));
                    Assert.AreEqual(longVal, deserializedLong);
                }
                else if (originalRequest.Id.TryGetString(out string stringVal))
                {
                    Assert.IsTrue(deserializedRequest.Id.TryGetString(out string deserializedString));
                    Assert.AreEqual(stringVal, deserializedString);
                }
            }
        }

#endregion
    }
}
