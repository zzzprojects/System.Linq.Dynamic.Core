namespace System.Linq.Dynamic.Core.Config;

/// <summary>
/// Defines the types of string literal parsing that can be performed.
/// </summary>
public enum StringLiteralParsingType : byte
{
    /// <summary>
    /// Represents the default string literal parsing type. Double quotes should be escaped using the default escape character (a \).
    /// To check if a Value equals a double quote, use this c# code:
    /// <code>
    /// var expression = "Value == \"\\\"\"";
    /// </code>
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a string literal parsing type where a double quote should be escaped by an extra double quote (").
    /// To check if a Value equals a double quote, use this c# code:
    /// <code>
    /// var expression = "Value == \"\"\"\"";
    /// </code>
    /// </summary>
    EscapeDoubleQuoteByTwoDoubleQuotes = 1
}