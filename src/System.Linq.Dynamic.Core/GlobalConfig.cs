using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Static configuration class for Dynamic Linq.
    /// </summary>
    public static class GlobalConfig
    {
        static IDynamicLinkCustomTypeProvider _customTypeProvider;

        /// <summary>
        /// Gets or sets the <see cref="IDynamicLinkCustomTypeProvider"/>.
        /// </summary>
        public static IDynamicLinkCustomTypeProvider CustomTypeProvider
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

                    ExpressionParser.ResetDynamicLinqTypes();
                }
            }
        }

        static bool _contextKeywordsEnabled = true;

        /// <summary>
        /// Determines if the context keywords (it, parent, and root) are valid and usable inside a Dynamic Linq string expression.  
        /// Does not affect the usability of the equivalent context symbols ($, ^ and ~).
        /// </summary>
        public static bool AreContextKeywordsEnabled
        {
            get { return _contextKeywordsEnabled; }
            set
            {
                if (value != _contextKeywordsEnabled)
                {
                    _contextKeywordsEnabled = value;

                    ExpressionParser.ResetDynamicLinqTypes();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool UseDynamicObjectClassForAnonymousTypes { get; set; }

        /// <summary>
        /// The DecimalSeparators to use, can be '.' or ','
        /// </summary>
        //public static char NumberDecimalSeparator { get; set; }
    }
}