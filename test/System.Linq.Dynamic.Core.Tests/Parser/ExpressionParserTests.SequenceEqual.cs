using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

partial class ExpressionParserTests
{
    [Fact]
    public void Parse_SequenceEqual()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Entity), "Entity");

        var parser = new ExpressionParser(
            [parameter],
            "Entity.ArrayA.SequenceEqual(Entity.ArrayB)",
            null,
            null);

        // Act
        parser.Parse(typeof(bool));
    }

    public class Entity
    {
        public string[] ArrayA { get; set; } = [];

        public string[] ArrayB { get; set; } = [];
    }
}