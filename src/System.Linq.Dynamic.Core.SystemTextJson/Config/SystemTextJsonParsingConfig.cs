namespace System.Linq.Dynamic.Core.SystemTextJson.Config;

/// <summary>
/// Configuration class for System.Linq.Dynamic.Core.SystemTextJson which implements the <see cref="ParsingConfig"/>.
/// </summary>
public class SystemTextJsonParsingConfig : ParsingConfig
{
    /// <summary>
    /// The default ParsingConfig for <see cref="SystemTextJsonParsingConfig"/>.
    /// </summary>
    public new static SystemTextJsonParsingConfig Default { get; } = new();
}