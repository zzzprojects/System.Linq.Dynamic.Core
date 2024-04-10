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
    public void First()
    {
        // Act + Assert
        ((string?)_source.First("Age > 30")["Name"]).Should().Be("Doe");
    }

    [Fact]
    public void FirstOrDefault()
    {
        // Act + Assert 1
        ((string?)_source.FirstOrDefault("Age > 30")!["Name"]).Should().Be("Doe");

        // Act + Assert 2
        _source.FirstOrDefault("Age > 999").Should().BeNull();
    }

    [Fact]
    public void Last()
    {
        // Act + Assert 
        ((string?)_source.First("Age > 30")["Name"]).Should().Be("Doe");
    }

    [Fact]
    public void LastOrDefault()
    {
        // Act + Assert 1
        ((string?)_source.LastOrDefault("Age > 0")!["Name"]).Should().Be("Doe");

        // Act + Assert 2
        _source.LastOrDefault("Age > 999").Should().BeNull();
    }

    [Fact]
    public void Max()
    {
        // Act + Assert 
        ((string?)_source.Max("Age")).Should().Be("40");
    }

    [Fact]
    public void Min()
    {
        // Act + Assert 
        ((string?)_source.Min("Age")).Should().Be("30");
    }

    [Fact]
    public void OrderBy()
    {
        // Act 1
        var result = _source.OrderBy("Age").Select("Name");

        // Assert 1
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("John", "Doe");

        // Act 1
        var resultAsc = _source.OrderBy("Age", "Asc").Select("Name");

        // Assert 1
        var arrayAsc = resultAsc.Select(x => x.Value<string>());
        arrayAsc.Should().BeEquivalentTo("Doe", "John");
    }

    [Fact]
    public void Page()
    {
        var json = @"[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JArray.Parse(json);

        // Act
        var result = source.Page(2, 3);

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(4, 5, 6);
    }

    [Fact]
    public void PageResult()
    {
        var json = @"[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JArray.Parse(json);

        // Act
        var pagedResult = source.PageResult(2, 3);

        // Assert
        pagedResult.Should().BeEquivalentTo(new PagedResult
        {
            CurrentPage = 2,
            PageCount = 4,
            PageSize = 3,
            RowCount = 10,
            Queryable = new[] { 4, 5, 6 }.AsQueryable()
        });
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
    public void Select_ResultType()
    {
        // Arrange
        var json = @"[1, 2, 3]";
        var source = JArray.Parse(json);

        // Act
        var result = source.Select(typeof(int), "it * it");

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(1, 4, 9);
    }

    [Fact]
    public void Single()
    {
        // Act + Assert 
        ((string?)_source.First("Age > 30")["Name"]).Should().Be("Doe");
    }

    [Fact]
    public void SingleOrDefault()
    {
        // Act + Assert 1
        ((string?)_source.LastOrDefault("Age > 30")!["Name"]).Should().Be("Doe");

        // Act + Assert 2
        _source.LastOrDefault("Age > 999").Should().BeNull();
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