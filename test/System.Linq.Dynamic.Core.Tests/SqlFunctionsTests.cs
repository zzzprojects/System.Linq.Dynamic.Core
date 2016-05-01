#if DNX452
using System.Collections;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Microsoft.Data.Entity;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    /// <summary>
    /// http://erikej.blogspot.nl/2014/06/entity-framework-6-and-sql-server.html
    /// </summary>
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
                var blog = new Blog { Name = "Blog" + (i + 1), Created = DateTime.Today.AddDays(-Rnd.Next(0, 100)) };

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
        public void SqlFunctions_Math_PI()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = _context.Blogs.Select(x => new { x.Name, p = x.BlogId + Math.PI }).First();

            //Act
            var test = _context.Blogs.Select("new (Name, BlogId + Math.PI as p)").ToDynamicArray().First();

            //Assert
            Assert.Equal(expected.Name, test.Name);
            Assert.Equal(expected.p, test.p, 10);
        }

        //[Fact]
        public void SqlFunctions_DateAdd()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = _context.Blogs.Select(x => new { x.Name, c = x.Created.AddDays(1) }).First();

            try
            {
                System.Data.Objects.EntityFunctions.AddDays(DateTime.Now, 1);
                System.Data.Entity.Core.Objects.EntityFunctions.AddDays(DateTime.Now, 1);

                System.Data.Objects.SqlClient.SqlFunctions.DateAdd("day", 1, DateTime.Now);
                System.Data.Entity.DbFunctions.AddDays(DateTime.Now, 1);
                System.Data.Entity.SqlServer.SqlFunctions.DateAdd("day", 1, DateTime.Now);
            }
            catch
            {
            }

            //Act
            //var test = _context.Blogs.Select("new (Name, DbFunctions.AddDays(Created, 1) as c)").ToDynamicArray().First();
            //var test2 = _context.Blogs.Select("new (Name, SqlFunctions.DateAdd(\"day\", 1, Created) as c)").ToDynamicArray().First();
            var test = _context.Blogs.Select("new (Name, EntityFunctions.AddDays(Created, 1) as c)").ToDynamicArray().First();

            //Assert
            Assert.Equal(expected.Name, test.Name);
            Assert.Equal(expected.c, test.c);
        }
    }
}
#endif