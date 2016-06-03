using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Take()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var resultFull = testListQry.Take(100);
            var resultMinus1 = testListQry.Take(99);
            var resultHalf = testListQry.Take(50);
            var resultOne = testListQry.Take(1);

            //Assert
            Assert.Equal(testList.Take(100).ToArray(), resultFull.Cast<User>().ToArray());
            Assert.Equal(testList.Take(99).ToArray(), resultMinus1.Cast<User>().ToArray());
            Assert.Equal(testList.Take(50).ToArray(), resultHalf.Cast<User>().ToArray());
            Assert.Equal(testList.Take(1).ToArray(), resultOne.Cast<User>().ToArray());
        }
    }
}