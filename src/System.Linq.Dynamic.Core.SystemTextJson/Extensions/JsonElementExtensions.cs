using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson.Extensions;

/// <summary>
/// Copied from https://github.com/StefH/JsonConverter/tree/main/src/JsonConverter.System.Text.Json/Extensions/JsonElementExtensions.cs
/// </summary>
internal static class JsonElementExtensions
{
    public static T? ToObject<T>(this JsonElement element, JsonSerializerOptions options)
    {
        var rawText = element.GetRawText();
        return JsonSerializer.Deserialize<T>(rawText, options);
    }
}