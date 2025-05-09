using System;
using System.Collections.Generic;
using System.Data;
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

public class SalesData
{
    public string Region { get; set; }
    public string Product { get; set; }
    public string Sales { get; set; }
}

public class GroupedSalesData
{
    public string Region { get; set; }
    public string? Product { get; set; }
    public int TotalSales { get; set; }
    public int GroupLevel { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Issue918();
        return;

        Issue912a();
        Issue912b();
        return;

        Json();
        NewtonsoftJson();

        return;

        Issue389DoesNotWork();
        return;
        Issue389_Works();
        return;

        var q = new[]
        {
            new X { Key = "x" },
            new X { Key = "a" },
            new X { Key = "a", Contestants = new List<Y> { new() } }
        }.AsQueryable();
        var groupByKey = q.GroupBy("Key");
        var selectQry = groupByKey.Select("new (Key, Sum(np(Contestants.Count, 0)) As TotalCount)").ToDynamicList();

        Normal();
        Dynamic();
    }

    private static void Issue918()
    {
        var persons = new DataTable();
        persons.Columns.Add("FirstName", typeof(string));
        persons.Columns.Add("Nickname", typeof(string));
        persons.Columns.Add("Income", typeof(decimal)).AllowDBNull = true;

        // Adding sample data to the first DataTable
        persons.Rows.Add("alex", DBNull.Value, 5000.50m);
        persons.Rows.Add("MAGNUS", "Mag", 5000.50m);
        persons.Rows.Add("Terry", "Ter", 4000.20m);
        persons.Rows.Add("Charlotte", "Charl", DBNull.Value);

        var linqQuery =
            from personsRow in persons.AsEnumerable()
            select personsRow;

        var queryableRows = linqQuery.AsQueryable();

        // Sorted at the top of the list
        var comparer = new DataColumnOrdinalIgnoreCaseComparer();
        var sortedRows = queryableRows.OrderBy("FirstName", comparer).ToList();

        int xxx = 0;
    }

    private static void Issue912a()
    {
        var extractedRows = new List<SalesData>
        {
            new() { Region = "North", Product = "Widget", Sales = "100" },
            new() { Region = "North", Product = "Gadget", Sales = "150" },
            new() { Region = "South", Product = "Widget", Sales = "200" },
            new() { Region = "South", Product = "Gadget", Sales = "100" },
            new() { Region = "North", Product = "Widget", Sales = "50" }
        };

        var rows = extractedRows.AsQueryable();

        // GROUPING SET 1: (Region, Product)
        var detailed = rows
            .GroupBy("new (Region, Product)")
            .Select<GroupedSalesData>("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        // GROUPING SET 2: (Region)
        var regionSubtotal = rows
            .GroupBy("Region")
            .Select<GroupedSalesData>("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = detailed.Concat(regionSubtotal).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        int x = 9;
    }

    private static void Issue912b()
    {
        var eInfoJoinTable = new DataTable();
        eInfoJoinTable.Columns.Add("Region", typeof(string));
        eInfoJoinTable.Columns.Add("Product", typeof(string));
        eInfoJoinTable.Columns.Add("Sales", typeof(int));

        eInfoJoinTable.Rows.Add("North", "Apples", 100);
        eInfoJoinTable.Rows.Add("North", "Oranges", 150);
        eInfoJoinTable.Rows.Add("South", "Apples", 200);
        eInfoJoinTable.Rows.Add("South", "Oranges", 250);

        var extractedRows =
            from row in eInfoJoinTable.AsEnumerable()
            select row;

        var rows = extractedRows.AsQueryable();

        // GROUPING SET 1: (Region, Product)
        var detailed = rows
            .GroupBy("new (Region, Product)")
            .Select("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        // GROUPING SET 2: (Region)
        var regionSubtotal = rows
            .GroupBy("Region")
            .Select("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = detailed.ToDynamicArray().Concat(regionSubtotal.ToDynamicArray()).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        int x = 9;
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
        foreach (var result in select.RootElement.EnumerateArray())
        {
            Console.WriteLine(result);
        }

        var whereWithSelect = doc.Where("City == @0", "Paris").Select("first");
        foreach (var result in whereWithSelect.RootElement.EnumerateArray())
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