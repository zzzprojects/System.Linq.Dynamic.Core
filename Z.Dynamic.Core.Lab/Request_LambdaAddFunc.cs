using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    class Request_LambdaAddFunc
    {
        public static void Execute()
        {
            var expression = (Action<int>)DynamicExpressionParser.ParseLambda(typeof(Action<int>), new[] { Expression.Parameter(typeof(int), "x") }, typeof(int),  "x + 1").Compile();
            var expression2 = (Func<int, int>)DynamicExpressionParser.ParseLambda(typeof(Func<int, int>), new[] { Expression.Parameter(typeof(int), "x") }, typeof(int), "x + 1").Compile();

            var r1 = expression2(3);
        }
    }
}
