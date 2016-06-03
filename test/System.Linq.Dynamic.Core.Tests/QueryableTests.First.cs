using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void First()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.First();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, result.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, result.Id);
#endif
        }

        [Fact]
        public void FirstOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var singleResult = testListQry.FirstOrDefault();
            var defaultResult = ((IQueryable)Enumerable.Empty<User>().AsQueryable()).FirstOrDefault();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, singleResult.Id);
#endif
            Assert.Null(defaultResult);
        }

        [Fact]
        public void First_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.First().Name").Select("Id");

            //Assert
#if NET35 || NETSTANDARD
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
#else
            Assert.Equal(realResult, testResult.ToDynamicArray().Cast<Guid>());
#endif
        }
    }
}