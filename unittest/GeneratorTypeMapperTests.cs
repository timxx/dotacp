using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.generator;

namespace dotacp.unittest
{
    [TestClass]
    public class TypeMapperTests
    {
        [TestMethod]
        [DataRow("string", "string")]
        [DataRow("number", "double")]
        [DataRow("integer", "int")]
        [DataRow("boolean", "bool")]
        [DataRow("object", "object")]
        [DataRow("array", "List<object>")]
        [DataRow("null", "object")]
        public void GetTypeName_MapsJsonTypesToCSharpTypes(string jsonType, string expected)
        {
            var result = TypeMapper.GetTypeName(jsonType);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetTypeName_UnknownTypesReturnObject()
        {
            var result = TypeMapper.GetTypeName("unknown");
            Assert.AreEqual("object", result);
        }

        [TestMethod]
        [DataRow(true, "true")]
        [DataRow(false, "false")]
        public void ConvertDefaultValue_ConvertsBooleanDefaults(bool value, string expected)
        {
            var result = TypeMapper.ConvertDefaultValue(value, "bool?");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(42, "int", "42")]
        [DataRow(100, "int", "100")]
        public void ConvertDefaultValue_ConvertsIntegerDefaults(int value, string type, string expected)
        {
            var result = TypeMapper.ConvertDefaultValue(value, type);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(1.5, "double", "1.5")]
        [DataRow(0.0, "double", "0")]
        public void ConvertDefaultValue_ConvertsDoubleDefaults(double value, string type, string expected)
        {
            var result = TypeMapper.ConvertDefaultValue(value, type);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("hello", "string", "\"hello\"")]
        [DataRow("test", "string", "\"test\"")]
        public void ConvertDefaultValue_ConvertsStringDefaults(string value, string type, string expected)
        {
            var result = TypeMapper.ConvertDefaultValue(value, type);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertDefaultValue_ReturnsNullForNullValue()
        {
            var result = TypeMapper.ConvertDefaultValue(null, "bool?");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertDefaultValue_ReturnsNullForNullList()
        {
            var result = TypeMapper.ConvertDefaultValue(null, "List<string>");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertDefaultValue_ReturnsNullForNullDictionary()
        {
            var result = TypeMapper.ConvertDefaultValue(null, "Dictionary<string, object>");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertDefaultValue_CreatesNewListForNonNullList()
        {
            var result = TypeMapper.ConvertDefaultValue("dummy", "List<string>");
            Assert.AreEqual("new List<string>()", result);
        }

        [TestMethod]
        public void IsValueType_IdentifiesValueTypes()
        {
            Assert.IsTrue(TypeMapper.IsValueType("int"), "int should be a value type");
            Assert.IsTrue(TypeMapper.IsValueType("bool"), "bool should be a value type");
            Assert.IsTrue(TypeMapper.IsValueType("double"), "double should be a value type");
            Assert.IsTrue(TypeMapper.IsValueType("long"), "long should be a value type");
        }

        [TestMethod]
        public void IsValueType_IdentifiesReferenceTypes()
        {
            Assert.IsFalse(TypeMapper.IsValueType("string"), "string should be a reference type");
            Assert.IsFalse(TypeMapper.IsValueType("object"), "object should be a reference type");
            Assert.IsFalse(TypeMapper.IsValueType("List<string>"), "List<string> should be a reference type");
        }

        [TestMethod]
        public void IsReferenceType_IdentifiesReferenceTypes()
        {
            Assert.IsTrue(TypeMapper.IsReferenceType("string"), "string should be a reference type");
            Assert.IsTrue(TypeMapper.IsReferenceType("object"), "object should be a reference type");
            Assert.IsTrue(TypeMapper.IsReferenceType("List<string>"), "List<string> should be a reference type");
        }

        [TestMethod]
        public void IsReferenceType_IdentifiesValueTypes()
        {
            Assert.IsFalse(TypeMapper.IsReferenceType("int"), "int should be a value type");
            Assert.IsFalse(TypeMapper.IsReferenceType("bool"), "bool should be a value type");
        }
    }
}
