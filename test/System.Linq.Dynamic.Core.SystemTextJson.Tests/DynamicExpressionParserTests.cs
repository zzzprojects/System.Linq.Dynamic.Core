using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;
using FluentAssertions;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests;

[ExcludeFromCodeCoverage]
public class DynamicExpressionParserTests
{
    private const string ExampleJson =
        """
        [
            {
                "Name": "John",
                "Age": 30
            },
            {
                "Name": "Doe",
                "Age": 40
            }
        ]
        """;
    private readonly JsonDocument _source = JsonDocument.Parse(ExampleJson);

    //[Fact]
    // This is not supported yet...
    public void X()
    {
        // Act
        var qry = _source.RootElement.EnumerateArray();
        var parameters = new[] { Expression.Parameter(_source.GetType(), "y") };

        // Assert
        var lambdaExpression = DynamicExpressionParser.ParseLambda(parameters, null, "y => y.Name");
        var @delegate = lambdaExpression.Compile();
        var result = @delegate.DynamicInvoke(qry) as IEnumerable<string>;

        result.Should().BeEquivalentTo(new[] { "John", "Doe" });
    }
}