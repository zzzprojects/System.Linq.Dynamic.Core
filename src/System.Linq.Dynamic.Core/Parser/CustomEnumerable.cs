using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq.Dynamic.Core.Parser
{
    public static class CustomEnumerable
    {
        //public static float? AVG(this IEnumerable<float?> source)
        //{
        //    return source.Average();
        //}
        //public static decimal AVG(this IEnumerable<decimal> source)
        //{
        //    return source.Average();
        //}
        //public static double AVG(this IEnumerable<int> source)
        //{
        //    return source.Average();
        //}
        //public static double AVG(this IEnumerable<long> source)
        //{
        //    return source.Average();
        //}
        public static double AVG(this IEnumerable<double> source)
        {
            return source.Average();
        }

        public static double SUM(this IEnumerable<double> source)
        {
            return source.Sum();
        }

        public static IEnumerable<double> SORT(this IEnumerable<double> source)
        {
            return source.OrderBy(v => v);
        }

        public static IEnumerable<double> SORT(this IEnumerable<double> source, int dir)
        {
            if (dir == -1)
            {
                return source.OrderByDescending(v => v);
            }

            return source.OrderBy(v => v);
        }

        public static double LAST(this IEnumerable<double> source)
        {
            return source.Last();
        }
        //public static double? AVG(this IEnumerable<double?> source)
        //{
        //    return source.Average();
        //}
        //public static double? AVG(this IEnumerable<int?> source)
        //{
        //    return source.Average();
        //}
        //public static double? AVG(this IEnumerable<long?> source)
        //{
        //    return source.Average();
        //}
        //public static decimal? AVG(this IEnumerable<decimal?> source)
        //{
        //    return source.Average();
        //}
        //public static float AVG(this IEnumerable<float> source)
        //{
        //    return source.Average();
        //}
    }
}
