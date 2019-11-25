using Sprache;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// See
    /// - https://thomaslevesque.com/tag/sprache/
    /// - https://stackoverflow.com/questions/32831401/sprache-parser-and-characters-escaping
    /// </summary>
    internal static class SpracheStringParser
    {
        private static Parser<char> DoubleQuote = Parse.Char('"');
        private static Parser<char> SingleQuote = Parse.Char('\'');
        private static Parser<char> Backslash = Parse.Char('\\');
        private static Parser<char> AnyCharExceptDoubleQuote = Parse.AnyChar.Except(DoubleQuote);
        private static Parser<char> AnyCharExceptSingleQuote = Parse.AnyChar.Except(SingleQuote);

        private static Parser<char> QuotedPair =
            from _ in Backslash
            from c in Parse.AnyChar
            select c;

        private static Parser<string> DoubleQuotedString =
            from open in DoubleQuote
            from text in QuotedPair.Or(AnyCharExceptDoubleQuote).Many().Text()
            from close in DoubleQuote
            select text;

        private static Parser<string> SingleQuotedString =
            from open in SingleQuote
            from text in AnyCharExceptSingleQuote.Many().Text()
            from close in SingleQuote
            select text;

        public static string ParseString(string s)
        {
            var tp = s[0] == '\'' ? SingleQuotedString : DoubleQuotedString;

            return tp.Parse(s);
        }
    }
}
