using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleAppEF2.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleAppEF2
{
    static class Program
    {
        class C : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
        {
            public HashSet<Type> GetCustomTypes()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var set = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies))
                {
                    typeof(TestContext)
                };

                return set;
            }
        }

        private static object GetObj()
        {
            return new
            {
                Id = 5,
                Value = 400
            };
        }

        class X : DynamicClass
        {

        }

        private static IQueryable GetQueryable()
        {
            var random = new Random((int)DateTime.Now.Ticks);

            var jt = typeof(JToken);

            var em = jt.GetTypeInfo().GetDeclaredMethods("op_Explicit");
            var im = jt.GetTypeInfo().GetDeclaredMethods("op_Explicit");

            var j = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            };

            //(j["Id"] as JValue).Value

            IQueryable jarray = new[] { j }.AsQueryable();
            var jresult = jarray.Select("new (int(Id) as Id, string(Name) as Name)");

            var an = jresult.Any("Id > 4");


            var dx = new X();
            dx["Id"] = 5;

            IQueryable srcDX = new[] { dx }.AsQueryable();
            var b = srcDX.Select("new (Id.ToString() as Id)");
            var anyDX = b.Any("int.Parse(Id) > 4");

            var x = Enumerable.Range(0, 10).Select(i => new
            {
                Id = i,
                Value = random.Next()
            }).AsQueryable();

            //var any = x.Any("Id > 4");

            //var obj = new
            //{
            //    Id = 5,
            //    Value = random.Next()
            //};
            //var x2 = Enumerable.Range(0, 1).Select(_ => obj).AsQueryable();
            //var any2 = x.Any("Id > 4");

            //var o = GetObj();
            //var t = o.GetType();
            //IQueryable source = new[] { o }.AsQueryable();
            //// source.ElementType = t;

            //var x2b = new[] { o }.AsQueryable();
            //var any2function = x2b.Any(null, "Id > 4", t);

            //var any2b = x2b.Any("Id > 4");

            //var x3 = new[] { obj }.AsQueryable();
            //var any3 = x3.Any("Id > 4");

            return x.Select("new (it as Id, @0 as Value)", random.Next());
            // return x.AsQueryable(); //x.AsQueryable().Select("new (Id, Value)");
        }

        public static IQueryable Transform(this IQueryable source, Type resultType)
        {
            var resultProperties = resultType.GetProperties().Where(p => p.CanWrite);

            ParameterExpression s = Expression.Parameter(source.ElementType, "s");

            var memberBindings =
                resultProperties.Select(p =>
                    Expression.Bind(resultType.GetMember(p.Name)[0], Expression.Property(s, p.Name))).OfType<MemberBinding>();

            Expression memberInit = Expression.MemberInit(
                Expression.New(resultType),
                memberBindings
            );

            var memberInitLambda = Expression.Lambda(memberInit, s);

            var typeArgs = new[]
            {
                source.ElementType,
                memberInit.Type
            };

            var mc = Expression.Call(typeof(Queryable), "Select", typeArgs, source.Expression, memberInitLambda);

            var query = source.Provider.CreateQuery(mc);

            return query;
        }

        public static IQueryable<T> EmptyQueryByExample<T>(this T _) => Enumerable.Empty<T>().AsQueryable();


        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source, Expression expression, Type t = null)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                ? operatorMethodInfo.MakeGenericMethod(t == null ? source.ElementType : t, typeof(TResult))
                : operatorMethodInfo.MakeGenericMethod(t == null ? source.ElementType : t);

            var optimized = Expression.Call(null, operatorMethodInfo, source.Expression, expression);
            return source.Provider.Execute<TResult>(optimized);
        }

        static void Main(string[] args)
        {
            IQueryable qry = GetQueryable();

            var result = qry.Select("it").OrderBy("Value");
            try
            {
                Console.WriteLine("result {0}", JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception)
            {
                // Console.WriteLine(e);
            }

            var all = new
            {
                test1 = new List<int> { 1, 2, 3 }.ToDynamicList(typeof(int)),
                test2 = new List<dynamic> { 4, 5, 6 }.ToDynamicList(typeof(int)),
                test3 = new List<object> { 7, 8, 9 }.ToDynamicList(typeof(int))
            };
            Console.WriteLine("all {0}", JsonConvert.SerializeObject(all, Formatting.Indented));

            var anyTest = new[]
            {
                new { id = "1", values =new [] { 1, 2, 3 } },
                new { id = "2", values =new [] { 1, 4 } },
                new { id = "3", values =new [] { 9, 5 } }
            }.AsQueryable();

            var any1 = anyTest.Where(x => x.values.Contains(1));
            Console.WriteLine("any1 {0}", JsonConvert.SerializeObject(any1, Formatting.Indented));

            var any2 = anyTest.Where("values.Contains(1)");
            Console.WriteLine("any2 {0}", JsonConvert.SerializeObject(any2, Formatting.Indented));

            var config = new ParsingConfig
            {
                CustomTypeProvider = new C()
            };

            var dateLastModified = new DateTime(2018, 1, 15);

            var context = new TestContext();
            context.Cars.Add(new Car { Brand = "Ford", Color = "Blue", Vin = "yes", Year = "2017", DateLastModified = dateLastModified });
            context.Cars.Add(new Car { Brand = "Fiat", Color = "Red", Vin = "yes", Year = "2016", DateLastModified = dateLastModified.AddDays(1) });
            context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "no", Year = "1979", DateLastModified = dateLastModified.AddDays(2) });
            context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "a%bc", Year = "1979", DateLastModified = dateLastModified.AddDays(3) });
            context.SaveChanges();

            context.Brands.Add(new Brand { BrandType = "Ford", BrandName = "Fiesta" });
            context.Brands.Add(new Brand { BrandType = "Fiat", BrandName = "Panda" });
            context.Brands.Add(new Brand { BrandType = "Alfa", BrandName = "Romeo" });
            context.SaveChanges();

            var carDateLastModified = context.Cars.Where(config, "DateLastModified > \"2018-01-16\"");
            Console.WriteLine("carDateLastModified {0}", JsonConvert.SerializeObject(carDateLastModified, Formatting.Indented));

            //var carFirstOrDefault = context.Cars.Where(config, "Brand == \"Ford\"");
            //Console.WriteLine("carFirstOrDefault {0}", JsonConvert.SerializeObject(carFirstOrDefault, Formatting.Indented));

            //var carsLike1 =
            //    from c in context.Cars
            //    where EF.Functions.Like(c.Brand, "%a%")
            //    select c;
            //Console.WriteLine("carsLike1 {0}", JsonConvert.SerializeObject(carsLike1, Formatting.Indented));

            //var cars2Like = context.Cars.Where(c => EF.Functions.Like(c.Brand, "%a%"));
            //Console.WriteLine("cars2Like {0}", JsonConvert.SerializeObject(cars2Like, Formatting.Indented));

            //var dynamicCarsLike1 = context.Cars.Where(config, "TestContext.Like(Brand, \"%a%\")");
            //Console.WriteLine("dynamicCarsLike1 {0}", JsonConvert.SerializeObject(dynamicCarsLike1, Formatting.Indented));

            //var dynamicCarsLike2 = context.Cars.Where(config, "TestContext.Like(Brand, \"%d%\")");
            //Console.WriteLine("dynamicCarsLike2 {0}", JsonConvert.SerializeObject(dynamicCarsLike2, Formatting.Indented));

            //var dynamicFunctionsLike1 = context.Cars.Where(config, "DynamicFunctions.Like(Brand, \"%a%\")");
            //Console.WriteLine("dynamicFunctionsLike1 {0}", JsonConvert.SerializeObject(dynamicFunctionsLike1, Formatting.Indented));

            //var dynamicFunctionsLike2 = context.Cars.Where(config, "DynamicFunctions.Like(Vin, \"%a.%b%\", \".\")");
            //Console.WriteLine("dynamicFunctionsLike2 {0}", JsonConvert.SerializeObject(dynamicFunctionsLike2, Formatting.Indented));

            //var testDynamic = context.Cars.Select(c => new
            //{
            //    K = c.Key,
            //    C = c.Color
            //});

            //var testDynamicResult = testDynamic.Select("it").OrderBy("C");
            //try
            //{
            //    Console.WriteLine("resultX {0}", JsonConvert.SerializeObject(testDynamicResult, Formatting.Indented));
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }
    }
}
