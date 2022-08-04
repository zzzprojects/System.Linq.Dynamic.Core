using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleAppEF2.Database;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_net5_0_EF5_InMemory;

static class Program
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    static async Task Main(string[] args)
    {
        await using (var context = new TestContextEF5())
        {
            context.Products.Add(new ProductDynamic { NullableInt = 1, Dict = new Dictionary<string, object> { { "Name", "test" } } });
            context.Products.Add(new ProductDynamic { NullableInt = null, Dict = new Dictionary<string, object> { { "Name2", "test2" } } });
            await context.SaveChangesAsync();
        }

        await using (var context = new TestContextEF5())
        {
            var results = context.Products.Where("Dict.Name == @0", "test");

            Console.WriteLine("results = ?");
            foreach (var result in results)
            {
                Console.WriteLine(result.Key + ": " + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }

            var orderedResults = context.Products.OrderBy("np(NullableInt)");
            foreach (var result in orderedResults)
            {
                Console.WriteLine(result.Key + ": " + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }

            // This fails: https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/541
            var query = EF.CompileQuery<TestContextEF5, string, ProductDynamic>((ctx, ordering) => ctx.Products.OrderBy(ordering));
            var enumerable = query.Invoke(context, "Name ASC");

            foreach (var result in enumerable)
            {
                Console.WriteLine(result.Key + ": " + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }
        }
    }
}