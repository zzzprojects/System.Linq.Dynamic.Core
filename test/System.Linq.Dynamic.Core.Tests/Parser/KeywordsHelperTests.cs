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
        [InlineData("it", false, true)]
        [InlineData("IT", false, true)]
        [InlineData("TestClass", false, true)]
        [InlineData("testClass", false, false)]
        [InlineData("nonExisting", false, false)]
        [InlineData("it", true, true)]
        [InlineData("IT", true, false)]
        [InlineData("TestClass", true, true)]
        [InlineData("testClass", true, false)]
        [InlineData("nonExisting", true ,false)]
        public void TryGetValue_WithCaseSensitiveSettings_ReturnsResultAsExpected(string name, bool areKeywordsCaseSensitive,bool expected)
        {
            // Arrange
            var keywordsHelper = this.CreateKeywordsHelper(new ParsingConfig { AreKeywordsCaseSensitive = areKeywordsCaseSensitive });
            object type;

            // Act
            var result = keywordsHelper.TryGetValue(name,out type);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
