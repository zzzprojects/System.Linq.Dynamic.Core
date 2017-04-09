namespace System.Linq.Dynamic.Core.Tokenizer
{
    internal struct Token
    {
        public TokenId Id { get; set; }

        public TokenId OriginalId { get; set; }

        public string Text { get; set; }

        public int Pos { get; set; }
    }
}