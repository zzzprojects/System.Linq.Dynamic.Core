using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DynamicExpressionParserTests
    {
        [Fact]
        public void Parse_ParameterExpressionMethodCall_ReturnsIntExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(true,
                new[] { Expression.Parameter(typeof(int), "x") },
                typeof(int),
                "x + 1");
            Assert.Equal(typeof(int), expression.Body.Type);
        }

        [Fact]
        public void Parse_TupleToStringMethodCall_ReturnsStringLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(
                typeof(Tuple<int>),
                typeof(string),
                "it.ToString()");
            Assert.Equal(typeof(string), expression.ReturnType);
        }

        [Fact]
        public void Parse_StringLiteral_ReturnsBooleanLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(Boolean), "Property1 == \"test\"");
            Assert.Equal(typeof(Boolean), expression.Body.Type);
        }

        [Fact]
        public void Parse_StringLiteralEmpty_ReturnsBooleanLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(Boolean), "Property1 == \"\"");
            Assert.Equal(typeof(Boolean), expression.Body.Type);
        }

        [Fact]
        public void Parse_StringLiteralEmbeddedQuote_ReturnsBooleanLambdaExpression()
        {
            string expectedRightValue = "\"test \\\"string\"";
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        [Fact]
        public void Parse_StringLiteralStartEmbeddedQuote_ReturnsBooleanLambdaExpression()
        {
            string expectedRightValue = "\"\\\"test\"";
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        [Fact]
        public void Parse_StringLiteral_MissingClosingQuote()
        {
            string expectedRightValue = "\"test\\\"";

            Assert.Throws<ParseException>(() => DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("Property1 == {0}", expectedRightValue)));
        }

        [Fact]
        public void Parse_StringLiteralEscapedBackslash_ReturnsBooleanLambdaExpression()
        {
            string expectedRightValue = "\"test\\string\"";
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        //[Fact]
        //public void ParseLambda_DelegateTypeMethodCall_ReturnsEventHandlerLambdaExpression()
        //{
        //    var expression = DynamicExpressionParser.ParseLambda(true,
        //        typeof(EventHandler),
        //        null,
        //        new[] { Expression.Parameter(typeof(object), "sender"), Expression.Parameter(typeof(EventArgs), "e") },
        //        "sender.ToString()");

        //    Assert.Equal(typeof(void), expression.ReturnType);
        //    Assert.Equal(typeof(EventHandler), expression.Type);
        //}

        //[Fact] this should fail : not allowed
        //public void ParseLambda_VoidMethodCall_ReturnsActionDelegate()
        //{
        //    var expression = DynamicExpressionParser.ParseLambda(
        //        typeof(IO.FileStream),
        //        null,
        //        "it.Close()");
        //    Assert.Equal(typeof(void), expression.ReturnType);
        //    Assert.Equal(typeof(Action<IO.FileStream>), expression.Type);
        //}
    }
}
