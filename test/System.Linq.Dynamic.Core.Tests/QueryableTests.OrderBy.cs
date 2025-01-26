using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    public class IntComparerT : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return new CaseInsensitiveComparer().Compare(y, x);
        }
    }

    public class StringComparerT : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return new CaseInsensitiveComparer().Compare(y, x);
        }
    }

    public class ObjectComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            return new CaseInsensitiveComparer().Compare(y, x);
        }
    }

    [Fact]
    public void OrderBy_Dynamic_NullPropagation_Int()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.NullableInt ?? -1).ToArray();
        var orderByDynamic = qry.OrderBy(config, "np(NullableInt, -1)").ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_NullPropagation_String()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.NullableString ?? "_").ToArray();
        var orderByDynamic = qry.OrderBy(config, "np(NullableString, \"_\")").ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_NullPropagation_NestedObject()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.Profile?.Age ?? -1).ToArray();
        var orderByDynamic = qry.OrderBy(config, "np(Profile.Age, -1)").ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_NullPropagation_NestedObject_Query()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var qry = User.GenerateSampleModels(2)
            .Select(u => new
            {
                u.UserName,
                X = new
                {
                    u.Profile?.Age
                }
            })
            .AsQueryable();

        // Act
        var orderByDynamic = qry.OrderBy(config, "np(X.Age, -1)").ToArray();

        // Assert
        Assert.NotNull(orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_StringComparer()
    {
        // Arrange
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.UserName, StringComparer.OrdinalIgnoreCase).ToArray();
        var orderByDynamic = qry.OrderBy("UserName", StringComparer.OrdinalIgnoreCase).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_ObjectComparer_Int()
    {
        // Arrange
        var testList = User.GenerateSampleModels(3);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.Income, new IntComparerT()).ToArray();
        var orderByDynamic = qry.OrderBy("Income", new ObjectComparer()).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_ObjectComparer_String()
    {
        // Arrange
        var testList = User.GenerateSampleModels(3);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.UserName, new StringComparerT()).ToArray();
        var orderByDynamic = qry.OrderBy("UserName", new ObjectComparer()).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_IntComparerT()
    {
        // Arrange
        var testList = User.GenerateSampleModels(3);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.Income, new IntComparerT()).ToArray();
        var orderByDynamic = qry.OrderBy("Income", new IntComparerT()).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_IntComparerT_Asc()
    {
        // Arrange
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderBy(x => x.Income, new IntComparerT()).ToArray();
        var orderByDynamic = qry.OrderBy("Income asc", new IntComparerT()).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic_IComparer_IntComparerT_Desc()
    {
        // Arrange
        var testList = User.GenerateSampleModels(2);
        var qry = testList.AsQueryable();

        // Act
        var orderBy = testList.OrderByDescending(x => x.Income, new IntComparerT()).ToArray();
        var orderByDynamic = qry.OrderBy("Income desc", new IntComparerT()).ToArray();

        // Assert
        Assert.Equal(orderBy, orderByDynamic);
    }

    [Fact]
    public void OrderBy_Dynamic()
    {
        // Arrange
        var testList = User.GenerateSampleModels(100);
        var qry = testList.AsQueryable();

        // Act
        var orderById = qry.OrderBy("Id");
        var orderByAge = qry.OrderBy("Profile.Age");
        var orderByComplex1 = qry.OrderBy("Profile.Age, Id");

        // Assert
        Assert.Equal(testList.OrderBy(x => x.Id).ToArray(), orderById.ToArray());
        Assert.Equal(testList.OrderBy(x => x.Profile.Age).ToArray(), orderByAge.ToArray());
        Assert.Equal(testList.OrderBy(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex1.ToArray());
    }

    [Fact]
    public void OrderBy_Dynamic_AsStringExpression()
    {
        // Arrange
        var testList = User.GenerateSampleModels(100);
        var qry = testList.AsQueryable();

        // Act
        var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name)).Select(x => x.Name);
        var orderById = qry.SelectMany("Roles.OrderBy(Name)").Select("Name");

        // Assert
        Assert.Equal(expected.ToArray(), orderById.Cast<string>().ToArray());
    }

    [Fact]
    public void OrderBy_Dynamic_Exceptions()
    {
        // Arrange
        var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
        var qry = testList.AsQueryable();

        // Act
        Assert.Throws<ParseException>(() => qry.OrderBy("Bad=3"));
        Assert.Throws<ParseException>(() => qry.Where("Id=123"));

        Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.OrderBy(null, "Id"));
        Assert.Throws<ArgumentNullException>(() => qry.OrderBy(null));
        Assert.Throws<ArgumentException>(() => qry.OrderBy(""));
        Assert.Throws<ArgumentException>(() => qry.OrderBy(" "));
    }

    [Theory]
    [InlineData(KeywordsHelper.KEYWORD_IT)]
    [InlineData(KeywordsHelper.SYMBOL_IT)]
    [InlineData(KeywordsHelper.KEYWORD_ROOT)]
    [InlineData(KeywordsHelper.SYMBOL_ROOT)]
    [InlineData("\"User\" + \"Name\"")]
    [InlineData("\"User\" + \"Name\" asc")]
    [InlineData("\"User\" + \"Name\" desc")]
    public void OrderBy_RestrictOrderByIsTrue_NonRestrictedExpressionShouldNotThrow(string expression)
    {
        // Arrange
        var queryable = User.GenerateSampleModels(3).AsQueryable();

        // Act
        Action action = () => _ = queryable.OrderBy(expression);

        // Assert 2
        action.Should().NotThrow();
    }
}