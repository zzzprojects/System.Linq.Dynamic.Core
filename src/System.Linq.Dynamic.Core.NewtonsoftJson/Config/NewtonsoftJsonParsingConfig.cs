using JsonConverter.Abstractions.Models;

namespace System.Linq.Dynamic.Core.NewtonsoftJson.Config;

public class NewtonsoftJsonParsingConfig : ParsingConfig
{
    public static NewtonsoftJsonParsingConfig Default { get; } = new();

    public DynamicJsonClassOptions? DynamicJsonClassOptions { get; set; } 
}