using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Skip()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var resultFull = testListQry.Skip(0);
            var resultMinus1 = testListQry.Skip(1);
            var resultHalf = testListQry.Skip(50);
            var resultNone = testListQry.Skip(100);

            //Assert
            Assert.Equal(testList.Skip(0).ToArray(), resultFull.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(1).ToArray(), resultMinus1.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(50).ToArray(), resultHalf.Cast<User>().ToArray());
            Assert.Equal(testList.Skip(100).ToArray(), resultNone.Cast<User>().ToArray());
        }
    }
}