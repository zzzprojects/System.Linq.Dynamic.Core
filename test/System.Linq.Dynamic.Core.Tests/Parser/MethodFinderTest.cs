﻿using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class MethodFinderTest
{
    [Fact(Skip = "867")]
    public void MethodsOfDynamicLinqAndSystemLinqShouldBeEqual()
    {
        Expression<Func<int?, string?>> expr = x => x.ToString();
            
        var selector = "ToString()";
        var prm = Parameter(typeof(int?));
        var parser = new ExpressionParser([prm], selector, [], ParsingConfig.Default);
        var expr1 = parser.Parse(null);
            
        Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expr1).Method.DeclaringType);
    }
}