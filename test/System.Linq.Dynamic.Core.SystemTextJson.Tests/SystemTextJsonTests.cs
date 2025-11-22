using System.Linq.Dynamic.Core.SystemTextJson.Config;
using System.Text.Json;
using AwesomeAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.SystemTextJson.Tests;

public class SystemTextJsonTests
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    private const string ExampleJsonObjectArray =
        """
        [
            {
                "Name": "John",
                "Age": 30,
                "IsNull": null
            },
            {
                "Name": "Doe",
                "Age": 40,
                "AlsoNull": null
            }
        ]
        """;
    private readonly JsonDocument _source = JsonDocument.Parse(ExampleJsonObjectArray);

    private const string ExampleJsonIntArray =
        """
        [
            30,
            40
        ]
        """;
    private readonly JsonDocument _sourceIntArray = JsonDocument.Parse(ExampleJsonIntArray);

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
        var result = _source.Any();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Any_Predicate()
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
        var result = _sourceIntArray.Average();

        // Assert
        result.Should().BeApproximately(35, 0.00001);
    }

    [Fact]
    public void Average_Predicate()
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
        var json =
            """
            [
                {
                    "Name": "John"
                },
                {
                    "Name": "Doe"
                },
                {
                    "Name": "John"
                }
            ]
            """;
        using var source = JsonDocument.Parse(json);

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
    public void GroupBySimpleKeySelector()
    {
        // Arrange
        var json =
            """
            [
              {
                "Name": "Mr. Test Smith",
                "Type": "PAY",
                "Something": {
                  "Field1": "Test1",
                  "Field2": "Test2"
                }
              },
              {
                "Name": "Mr. Test Smith",
                "Type": "DISPATCH",
                "Something": {
                  "Field1": "Test1",
                  "Field2": "Test2"
                }
              },
              {
                "Name": "Different Name",
                "Type": "PAY",
                "Something": {
                  "Field1": "Test3",
                  "Field2": "Test4"
                }
              }
            ]
            """;
        using var source = JsonDocument.Parse(json);

        // Act
        var result = source.GroupBy("Type");
        var resultAsJson = JsonSerializer.Serialize(result, _options);

        // Assert
        var expected =
            """
            [
              {
                "Key": "PAY",
                "Values": [
                  {
                    "Name": "Mr. Test Smith",
                    "Type": "PAY",
                    "Something": {
                      "Field1": "Test1",
                      "Field2": "Test2"
                    }
                  },
                  {
                    "Name": "Different Name",
                    "Type": "PAY",
                    "Something": {
                      "Field1": "Test3",
                      "Field2": "Test4"
                    }
                  }
                ]
              },
              {
                "Key": "DISPATCH",
                "Values": [
                  {
                    "Name": "Mr. Test Smith",
                    "Type": "DISPATCH",
                    "Something": {
                      "Field1": "Test1",
                      "Field2": "Test2"
                    }
                  }
                ]
              }
            ]
            """;

        resultAsJson.Should().Be(expected);
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
        _source.Max("Age").Should().BeEquivalentTo(40);
    }

    [Fact]
    public void Min()
    {
        // Act + Assert
        _source.Min("Age").Should().BeEquivalentTo(30);
    }

    [Fact]
    public void OrderBy()
    {
        // Act
        var result = _source.OrderBy("Age").Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void OrderBy_Asc()
    {
        // Act
        var resultAsc = _source.OrderBy("Age asc").Select("Name");

        // Assert
        var arrayAsc = resultAsc.RootElement.EnumerateArray().Select(x => x.GetString());
        arrayAsc.Should().BeEquivalentTo("Doe", "John");
    }

    [Fact]
    public void OrderBy_Desc()
    {
        // Act 1
        var resultAsc = _source.OrderBy("Age desc").Select("Name");

        // Assert 1
        var arrayAsc = resultAsc.RootElement.EnumerateArray().Select(x => x.GetString());
        arrayAsc.Should().BeEquivalentTo("Doe", "John");
    }

    [Fact]
    public void OrderBy_Multiple()
    {
        // Arrange
        var json =
            """
            [
                {
                    "Name": "John",
                    "Age": 30
                },
                {
                    "Name": "Doe",
                    "Age": 30
                },
                {
                    "Name": "Stef",
                    "Age": 18
               }
            ]
            """;
        using var source = JsonDocument.Parse(json);

        // Act
        var result = source.OrderBy("Age, Name").Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("Stef", "John", "Doe");
    }

    [Fact]
    public void Page()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        using var source = JsonDocument.Parse(json);

        // Act
        var result = source.Page(2, 3);

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(4, 5, 6);
    }

    [Fact]
    public void PageResult()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        using var source = JsonDocument.Parse(json);

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
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void SelectMany()
    {
        // Arrange
        var json =
            """
               [{
                   "PhoneNumbers": [
                       { "Number": "123" },
                       { "Number": "456" }
                   ]
               },
               {
                   "PhoneNumbers": [
                       { "Number": "789" },
                       { "Number": "012" }
                   ]
               }]
            """;
        using var source = JsonDocument.Parse(json);

        // Act
        var result = source
            .SelectMany("PhoneNumbers")
            .Select("Number");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetString());
        array.Should().BeEquivalentTo("123", "456", "789", "012");
    }

    [Fact]
    public void Single()
    {
        // Act + Assert
        _source.Single("Age > 30").GetRawText().Should().BeEquivalentTo(JsonDocument.Parse(@"{""Name"":""Doe"",""Age"":40}").RootElement.GetRawText());
    }

    [Fact]
    public void SingleOrDefault()
    {
        // Act + Assert
        _source.SingleOrDefault("Age > 999").Should().BeNull();
    }

    [Fact]
    public void Select_ResultType()
    {
        // Arrange
        var json = "[1, 2, 3]";
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.Select(typeof(int), "it * it");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(1, 4, 9);
    }

    [Fact]
    public void Skip()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.Skip(3);

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(4, 5, 6, 7, 8, 9, 0);
    }

    [Fact]
    public void SkipWhile()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.SkipWhile("it > 5");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(6, 7, 8, 9, 0);
    }

    [Fact]
    public void Take()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.Take(3);

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(1, 2, 3);
    }

    [Fact]
    public void TakeWhile()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JsonDocument.Parse(json);

        // Act
        var result = source.TakeWhile("it < 5");

        // Assert
        var array = result.RootElement.EnumerateArray().Select(x => x.GetInt32());
        array.Should().ContainInOrder(1, 2, 3, 4);
    }

    [Fact]
    public void Where_With_Select()
    {
        // Act
        var result = _source.Where("Age > 30").Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray();
        array.Should().HaveCount(1);
        array.First().GetString().Should().Be("Doe");
    }

    [Theory]
    [InlineData("notExisting == true")]
    [InlineData("notExisting == \"true\"")]
    [InlineData("notExisting == 1")]
    [InlineData("notExisting == \"1\"")]
    [InlineData("notExisting == \"something\"")]
    [InlineData("notExisting > 1")]
    [InlineData("true == notExisting")]
    [InlineData("\"true\" == notExisting")]
    [InlineData("1 == notExisting")]
    [InlineData("\"1\" == notExisting")]
    [InlineData("\"something\" == notExisting")]
    [InlineData("1 < notExisting")]
    public void Where_NonExistingMember_EmptyResult(string predicate)
    {
        // Act
        var result = _source.Where(predicate).RootElement.EnumerateArray();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("""[ { "Name": "John", "Age": 30 }, { "Name": "Doe" }, { } ]""")]
    [InlineData("""[ { "Name": "Doe" }, { "Name": "John", "Age": 30 }, { } ]""")]
    public void NormalizeArray(string data)
    {
        // Arrange
        var source = JsonDocument.Parse(data);

        // Act
        var result = source
            .Where("Age >= 30")
            .Select("Name");

        // Assert
        var array = result.RootElement.EnumerateArray();
        array.Should().HaveCount(1);
        var first = result.First();
        array.First().GetString().Should().Be("John");
    }

    [Fact]
    public void NormalizeArray_When_NormalizeIsFalse_ShouldThrow()
    {
        // Arrange
        var config = new SystemTextJsonParsingConfig
        {
            Normalize = false
        };
        var data = """[ { "Name": "Doe" }, { "Name": "John", "Age": 30 }, { } ]""";

        // Act
        Action act = () => JsonDocument.Parse(data).Where(config, "Age >= 30");

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Unable to find property 'Age' on type '<>f__AnonymousType*");
    }
}