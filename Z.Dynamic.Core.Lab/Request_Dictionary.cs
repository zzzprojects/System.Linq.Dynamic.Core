using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    public class Request_Dictionary
    {
        public static void Execute()
        {
            object CreateDicParameter(string name) => new Dictionary<string, object>
                {{"Name", new Dictionary<string, object> {{"FirstName", name }}}};

            var config = new ParsingConfig()
            {
                CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider()
            };
            var parType = new Dictionary<string, object>().GetType();
            var lambda = DynamicExpressionParser.ParseLambda(config, new[] { Expression.Parameter(parType, "item") }, typeof(object), "item.Name.FirstName").Compile();

            var x1 = lambda.DynamicInvoke(CreateDicParameter("Julio"));
            var x2 = lambda.DynamicInvoke(CreateDicParameter("John"));
        }
    }
}
