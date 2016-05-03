using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    internal class MethodInfoHelper
    {
        public static MethodInfo GetMethodInfoOf<T>(Expression<Func<T>> expression)
        {
            var body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
    }
}