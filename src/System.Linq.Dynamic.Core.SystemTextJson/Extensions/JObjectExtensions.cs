using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.SystemTextJson.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.SystemTextJson.Extensions;

/// <summary>
/// Based on https://github.com/StefH/JsonConverter/blob/main/src/JsonConverter.Newtonsoft.Json/Extensions/JObjectExtensions.cs
/// </summary>
internal static class JsonDocumentExtensions
{
    private class JTokenResolvers : Dictionary<JsonValueKind, Func<JsonElement, DynamicJsonClassOptions?, object?>>
    {
    }

    private static readonly JTokenResolvers Resolvers = new()
    {
        { JsonValueKind.Array, ConvertJTokenArray },
        { JsonValueKind.Boolean, (jToken, _) => jToken.Value<bool>() },
        { JsonValueKind.Bytes, (jToken, _) => jToken.Value<byte[]>() },
        { JsonValueKind.Date, (jToken, _) => jToken.Value<DateTime>() },
        { JsonValueKind.Float, ConvertJTokenFloat },
        { JsonValueKind.Guid, (jToken, _) => jToken.Value<Guid>() },
        { JsonValueKind.Integer, ConvertJTokenInteger },
        { JsonValueKind.None, (_, _) => null },
        { JsonValueKind.Null, (_, _) => null },
        { JsonValueKind.Object, ConvertJObject },
        { JsonValueKind.Property, ConvertJTokenProperty },
        { JsonValueKind.String, (jToken, _) => jToken.Value<string>() },
        { JsonValueKind.TimeSpan, (jToken, _) => jToken.Value<TimeSpan>() },
        { JsonValueKind.Undefined, (_, _) => null },
        { JsonValueKind.Uri, (o, _) => o.Value<Uri>() },
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

        var dynamicPropertyWithValues = new List<DynamicPropertyWithValue>();

        foreach (var prop in src.RootElement.EnumerateObject())
        {
            var value = Resolvers[prop.Value.ValueKind](prop.Value, options);
            if (value != null)
            {
                dynamicPropertyWithValues.Add(new DynamicPropertyWithValue(prop.Key, value));
            }
        }

        return CreateInstance(dynamicPropertyWithValues);
    }

    internal static IEnumerable ToDynamicJsonClassArray(this JArray? src, DynamicJsonClassOptions? options = null)
    {
        return src == null ? new object?[0] : ConvertJTokenArray(src, options);
    }

    internal static object? ToDynamicClass(this JToken? src, DynamicJsonClassOptions? options = null)
    {
        return src == null ? null : GetResolverFor(src)(src, options);
    }

    private static object? ConvertJObject(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg is JObject asJObject)
        {
            return asJObject.ToDynamicClass(options);
        }

        return GetResolverFor(arg)(arg, options);
    }

    private static object PassThrough(JToken arg, DynamicJsonClassOptions? options)
    {
        return arg;
    }

    private static Func<JToken, DynamicJsonClassOptions?, object?> GetResolverFor(JToken arg)
    {
        return Resolvers.TryGetValue(arg.Type, out var result) ? result : PassThrough;
    }

    private static object ConvertJTokenFloat(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.Type != JTokenType.Float)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to double or float.");
        }

        if (options?.FloatConvertBehavior == FloatBehavior.UseFloat)
        {
            try
            {
                return arg.Value<float>();
            }
            catch
            {
                return arg.Value<double>();
            }
        }

        if (options?.FloatConvertBehavior == FloatBehavior.UseDecimal)
        {
            try
            {
                return arg.Value<decimal>();
            }
            catch
            {
                return arg.Value<double>();
            }
        }


        return arg.Value<double>();
    }

    private static object ConvertJTokenInteger(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg.Type != JTokenType.Integer)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to long or int.");
        }

        var longValue = arg.Value<long>();

        if (options is null || options.IntegerConvertBehavior == IntegerBehavior.UseInt)
        {
            if (longValue is >= int.MinValue and <= int.MaxValue)
            {
                return Convert.ToInt32(longValue);
            }
        }

        return longValue;
    }

    private static object? ConvertJTokenProperty(JToken arg, DynamicJsonClassOptions? options = null)
    {
        var resolver = GetResolverFor(arg);
        if (resolver is null)
        {
            throw new InvalidOperationException($"Unable to handle {nameof(JToken)} of type: {arg.Type}.");
        }

        return resolver(arg, options);
    }

    private static IEnumerable ConvertJTokenArray(JToken arg, DynamicJsonClassOptions? options = null)
    {
        if (arg is not JArray array)
        {
            throw new InvalidOperationException($"Unable to convert {nameof(JToken)} of type: {arg.Type} to {nameof(JArray)}.");
        }

        var result = new List<object?>();
        foreach (var item in array)
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