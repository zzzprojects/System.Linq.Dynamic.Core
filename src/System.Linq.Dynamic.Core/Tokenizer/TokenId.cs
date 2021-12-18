#pragma warning disable CS1591
namespace System.Linq.Dynamic.Core.Tokenizer
{
    /// <summary>
    /// TokenId which defines the text which is parsed.
    /// </summary>
    public enum TokenId
    {
        Unknown,
        End,
        Identifier,
        StringLiteral,
        IntegerLiteral,
        RealLiteral,
        Exclamation,
        Percent,
        Ampersand,
        OpenParen,
        CloseParen,
        OpenCurlyParen,
        CloseCurlyParen,
        Asterisk,
        Plus,
        Comma,
        Minus,
        Dot,
        Slash,
        Colon,
        LessThan,
        Equal,
        GreaterThan,
        Question,
        OpenBracket,
        CloseBracket,
        Bar,
        ExclamationEqual,
        DoubleAmpersand,
        LessThanEqual,
        LessGreater,
        DoubleEqual,
        GreaterThanEqual,
        DoubleBar,
        DoubleGreaterThan,
        DoubleLessThan,
        NullCoalescing,
        Lambda,
        NullPropagation
    }
}
