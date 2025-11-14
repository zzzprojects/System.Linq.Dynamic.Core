using Newtonsoft.Json.Linq;

namespace ConsoleApp3;

internal struct JsonValueInfo(JTokenType type,  object? value)
{
    public JTokenType Type { get; } = type;

    public object? Value { get; } = value;
}