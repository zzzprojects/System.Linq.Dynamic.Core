using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Where_Dynamic()
    {
        // Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        // Act
        var userById = qry.Where("Id=@0", testList[10].Id);
        var userByUserName = qry.Where("UserName=\"User5\"");
        var nullProfileCount = qry.Where("Profile=null");
        var userByFirstName = qry.Where("Profile!=null && Profile.FirstName=@0", testList[1].Profile.FirstName);

        // Assert
        Assert.Equal(testList[10], userById.Single());
        Assert.Equal(testList[5], userByUserName.Single());
        Assert.Equal(testList.Count(x => x.Profile == null), nullProfileCount.Count());
        Assert.Equal(testList[1], userByFirstName.Single());
    }

    [Fact]
    public void Where_Dynamic_CheckCastToObject()
    {
        // Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        // Act
        string dynamicExpression = qry.Where("Profile == null").Expression.ToDebugView();
        string expresion = qry.Where(var1 => var1.Profile == null).Expression.ToDebugView();

        // Assert
        NFluent.Check.That(dynamicExpression).Equals(expresion);
    }

    [Theory]
    [InlineData("Fri, 10 May 2019 11:03:17 GMT", 11)]
    [InlineData("Fri, 10 May 2019 11:03:17 -07:00", 18)]
    public void Where_Dynamic_DateTimeIsParsedAsUTC(string time, int hours)
    {
        // Arrange
        var queryable = new List<Example> {
            new Example
            {
                TimeNull = new DateTime(2019, 5, 10, hours, 3, 17, DateTimeKind.Utc)
            }
        }.AsQueryable();

        // Act
        var parsingConfig = new ParsingConfig
        {
            DateTimeIsParsedAsUTC = true
        };
        var result = queryable.Where(parsingConfig, $"it.TimeNull >= \"{time}\"");

        // Assert
        Assert.Equal(1, result.Count());
    }

    /// <summary>
    /// https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
    /// </summary>
    [Fact]
    public void Where_Dynamic_DateTime_NotEquals_Null()
    {
        //Arrange
        IQueryable<Post> queryable = new[] { new Post() }.AsQueryable();

        //Act
        var expected = queryable.Where(p => p.PostDate != null).ToArray();
        var result1 = queryable.Where("PostDate != null").ToArray();
        var result2 = queryable.Where("null != PostDate").ToArray();

        //Assert
        Assert.Equal(expected, result1);
        Assert.Equal(expected, result2);
    }

    [Fact]
    public void Where_Dynamic_DateTime_Equals_Null()
    {
        //Arrange
        IQueryable<Post> queryable = new[] { new Post() }.AsQueryable();

        //Act
        var expected = queryable.Where(p => p.PostDate == null).ToArray();
        var result1 = queryable.Where("PostDate == null").ToArray();
        var result2 = queryable.Where("null == PostDate").ToArray();

        //Assert
        Assert.Equal(expected, result1);
        Assert.Equal(expected, result2);
    }

    [Fact]
    public void Where_Dynamic_IQueryable_LambdaExpression()
    {
        // Arrange
        var queryable = (IQueryable)new[] { new User { Income = 5 } }.AsQueryable();

        Expression<Func<User, bool>> userExpression = u => u.Income > 1;
        LambdaExpression lambdaExpression = userExpression;

        // Act
        var result = queryable.Where(lambdaExpression).ToDynamicArray();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void Where_Dynamic_IQueryableT_LambdaExpression()
    {
        // Arrange
        var queryable = new[] { new User { Income = 5 } }.AsQueryable();

        Expression<Func<User, bool>> userExpression = u => u.Income > 1;
        LambdaExpression lambdaExpression = userExpression;

        // Act
        var result = queryable.Where(lambdaExpression);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void Where_Dynamic_Exceptions()
    {
        //Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        //Act
        Assert.Throws<InvalidOperationException>(() => qry.Where("Id"));
        Assert.Throws<ParseException>(() => qry.Where("Bad=3"));
        Assert.Throws<ParseException>(() => qry.Where("Id=123"));

        Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.Where(null, "Id=1"));
        Assert.Throws<ArgumentNullException>(() => qry.Where((string?)null));
        Assert.Throws<ArgumentException>(() => qry.Where(""));
        Assert.Throws<ArgumentException>(() => qry.Where(" "));
    }

    [Fact]
    public void Where_Dynamic_StringQuoted()
    {
        // Arrange
        var testList = User.GenerateSampleModels(2, allowNullableProfiles: true);
        testList[0].UserName = @"This \""is\"" a test.";
        var qry = testList.AsQueryable();

        // Act
        // var result1a = qry.Where(@"UserName == ""This \\""is\\"" a test.""").ToArray();
        var result1b = qry.Where("UserName == \"This \\\\\\\"is\\\\\\\" a test.\"").ToArray();
        var result2a = qry.Where("UserName == @0", @"This \""is\"" a test.").ToArray();
        var result2b = qry.Where("UserName == @0", "This \\\"is\\\" a test.").ToArray();

        var expected = qry.Where(x => x.UserName == @"This \""is\"" a test.").ToArray();

        // Assert
        Assert.Single(expected);
        // Assert.Equal(expected, result1a);
        Assert.Equal(expected, result1b);
        Assert.Equal(expected, result2a);
        Assert.Equal(expected, result2b);
    }

    [Fact]
    public void Where_Dynamic_EmptyString()
    {
        // Arrange
        var testList = User.GenerateSampleModels(2, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        // Act
        var expected1 = qry.Where(u => u.UserName != string.Empty).ToArray();
        var expected2 = qry.Where(u => u.UserName != "").ToArray();
        var resultDynamic1 = qry.Where("UserName != @0", string.Empty).ToArray();
        var resultDynamic2 = qry.Where("UserName != @0", "").ToArray();

        // Assert
        resultDynamic1.Should().Contain(expected1);
        resultDynamic2.Should().Contain(expected2);
    }

    [Fact]
    public void Where_Dynamic_SelectNewObjects()
    {
        //Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        //Act
        var expectedResult = testList.Where(x => x.Income > 4000).Select(x => new { Id = x.Id, Income = x.Income + 1111 });
        var dynamicList = qry.Where("Income > @0", 4000).ToDynamicList();

        var newUsers = dynamicList.Select(x => new { Id = x.Id, Income = x.Income + 1111 });
        Assert.Equal(newUsers.Cast<object>().ToList(), expectedResult);
    }

    [Fact]
    public void Where_Dynamic_ExpandoObject_As_Dictionary_Is_Null_Should_Throw_InvalidOperationException()
    {
        // Arrange
        var productsQuery = new[] { new ProductDynamic { ProductId = 1 } }.AsQueryable();

        // Act
        Action action = () => productsQuery.Where("Properties.Name == @0", "First Product").ToDynamicList();

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact(Skip = "NP does not work here")]
    public void Where_Dynamic_ExpandoObject_As_Dictionary_Is_Null_With_NullPropagating()
    {
        // Arrange
        var productsQuery = new[] { new ProductDynamic { ProductId = 1 } }.AsQueryable();

        // Act
        var results = productsQuery.Where("np(Properties.Name, \"no\") == @0", "First Product").ToDynamicList();

        // Assert
        results.Should().HaveCount(0);
    }

    [Fact]
    public void Where_Dynamic_ExpandoObject_As_Dictionary()
    {
        // Arrange
        var productsQuery = new[] { new ProductDynamic { ProductId = 1, Properties = new Dictionary<string, object> { { "Name", "test" } } } }.AsQueryable();

        // Act
        var results = productsQuery.Where("Properties.Name == @0", "test").ToDynamicList();

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void Where_Dynamic_Object_As_Dictionary()
    {
        // Arrange
        var productsQuery = new[] { new ProductDynamic { ProductId = 1, PropertiesAsObject = new Dictionary<string, object> { { "Name", "test" } } } }.AsQueryable();

        // Act
        var results = productsQuery.Where("PropertiesAsObject.Name == @0", "test").ToDynamicList();

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void Where_Dynamic_ExpandoObject_As_AnonymousType()
    {
        // Arrange
        var productsQuery = new[] { new ProductDynamic { ProductId = 1, Properties = new { Name = "test" } } }.AsQueryable();

        // Act
        var results = productsQuery.Where("Properties.Name == @0", "test").ToDynamicList<ProductDynamic>();

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void Where_Dynamic_DateTimeConstructor_Issue662()
    {
        // Arrange
        var config = new ParsingConfig
        {
            PrioritizePropertyOrFieldOverTheType = false
        };
        var date = new DateTime(2023, 1, 13, 12, 0, 0);
        var queryable = new List<Foo>
        {
            new() { DateTime = date },
            new() { DT = date }
        }.AsQueryable();

        // Act 1
        var result1 = queryable.Where(config, "DT > DateTime(2022, 1, 1, 0, 0, 0)").ToArray();

        // Assert 1
        result1.Should().HaveCount(1);

        // Act 2
        var result2 = queryable.Where(config, "it.DateTime > DateTime(2022, 1, 1, 0, 0, 0)").ToArray();

        // Assert 2
        result2.Should().HaveCount(1);
    }

    public class ProductDynamic
    {
        public int ProductId { get; set; }

        public dynamic Properties { get; set; }

        public object PropertiesAsObject { get; set; }
    }

    public class Foo
    {
        public DateTime DT { get; set; }

        public DateTime DateTime { get; set; }
    }
}