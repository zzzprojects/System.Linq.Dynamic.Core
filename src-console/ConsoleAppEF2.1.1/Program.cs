﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ConsoleAppEF2.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConsoleAppEF211
{
    class Program
    {
        class C : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
        {
            public HashSet<Type> GetCustomTypes()
            {
                // https://stackoverflow.com/a/2384679/255966
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                var loadedPaths = loadedAssemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

                var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

                toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

                return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(loadedAssemblies))
                {
                    typeof(TestContext)
                };
            }

            public Dictionary<Type, List<MethodInfo>> GetExtensionMethods()
            {
                var types = GetCustomTypes();

                List<Tuple<Type, MethodInfo>> list = new List<Tuple<Type, MethodInfo>>();

                foreach (var type in types)
                {
                    var extensionMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(x => x.IsDefined(typeof(ExtensionAttribute), false)).ToList();

                    extensionMethods.ForEach(x => list.Add(new Tuple<Type, MethodInfo>(x.GetParameters()[0].ParameterType, x)));
                }

                return list.GroupBy(x => x.Item1, tuple => tuple.Item2).ToDictionary(key => key.Key, methods => methods.ToList());
            }

            public Type ResolveType(string typeName)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                return ResolveType(assemblies, typeName);
            }

            public Type ResolveTypeBySimpleName(string typeName)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                return ResolveTypeBySimpleName(assemblies, typeName);
            }
        }

        private static IQueryable GetQueryable()
        {
            var random = new Random((int)DateTime.Now.Ticks);

            var x = Enumerable.Range(0, 10).Select(i => new
            {
                Id = i,
                Value = random.Next(),
            });

            return x.AsQueryable().Select("new (it as Id, @0 as Value)", random.Next());
            // return x.AsQueryable(); //x.AsQueryable().Select("new (Id, Value)");
        }

        static void Main(string[] args)
        {
            var value = "\"\\\\192.168.1.1\\audio\\new\"";
            var dataParameter = Expression.Parameter(typeof(Dictionary<string, object>), "data");
            var sourceExpr = DynamicExpressionParser.ParseLambda(new[] { dataParameter }, typeof(object), value);
            var func = sourceExpr.Compile();
            var resultX = func.DynamicInvoke(new Dictionary<string, object>());
            Console.WriteLine(resultX);

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

            var config = ParsingConfig.DefaultEFCore21;
            //config.ResolveTypesBySimpleName = true;
            config.CustomTypeProvider = new C();

            var context = new TestContext();

            // context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var dateLastModified = new DateTime(2018, 1, 15);
            if (!context.Cars.Any())
            {
                context.Cars.Add(new Car { Brand = "Ford", Color = "Blue", Vin = "yes", Year = "2017", DateLastModified = dateLastModified });
                context.Cars.Add(new Car { Brand = "Fiat", Color = "Red", Vin = "yes", Year = "2016", DateLastModified = dateLastModified.AddDays(1) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "no", Year = "1979", DateLastModified = dateLastModified.AddDays(2) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "a%bc", Year = "1979", DateLastModified = dateLastModified.AddDays(3) }); ;
                context.SaveChanges();
            }

            var carFirstOrDefault1 = context.Cars.Where(config, "Brand == \"Ford\"");
            Console.WriteLine("carFirstOrDefault1 {0}", JsonConvert.SerializeObject(carFirstOrDefault1, Formatting.Indented));

            var carFirstOrDefault2 = context.Cars.Where(config, "Brand == @0", "Alfa");
            Console.WriteLine("carFirstOrDefault2 {0}", JsonConvert.SerializeObject(carFirstOrDefault2, Formatting.Indented));

            var carsLike1 =
                from c in context.Cars
                where EF.Functions.Like(c.Brand, "%a%")
                select c;
            Console.WriteLine("carsLike1 {0}", JsonConvert.SerializeObject(carsLike1, Formatting.Indented));

            var cars2Like = context.Cars.Where(c => EF.Functions.Like(c.Brand, "%a%"));
            Console.WriteLine("cars2Like {0}", JsonConvert.SerializeObject(cars2Like, Formatting.Indented));

            var dynamicCarsLike1 = context.Cars.Where(config, "TestContext.Like(Brand, \"%a%\")");
            Console.WriteLine("dynamicCarsLike1 {0}", JsonConvert.SerializeObject(dynamicCarsLike1, Formatting.Indented));

            var dynamicCarsLike2 = context.Cars.Where(config, "TestContext.Like(Brand, \"%d%\")");
            Console.WriteLine("dynamicCarsLike2 {0}", JsonConvert.SerializeObject(dynamicCarsLike2, Formatting.Indented));

            var dynamicFunctionsLike1 = context.Cars.Where(config, "DynamicFunctions.Like(Brand, \"%a%\")");
            Console.WriteLine("dynamicFunctionsLike1 {0}", JsonConvert.SerializeObject(dynamicFunctionsLike1, Formatting.Indented));

            var dynamicFunctionsLike2 = context.Cars.Where(config, "DynamicFunctions.Like(Vin, \"%a.%b%\", \".\")");
            Console.WriteLine("dynamicFunctionsLike2 {0}", JsonConvert.SerializeObject(dynamicFunctionsLike2, Formatting.Indented));

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
    }
}
