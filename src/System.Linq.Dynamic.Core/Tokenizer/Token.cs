
namespace System.Linq.Dynamic.Core.Tokenizer
{
    public struct Token
    {
        public TokenId Id { get; set; }
        public string Text { get; set; }
        public int Pos { get; set; }
    }
}