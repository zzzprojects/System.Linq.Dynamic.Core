using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    public class Request_Promote
    {
        public static void Execute()
        {
            var strArray = new[] { "1", "2", "3", "4" };
            var x = new List<ParameterExpression>();
            x.Add(Expression.Parameter(strArray.GetType(), "strArray"));

            var config = new ParsingConfig();
            string query = "string.Join(\",\" , strArray)";

            var e = DynamicExpressionParser.ParseLambda(config, x.ToArray(), null, query);
            Delegate del = e.Compile();
            var result = del.DynamicInvoke(strArray);

            //var intArray = new[] { 1, 2, 3, 4 };
            //var x = new List<ParameterExpression>();
            //x.Add(Expression.Parameter(intArray.GetType(), "intArray"));

            //var config = new ParsingConfig();
            //string query = "string.Join(\",\" , intArray)";

            //var e = DynamicExpressionParser.ParseLambda(config, x.ToArray(), null, query);
            //Delegate del = e.Compile();
            //var result = del.DynamicInvoke(intArray);
        }
    }
}
