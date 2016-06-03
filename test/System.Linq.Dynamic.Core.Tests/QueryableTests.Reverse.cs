using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Reverse()
        {
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Reverse();

            //Assert
            Assert.Equal(testList.Reverse().ToArray(), result.Cast<User>().ToArray());
        }
    }
}