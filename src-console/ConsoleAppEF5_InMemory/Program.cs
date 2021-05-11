using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using ConsoleAppEF2.Database;

namespace ConsoleApp_net5_0_EF5_InMemory
{
    static class Program
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        static void Main(string[] args)
        {
            using (var context = new TestContextEF5())
            {
                context.Products.Add(new ProductDynamic { NullableInt = 1, Dict = new Dictionary<string, object> { { "Name", "test" } } });
                context.Products.Add(new ProductDynamic { NullableInt = null, Dict = new Dictionary<string, object> { { "Name2", "test2" } } });
                context.SaveChanges();
            }

            using (var context = new TestContextEF5())
            {
                var results = context.Products.Where("Dict.Name == @0", "test");

                Console.WriteLine("results = ?");
                foreach (var result in results)
                {
                    Console.WriteLine(result.Key + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
                }

                var orderedResults = context.Products.OrderBy("np(NullableInt)");
                foreach (var result in orderedResults)
                {
                    Console.WriteLine(result.Key + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
                }
            }
        }
    }
}
