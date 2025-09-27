using System.Collections.Generic;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void GroupByMany_Dynamic_LambdaExpressions()
    {
        var lst = new List<Tuple<int, int, int>>
        {
            new(1, 1, 1),
            new(1, 1, 2),
            new(1, 1, 3),
            new(2, 2, 4),
            new(2, 2, 5),
            new(2, 2, 6),
            new(2, 3, 7)
        };

        var sel = lst.GroupByMany(x => x.Item1, x => x.Item2).ToArray();

        Assert.Equal(2, sel.Length);
        Assert.Single(sel.First().Subgroups);
        Assert.Equal(2, sel.Skip(1).First().Subgroups.Count());
    }

    [Fact]
    public void GroupByMany_Dynamic_StringExpressions()
    {
        var lst = new List<Tuple<int, int, int>>
        {
            new(1, 1, 1),
            new(1, 1, 2),
            new(1, 1, 3),
            new(2, 2, 4),
            new(2, 2, 5),
            new(2, 2, 6),
            new(2, 3, 7)
        };

        var sel = lst.GroupByMany("Item1", "Item2").ToList();

        Check.That(sel.Count).Equals(2);

        var firstGroupResult = sel.First();
        Check.That(firstGroupResult.ToString()).Equals("1 (3)");
        Check.That(firstGroupResult.Subgroups.Count()).Equals(1);

        var skippedGroupResult = sel.Skip(1).First();
        Check.That(skippedGroupResult.ToString()).Equals("2 (4)");
        Check.That(skippedGroupResult.Subgroups.Count()).Equals(2);
    }

    [Fact]
    public void GroupByMany_Dynamic_CompositeKey()
    {
        // Arrange
        var data = new[]
        {
            new { MachineId = 1, Machine = new { Id = 1, Name = "A" } },
            new { MachineId = 1, Machine = new { Id = 1, Name = "A" } },
            new { MachineId = 2, Machine = new { Id = 2, Name = "B" } }
        };

        // Act
        var normalResult = data
            .GroupByMany(d => new { d.MachineId, d.Machine.Name }, a => a.Machine.Id)
            .Select(x => x.ToString())
            .ToList();
        var result = data
            .GroupByMany("new (MachineId, Machine.Name)", "Machine.Id")
            .Select(x => x.ToString())
            .ToList();

        // Assert 
        result.Should().BeEquivalentTo(normalResult);
    }
}