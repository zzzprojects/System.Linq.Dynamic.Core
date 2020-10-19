using System;
using System.Collections.Generic;
using System.Linq; 
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NFluent;

namespace System.Linq.Dynamic.Core.Tests.MikArea
{
    public class ParseLambda
    {
        [Fact]
        public void Test_ParseLambda_1()
        {

            var expression = (Action<int>)DynamicExpressionParser.ParseLambda(typeof(Action<int>), new[] { Expression.Parameter(typeof(int), "x") }, typeof(int), "x + 1").Compile();
            var expression2 = (Func<int, int>)DynamicExpressionParser.ParseLambda(typeof(Func<int, int>), new[] { Expression.Parameter(typeof(int), "x") }, typeof(int), "x + 1").Compile();

            expression(3);
            var t = expression2(4);
            Check.That(t).IsEqualTo(5);
        }
    }
}
