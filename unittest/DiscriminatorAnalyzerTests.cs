using dotacp.generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace dotacp.unittest
{
    [TestClass]
    public class DiscriminatorAnalyzerTests
    {
        [TestMethod]
        public void DiscriminatorAnalyzer_IdentifiesSimpleDiscriminator()
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

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.IsTrue(analyzer.BaseInfo.ContainsKey("ContentBlock"), "ContentBlock should be identified as a discriminator base");
            Assert.HasCount(1, analyzer.BaseInfo["ContentBlock"].Mapping);
            Assert.AreEqual("TextContent", analyzer.BaseInfo["ContentBlock"].Mapping["text"]);
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_IdentifiesDerivedTypes()
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

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.IsTrue(analyzer.DerivedInfo.ContainsKey("TextContent"), "TextContent should be identified as derived");
            Assert.AreEqual("ContentBlock", analyzer.DerivedInfo["TextContent"].BaseName);
            Assert.AreEqual("text", analyzer.DerivedInfo["TextContent"].DiscriminatorValue);
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_HandlesMultipleVariants()
        {
            var defs = JObject.Parse(@"{
  'TextContent': { 'type': 'object', 'properties': { 'text': { 'type': 'string' } } },
  'CodeContent': { 'type': 'object', 'properties': { 'code': { 'type': 'string' } } },
  'ContentBlock': {
    'discriminator': { 'propertyName': 'type' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/TextContent' } ],
        'properties': { 'type': { 'const': 'text', 'type': 'string' } }
      },
      {
        'allOf': [ { '$ref': '#/$defs/CodeContent' } ],
        'properties': { 'type': { 'const': 'code', 'type': 'string' } }
      }
    ]
  }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.HasCount(2, analyzer.BaseInfo["ContentBlock"].Mapping);
            Assert.AreEqual("TextContent", analyzer.BaseInfo["ContentBlock"].Mapping["text"]);
            Assert.AreEqual("CodeContent", analyzer.BaseInfo["ContentBlock"].Mapping["code"]);
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_IdentifiesVariantClasses()
        {
            var defs = JObject.Parse(@"{
  'Chunk': { 'type': 'object', 'properties': { 'content': { 'type': 'string' } } },
  'Update': {
    'discriminator': { 'propertyName': 'kind' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/Chunk' } ],
        'properties': { 'kind': { 'const': 'first', 'type': 'string' } }
      },
      {
        'allOf': [ { '$ref': '#/$defs/Chunk' } ],
        'properties': { 'kind': { 'const': 'second', 'type': 'string' } }
      }
    ]
  }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.IsTrue(analyzer.VariantClasses.ContainsKey("Update"), "Update should have variant classes");
            Assert.HasCount(2, analyzer.VariantClasses["Update"], "Should have 2 variants");
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_ExtractsPropertyName()
        {
            var defs = JObject.Parse(@"{
  'TextContent': { 'type': 'object', 'properties': { 'text': { 'type': 'string' } } },
  'ContentBlock': {
    'discriminator': { 'propertyName': 'block_type' },
    'oneOf': [
      {
        'allOf': [ { '$ref': '#/$defs/TextContent' } ],
        'properties': { 'block_type': { 'const': 'text', 'type': 'string' } }
      }
    ]
  }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.AreEqual("block_type", analyzer.BaseInfo["ContentBlock"].PropertyName);
            Assert.AreEqual("BlockType", analyzer.BaseInfo["ContentBlock"].PropertyCsName);
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_HandlesNoDiscriminators()
        {
            var defs = JObject.Parse(@"{
  'SimpleClass': { 'type': 'object', 'properties': { 'name': { 'type': 'string' } } }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.IsEmpty(analyzer.BaseInfo);
            Assert.IsEmpty(analyzer.DerivedInfo);
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_IdentifiesAbstractBases()
        {
            var defs = JObject.Parse(@"{
  'ResponseA': { 'type': 'object', 'properties': { 'a': { 'type': 'string' } } },
  'ResponseB': { 'type': 'object', 'properties': { 'b': { 'type': 'string' } } },
  'Response': {
    'anyOf': [
      { 'allOf': [ { '$ref': '#/$defs/ResponseA' } ] },
      { 'allOf': [ { '$ref': '#/$defs/ResponseB' } ] }
    ]
  }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            // Response should be identified as an abstract base
            Assert.Contains("Response", analyzer.AbstractBases, "Response should be identified as abstract base");
        }

        [TestMethod]
        public void DiscriminatorAnalyzer_PopulatesChildToAbstractBaseMapping()
        {
            var defs = JObject.Parse(@"{
  'ResponseA': { 'type': 'object', 'properties': { 'a': { 'type': 'string' } } },
  'ResponseB': { 'type': 'object', 'properties': { 'b': { 'type': 'string' } } },
  'Response': {
    'anyOf': [
      { 'allOf': [ { '$ref': '#/$defs/ResponseA' } ] },
      { 'allOf': [ { '$ref': '#/$defs/ResponseB' } ] }
    ]
  }
}");

            var analyzer = new DiscriminatorAnalyzer(defs);

            Assert.IsTrue(analyzer.ChildToAbstractBase.ContainsKey("ResponseA"));
            Assert.AreEqual("Response", analyzer.ChildToAbstractBase["ResponseA"]);
        }
    }
}
