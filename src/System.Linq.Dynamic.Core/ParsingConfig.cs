using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Configuration class for Dynamic Linq.
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
        /// </summary>
        public bool AreContextKeywordsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use dynamic object class for anonymous types.
        /// </summary>
        public bool UseDynamicObjectClassForAnonymousTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the EntityFramwwork version supports evaluating GroupBy at database level.
        /// See https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.1#linq-groupby-translation
        /// </summary>
        public bool EvaluateGroupByAtDatabase { get; set; }
    }
}
