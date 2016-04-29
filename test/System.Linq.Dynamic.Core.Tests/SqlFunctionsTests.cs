#if DNX452
using System.Collections;
using System.Data.Entity;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class SqlFunctionsTests : IDisposable
    {
        static readonly Random Rnd = new Random(5);

        BlogContext _context;

        public SqlFunctionsTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=SqlFunctionsTests_{Guid.NewGuid()}.db");

            _context = new BlogContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _context.EnableLogging();
        }

        // Use TestCleanup to run code after each test has run
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _context = null;
        }

        void PopulateTestData(int blogCount = 25, int postCount = 10)
        {
            for (int i = 0; i < blogCount; i++)
            {
                var blog = new Blog { Name = "Blog" + (i + 1) };

                _context.Blogs.Add(blog);

                for (int j = 0; j < postCount; j++)
                {
                    var post = new Post
                    {
                        Blog = blog,
                        Title = $"Blog {i + 1} - Post {j + 1}",
                        Content = "My Content",
                        PostDate = DateTime.Today.AddDays(-Rnd.Next(0, 100)).AddSeconds(Rnd.Next(0, 30000)),
                        NumberOfReads = Rnd.Next(0, 5000)
                    };

                    _context.Posts.Add(post);
                }
            }

            _context.SaveChanges();
        }

        [Fact]
        public void SqlFunctions_PI()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = _context.Blogs.Select(x => new { x.Name, p = Math.PI }).First();

            //Act
            var test = _context.Blogs.Select("new (Name, Math.PI as p)").ToDynamicArray().First();

            //Assert
            Assert.Equal(expected.Name, test.Name);
            Assert.Equal(expected.p, test.p);
        }
    }
}
#endif