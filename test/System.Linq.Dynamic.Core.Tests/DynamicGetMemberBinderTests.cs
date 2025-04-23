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

    [Fact]
    public void DynamicGetMemberBinder_Test1()
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
    public void DynamicGetMemberBinder_Test2()
    {
        // Arrange
        var dynamicData = new[] { 1, 2 }
            .AsQueryable()
            .Select("new { it as Value }")
            .ToDynamicArray();

        // Act
        var dynamicResult = dynamicData
            .AsQueryable()
            .Select("Value")
            .ToDynamicArray();

        // Assert
        dynamicResult.Should().HaveCount(2);
    }
}