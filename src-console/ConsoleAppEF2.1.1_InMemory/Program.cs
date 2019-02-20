using ConsoleAppEF2.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace ConsoleAppEF2
{
    static class Program
    {
        public class NestedDto
        {
            public string Name { get; set; }

            public NestedDto2 NestedDto2 { get; set; }
        }

        public class NestedDto2
        {
            public string Name2 { get; set; }

            public int Id { get; set; }
            public NestedDto3 NestedDto3 { get; set; }
        }

        public class NestedDto3
        {
            public string Name2 { get; set; }

            public int Id { get; set; }
        }

        class TestCustomTypeProvider : DefaultDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
        {
            public new HashSet<Type> GetCustomTypes()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var set = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies))
                {
                    typeof(TestContext)
                };

                return set;
            }
        }

        static void Main(string[] args)
        {
            //var q = new[] { new NestedDto(), new NestedDto { NestedDto2 = new NestedDto2 { NestedDto3 = new NestedDto3 { Id = 42 } } } }.AsQueryable();

            //var np1 = q.Select("np(it.NestedDto2.NestedDto3.Id, 0)");
            //var npResult1 = np1.ToDynamicList<int>();
            //Console.WriteLine("npResult1 {0}", JsonConvert.SerializeObject(npResult1, Formatting.Indented));

            //var np2 = q.Select("np(it.NestedDto2.NestedDto3.Id)");
            //var npResult2 = np2.ToDynamicList<int?>();
            //Console.WriteLine("npResult2 {0}", JsonConvert.SerializeObject(npResult2, Formatting.Indented));

            //var r1 = q.Select("it != null && it.NestedDto2 != null ? it.NestedDto2.Id : null");
            //var list1 = r1.ToDynamicList<int?>();

            //var r2 = q.Select("it != null && it.NestedDto2 != null ? it.NestedDto2 : null");
            //var list2 = r2.ToDynamicList<NestedDto2>();

            var config = new ParsingConfig
            {
                AllowNewToEvaluateAnyType = true,
                CustomTypeProvider = new TestCustomTypeProvider()
            };

            //// Act
            //var testDataAsQueryable = new List<string> { "name1", "name2" }.AsQueryable();
            //var projectedData = (IQueryable<NestedDto>)testDataAsQueryable.Select(config, $"new {typeof(NestedDto).FullName}(~ as Name)");
            //Console.WriteLine(projectedData.First().Name);
            //Console.WriteLine(projectedData.Last().Name);

            //var all = new
            //{
            //    test1 = new List<int> { 1, 2, 3 }.ToDynamicList(typeof(int)),
            //    test2 = new List<dynamic> { 4, 5, 6 }.ToDynamicList(typeof(int)),
            //    test3 = new List<object> { 7, 8, 9 }.ToDynamicList(typeof(int))
            //};
            //Console.WriteLine("all {0}", JsonConvert.SerializeObject(all, Formatting.Indented));

            //var anyTest = new[]
            //{
            //    new { id = "1", values =new [] { 1, 2, 3 } },
            //    new { id = "2", values =new [] { 1, 4 } },
            //    new { id = "3", values =new [] { 9, 5 } }
            //}.AsQueryable();

            //var any1 = anyTest.Where(x => x.values.Contains(1));
            //Console.WriteLine("any1 {0}", JsonConvert.SerializeObject(any1, Formatting.Indented));

            //var any2 = anyTest.Where("values.Contains(1)");
            //Console.WriteLine("any2 {0}", JsonConvert.SerializeObject(any2, Formatting.Indented));

            DateTime dateLastModified = new DateTime(2018, 1, 15);

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

            var testDto1 = new TestDto { BaseName = "a", Name = "t" };
            context.BaseDtos.Add(testDto1);
            var testDto2 = new TestDto { BaseName = "b", Name = "t" };
            context.BaseDtos.Add(testDto2);

            var otherTestDto = new OtherTestDto { BaseName = "c", Name = "t" };
            context.BaseDtos.Add(otherTestDto);
            context.SaveChanges();

            context.ComplexDtos.Add(new ComplexDto { X = "both", ListOfBaseDtos = new BaseDto[] { testDto1, otherTestDto } });
            context.ComplexDtos.Add(new ComplexDto { X = "testDto", ListOfBaseDtos = new BaseDto[] { testDto2 } });
            context.SaveChanges();

            OfTypeAndCastTests(context, config);

            var carDateLastModified = context.Cars.Where(config, "DateLastModified > \"2018-01-16\"");
            Console.WriteLine("carDateLastModified {0}", JsonConvert.SerializeObject(carDateLastModified, Formatting.Indented));

            var carFirstOrDefault = context.Cars.Where(config, "Brand == \"Ford\"");
            Console.WriteLine("carFirstOrDefault {0}", JsonConvert.SerializeObject(carFirstOrDefault, Formatting.Indented));

            LikeTests(context, config);

            var testDynamic = context.Cars.Select(c => new
            {
                K = c.Key,
                C = c.Color
            });

            var testDynamicResult = testDynamic.Select("it").OrderBy("C");
            try
            {
                Console.WriteLine("resultX {0}", JsonConvert.SerializeObject(testDynamicResult, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void LikeTests(TestContext context, ParsingConfig config)
        {
            //var carsLike1 =
            //    from c in context.Cars
            //    where EF.Functions.Like(c.Brand, "%a%")
            //    select c;
            //Console.WriteLine("carsLike1 {0}", JsonConvert.SerializeObject(carsLike1, Formatting.Indented));

            //var cars2Like = context.Cars.Where(c => EF.Functions.Like(c.Brand, "%a%"));
            //Console.WriteLine("cars2Like {0}", JsonConvert.SerializeObject(cars2Like, Formatting.Indented));

            var dynamicCarsLike1 = context.Cars.Where(config, "TestContext.Like(Brand, \"%a%\")");
            Console.WriteLine("dynamicCarsLike1 {0}", JsonConvert.SerializeObject(dynamicCarsLike1, Formatting.Indented));

            var dynamicCarsLike2 = context.Cars.Where(config, "TestContext.Like(Brand, \"%d%\")");
            Console.WriteLine("dynamicCarsLike2 {0}", JsonConvert.SerializeObject(dynamicCarsLike2, Formatting.Indented));

            var dynamicFunctionsLike1 = context.Cars.Where(config, "DynamicFunctions.Like(Brand, \"%a%\")");
            Console.WriteLine("dynamicFunctionsLike1 {0}",
            JsonConvert.SerializeObject(dynamicFunctionsLike1, Formatting.Indented));

            var dynamicFunctionsLike2 = context.Cars.Where(config, "DynamicFunctions.Like(Vin, \"%a.%b%\", \".\")");
            Console.WriteLine("dynamicFunctionsLike2 {0}",
            JsonConvert.SerializeObject(dynamicFunctionsLike2, Formatting.Indented));
        }

        private static void OfTypeAndCastTests(TestContext context, ParsingConfig config)
        {
            var cast = context.BaseDtos.Where(b => b is TestDto).Cast<TestDto>().ToArray();
            var castDynamicWithType = context.BaseDtos.Where(b => b is TestDto).Cast(typeof(TestDto)).ToDynamicArray();
            var castDynamicWithString = context.BaseDtos.Where(b => b is TestDto).Cast(config, "ConsoleAppEF2.Database.TestDto").ToDynamicArray();

            var oftype = context.BaseDtos.OfType<TestDto>().ToArray();
            var oftypeDynamicWithType = context.BaseDtos.OfType(typeof(TestDto)).ToDynamicArray();
            var oftypeDynamicWithString = context.BaseDtos.OfType(config, "ConsoleAppEF2.Database.TestDto").ToDynamicArray();

            var oftypeTestDto = context.BaseDtos.OfType<TestDto>().Where(x => x.Name == "t").ToArray();
            var oftypeTestDtoDynamic = context.BaseDtos.OfType<TestDto>().Where("Name == \"t\"").ToArray();

            var complexOfType = context.ComplexDtos.Select(c => c.ListOfBaseDtos.OfType<TestDto>().Where(x => x.Name == "t"))
                .ToArray();
            var complexOfTypeDynamic = context.ComplexDtos
                .Select(config, "ListOfBaseDtos.OfType(\"ConsoleAppEF2.Database.TestDto\").Where(Name == \"t\")")
                .ToDynamicArray();

            var complexCast = context.ComplexDtos.Where(c => c.X == "testDto").ToList()
                .Select(c => c.ListOfBaseDtos.Cast<TestDto>().Where(x => x.Name == "t"))
                .ToArray();
            var complexCastDynamic = context.ComplexDtos.Where(c => c.X == "testDto").ToList().AsQueryable()
                .Select(config, "ListOfBaseDtos.Cast(\"ConsoleAppEF2.Database.TestDto\").Where(Name == \"t\")")
                .ToDynamicArray();
        }
    }
}
