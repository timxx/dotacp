using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.generator;
using Newtonsoft.Json.Linq;

namespace dotacp.unittest
{
    [TestClass]
    public class ModelClassBuilderTests
    {
        private static string BuildModel(string name, JObject defs)
        {
            var resolver = new PropertyTypeResolver(defs);
            var discriminatorAnalyzer = new DiscriminatorAnalyzer(defs);
            var definition = (JObject?)defs[name];
            var builder = new ModelClassBuilder(name, definition, defs, resolver, discriminatorAnalyzer);
            return builder.Generate();
        }

        [TestMethod]
        public void DiscriminatorHandling_GeneratesAbstractBaseAndMapping()
        {
            var defs = JObject.Parse(@"{
  'TextContent': { 'type': 'object', 'properties': { 'text': { 'type': 'string' } }, 'required': ['text'] },
  'ContentBlock': {
    'discriminator': { 'propertyName': 'type' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/TextContent' } ],
        'properties': { 'type': { 'const': 'text', 'type': 'string' } },
        'required': ['type'],
        'type': 'object'
      }
    ]
  }
}");

            var result = BuildModel("ContentBlock", defs);

            StringAssert.Contains(result, "abstract class ContentBlock");
            StringAssert.Contains(result, "DiscriminatorPropertyName = \"type\"");
            StringAssert.Contains(result, "typeof(TextContent)");
        }

        [TestMethod]
        public void DiscriminatorHandling_AddsOverrideForDerivedType()
        {
            var defs = JObject.Parse(@"{
  'TextContent': { 'type': 'object', 'properties': { 'text': { 'type': 'string' } }, 'required': ['text'] },
  'ContentBlock': {
    'discriminator': { 'propertyName': 'type' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/TextContent' } ],
        'properties': { 'type': { 'const': 'text', 'type': 'string' } },
        'required': ['type'],
        'type': 'object'
      }
    ]
  }
}");

            var result = BuildModel("TextContent", defs);

            StringAssert.Contains(result, "class TextContent : ContentBlock");
            StringAssert.Contains(result, "override string Type");
            StringAssert.Contains(result, "=> \"text\"");
        }

        [TestMethod]
        public void DiscriminatorHandling_UsesWrapperVariantsWhenRefsRepeat()
        {
            var defs = JObject.Parse(@"{
  'Chunk': { 'type': 'object', 'properties': { 'content': { 'type': 'string' } }, 'required': ['content'] },
  'Update': {
    'discriminator': { 'propertyName': 'kind' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/Chunk' } ],
        'properties': { 'kind': { 'const': 'first', 'type': 'string' } },
        'required': ['kind'],
        'type': 'object'
      },
      {
        'allOf': [ { '$ref': '#/$defs/Chunk' } ],
        'properties': { 'kind': { 'const': 'second', 'type': 'string' } },
        'required': ['kind'],
        'type': 'object'
      }
    ]
  }
}");

            var updateResult = BuildModel("Update", defs);
            var chunkResult = BuildModel("Chunk", defs);

            StringAssert.Contains(updateResult, "class UpdateFirst");
            StringAssert.Contains(updateResult, "class UpdateSecond");
            StringAssert.Contains(updateResult, "override string Kind");
            Assert.DoesNotContain("class Chunk : Update", chunkResult, "Chunk should not inherit from Update when refs repeat");
        }

        [TestMethod]
        public void TypeAliasStruct_GeneratesStringAliasStruct()
        {
            var defs = JObject.Parse(@"{ 'TestId': { 'type': 'string', 'description': 'Test string alias' } }");
            var result = BuildModel("TestId", defs);

            StringAssert.Contains(result, "public readonly struct TestId");
            StringAssert.Contains(result, "IEquatable<TestId>");
            StringAssert.Contains(result, "private readonly string _value");
            StringAssert.Contains(result, "Test string alias");
        }

        [TestMethod]
        public void TypeAliasStruct_GeneratesUShortAliasStruct()
        {
            var defs = JObject.Parse(@"{ 'TestVersion': { 'type': 'integer', 'format': 'uint16', 'description': 'Test version number' } }");
            var result = BuildModel("TestVersion", defs);

            StringAssert.Contains(result, "public readonly struct TestVersion");
            StringAssert.Contains(result, "private readonly ushort _value");
            StringAssert.Contains(result, "Test version number");
        }

        [TestMethod]
        public void TypeAliasStruct_IncludesImplicitOperatorsAndOverrides()
        {
            var defs = JObject.Parse(@"{ 'TestId': { 'type': 'string' } }");
            var result = BuildModel("TestId", defs);

            StringAssert.Contains(result, "public static implicit operator TestId(string value)");
            StringAssert.Contains(result, "public static implicit operator string(TestId alias)");
            StringAssert.Contains(result, "public bool Equals(TestId other)");
            StringAssert.Contains(result, "public override bool Equals(object obj)");
            StringAssert.Contains(result, "public override int GetHashCode()");
            StringAssert.Contains(result, "public override string ToString()");
        }

        [TestMethod]
        public void TypeAliasStruct_HandlesNullForReferenceTypeHashCode()
        {
            var defs = JObject.Parse(@"{ 'TestId': { 'type': 'string' } }");
            var result = BuildModel("TestId", defs);

            StringAssert.Contains(result, "_value?.GetHashCode() ?? 0");
        }

        [TestMethod]
        public void TypeAliasStruct_UsesValueTypeHashCodeForValueTypes()
        {
            var defs = JObject.Parse(@"{ 'TestVersion': { 'type': 'integer', 'format': 'uint16' } }");
            var result = BuildModel("TestVersion", defs);

            Assert.DoesNotContain("_value?.GetHashCode", result, "Value types should not use null-conditional hash code");
            StringAssert.Contains(result, "_value.GetHashCode()");
        }

        [TestMethod]
        public void TypeAliasStruct_IncludesJsonConverterAttribute()
        {
            var defs = JObject.Parse(@"{ 'TestId': { 'type': 'string' } }");
            var result = BuildModel("TestId", defs);

            StringAssert.Contains(result, "[JsonConverter(typeof(TypeAliasConverter<TestId, string>))]");
        }

        [TestMethod]
        public void AnyOfEnumDetection_GeneratesStringEnum()
        {
            var defs = JObject.Parse(@"{
  'TestEnum': {
    'description': 'Test enum',
    'anyOf': [
      { 'type': 'string', 'const': 'value1', 'description': 'First value' },
      { 'type': 'string', 'const': 'value2', 'description': 'Second value' }
    ]
  }
}");

            var result = BuildModel("TestEnum", defs);

            StringAssert.Contains(result, "public enum TestEnum");
            StringAssert.Contains(result, "JsonEnumValue(\"value1\")");
            StringAssert.Contains(result, "JsonEnumValue(\"value2\")");
            StringAssert.Contains(result, "Value1");
            StringAssert.Contains(result, "Value2");
        }

        [TestMethod]
        public void AnyOfEnumDetection_GeneratesEnumWithTitleFallback()
        {
            var defs = JObject.Parse(@"{
  'TestCategory': {
    'description': 'Test enum with titles',
    'anyOf': [
      { 'type': 'string', 'title': 'Mode', 'const': 'mode' },
      { 'type': 'string', 'title': 'Other' }
    ]
  }
}");

            var result = BuildModel("TestCategory", defs);

            StringAssert.Contains(result, "public enum TestCategory");
            StringAssert.Contains(result, "JsonEnumValue(\"mode\")");
            StringAssert.Contains(result, "Mode");
            StringAssert.Contains(result, "Other");
        }

        [TestMethod]
        public void AnyOfEnumDetection_GeneratesIntegerEnum()
        {
            var defs = JObject.Parse(@"{
  'ErrorCode': {
    'description': 'Error codes',
    'anyOf': [
      { 'type': 'integer', 'format': 'int32', 'const': -32700, 'title': 'Parse error', 'description': 'Parse error' },
      { 'type': 'integer', 'format': 'int32', 'const': -32600, 'title': 'Invalid request', 'description': 'Invalid request' },
      { 'type': 'integer', 'format': 'int32', 'title': 'Other' }
    ]
  }
}");

            var result = BuildModel("ErrorCode", defs);

            StringAssert.Contains(result, "public enum ErrorCode");
            StringAssert.Contains(result, ": int");
            StringAssert.Contains(result, "ParseError = -32700");
            StringAssert.Contains(result, "InvalidRequest = -32600");
            StringAssert.Contains(result, "Other = 0");
        }

        [TestMethod]
        public void AnyOfEnumDetection_GeneratesLongBackedEnum()
        {
            var defs = JObject.Parse(@"{ 'LongEnum': { 'anyOf': [ { 'type': 'integer', 'format': 'int64', 'const': 9223372036854775807, 'title': 'MaxLong' } ] } }");
            var result = BuildModel("LongEnum", defs);

            StringAssert.Contains(result, "public enum LongEnum");
            StringAssert.Contains(result, ": long");
        }

        [TestMethod]
        public void AnyOfUnionDetection_GeneratesUnionStruct()
        {
            var defs = JObject.Parse(@"{
  'RequestId': {
    'description': 'Request ID',
    'anyOf': [
      { 'type': 'null', 'title': 'Null' },
      { 'type': 'integer', 'format': 'int64', 'title': 'Number' },
      { 'type': 'string', 'title': 'Str' }
    ]
  }
}");

            var result = BuildModel("RequestId", defs);

            StringAssert.Contains(result, "public readonly struct RequestId");
            StringAssert.Contains(result, "IEquatable<RequestId>");
            StringAssert.Contains(result, "UnionTypeConverter<RequestId>");
            StringAssert.Contains(result, "public RequestId(long value)");
            StringAssert.Contains(result, "public RequestId(string value)");
            StringAssert.Contains(result, "public bool IsNull");
            StringAssert.Contains(result, "public static RequestId Null");
            StringAssert.Contains(result, "public bool TryGetLong(out long value)");
            StringAssert.Contains(result, "public bool TryGetString(out string value)");
        }

        [TestMethod]
        public void AnyOfUnionDetection_DoesNotIncludeNullFlagWhenMissing()
        {
            var defs = JObject.Parse(@"{ 'SimpleUnion': { 'anyOf': [ { 'type': 'integer', 'format': 'int32' }, { 'type': 'string' } ] } }");
            var result = BuildModel("SimpleUnion", defs);

            Assert.DoesNotContain("_isNull", result, "Union should not include null flag when null is not present");
            Assert.DoesNotContain("IsNull", result, "Union should not include IsNull when null is not present");
        }

        [TestMethod]
        public void AnyOfVsDiscriminatedUnion_GeneratesClassWhenAnyOfHasProperties()
        {
            var defs = JObject.Parse(@"{
  'Content': {
    'anyOf': [
      {
        'type': 'object',
        'properties': {
          'type': { 'type': 'string', 'const': 'text' },
          'text': { 'type': 'string' }
        },
        'required': ['type', 'text']
      },
      {
        'type': 'object',
        'properties': {
          'type': { 'type': 'string', 'const': 'image' },
          'data': { 'type': 'string' }
        },
        'required': ['type', 'data']
      }
    ]
  }
}");

            var result = BuildModel("Content", defs);

            StringAssert.Contains(result, "public class Content");
            Assert.DoesNotContain("public enum", result, "Should not generate enum for discriminated union class");
            Assert.DoesNotContain("UnionTypeConverter", result, "Should not generate union struct for discriminated union class");
        }

        [TestMethod]
        public void SimpleEnum_GeneratesStringEnum()
        {
            var defs = JObject.Parse(@"{
  'Role': {
    'description': 'The sender or recipient of messages and data in a conversation.',
    'type': 'string',
    'enum': ['assistant', 'user']
  }
}");

            var result = BuildModel("Role", defs);

            StringAssert.Contains(result, "public enum Role");
            StringAssert.Contains(result, "JsonEnumMemberConverter<Role>");
            StringAssert.Contains(result, "JsonEnumValue(\"assistant\")");
            StringAssert.Contains(result, "JsonEnumValue(\"user\")");
            StringAssert.Contains(result, "Assistant");
            StringAssert.Contains(result, "User");
            StringAssert.Contains(result, "The sender or recipient");
        }

        [TestMethod]
        public void SimpleEnum_GeneratesIntegerEnum()
        {
            var defs = JObject.Parse(@"{
  'HttpStatus': {
    'description': 'HTTP status codes',
    'type': 'integer',
    'format': 'uint32',
    'enum': [200, 404, 500]
  }
}");

            var result = BuildModel("HttpStatus", defs);

            StringAssert.Contains(result, "public enum HttpStatus");
            StringAssert.Contains(result, ": uint");
            StringAssert.Contains(result, "200");
            StringAssert.Contains(result, "404");
            StringAssert.Contains(result, "500");
        }
    }
}
