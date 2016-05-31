using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class BasicTests
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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(), result.ToDynamicArray<User>());
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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(), result.Queryable.ToDynamicArray<User>());
        }

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
            var defaultResult = ((IQueryable) Enumerable.Empty<User>().AsQueryable()).SingleOrDefault();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, singleResult.Id);
#endif
            Assert.Null(defaultResult);
        }

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
            var defaultResult = ((IQueryable) Enumerable.Empty<User>().AsQueryable()).FirstOrDefault();

            //Assert
#if NET35
            Assert.Equal(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.Equal(testList[0].Id, singleResult.Id);
#endif
            Assert.Null(defaultResult);
        }

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

    }
}