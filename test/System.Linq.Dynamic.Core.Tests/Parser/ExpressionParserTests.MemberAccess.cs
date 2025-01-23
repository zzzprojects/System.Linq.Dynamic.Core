using System.Linq.Dynamic.Core.CustomTypeProviders;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    partial class ExpressionParserTests
    {
        [Fact]
        public void ParseMemberAccess_DictionaryIndex_On_Dynamic()
        {
            // Arrange
            var products = new ProductDynamic[0].AsQueryable();

            // Act
            var expression = products.Where("Properties.Name == @0", "First Product").Expression;

            // Assert
#if NET461 || NET48
            expression.ToString().Should().Be("System.Linq.Dynamic.Core.Tests.Parser.ProductDynamic[].Where(Param_0 => (GetMember Name(Param_0.Properties) == Convert(\"First Product\")))");
#else
            expression.ToString().Should().Be("System.Linq.Dynamic.Core.Tests.Parser.ProductDynamic[].Where(Param_0 => ([Dynamic] == Convert(\"First Product\", Object)))");
#endif
        }

        [Theory]
        [InlineData("Prop", "TestProp")]
        [InlineData("Field", "TestField")]
        public void Parse_StaticPropertyOrField_In_StaticClass1(string name, string value)
        {
            // Arrange
            var queryable = new int[1].AsQueryable();

            // Act
            var result = queryable.Select<string>($"{typeof(StaticClassExample)}.{name}").First();

            // Assert
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData("Prop", "TestProp")]
        [InlineData("Field", "TestField")]
        public void Parse_StaticPropertyOrField_In_NonStaticClass1(string name, string value)
        {
            // Arrange
            var queryable = new int[1].AsQueryable();

            // Act
            var result = queryable.Select<string>($"new {typeof(NonStaticClassExample)}().{name}").First();

            // Assert
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData("Prop", "TestProp")]
        [InlineData("Field", "TestField")]
        public void Parse_StaticPropertyOrField_In_NonStaticClass2(string name, string value)
        {
            // Arrange
            var queryable = new[] { new NonStaticClassExample() }.AsQueryable();

            // Act
            var result = queryable.Select<string>(name).First();

            // Assert
            Assert.Equal(value, result);
        }
    }

    [DynamicLinqType]
    public class StaticClassExample
    {
        public static string Prop { get; set; } = "TestProp";

        public static string Field = "TestField";
    }

    [DynamicLinqType]
    public class NonStaticClassExample
    {
        public static string Prop { get; set; } = "TestProp";

        public static string Field = "TestField";
    }

    public class ProductDynamic
    {
        public string ProductId { get; set; }

        public dynamic Properties { get; set; }
    }
}