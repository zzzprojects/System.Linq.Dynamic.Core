using System.Collections;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
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
#if EFCORE
            var builder = new DbContextOptionsBuilder();
            // builder.UseSqlite($"Filename=System.Linq.Dynamic.Core.{Guid.NewGuid()}.db");
            builder.UseInMemoryDatabase($"System.Linq.Dynamic.Core.{Guid.NewGuid()}");
            //builder.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=.\\System.Linq.Dynamic.Core.{Guid.NewGuid()}.mdf;");

            _context = new BlogContext(builder.Options);
            //_context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
#else
            //_context = new BlogContext(@"data source=.\EntityFramework.DynamicLinq.sqlite");
            //_context = new BlogContext($"Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=.\\System.Linq.Dynamic.Core.{Guid.NewGuid()}.mdf;");
            //_context = new BlogContext($"Filename=System.Linq.Dynamic.Core.{Guid.NewGuid()}.sqlitedb");
            _context = new BlogContext($"data source=(LocalDB)\\MSSQLLocalDB;attachdbfilename=|DataDirectory|\\System.Linq.Dynamic.Core.{Guid.NewGuid()}.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework");
            //_context.Database.Delete();
            //_context.Database.CreateIfNotExists();
#endif
        }

        // Use TestCleanup to run code after each test has run
        public void Dispose()
        {
#if EFCORE
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
                var blog = new Blog { X = i.ToString(), Name = "Blog" + (i + 1), BlogId = 1000 + i, Created = DateTime.Now.AddDays(-Rnd.Next(0, 100)) };

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
