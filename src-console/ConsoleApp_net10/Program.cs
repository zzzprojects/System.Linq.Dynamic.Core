using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.NewtonsoftJson;
using System.Linq.Dynamic.Core.SystemTextJson;
using System.Linq.Expressions;
using System.Text.Json;
using ConsoleApp_net6._0;
using Newtonsoft.Json.Linq;

namespace ConsoleApp;

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

class MyEntity
{
    // Factory method to create a list of MyEntity objects from a given list of ids
    public static IEnumerable<MyEntity> CreateList(IEnumerable<int> ids)
    {
        foreach (var id in ids) yield return new MyEntity { Id = id };
    }

    public int Id { get; set; }
}

public enum TestEnum
{
    T1, T2
}

public class TestClass
{
    public TestEnum P1 { get; set; }

    public IEnumerable<TestEnum> P2 { get; set; } = [];
}

class Program
{
    static void Main(string[] args)
    {
        Issue963();
        return;

        Issue987();
        return;

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

    private static void Issue963()
    {
        var list = new List<TestClass> 
        {
            new TestClass { P1 = TestEnum.T1, P2 = [TestEnum.T1, TestEnum.T2] },
            new TestClass { P1 = TestEnum.T2, P2 = [TestEnum.T2] }
        };

        var result1 = list.AsQueryable().Where("\"T1\" in P2").ToArray();
        var result2 = list.AsQueryable().Where("P2.Contains(\"T1\")").ToArray();
    }

    private static void Issue987()
    {
        var list = new List<MyEntity>();
        for (int i = 0; i < 10000; i++)
            list.Add(new MyEntity { Id = i });

        var test1 = list.AsQueryable()
            .Where("Id in (9495, 9496, 9498, 9500, 9501, 9503, 9505, 9508, 9509, 9510, 9511, 9514, 9515, 9517, 9518, 9519, 9520, 9521, 9523, 9524, 9525, 9526, 9527, 9528, 9529, 9530, 9531, 9532, 9533, 9534, 9535, 9536, 9538, 9539, 9540, 9541, 9542, 9543, 9544, 9545, 9546, 9547, 9548, 9549, 9550, 9552, 9554, 9556, 9557, 9558, 9559, 9560, 9561, 9562, 9563, 9565, 9567, 9569, 9570, 9575, 9576, 9577, 9578, 9579, 9580, 9581, 9582, 9583, 9584, 9585, 9586, 9587, 9588, 9589, 9590, 9591, 9592, 9593, 9594, 9595, 9596, 9597, 9598, 9599, 9600, 9601, 9602, 9603, 9604, 9605, 9606, 9607, 9608, 9609, 9610, 9611, 9612, 9613, 9614, 9615, 9616, 9617, 9618, 9619, 9620, 9621, 9622, 9623, 9624, 9625, 9626, 9627, 9628, 9629)")
            .ToList();

        Console.WriteLine("Number of elements : " + test1.Count);

        //the list of ids that were actually used in our application and resulted in the discovery of this bug
        var originalIdList = new List<int>() { 9495, 9496, 9498, 9500, 9501, 9503, 9505, 9508, 9509, 9510, 9511, 9514, 9515, 9517, 9518, 9519, 9520, 9521, 9523, 9524, 9525, 9526, 9527, 9528, 9529, 9530, 9531, 9532, 9533, 9534, 9535, 9536, 9538, 9539, 9540, 9541, 9542, 9543, 9544, 9545, 9546, 9547, 9548, 9549, 9550, 9552, 9554, 9556, 9557, 9558, 9559, 9560, 9561, 9562, 9563, 9565, 9567, 9569, 9570, 9575, 9576, 9577, 9578, 9579, 9580, 9581, 9582, 9583, 9584, 9585, 9586, 9587, 9588, 9589, 9590, 9591, 9592, 9593, 9594, 9595, 9596, 9597, 9598, 9599, 9600, 9601, 9602, 9603, 9604, 9605, 9606, 9607, 9608, 9609, 9610, 9611, 9612, 9613, 9614, 9615, 9616, 9617, 9618, 9619, 9620, 9621, 9622, 9623, 9624, 9625, 9626, 9627, 9628, 9629 };
        //list of ids also starting at 9495, but without gaps
        var adjacentIdList = Enumerable.Range(9495, 114);
        //original list starting at Id1
        var originalIdListStartingAt1 = originalIdList.Select(id => id - 9494);
        //list with gaps of 1
        var listWithGapsOf1 = Enumerable.Range(1, 114).Select(id => id * 2);
        //list with gaps of 2
        var listWithGapsOf2 = Enumerable.Range(1, 114).Select(id => id * 3);
        //list with gaps of 3
        var listWithGapsOf3 = Enumerable.Range(1, 114).Select(id => id * 4);
        //list with gaps of 4
        var listWithGapsOf4 = Enumerable.Range(1, 114).Select(id => id * 5);

        //list of 10.000 entities , with ids starting at 0
        var entityList = MyEntity.CreateList(Enumerable.Range(1, 10_000));

        //filter the list of entities by the list of ids using dynamic linq and write the number of elements in the filtered list to the console
        static void Filter(IEnumerable<MyEntity> entities, IEnumerable<int> ids)
        {
            var filtered = entities.AsQueryable().Where($"Id in ({string.Join(',', ids)})").ToList();
            Console.WriteLine("Number of elements : " + filtered.Count);
        }

        Filter(entityList, originalIdList);
        Filter(entityList, adjacentIdList);
        Filter(entityList, originalIdListStartingAt1);
        Filter(entityList, listWithGapsOf1);
        Filter(entityList, listWithGapsOf2);
        Filter(entityList, listWithGapsOf3);
        Filter(entityList, listWithGapsOf4);
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