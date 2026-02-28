using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotacp.generator;

namespace dotacp.unittest
{
    [TestClass]
    public class NamingHelperTests
    {
        [TestMethod]
        [DataRow("load_session", "LoadSession")]
        [DataRow("read_text_file", "ReadTextFile")]
        [DataRow("_meta", "Meta")]
        [DataRow("name", "Name")]
        [DataRow("data_id", "DataId")]
        [DataRow("_property", "Property")]
        [DataRow("a", "A")]
        [DataRow("userID", "UserID")]
        public void ConvertPropertyName_ConvertsSnakeCaseToPascalCase(string input, string expected)
        {
            var result = NamingHelper.ConvertPropertyName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("Parse error", "ParseError")]
        [DataRow("Invalid request", "InvalidRequest")]
        [DataRow("parse error", "ParseError")]
        [DataRow("Internal error", "InternalError")]
        public void ConvertPropertyName_HandlesMultiwordTitlesWithSpaces(string input, string expected)
        {
            var result = NamingHelper.ConvertPropertyName(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("content", "Content")]
        [DataRow("Content", "Content")]
        [DataRow("a", "A")]
        [DataRow("data", "Data")]
        public void ConvertNameToClass_CapitalizesFirstLetter(string input, string expected)
        {
            var result = NamingHelper.ConvertNameToClass(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("", "")]
        public void ConvertNameToClass_HandlesEmptyString(string input, string expected)
        {
            var result = NamingHelper.ConvertNameToClass(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertPropertyName_RemovesInvalidCharacters()
        {
            var result = NamingHelper.ConvertPropertyName("invalid-name");
            Assert.AreEqual("Invalidname", result);
        }

        [TestMethod]
        public void ConvertPropertyName_HandlesNamesStartingWithDigits()
        {
            var result = NamingHelper.ConvertPropertyName("123abc");
            Assert.StartsWith("_", result, "Names starting with digits should be prefixed with underscore");
        }

        [TestMethod]
        public void CaseSensitiveComparison_DetectsDifferences()
        {
            var name1 = "test";
            var name2 = "TEST";

            Assert.IsTrue(string.Equals(name1, name2, System.StringComparison.OrdinalIgnoreCase), "OrdinalIgnoreCase comparison should be equal");
            Assert.IsFalse(string.Equals(name1, name2, System.StringComparison.Ordinal), "Ordinal comparison should be different");
        }
    }
}
