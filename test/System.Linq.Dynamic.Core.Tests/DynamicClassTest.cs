using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Dynamic.Core.Tests.TestHelpers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class DynamicClassTest
{
    public DynamicClassTest()
    {
        DynamicClassFactory.ClearGeneratedTypes();
    }

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

    [Fact]
    public void DynamicClass_SerializeToNewtonsoftJson_Should_Work()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        // Act
        var json = JsonConvert.SerializeObject(dynamicClass);

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }

    private static Type GetRuntimeType<TValue>(in TValue value)
    {
        return typeof(TValue);
    }

    [Fact]
    public void DynamicClass_GetRuntimeType()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        // Act
        var dynamicInstance = (DynamicClass)Activator.CreateInstance(type)!;

        // Assert 1
        var getType = dynamicInstance.GetType();
        getType.ToString().Should().Contain("<>f__AnonymousType");

        // Assert 2
        var typeOf = GetRuntimeType(dynamicInstance);
        typeOf.ToString().Should().Be("System.Linq.Dynamic.Core.DynamicClass"); // ???
    }

    [SkipIfGitHubActions]
    public void DynamicClassArray()
    {
        // Arrange
        var field = new
        {
            Name = "firstName",
            Value = "firstValue"
        };
        var dynamicClasses = new List<DynamicClass>();

        var props = new DynamicProperty[]
        {
            new (field.Name, typeof(string))
        };

        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue(field.Name, field.Value);

        dynamicClasses.Add(dynamicClass);

        var query = dynamicClasses.AsQueryable();

        // Act
        var isValid = query.Any("firstName eq \"firstValue\"");

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void DynamicClassArray_Issue593_Fails()
    {
        // Arrange
        var field = new
        {
            Name = "firstName",
            Value = string.Concat("first", "Value")
        };
        var dynamicClasses = new List<DynamicClass>();

        var props = new DynamicProperty[]
        {
            new (field.Name, typeof(string))
        };

        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue(field.Name, field.Value);

        dynamicClasses.Add(dynamicClass);

        var query = dynamicClasses.AsQueryable();

        // Act
        var isValid = query.Any("firstName eq \"firstValue\"");

        // Assert
        isValid.Should().BeFalse(); // This should actually be true, but fails. For solution see Issue593_Solution1 and Issue593_Solution2.
    }

    [SkipIfGitHubActions]
    public void DynamicClassArray_Issue593_Solution1()
    {
        // Arrange
        var field = new
        {
            Name = "firstName",
            Value = string.Concat("first", "Value")
        };
        var dynamicClasses = new List<DynamicClass>();

        var props = new DynamicProperty[]
        {
            new (field.Name, typeof(string))
        };

        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue(field.Name, field.Value);

        dynamicClasses.Add(dynamicClass);

        var query = dynamicClasses.AsQueryable();

        // Act
        var isValid = query.Any("firstName.ToString() eq \"firstValue\"");

        // Assert
        isValid.Should().BeTrue();
    }

    [SkipIfGitHubActions]
    public void DynamicClassArray_Issue593_Solution2()
    {
        // Arrange
        var field = new
        {
            Name = "firstName",
            Value = string.Concat("first", "Value")
        };
        var dynamicClasses = new List<DynamicClass>();

        var props = new DynamicProperty[]
        {
            new (field.Name, typeof(string))
        };

        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue(field.Name, field.Value);

        dynamicClasses.Add(dynamicClass);

        var query = dynamicClasses.AsQueryable();

        // Act
        var isValid = query.Any("string(firstName) eq \"firstValue\"");

        // Assert
        isValid.Should().BeTrue();
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void ExpandoObject_SerializeToSystemTextJson_Should_Work()
    {
        // Arrange
        dynamic expandoObject = new ExpandoObject();
        expandoObject.Name = "Albert";
        expandoObject.Birthday = new DateTime(1879, 3, 14);

        // Act
        string json = Text.Json.JsonSerializer.Serialize(expandoObject);

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }

    [Fact(Skip = "fails")]
    public void DynamicClass_SerializeToSystemTextJson_Should_Work()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        // Act
        var json = Text.Json.JsonSerializer.Serialize(dynamicClass);

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }

    [Fact]
    public void DynamicClass_SerializeToSystemTextJson_TypeOfObject_Should_Work()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        // Act
        var json = Text.Json.JsonSerializer.Serialize(dynamicClass, typeof(object));

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }

    [Fact]
    public void DynamicClass_SerializeToSystemTextJson_TypeOfInstanceType_Should_Work()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        // Act
        var json = Text.Json.JsonSerializer.Serialize(dynamicClass, type);

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }

    [Fact(Skip = "fails")]
    public void DynamicClass_SerializeToSystemTextJson_TypeOfDynamicClass_Should_Work()
    {
        // Arrange
        var props = new[]
        {
            new DynamicProperty("Name", typeof(string)),
            new DynamicProperty("Birthday", typeof(DateTime))
        };
        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        // Act
        var json = Text.Json.JsonSerializer.Serialize(dynamicClass, typeof(DynamicClass));

        // Assert
        json.Should().Be("{\"Name\":\"Albert\",\"Birthday\":\"1879-03-14T00:00:00\"}");
    }
#endif
}