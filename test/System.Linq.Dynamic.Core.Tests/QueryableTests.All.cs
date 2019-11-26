using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void All_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.All(u => u.Income > 50);
            bool result = queryable.All("Income > 50");

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void All_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.All(u => u.Income > value);
            bool result = queryable.All("Income > @0", value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void All_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.All(r => r.Name != null)).ToArray();
            var result = queryable.Select("Roles.All(Name != null)").ToDynamicArray<bool>();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void All_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.All(r => r.Name.Contains(search))).ToArray();
            var result = queryable.Where("Roles.All(Name.Contains(@0))", search).ToArray();

            Assert.Equal(expected, result);
        }
    }
}
