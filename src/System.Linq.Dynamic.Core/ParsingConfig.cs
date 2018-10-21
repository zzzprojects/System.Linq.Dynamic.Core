using System.Linq.Dynamic.Core.CustomTypeProviders;

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

        /// <summary>
        /// Gets or sets the <see cref="IDynamicLinkCustomTypeProvider"/>.
        /// </summary>
        public IDynamicLinkCustomTypeProvider CustomTypeProvider
        {
            get
            {
#if !(DOTNET5_1 || WINDOWS_APP || UAP10_0 || NETSTANDARD)
                // only use DefaultDynamicLinqCustomTypeProvider if not WINDOWS_APP || UAP10_0 || NETSTANDARD
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
        /// Determines if the context keywords (it, parent, and root) are valid and usable inside a Dynamic Linq string expression.  
        /// Does not affect the usability of the equivalent context symbols ($, ^ and ~).
        /// Default value is true.
        /// </summary>
        public bool AreContextKeywordsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use dynamic object class for anonymous types. Default value is false.
        /// </summary>
        public bool UseDynamicObjectClassForAnonymousTypes { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the EntityFramwwork version supports evaluating GroupBy at database level. Default value is false.
        /// See https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.1#linq-groupby-translation
        /// </summary>
        public bool EvaluateGroupByAtDatabase { get; set; } = false;

        /// <summary>
        /// Allows the New() keyword to evaluate any available Type. Default value is false.
        /// </summary>
        public bool AllowNewToEvaluateAnyType { get; set; } = false;

        /// <summary>
        /// Renames the (Typed)ParameterExpression empty Name to a the correct supplied name from `it`. Default value is false.
        /// </summary>
        public bool RenameParameterExpression { get; set; } = false;
    }
}
