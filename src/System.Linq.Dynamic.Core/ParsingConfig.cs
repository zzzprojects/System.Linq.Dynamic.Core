using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Configuration class for System.Linq.Dynamic.Core.
    /// </summary>
    public class ParsingConfig
    {
        /// <summary>
        /// Default ParsingConfig
        /// </summary>
        public static ParsingConfig Default { get; } = new ParsingConfig();

        /// <summary>
        /// Default ParsingConfig for EntityFramework Core 2.1 and higher
        /// </summary>
        public static ParsingConfig DefaultEFCore21 { get; } = new ParsingConfig
        {
            EvaluateGroupByAtDatabase = true
        };

        private IDynamicLinkCustomTypeProvider _customTypeProvider;

        private IExpressionPromoter _expressionPromoter;

        private IQueryableAnalyzer _queryableAnalyzer;

        /// <summary>
        /// Gets or sets the <see cref="IDynamicLinkCustomTypeProvider"/>.
        /// </summary>
        public IDynamicLinkCustomTypeProvider CustomTypeProvider
        {
            get
            {
#if !(DOTNET5_1 || WINDOWS_APP || UAP10_0 || NETSTANDARD)
                // only use DefaultDynamicLinqCustomTypeProvider for full .NET Framework and NET Core App 2.x
                return _customTypeProvider ?? (_customTypeProvider = new DefaultDynamicLinqCustomTypeProvider());
#else
                return _customTypeProvider;
#endif
            }

            set
            {
                if (_customTypeProvider != value)
                {
                    _customTypeProvider = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IExpressionPromoter"/>.
        /// </summary>
        public IExpressionPromoter ExpressionPromoter
        {
            get
            {
                return _expressionPromoter ?? (_expressionPromoter = new ExpressionPromoter());
            }

            set
            {
                if (_expressionPromoter != value)
                {
                    _expressionPromoter = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IQueryableAnalyzer"/>.
        /// </summary>
        public IQueryableAnalyzer QueryableAnalyzer
        {
            get
            {
                return _queryableAnalyzer ?? (_queryableAnalyzer = new DefaultQueryableAnalyzer());
            }

            set
            {
                if (_queryableAnalyzer != value)
                {
                    _queryableAnalyzer = value;
                }
            }
        }

        /// <summary>
        /// Determines if the context keywords (it, parent, and root) are valid and usable inside a Dynamic Linq string expression.  
        /// Does not affect the usability of the equivalent context symbols ($, ^ and ~).
        /// 
        /// Default value is true.
        /// </summary>
        public bool AreContextKeywordsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use dynamic object class for anonymous types.
        ///
        /// Default value is false.
        /// </summary>
        public bool UseDynamicObjectClassForAnonymousTypes { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the EntityFramework version supports evaluating GroupBy at database level.
        /// See https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.1#linq-groupby-translation
        /// 
        /// Remark: when this setting is set to 'true', make sure to supply this ParsingConfig as first parameter on the extension methods.
        ///
        /// Default value is false.
        /// </summary>
        public bool EvaluateGroupByAtDatabase { get; set; } = false;

        /// <summary>
        /// Use Parameterized Names in generated dynamic SQL query.
        /// See https://github.com/graeme-hill/gblog/blob/master/source_content/articles/2014.139_entity-framework-dynamic-queries-and-parameterization.mkd
        ///
        /// Default value is false.
        /// </summary>
        public bool UseParameterizedNamesInDynamicQuery { get; set; } = false;

        /// <summary>
        /// Allows the New() keyword to evaluate any available Type.
        ///
        /// Default value is false.
        /// </summary>
        public bool AllowNewToEvaluateAnyType { get; set; } = false;

        /// <summary>
        /// Renames the (Typed)ParameterExpression empty Name to a the correct supplied name from `it`.
        ///
        /// Default value is false.
        /// </summary>
        public bool RenameParameterExpression { get; set; } = false;

        /// <summary>
        /// By default when a member is not found in a type and the type has a string based index accessor it will be parsed as an index accessor. Use
        /// this flag to disable this behaviour and have parsing fail when parsing an expression
        /// where a member access on a non existing member happens.
        ///
        /// Default value is false.
        /// </summary>
        public bool DisableMemberAccessToIndexAccessorFallback { get; set; } = false;

        /// <summary>
        /// By default finding types by a simple name is not supported.
        /// Use this flag to use the CustomTypeProvider to resolve types by a simple name like "Employee" instead of "MyDatabase.Entities.Employee".
        /// Note that a first matching type is returned and this functionality needs to scan all types from all assemblies, so use with caution.
        /// 
        /// Default value is false.
        /// </summary>
        public bool ResolveTypesBySimpleName { get; set; } = false;

        /// <summary>
        /// Support enumeration-types from the System namespace in mscorlib. An example could be "StringComparison".
        /// 
        /// Default value is true.
        /// </summary>
        public bool SupportEnumerationsFromSystemNamespace { get; set; } = true;

        /// <summary>
        /// By default DateTime (like 'Fri, 10 May 2019 11:03:17 GMT') is parsed as local time.
        /// Use this flag to parse all DateTime strings as UTC.
        ///
        /// Default value is false.
        /// </summary>
        public bool DateTimeIsParsedAsUTC { get; set; } = false;
    }
}
