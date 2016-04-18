using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.ConsoleTestApp
{
    public static class E
    {
        public static IQueryable GroupBy2<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keyLambda2)
        {
            //LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, "new (Profile.Age)", null);
            LambdaExpression x = (LambdaExpression)keyLambda2;

            //return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
            //    Expression.Call(
            //        typeof(Queryable), "GroupBy",
            //        new Type[] { source.ElementType, keySelector.Body.Type },
            //        new Expression[] { source.Expression, Expression.Quote(keySelector) }
            //        ));


            return source.Provider.CreateQuery(
               Expression.Call(
                   typeof(Queryable), "GroupBy",
                   new Type[] { source.ElementType, x.Body.Type },
                   new Expression[] { source.Expression, Expression.Quote(x) }));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("--start");

            GroupByAndSelect_TestDynamicSelectMember();
            //Select();
            //TestDyn();
            //ExpressionTests_Enum();
            //Where();
            //ExpressionTests_Sum();

            Console.WriteLine("--end");
        }

        private static void GroupByAndSelect_TestDynamicSelectMember()
        {
            var testList = User.GenerateSampleModels(51).Where(u => u.Profile.Age < 23);
            var qry = testList.AsQueryable();

            var rrrr = qry.GroupBy2(x => new { x.Profile.Age });
            var ll = rrrr.ToDynamicList();

            var byAgeReturnAllReal = qry.GroupBy(x => new { x.Profile.Age }).ToList();
            var r1 = byAgeReturnAllReal[0];

            //var byAgeReturnOK = qry.GroupBy("Profile.Age").ToDynamicList();

            // -		[0]	{System.Linq.Grouping<<>f__AnonymousType0<int?>, System.Linq.Dynamic.Core.Tests.Helpers.Models.User>}	object {System.Linq.Grouping<<>f__AnonymousType0<int?>, System.Linq.Dynamic.Core.Tests.Helpers.Models.User>}
            var byAgeReturnAll = qry.GroupBy("new (Profile.Age)").OrderBy("Key.Age").ToDynamicList();
            var q1 = byAgeReturnAll[0];

            var k = q1.Key;
            int? age = k.Age;

            foreach (var x in byAgeReturnAllReal.OrderBy(a => a.Key.Age))
            {
                Console.WriteLine($"age={x.Key.Age} : users={x.ToList().Count}");
            }

            foreach (var x in byAgeReturnAll)
            {
                Console.WriteLine($"age={x.Key.Age} : users={x}");
            }
        }

        private static void TestDyn()
        {
            var user = new User { UserName = "x" };

            dynamic userD = user;
            string username = userD.UserName;

            Console.WriteLine("..." + username);
        }

        public static void ExpressionTests_Enum()
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
            int idx = 0;
            var ar1 = result1.ToArray();
            foreach (var c in new[] { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3 })
            {
                Console.Write("*");
                Write((int)c, (int)ar1[idx]);
                idx++;
            }

            idx = 0;
            ar1 = result2.ToArray();
            foreach (var c in new[] { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3 })
            {
                Console.Write("*");
                Write((int)c, (int)ar1[idx]);
                idx++;
            }

            Write((int)TestEnum.Var5, (int)result3.Single());
            Write((int)TestEnum.Var5, (int)result4.Single());
            Write((int)TestEnum.Var5, (int)result5.Single());
        }

        public static void Where()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();


            //Act
            //var xxx = DynamicQueryable
            var userById = qry.Where("Id=@0", testList[10].Id);
            var userByUserName = qry.Where("UserName=\"User5\"");
            var nullProfileCount = qry.Where("Profile=null");
            var userByFirstName = qry.Where("Profile!=null && Profile.FirstName=@0", testList[1].Profile.FirstName);


            //Assert
            Write(testList[10], userById.Single());
            Write(testList[5], userByUserName.Single());
            Write(testList.Count(x => x.Profile == null), nullProfileCount.Count());
            Write(testList[1], userByFirstName.Single());
        }

        public static void ExpressionTests_Sum()
        {
            //Arrange
            int[] initValues = { 1, 2, 3, 4, 5 };
            var qry = initValues.AsQueryable().Select(x => new { strValue = "str", intValue = x }).GroupBy(x => x.strValue);

            //Act
            var result = qry.Select("Sum(intValue)").AsEnumerable().ToArray()[0];

            //Assert
            Write(15, result);
        }

        public static void Select()
        {
            //Arrange
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            IEnumerable rangeResult = range.AsQueryable().Select("it * it");
            var userNames = qry.Select("UserName");
            var userFirstName = qry.Select("new (UserName, Profile.FirstName as MyFirstName)");
            var userRoles = qry.Select("new (UserName, Roles.Select(Id) AS RoleIds)");


            //Assert
            WriteArray(range.Select(x => x * x).ToArray(), rangeResult.Cast<int>().ToArray());
            WriteArray(testList.Select(x => x.UserName).ToArray(), userNames.ToDynamicArray());
            WriteArray(testList.Select(x => "{UserName=" + x.UserName + ", MyFirstName=" + x.Profile.FirstName + "}").ToArray(), userFirstName.AsEnumerable().Select(x => x.ToString()).ToArray());

            Guid[] check = testList[0].Roles.Select(x => x.Id).ToArray();
            //dynamic f = userRoles.First();
            //Guid[] ids = f.RoleIds.ToArray();

            var userRole = userRoles.First();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>" + userRole.ToString());

            PropertyInfo[] props = userRole.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Console.WriteLine(">>>>>>>>>>>>>>>>>> GetProperties = {0}", string.Join(", ", props.Select(p => p.Name)));
            Console.WriteLine(">>>>>>>>>>>>>>>>>> GetPropertyValues = {0}", string.Join(", ", props.Select(p => p.GetValue(userRole, null))));

            string name = userRole.UserName;

            Guid[] result = Enumerable.ToArray(userRole.RoleIds ?? new object[0]);

            WriteArray(check, result);
        }

        private static void Write<T>(T check, T result) where T : class
        {
            Console.WriteLine("> '{0}'", check == result);
        }

        private static void Write(int check, int result)
        {
            Console.WriteLine("> {0} == {1} = '{2}'", check, result, check == result);
        }

        private static void WriteArray<T>(T[] check, T[] result) where T : class
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }

        private static void WriteArray(Guid[] check, Guid[] result)
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }

        private static void WriteArray(int[] check, int[] result)
        {
            for (int i = 0; i < check.Length; i++)
            {
                Console.WriteLine("> {0} : c={1}, r={2}  '{3}'", i, check[i], result[i], check[i] == result[i]);
            }
        }
    }
}