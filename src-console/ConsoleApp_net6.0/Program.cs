using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleApp_net6._0
{
    public class X
    {
        public string Key { get; set; } = null!;

        public List<Y>? Contestants { get; set; }
    }

    public class Y
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            var q = new[]
            {
                new X { Key = "x" },
                new X { Key = "a" },
                new X { Key = "a", Contestants = new List<Y> { new Y() } }
            }.AsQueryable();
            var groupByKey = q.GroupBy("Key");
            var selectQry = groupByKey.Select("new (Key, Sum(np(Contestants.Count, 0)) As TotalCount)").ToDynamicList();

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