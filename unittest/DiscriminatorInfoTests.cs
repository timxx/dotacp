using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace dotacp.unittest
{
    [TestClass]
    public class DiscriminatorInfoTests
    {
        [TestMethod]
        public void DiscriminatorBaseInfo_InitializesWithDefaults()
        {
            var info = new DiscriminatorBaseInfo();

            Assert.AreEqual("", info.PropertyName);
            Assert.AreEqual("", info.PropertyCsName);
            Assert.AreEqual("", info.PropertyJsonName);
            Assert.IsNotNull(info.Mapping);
            Assert.IsEmpty(info.Mapping);
        }

        [TestMethod]
        public void DiscriminatorBaseInfo_CanSetProperties()
        {
            var info = new DiscriminatorBaseInfo
            {
                PropertyName = "type",
                PropertyCsName = "Type",
                PropertyJsonName = "type"
            };

            Assert.AreEqual("type", info.PropertyName);
            Assert.AreEqual("Type", info.PropertyCsName);
            Assert.AreEqual("type", info.PropertyJsonName);
        }

        [TestMethod]
        public void DiscriminatorBaseInfo_CanPopulateMapping()
        {
            var info = new DiscriminatorBaseInfo
            {
                PropertyName = "type",
                PropertyCsName = "Type",
                PropertyJsonName = "type"
            };

            info.Mapping["text"] = "TextContent";
            info.Mapping["code"] = "CodeContent";

            Assert.HasCount(2, info.Mapping);
            Assert.AreEqual("TextContent", info.Mapping["text"]);
            Assert.AreEqual("CodeContent", info.Mapping["code"]);
        }

        [TestMethod]
        public void DiscriminatorDerivedInfo_InitializesWithDefaults()
        {
            var info = new DiscriminatorDerivedInfo();

            Assert.AreEqual("", info.BaseName);
            Assert.AreEqual("", info.PropertyName);
            Assert.AreEqual("", info.PropertyCsName);
            Assert.AreEqual("", info.PropertyJsonName);
            Assert.AreEqual("", info.DiscriminatorValue);
            Assert.IsFalse(info.IsAbstract);
        }

        [TestMethod]
        public void DiscriminatorDerivedInfo_CanSetProperties()
        {
            var info = new DiscriminatorDerivedInfo
            {
                BaseName = "ContentBlock",
                PropertyName = "type",
                PropertyCsName = "Type",
                PropertyJsonName = "type",
                DiscriminatorValue = "text",
                IsAbstract = false
            };

            Assert.AreEqual("ContentBlock", info.BaseName);
            Assert.AreEqual("Type", info.PropertyCsName);
            Assert.AreEqual("text", info.DiscriminatorValue);
            Assert.IsFalse(info.IsAbstract);
        }

        [TestMethod]
        public void DiscriminatorDerivedInfo_CanMarkAsAbstract()
        {
            var info = new DiscriminatorDerivedInfo
            {
                IsAbstract = true
            };

            Assert.IsTrue(info.IsAbstract);
        }

        [TestMethod]
        public void DiscriminatorVariant_InitializesWithDefaults()
        {
            var variant = new DiscriminatorVariant();

            Assert.AreEqual("", variant.ClassName);
            Assert.AreEqual("", variant.BaseClassName);
            Assert.AreEqual("", variant.DiscriminatorPropertyName);
            Assert.AreEqual("", variant.DiscriminatorPropertyCsName);
            Assert.AreEqual("", variant.DiscriminatorPropertyJsonName);
            Assert.AreEqual("", variant.DiscriminatorValue);
            Assert.IsNotNull(variant.Definition);
            Assert.IsNull(variant.Description);
        }

        [TestMethod]
        public void DiscriminatorVariant_CanSetProperties()
        {
            var variant = new DiscriminatorVariant
            {
                ClassName = "TextContent",
                BaseClassName = "ContentBlock",
                DiscriminatorPropertyName = "type",
                DiscriminatorPropertyCsName = "Type",
                DiscriminatorPropertyJsonName = "type",
                DiscriminatorValue = "text",
                Description = "Text content variant"
            };

            Assert.AreEqual("TextContent", variant.ClassName);
            Assert.AreEqual("ContentBlock", variant.BaseClassName);
            Assert.AreEqual("text", variant.DiscriminatorValue);
            Assert.AreEqual("Text content variant", variant.Description);
        }

        [TestMethod]
        public void DiscriminatorVariant_CanSetDefinition()
        {
            var variantDef = JObject.Parse(@"{ 'type': 'object', 'properties': { 'text': { 'type': 'string' } } }");
            var variant = new DiscriminatorVariant
            {
                Definition = variantDef
            };

            Assert.IsNotNull(variant.Definition);
            Assert.IsTrue(variant.Definition.ContainsKey("properties"));
        }
    }
}
