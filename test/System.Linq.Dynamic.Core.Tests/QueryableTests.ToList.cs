using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void ToDynamicList()
        {
            // Arrange
            var testList = User.GenerateSampleModels(3);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.ToList().First().Name).Select(x => x.Id);
            var testResult = testListQry.OrderBy("Roles.ToList().First().Name").Select("Id");

            // Assert
            Assert.Equal(realResult.ToArray(), testResult.ToDynamicList().Cast<Guid>());
        }

        [Fact]
        public async Task ToDynamicListAsync()
        {
            // Arrange
            var testList = User.GenerateSampleModels(3);
            IQueryable testListQry = testList.AsQueryable();

            // Act
            var realResult = testList.OrderBy(x => x.Roles.ToList().First().Name).Select(x => x.Id);
            var testResult = testListQry.OrderBy("Roles.ToList().First().Name").Select("Id");

            // Assert
            var dynamicResult = await testResult.ToDynamicListAsync();
            Assert.Equal(realResult.ToArray(), dynamicResult.Cast<Guid>());
        }
    }
}