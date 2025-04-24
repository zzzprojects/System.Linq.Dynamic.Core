using System.Data;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class DynamicGetMemberBinderTests
{
    public class SalesData
    {
        public string Region { get; set; } = null!;

        public string Product { get; set; } = null!;

        public string Sales { get; set; } = null!;
    }

    public class GroupedSalesData
    {
        public string Region { get; set; } = null!;
        public string? Product { get; set; }
        public int TotalSales { get; set; }
        public int GroupLevel { get; set; }
    }

    [Fact]
    public void DynamicGetMemberBinder_SelectOnArrayWithComplexObjects()
    {
        // Arrange
        var rows = new SalesData[]
        {
            new() { Region = "North", Product = "Widget", Sales = "100" },
            new() { Region = "North", Product = "Gadget", Sales = "150" },
            new() { Region = "South", Product = "Widget", Sales = "200" },
            new() { Region = "South", Product = "Gadget", Sales = "100" },
            new() { Region = "North", Product = "Widget", Sales = "50" }
        }.AsQueryable();

        // Act
        var grouping1 = rows
            .GroupBy("new (Region, Product)")
            .Select("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        var grouping2 = rows
            .GroupBy("Region")
            .Select("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = grouping1.ToDynamicArray().Concat(grouping2.ToDynamicArray()).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        // Assert
        ordered.Should().HaveCount(6);
    }

    [Fact]
    public void DynamicGetMemberBinder_SelectTypeOnArrayWithComplexObjects()
    {
        // Arrange
        var rows = new SalesData[]
        {
            new() { Region = "North", Product = "Widget", Sales = "100" },
            new() { Region = "North", Product = "Gadget", Sales = "150" },
            new() { Region = "South", Product = "Widget", Sales = "200" },
            new() { Region = "South", Product = "Gadget", Sales = "100" },
            new() { Region = "North", Product = "Widget", Sales = "50" }
        }.AsQueryable();

        // Act
        var grouping1 = rows
            .GroupBy("new (Region, Product)")
            .Select<GroupedSalesData>("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        var grouping2 = rows
            .GroupBy("Region")
            .Select<GroupedSalesData>("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = grouping1.Concat(grouping2).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        // Assert
        ordered.Should().HaveCount(6);
    }

    [Fact]
    public void DynamicGetMemberBinder_SelectOnDataTable()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Region", typeof(string));
        dataTable.Columns.Add("Product", typeof(string));
        dataTable.Columns.Add("Sales", typeof(int));

        dataTable.Rows.Add("North", "Apples", 100);
        dataTable.Rows.Add("North", "Oranges", 150);
        dataTable.Rows.Add("South", "Apples", 200);
        dataTable.Rows.Add("South", "Oranges", 250);

        var rows = dataTable.Rows.Cast<DataRow>().AsQueryable();

        // Act
        var grouping1 = rows
            .GroupBy("new (Region, Product)")
            .Select("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        var grouping2 = rows
            .GroupBy("Region")
            .Select("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = grouping1.ToDynamicArray().Concat(grouping2.ToDynamicArray()).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        // Assert
        ordered.Should().HaveCount(6);
    }

    [Fact]
    public void DynamicGetMemberBinder_SelectTypeOnDataTable()
    {
        // Arrange
        var dataTable = new DataTable();
        dataTable.Columns.Add("Region", typeof(string));
        dataTable.Columns.Add("Product", typeof(string));
        dataTable.Columns.Add("Sales", typeof(int));

        dataTable.Rows.Add("North", "Apples", 100);
        dataTable.Rows.Add("North", "Oranges", 150);
        dataTable.Rows.Add("South", "Apples", 200);
        dataTable.Rows.Add("South", "Oranges", 250);

        var rows = dataTable.Rows.Cast<DataRow>().AsQueryable();

        // Act
        var grouping1 = rows
            .GroupBy("new (Region, Product)")
            .Select<GroupedSalesData>("new (Key.Region as Region, Key.Product as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 0 as GroupLevel)");

        var grouping2 = rows
            .GroupBy("Region")
            .Select<GroupedSalesData>("new (Key as Region, null as Product, Sum(Convert.ToInt32(Sales)) as TotalSales, 1 as GroupLevel)");

        var combined = grouping1.ToDynamicArray().Concat(grouping2.ToDynamicArray()).AsQueryable();
        var ordered = combined.OrderBy("Product").ToDynamicList();

        // Assert
        ordered.Should().HaveCount(6);
    }

    [Fact]
    public void DynamicGetMemberBinder_SelectOnArrayWithIntegers()
    {
        // Arrange
        var dynamicData = new[] { 1, 2 }
            .AsQueryable()
            .Select("new { it * 2 as Value }")
            .ToDynamicArray()
            .AsQueryable();

        // Act
        var dynamicResult1 = dynamicData
            .Select("Value")
            .ToDynamicArray();

        var dynamicResult2 = dynamicData
            .Select("Value")
            .ToDynamicArray<int>();

        // Assert
        dynamicResult1.Should().HaveCount(2);
        dynamicResult2.Should().BeEquivalentTo([2, 4]);
    }
}