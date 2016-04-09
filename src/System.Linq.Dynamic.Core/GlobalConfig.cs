namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Static configuration class for Dynamic Linq.
    /// </summary>
    public static class GlobalConfig
    {
        static IDynamicLinkCustomTypeProvider _customTypeProvider;

        /// <summary>
        /// Gets or sets the <see cref="IDynamicLinkCustomTypeProvider"/>.  Defaults to <see cref="DefaultDynamicLinqCustomTypeProvider" />.
        /// </summary>
        public static IDynamicLinkCustomTypeProvider CustomTypeProvider
        {
            get
            {
                if (_customTypeProvider == null) _customTypeProvider = new DefaultDynamicLinqCustomTypeProvider();

                return _customTypeProvider;
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
    }
}