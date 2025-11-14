namespace System.Linq.Dynamic.Core.SystemTextJson.Config;

/// <summary>
/// Configuration class for System.Linq.Dynamic.Core.SystemTextJson which implements the <see cref="ParsingConfig"/>.
/// </summary>
public class SystemTextJsonParsingConfig : ParsingConfig
{
    /// <summary>
    /// The default ParsingConfig for <see cref="SystemTextJsonParsingConfig"/>.
    /// </summary>
    public new static SystemTextJsonParsingConfig Default { get; } = new SystemTextJsonParsingConfig
    {
        ConvertObjectToSupportComparison = true
    };

    /// <summary>
    /// Gets or sets a value indicating whether the objecs in an array should be normalized before processing.
    /// </summary>
    public bool Normalize { get; set; } = true;

    /// <summary>
    /// Gets or sets the behavior to apply when a property value does not exist during normalization.
    /// </summary>
    /// <remarks>
    /// Use this property to control how the normalization process handles properties that are missing or undefined.
    /// The selected behavior may affect the output or error handling of normalization operations.
    /// The default value is <see cref="NormalizationNonExistingPropertyValueBehavior.UseDefaultValue"/>.
    /// </remarks>
    public NormalizationNonExistingPropertyValueBehavior NormalizationNonExistingPropertyValueBehavior { get; set; }
}