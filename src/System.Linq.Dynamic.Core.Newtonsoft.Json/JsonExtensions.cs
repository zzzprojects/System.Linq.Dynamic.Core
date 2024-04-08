using System.Linq.Dynamic.Core.Validation;
using Newtonsoft.Json.Linq;

namespace System.Linq.Dynamic.Core.Newtonsoft.Json;

public static class JsonExtensions
{
    public static JArray Where(this JArray source, JsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNullOrEmpty(predicate);

        var array = new JArray();

        if (source.Count == 0)
        {
            return array;
        }

        // Convert the JArray to a dynamic object array / queryable.
        var enumerable = source.ToDynamicJsonClassArray(config.DynamicJsonClassOptions).AsQueryable();

        // Apply the where clause.
        var results = enumerable.Where(config, predicate, args);

        // Convert the dynamic results back to a JArray.
        foreach (var result in results)
        {
            array.Add(JObject.FromObject(result));
        }

        return array;
    }

    public static JArray Where(this JArray source, string predicate, params object?[] args)
    {
        return Where(source, JsonParsingConfig.Default, predicate, args);
    }

    //private static object? ConvertToDynamicClass(object value, DynamicJsonClassOptions? options = null)
    //{
    //    Check.NotNull(value);

    //    if (value is JObject jObject)
    //    {
    //        return jObject.ToDynamicClass(options);
    //    }

    //    if (value is JArray jArray)
    //    {
    //        return jArray.ToDynamicJsonClassArray(options);
    //    }

    //    if (value is JValue jValue)
    //    {
    //        return jValue.ToDynamicClass(options);
    //    }

    //    if (value is JToken jToken)
    //    {
    //        return jToken.ToDynamicClass(options);
    //    }

    //    return value;
    //}
}