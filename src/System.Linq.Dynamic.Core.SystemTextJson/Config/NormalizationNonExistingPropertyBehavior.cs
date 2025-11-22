namespace System.Linq.Dynamic.Core.SystemTextJson.Config;

/// <summary>
/// Specifies the behavior to use when setting a property value that does not exist or is missing during normalization.
/// </summary>
public enum NormalizationNonExistingPropertyBehavior
{
    /// <summary>
    /// Specifies that a null value should be used.
    /// </summary>
    UseNull = 0,

    /// <summary>
    /// Specifies that the default value should be used.
    /// </summary>
    UseDefaultValue = 1    
}