using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Tests
{
    public static class StaticHelper
    {
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

            //var quote = string.Concat(Enumerable.Repeat("\"", quoteCount));
            //return quote;
            return new string('"', quoteCount);
        }
    }
}