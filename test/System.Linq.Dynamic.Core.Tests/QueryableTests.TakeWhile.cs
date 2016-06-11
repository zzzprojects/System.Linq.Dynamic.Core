using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void TakeWhile_Predicate()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var expected = testList.TakeWhile(u => u.Income > 1000);
            var result = testListQry.TakeWhile("Income > 1000");

            //Assert
            Assert.Equal(expected.ToArray(), result.Cast<User>().ToArray());
        }

        [Fact]
        public void TakeWhile_Predicate_Args()
        {
            const int income = 1000;

            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var expected = testList.TakeWhile(u => u.Income > income);
            var result = testListQry.TakeWhile("Income > @0", income);

            //Assert
            Assert.Equal(expected.ToArray(), result.Cast<User>().ToArray());
        }
    }
}