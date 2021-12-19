namespace System.Linq.Dynamic.Core.Tokenizer
{
    /// <summary>
    /// Token
    /// </summary>
    public struct Token
    {
        /// <summary>
        /// The TokenId.
        /// </summary>
        public TokenId Id { get; set; }

        /// <summary>
        /// The Original TokenId.
        /// </summary>
        public TokenId OriginalId { get; set; }

        /// <summary>
        /// The text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The position.
        /// </summary>
        public int Pos { get; set; }
    }
}
