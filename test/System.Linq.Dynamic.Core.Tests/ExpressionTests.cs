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
    public class ExpressionTests
    {
        [TestMethod]
        public void ExpressionTests_Sum()
        {
            //Arrange
            int[] initValues = { 1, 2, 3, 4, 5 };
            var qry = initValues.AsQueryable().Select(x => new { strValue = "str", intValue = x }).GroupBy(x => x.strValue);

            //Act
            var result = qry.Select("Sum(intValue)").AsEnumerable().ToArray()[0];

            //Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ExpressionTests_Sum2()
        {
            //Arrange
            var initValues = new[]
            {
                new SimpleValuesModel { FloatValue = 1 },
                new SimpleValuesModel { FloatValue = 2 },
                new SimpleValuesModel { FloatValue = 3 },
            };

            var qry = initValues.AsQueryable();

            //Act
            var result = qry.Select("FloatValue").Sum();
            var result2 = ((IQueryable<float>)qry.Select("FloatValue")).Sum();

            //Assert
            Assert.AreEqual(6.0f, result);
            Assert.AreEqual(6.0f, result2);
        }

        [TestMethod]
        public void ExpressionTests_ContainsGuid()
        {
            //Arrange
            var userList = User.GenerateSampleModels(5, false);
            var userQry = userList.AsQueryable();

            var failValues = new List<Guid>
            {
                new Guid("{22222222-7651-4045-962A-3D44DEE71398}"),
                new Guid("{33333333-8F80-4497-9125-C96DEE23037D}"),
                new Guid("{44444444-E32D-4DE1-8F1C-A144C2B0424D}")
            };
            var successValues = failValues.Concat(new[] { userList[0].Id }).ToArray();


            //Act
            var found1 = userQry.Where("Id in @0", successValues);
            var found2 = userQry.Where("@0.Contains(Id)", successValues);
            var notFound1 = userQry.Where("Id in @0", failValues);
            var notFound2 = userQry.Where("@0.Contains(Id)", failValues);

            //Assert
#if NET35
            Assert.AreEqual(userList[0].Id, ((User)found1.Single()).Id);
            Assert.AreEqual(userList[0].Id, ((User)found2.Single()).Id);
#else
            Assert.AreEqual(userList[0].Id, found1.Single().Id);
            Assert.AreEqual(userList[0].Id, found2.Single().Id);
#endif
            Assert.IsFalse(notFound1.Any());
            Assert.IsFalse(notFound2.Any());
        }

        [TestMethod]
        public void ExpressionTests_Enum()
        {
            //Arrange
            var lst = new List<TestEnum> { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3, TestEnum.Var4, TestEnum.Var5, TestEnum.Var6 };
            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.Where("it < TestEnum.Var4");
            var result2 = qry.Where("TestEnum.Var4 > it");
            var result3 = qry.Where("it = Var5");
            var result4 = qry.Where("it = @0", TestEnum.Var5);
            var result5 = qry.Where("it = @0", 8);

            //Assert
            CollectionAssert.AreEqual(new[] { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3 }, result1.ToArray());
            CollectionAssert.AreEqual(new[] { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3 }, result2.ToArray());
            Assert.AreEqual(TestEnum.Var5, result3.Single());
            Assert.AreEqual(TestEnum.Var5, result4.Single());
            Assert.AreEqual(TestEnum.Var5, result5.Single());
        }

        [TestMethod]
        public void ExpressionTests_CompareWithGuid()
        {
            //Arrange
            var lst = new List<Guid>
            {
                new Guid("{1AF7AD2B-7651-4045-962A-3D44DEE71398}"),
                new Guid("{99610563-8F80-4497-9125-C96DEE23037D}"),
                new Guid("{0A191E77-E32D-4DE1-8F1C-A144C2B0424D}")
            };
            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.Where("it = \"0A191E77-E32D-4DE1-8F1C-A144C2B0424D\"");
            var result2 = qry.Where("\"0A191E77-E32D-4DE1-8F1C-A144C2B0424D\" = it");
            var result3 = qry.Where("it = @0", "0A191E77-E32D-4DE1-8F1C-A144C2B0424D");
            var result4 = qry.Where("it = @0", lst[2]);

            //Assert
            Assert.AreEqual(lst[2], result1.Single());
            Assert.AreEqual(lst[2], result2.Single());
            Assert.AreEqual(lst[2], result3.Single());
            Assert.AreEqual(lst[2], result4.Single());
        }

        [TestMethod]
        public void ExpressionTests_Shift()
        {
            //Arrange
            var lst = new List<int> { 10, 20, 30 };
            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.Select("it << 1");
            var result2 = qry.Select("it >> 1");
            var result3 = qry.Where("it << 2 = 80");

            //Assert
            CollectionAssert.AreEqual(new object[] { 20, 40, 60 }, result1.Cast<object>().ToArray());
            CollectionAssert.AreEqual(new object[] { 5, 10, 15 }, result2.Cast<object>().ToArray());
            Assert.AreEqual(20, result3.Single());
        }

        [TestMethod]
        public void ExpressionTests_LogicalAndOr()
        {
            //Arrange
            var lst = new List<int> { 0x20, 0x21, 0x30, 0x31, 0x41 };
            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.Where("(it & 1) > 0");
            var result2 = qry.Where("(it & 32) > 0");

            //Assert
            CollectionAssert.AreEqual(new[] { 0x21, 0x31, 0x41 }, result1.ToArray());
            CollectionAssert.AreEqual(qry.Where(x => (x & 32) > 0).ToArray(), result2.ToArray());
        }

        [TestMethod]
        public void ExpressionTests_Uri()
        {
            //Arrange
            var lst = new List<Uri>
            {
                new Uri("http://127.0.0.1"),
                new Uri("http://192.168.1.1"),
                new Uri("http://127.0.0.1")
            };

            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.AsQueryable().Where("it = @0", new Uri("http://127.0.0.1"));

            //Assert
            Assert.AreEqual(result1.Count(), 2);
        }

        [TestMethod]
        public void ExpressionTests_DistinctBy()
        {
            //Arrange
            //Makes a Distinct By Tuple.Item1 but returns a full Tuple
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var p = lst.AsQueryable() as IQueryable;
            var qry = p.GroupBy("Item1", "it").Select("it.Max(it.Item3)");


            //Act
            var qry1 = p.Where("@0.Any(it == parent.Item3)", qry);
            var qry2 = p.Where("@0.Any($ == ^.Item3)", qry);
            var qry3 = p.Where("@0.Any($ == ~.Item3)", qry);

            //Assert
            Assert.AreEqual(qry1.Count(), 2);
            Assert.AreEqual(qry2.Count(), 2);
            Assert.AreEqual(qry3.Count(), 2);
        }

        [TestMethod]
        public void ExpressionTests_ContextKeywordsAndSymbols()
        {
            try
            {
                //Arrange
                int[] values = new[] { 1, 2, 3, 4, 5 };

                //Act
                GlobalConfig.AreContextKeywordsEnabled = false;
                Helper.ExpectException<ParseException>(() => values.AsQueryable().Where("it = 2"));
                Helper.ExpectException<ParseException>(() => values.AsQueryable().Where("root = 2"));
                values.AsQueryable().Where("$ = 2");
                values.AsQueryable().Where("~ = 2");
                GlobalConfig.AreContextKeywordsEnabled = true;

                var qry1 = values.AsQueryable().Where("it = 2");
                var qry2 = values.AsQueryable().Where("$ = 2");

                //Assert
                Assert.AreEqual(2, qry1.Single());
                Assert.AreEqual(2, qry2.Single());
            }
            finally
            {
                GlobalConfig.AreContextKeywordsEnabled = true;
            }
        }

        [TestMethod]
        public void ExpressionTests_FirstOrDefault()
        {
            //Arrange
            var testList = User.GenerateSampleModels(2);
            testList[0].Roles.Clear();

            var testListQry = testList.AsQueryable();


            //Act

            //find first user that has the role of admin
            var realSingleResult = testListQry.Where(x => x.Roles.FirstOrDefault(y => y.Name == "Admin") != null).FirstOrDefault();
            var testSingleResult = testListQry.Where("Roles.FirstOrDefault(Name = \"Admin\") != null").FirstOrDefault();

            testList[1].Roles.Clear(); //remove roles so the next set fails
            var realSingleFailResult = testListQry.Where(x => x.Roles.FirstOrDefault(y => y.Name == "Admin") != null).FirstOrDefault();
            var testSingleFailResult = testListQry.Where("Roles.FirstOrDefault(Name = \"Admin\") != null").FirstOrDefault();

            //Assert
            Assert.AreEqual(realSingleResult, testSingleResult);
            Assert.AreEqual((User)realSingleFailResult, (User)testSingleFailResult);
        }
    }
}