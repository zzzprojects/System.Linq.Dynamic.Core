using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_Any()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            bool expectedAny1 = expectedQueryable1.Any();

            var expectedQueryable2 = _context.Blogs.Where(b => b.BlogId > 9999);
            bool expectedAny2 = expectedQueryable2.Any();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            bool any1 = queryable1.Any();

            IQueryable queryable2 = _context.Blogs.Where("BlogId > 9999");
            bool any2 = queryable2.Any();

            //Assert
            Assert.Equal(expectedAny1, any1);
            Assert.Equal(expectedAny2, any2);
        }

        // Expression of type 'System.Collections.Generic.IEnumerable`1[System.Boolean]' cannot be used for return type 'System.Collections.Generic.IEnumerable`1[System.Object]'
        //[Fact]
        //public void Entities_Any_Predicate()
        //{
        //    //Arrange
        //    PopulateTestData(1, 0);

        //    bool expectedAny1 = _context.Blogs.Any(b => b.BlogId > 0);
        //    bool expectedAny2 = _context.Blogs.Any(b => b.BlogId > 9999);

        //    //Act
        //    bool any1 = _context.Blogs.AsQueryable().Any("it.BlogId > 0");
        //    bool any2 = _context.Blogs.AsQueryable().Any("it.BlogId > 9999");

        //    //Assert
        //    Assert.Equal(expectedAny1, any1);
        //    Assert.Equal(expectedAny2, any2);
        //}
    }
}