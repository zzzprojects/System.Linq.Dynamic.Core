using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson.Utils;

internal static class JsonDocumentUtils
{
    internal static JsonDocument FromObject(object value)
    {
        var jsonString = JsonSerializer.Serialize(value);

        return JsonDocument.Parse(jsonString);
    }
}