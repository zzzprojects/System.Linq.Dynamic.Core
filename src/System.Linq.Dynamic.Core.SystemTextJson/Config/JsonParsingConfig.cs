using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.SystemTextJson.Config;

public class JsonParsingConfig : ParsingConfig
{
    public static JsonParsingConfig Default { get; } = new();

    public DynamicJsonClassOptions? DynamicJsonClassOptions { get; set; } 
}