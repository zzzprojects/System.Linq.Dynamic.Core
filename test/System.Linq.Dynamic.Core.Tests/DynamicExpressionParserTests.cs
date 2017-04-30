using NFluent;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using Xunit;
using User = System.Linq.Dynamic.Core.Tests.Helpers.Models.User;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DynamicExpressionParserTests
    {
        private class MyClass
        {
            public int Foo()
            {
                return 42;
            }
        }

        private class ComplexParseLambda1Result
        {
            public int? Age;
            public int TotalIncome;
        }

        [Fact]
        public void Parse_Lambda1()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                { "Users", qry }
            };

            // Act
            string query = "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke() as IEnumerable<dynamic>;

            var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) }).Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>().ToArray();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Length);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        [Fact]
        public void Parse_Lambda2()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string query = "Users.Select(j => new User(j.Income As Income))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Parse_Lambda3()
        {
            // Arrange
            var testList = User.GenerateSampleModels(5);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string query = "Users.Select(j => j)";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);
        }

        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/58
        [Fact]
        public void Parse_Lambda4_Issue58()
        {
            var expressionParams = new ParameterExpression[]
            {
                Expression.Parameter(typeof (MyClass), "myObj")
            };

            var myClassInstance = new MyClass();
            var invokersMerge = new List<object>() { myClassInstance };

            LambdaExpression expression = DynamicExpressionParser.ParseLambda(false, expressionParams, null, "myObj.Foo()");
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke(invokersMerge.ToArray());

            Check.That(result).Equals(42);
        }

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
