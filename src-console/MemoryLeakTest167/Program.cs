using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;

namespace MemoryLeakTest167
{
    class Program
    {
        static void Main(string[] asArgs)
        {
            RunThreads(8, RawMemoryLeak);
        }

        private static void RunThreads(int nThreads, Action actThreadStart)
        {
            for (int i = 0; i < nThreads; i++)
            {
                Thread thread = new Thread(new ThreadStart(actThreadStart));
                thread.Start();
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }

        private static void RawMemoryLeak()
        {
            while (true)
            {
                string sExpr = "1234567890";
                LambdaExpression expr = DynamicExpressionParser.ParseLambda(new ParameterExpression[0], typeof(double), sExpr);
            }
        }
    }
}
