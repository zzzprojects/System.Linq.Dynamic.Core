using System.Collections.Generic;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Util;

internal static class QueryableMethodFinder
{
    public static MethodInfo GetGenericMethod(string name)
    {
        return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi => mi.IsGenericMethod);
    }

    public static MethodInfo GetMethod(string name, Type argumentType, Type returnType, int parameterCount = 0, Func<MethodInfo, bool>? predicate = null) =>
        GetMethod(name, returnType, parameterCount, mi => mi.ToString().Contains(argumentType.ToString()) && ((predicate == null) || predicate(mi)));

    public static MethodInfo GetMethod(string name, Type returnType, int parameterCount = 0, Func<MethodInfo, bool>? predicate = null)
    {
        var returnTypes = new List<Type> { returnType };
        if (!TypeHelper.IsNullableType(returnType))
        {
            returnTypes.Add(TypeHelper.GetNullableType(returnType));
        }

        return GetMethod(name, parameterCount, mi => returnTypes.Contains(mi.ReturnType) && (predicate == null || predicate(mi)));
    }

    public static MethodInfo GetMethodWithExpressionParameter(string name) =>
        GetMethod(name, 1, mi =>
            mi.GetParameters().Length == 2 &&
            mi.GetParameters()[1].ParameterType.GetTypeInfo().IsGenericType &&
            mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
            mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetTypeInfo().IsGenericType &&
            mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>)
        );

    public static MethodInfo GetMethodWithIntParameter(string name) =>
        GetMethod(name, 1, mi =>
            mi.GetParameters().Length == 2 &&
            mi.GetParameters()[1].ParameterType == typeof(int)
        );

    public static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool>? predicate = null)
    {
        try
        {
            return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).First(mi =>
                mi.GetParameters().Length == parameterCount + 1 && (predicate == null || predicate(mi)));
        }
        catch (Exception ex)
        {
            throw new Exception("Specific method not found: " + name, ex);
        }
    }
}
