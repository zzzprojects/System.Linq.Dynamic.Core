﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleAppEF2.Database;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_net5_0_EF6_InMemory;

static class Program
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    static async Task Main(string[] args)
    {
        await using (var context = new TestContextEF6())
        {
            context.Products.Add(new ProductDynamic { NullableInt = 1, Dict = new Dictionary<string, object> { { "Name", "test" } } });
            context.Products.Add(new ProductDynamic { NullableInt = 2, Dict = new Dictionary<string, object> { { "Name1", "test1" } } });
            await context.SaveChangesAsync();
        }

        await using (var context = new TestContextEF6())
        {
            var intType = typeof(int).FullName;

            var a1 = context.Products.Select($"\"{intType}\"(Key)").ToDynamicArray();
            Console.WriteLine("a1 {0}", string.Join(",", a1));

            var a2 = context.Products.Select($"\"{intType}\"?(Key)").ToDynamicArray();
            Console.WriteLine("a2 {0}", string.Join(",", a2));
        }

        await using (var context = new TestContextEF6())
        {
            var resultsNormal = context.Products.Where(p => p.Dict["Name"] == "test").ToListAsync();

            var results1 = await context.Products.Where("Dict.Name == @0", "test").ToListAsync();
            Console.WriteLine("results1:");
            foreach (var result in results1)
            {
                Console.WriteLine(result.Key + ":" + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }

            var results2 = await context.Products.Where("Dict.Name == @0", "test").ToDynamicListAsync();
            Console.WriteLine("results2:");
            foreach (var result in results2)
            {
                Console.WriteLine(result.Key + ":" + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }

            var results3 = await context.Products.Where("NullableInt == 1").ToDynamicListAsync(typeof(ProductDynamic));
            Console.WriteLine("results3:");
            foreach (var result in results3)
            {
                Console.WriteLine(result.Key + ":" + JsonSerializer.Serialize(result.Dict, JsonSerializerOptions));
            }
        }
    }
}