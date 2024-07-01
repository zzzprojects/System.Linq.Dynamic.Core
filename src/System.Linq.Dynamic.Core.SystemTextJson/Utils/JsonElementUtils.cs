using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson.Utils;

internal static class JsonElementUtils
{
    internal static JsonElement FromObject(object value)
    {
        var jsonString = JsonSerializer.Serialize(value);

        var doc = JsonDocument.Parse(jsonString);

        return doc.RootElement;
    }
}