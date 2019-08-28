using ETG.SABENTISpro.Utils.HelpersUtils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TypeLite.Extensions;

namespace System.Linq.Dynamic.Core.Parser.SupportedMethods
{
    /// <summary>
    /// Differently from MethodFinder, MethodReducer
    /// iteratively finds available method signatures
    /// while arguments are being parsed.
    /// </summary>
    internal class MethodReducer
    {
        private readonly ParsingConfig _parsingConfig;

        private readonly Type _ownerType;

        private readonly string _methodName;

        private List<MethodData> _applicableMethods;

        private Expression _instance;

        /// <summary>
        /// Get an instance of MethodReducer
        /// </summary>
        /// <param name="parsingConfig"></param>
        /// <param name="ownerType"></param>
        /// <param name="methodName"></param>
        /// <param name="instance">Instance, used to feed extension methods</param>
        public MethodReducer(ParsingConfig parsingConfig, Type ownerType, string methodName, Expression instance)
        {
            _parsingConfig = parsingConfig;
            _ownerType = ownerType;
            _methodName = methodName;
            _instance = instance;

            this._applicableMethods = this.LoadMembers(_ownerType, true, methodName, instance);
        }

        public List<MethodData> GetCurrentApplicableMethods()
        {
            return _applicableMethods;
        }

        /// <summary>
        /// Add a new argument and reduce the set of available methods
        /// </summary>
        /// <param name="arg"></param>
        public void Reduce(Expression arg)
        {
            // Reduce the set of matching methods
            _applicableMethods = this.ReduceMethodsWithArgument(_applicableMethods, arg);
        }

        /// <summary>
        /// Remove any potentially matching methods that do not accept any
        /// additional arguments
        /// </summary>
        /// <returns></returns>
        public void PrepareReduce()
        {
            // Reduce the set of matching methods
            _applicableMethods = this.PrepareReduceMethods(_applicableMethods);
        }

        /// <summary>
        /// Get a hint on what the next argument Type might look like..
        /// </summary>
        /// <returns></returns>
        public Type HintNextLambdaArgumentTypes()
        {
            // Only support single dimensional lambdas (p) => p.x
            List<Type> lambdaTypes = new List<Type>();
            lambdaTypes.Add(typeof(Func<,>));

            var nextLambdaTypes = (from p in _applicableMethods
                select p.Parameters[p.Args.Count])
                .Where((i) => i.ParameterType.IsGenericType &&
                              lambdaTypes.Contains(i.ParameterType.GetGenericTypeDefinition()))
                .Select((i) => i.ParameterType.GenericTypeArguments.First())
                .ToList();

            // TODO: Hint is only valid if they are all the same!
            return nextLambdaTypes.First();
        }

        /// <summary>
        /// Find all available methods without considering method signature.
        /// </summary>
        protected List<MethodData> LoadMembers(Type type, bool staticAccess, string methodName, Expression instance)
        {
            MemberInfo[] members = new MemberInfo[0];

#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in type.SelfAndBaseTypes())
            {
                members = t.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
            }
#else
            foreach (Type t in type.SelfAndBaseTypes())
            {
                members = t.GetTypeInfo().DeclaredMethods.Where(x => (x.IsStatic || !staticAccess) && x.Name.ToLowerInvariant() == methodName.ToLowerInvariant()).ToArray();
            }
#endif
            // TODO: This is very slow/heavy, try to cache or find different approach
            // plus this returns too much stuff, we need to scope this only to IEnumerable, IQueryable, etc..
            // which is what makes sense in Dynamic.Linq.Core, or delegate this to the TypeResolver
            var extensionMethods = type.GetExtensionMethods(methodName);
            extensionMethods = this.PrepareReduceMethods(extensionMethods);
            extensionMethods = this.ReduceMethodsWithArgument(extensionMethods, instance);

            var result = new List<MethodData>();

            result.AddRange(members.Select((i) => new MethodData((MethodBase)i)));
            result.AddRange(extensionMethods);

            return result;
        }

        /// <summary>
        /// Try to promote and absorb an argument in the argument chain. Return false if it fails.
        /// </summary>
        /// <param name="md"></param>
        /// <param name="argument"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        protected bool MergeArgument(MethodData md, Expression argument)
        {
            if (md.Args.Count >= md.Parameters.Length)
            {
                throw new Exception("Method dos not admit any more arguments. Use PrepareReduce before calling Reduce.");
            }

            var nextMethodParameter = md.MethodBase.GetParameters()[md.Args.Count];

            var promotedArgument = this._parsingConfig.ExpressionPromoter.Promote(
                argument,
                nextMethodParameter.ParameterType,
                false,
                true,
                out var promotedTarget);

            if (promotedArgument != null)
            {
                md.Args.Add(promotedArgument);

                if (!md.MethodGenericsResolved)
                {
                    foreach (var methodGa in md.MethodBase.GetGenericArguments())
                    {
                        var paramGenericArguments = nextMethodParameter.ParameterType.GetGenericArguments();

                        for (int x = 0; x < paramGenericArguments.Length; x++)
                        {
                            if (paramGenericArguments[x].Name == methodGa.Name)
                            {
                                md.GenericArguments[methodGa.Name] = promotedTarget.GetGenericArguments()[x];
                            }
                        }
                    }

                    this.CheckMethodResolved(md);
                }

                return true;
            }

            md.PerfectMatch = true;

            for (int x = 0; x < md.Args.Count; x++)
            {
                if (md.Args[x].Type != md.Parameters[x].ParameterType)
                {
                    md.PerfectMatch = false;
                    break;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        protected List<MethodData> PrepareReduceMethods(List<MethodData> methods)
        {
            return methods.Where((i) => i.Parameters.Length > i.Args.Count).ToList();
        }

        /// <summary>
        /// Reduce this methods...
        /// </summary>
        /// <returns></returns>
        protected List<MethodData> ReduceMethodsWithArgument(List<MethodData> methods, Expression arg)
        {
            List<MethodData> results = new List<MethodData>();

            foreach (var m in methods)
            {
                // TODO: Remove this debugging hints
                var fullName = ((MethodInfo)m.MethodBase).GetGenericArguments().FirstOrDefault()?.Name;
                var declaringType = m.MethodBase.DeclaringType.Name;

                if (this.MergeArgument(m, arg))
                {
                    results.Add(m);
                }
            }

            return results;
        }

        /// <summary>
        /// Checks if we have enough generic arguments to resolve the method
        /// </summary>
        /// <returns></returns>
        protected void CheckMethodResolved(MethodData md)
        {
            if (md.MethodGenericsResolved)
            {
                return;
            }

            if (!md.GenericArguments.Keys.Except(md.MethodBase.GetGenericArguments().Select((i) => i.Name)).Any())
            {
                md.MethodBase = ((MethodInfo)md.MethodBase).GetGenericMethodDefinition()
                    .MakeGenericMethod(md.GenericArguments.Values.ToArray());

                md.MethodGenericsResolved = true;
                md.Parameters = md.MethodBase.GetParameters().ToArray();
            }
        }

        protected bool IsBetterThan(Expression[] args, MethodData first, MethodData second)
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                CompareConversionType result = CompareConversions(args[i].Type, first.Parameters[i].ParameterType, second.Parameters[i].ParameterType);

                // If second is better, return false
                if (result == CompareConversionType.Second)
                {
                    return false;
                }

                // If first is better, return true
                if (result == CompareConversionType.First)
                {
                    return true;
                }

                // If both are same, just set better to true and continue
                if (result == CompareConversionType.Both)
                {
                    better = true;
                }
            }

            return better;
        }

        // Return "First" if s -> t1 is a better conversion than s -> t2
        // Return "Second" if s -> t2 is a better conversion than s -> t1
        // Return "Both" if neither conversion is better
        protected CompareConversionType CompareConversions(Type source, Type first, Type second)
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

            bool firstIsCompatibleWithSecond = TypeHelper.IsCompatibleWith(first, second, out _);
            bool secondIsCompatibleWithFirst = TypeHelper.IsCompatibleWith(second, first, out _);

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
    }
}
