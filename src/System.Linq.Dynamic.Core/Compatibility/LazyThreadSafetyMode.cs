#if NET35
namespace System.Threading
{
    internal enum LazyThreadSafetyMode
    {
        None,
        PublicationOnly,
        ExecutionAndPublication,
    }
}
#endif