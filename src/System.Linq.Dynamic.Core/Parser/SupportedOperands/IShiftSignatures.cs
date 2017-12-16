namespace System.Linq.Dynamic.Core.Parser.SupportedOperands
{
    internal interface IShiftSignatures
    {
        void F(int x, int y);
        void F(uint x, int y);
        void F(long x, int y);
        void F(ulong x, int y);
        void F(int? x, int y);
        void F(uint? x, int y);
        void F(long? x, int y);
        void F(ulong? x, int y);
        void F(int x, int? y);
        void F(uint x, int? y);
        void F(long x, int? y);
        void F(ulong x, int? y);
        void F(int? x, int? y);
        void F(uint? x, int? y);
        void F(long? x, int? y);
        void F(ulong? x, int? y);
    }
}
