using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests;

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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.Aggregate("Sum", "Age");

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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.All("Age > 20");

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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.Any("Age > 20");

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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.Average("Age");

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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
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

        var doc = JsonDocument.Parse(json);

        // Act
        var result = doc.Where("Age > 25").Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray();
        array.Should().HaveCount(1);
        array.First().GetString().Should().Be("John");
    }
}