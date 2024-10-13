using System.Linq.Dynamic.Core.SystemTextJson.Extensions;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests.Extensions;

public class JsonDocumentExtensionsTests
{
    [Fact]
    public void ToDynamicClass()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(@"{
            ""Name"": ""John"",
            ""Age"": 30,
            ""IsMale"": true,
            ""IsTest"": false,
            ""DateOfBirth"": ""1990-01-01"",
            ""Guid"": """ + guid + @""",
            ""TimeSpan"": ""01:02:03"",
            ""Uri"": ""http://www.test.com"",
            ""Null"": null,
            ""Object"": { ""Name"": ""Jane"" },
            ""Array"": [1, 2, 3]
        }"));
        JsonElement? jsonElement = JsonElement.ParseValue(ref utf8JsonReader);

        // Act
        var result = jsonElement.ToDynamicClass();

        // Assert
        result.Should().BeEquivalentTo(new
        {
            Name = "John",
            Age = 30,
            IsMale = true,
            IsTest = false,
            DateOfBirth = new DateTime(1990, 1, 1),
            Guid = guid,
            TimeSpan = "01:02:03",
            Uri = "http://www.test.com",
            Object = new { Name = "Jane" },
            Array = new[] { 1, 2, 3 }
        });
    }
}