using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests;

public class SystemTextJsonTests
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
    private readonly JsonDocument _source = JsonDocument.Parse(ExampleJson);

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
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.Select("Name").Distinct();

        // Assert
        result.RootElement.EnumerateArray().Should().HaveCount(2);
    }

    [Fact]
    public void First()
    {
        // Act + Assert 1
        _source.First().GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""John"",""Age"":30}").RootElement.GetRawText());

        // Act + Assert 2
        _source.First("Age > 30").GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());
    }

    [Fact]
    public void FirstOrDefault()
    {
        // Act + Assert 1
        _source.FirstOrDefault()!.Value.GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""John"",""Age"":30}").RootElement.GetRawText());

        // Act + Assert 2
        _source.FirstOrDefault("Age > 30")!.Value.GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());

        // Act + Assert 3
        _source.FirstOrDefault("Age > 999").Should().BeNull();
    }

    [Fact]
    public void Last()
    {
        // Act + Assert 1
        _source.Last().GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());

        // Act + Assert 2
        _source.Last("Age > 0").GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());
    }

    [Fact]
    public void LastOrDefault()
    {
        // Act + Assert 1
        _source.LastOrDefault()!.Value.GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());

        // Act + Assert 2
        _source.LastOrDefault("Age > 0")!.Value.GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());

        // Act + Assert 3
        _source.LastOrDefault("Age > 999").Should().BeNull();
    }

    [Fact]
    public void Max()
    {
        // Act + Assert
        _source.Max("Age").GetRawText().Should().BeEquivalentTo("40");
    }

    [Fact]
    public void Min()
    {
        // Act + Assert
        _source.Min("Age").GetRawText().Should().BeEquivalentTo("30");
    }

    [Fact]
    public void OrderBy()
    {
        // Act 1
        var result = _source.OrderBy("Age").Select("Name");

        // Assert 1
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("John", "Doe");

        // Act 1
        var resultAsc = _source.OrderBy("Age", "Asc").Select("Name");

        // Assert 1
        var arrayAsc = resultAsc.RootElement.EnumerateArray().Select(x => x.GetString());
        arrayAsc.Should().BeEquivalentTo("Doe", "John");
    }

    [Fact]
    public void Select()
    {
        // Act
        var result = _source.Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void Where_Select()
    {
        // Act
        var result = _source.Where("Age > 30").Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray();
        array.Should().HaveCount(1);
        array.First().GetString().Should().Be("Doe");
    }
}