﻿using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Average()
    {
        // Arrange
        var incomes = User.GenerateSampleModels(100).Select(u => u.Income).ToArray();

        // Act
        var expected = incomes.Average();
        var actual = incomes.AsQueryable().Average();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Average_Nullable()
    {
        // Arrange
        var list = new List<Entity> { new Entity { Value = 1 }, new Entity { Value = 2 }, new Entity { Value = null } };
        var queryable = list.AsQueryable();


        // Act
        var expected = queryable.Average(p => p.Value);
        var actual = queryable.Average("Value");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Average_Selector()
    {
        // Arrange
        var users = User.GenerateSampleModels(100);

        // Act
        var expected = users.Average(u => u.Income);
        var result = users.AsQueryable().Average("Income");

        // Assert
        Assert.Equal(expected, result);
    }

    public class Entity
    {
        public int? Value { get; set; }
    }
}