using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using static System.Console;
using static System.Linq.Expressions.Expression;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class MethodFinderTest
    {
        [Fact]
        public void MethodsOfDynamicLinqAndSystemLinqShouldBeEqual()
        {
            Expression<Func<int?, string?>> expr = x => x.ToString();
            
            var selector = "ToString()";
            var prm = Parameter(typeof(int?));
            var parser = new ExpressionParser(new[] { prm }, selector, new object[] { }, ParsingConfig.Default);
            var expr1 = parser.Parse(null);
            
            Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expr1).Method.DeclaringType);
        }
    }
}
