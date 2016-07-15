using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Distinct()
        {
            //Arrange
            var list = new[] { 1, 2, 2, 3 };
            IQueryable queryable = list.AsQueryable();

            //Act
            var expected = Queryable.Distinct(list.AsQueryable());
            var result = queryable.Distinct();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Distinct_Dynamic_1()
        {
            //Arrange
            var list = new[]
            {
                new { x = "a", list = new[] { 1, 2, 2, 3 } },
                new { x = "b", list = new[] { 5, 6, 6, 8 } },
            };
            IQueryable queryable = list.AsQueryable();

            //Act
            var expected = list.Select(l => l.list.Distinct()).ToArray<object>();
            var result = queryable.Select("list.Distinct()").ToDynamicArray();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Distinct_Dynamic_2()
        {
            //Arrange
            var list = new[]
            {
                new User { UserName = "a", Income = 1 },
                new User { UserName = "a", Income = 2 },
                new User { UserName = "b", Income = 1 }
            };

            //Act
            var expected = list.Select(u => u.UserName).Distinct().ToArray();

            IQueryable queryable = list.AsQueryable();
            var result = queryable.Select("UserName").Distinct().ToDynamicArray<string>();

            //Assert
            Assert.Equal(expected, result);
        }
    }
}