namespace System.Linq.Dynamic.Core;

/// <summary>
/// DynamicPropertyWithValue
/// </summary>
public class DynamicPropertyWithValue : DynamicProperty
{
    /// <summary>
    /// Gets the value from the property.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPropertyWithValue"/> class.
    /// </summary>
    /// <param name="name">The name from the property.</param>
    /// <param name="value">The value from the property.</param>
    public DynamicPropertyWithValue(string name, object? value) : base(name, value?.GetType() ?? typeof(object))
    {
        Value = value;
    }
}