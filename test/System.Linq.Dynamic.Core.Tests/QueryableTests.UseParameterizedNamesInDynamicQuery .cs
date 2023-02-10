using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    /// <summary>
    /// Issue #645
    /// </summary>
    [Fact]
    public void When_UseParameterizedNamesInDynamicQuery_IsTrue_WrappedStringValueForDateTime_Should_Be_Unwrapped()
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

        // Act 1A
        var result1A = list.AsQueryable().Where(config, "LastContact = \"2022-11-16\"").ToArray();

        // Assert 1A
        result1A.Should().HaveCount(1);

        // Act 1B
        var result1B = list.AsQueryable().Where(config, "\"2022-11-16\" == LastContact").ToArray();

        // Assert 1B
        result1B.Should().HaveCount(1);

        // Act 2A
        var result2A = list.AsQueryable().Where("Location.UpdateAt = \"2022-11-16\"").ToArray();

        // Assert 2A
        result2A.Should().HaveCount(1);

        // Act 2B
        var result2B = list.AsQueryable().Where("\"2022-11-16\" == Location.UpdateAt").ToArray();

        // Assert 2B
        result2B.Should().HaveCount(1);
    }

    /// <summary>
    /// Issue #668
    /// </summary>
    [Fact]
    public void When_UseParameterizedNamesInDynamicQuery_IsTrue_WrappedStringValueEnum_Should_Be_Unwrapped()
    {
        // Arrange
        var list = new List<Customer>
        {
            new()
            {
                Name = "Duffy",
                GenderType = Gender.Female
            },
            new()
            {
                Name = "Garry",
                GenderType = Gender.Male
            }
        };

        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };

        // Act
        var result = list.AsQueryable().Where(config, "GenderType = \"Female\"").ToArray();

        // Assert
        result.Should().HaveCount(1);
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
    public Gender GenderType { get; set; }
}

public class Location
{
    public int LocationID { get; set; }
    public string Name { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}