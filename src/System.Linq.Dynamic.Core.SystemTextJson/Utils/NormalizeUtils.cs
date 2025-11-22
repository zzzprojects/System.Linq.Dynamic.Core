using System.Collections.Generic;
using System.Linq.Dynamic.Core.SystemTextJson.Config;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace System.Linq.Dynamic.Core.SystemTextJson.Utils;

internal static class NormalizeUtils
{
    /// <summary>
    /// Normalizes a document so that each object contains all properties found in the array, including nested objects.
    /// </summary>
    internal static JsonDocument NormalizeJsonDocument(JsonDocument jsonDocument, NormalizationNonExistingPropertyBehavior normalizationBehavior)
    {
        if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new NotSupportedException("The source is not a JSON array.");
        }

        var jsonArray = JsonNode.Parse(jsonDocument.RootElement.GetRawText())!.AsArray();
        var normalizedArray = NormalizeJsonArray(jsonArray, normalizationBehavior);

        return JsonDocument.Parse(normalizedArray.ToJsonString());
    }

    /// <summary>
    /// Normalizes an array of JSON objects so that each object contains all properties found in the array, including nested objects.
    /// </summary>
    internal static JsonArray NormalizeJsonArray(JsonArray jsonArray, NormalizationNonExistingPropertyBehavior normalizationBehavior)
    {
        if (jsonArray.Any(item => item != null && item.GetValueKind() != JsonValueKind.Object))
        {
            return jsonArray;
        }

        var schema = BuildSchema(jsonArray);
        var normalizedArray = new JsonArray();

        foreach (var item in jsonArray)
        {
            if (item is JsonObject obj)
            {
                var normalizedObj = NormalizeObject(obj, schema, normalizationBehavior);
                normalizedArray.Add(normalizedObj);
            }
        }

        return normalizedArray;
    }

    private static Dictionary<string, JsonValueInfo> BuildSchema(JsonArray array)
    {
        var schema = new Dictionary<string, JsonValueInfo>();

        foreach (var item in array)
        {
            if (item is JsonObject obj)
            {
                MergeSchema(schema, obj);
            }
        }

        return schema;
    }

    private static void MergeSchema(Dictionary<string, JsonValueInfo> schema, JsonObject obj)
    {
        foreach (var prop in obj)
        {
            if (prop.Value is JsonObject nested)
            {
                if (!schema.TryGetValue(prop.Key, out var jsonValueInfo))
                {
                    jsonValueInfo = new JsonValueInfo(JsonValueKind.Object, new Dictionary<string, JsonValueInfo>());
                    schema[prop.Key] = jsonValueInfo;
                }

                MergeSchema((Dictionary<string, JsonValueInfo>)jsonValueInfo.Value!, nested);
            }
            else
            {
                if (!schema.ContainsKey(prop.Key))
                {
                    schema[prop.Key] = new JsonValueInfo(prop.Value?.GetValueKind() ?? JsonValueKind.Null, null);
                }
            }
        }
    }

    private static JsonObject NormalizeObject(JsonObject source, Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyBehavior normalizationBehavior)
    {
        var result = new JsonObject();

        foreach (var kvp in schema)
        {
            var key = kvp.Key;
            var jType = kvp.Value;

            if (jType.Value is Dictionary<string, JsonValueInfo> nestedSchema)
            {
                result[key] = source.ContainsKey(key) && source[key] is JsonObject jo ? NormalizeObject(jo, nestedSchema, normalizationBehavior) : CreateEmptyObject(nestedSchema, normalizationBehavior);
            }
            else
            {
                if (source.ContainsKey(key))
                {
                    var value = source[key];
#if NET8_0_OR_GREATER
                    result[key] = value?.DeepClone();
#else
                    result[key] = value != null ? JsonNode.Parse(value.ToJsonString()) : null;
#endif
                }
                else
                {
                    result[key] = normalizationBehavior == NormalizationNonExistingPropertyBehavior.UseDefaultValue ? GetDefaultValue(jType) : null;
                }
            }
        }

        return result;
    }

    private static JsonObject CreateEmptyObject(Dictionary<string, JsonValueInfo> schema, NormalizationNonExistingPropertyBehavior normalizationBehavior)
    {
        var obj = new JsonObject();
        foreach (var kvp in schema)
        {
            var key = kvp.Key;
            var jType = kvp.Value;

            if (jType.Value is Dictionary<string, JsonValueInfo> nestedSchema)
            {
                obj[key] = CreateEmptyObject(nestedSchema, normalizationBehavior);
            }
            else
            {
                obj[key] = normalizationBehavior == NormalizationNonExistingPropertyBehavior.UseDefaultValue ? GetDefaultValue(jType) : null;
            }
        }

        return obj;
    }

    private static JsonNode? GetDefaultValue(JsonValueInfo jType)
    {
        return jType.Type switch
        {
            JsonValueKind.Array => new JsonArray(),
            JsonValueKind.False => false,
            JsonValueKind.Number => default(int),
            JsonValueKind.String => string.Empty,
            JsonValueKind.True => false,
            _ => null,
        };
    }
}