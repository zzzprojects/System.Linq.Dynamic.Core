using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Tests;

public class NewtonsoftJsonTests
{
    private const string ExampleJson = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 40
            }
        ]";
    private readonly JArray _source = JArray.Parse(ExampleJson);

    [Fact]
    public void Aggregate()
    {
        // Act
        var result = _source.Aggregate("Sum", "Age");

        // Assert
        result.Should().Be(70);
    }

    [Fact]
    public void All()
    {
        // Act
        var result = _source.All("Age > 20");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Any()
    {
        // Act
        var result = _source.Any("Age > 20");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Average()
    {
        // Act
        var result = _source.Average("Age");

        // Assert
        result.Should().BeApproximately(35, 0.00001);
    }

    [Fact]
    public void Cast()
    {
        // Arrange
        var expected = new[] { "John", "Doe" };

        // Act 1
        var resultType = _source.Select("Name").Cast(typeof(string)).ToDynamicArray<string>();

        // Assert 1
        resultType.Should().Contain(expected);

        // Act 2
        var resultTypeName = _source.Select("Name").Cast("string").ToDynamicArray<string>();

        // Assert 2
        resultTypeName.Should().Contain(expected);
    }

    [Fact]
    public void Count()
    {
        // Act 1
        var result1 = _source.Count();

        // Assert 1
        result1.Should().Be(2);

        // Act 2
        var result2 = _source.Count("Age > 30");

        // Assert 2
        result2.Should().Be(1);
    }

    [Fact]
    public void Distinct()
    {
        var json = @"[
            {
                ""Name"": ""John""
            },
            {
                ""Name"": ""Doe""
            },
            {
                ""Name"": ""John""
            }
        ]";
        var source = JArray.Parse(json);

        // Act
        var result = source.Select("Name").Distinct();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Select()
    {
        // Act
        var result = _source.Select("Name");

        // Assert
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void Where_Select()
    {
        // Act
        var result = _source.Where("Age > 30").Select("Name");

        // Assert
        result.Should().HaveCount(1);
        var first = result.First();
        first.Value<string>().Should().Be("Doe");
    }
}