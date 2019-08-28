using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ETG.SABENTISpro.Utils.HelpersUtils;
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

        private List<Expression> _args;

        /// <summary>
        /// Get an instance of MethodReducer
        /// </summary>
        /// <param name="parsingConfig"></param>
        public MethodReducer(ParsingConfig parsingConfig, Type ownerType, string methodName)
        {
            _parsingConfig = parsingConfig;
            _ownerType = ownerType;
            _methodName = methodName;
            _args = new List<Expression>();

            this._applicableMethods = this.LoadMembers(_ownerType, true, methodName);
        }

        public List<MethodData> GetCurrentApplicableMethods()
        {
            return _applicableMethods;
        }

        /// <summary>
        /// Add a new argument and reduce the set of available methods
        /// </summary>
        /// <param name="arg"></param>
        public List<MethodData> Reduce(Expression arg)
        {
            int currentPosition = _args.Count;
            _args.Add(arg);

            // Reduce the set of matching methods
            _applicableMethods = _applicableMethods
                .Where(m => IsApplicable(m, _args, currentPosition))
                .ToList();

            return _applicableMethods;
        }

        /// <summary>
        /// Find all available methods without considering method signature.
        /// </summary>
        protected List<MethodData> LoadMembers(Type type, bool staticAccess, string methodName)
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
            var extensionMethods = type.GetExtensionMethods();

            var result = new List<MethodData>();
            result.AddRange(members.Select((i) => new MethodData((MethodBase)i)));
            result.AddRange(extensionMethods.Select((i) => new MethodData((MethodBase)i)));
            return result;
        }

        /// <summary>
        /// Se if the current set of arguments apply to a method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        protected bool IsApplicable(MethodData method, List<Expression> args, int currentPosition)
        {
            if (args.Count > method.Parameters.Length)
            {
                return false;
            }

            Expression[] promotedArgs = new Expression[args.Count];

            for (int i = currentPosition; i < args.Count; i++)
            {
                ParameterInfo pi = method.Parameters[i];

                if (pi.IsOut)
                {
                    return false;
                }

                // TODO: Not sure why this criteria here... kept from previous implementation
                bool canConvert = method.MethodBase.DeclaringType != typeof(IEnumerableSignatures);

                Expression promoted = this._parsingConfig.ExpressionPromoter.Promote(
                    args[i],
                    pi.ParameterType,
                    false,
                    canConvert);

                if (promoted == null)
                {
                    return false;
                }

                promotedArgs[i] = promoted;
            }

            method.Args = promotedArgs;
            return true;
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

            bool firstIsCompatibleWithSecond = TypeHelper.IsCompatibleWith(first, second);
            bool secondIsCompatibleWithFirst = TypeHelper.IsCompatibleWith(second, first);

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
