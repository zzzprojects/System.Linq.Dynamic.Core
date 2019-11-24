using NFluent;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tokenizer;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Tokenizer
{
    public class TextParserTests
    {
        private readonly ParsingConfig _config = new ParsingConfig();

        [Fact]
        public void TextParser_Parse_Bar()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " | ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.Bar);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("|");
        }

        [Fact]
        public void TextParser_Parse_Colon()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " : ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.Colon);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals(":");
        }

        [Fact]
        public void TextParser_Parse_Exclamation()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " !x");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.Exclamation);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("!");
        }

        [Fact]
        public void TextParser_Parse_ExclamationEqual()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " != 1");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.ExclamationEqual);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("!=");
        }

        [Fact]
        public void TextParser_Parse_GreaterThanEqual()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " >= ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.GreaterThanEqual);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals(">=");
        }

        [Fact]
        public void TextParser_Parse_HexadecimalIntegerLiteral()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 0x1234567890AbCdEfL ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.IntegerLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("0x1234567890AbCdEfL");

            Check.ThatCode(() => new TextParser(_config, "0xz1234")).Throws<ParseException>();
        }

        [Fact]
        public void TextParser_Parse_LessGreater()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " <> ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.LessGreater);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("<>");
        }

        [Fact]
        public void TextParser_Parse_NullPropagation()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " ?. ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.NullPropagation);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("?.");
        }

        [Fact]
        public void TextParser_Parse_RealLiteral()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 1.0E25 ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("1.0E25");

            Check.ThatCode(() => new TextParser(_config, "1.e25")).Throws<ParseException>();
        }

        [Fact]
        public void TextParser_Parse_RealLiteralDecimalQualifier()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 12.5m ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("12.5m");
        }

        [Fact]
        public void TextParser_Parse_RealLiteralDecimalQualifier_Other_NumberDecimalSeparator()
        {
            // Assign
            var config = new ParsingConfig
            {
                NumberParseCulture = CultureInfo.CreateSpecificCulture("de-DE")
            };

            // Act
            var textParser = new TextParser(config, " 12,5m ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("12,5m");
        }

        [Fact]
        public void TextParser_Parse_RealLiteralFloatQualifier()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 12.5f ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("12.5f");
        }

        [Fact]
        public void TextParser_Parse_RealLiteralMinus()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 1.0E-25 ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("1.0E-25");
        }

        [Fact]
        public void TextParser_Parse_RealLiteralPlus()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " 1.0E+25 ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.RealLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("1.0E+25");
        }

        [Fact]
        public void TextParser_Parse_Percent()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " % ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.Percent);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("%");
        }

        [Fact]
        public void TextParser_Parse_Slash()
        {
            // Assign + Act
            var textParser = new TextParser(_config, " / ");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.Slash);
            Check.That(textParser.CurrentToken.Pos).Equals(1);
            Check.That(textParser.CurrentToken.Text).Equals("/");
        }

        [Fact]
        public void TextParser_Parse_StringLiteral_WithSingleQuotes_Backslash()
        {
            // Assign + Act
            var textParser = new TextParser(_config, "'\\'");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.StringLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(0);
            Check.That(textParser.CurrentToken.Text[1]).Equals('\\');
        }

        [Fact]
        public void TextParser_Parse_StringLiteral_WithSingleQuotes_DoubleQuote()
        {
            // Assign + Act
            var textParser = new TextParser(_config, "'\"'");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.StringLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(0);
            Check.That(textParser.CurrentToken.Text[1]).Equals('"');
        }

        [Fact]
        public void TextParser_Parse_StringLiteral_WithSingleQuotes_SingleQuote()
        {
            // Assign + Act
            var textParser = new TextParser(_config, "'\''");

            // Assert
            Check.That(textParser.CurrentToken.Id).Equals(TokenId.StringLiteral);
            Check.That(textParser.CurrentToken.Pos).Equals(0);
            Check.That(textParser.CurrentToken.Text[1]).Equals('\'');
        }

        [Fact]
        public void TextParser_Parse_ThrowsException()
        {
            // Assign + Act + Assert
            //Check.ThatCode(() => { new TextParser("ಬಾ"); }).Throws<ParseException>();
            Check.ThatCode(() => { new TextParser(_config, ";"); }).Throws<ParseException>();
        }
    }
}
