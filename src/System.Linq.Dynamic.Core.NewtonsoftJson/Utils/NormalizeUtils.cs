using System.Collections.Generic;
using System.Linq.Dynamic.Core.NewtonsoftJson.Config;
using Newtonsoft.Json.Linq;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Utils;

internal static class NormalizeUtils
{
    /// <summary>
    /// Normalizes an array of JSON objects so that each object contains all properties found in the array,
    /// including nested objects. Missing properties will have null values.
    /// </summary>
    internal static JArray NormalizeArray(JArray jsonArray, NormalizationNonExistingPropertyBehavior normalizationBehavior)
    {
        if (jsonArray.Any(item => item is not JObject))
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
                if (!schema.TryGetValue(prop.Name, out var jsonValueInfo))
                {
                    jsonValueInfo = new JsonValueInfo(JTokenType.Object, new Dictionary<string, JsonValueInfo>());
                    schema[prop.Name] = jsonValueInfo;
                }

                MergeSchema((Dictionary<string, JsonValueInfo>)jsonValueInfo.Value!, nested);
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

    private static JObject NormalizeObject(JObject source, Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyBehavior normalizationBehavior)
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
                    result[key] = GetDefaultOrNullValue(normalizationBehavior, schema[key]);
                }
            }
        }

        return result;
    }

    private static JObject CreateEmptyObject(Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyBehavior normalizationBehavior)
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
                obj[key] = GetDefaultOrNullValue(normalizationBehavior, schema[key]);
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
            _ => GetNullValue(jType),
        };
    }

    private static JValue GetNullValue(JsonValueInfo jType)
    {
        return jType.Type switch
        {
            JTokenType.Boolean => new JValue((bool?)null),
            JTokenType.Bytes => new JValue((byte[]?)null),
            JTokenType.Date => new JValue((DateTime?)null),
            JTokenType.Float => new JValue((float?)null),
            JTokenType.Guid => new JValue((Guid?)null),
            JTokenType.Integer => new JValue((int?)null),
            JTokenType.String => new JValue((string?)null),
            JTokenType.TimeSpan => new JValue((TimeSpan?)null),
            _ => JValue.CreateNull(),
        };
    }

    private static JToken GetDefaultOrNullValue(NormalizationNonExistingPropertyBehavior behavior, JsonValueInfo jType)
    {
        return behavior == NormalizationNonExistingPropertyBehavior.UseDefaultValue ? GetDefaultValue(jType) : GetNullValue(jType);
    }
}