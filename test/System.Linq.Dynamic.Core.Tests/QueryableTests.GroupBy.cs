using System.Collections.Generic;
using NFluent;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    public class DataSetA
    {
        public DateTime Time { get; set; }

        public int? I { get; set; }
    }

    public class DateTimeTest
    {
        public DateTimeTest? Test { get; set; }

        public DateTime? D { get; set; }
    }

    [Fact]
    public void GroupBy_Dynamic()
    {
        //Arrange
        var testList = User.GenerateSampleModels(100);
        var qry = testList.AsQueryable();

        //Act
        var byAgeReturnUserName = qry.GroupBy("Profile.Age", "UserName");
        var byAgeReturnAll = qry.GroupBy("Profile.Age");

        //Assert
        Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnUserName.Count());
        Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnAll.Count());
    }

    [Fact]
    public void GroupBy_Dynamic_NullableDateTime()
    {
        // Arrange
        var qry = new[] { new DateTime(2020, 1, 1), (DateTime?)null }.AsQueryable();

        // Act
        var byYear = qry.GroupBy("np(Value.Year, 2019)").ToDynamicArray();

        // Assert
        byYear.Should().HaveCount(2);
    }

    [Fact]
    public void GroupBy_Dynamic_NullableDateTime_2Levels()
    {
        // Arrange
        var qry = new[]
        {
            new DateTimeTest { Test = new DateTimeTest { D = new DateTime(2020, 1, 1) } },
            new DateTimeTest { Test = null }
        }.AsQueryable();

        // Act
        var byYear = qry.GroupBy("np(Test.D.Value.Year, 2019)").ToDynamicArray();

        // Assert
        byYear.Should().HaveCount(2);
    }

    [Fact]
    public void GroupBy_Dynamic_NullableDateTime_3Levels()
    {
        // Arrange
        var qry = new[]
        {
            new DateTimeTest { Test = new DateTimeTest { Test = new DateTimeTest { D = new DateTime(2020, 1, 1) } } },
            new DateTimeTest { Test = null }
        }.AsQueryable();

        // Act
        var byYear = qry.GroupBy("np(Test.Test.D.Value.Year, 2019)").ToDynamicArray();

        // Assert
        byYear.Should().HaveCount(2);
    }

    // https://github.com/StefH/System.Linq.Dynamic.Core/issues/75
    [Fact]
    public void GroupBy_Dynamic_Issue75()
    {
        var testList = User.GenerateSampleModels(100);

        var resultDynamic = testList.AsQueryable().GroupBy("Profile.Age").Select("new (it.key as PropertyKey)");
        var result = testList.GroupBy(e => e.Profile.Age).Select(e => new { PropertyKey = e.Key }).AsQueryable();

        // I think this should be true, but it isn't. dynamicResult add System.Object Item [System.String] property.
        PropertyInfo[] properties = result.ElementType.GetTypeInfo().GetProperties();
        PropertyInfo[] propertiesDynamic = resultDynamic.ElementType.GetTypeInfo().GetProperties();

        Check.That(propertiesDynamic.Length).IsStrictlyGreaterThan(properties.Length);
    }

    [Fact]
    public void GroupBy_Dynamic_Exceptions()
    {
        //Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        //Act
        Assert.Throws<ParseException>(() => qry.GroupBy("Bad"));
        Assert.Throws<ParseException>(() => qry.GroupBy("Id, UserName"));
        Assert.Throws<ParseException>(() => qry.GroupBy("new Id, UserName"));
        Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName"));
        Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName, Bad)"));

        Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.GroupBy((IQueryable<string>)null, "Id"));
        Assert.Throws<ArgumentNullException>(() => qry.GroupBy(null));
        Assert.Throws<ArgumentException>(() => qry.GroupBy(""));
        Assert.Throws<ArgumentException>(() => qry.GroupBy(" "));

        Assert.Throws<ArgumentNullException>(() => qry.GroupBy("Id", (string)null));
        Assert.Throws<ArgumentException>(() => qry.GroupBy("Id", ""));
        Assert.Throws<ArgumentException>(() => qry.GroupBy("Id", " "));
    }

    [Fact(Skip = "This test is skipped because it fails in CI.")]
    public void GroupBy_Dynamic_Issue403()
    {
        var data = new List<object> {
            new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO01" } ,
            new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO02" } ,
            new { ItemCode = "AAAA", Flag = false, SoNo="aaa",JobNo="JNO03" } ,
            new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO04" },
            new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO05" } ,
            new { ItemCode = "BBBB", Flag = true, SoNo="ccc",JobNo="JNO06" } ,
        };
        var jsonString = JsonConvert.SerializeObject(data);
        var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

        var groupList = list.AsQueryable().GroupBy("new (ItemCode, Flag)").ToDynamicList();

        Assert.Equal(3, groupList.Count);
    }

    [Fact]
    public void GroupBy_Dynamic_SelectWhereMin()
    {
        // Arrange
        var q = new[]
        {
            new DataSetA
            {
                I = 5
            },
            new DataSetA
            {
                I = 7
            }
        }
        .AsQueryable();

        // Act
        var result = q
            .GroupBy(x => x.Time)
            .Select(x => new { q = x.Select(d => d.I).Where(d => d != null).Min() })
            .ToArray();

        var resultDynamic = q
            .GroupBy("Time")
            .Select("new (Select(I).Where(it != null).Min() as q)")
            .ToDynamicArray();

        // Assert
        resultDynamic.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void GroupBy_Dynamic_SelectWhereMax()
    {
        // Arrange
        var q = new[]
            {
                new DataSetA
                {
                    I = 5
                },
                new DataSetA
                {
                    I = 7
                }
            }
            .AsQueryable();

        // Act
        var result = q
            .GroupBy(x => x.Time)
            .Select(x => new { q = x.Select(d => d.I).Where(d => d != null).Max() })
            .ToArray();

        var resultDynamic = q
            .GroupBy("Time")
            .Select("new (Select(I).Where(it != null).Max() as q)")
            .ToDynamicArray();

        // Assert
        resultDynamic.Should().BeEquivalentTo(result);
    }

    // #633
    [Fact]
    public void GroupBy_Dynamic_SelectWhereAverage()
    {
        // Arrange
        var q = new[]
            {
                new DataSetA
                {
                    I = 5
                },
                new DataSetA
                {
                    I = 7
                }
            }
            .AsQueryable();

        // Act
        var result = q
            .GroupBy(x => x.Time)
            .Select(x => new { q = x.Select(d => d.I).Where(d => d != null).Average() })
            .ToArray();

        var resultDynamic = q
            .GroupBy("Time")
            .Select("new (Select(I).Where(it != null).Average() as q)")
            .ToDynamicArray();

        // Assert
        resultDynamic.Should().BeEquivalentTo(result);
    }
    
    [Fact]
    public void GroupBy_Dynamic_SelectWhereAverageWithSelector()
    {
        // Arrange
        var q = new[]
            {
                new DataSetA
                {
                    I = 5
                },
                new DataSetA
                {
                    I = 7
                }
            }
            .AsQueryable();

        // Act
        var result = q
            .GroupBy(x => x.Time)
            .Select(x => new { q = x.Select(d => d).Average(y => y.I) })
            .ToArray();

        var resultDynamic = q
            .GroupBy("Time")
            .Select("new (Select(it).Average(I) as q)")
            .ToDynamicArray();

        // Assert
        resultDynamic.Should().BeEquivalentTo(result);
    }
}