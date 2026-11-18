using Newtonsoft.Json.Linq;

namespace System.Linq.Dynamic.Core.NewtonsoftJson;

internal readonly struct JsonValueInfo(JTokenType type,  object? value)
{
    public JTokenType Type { get; } = type;

    public object? Value { get; } = value;
}