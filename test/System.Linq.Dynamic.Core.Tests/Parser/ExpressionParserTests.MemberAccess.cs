using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    partial class ExpressionParserTests
    {
        [Fact]
        public void ParseMemberAccess_DictionaryIndex()
        {
            // Arrange
            var products = (new ProductDynamic[0]).AsQueryable();

            // Act
            var expression = products.Where("Properties.Name == @0", "First Product").Expression;

            // Assert
#if NET452 || NET461
            expression.ToString().Should().Be("System.Linq.Dynamic.Core.Tests.Parser.ProductDynamic[].Where(Param_0 => (Convert(Param_0.Properties).Item[\"Name\"] == Convert(\"First Product\")))");
#else
            expression.ToString().Should().Be("System.Linq.Dynamic.Core.Tests.Parser.ProductDynamic[].Where(Param_0 => (Convert(Param_0.Properties, IDictionary`2).Item[\"Name\"] == Convert(\"First Product\", Object)))");
#endif
        }
    }

    public class ProductDynamic
    {
        public string ProductId { get; set; }

        public dynamic Properties { get; set; }
    }
}
