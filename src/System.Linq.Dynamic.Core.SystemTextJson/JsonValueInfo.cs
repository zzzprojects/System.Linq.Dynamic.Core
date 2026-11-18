using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson;

internal readonly struct JsonValueInfo(JsonValueKind type, object? value)
{
    public JsonValueKind Type { get; } = type;

    public object? Value { get; } = value;
}