﻿using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Sum()
    {
        // Arrange
        var incomes = User.GenerateSampleModels(100).Select(u => u.Income);

        // Act
        var expected = incomes.Sum();
        var actual = incomes.AsQueryable().Sum();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sum_Selector()
    {
        // Arrange
        var users = User.GenerateSampleModels(100);

        // Act
        var expected = users.Sum(u => u.Income);
        var result = users.AsQueryable().Sum("Income");

        // Assert
        Assert.Equal(expected, result);
    }
}