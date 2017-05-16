using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_Last_Predicate()
        {
            const string search = "a";

            //Arrange
            var blog1 = new Blog { Name = "blog a", BlogId = 1000 };
            var blog2 = new Blog { Name = "blog b", BlogId = 3000 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            //Act
            var expected = _context.Blogs.Last(b => b.Name.Contains(search));
            var result = _context.Blogs.Last("Name.Contains(@0)", search);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_LastOrDefault_Predicate()
        {
            const string search = "a";

            //Arrange
            var blog1 = new Blog { Name = "blog a", BlogId = 1000 };
            var blog2 = new Blog { Name = "blog b", BlogId = 3000 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            //Act
            var expected = _context.Blogs.LastOrDefault(b => b.Name.Contains(search));
            var result = _context.Blogs.LastOrDefault("Name.Contains(@0)", search);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_Single_Predicate()
        {
            const string search = "a";

            //Arrange
            var blog1 = new Blog { Name = "blog a", BlogId = 1000 };
            var blog2 = new Blog { Name = "blog b", BlogId = 3000 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            //Act
            var expected = _context.Blogs.Single(b => b.Name.Contains(search));
            var result = _context.Blogs.Single("Name.Contains(@0)", search);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_SingleOrDefault_Predicate()
        {
            const string search = "a";

            //Arrange
            var blog1 = new Blog { Name = "blog a", BlogId = 1000 };
            var blog2 = new Blog { Name = "blog b", BlogId = 3000 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            //Act
            var expected = _context.Blogs.SingleOrDefault(b => b.Name.Contains(search));
            var result = _context.Blogs.SingleOrDefault("Name.Contains(@0)", search);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}