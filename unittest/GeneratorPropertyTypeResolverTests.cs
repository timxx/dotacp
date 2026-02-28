using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.generator;
using Newtonsoft.Json.Linq;

namespace dotacp.unittest
{
    [TestClass]
    public class PropertyTypeResolverTests
    {
        private static PropertyTypeResolver CreateResolver()
        {
            return new PropertyTypeResolver(new JObject());
        }

        [TestMethod]
        public void GetPropertyType_ExtractsClassNameFromRef()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ '$ref': '#/$defs/MyClass' }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("MyClass", result);
        }

        [TestMethod]
        public void GetPropertyType_WrapsArrayItemsInArray()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'type': 'array', 'items': { 'type': 'string' } }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("string[]", result);
        }

        [TestMethod]
        public void GetPropertyType_DefaultsToObjectArrayForArraysWithoutItems()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'type': 'array' }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("object[]", result);
        }

        [TestMethod]
        public void GetPropertyType_DoesNotAddNullableToString()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'type': ['string', 'null'] }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("string", result);
        }

        [TestMethod]
        public void GetPropertyType_ReturnsStringForEnumType()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'type': 'string', 'enum': ['value1', 'value2'] }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("string", result);
        }

        [TestMethod]
        public void GetPropertyType_ConvertsPrimitiveTypes()
        {
            var resolver = CreateResolver();

            Assert.AreEqual("string", resolver.GetPropertyType(JObject.Parse(@"{ 'type': 'string' }")));
            Assert.AreEqual("int", resolver.GetPropertyType(JObject.Parse(@"{ 'type': 'integer' }")));
            Assert.AreEqual("bool", resolver.GetPropertyType(JObject.Parse(@"{ 'type': 'boolean' }")));
        }

        [TestMethod]
        public void GetPropertyType_ReturnsObjectForAnyOfUnion()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'anyOf': [ { 'type': 'string' }, { 'type': 'number' } ] }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("object", result);
        }

        [TestMethod]
        public void GetPropertyType_ReturnsObjectForOneOf()
        {
            var resolver = CreateResolver();
            var property = JObject.Parse(@"{ 'oneOf': [ { 'type': 'string' }, { 'type': 'number' } ] }");

            var result = resolver.GetPropertyType(property);
            Assert.AreEqual("object", result);
        }

        [TestMethod]
        public void GetPropertyType_HandlesNullableValueTypeFormats()
        {
            var resolver = CreateResolver();

            Assert.AreEqual("uint?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'uint32' }")));
            Assert.AreEqual("ulong?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'uint64' }")));
            Assert.AreEqual("ushort?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'uint16' }")));
            Assert.AreEqual("int?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'int32' }")));
            Assert.AreEqual("long?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'int64' }")));
            Assert.AreEqual("short?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'], 'format': 'int16' }")));
            Assert.AreEqual("int?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['integer', 'null'] }")));
            Assert.AreEqual("bool?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['boolean', 'null'] }")));
            Assert.AreEqual("double?", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['number', 'null'] }")));
        }

        [TestMethod]
        public void GetPropertyType_HandlesNullableReferenceTypes()
        {
            var resolver = CreateResolver();

            Assert.AreEqual("string", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['string', 'null'] }")));
            Assert.AreEqual("object", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['object', 'null'] }")));
            Assert.AreEqual("object[]", resolver.GetPropertyType(JObject.Parse(@"{ 'type': ['array', 'null'] }")));
        }

        [TestMethod]
        public void GetPropertyType_HandlesAnyOfRefPlusNullPatterns()
        {
            var resolver = CreateResolver();

            Assert.AreEqual("Annotations", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { '$ref': '#/$defs/Annotations' }, { 'type': 'null' } ] }")));
            Assert.AreEqual("Implementation", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { 'type': 'null' }, { '$ref': '#/$defs/Implementation' } ] }")));
            Assert.AreEqual("SessionModeState", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { '$ref': '#/$defs/SessionModeState' }, { 'type': 'null' } ] }")));
            Assert.AreEqual("TerminalExitStatus", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { '$ref': '#/$defs/TerminalExitStatus' }, { 'type': 'null' } ] }")));
        }

        [TestMethod]
        public void GetPropertyType_HandlesComplexAnyOfPatterns()
        {
            var resolver = CreateResolver();

            Assert.AreEqual("object", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { '$ref': '#/$defs/TypeA' }, { '$ref': '#/$defs/TypeB' }, { '$ref': '#/$defs/TypeC' } ] }")));
            Assert.AreEqual("object", resolver.GetPropertyType(JObject.Parse(@"{ 'anyOf': [ { 'allOf': [ { '$ref': '#/$defs/ResponseA' } ] }, { 'allOf': [ { '$ref': '#/$defs/ResponseB' } ] } ] }")));
        }
    }
}
