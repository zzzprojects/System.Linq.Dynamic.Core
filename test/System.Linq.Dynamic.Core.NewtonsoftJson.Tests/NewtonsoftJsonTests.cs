using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Tests;

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
}