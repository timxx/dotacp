using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [DataRow("array", "object[]")]
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
        public void ConvertDefaultValue_ReturnsNullForNullArray()
        {
            var result = TypeMapper.ConvertDefaultValue(null, "string[]");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertDefaultValue_ReturnsNullForNullDictionary()
        {
            var result = TypeMapper.ConvertDefaultValue(null, "Dictionary<string, object>");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertDefaultValue_CreatesNewArrayForNonNullArray()
        {
            var result = TypeMapper.ConvertDefaultValue("dummy", "string[]");
            Assert.AreEqual("new string[0]", result);
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
            Assert.IsFalse(TypeMapper.IsValueType("string[]"), "string[] should be a reference type");
        }

        [TestMethod]
        public void IsReferenceType_IdentifiesReferenceTypes()
        {
            Assert.IsTrue(TypeMapper.IsReferenceType("string"), "string should be a reference type");
            Assert.IsTrue(TypeMapper.IsReferenceType("object"), "object should be a reference type");
            Assert.IsTrue(TypeMapper.IsReferenceType("string[]"), "string[] should be a reference type");
        }

        [TestMethod]
        public void IsReferenceType_IdentifiesValueTypes()
        {
            Assert.IsFalse(TypeMapper.IsReferenceType("int"), "int should be a value type");
            Assert.IsFalse(TypeMapper.IsReferenceType("bool"), "bool should be a value type");
        }
        [TestMethod]
        public void ConvertDefaultValue_HandlesJTokenValues()
        {
            var jValue = new Newtonsoft.Json.Linq.JValue(true);
            var result = TypeMapper.ConvertDefaultValue(jValue, "bool");
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesLongIntegerConversion()
        {
            long longValue = 100L;
            var result = TypeMapper.ConvertDefaultValue(longValue, "int");
            Assert.AreEqual("100", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesStringNumberConversion()
        {
            var result = TypeMapper.ConvertDefaultValue("42", "int");
            Assert.AreEqual("42", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_CreatesNewDictionary()
        {
            var result = TypeMapper.ConvertDefaultValue("dummy", "Dictionary<string, int>");
            Assert.AreEqual("new Dictionary<string, int>()", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesComplexDictionaryTypes()
        {
            var result = TypeMapper.ConvertDefaultValue("val", "Dictionary<string, object>");
            Assert.AreEqual("new Dictionary<string, object>()", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesIntArrayCreation()
        {
            var result = TypeMapper.ConvertDefaultValue("dummy", "int[]");
            Assert.AreEqual("new int[0]", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesObjectArrayCreation()
        {
            var result = TypeMapper.ConvertDefaultValue("val", "object[]");
            Assert.AreEqual("new object[0]", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesDoubleWithGFormat()
        {
            double value = 1.23456789;
            var result = TypeMapper.ConvertDefaultValue(value, "double");
            Assert.IsNotNull(result);
            Assert.IsTrue(double.TryParse(result, out _), "Result should be parseable as double");
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesBoolStringParsing()
        {
            var result = TypeMapper.ConvertDefaultValue("true", "bool");
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesBoolFalseParsing()
        {
            var result = TypeMapper.ConvertDefaultValue("false", "bool?");
            Assert.AreEqual("false", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesStringType()
        {
            var result = TypeMapper.ConvertDefaultValue("hello", "string");
            Assert.AreEqual("\"hello\"", result);
        }

        [TestMethod]
        public void ConvertDefaultValue_HandlesEmptyString()
        {
            var result = TypeMapper.ConvertDefaultValue("", "string");
            Assert.AreEqual("\"\"", result);
        }
    }
}
