using System.Linq;
using System.Linq.Dynamic.Core.NewtonsoftJson;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Json;

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
}