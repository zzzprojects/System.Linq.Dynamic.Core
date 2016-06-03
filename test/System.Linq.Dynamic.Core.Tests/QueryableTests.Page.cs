using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Page()
        {
            //Arrange
            const int total = 35;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Page(page, pageSize);

            //Assert
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        [Fact]
        public void Page_TSource()
        {
            //Arrange
            const int total = 35;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            var testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Page(page, pageSize);

            //Assert
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
        }

        [Fact]
        public void PageResult()
        {
            //Arrange
            const int total = 44;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.PageResult(page, pageSize);

            //Assert
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.RowCount);
            Assert.Equal(5, result.PageCount);
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }

        [Fact]
        public void PageResult_TSource()
        {
            //Arrange
            const int total = 44;
            const int page = 2;
            const int pageSize = 10;
            var testList = User.GenerateSampleModels(total);
            var testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.PageResult(page, pageSize);

            //Assert
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.RowCount);
            Assert.Equal(5, result.PageCount);
            Assert.Equal(testList.Skip((page - 1) * pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }
    }
}