namespace System.Linq.Dynamic.Core.Parser.SupportedOperands
{
    internal interface IEqualitySignatures : IRelationalSignatures
    {
        void F(bool x, bool y);
        void F(bool? x, bool? y);

        // Disabled 4 lines below because of : https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
        //void F(DateTime x, string y);
        //void F(DateTime? x, string y);
        //void F(string x, DateTime y);
        //void F(string x, DateTime? y);

        void F(Guid x, Guid y);
        void F(Guid? x, Guid? y);
        void F(Guid x, string y);
        void F(Guid? x, string y);
        void F(string x, Guid y);
        void F(string x, Guid? y);
    }
}
