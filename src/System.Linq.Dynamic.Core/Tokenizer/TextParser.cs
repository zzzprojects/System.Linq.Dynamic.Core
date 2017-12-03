using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;

namespace System.Linq.Dynamic.Core.Tokenizer
{
    internal class TextParser
    {
        private static char NumberDecimalSeparator = '.';

        // These aliases are supposed to simply the where clause and make it more human readable
        // As an addition it is compatible with the OData.Filter specification
        private static readonly Dictionary<string, TokenId> _predefinedAliases = new Dictionary<string, TokenId>
        {
            {"eq", TokenId.Equal},
            {"ne", TokenId.ExclamationEqual},
            {"neq", TokenId.ExclamationEqual},
            {"lt", TokenId.LessThan},
            {"le", TokenId.LessThanEqual},
            {"gt", TokenId.GreaterThan},
            {"ge", TokenId.GreaterThanEqual},
            {"and", TokenId.DoubleAmphersand},
            {"or", TokenId.DoubleBar},
            {"not", TokenId.Exclamation},
            {"mod", TokenId.Percent}
        };

        private readonly string _text;
        private readonly int _textLen;

        private int _textPos;
        private char _ch;
        public Token CurrentToken;

        public TextParser(string text)
        {
            _text = text;
            _textLen = _text.Length;
            SetTextPos(0);
            NextToken();
        }

        private void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        private void NextChar()
        {
            if (_textPos < _textLen) _textPos++;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(_ch))
            {
                NextChar();
            }

            TokenId tokenId = TokenId.Unknown;
            int tokenPos = _textPos;

            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        tokenId = TokenId.Exclamation;
                    }
                    break;

                case '%':
                    NextChar();
                    tokenId = TokenId.Percent;
                    break;

                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        tokenId = TokenId.Amphersand;
                    }
                    break;

                case '(':
                    NextChar();
                    tokenId = TokenId.OpenParen;
                    break;

                case ')':
                    NextChar();
                    tokenId = TokenId.CloseParen;
                    break;

                case '{':
                    NextChar();
                    tokenId = TokenId.OpenCurlyParen;
                    break;

                case '}':
                    NextChar();
                    tokenId = TokenId.CloseCurlyParen;
                    break;

                case '*':
                    NextChar();
                    tokenId = TokenId.Asterisk;
                    break;

                case '+':
                    NextChar();
                    tokenId = TokenId.Plus;
                    break;

                case ',':
                    NextChar();
                    tokenId = TokenId.Comma;
                    break;

                case '-':
                    NextChar();
                    tokenId = TokenId.Minus;
                    break;

                case '.':
                    NextChar();
                    tokenId = TokenId.Dot;
                    break;

                case '/':
                    NextChar();
                    tokenId = TokenId.Slash;
                    break;

                case ':':
                    NextChar();
                    tokenId = TokenId.Colon;
                    break;

                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.LessGreater;
                    }
                    else if (_ch == '<')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleLessThan;
                    }
                    else
                    {
                        tokenId = TokenId.LessThan;
                    }
                    break;

                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.Lambda;
                    }
                    else
                    {
                        tokenId = TokenId.Equal;
                    }
                    break;

                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.GreaterThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleGreaterThan;
                    }
                    else
                    {
                        tokenId = TokenId.GreaterThan;
                    }
                    break;

                case '?':
                    NextChar();
                    if (_ch == '?')
                    {
                        NextChar();
                        tokenId = TokenId.NullCoalescing;
                    }
                    else if (_ch == '.')
                    {
                        NextChar();
                        tokenId = TokenId.NullPropagation;
                    }
                    else
                    {
                        tokenId = TokenId.Question;
                    }
                    break;

                case '[':
                    NextChar();
                    tokenId = TokenId.OpenBracket;
                    break;

                case ']':
                    NextChar();
                    tokenId = TokenId.CloseBracket;
                    break;

                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleBar;
                    }
                    else
                    {
                        tokenId = TokenId.Bar;
                    }
                    break;

                case '"':
                case '\'':
                    char quote = _ch;
                    do
                    {
                        bool escaped;

                        do
                        {
                            escaped = false;
                            NextChar();

                            if (_ch == '\\')
                            {
                                escaped = true;
                                if (_textPos < _textLen) NextChar();
                            }
                        }
                        while (_textPos < _textLen && (_ch != quote || escaped));

                        if (_textPos == _textLen)
                            throw ParseError(_textPos, Res.UnterminatedStringLiteral);

                        NextChar();
                    } while (_ch == quote);

                    tokenId = TokenId.StringLiteral;
                    break;

                default:
                    if (char.IsLetter(_ch) || _ch == '@' || _ch == '_' || _ch == '$' || _ch == '^' || _ch == '~')
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(_ch) || _ch == '_');
                        tokenId = TokenId.Identifier;
                        break;
                    }

                    if (char.IsDigit(_ch))
                    {
                        tokenId = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(_ch));

                        bool hexInteger = false;
                        if (_ch == 'X' || _ch == 'x')
                        {
                            NextChar();
                            ValidateHexChar();
                            do
                            {
                                NextChar();
                            } while (IsHexChar(_ch));

                            hexInteger = true;
                        }

                        if (_ch == 'U' || _ch == 'L')
                        {
                            NextChar();
                            if (_ch == 'L')
                            {
                                if (_text[_textPos - 1] == 'U') NextChar();
                                else throw ParseError(_textPos, Res.InvalidIntegerQualifier, _text.Substring(_textPos - 1, 2));
                            }
                            ValidateExpression();
                            break;
                        }

                        if (hexInteger)
                        {
                            break;
                        }

                        if (_ch == NumberDecimalSeparator)
                        {
                            tokenId = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'E' || _ch == 'e')
                        {
                            tokenId = TokenId.RealLiteral;
                            NextChar();
                            if (_ch == '+' || _ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'F' || _ch == 'f') NextChar();
                        if (_ch == 'D' || _ch == 'd') NextChar();
                        if (_ch == 'M' || _ch == 'm') NextChar();
                        break;
                    }

                    if (_textPos == _textLen)
                    {
                        tokenId = TokenId.End;
                        break;
                    }

                    throw ParseError(_textPos, Res.InvalidCharacter, _ch);
            }

            CurrentToken.Pos = tokenPos;
            CurrentToken.Text = _text.Substring(tokenPos, _textPos - tokenPos);
            CurrentToken.OriginalId = tokenId;
            CurrentToken.Id = GetAliasedTokenId(tokenId, CurrentToken.Text);
        }

        public void ValidateToken(TokenId t, string errorMessage)
        {
            if (CurrentToken.Id != t)
            {
                throw ParseError(errorMessage);
            }
        }

        public void ValidateToken(TokenId t)
        {
            if (CurrentToken.Id != t)
            {
                throw ParseError(Res.SyntaxError);
            }
        }

        private void ValidateExpression()
        {
            if (char.IsLetterOrDigit(_ch))
            {
                throw ParseError(_textPos, Res.ExpressionExpected);
            }
        }

        private void ValidateDigit()
        {
            if (!char.IsDigit(_ch))
            {
                throw ParseError(_textPos, Res.DigitExpected);
            }
        }

        private void ValidateHexChar()
        {
            if (!IsHexChar(_ch))
            {
                throw ParseError(_textPos, Res.HexCharExpected);
            }
        }

        private Exception ParseError(string format, params object[] args)
        {
            return ParseError(CurrentToken.Pos, format, args);
        }

        private static Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }

        private static TokenId GetAliasedTokenId(TokenId t, string alias)
        {
            TokenId id;
            return t == TokenId.Identifier && _predefinedAliases.TryGetValue(alias, out id) ? id : t;
        }

        private static bool IsHexChar(char c)
        {
            if (char.IsDigit(c))
            {
                return true;
            }

            if (c <= '\x007f')
            {
                c |= (char)0x20;
                return c >= 'a' && c <= 'f';
            }

            return false;
        }
    }
}
