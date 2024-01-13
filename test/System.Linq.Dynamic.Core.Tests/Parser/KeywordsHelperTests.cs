using Moq;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    [DynamicLinqType]
    public class TestClass
    {
        public string Hello { get; set; }
    }

    public class KeywordsHelperTests
    {
        public KeywordsHelperTests()
        {
        }

        private KeywordsHelper CreateKeywordsHelper(ParsingConfig config)
        {
            return new KeywordsHelper(config);
        }

        [Theory]
        [InlineData("it",true)]
        [InlineData("IT", false)]
        [InlineData("TestClass",true)]
        [InlineData("testClass", false)]
        [InlineData("nonExisting", false)]
        public void TryGetValue_WithCaseSensitive_ReturnsResultAsExpected(string name, bool expected)
        {
            // Arrange
            var keywordsHelper = this.CreateKeywordsHelper( new ParsingConfig { AreKeywordsCaseSensitive = true});
            object type = null;

            // Act
            var result = keywordsHelper.TryGetValue(
                name,
                out type);

            // Assert
            Assert.Equal(expected,result);
        }

        [Theory]
        [InlineData("it", true)]
        [InlineData("IT", true)]
        [InlineData("TestClass", true)]
        [InlineData("testClass", false)]
        [InlineData("nonExisting", false)]
        public void TryGetValue_WithCaseInSensitive_ReturnsResultAsExpected(string name, bool expected)
        {
            // Arrange
            var keywordsHelper = this.CreateKeywordsHelper(new ParsingConfig { AreKeywordsCaseSensitive = false });
            object type;

            // Act
            var result = keywordsHelper.TryGetValue(name,out type);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
