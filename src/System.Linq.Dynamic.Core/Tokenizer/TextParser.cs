using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;

namespace System.Linq.Dynamic.Core.Tokenizer
{
    public class TextParser
    {
        private static char NumberDecimalSeparator = '.';


        // These aliases are supposed to simply the where clause and make it more human readable
        // As an addition it is compatible with the OData.Filter specification
        //
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
            while (char.IsWhiteSpace(_ch)) NextChar();
            TokenId t;
            int tokenPos = _textPos;
            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else if (_ch == '<')
                    {
                        NextChar();
                        t = TokenId.DoubleLessThan;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.DoubleGreaterThan;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    if (_ch == '?')
                    {
                        NextChar();
                        t = TokenId.NullCoalescing;
                    }
                    else
                    {
                        t = TokenId.Question;
                    }
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    char quote = _ch;
                    do
                    {
                        NextChar();
                        while (_textPos < _textLen && _ch != quote) NextChar();
                        if (_textPos == _textLen)
                            throw ParseError(_textPos, Res.UnterminatedStringLiteral);
                        NextChar();
                    } while (_ch == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (char.IsLetter(_ch) || _ch == '@' || _ch == '_' || _ch == '$' || _ch == '^' || _ch == '~')
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(_ch) || _ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }

                    if (char.IsDigit(_ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(_ch));

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

                        if (_ch == NumberDecimalSeparator)
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'E' || _ch == 'e')
                        {
                            t = TokenId.RealLiteral;
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
                        break;
                    }

                    if (_textPos == _textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(_textPos, Res.InvalidCharacter, _ch);
            }

            CurrentToken.Pos = tokenPos;
            CurrentToken.Text = _text.Substring(tokenPos, _textPos - tokenPos);
            CurrentToken.Id = GetAliasedTokenId(t, CurrentToken.Text);
        }


        public void ValidateToken(TokenId t, string errorMessage)
        {
            if (CurrentToken.Id != t) throw ParseError(errorMessage);
        }

        public void ValidateToken(TokenId t)
        {
            if (CurrentToken.Id != t) throw ParseError(Res.SyntaxError);
        }

        private void ValidateExpression()
        {
            if (char.IsLetterOrDigit(_ch)) throw ParseError(_textPos, Res.ExpressionExpected);
        }

        private void ValidateDigit()
        {
            if (!char.IsDigit(_ch)) throw ParseError(_textPos, Res.DigitExpected);
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
    }
}