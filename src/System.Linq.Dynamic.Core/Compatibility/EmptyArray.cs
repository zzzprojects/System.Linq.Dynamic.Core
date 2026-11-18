// ReSharper disable once CheckNamespace
namespace System;

internal static class EmptyArray<T>
{
#if NET35 || NET40 || NET45 || NET452
    public static readonly T[] Value = [];
#else
    public static readonly T[] Value = Array.Empty<T>();
#endif
}