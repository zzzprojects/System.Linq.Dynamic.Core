namespace System.Linq.Dynamic.Core;

/// <summary>
/// Defines the types of string literal parsing that can be performed.
/// </summary>
public enum StringLiteralParsingType : byte
{
    /// <summary>
    /// Represents the default string literal parsing type.
    /// [Default]
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a string literal parsing type where two consecutive double quotes are replaced by a single double quote.
    /// </summary>
    ReplaceTwoDoubleQuotesByASingleDoubleQuote = 1
}
