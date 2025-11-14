using System.Text.Json;

internal struct JsonValueInfo(JsonValueKind type, object? value)
{
    public JsonValueKind Type { get; } = type;

    public object? Value { get; } = value;
}