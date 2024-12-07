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
    public new static NewtonsoftJsonParsingConfig Default { get; } = new();

    /// <summary>
    /// The default <see cref="DynamicJsonClassOptions"/> to use.
    /// </summary>
    public DynamicJsonClassOptions? DynamicJsonClassOptions { get; set; } 
}