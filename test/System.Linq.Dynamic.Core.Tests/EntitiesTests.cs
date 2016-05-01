#if (!NETSTANDARD)
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
    /// <summary>
    /// Summary description for EntitiesTests
    /// </summary>
    public class EntitiesTests : IDisposable
    {
        BlogContext _context;

        #region Entities Test Support

        static readonly Random Rnd = new Random(1);

        public EntitiesTests()
        {
#if !(NET452)
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=DynamicLinqTestDb_{Guid.NewGuid()}.db");

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

        #endregion

        #region Select Tests

        [Fact]
        public void Entities_Select_SingleColumn_NullCoalescing()
        {
            //Arrange
            var blog1 = new Blog { BlogId = 1000, Name = "Blog1", NullableInt = null };
            var blog2 = new Blog { BlogId = 2000, Name = "Blog2", NullableInt = 5 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            var expected1 = _context.Blogs.Select(x => x.NullableInt ?? 10).ToArray();
            var expected2 = _context.Blogs.Select(x => x.NullableInt ?? 9 + x.BlogId).ToArray();

            //Act
            var test1 = _context.Blogs.Select<int>("NullableInt ?? 10").ToArray();
            var test2 = _context.Blogs.Select<int>("NullableInt ?? 9 + BlogId").ToArray();

            //Assert
            Assert.Equal(expected1, test1);
            Assert.Equal(expected2, test2);
        }

        [Fact]
        public void Entities_Select_SingleColumn()
        {
            //Arrange
            PopulateTestData(5, 0);

            var expected = _context.Blogs.Select(x => x.BlogId).ToArray();

            //Act
            var test = _context.Blogs.Select<int>("BlogId").ToArray();

            //Assert
            Assert.Equal<ICollection>(expected, test);
        }

        [Fact]
        public void Entities_Select_MultipleColumn()
        {
            //Arrange
            PopulateTestData(5, 0);

            var expected = _context.Blogs.Select(x => new { X = "x", x.BlogId, x.Name }).ToArray();

            //Act
            var test = _context.Blogs.Select("new (\"x\" as X, BlogId, Name)").ToDynamicArray();

            //Assert
            Assert.Equal(
                expected,
                test.Select(x => new { X = "x", BlogId = (int)x.BlogId, Name = (string)x.Name }).ToArray() //convert to same anomymous type used by expected so they can be found equal
                );
        }

        [Fact]
        public void Entities_Select_BlogPosts()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Blogs.Where(x => x.BlogId == 1).SelectMany(x => x.Posts).Select(x => x.PostId).ToArray();

            //Act
            var test = _context.Blogs.Where(x => x.BlogId == 1).SelectMany("Posts").Select<int>("PostId").ToArray();

            //Assert
            Assert.Equal(expected, test);
        }

        // TODO : EF issue !!! https://github.com/aspnet/EntityFramework/issues/4968
        //[Fact]
        //public void Entities_Select_BlogAndPosts()
        //{
        //    //Arrange
        //    PopulateTestData(5, 5);

        //    var expected = _context.Blogs.Select(x => new { x.BlogId, x.Name, x.Posts }).ToArray();

        //    //Act
        //    var test = _context.Blogs.Select("new (BlogId, Name, Posts)").ToDynamicArray();

        //    //Assert
        //    Assert.Equal(expected.Length, test.Length);
        //    for (int i = 0; i < expected.Length; i++)
        //    {
        //        var expectedRow = expected[i];
        //        var testRow = test[i];

        //        Assert.Equal(expectedRow.BlogId, testRow.BlogId);
        //        Assert.Equal(expectedRow.Name, testRow.Name);

        //        Assert.True(expectedRow.Posts != null);
        //        Assert.Equal(expectedRow.Posts.ToList(), testRow.Posts);
        //    }
        //}

        #endregion

        #region GroupBy Tests

        [Fact]
        public void Entities_GroupBy_SingleKey()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.BlogId).ToArray();

            //Act
            var test = _context.Posts.GroupBy("BlogId").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<int, Post>)test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [Fact]
        public void Entities_GroupBy_MultiKey()
        {
            //Arrange
            PopulateTestData(5, 15);

            var expected = _context.Posts.GroupBy(x => new { x.BlogId, x.PostDate }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("new (BlogId, PostDate)").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DynamicClass, Post>)test[i];

                Assert.Equal(expectedRow.Key.BlogId, ((dynamic)testRow.Key).BlogId);
                Assert.Equal(expectedRow.Key.PostDate, ((dynamic)testRow.Key).PostDate);
                Assert.Equal(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_SingleResult()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate, x => x.Title).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate", "Title").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DateTime, String>)test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_MultiResult()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate, x => new { x.Title, x.Content }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate", "new (Title, Content)").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DateTime, DynamicClass>)test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(
                    expectedRow.ToArray(),
                    testRow.Cast<dynamic>().Select(x => new { Title = (string)x.Title, Content = (string)x.Content }).ToArray());
            }
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_Count()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate).Select(x => new { x.Key, Count = x.Count() }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate").Select("new(Key, Count() AS Count)").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];
                var testRow = test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(expectedRow.Count, testRow.Count);
            }
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_Sum()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate).Select(x => new { x.Key, Reads = x.Sum(y => y.NumberOfReads) }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate").Select("new(Key, Sum(NumberOfReads) AS Reads)").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];
                var testRow = test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(expectedRow.Reads, testRow.Reads);
            }
        }

        #endregion

        #region Executor Tests

        [Fact]
        public void FirstOrDefault_AsStringExpressions()
        {
            //Arrange
            PopulateTestData();

            //remove all posts from first record (to allow Defaults case to validate)
            _context.Posts.RemoveRange(_context.Blogs.OrderBy(x => x.BlogId).First<Blog>().Posts);
            _context.SaveChanges();

            //Act
            var firstExpected = _context.Blogs.OrderBy(x => x.Posts.OrderBy(y => y.PostDate).FirstOrDefault().PostDate).Select(x => x.BlogId);
            var firstTest = _context.Blogs.OrderBy("Posts.OrderBy(PostDate).FirstOrDefault().PostDate").Select<int>("BlogId");

            //Assert
            Assert.Equal(firstExpected.ToArray(), firstTest.ToArray());
        }

        #endregion
    }
}
#endif