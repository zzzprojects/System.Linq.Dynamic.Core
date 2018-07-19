using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using ConsoleAppEF21.Database;
using Newtonsoft.Json;

namespace ConsoleAppEF21
{
    class Program
    {
        //class C : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
        //{
        //    public HashSet<Type> GetCustomTypes()
        //    {
        //        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //        var set = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies))
        //        {
        //            typeof(TestContext)
        //        };

        //        return set;
        //    }
        //}

        //private static IQueryable GetQueryable()
        //{
        //    var random = new Random((int)DateTime.Now.Ticks);

        //    var x = Enumerable.Range(0, 10).Select(i => new
        //    {
        //        Id = i,
        //        Value = random.Next(),
        //    });

        //    return x.AsQueryable().Select("new (it as Id, @0 as Value)", random.Next());
        //    // return x.AsQueryable(); //x.AsQueryable().Select("new (Id, Value)");
        //}

        static void Main(string[] args)
        {
            var list = new List<Car> { new Car { Key = 1 }, new Car { Key = 2 } };

            var carsTest = list.AsQueryable().Where("Key = @0", "1").ToList();

            //IQueryable qry = GetQueryable();

            //var result = qry.Select("it").OrderBy("Value");
            //try
            //{
            //    Console.WriteLine("result {0}", JsonConvert.SerializeObject(result, Formatting.Indented));
            //}
            //catch (Exception)
            //{
            //    // Console.WriteLine(e);
            //}

            //var all = new
            //{
            //    test1 = new List<int> { 1, 2, 3 }.ToDynamicList(typeof(int)),
            //    test2 = new List<dynamic> { 4, 5, 6 }.ToDynamicList(typeof(int)),
            //    test3 = new List<object> { 7, 8, 9 }.ToDynamicList(typeof(int))
            //};
            // Console.WriteLine("all {0}", JsonConvert.SerializeObject(all, Formatting.None));

            var config = new ParsingConfig
            {
                EvaluateGroupByAtDatabase = true
            };

            var context = new TestContext();
            if (!context.Cars.Any())
            {
                context.Cars.Add(new Car { Brand = "Ford", Color = "Blue", Vin = "yes", Year = "2017" });
                context.Cars.Add(new Car { Brand = "Fiat", Color = "Red", Vin = "yes", Year = "2016" });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "no", Year = "1979" });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "a%bc", Year = "1979" });
                context.SaveChanges();
            }

            //var g1 = context.Cars.GroupBy("new(Brand)").Select("new(Key.Brand as KeyValue1, it.Count() as CountValue1)").ToDynamicList();
            //Console.WriteLine("GroupBy @ local {0}", JsonConvert.SerializeObject(g1, Formatting.Indented));

            //Console.WriteLine(new string('_', 80));

            var g2 = context.Cars.GroupBy("new(Brand)", config).Select("new(Key.Brand as KeyValue2, it.Count() as CountValue2)").ToDynamicList();
            Console.WriteLine("GroupBy @ database {0}", JsonConvert.SerializeObject(g2, Formatting.Indented));

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
