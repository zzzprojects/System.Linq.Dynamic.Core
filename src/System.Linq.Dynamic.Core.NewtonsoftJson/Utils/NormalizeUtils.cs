using System.Collections.Generic;
using System.Linq.Dynamic.Core.NewtonsoftJson.Config;
using ConsoleApp3;
using Newtonsoft.Json.Linq;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Utils;

internal static class NormalizeUtils
{
    /// <summary>
    /// Normalizes an array of JSON objects so that each object contains all properties found in the array,
    /// including nested objects. Missing properties will have null values.
    /// </summary>
    internal static JArray NormalizeArray(JArray jsonArray, NormalizationNonExistingPropertyValueBehavior normalizationBehavior)
    {
        if (jsonArray.Count(item => item is not JObject) > 0)
        {
            return jsonArray;
        }

        var schema = BuildSchema(jsonArray);
        var normalizedArray = new JArray();

        foreach (var jo in jsonArray.OfType<JObject>())
        {
            var normalizedObj = NormalizeObject(jo, schema, normalizationBehavior);
            normalizedArray.Add(normalizedObj);
        }

        return normalizedArray;
    }

    private static Dictionary<string, JsonValueInfo> BuildSchema(JArray array)
    {
        var schema = new Dictionary<string, JsonValueInfo>();

        foreach (var item in array)
        {
            if (item is JObject obj)
            {
                MergeSchema(schema, obj);
            }
        }

        return schema;
    }

    private static void MergeSchema(Dictionary<string, JsonValueInfo> schema, JObject obj)
    {
        foreach (var prop in obj.Properties())
        {
            if (prop.Value is JObject nested)
            {
                if (!schema.ContainsKey(prop.Name))
                {
                    schema[prop.Name] = new JsonValueInfo(JTokenType.Object, new Dictionary<string, JsonValueInfo>());
                }

                MergeSchema((Dictionary<string, JsonValueInfo>)schema[prop.Name].Value!, nested);
            }
            else
            {
                if (!schema.ContainsKey(prop.Name))
                {
                    schema[prop.Name] = new JsonValueInfo(prop.Value.Type, null);
                }
            }
        }
    }

    private static JObject NormalizeObject(JObject source, Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyValueBehavior normalizationBehavior)
    {
        var result = new JObject();

        foreach (var key in schema.Keys)
        {
            if (schema[key].Value is Dictionary<string, JsonValueInfo> nestedSchema)
            {
                result[key] = source.ContainsKey(key) && source[key] is JObject jo ? NormalizeObject(jo, nestedSchema, normalizationBehavior) : CreateEmptyObject(nestedSchema, normalizationBehavior);
            }
            else
            {
                if (source.ContainsKey(key))
                {
                    result[key] = source[key];
                }
                else
                {
                    result[key] = normalizationBehavior == NormalizationNonExistingPropertyValueBehavior.UseDefaultValue ? GetDefaultValue(schema[key]) : JValue.CreateNull();
                }
            }
        }

        return result;
    }

    private static JObject CreateEmptyObject(Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyValueBehavior normalizationBehavior)
    {
        var obj = new JObject();
        foreach (var key in schema.Keys)
        {
            if (schema[key].Value is Dictionary<string, JsonValueInfo> nestedSchema)
            {
                obj[key] = CreateEmptyObject(nestedSchema, normalizationBehavior);
            }
            else
            {
                obj[key] = normalizationBehavior == NormalizationNonExistingPropertyValueBehavior.UseDefaultValue ? GetDefaultValue(schema[key]) : JValue.CreateNull();
            }
        }

        return obj;
    }

    private static JToken GetDefaultValue(JsonValueInfo jType)
    {
        return jType.Type switch
        {
            JTokenType.Array => new JArray(),
            JTokenType.Boolean => default(bool),
            JTokenType.Bytes => new byte[0],
            JTokenType.Date => DateTime.MinValue,
            JTokenType.Float => default(float),
            JTokenType.Guid => Guid.Empty,
            JTokenType.Integer => default(int),
            JTokenType.String => string.Empty,
            JTokenType.TimeSpan => TimeSpan.MinValue,
            _ => JValue.CreateNull(),
        };
    }
}