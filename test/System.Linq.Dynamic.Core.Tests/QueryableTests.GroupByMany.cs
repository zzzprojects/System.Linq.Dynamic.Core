using System.Collections.Generic;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void GroupByMany_Dynamic_StringExpressions()
        {
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var sel = lst.AsQueryable().GroupByMany("Item1", "Item2");

            Assert.Equal(sel.Count(), 2);
            Assert.Equal(sel.First().Subgroups.Count(), 1);
            Assert.Equal(sel.Skip(1).First().Subgroups.Count(), 2);
        }
    }
}