namespace System.Linq.Dynamic.Core.Util.Cache;

internal static class SlidingCacheConstants
{
    // Default cleanup frequency
    public static readonly TimeSpan DefaultCleanupFrequency = TimeSpan.FromMinutes(10);

    // Default Time-To-Live
    public static readonly TimeSpan DefaultTimeToLive = TimeSpan.FromMinutes(10);
}