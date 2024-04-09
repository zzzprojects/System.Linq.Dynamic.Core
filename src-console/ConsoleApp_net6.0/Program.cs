using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.NewtonsoftJson;
using System.Linq.Dynamic.Core.SystemTextJson;
using System.Linq.Expressions;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp_net6._0;

public class X
{
    public string Key { get; set; } = null!;

    public List<Y>? Contestants { get; set; }
}

public class Y
{
}

class Program
{
    static void Main(string[] args)
    {
        Json();
        // NewtonsoftJson();

        return;

        Issue389DoesNotWork();
        return;
        Issue389_Works();
        return;

        var q = new[]
        {
            new X { Key = "x" },
            new X { Key = "a" },
            new X { Key = "a", Contestants = new List<Y> { new Y() } }
        }.AsQueryable();
        var groupByKey = q.GroupBy("Key");
        var selectQry = groupByKey.Select("new (Key, Sum(np(Contestants.Count, 0)) As TotalCount)").ToDynamicList();

        Normal();
        Dynamic();
    }

    private static void NewtonsoftJson()
    {
        var array = JArray.Parse(@"[
        {
            ""first"": 1,
            ""City"": ""Paris"",
            ""third"": ""test""
        },
        {
            ""first"": 2,
            ""City"": ""New York"",
            ""third"": ""abc""
        }]");

        var where = array.Where("City == @0", "Paris");
        foreach (var result in where)
        {
            Console.WriteLine(result["first"]);
        }

        var select = array.Select("City");
        foreach (var result in select)
        {
            Console.WriteLine(result);
        }

        var whereWithSelect = array.Where("City == @0", "Paris").Select("first");
        foreach (var result in whereWithSelect)
        {
            Console.WriteLine(result);
        }
    }

    private static void Json()
    {
        var doc = JsonDocument.Parse(@"[
        {
            ""first"": 1,
            ""City"": ""Paris"",
            ""third"": ""test""
        },
        {
            ""first"": 2,
            ""City"": ""New York"",
            ""third"": ""abc""
        }]");

        var where = doc.Where("City == @0", "Paris");
        foreach (var result in where.RootElement.EnumerateArray())
        {
            Console.WriteLine(result.GetProperty("first"));
        }

        var select = doc.Select("City");
        foreach (var result in select.EnumerateArray())
        {
            Console.WriteLine(result);
        }

        var whereWithSelect = doc.Where("City == @0", "Paris").Select("first");
        foreach (var result in whereWithSelect.EnumerateArray())
        {
            Console.WriteLine(result);
        }
    }

    private static void Issue389_Works()
    {
        var strArray = new[] { "1", "2", "3", "4" };
        var x = new List<ParameterExpression>();
        x.Add(Expression.Parameter(strArray.GetType(), "strArray"));

        string query = "string.Join(\",\", strArray)";

        var e = DynamicExpressionParser.ParseLambda(x.ToArray(), null, query);
        Delegate del = e.Compile();
        var result1 = del.DynamicInvoke(new object?[] { strArray });
        Console.WriteLine(result1);
    }

    private static void Issue389WorksWithInts()
    {
        var intArray = new object[] { 1, 2, 3, 4 };
        var x = new List<ParameterExpression>();
        x.Add(Expression.Parameter(intArray.GetType(), "intArray"));

        string query = "string.Join(\",\", intArray)";

        var e = DynamicExpressionParser.ParseLambda(x.ToArray(), null, query);
        Delegate del = e.Compile();
        var result = del.DynamicInvoke(new object?[] { intArray });

        Console.WriteLine(result);
    }

    private static void Issue389DoesNotWork()
    {
        var intArray = new[] { 1, 2, 3, 4 };
        var x = new List<ParameterExpression>();
        x.Add(Expression.Parameter(intArray.GetType(), "intArray"));

        string query = "string.Join(\",\", intArray)";

        var e = DynamicExpressionParser.ParseLambda(x.ToArray(), null, query);
        Delegate del = e.Compile();
        var result = del.DynamicInvoke(new object?[] { intArray });

        Console.WriteLine(result);
    }

    private static void Normal()
    {
        var e = new int[0].AsQueryable();
        var q = new[] { 1 }.AsQueryable();

        var a = q.FirstOrDefault();
        var b = e.FirstOrDefault(44);

        var c = q.FirstOrDefault(i => i == 0);
        var d = q.FirstOrDefault(i => i == 0, 42);

        var t = q.Take(1);
    }

    private static void Dynamic()
    {
        var e = new int[0].AsQueryable() as IQueryable;
        var q = new[] { 1 }.AsQueryable() as IQueryable;

        var a = q.FirstOrDefault();
        //var b = e.FirstOrDefault(44);

        var c = q.FirstOrDefault("it == 0");
        //var d = q.FirstOrDefault(i => i == 0, 42);
    }
}