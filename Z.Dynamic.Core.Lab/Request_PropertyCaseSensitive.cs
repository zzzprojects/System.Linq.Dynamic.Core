﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Z.Dynamic.Core.Lab
{
    class Request_PropertyCaseSensitive
    {
        class DataClass
        {
            public bool IsTrue;
            public bool ISTrue;
            public string Name;
        }

        public static void Execute()
        {

            var isTrue = true;
            var abc = new List<DataClass>() { new DataClass() { IsTrue = true, Name = "1" } };
            var x = new List<ParameterExpression>();
            x.Add(Expression.Parameter(abc.GetType(), "abc"));

            var config = new ParsingConfig();
            var query = "abc.Where(IsTrue == true)";
            config.IsCaseSensitive = true;
            var e = DynamicExpressionParser.ParseLambda(config, x.ToArray(), null, query);
            var del = e.Compile();
            var result = ((IEnumerable)del.DynamicInvoke(abc)).GetEnumerator();
            result.MoveNext();
            var t = result.Current;
        }
    }
}
