using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Issue_645()
    {
        // Arrange
        var list = new List<Customer>
        {
            new()
            {
                Name = "Terri Lee Duffy",
                CompanyName = "ABC",
                City = "Paris",
                Phone = "333-444444",
                Location = new Location { Name = "test" },
                LastContact = DateTimeOffset.Parse("2022-11-14")
            },
            new()
            {
                Name = "Garry Moore",
                CompanyName = "ABC",
                City = "Paris",
                Phone = "54654-444444",
                Location = new Location { Name = "other test", UpdateAt = DateTimeOffset.Parse("2022-11-16") },
                LastContact = DateTimeOffset.Parse("2022-11-16")
            }
        };

        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };

        // Act 1
        var result1 = list.AsQueryable().Where(config, "LastContact = \"2022-11-16\"").ToArray();

        // Assert 1
        result1.Should().HaveCount(1);

        // Act 2
        var result2 = list.AsQueryable().Where("Location.UpdateAt = \"2022-11-16\"").ToArray();

        // Assert 2
        result2.Should().HaveCount(1);
    }
}

public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string City { get; set; }
    public string Phone { get; set; }
    public Location Location { get; set; }
    public DateTimeOffset? LastContact { get; set; }
}

public class Location
{
    public int LocationID { get; set; }
    public string Name { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}