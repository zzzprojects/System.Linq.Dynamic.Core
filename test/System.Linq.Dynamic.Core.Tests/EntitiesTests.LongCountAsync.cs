﻿#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using System.Data.Entity;
using EntityFramework.DynamicLinq;
#endif
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public async Task Entities_LongCountAsync_Predicate_Args()
    {
        // Arrange
        const string search = "a";

        Expression<Func <Blog, bool>> predicate = b => b.Name.Contains(search);

#if EFCORE
        var expected = await EntityFrameworkQueryableExtensions.LongCountAsync(_context.Blogs, predicate);
#else
        var expected = await QueryableExtensions.LongCountAsync(_context.Blogs, predicate);
#endif

        //Act
        long result = (_context.Blogs as IQueryable).LongCount("Name.Contains(@0)", search);

        //Assert
        Assert.Equal(expected, result);
    }
}