using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// Parse a Double and Single Quoted string.
    /// Some parts of the code is based on https://github.com/zzzprojects/Eval-Expression.NET
    /// </summary>
    internal static class StringParser
    {
        public static string ParseString(string s)
        {
            var inputStringBuilder = new StringBuilder(s);
            var tempStringBuilder = new StringBuilder();
            string found = null;

            char quote = inputStringBuilder[0];
            int pos = 1;

            while (pos < inputStringBuilder.Length)
            {
                char ch = inputStringBuilder[pos];

                if (ch == '\\' && pos + 1 < inputStringBuilder.Length && (inputStringBuilder[pos + 1] == '\\' || inputStringBuilder[pos + 1] == quote))
                {
                    tempStringBuilder.Append(inputStringBuilder[pos + 1]);
                    pos++; // Treat as escape character for \\ or \'
                }
                else if (ch == '\\' && pos + 1 < inputStringBuilder.Length && inputStringBuilder[pos + 1] == 'u')
                {
                    if (pos + 5 >= inputStringBuilder.Length)
                    {
                        throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.UnexpectedUnrecognizedEscapeSequence, pos, inputStringBuilder.ToString(pos, inputStringBuilder.Length - pos - 1)), pos);
                    }

                    string unicode = inputStringBuilder.ToString(pos, 6);
                    tempStringBuilder.Append(Regex.Unescape(unicode));
                    pos += 5;
                }
                else if (ch == quote)
                {
                    found = Replace(tempStringBuilder);
                    break;
                }
                else
                {
                    tempStringBuilder.Append(ch);
                }

                pos++;
            }

            if (found == null)
            {
                throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.UnexpectedUnclosedString, pos, inputStringBuilder.ToString()), pos);
            }

            return found;
        }

        private static string Replace(StringBuilder inputStringBuilder)
        {
            var sb = new StringBuilder(inputStringBuilder.ToString())
                .Replace(@"\\", "\\") // \\ – backslash
                .Replace(@"\0", "\0") // Unicode character 0
                .Replace(@"\a", "\a") // Alert(character 7)
                .Replace(@"\b", "\b") // Backspace(character 8)
                .Replace(@"\f", "\f") // Form feed(character 12)
                .Replace(@"\n", "\n") // New line(character 10)
                .Replace(@"\r", "\r") // Carriage return (character 13)
                .Replace(@"\t", "\t") // Horizontal tab(character 9)
                .Replace(@"\v", "\v") // Vertical quote(character 11)
            ; 

            return sb.ToString();
        }
    }
}
