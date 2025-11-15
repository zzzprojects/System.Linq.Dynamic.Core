#if !NET8_0_OR_GREATER
namespace System.Text.Json.Nodes;

internal static class JsonValueExtensions
{
    internal static JsonValueKind? GetValueKind(this JsonNode node)
    {
        if (node is JsonObject)
        {
            return JsonValueKind.Object;
        }

        if (node is JsonArray)
        {
            return JsonValueKind.Array;
        }

        return node.GetValue<object>() is JsonElement je ? je.ValueKind : null;
    }
}
#endif