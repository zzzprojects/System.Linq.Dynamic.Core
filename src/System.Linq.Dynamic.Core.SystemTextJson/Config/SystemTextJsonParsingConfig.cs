using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.SystemTextJson.Config;

public class SystemTextJsonParsingConfig : ParsingConfig
{
    public static SystemTextJsonParsingConfig Default { get; } = new();

    public DynamicJsonClassOptions? DynamicJsonClassOptions { get; set; } 
}