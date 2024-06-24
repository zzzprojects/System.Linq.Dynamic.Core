using System.Linq.Dynamic.Core.NewtonsoftJson.Extensions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Tests.Extensions;

public class JObjectExtensionsTests
{
    [Fact]
    public void ToDynamicClass()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var jObject = new JObject
        {
            { "Name", "John" },
            { "Age", 30 },
            { "IsMale", true },
            { "DateOfBirth", new DateTime(1990, 1, 1) },
            { "Guid", guid },
            { "Bytes", new byte[] { 1, 2, 3 } },
            { "TimeSpan", new TimeSpan(1, 2, 3) },
            { "Uri", new Uri("http://www.test.com") },
            { "Null", null },
            { "Undefined", JValue.CreateUndefined() },
            { "None", JValue.CreateNull() },
            { "Object", new JObject { { "Name", "Jane" } } },
            { "Array", new JArray { 1, 2, 3 } }
        };

        // Act
        var result = jObject.ToDynamicClass();

        // Assert
        result.Should().BeEquivalentTo(new
        {
            Name = "John",
            Age = 30,
            IsMale = true,
            DateOfBirth = new DateTime(1990, 1, 1),
            Guid = guid,
            Bytes = new byte[] { 1, 2, 3 },
            TimeSpan = new TimeSpan(1, 2, 3),
            Uri = new Uri("http://www.test.com"),
            Object = new { Name = "Jane" },
            Array = new[] { 1, 2, 3 }
        });
    }
}