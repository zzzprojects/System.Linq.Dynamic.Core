using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class BasicTests
    {
        #region Aggregates

        [Fact]
        public void Any()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.Any();
            var resultOne = testListOne.Any();
            var resultNone = testListNone.Any();

            //Assert
            Assert.True(resultFull);
            Assert.True(resultOne);
            Assert.False(resultNone);
        }

        [Fact]
        public void Contains()
        {
            //Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var containsList = new List<string>() {"User1", "User5", "User10"};


            //Act
            var realQuery = baseQuery.Where(x => containsList.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", containsList).Select("Id");

            //Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

        [Fact]
        public void Count()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.Count();
            var resultOne = testListOne.Count();
            var resultNone = testListNone.Count();

            //Assert
            Assert.Equal(100, resultFull);
            Assert.Equal(1, resultOne);
            Assert.Equal(0, resultNone);
        }

        [Fact]
        public void In()
        {
            //Arrange
            var testRange = Enumerable.Range(1, 100).ToArray();
            var testModels = User.GenerateSampleModels(10);
            var testModelByUsername = String.Format("Username in (\"{0}\",\"{1}\",\"{2}\")", testModels[0].UserName,
                testModels[1].UserName, testModels[2].UserName);
            var testInExpression = new int[] {2, 4, 6, 8};

            //Act
            var result1a = testRange.AsQueryable().Where("it in (2,4,6,8)").ToArray();
            var result1b = testRange.AsQueryable().Where("it in (2, 4,  6, 8)").ToArray();
            // https://github.com/NArnott/System.Linq.Dynamic/issues/52
            var result2 = testModels.AsQueryable().Where(testModelByUsername).ToArray();
            var result3 =
                testModels.AsQueryable()
                    .Where("Id in (@0, @1, @2)", testModels[0].Id, testModels[1].Id, testModels[2].Id)
                    .ToArray();
            var result4 = testRange.AsQueryable().Where("it in @0", testInExpression).ToArray();

            //Assert
            Assert.Equal(new int[] {2, 4, 6, 8}, result1a);
            Assert.Equal(new int[] {2, 4, 6, 8}, result1b);
            Assert.Equal(testModels.Take(3).ToArray(), result2);
            Assert.Equal(testModels.Take(3).ToArray(), result3);
            Assert.Equal(new int[] {2, 4, 6, 8}, result4);
        }

        #endregion

        #region Adjustors

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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(),
                result.Queryable.ToDynamicArray<User>());
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
            Assert.Equal(testList.Skip((page - 1)*pageSize).Take(pageSize).ToArray(),
                result.Queryable.ToDynamicArray<User>());
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

        #endregion

        #region Executors

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
        public void First_AsStringExpression()
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
        public void Last_AsStringExpression()
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

        [Fact]
        public void Single_AsStringExpression()
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

        #endregion
    }
}