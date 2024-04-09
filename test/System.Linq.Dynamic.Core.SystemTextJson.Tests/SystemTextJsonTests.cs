using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests;

public class NewtonsoftJsonTests
{
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
}