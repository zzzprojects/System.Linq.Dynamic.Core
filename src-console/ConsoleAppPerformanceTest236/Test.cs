using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleAppPerformanceTest236
{
    public static class Test
    {
        public static void DoIt()
        {
            var q = User.GenerateSampleModels(0).AsQueryable();

            int count = 1000;
            for (int i = 0; i <= count; i++)
            {
                if (i % 100 == 0)
                {
                    Console.WriteLine(i);
                }
                var result = q.OrderBy("FullName ASC, City DESC").FirstOrDefault();
            }
        }
    }
}
