using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.SystemTextJson.Models;
using System.Reflection;
using System.Text.Json;
using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.SystemTextJson.Extensions;

internal static class JsonDocumentExtensions
{
    private class JTokenResolvers : Dictionary<JsonValueKind, Func<JsonElement, DynamicJsonClassOptions?, object?>>
    {
    }

    private static readonly JTokenResolvers Resolvers = new()
    {
        { JsonValueKind.Array, ConvertJsonElementToEnumerable },
        { JsonValueKind.False, (_, _) => false },
        { JsonValueKind.True, (_, _) => true },
        { JsonValueKind.Number, ConvertNumber },
        { JsonValueKind.Null, (_, _) => null },
        { JsonValueKind.Object, ConvertJsonElement },
        { JsonValueKind.String, ConvertString },
        { JsonValueKind.Undefined, (_, _) => null }
    };

    internal static DynamicClass? ToDynamicClass(this JsonElement? src, DynamicJsonClassOptions? options = null)
    {
        if (src == null)
        {
            return null;
        }

        var dynamicPropertiesWithValue = new List<DynamicPropertyWithValue>();

        foreach (var prop in src.Value.EnumerateObject())
        {
            var value = Resolvers[prop.Value.ValueKind](prop.Value, options);
            if (value != null)
            {
                dynamicPropertiesWithValue.Add(new DynamicPropertyWithValue(prop.Name, value));
            }
        }

        return CreateInstance(dynamicPropertiesWithValue);
    }

    public static IEnumerable ToDynamicJsonClassArray(this JsonElement? src, DynamicJsonClassOptions? options = null)
    {
        return src == null ? Array.Empty<object?>() : ConvertJsonElementToEnumerable(src.Value, options);
    }

    private static object? ConvertJsonElement(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        return arg.ValueKind == JsonValueKind.Object ? ToDynamicClass(arg, options) : GetResolverFor(arg)(arg, options);
    }

    private static object PassThrough(JsonElement arg, DynamicJsonClassOptions? options)
    {
        return arg;
    }

    private static Func<JsonElement, DynamicJsonClassOptions?, object?> GetResolverFor(JsonElement arg)
    {
        return Resolvers.TryGetValue(arg.ValueKind, out var result) ? result : PassThrough;
    }

    private static object? ConvertString(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.TryGetGuid(out var guid))
        {
            return guid;
        }

        if (arg.TryGetDateTime(out var dt))
        {
            return dt;
        }

        return arg.GetString();
    }

    private static object ConvertNumber(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.TryGetInt32(out var int32))
        {
            return int32;
        }

        if (arg.TryGetInt64(out var int64))
        {
            return int64;
        }

        if (arg.TryGetDouble(out var @double))
        {
            return @double;
        }

        if (arg.TryGetDecimal(out var @decimal))
        {
            return @decimal;
        }

        if (arg.TryGetByte(out var @byte))
        {
            return @byte;
        }

        throw new InvalidOperationException($"Unable to convert {nameof(JsonElement)} of type: {arg.ValueKind} to int, long, double or decimal.");
    }

    private static IEnumerable ConvertJsonElementToEnumerable(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        var result = new List<object?>();
        foreach (var item in arg.EnumerateArray())
        {
            result.Add(ConvertJsonElement(item));
        }

        var distinctType = FindSameTypeOf(result);
        return distinctType == null ? result.ToArray() : ConvertToTypedArray(result, distinctType);
    }

    private static Type? FindSameTypeOf(IEnumerable<object?> src)
    {
        var types = src.Select(o => o?.GetType()).Distinct().OfType<Type>().ToArray();
        return types.Length == 1 ? types[0] : null;
    }

    private static IEnumerable ConvertToTypedArray(IEnumerable<object?> src, Type newType)
    {
        var method = ConvertToTypedArrayGenericMethod.MakeGenericMethod(newType);
        return (IEnumerable)method.Invoke(null, new object[] { src })!;
    }

    private static readonly MethodInfo ConvertToTypedArrayGenericMethod = typeof(JsonDocumentExtensions).GetMethod(nameof(ConvertToTypedArrayGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static T[] ConvertToTypedArrayGeneric<T>(IEnumerable<object> src)
    {
        return src.Cast<T>().ToArray();
    }

    private static DynamicClass CreateInstance(IList<DynamicPropertyWithValue> dynamicPropertiesWithValue)
    {
        var type = DynamicClassFactory.CreateType(dynamicPropertiesWithValue.Cast<DynamicProperty>().ToArray());
        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        foreach (var dynamicPropertyWithValue in dynamicPropertiesWithValue.Where(p => p.Value != null))
        {
            dynamicClass.SetDynamicPropertyValue(dynamicPropertyWithValue.Name, dynamicPropertyWithValue.Value!);
        }

        return dynamicClass;
    }
}