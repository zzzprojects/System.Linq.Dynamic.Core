namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Configuration class for Dynamic Linq.
    /// </summary>
    public class ParsingConfig
    {
        internal static ParsingConfig Default { get; } = new ParsingConfig();

        /// <summary>
        /// Gets or sets a value indicating whether to use dynamic object class for anonymous types.
        /// </summary>
        /// <value>
        /// <c>true</c> if wether to use dynamic object class for anonymous types; otherwise, <c>false</c>.
        /// </value>
        public bool UseDynamicObjectClassForAnonymousTypes { get; set; }
    }
}
