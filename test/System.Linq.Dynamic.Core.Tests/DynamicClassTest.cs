using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class DynamicClassTest
{
    [Fact]
    public void DynamicClass_GetProperties_Should_Work()
    {
        // Arrange
        var range = new List<object>
        {
            new { FieldName = "TestFieldName", Value = 3.14159 }
        };

        // Act
        var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
        var item = rangeResult.First();

        var call = () => item.GetDynamicMemberNames();
        call.Should().NotThrow();
    }

    [Fact]
    public void DynamicClass_GetPropertyValue_Should_Work()
    {
        // Arrange
        var test = "Test";
        var range = new List<object>
        {
            new { FieldName = test, Value = 3.14159 }
        };

        // Act
        var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
        var item = rangeResult.First();

        var value = item.FieldName as string;
        value.Should().Be(test);
    }

    [Fact]
    public void DynamicClass_GettingValue_ByIndex_Should_Work()
    {
        // Arrange
        var test = "Test";
        var range = new List<object>
        {
            new { FieldName = test, Value = 3.14159 }
        };

        // Act
        var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
        var item = rangeResult.First();

        var value = item["FieldName"] as string;
        value.Should().Be(test);
    }

    [Fact]
    public void DynamicClass_SettingExistingPropertyValue_ByIndex_Should_Work()
    {
        // Arrange
        var test = "Test";
        var newTest = "abc";
        var range = new List<object>
        {
            new { FieldName = test, Value = 3.14159 }
        };

        // Act
        var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
        var item = rangeResult.First();

        item["FieldName"] = newTest;
        var value = item["FieldName"] as string;
        value.Should().Be(newTest);
    }

    [Fact]
    public void DynamicClass_SettingNewProperty_ByIndex_Should_Work()
    {
        // Arrange
        var test = "Test";
        var newTest = "abc";
        var range = new List<object>
        {
            new { FieldName = test, Value = 3.14159 }
        };

        // Act
        var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
        var item = rangeResult.First();

        item["X"] = newTest;
        var value = item["X"] as string;
        value.Should().Be(newTest);
    }
}