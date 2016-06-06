using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void FirstOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var queryable = testList.AsQueryable();

            //Act
            var expected = Queryable.FirstOrDefault(queryable);
            var expectedDefault = Queryable.FirstOrDefault(Enumerable.Empty<User>().AsQueryable());

            var result = (queryable as IQueryable).FirstOrDefault();
            var resultDefault = (Enumerable.Empty<User>().AsQueryable() as IQueryable).FirstOrDefault();

            Assert.Equal(expected, result);
            Assert.Null(expectedDefault);
            Assert.Null(resultDefault);
        }
    }
}