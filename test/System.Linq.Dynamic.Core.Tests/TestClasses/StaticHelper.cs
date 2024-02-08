using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Tests
{
    public static class StaticHelper
    {
        public static Guid NewStaticGuid => new("43b17e59-2b66-4697-a3ab-7b45baedee72");

        public static class Nested
        {
            public static Guid NewNestedStaticProperty => new("954692b3-6a37-4c9c-ad55-07fbb593f046");

            public static Guid NewNestedStaticMethod() => new ("954692b3-6a37-4c9c-ad55-07fbb593f046");

            public static bool IsNull(object? value) => value != null;
        }

        public static Guid? GetGuid(string name)
        {
            return Guid.NewGuid();
        }

        public static string Filter(string filter)
        {
            return filter;
        }

        public static StaticHelperSqlExpression SubSelect(string columnName, string objectClassName, string? filter, string order)
        {
            Expression<Func<User, bool>>? expFilter = null;

            if (filter != null)
            {
                var config = new ParsingConfig
                {
                    CustomTypeProvider = new TestCustomTypeProvider()
                };

                expFilter = DynamicExpressionParser.ParseLambda<User, bool>(config, true, filter); // Failed Here!
            }

            return new StaticHelperSqlExpression
            {
                Filter = expFilter
            };
        }

        public static bool In(Guid? value, StaticHelperSqlExpression expression)
        {
            return value != Guid.Empty;
        }

        public static Guid First(StaticHelperSqlExpression staticHelperSqlExpression)
        {
            return Guid.NewGuid();
        }

        public static string ToExpressionString(Guid? value, int subQueryLevel)
        {
            if (value == null)
            {
                return "NULL";
            }

            var quote = GetQuote(subQueryLevel);
            return $"Guid.Parse({quote}{value}{quote})";
        }

        public static Guid Get(string settingName)
        {
            return Guid.NewGuid();
        }

        private static string GetQuote(int subQueryLevel)
        {
            var quoteCount = (int)Math.Pow(2, subQueryLevel - 1);
            return new string('"', quoteCount);
        }
    }
}