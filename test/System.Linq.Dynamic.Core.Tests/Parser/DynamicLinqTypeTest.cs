using System.Linq.Dynamic.Core.CustomTypeProviders;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class DynamicLinqTypeTest
    {
        [DynamicLinqType]
        public static class Utils
        {
            public static string[] ConvertToArray(params string[] values)
            {
                if (values == null)
                {
                    return null;
                }

                return values.ToArray();
            }

            public static string[] ConvertToArray(int a, params string[] values)
            {
                if (values == null)
                {
                    return null;
                }

                return values.ToArray();
            }
        }


        [Fact]
        public void ParamArray_EmptyValue()
        {
            var query = "Utils.ConvertToArray()";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[])del.DynamicInvoke();

            Check.That(result).IsNull();
        }

        [Fact]
        public void ParamArray_NullValue()
        {
            var query = "Utils.ConvertToArray(null)";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[]) del.DynamicInvoke();

            Check.That(result).IsNull();
        }

        [Fact]
        public void ParamArray_WithManyValue()
        {
            var query = "Utils.ConvertToArray(\"a\", \"b\")";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[]) del.DynamicInvoke();

            Check.That(result.Length).Equals(2);
            Check.That(result[0]).Equals("a");
            Check.That(result[1]).Equals("b");
        }

        [Fact]
        public void ParamArray_WithSingleValue()
        {
            var query = "Utils.ConvertToArray(\"a\")";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[]) del.DynamicInvoke();

            Check.That(result.Length).Equals(1);
            Check.That(result[0]).Equals("a");
        }

        [Fact]
        public void ParamArray_NullValue2()
        {
            var query = "Utils.ConvertToArray(0, null)";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[])del.DynamicInvoke();

            Check.That(result).IsNull();
        }

        [Fact]
        public void ParamArray_WithManyValue2()
        {
            var query = "Utils.ConvertToArray(0, \"a\", \"b\")";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[])del.DynamicInvoke();

            Check.That(result.Length).Equals(2);
            Check.That(result[0]).Equals("a");
            Check.That(result[1]).Equals("b");
        }

        [Fact]
        public void ParamArray_WithSingleValue2()
        {
            var query = "Utils.ConvertToArray(0, \"a\")";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[])del.DynamicInvoke();

            Check.That(result.Length).Equals(1);
            Check.That(result[0]).Equals("a");
        }
    }
}
