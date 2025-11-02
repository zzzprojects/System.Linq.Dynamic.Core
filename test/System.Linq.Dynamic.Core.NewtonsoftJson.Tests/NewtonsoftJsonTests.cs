using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.NewtonsoftJson.Config;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Tests;

public class NewtonsoftJsonTests
{
    private const string ExampleJsonObjectArray =
        """
        [
            {
                "Name": "John",
                "Age": 30
            },
            {
                "Name": "Doe",
                "Age": 40
            }
        ]
        """;
    private readonly JArray _source = JArray.Parse(ExampleJsonObjectArray);

    private const string ExampleJsonIntArray =
        """
        [
            30,
            40
        ]
        """;
    private readonly JArray _sourceIntArray = JArray.Parse(ExampleJsonIntArray);

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
        var result1 = _source.Count;

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
        var source = JArray.Parse(json);

        // Act
        var resultAsJson = source.GroupBy("Type").ToString();

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
        // Act + Assert 
        ((string?)_source.Last("Age > 30")["Name"]).Should().Be("Doe");
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
        ((int?)_source.Max("Age")).Should().Be(40);
    }

    [Fact]
    public void Min()
    {
        // Act + Assert 
        ((int?)_source.Min("Age")).Should().Be(30);
    }

    [Fact]
    public void OrderBy()
    {
        // Act
        var result = _source.OrderBy("Age").Select("Name");

        // Assert
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("John", "Doe");
    }

    [Fact]
    public void OrderBy_Asc()
    {
        // Act
        var resultAsc = _source.OrderBy("Age asc").Select("Name");

        // Assert
        var arrayAsc = resultAsc.Select(x => x.Value<string>());
        arrayAsc.Should().BeEquivalentTo("Doe", "John");
    }

    [Fact]
    public void OrderBy_Desc()
    {
        // Act
        var resultAsc = _source.OrderBy("Age desc").Select("Name");

        // Assert
        var arrayAsc = resultAsc.Select(x => x.Value<string>());
        arrayAsc.Should().BeEquivalentTo("John", "Doe");
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
        var source = JArray.Parse(json);

        // Act
        var result = source.OrderBy("Age, Name").Select("Name");

        // Assert
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("Stef", "John", "Doe");
    }

    [Fact]
    public void Page()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
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
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
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
        var json = "[1, 2, 3]";
        var source = JArray.Parse(json);

        // Act
        var result = source.Select(typeof(int), "it * it");

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(1, 4, 9);
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
        var source = JArray.Parse(json);

        // Act
        var result = source
            .SelectMany("PhoneNumbers")
            .Select("Number");

        // Assert
        var array = result.Select(x => x.Value<string>());
        array.Should().BeEquivalentTo("123", "456", "789", "012");
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
    public void SkipWhile()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JArray.Parse(json);

        // Act
        var result = source.SkipWhile("it > 5");

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(6, 7, 8, 9, 0);
    }

    [Fact]
    public void Take()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JArray.Parse(json);

        // Act
        var result = source.Take(3);

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(1, 2, 3);
    }

    [Fact]
    public void TakeWhile()
    {
        var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 0]";
        var source = JArray.Parse(json);

        // Act
        var result = source.TakeWhile("it < 5");

        // Assert
        var array = result.Select(x => x.Value<int>());
        array.Should().ContainInOrder(1, 2, 3, 4);
    }

    [Fact]
    public void Where_With_Select()
    {
        // Act
        var result = _source.Where("Age > 30").Select("Name");

        // Assert
        result.Should().HaveCount(1);
        var first = result.First();
        first.Value<string>().Should().Be("Doe");
    }

    //[Fact]
    //public void Where_OptionalProperty()
    //{
    //    // Arrange
    //    var config = new NewtonsoftJsonParsingConfig
    //    {
    //        ConvertObjectToSupportComparison = true
    //    };
    //    var array =
    //        """
    //        [
    //            {
    //                "Name": "John",
    //                "Age": 30
    //            },
    //            {
    //                "Name": "Doe"
    //            }
    //        ]
    //        """;

    //    // Act
    //    var result = JArray.Parse(array).Where(config, "Age > 30").Select("Name");

    //    // Assert
    //    result.Should().HaveCount(1);
    //    var first = result.First();
    //    first.Value<string>().Should().Be("John");
    //}

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
        // Arrange
        var config = new NewtonsoftJsonParsingConfig
        {
            ConvertObjectToSupportComparison = true
        };

        // Act
        var result = _source.Where(config, predicate);

        // Assert
        result.Should().BeEmpty();
    }
}