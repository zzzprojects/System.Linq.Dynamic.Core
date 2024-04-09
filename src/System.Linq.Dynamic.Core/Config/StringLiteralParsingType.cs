namespace System.Linq.Dynamic.Core.Config;

/// <summary>
/// Defines the types of string literal parsing that can be performed.
/// </summary>
public enum StringLiteralParsingType : byte
{
    /// <summary>
    /// Represents the default string literal parsing type. Double quotes should be escaped using the default escaping.
    /// E.G. var expression = "StaticHelper.Filter(\"UserName == \\\"x\\\"\")";
    /// 
    /// [Default]
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a string literal parsing type where a double quotes should be escaped by two double quotes.
    /// E.G. var expression = "StaticHelper.Filter(\"UserName == "\"\"x\"\"\")";
    /// </summary>
    EscapeDoubleQuoteByTwoDoubleQuotes = 1
}