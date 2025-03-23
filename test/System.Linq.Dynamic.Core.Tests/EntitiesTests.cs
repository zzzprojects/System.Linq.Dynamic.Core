using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests : IClassFixture<EntitiesTestsDatabaseFixture>
{
    private static readonly Random Rnd = new Random(1);

    private readonly BlogContext _context;

    public EntitiesTests(EntitiesTestsDatabaseFixture fixture)
    {
#if EFCORE
        var builder = new DbContextOptionsBuilder();
        if (fixture.UseInMemory)
        {
            builder.UseInMemoryDatabase($"System.Linq.Dynamic.Core.{Guid.NewGuid()}");
        }
        else
        {
            builder.UseSqlServer(fixture.ConnectionString);
        }

        _context = new BlogContext(builder.Options);
        _context.Database.EnsureCreated();
#else
        _context = new BlogContext(fixture.ConnectionString);
#endif
        InternalPopulateTestData();
    }

    private void InternalPopulateTestData()
    {
        if (_context.Blogs.Any())
        {
            return;
        }

        for (int i = 0; i < 25; i++)
        {
            var blog = new Blog
            {
                X = i.ToString(),
                Name = "Blog" + (i + 1),
                BlogId = 1000 + i,
                Created = DateTime.Now.AddDays(-Rnd.Next(0, 100))
            };

            _context.Blogs.Add(blog);

            for (int j = 0; j < 10; j++)
            {
                var postDate = DateTime.Today.AddDays(-Rnd.Next(0, 100)).AddSeconds(Rnd.Next(0, 30000));
                var post = new Post
                {
                    PostId = 10000 + i * 10 + j,
                    Blog = blog,
                    Title = $"Blog {i + 1} - Post {j + 1}",
                    Content = "My Content",
                    PostDate = postDate,
                    CloseDate = Rnd.Next(0, 10) < 5 ? postDate.AddDays(1) : null,
                    NumberOfReads = Rnd.Next(0, 5000)
                };

                _context.Posts.Add(post);
            }
        }

        var singleBlog = new Blog
        {
            X = "42",
            Name = "SingleBlog",
            BlogId = 12345678,
            Created = DateTime.Now.AddDays(-Rnd.Next(0, 100))
        };
        _context.Blogs.Add(singleBlog);

        _context.Blogs.Add(new Blog { BlogId = 2000, X = "0", Name = "blog a", Created = DateTime.Now });
        _context.Blogs.Add(new Blog { BlogId = 2001, X = "0", Name = "blog b", Created = DateTime.Now });
        _context.Blogs.Add(new Blog { BlogId = 3000, X = "0", Name = "Blog1", Created = DateTime.Now, NullableInt = null });
        _context.Blogs.Add(new Blog { BlogId = 3001, X = "0", Name = "Blog2", Created = DateTime.Now, NullableInt = 5 });

        _context.SaveChanges();
    }
}