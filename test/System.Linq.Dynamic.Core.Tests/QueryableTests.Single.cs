using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Single()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Take(1).Single();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, result.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, result.Id);
#endif
        }

        [Fact]
        public void SingleOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var singleResult = testListQry.Take(1).SingleOrDefault();
            var defaultResult = ((IQueryable)Enumerable.Empty<User>().AsQueryable()).SingleOrDefault();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, singleResult.Id);
#endif
            Assert.Null(defaultResult);
        }

        [Fact]
        public void Single_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(1);
            while (testList[0].Roles.Count > 1) testList[0].Roles.RemoveAt(0);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            dynamic realResult = testList.OrderBy(x => x.Roles.Single().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.Single().Name").Select("Id");

            //Assert
#if NET35 || NETSTANDARD
            Assert.Equal(realResult, testResult.Cast<Guid>().ToArray());
#else
            Assert.Equal(realResult, testResult.ToDynamicArray());
#endif
        }
    }
}