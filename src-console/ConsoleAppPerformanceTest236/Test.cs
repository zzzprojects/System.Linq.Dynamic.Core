using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleAppPerformanceTest236
{
    public static class Test
    {
        public static void DoIt()
        {
            var q = User.GenerateSampleModels(10000).AsQueryable();

            int count = 100;
            for (int i = 0; i <= count; i++)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine(i);
                }
                var result = q.OrderBy("FullName ASC, City DESC").FirstOrDefault();
            }
        }
    }
}
