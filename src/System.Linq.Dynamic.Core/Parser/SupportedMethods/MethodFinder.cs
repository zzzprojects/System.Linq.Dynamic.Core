﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser.SupportedMethods;

internal class MethodFinder
{
    private readonly ParsingConfig _parsingConfig;
    private readonly IExpressionHelper _expressionHelper;
    private readonly IDictionary<Type, MethodInfo[]> _cachedMethods;

    /// <summary>
    /// #794
    /// </summary>
    private interface IAggregateSignatures
    {
        void Average(decimal? selector);
        void Average(decimal selector);
        void Average(double? selector);
        void Average(double selector);
        void Average(float? selector);
        void Average(float selector);
        void Average(int? selector);
        void Average(int selector);
        void Average(long? selector);
        void Average(long selector);

        void Sum(decimal? selector);
        void Sum(decimal selector);
        void Sum(double? selector);
        void Sum(double selector);
        void Sum(float? selector);
        void Sum(float selector);
        void Sum(int? selector);
        void Sum(int selector);
        void Sum(long? selector);
        void Sum(long selector);
    }

    public MethodFinder(ParsingConfig parsingConfig, IExpressionHelper expressionHelper)
    {
        _parsingConfig = Check.NotNull(parsingConfig);
        _expressionHelper = Check.NotNull(expressionHelper);
        _cachedMethods = new Dictionary<Type, MethodInfo[]>
        {
            { typeof(Enumerable), typeof(Enumerable).GetMethods().Where(m => !m.IsGenericMethodDefinition).ToArray() },
            { typeof(Queryable), typeof(Queryable).GetMethods().Where(m => !m.IsGenericMethodDefinition).ToArray() }
        };
    }

    public bool TryFindAggregateMethod(Type callType, string methodName, Type parameterType, [NotNullWhen(true)] out MethodInfo? aggregateMethod)
    {
        var nonGenericMethodsByName = _cachedMethods[callType]
            .Where(m => m.Name == methodName)
            .ToArray();

        if (TypeHelper.TryGetAsEnumerable(parameterType, out var parameterTypeAsEnumerable))
        {
            aggregateMethod = nonGenericMethodsByName
                .SelectMany(m => m.GetParameters(), (m, p) => new { Method = m, Parameter = p })
                .Where(x => x.Parameter.ParameterType == parameterTypeAsEnumerable)
                .Select(x => x.Method)
                .FirstOrDefault();

            return aggregateMethod != null;
        }

        aggregateMethod = null;
        return false;
    }

    public bool CheckAggregateMethodAndTryUpdateArgsToMatchMethodArgs(string methodName, ref Expression[] args)
    {
        if (methodName is nameof(IAggregateSignatures.Average) or nameof(IAggregateSignatures.Sum))
        {
            ContainsMethod(typeof(IAggregateSignatures), methodName, false, null, ref args);
            return true;
        }

        return false;
    }

    public bool ContainsMethod(Type type, string methodName, bool staticAccess = true)
    {
        Check.NotNull(type);

#if !(UAP10_0 || NETSTANDARD)
        var flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
        return type.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName).Any();
#else
        return type.GetTypeInfo().DeclaredMethods.Any(m => (m.IsStatic || !staticAccess) && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
#endif
    }

    public bool ContainsMethod(Type type, string methodName, bool staticAccess, Expression? instance, ref Expression[] args)
    {
        Check.NotNull(type);

        // NOTE: `instance` is not passed by ref in the method signature by design. The ContainsMethod should not change the instance.
        // However, args by reference is required for backward compatibility (removing "ref" will break some tests)

        return FindMethod(type, methodName, staticAccess, ref instance, ref args, out _) == 1;
    }

    public int FindMethod(Type? type, string methodName, bool staticAccess, ref Expression? instance, ref Expression[] args, out MethodBase? method)
    {
#if !(UAP10_0 || NETSTANDARD)
        BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
        foreach (Type t in SelfAndBaseTypes(type))
        {
            MemberInfo[] members = t.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
            int count = FindBestMethodBasedOnArguments(members.Cast<MethodBase>(), ref args, out method);
            if (count != 0)
            {
                return count;
            }
        }
#else
        foreach (Type t in SelfAndBaseTypes(type))
        {
            var methods = t.GetTypeInfo().DeclaredMethods.Where(m => (m.IsStatic || !staticAccess) && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).ToArray();
            int count = FindBestMethodBasedOnArguments(methods, ref args, out method);
            if (count != 0)
            {
                return count;
            }
        }
#endif

        if (instance != null && _parsingConfig.CustomTypeProvider != null)
        {
            // Try to solve with registered extension methods from this type and all base types
            var methods = new List<MethodInfo>();
            foreach (var t in SelfAndBaseTypes(type))
            {
                if (_parsingConfig.CustomTypeProvider.GetExtensionMethods().TryGetValue(t, out var extensionMethodsOfType))
                {
                    methods.AddRange(extensionMethodsOfType.Where(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)));
                }
            }

            if (methods.Any())
            {
                var argsList = args.ToList();
                argsList.Insert(0, instance);

                var extensionMethodArgs = argsList.ToArray();

                // ReSharper disable once RedundantEnumerableCastCall
                int count = FindBestMethodBasedOnArguments(methods.Cast<MethodBase>(), ref extensionMethodArgs, out method);
                if (count != 0)
                {
                    instance = null;
                    args = extensionMethodArgs;
                    return count;
                }
            }
        }

        method = null;
        return 0;
    }

    public int FindBestMethodBasedOnArguments(IEnumerable<MethodBase> methods, ref Expression[] args, out MethodBase? method)
    {
        // Passing args by reference is now required with the params array support.
        var inlineArgs = args;

        MethodData[] applicable = methods
            .Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() })
            .Where(m => IsApplicable(m, inlineArgs))
            .ToArray();

        if (applicable.Length > 1)
        {
            applicable = applicable.Where(m => applicable.All(n => m == n || FirstIsBetterThanSecond(inlineArgs, m, n))).ToArray();
        }

        if (args.Length == 2 && applicable.Length > 1 && (args[0].Type == typeof(Guid?) || args[1].Type == typeof(Guid?)))
        {
            applicable = applicable.Take(1).ToArray();
        }

        if (applicable.Length == 1)
        {
            var methodData = applicable[0];
            if (methodData.MethodBase is MethodInfo methodInfo)
            {
                method = methodInfo.GetBaseDefinition();
            }
            else
            {
                method = methodData.MethodBase;
            }

            if (args.Length == 0 || args.Length != methodData.Args.Length)
            {
                args = methodData.Args;
            }
            else
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i].Type != methodData.Args[i].Type &&
                        args[i].Type.IsArray && methodData.Args[i].Type.IsArray &&
                        args[i].Type != typeof(string) && methodData.Args[i].Type == typeof(object[]))
                    {
                        args[i] = _expressionHelper.ConvertAnyArrayToObjectArray(args[i]);
                    }
                    else
                    {
                        args[i] = methodData.Args[i];
                    }
                }
            }
        }
        else
        {
            method = null;
        }

        return applicable.Length;
    }

    public int FindIndexer(Type type, Expression[] args, out MethodBase? method)
    {
        foreach (var t in SelfAndBaseTypes(type))
        {
            var members = t.GetDefaultMembers();
            if (members.Length != 0)
            {
                IEnumerable<MethodBase> methods = members.OfType<PropertyInfo>().
#if !(UAP10_0 || NETSTANDARD)
                    Select(p => (MethodBase?)p.GetGetMethod()).
                    OfType<MethodBase>();
#else
                    Select(p => (MethodBase)p.GetMethod);
#endif
                var count = FindBestMethodBasedOnArguments(methods, ref args, out method);
                if (count != 0)
                {
                    return count;
                }
            }
        }

        method = null;
        return 0;
    }

    private bool IsApplicable(MethodData method, Expression[] args)
    {
        bool isParamArray = method.Parameters.Length > 0 && method.Parameters.Last().IsDefined(typeof(ParamArrayAttribute), false);

        // if !paramArray, the number of parameter must be equal
        // if paramArray, the last parameter is optional
        if ((!isParamArray && method.Parameters.Length != args.Length) ||
            (isParamArray && method.Parameters.Length - 1 > args.Length))
        {
            return false;
        }

        Expression[] promotedArgs = new Expression[method.Parameters.Length];
        for (int i = 0; i < method.Parameters.Length; i++)
        {
            if (isParamArray && i == method.Parameters.Length - 1)
            {
                if (method.Parameters.Length == args.Length + 1
                    || (method.Parameters.Length == args.Length && args[i] is ConstantExpression constantExpression && constantExpression.Value == null))
                {
                    promotedArgs[promotedArgs.Length - 1] = Expression.Constant(null, method.Parameters.Last().ParameterType);
                }
                else if (method.Parameters.Length == args.Length && method.Parameters.Last().ParameterType == args.Last().Type)
                {
                    promotedArgs[promotedArgs.Length - 1] = args.Last();
                }
                else
                {
                    var paramType = method.Parameters.Last().ParameterType;
                    var paramElementType = paramType.GetElementType()!;

                    var arrayInitializerExpressions = new List<Expression>();

                    for (int j = method.Parameters.Length - 1; j < args.Length; j++)
                    {
                        var promotedExpression = _parsingConfig.ExpressionPromoter.Promote(args[j], paramElementType, false, true);
                        if (promotedExpression == null)
                        {
                            return false;
                        }

                        arrayInitializerExpressions.Add(promotedExpression);
                    }

                    var paramExpression = Expression.NewArrayInit(paramElementType, arrayInitializerExpressions);

                    promotedArgs[promotedArgs.Length - 1] = paramExpression;
                }
            }
            else
            {
                var methodParameter = method.Parameters[i];
                if (methodParameter.IsOut && args[i] is ParameterExpression parameterExpression)
                {
#if NET35
                    return false;
#else
                    if (!parameterExpression.IsByRef)
                    {
                        return false;
                    }

                    promotedArgs[i] = Expression.Parameter(methodParameter.ParameterType, methodParameter.Name);
#endif
                }
                else
                {
                    var promotedExpression = _parsingConfig.ExpressionPromoter.Promote(args[i], methodParameter.ParameterType, false, true);
                    if (promotedExpression == null)
                    {
                        return false;
                    }

                    promotedArgs[i] = promotedExpression;
                }
            }
        }

        method.Args = promotedArgs;
        return true;
    }

    private static bool FirstIsBetterThanSecond(Expression[] args, MethodData first, MethodData second)
    {
        // If args count is 0 -> parameterless method is better than a method with parameters
        if (args.Length == 0)
        {
            return first.Parameters.Length == 0 && second.Parameters.Length != 0;
        }

        var better = false;
        for (var i = 0; i < args.Length; i++)
        {
            var result = CompareConversions(args[i].Type, first.Parameters[i].ParameterType, second.Parameters[i].ParameterType);

            switch (result)
            {
                // If second is better, return false
                case CompareConversionType.Second:
                    return false;

                // If first is better, return true
                case CompareConversionType.First:
                    return true;

                // If both are same, just set better to true and continue
                case CompareConversionType.Both:
                    better = true;
                    break;
            }
        }

        return better;
    }

    // Return "First" if s -> t1 is a better conversion than s -> t2
    // Return "Second" if s -> t2 is a better conversion than s -> t1
    // Return "Both" if neither conversion is better
    private static CompareConversionType CompareConversions(Type source, Type first, Type second)
    {
        if (first == second)
        {
            return CompareConversionType.Both;
        }
        if (source == first)
        {
            return CompareConversionType.First;
        }
        if (source == second)
        {
            return CompareConversionType.Second;
        }

        var firstIsCompatibleWithSecond = TypeHelper.IsCompatibleWith(first, second);
        var secondIsCompatibleWithFirst = TypeHelper.IsCompatibleWith(second, first);

        if (firstIsCompatibleWithSecond && !secondIsCompatibleWithFirst)
        {
            return CompareConversionType.First;
        }
        if (secondIsCompatibleWithFirst && !firstIsCompatibleWithSecond)
        {
            return CompareConversionType.Second;
        }

        if (TypeHelper.IsSignedIntegralType(first) && TypeHelper.IsUnsignedIntegralType(second))
        {
            return CompareConversionType.First;
        }
        if (TypeHelper.IsSignedIntegralType(second) && TypeHelper.IsUnsignedIntegralType(first))
        {
            return CompareConversionType.Second;
        }

        return CompareConversionType.Both;
    }

    private static IEnumerable<Type> SelfAndBaseTypes(Type? type)
    {
        if (type?.GetTypeInfo().IsInterface == true)
        {
            var types = new List<Type>();
            AddInterfaces(types, type);
            return types;
        }

        return SelfAndBaseClasses(type);
    }

    private static IEnumerable<Type> SelfAndBaseClasses(Type? type)
    {
        while (type != null)
        {
            yield return type;
            type = type.GetTypeInfo().BaseType;
        }
    }

    private static void AddInterfaces(ICollection<Type> types, Type type)
    {
        if (!types.Contains(type))
        {
            types.Add(type);
            foreach (var interfaceType in type.GetInterfaces())
            {
                AddInterfaces(types, interfaceType);
            }
        }
    }
}