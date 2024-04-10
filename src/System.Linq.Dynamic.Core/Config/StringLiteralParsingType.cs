namespace System.Linq.Dynamic.Core.Config;

/// <summary>
/// Defines the types of string literal parsing that can be performed.
/// </summary>
public enum StringLiteralParsingType : byte
{
    /// <summary>
    /// Double quotes should be escaped using the default escape character: Backslash. 
    /// E.G. var expression = "StaticHelper.Filter(\"UserName == \\\"x\\\"\")";
    /// 
    /// [Default]
    /// </summary>
    Default = 0,

    /// <summary>
    /// Double quotes should be escaped by a double quote.
    /// E.G. var expression = "StaticHelper.Filter(\"UserName == \"\"x\"\"\")";
    /// </summary>
    EscapeDoubleQuoteByTwoDoubleQuotes = 1
}