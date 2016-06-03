using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Last()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.Last();
            var result = testListQry.Last();

            //Assert
#if NET35
            Assert.Equal(realResult.Id, result.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(realResult.Id, result.Id);
#endif
        }

        [Fact]
        public void LastOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.LastOrDefault();
            var singleResult = testListQry.LastOrDefault();
            var defaultResult = Enumerable.Empty<User>().AsQueryable().FirstOrDefault();

            //Assert
#if NET35
            Assert.Equal(realResult.Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(realResult.Id, singleResult.Id);
#endif
            Assert.Null(defaultResult);
        }

        [Fact]
        public void Last_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.Last().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.Last().Name").Select("Id");

            //Assert
#if NET35 || NETSTANDARD
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
#else
            Assert.Equal(realResult, testResult.ToDynamicArray().Cast<Guid>());
#endif
        }
    }
}