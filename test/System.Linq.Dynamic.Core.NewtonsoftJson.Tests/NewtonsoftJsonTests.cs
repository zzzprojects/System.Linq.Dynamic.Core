using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Tests;

public class NewtonsoftJsonTests
{
    [Fact]
    public void Aggregate()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 25
            }
        ]";

        var jArray = JArray.Parse(json);

        // Act
        var result = jArray.Aggregate("Sum", "Age");

        // Assert
        result.Should().Be(55);
    }

    [Fact]
    public void All()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 25
            }
        ]";

        var jArray = JArray.Parse(json);

        // Act
        var result = jArray.All("Age > 20");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Any()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 25
            }
        ]";

        var jArray = JArray.Parse(json);

        // Act
        var result = jArray.Any("Age > 20");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Average()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 40
            }
        ]";

        var jArray = JArray.Parse(json);

        // Act
        var result = jArray.Average("Age");

        // Assert
        result.Should().BeApproximately(35, 0.00001);
    }

    [Fact]
    public void Select()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 25
            }
        ]";

        var doc = JArray.Parse(json);

        // Act
        var result = doc.Select("Name");

        // Assert
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void Where_Select()
    {
        // Arrange
        var json = @"[
            {
                ""Name"": ""John"",
                ""Age"": 30
            },
            {
                ""Name"": ""Doe"",
                ""Age"": 25
            }
        ]";

        var doc = JArray.Parse(json);

        // Act
        var result = doc.Where("Age > 25").Select("Name");

        // Assert
        result.Should().HaveCount(1);
        var first = result.First();
        first.Value<string>().Should().Be("John");
    }
}