using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers;
#if DNXCORE50 || DNX451 || DNX452
using TestToolsToXunitProxy;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace System.Linq.Dynamic.Core.Tests
{
    [TestClass]
    public class BasicTests
    {
        #region Aggregates

        [TestMethod]
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
            Assert.IsTrue(resultFull);
            Assert.IsTrue(resultOne);
            Assert.IsFalse(resultNone);
        }

        [TestMethod]
        public void Contains()
        {
            //Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var containsList = new List<string>() { "User1", "User5", "User10" };


            //Act
            var realQuery = baseQuery.Where(x => containsList.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", containsList).Select("Id");

            //Assert
            CollectionAssert.AreEqual(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

        [TestMethod]
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
            Assert.AreEqual(100, resultFull);
            Assert.AreEqual(1, resultOne);
            Assert.AreEqual(0, resultNone);
        }

        [TestMethod]
        public void In()
        {
            //Arrange
            var testRange = Enumerable.Range(1, 100).ToArray();
            var testModels = User.GenerateSampleModels(10);
            var testModelByUsername = String.Format("Username in (\"{0}\",\"{1}\",\"{2}\")", testModels[0].UserName, testModels[1].UserName, testModels[2].UserName);
            var testInExpression = new int[] { 2, 4, 6, 8 };

            //Act
            var result1 = testRange.AsQueryable().Where("it in (2,4,6,8)").ToArray();
            var result2 = testModels.AsQueryable().Where(testModelByUsername).ToArray();
            var result3 = testModels.AsQueryable().Where("Id in (@0, @1, @2)", testModels[0].Id, testModels[1].Id, testModels[2].Id).ToArray();
            var result4 = testRange.AsQueryable().Where("it in @0", testInExpression).ToArray();

            //Assert
            CollectionAssert.AreEqual(new int[] { 2, 4, 6, 8 }, result1);
            CollectionAssert.AreEqual(testModels.Take(3).ToArray(), result2);
            CollectionAssert.AreEqual(testModels.Take(3).ToArray(), result3);
            CollectionAssert.AreEqual(new int[] { 2, 4, 6, 8 }, result4);
        }

        #endregion

        #region Adjustors

        [TestMethod]
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
            CollectionAssert.AreEqual(testList.Skip(0).ToArray(), resultFull.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Skip(1).ToArray(), resultMinus1.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Skip(50).ToArray(), resultHalf.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Skip(100).ToArray(), resultNone.Cast<User>().ToArray());
        }

        [TestMethod]
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
            CollectionAssert.AreEqual(testList.Take(100).ToArray(), resultFull.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Take(99).ToArray(), resultMinus1.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Take(50).ToArray(), resultHalf.Cast<User>().ToArray());
            CollectionAssert.AreEqual(testList.Take(1).ToArray(), resultOne.Cast<User>().ToArray());
        }

        [TestMethod]
        public void Reverse()
        {
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = BasicQueryable.Reverse(testListQry);

            //Assert
            CollectionAssert.AreEqual(Enumerable.Reverse(testList).ToArray(), result.Cast<User>().ToArray());
        }

        #endregion

        #region Executors

        [TestMethod]
        public void Single()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.Take(1).Single();

            //Assert
#if NET35
            Assert.AreEqual(testList[0].Id, result.GetDynamicProperty<Guid>("Id"));
#else
            Assert.AreEqual(testList[0].Id, result.Id);
#endif
        }

        [TestMethod]
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
            Assert.AreEqual(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.AreEqual(testList[0].Id, singleResult.Id);
#endif
            Assert.IsNull(defaultResult);
        }

        [TestMethod]
        public void First()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var result = testListQry.First();

            //Assert
#if NET35
            Assert.AreEqual(testList[0].Id, result.GetDynamicProperty<Guid>("Id"));
#else
            Assert.AreEqual(testList[0].Id, result.Id);
#endif
        }

        [TestMethod]
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
            Assert.AreEqual(testList[0].Id, singleResult.GetDynamicProperty<Guid>("Id"));
#else
            Assert.AreEqual(testList[0].Id, singleResult.Id);
#endif
            Assert.IsNull(defaultResult);
        }


        [TestMethod]
        public void First_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            IQueryable testListQry = testList.AsQueryable();

            //Act
            var realResult = testList.OrderBy(x => x.Roles.First().Name).Select(x => x.Id).ToArray();
            var testResult = testListQry.OrderBy("Roles.First().Name").Select("Id");

            //Assert
#if NET35 || DNX452 || DNXCORE50
            CollectionAssert.AreEqual(realResult, testResult.Cast<Guid>().ToArray());
#else
            CollectionAssert.AreEqual(realResult, testResult.ToDynamicArray());
#endif
        }

        [TestMethod]
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
#if NET35 || DNX452 || DNXCORE50
            CollectionAssert.AreEqual(realResult, testResult.Cast<Guid>().ToArray());
#else
            CollectionAssert.AreEqual(realResult, testResult.ToDynamicArray());
#endif
        }


        #endregion

    }

}
