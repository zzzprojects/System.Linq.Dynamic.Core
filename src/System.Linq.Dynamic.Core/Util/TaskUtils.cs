using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Util;

internal static class TaskUtils
{
    public static void Run(Action action)
    {
        Check.NotNull(action);
#if NET35 || NET40
        System.Threading.ThreadPool.QueueUserWorkItem(_ => action.Invoke());
#else
        System.Threading.Tasks.Task.Run(action);
#endif
    }
}