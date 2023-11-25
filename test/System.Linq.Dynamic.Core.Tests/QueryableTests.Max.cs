using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Max()
    {
        // Arrange
        var incomes = User.GenerateSampleModels(10).Select(u => u.Income).ToArray();

        // Act
        var expected = incomes.Max();
        var actual = incomes.AsQueryable().Max();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Max_Selector()
    {
        // Arrange
        var users = User.GenerateSampleModels(10);

        // Act
        var expected = users.Max(u => u.Income);
        var result = users.AsQueryable().Max("Income");

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Max_Where_On_Int()
    {
        // Arrange
        var users = User.GenerateSampleModels(10);

        // Act
        var typed = users
            .Where(u => users.Max(m => m.Income) == u.Income)
            .ToList();
        var dynamic = users.AsQueryable()
            .Where("@0.Max(Income) == Income", users)
            .ToList();

        // Assert
        dynamic.Should().BeEquivalentTo(typed);
    }

    [Fact]
    public void Max_Where_On_DateTime()
    {
        // Arrange
        var users = User.GenerateSampleModels(10);

        // Act
        var typed = users
            .Where(u => users.Max(m => m.BirthDate) == u.BirthDate)
            .ToList();
        var dynamic = users.AsQueryable()
            .Where("@0.Max(BirthDate) == BirthDate", users)
            .ToList();

        // Assert
        dynamic.Should().BeEquivalentTo(typed);
    }

    [Fact]
    public void Max_Where_On_NullableDateTime()
    {
        // Arrange
        var users = User.GenerateSampleModels(10);

        // Act
        var typed = users
            .Where(u => users.Max(m => m.EndDate) == u.EndDate)
            .ToList();
        var dynamic = users.AsQueryable()
            .Where("@0.Max(EndDate) == EndDate", users)
            .ToList();

        // Assert
        dynamic.Should().BeEquivalentTo(typed);
    }
}