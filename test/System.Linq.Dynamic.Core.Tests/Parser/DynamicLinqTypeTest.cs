using System.Linq.Dynamic.Core.CustomTypeProviders;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
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

        public static int IncrementMe(this int value)
        {
            return value + 1;
        }

        public static int IncrementMe(this int value, int y)
        {
            return value + y;
        }

        public static int IncrementMeAlso(this int value)
        {
            return value + 1;
        }
    }

    public class DynamicLinqTypeTest
    {
        public class EntityValue
        {
            public string[] Values { get; set; }
            public int ValueInt { get; set; }
        }

        [Flags]
        public enum Status
        {
            Inactive = 0,

            Active = 1,

            Other = 2,

            ActiveOther = 3
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
            var result = (string[])del.DynamicInvoke();

            Check.That(result).IsNull();
        }

        [Fact]
        public void ParamArray_WithManyValue()
        {
            var query = "Utils.ConvertToArray(\"a\", \"b\")";
            var expression = DynamicExpressionParser.ParseLambda(null, query, null);
            var del = expression.Compile();
            var result = (string[])del.DynamicInvoke();

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
            var result = (string[])del.DynamicInvoke();

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

        [Fact]
        public void ParamArray_Array()
        {
            var list = new[] { new EntityValue { }, new EntityValue { Values = new[] { "a", "b" } } }.AsQueryable();
            var result = list.Select("Utils.ConvertToArray(Values)").ToDynamicList<string[]>();

            Check.That(result.Count).Equals(2);
            Check.That(result[0]).IsNull();
            Check.That(result[1][0]).Equals("a");
            Check.That(result[1][1]).Equals("b");
        }

        [Fact]
        public void ExtensionMethod_OnString_NoParameter()
        {
            var list = new string?[] { "a", "", null }.AsQueryable();
            var result = list.Select("it.EmptyIfNull()").ToDynamicList<string?>();

            result.Should().Equal("a", "", "");
        }

        [Fact]
        public void ExtensionMethod_OnInt_NoParameter()
        {
            var list = new[] { new EntityValue { ValueInt = 1 }, new EntityValue { ValueInt = 2 } }.AsQueryable();
            var result = list.Select("ValueInt.IncrementMe()").ToDynamicList<int>();

            Check.That(result.Count).Equals(2);
            Check.That(result[0]).Equals(2);
            Check.That(result[1]).Equals(3);
        }

        [Fact]
        public void ExtensionMethod_OnInt_SingleParameter()
        {
            var list = new[] { new EntityValue { ValueInt = 1 }, new EntityValue { ValueInt = 2 } }.AsQueryable();
            var result = list.Select("ValueInt.IncrementMe(5)").ToDynamicList<int>();

            Check.That(result.Count).Equals(2);
            Check.That(result[0]).Equals(6);
            Check.That(result[1]).Equals(7);
        }

        [Fact]
        public void CallMethodOnEnum()
        {
            // Arrange
            var queryable = new[] { Status.Active, Status.ActiveOther, Status.Inactive }.AsQueryable();

            // Act
            var status = Status.Active;
            var result = queryable.Where("@0.HasFlag(it)", status).ToDynamicArray<Status>();

            // Assert
            result.Should().HaveCount(2);
        }
    }
}