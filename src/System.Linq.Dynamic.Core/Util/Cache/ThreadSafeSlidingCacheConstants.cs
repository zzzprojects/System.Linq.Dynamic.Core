namespace System.Linq.Dynamic.Core.Util.Cache;

internal static class ThreadSafeSlidingCacheConstants
{
    // Default cleanup frequency
    public static readonly TimeSpan DefaultCleanupFrequency = TimeSpan.FromMinutes(10);
}
