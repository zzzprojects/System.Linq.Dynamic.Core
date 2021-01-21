using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using ConsoleAppEF2.Database;

namespace ConsoleApp_net5_0_EF5_InMemory
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var context = new TestContextEF5())
            {
                context.Products.Add(new ProductDynamic { Dict = new Dictionary<string, object> { { "Name", "test" } } });
                context.SaveChanges();
            }

            using (var context = new TestContextEF5())
            {
                var results = context.Products.Where("Dict.Name == @0", "test");

                Console.WriteLine("results = ?");
                foreach (var result in results)
                {
                    Console.WriteLine(result.Key);
                }
            }
        }
    }
}
