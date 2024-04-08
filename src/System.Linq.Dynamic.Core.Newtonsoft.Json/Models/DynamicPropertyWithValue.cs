namespace System.Linq.Dynamic.Core.Newtonsoft.Json.Models;

internal class DynamicPropertyWithValue : DynamicProperty
{
    public object? Value { get; }

    public DynamicPropertyWithValue(string name, object? value) : base(name, value?.GetType() ?? typeof(object))
    {
        Value = value;
    }
}