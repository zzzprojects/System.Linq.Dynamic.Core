using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
using System.Text.RegularExpressions;

namespace System.Linq.Dynamic.Core.Parser;

/// <summary>
/// Parse a Double and Single Quoted string.
/// Some parts of the code is based on https://github.com/zzzprojects/Eval-Expression.NET
/// </summary>
internal static class StringParser
{
    private const string TwoDoubleQuotes = "\"\"";
    private const string SingleDoubleQuote = "\"";

    internal static string ParseString(string s, int pos = default)
    {
        if (s == null || s.Length < 2)
        {
            throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.InvalidStringLength, s, 2), pos);
        }

        if (s[0] != '"' && s[0] != '\'')
        {
            throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.InvalidStringQuoteCharacter), pos);
        }

        char quote = s[0]; // This can be single or a double quote
        if (s.Last() != quote)
        {
            throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.UnexpectedUnclosedString, s.Length, s), pos);
        }

        try
        {
            return Regex.Unescape(s.Substring(1, s.Length - 2));
        }
        catch (Exception ex)
        {
            throw new ParseException(ex.Message, pos, ex);
        }
    }

    internal static string ParseStringAndEscapeTwoDoubleQuotesByASingleDoubleQuote(string input, int position)
    {
        return ReplaceTwoDoubleQuotesByASingleDoubleQuote(ParseString(input, position), position);
    }

    private static string ReplaceTwoDoubleQuotesByASingleDoubleQuote(string input, int position)
    {
        try
        {
            return Regex.Replace(input, TwoDoubleQuotes, SingleDoubleQuote);
        }
        catch (Exception ex)
        {
            throw new ParseException(ex.Message, position, ex);
        }
    }
}