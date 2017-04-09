using NFluent;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void FirstOrDefault()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = Queryable.FirstOrDefault(queryable);
            var expectedDefault = Queryable.FirstOrDefault(Enumerable.Empty<User>().AsQueryable());

            var result = (queryable as IQueryable).FirstOrDefault();
            var resultDefault = (Enumerable.Empty<User>().AsQueryable() as IQueryable).FirstOrDefault();

            // Assert
            Check.That(result).Equals(expected);
            Assert.Null(expectedDefault);
            Assert.Null(resultDefault);
        }

        [Fact]
        public void FirstOrDefault_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.FirstOrDefault(u => u.Income > 1000);
            var result = queryable.FirstOrDefault("Income > 1000");

            // Assert
            Check.That(result).Equals(expected);
        }

        [Fact]
        public void FirstOrDefault_Predicate_WithArgs()
        {
            const int value = 1000;

            // Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.FirstOrDefault(u => u.Income > value);
            var result = queryable.FirstOrDefault("Income > @0", value);

            // Assert
            Check.That(result).Equals(expected);
        }

        [Fact]
        public void FirstOrDefault_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var result = testListQry.OrderBy("Roles.FirstOrDefault().Name").Select("Id");

            // Assert
            Check.That(result.ToDynamicArray().Cast<Guid>()).ContainsExactly(realResult);
        }
    }
}