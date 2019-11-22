using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_LongCount_Predicate()
        {
            const string search = "a";

            //Arrange
            var blog1 = new Blog { Name = "blog a", BlogId = 1000, Created = DateTime.Now };
            var blog2 = new Blog { Name = "blog b", BlogId = 3000, Created = DateTime.Now };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            //Act
            long expected = _context.Blogs.LongCount(b => b.Name.Contains(search));
            long result = _context.Blogs.LongCount("Name.Contains(@0)", search);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
