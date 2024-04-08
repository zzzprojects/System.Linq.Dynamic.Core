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
        { JsonValueKind.Array, ConvertJTokenArray },
        { JsonValueKind.False, (_, _) => false },
        { JsonValueKind.True, (_, _) => true },
        // { JsonValueKind.Bytes, (jToken, _) => jToken.Value<byte[]>() },
        // { JsonValueKind.Date, (jToken, _) => jToken.Value<DateTime>() },
        // { JsonValueKind.Float, ConvertJTokenFloat },
        // { JsonValueKind.Guid, (jToken, _) => jToken.Value<Guid>() },
        { JsonValueKind.Number, ConvertNumber },
        // { JsonValueKind.None, (_, _) => null },
        { JsonValueKind.Null, (_, _) => null },
        { JsonValueKind.Object, ConvertJObject },
        // { JsonValueKind.Property, ConvertJTokenProperty },
        { JsonValueKind.String, ConvertString },
        // { JsonValueKind.TimeSpan, (jToken, _) => jToken.Value<TimeSpan>() },
        { JsonValueKind.Undefined, (_, _) => null },
        // { JsonValueKind.Uri, (o, _) => o.Value<Uri>() },
    };

    //internal static object? ToDynamicClass(this JValue src)
    //{
    //    return src.Value;
    //}

    internal static DynamicClass? ToDynamicClass(this JsonDocument? src, DynamicJsonClassOptions? options = null)
    {
        if (src == null)
        {
            return null;
        }

        var dynamicPropertiesWithValue = new List<DynamicPropertyWithValue>();

        foreach (var prop in src.RootElement.EnumerateObject())
        {
            var value = Resolvers[prop.Value.ValueKind](prop.Value, options);
            if (value != null)
            {
                dynamicPropertiesWithValue.Add(new DynamicPropertyWithValue(prop.Name, value));
            }
        }

        return CreateInstance(dynamicPropertiesWithValue);
    }

    internal static IEnumerable ToDynamicJsonClassArray(this JsonElement? src, DynamicJsonClassOptions? options = null)
    {
        return src == null ? new object?[0] : ConvertJTokenArray(src.Value, options);
    }

    internal static object? ToDynamicClass(this JsonElement? src, DynamicJsonClassOptions? options = null)
    {
        return src == null ? null : GetResolverFor(src.Value)(src.Value, options);
    }

    private static object? ConvertJObject(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        //if (arg is JObject asJObject)
        //{
        //    return asJObject.ToDynamicClass(options);
        //}

        return GetResolverFor(arg)(arg, options);
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

        if (arg.TryGetByte(out var @byte))
        {
            return @byte;
        }

        if (arg.TryGetBytesFromBase64(out var base64))
        {
            return base64;
        }

        return arg.GetString();
    }

    private static object ConvertNumber(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        //if (arg.ValueKind != JsonValueKind.Number)
        //{
        //    throw new InvalidOperationException($"Unable to convert {nameof(JsonElement)} of type: {arg.ValueKind} to Number.");
        //}

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

        throw new InvalidOperationException($"Unable to convert {nameof(JsonElement)} of type: {arg.ValueKind} to int, long, double or decimal.");
    }

    //private static object? ConvertJTokenProperty(JToken arg, DynamicJsonClassOptions? options = null)
    //{
    //    var resolver = GetResolverFor(arg);
    //    if (resolver is null)
    //    {
    //        throw new InvalidOperationException($"Unable to handle {nameof(JToken)} of type: {arg.Type}.");
    //    }

    //    return resolver(arg, options);
    //}

    private static IEnumerable ConvertJTokenArray(JsonElement arg, DynamicJsonClassOptions? options = null)
    {
        //if (arg is not JArray array)
        //{
        //    throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to {nameof(JArray)}.");
        //}

        var result = new List<object?>();
        foreach (var item in arg.EnumerateArray())
        {
            result.Add(ConvertJObject(item));
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