using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void LongCount()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.LongCount();
            var resultOne = testListOne.LongCount();
            var resultNone = testListNone.LongCount();

            //Assert
            Assert.Equal(100, resultFull);
            Assert.Equal(1, resultOne);
            Assert.Equal(0, resultNone);
        }

        [Fact]
        public void LongCount_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            long expected = queryable.LongCount(u => u.Income > 50);
            long result = queryable.LongCount("Income > 50");

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LongCount_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            long expected = queryable.LongCount(u => u.Income > value);
            long result = queryable.LongCount("Income > @0", value);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LongCount_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.LongCount()).ToArray();
            var result = queryable.Select("Roles.LongCount()").ToDynamicArray<long>();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LongCount_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.LongCount(r => r.Name.Contains(search)) > 0).ToArray();
            var result = queryable.Where("Roles.LongCount(Name.Contains(@0)) > 0", search).ToArray();

            Assert.Equal(expected, result);
        }
    }
}
