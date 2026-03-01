using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace dotacp.unittest
{
    [TestClass]
    public class StringBuilderExtTests
    {
        [TestMethod]
        public void AppendLineLf_AppendsValueAndLineFeed()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("Hello");

            var result = sb.ToString();
            Assert.AreEqual("Hello\n", result);
        }

        [TestMethod]
        public void AppendLineLf_WithNullValue_AppendsOnlyLineFeed()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf(null);

            var result = sb.ToString();
            Assert.AreEqual("\n", result);
        }

        [TestMethod]
        public void AppendLineLf_WithEmptyString_AppendsOnlyLineFeed()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("");

            var result = sb.ToString();
            Assert.AreEqual("\n", result);
        }

        [TestMethod]
        public void AppendLineLf_MultipleCallsAppendCorrectly()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("Line1");
            sb.AppendLineLf("Line2");
            sb.AppendLineLf("Line3");

            var result = sb.ToString();
            Assert.AreEqual("Line1\nLine2\nLine3\n", result);
        }

        [TestMethod]
        public void AppendLineLf_ReturnsSelfForChaining()
        {
            var sb = new StringBuilder();
            var returned = sb.AppendLineLf("Test");

            Assert.AreSame(sb, returned, "Should return the same StringBuilder instance for method chaining");
        }

        [TestMethod]
        public void AppendLineLf_PreservesExistingContent()
        {
            var sb = new StringBuilder();
            sb.Append("Existing");
            sb.AppendLineLf("New");

            var result = sb.ToString();
            Assert.AreEqual("ExistingNew\n", result);
        }

        [TestMethod]
        public void AppendLineLf_WithMultilineString_AppendsFullString()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("Line with\nmultiple\nlines");

            var result = sb.ToString();
            Assert.AreEqual("Line with\nmultiple\nlines\n", result);
        }

        [TestMethod]
        public void AppendLineLf_WithWhitespace_PreservesWhitespace()
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("  spaced  ");

            var result = sb.ToString();
            Assert.AreEqual("  spaced  \n", result);
        }
    }
}
