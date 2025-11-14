namespace System.Linq.Dynamic.Core.NewtonsoftJson.Config;

/// <summary>
/// Specifies the behavior to use when setting a property vlue that does not exist or is missing during normalization.
/// </summary>
public enum NormalizationNonExistingPropertyValueBehavior
{
    /// <summary>
    /// Specifies that the default value should be used.
    /// </summary>
    UseDefaultValue = 0,

    /// <summary>
    /// Specifies that null values should be used.
    /// </summary>
    UseNull = 1
}