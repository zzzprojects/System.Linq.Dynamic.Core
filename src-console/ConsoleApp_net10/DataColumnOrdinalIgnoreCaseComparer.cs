using System;
using System.Collections;

namespace ConsoleApp_net6._0;

public class DataColumnOrdinalIgnoreCaseComparer : IComparer
{
    public int Compare(object? x, object? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        if (x is string xAsString && y is string yAsString)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(xAsString, yAsString);
        }

        return Comparer.Default.Compare(x, y);
    }
}