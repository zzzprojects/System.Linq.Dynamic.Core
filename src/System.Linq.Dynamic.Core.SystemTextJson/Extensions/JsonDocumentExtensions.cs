using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson.Extensions;

/// <summary>
/// Copied from https://github.com/StefH/JsonConverter/tree/main/src/JsonConverter.System.Text.Json/Extensions/JsonDocumentExtensions.cs
/// </summary>
internal static class JsonDocumentExtensionsOld
{
    public static T? ToObject<T>(this JsonDocument document, JsonSerializerOptions options)
    {
        var rawText = document.RootElement.GetRawText();
        return JsonSerializer.Deserialize<T>(rawText, options);
    }
}