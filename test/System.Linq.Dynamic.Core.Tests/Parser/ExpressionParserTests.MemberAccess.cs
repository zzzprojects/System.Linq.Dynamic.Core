using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    partial class ExpressionParserTests
    {
        [Fact]
        public void ParseMemberAccess_DynamicIndex()
        {
            // Arrange
            var products = (new ProductDynamic[0]).AsQueryable();

            // Act
            var expression = products.Where("Properties.Name == @0", "First Product").Expression;

            // Assert
            expression.ToString().Should().Be("System.Linq.Dynamic.Core.Tests.Parser.ProductDynamic[].Where(Param_0 => (DynamicIndex(Param_0.Properties, \"Name\") == Convert(\"First Product\", Object)))");
        }
    }

    public class ProductDynamic
    {
        public virtual string ProductId { get; set; }

        public virtual dynamic Properties { get; set; }
    }
}
