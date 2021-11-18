using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleApp_net6._0
{
    class Program
    {
        static void Main(string[] args)
        {
            Normal();
            Dynamic();
        }

        private static void Normal()
        {
            var e = new int[0].AsQueryable();
            var q = new[] { 1 }.AsQueryable();

            var a = q.FirstOrDefault();
            var b = e.FirstOrDefault(44);

            var c = q.FirstOrDefault(i => i == 0);
            var d = q.FirstOrDefault(i => i == 0, 42);

            var t = q.Take(1);

            var al = q.All("it >= 0");
        }

        private static void Dynamic()
        {
            var e = new int[0].AsQueryable() as IQueryable;
            var q = new[] { 1 }.AsQueryable() as IQueryable;

            var a = q.FirstOrDefault();
            //var b = e.FirstOrDefault(44);

            var c = q.FirstOrDefault("it == 0");
            //var d = q.FirstOrDefault(i => i == 0, 42);
        }
    }
}
