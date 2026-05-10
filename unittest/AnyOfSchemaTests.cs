using dotacp.protocol.unstable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
            ErrorCode unknown = (ErrorCode)(-32055);

            // Assert
            Assert.AreEqual(-32700, (int)parseError);
            Assert.AreEqual(-32600, (int)invalidRequest);
            Assert.AreEqual(-32055, (int)unknown);
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

            var unknownError = new Error
            {
                Code = (ErrorCode)(-42055),
                Message = "Unknown error"
            };

            json = JsonConvert.SerializeObject(unknownError);
            Assert.Contains("-42055", json, $"Should serialize as integer value, got: {json}");
            Assert.Contains("Unknown error", json);
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

            json = "{\"code\":-42055,\"message\":\"Unknown error\"}";
            error = JsonConvert.DeserializeObject<Error>(json);
            Assert.IsNotNull(error);
            Assert.AreEqual((ErrorCode)(-42055), error.Code);
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
        public void SessionConfigOptionCategory_SerializesKnownValuesToCorrectJsonValues()
        {
            var categories = new[]
            {
                SessionConfigOptionCategory.Mode,
                SessionConfigOptionCategory.Model,
                SessionConfigOptionCategory.ThoughtLevel,
            };

            var expectedValues = new[] { "mode", "model", "thought_level" };

            for (int i = 0; i < categories.Length; i++)
            {
                string json = JsonConvert.SerializeObject(categories[i]);
                Assert.AreEqual($"\"{expectedValues[i]}\"", json);
            }
        }

        [TestMethod]
        public void SessionConfigOptionCategory_CanDeserializeKnownValueFromJson()
        {
            string json = "\"thought_level\"";

            var category = JsonConvert.DeserializeObject<SessionConfigOptionCategory>(json);

            Assert.AreEqual(SessionConfigOptionCategory.ThoughtLevel, category);
            Assert.AreEqual("thought_level", (string)category);
        }

        [TestMethod]
        public void SessionConfigOptionCategory_CanDeserializeUnknownValueFromJson()
        {
            string json = "\"permissions\"";

            var category = JsonConvert.DeserializeObject<SessionConfigOptionCategory>(json);

            Assert.AreEqual("permissions", (string)category);
            Assert.AreNotEqual(SessionConfigOptionCategory.Mode, category);
        }

        [TestMethod]
        public void SessionConfigOptionCategory_UnknownValue_RoundTripsUnchanged()
        {
            SessionConfigOptionCategory category = "permissions";

            string json = JsonConvert.SerializeObject(category);
            var roundTripped = JsonConvert.DeserializeObject<SessionConfigOptionCategory>(json);

            Assert.AreEqual("\"permissions\"", json);
            Assert.AreEqual(category, roundTripped);
            Assert.AreEqual("permissions", (string)roundTripped);
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
            var request = new CancelRequestNotification
            {
                RequestId = 42L,
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"requestId\":42", json, $"Expected requestId:42 but got {json}");
        }

        [TestMethod]
        public void RequestId_SerializeStringValue()
        {
            // Arrange
            var request = new CancelRequestNotification
            {
                RequestId = "request-123",
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"requestId\":\"request-123\"",
                json, $"Expected requestId:\"request-123\" but got {json}");
        }

        [TestMethod]
        public void RequestId_SerializeNullValue()
        {
            // Arrange
            var request = new CancelRequestNotification
            {
                RequestId = RequestId.Null,
            };

            // Act
            string json = JsonConvert.SerializeObject(request);

            // Assert
            Assert.Contains("\"requestId\":null",
                json, $"Expected requestId:null but got {json}");
        }

        [TestMethod]
        public void RequestId_DeserializeLongValue()
        {
            // Arrange
            string json = "{\"requestId\":100}";

            // Act
            var request = JsonConvert.DeserializeObject<CancelRequestNotification>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.RequestId.TryGetLong(out long value));
            Assert.AreEqual(100L, value);
        }

        [TestMethod]
        public void RequestId_DeserializeStringValue()
        {
            // Arrange
            string json = "{\"requestId\":\"test-id\"}";
            // Act
            var request = JsonConvert.DeserializeObject<CancelRequestNotification>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.RequestId.TryGetString(out string value));
            Assert.AreEqual("test-id", value);
        }

        [TestMethod]
        public void RequestId_DeserializeNullValue()
        {
            // Arrange
            string json = "{\"requestId\":null}";

            // Act
            var request = JsonConvert.DeserializeObject<CancelRequestNotification>(json);

            // Assert
            Assert.IsNotNull(request);
            Assert.IsTrue(request.RequestId.IsNull);
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

        [TestMethod]
        public void SessionConfigSelectOptions_CanHoldUngroupedOptionsArray()
        {
            // Arrange
            var optionValues = new[]
            {
                new SessionConfigSelectOption { Value = "v1", Name = "Option 1" }
            };

            // Act
            SessionConfigSelectOptions options = optionValues;

            // Assert
            Assert.IsTrue(options.TryGetSessionConfigSelectOption(out var ungrouped));
            Assert.HasCount(1, ungrouped);
            Assert.AreEqual("v1", (string)ungrouped[0].Value);
            Assert.IsFalse(options.TryGetSessionConfigSelectGroup(out _));
        }

        [TestMethod]
        public void SessionConfigSelectOptions_CanHoldGroupedOptionsArray()
        {
            // Arrange
            var groupedValues = new[]
            {
                new SessionConfigSelectGroup
                {
                    Group = "g1",
                    Name = "Group 1",
                    Options = new[]
                    {
                        new SessionConfigSelectOption { Value = "v1", Name = "Option 1" }
                    }
                }
            };

            // Act
            SessionConfigSelectOptions options = groupedValues;

            // Assert
            Assert.IsTrue(options.TryGetSessionConfigSelectGroup(out var grouped));
            Assert.HasCount(1, grouped);
            Assert.AreEqual("g1", (string)grouped[0].Group);
            Assert.IsFalse(options.TryGetSessionConfigSelectOption(out _));
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
                new CancelRequestNotification { RequestId = 1L},
                new CancelRequestNotification { RequestId = "string-id" },
                new CancelRequestNotification { RequestId = RequestId.Null}
            };

            foreach (var originalRequest in requests)
            {
                // Act
                string json = JsonConvert.SerializeObject(originalRequest);
                var deserializedRequest = JsonConvert.DeserializeObject<CancelRequestNotification>(json);

                // Assert
                Assert.IsNotNull(deserializedRequest);

                if (originalRequest.RequestId.IsNull)
                {
                    Assert.IsTrue(deserializedRequest.RequestId.IsNull);
                }
                else if (originalRequest.RequestId.TryGetLong(out long longVal))
                {
                    Assert.IsTrue(deserializedRequest.RequestId.TryGetLong(out long deserializedLong));
                    Assert.AreEqual(longVal, deserializedLong);
                }
                else if (originalRequest.RequestId.TryGetString(out string stringVal))
                {
                    Assert.IsTrue(deserializedRequest.RequestId.TryGetString(out string deserializedString));
                    Assert.AreEqual(stringVal, deserializedString);
                }
            }
        }

        [TestMethod]
        public void SessionConfigSelectOptions_RoundTripSerialization_WorksForUngroupedAndGrouped()
        {
            // Arrange
            var ungroupedSelect = new SessionConfigSelect
            {
                CurrentValue = "v1",
                Id = "opt1",
                Name = "Option",
                Options = new[]
                {
                    new SessionConfigSelectOption { Value = "v1", Name = "Option 1" }
                }
            };

            var groupedSelect = new SessionConfigSelect
            {
                CurrentValue = "v1",
                Id = "opt2",
                Name = "Grouped Option",
                Options = new[]
                {
                    new SessionConfigSelectGroup
                    {
                        Group = "g1",
                        Name = "Group 1",
                        Options = new[]
                        {
                            new SessionConfigSelectOption { Value = "v1", Name = "Option 1" }
                        }
                    }
                }
            };

            // Act
            var ungroupedJson = JsonConvert.SerializeObject(ungroupedSelect);
            var groupedJson = JsonConvert.SerializeObject(groupedSelect);

            var ungroupedRoundTrip = JsonConvert.DeserializeObject<SessionConfigSelect>(ungroupedJson);
            var groupedRoundTrip = JsonConvert.DeserializeObject<SessionConfigSelect>(groupedJson);

            // Assert
            Assert.IsNotNull(ungroupedRoundTrip);
            Assert.IsTrue(ungroupedRoundTrip.Options.TryGetSessionConfigSelectOption(out var ungroupedOptions));
            Assert.HasCount(1, ungroupedOptions);
            Assert.AreEqual("v1", (string)ungroupedOptions[0].Value);

            Assert.IsNotNull(groupedRoundTrip);
            Assert.IsTrue(groupedRoundTrip.Options.TryGetSessionConfigSelectGroup(out var groupedOptions));
            Assert.HasCount(1, groupedOptions);
            Assert.AreEqual("g1", (string)groupedOptions[0].Group);
        }

        #endregion
    }
}
