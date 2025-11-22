using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Config;

/// <summary>
/// Configuration class for System.Linq.Dynamic.Core.NewtonsoftJson which implements the <see cref="ParsingConfig"/>.
/// </summary>
public class NewtonsoftJsonParsingConfig : ParsingConfig
{
    /// <summary>
    /// The default ParsingConfig for <see cref="NewtonsoftJsonParsingConfig"/>.
    /// </summary>
    public new static NewtonsoftJsonParsingConfig Default { get; } = new NewtonsoftJsonParsingConfig
    {
        ConvertObjectToSupportComparison = true
    };

    /// <summary>
    /// The default <see cref="DynamicJsonClassOptions"/> to use.
    /// </summary>
    public DynamicJsonClassOptions? DynamicJsonClassOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the objects in an array should be normalized before processing.
    /// </summary>
    public bool Normalize { get; set; } = true;

    /// <summary>
    /// Gets or sets the behavior to apply when a property value does not exist during normalization.
    /// </summary>
    /// <remarks>
    /// Use this property to control how the normalization process handles properties that are missing or undefined.
    /// The selected behavior may affect the output or error handling of normalization operations.
    /// The default value is <see cref="NormalizationNonExistingPropertyBehavior.UseNull"/>.
    /// </remarks>
    public NormalizationNonExistingPropertyBehavior NormalizationNonExistingPropertyValueBehavior { get; set; }
}