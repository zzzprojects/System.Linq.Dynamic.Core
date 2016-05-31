using System.Collections;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
#if (NETSTANDARD)
using Microsoft.EntityFrameworkCore;
#elif NET452
using System.Data.Entity;
#else
using Microsoft.Data.Entity;
#endif
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests : IDisposable
    {
        BlogContext _context;

        static readonly Random Rnd = new Random(1);

        public EntitiesTests()
        {
#if !(NET452)
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=System.Linq.Dynamic.Core.{Guid.NewGuid()}.db");

            _context = new BlogContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
#else
            _context = new BlogContext();
            _context.Database.CreateIfNotExists();
#endif
        }

        // Use TestCleanup to run code after each test has run
        public void Dispose()
        {
#if !(NET452)
            _context.Database.EnsureDeleted();
#else
            _context.Database.Delete();
#endif

            _context.Dispose();
            _context = null;
        }

        private void PopulateTestData(int blogCount = 25, int postCount = 10)
        {
            for (int i = 0; i < blogCount; i++)
            {
                var blog = new Blog { Name = "Blog" + (i + 1), BlogId = 1000 + i };

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
    }
}