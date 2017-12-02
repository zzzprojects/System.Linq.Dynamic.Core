namespace System.Linq.Dynamic.Core.Parser.SupportedOperands
{
    internal interface IRelationalSignatures : IArithmeticSignatures
    {
        void F(string x, string y);
        void F(char x, char y);
        void F(DateTime x, DateTime y);
        void F(DateTimeOffset x, DateTimeOffset y);
        void F(TimeSpan x, TimeSpan y);
        void F(char? x, char? y);
        void F(DateTime? x, DateTime? y);
        void F(DateTimeOffset? x, DateTimeOffset? y);
        void F(TimeSpan? x, TimeSpan? y);
    }
}
