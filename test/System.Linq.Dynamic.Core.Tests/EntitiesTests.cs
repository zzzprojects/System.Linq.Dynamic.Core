#if !DNXCORE50
using System.Collections;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Microsoft.Data.Entity;
using TestToolsToXunitProxy;
using Assert = Xunit.Assert;

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
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=DynamicLinqTestDb_{Guid.NewGuid()}.db");

            _context = new BlogContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
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
                    var post = new Post()
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

        [TestMethod]
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

        [TestMethod]
        public void Entities_Select_MultipleColumn()
        {
            //Arrange
            PopulateTestData(5, 0);

            var expected = _context.Blogs.Select(x => new { x.BlogId, x.Name }).ToArray();

            //Act
            var test = _context.Blogs.Select("new (BlogId, Name)").ToDynamicArray();


            //Assert
            CollectionAssert.AreEqual(
                expected,
                test.Select(x => new { BlogId = (int)x.BlogId, Name = (string)x.Name }).ToArray() //convert to same anomymous type used by expected so they can be found equal
                );
        }

        [TestMethod]
        public void Entities_Select_BlogPosts()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Blogs.Where(x => x.BlogId == 1).SelectMany(x => x.Posts).Select(x => x.PostId).ToArray();

            //Act
            var test = _context.Blogs.Where(x => x.BlogId == 1).SelectMany("Posts").Select<int>("PostId").ToArray();

            //Assert
            CollectionAssert.AreEqual(expected, test);
        }

        //[TestMethod]
        //public void Entities_Select_BlogAndPosts()
        //{
        //    //Arrange
        //    PopulateTestData(5, 5);

        //    var expected = _context.Blogs.Select(x => new { x.BlogId, x.Name, x.Posts }).ToArray();

        //    //Act
        //    var test = _context.Blogs.Select("new (BlogId, Name, Posts)").ToDynamicArray();

        //    //Assert
        //    Assert.AreEqual(expected.Length, test.Length);
        //    for (int i = 0; i < expected.Length; i++)
        //    {
        //        var expectedRow = expected[i];
        //        var testRow = test[i];

        //        Assert.AreEqual(expectedRow.BlogId, testRow.BlogId);
        //        Assert.AreEqual(expectedRow.Name, testRow.Name);

        //        Assert.IsTrue(expectedRow.Posts != null);
        //        CollectionAssert.AreEqual(expectedRow.Posts.ToList(), testRow.Posts);
        //    }
        //}

#endregion

#region GroupBy Tests

        [TestMethod]
        public void Entities_GroupBy_SingleKey()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.BlogId).ToArray();

            //Act
            var test = _context.Posts.GroupBy("BlogId").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<int, Post>)test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key, testRow.Key);
                CollectionAssert.AreEqual(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [TestMethod]
        public void Entities_GroupBy_MultiKey()
        {
            //Arrange
            PopulateTestData(5, 15);

            var expected = _context.Posts.GroupBy(x => new { x.BlogId, x.PostDate }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("new (BlogId, PostDate)").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DynamicClass, Post>)test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key.BlogId, ((dynamic)testRow.Key).BlogId);
                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key.PostDate, ((dynamic)testRow.Key).PostDate);
                CollectionAssert.AreEqual(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [TestMethod]
        public void Entities_GroupBy_SingleKey_SingleResult()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate, x => x.Title).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate", "Title").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DateTime, String>)test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key, testRow.Key);
                CollectionAssert.AreEqual(expectedRow.ToArray(), testRow.ToArray());
            }
        }

        [TestMethod]
        public void Entities_GroupBy_SingleKey_MultiResult()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate, x => new { x.Title, x.Content }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate", "new (Title, Content)").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];

                //For some reason, the DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DateTime, DynamicClass>)test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key, testRow.Key);
                CollectionAssert.AreEqual(
                    expectedRow.ToArray(),
                    testRow.Cast<dynamic>().Select(x => new { Title = (string)x.Title, Content = (string)x.Content }).ToArray());
            }
        }

        [TestMethod]
        public void Entities_GroupBy_SingleKey_Count()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate).Select(x => new { x.Key, Count = x.Count() }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate").Select("new(Key, Count() AS Count)").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];
                var testRow = test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key, testRow.Key);
                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Count, testRow.Count);
            }
        }

        [TestMethod]
        public void Entities_GroupBy_SingleKey_Sum()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Posts.GroupBy(x => x.PostDate).Select(x => new { x.Key, Reads = x.Sum(y => y.NumberOfReads) }).ToArray();

            //Act
            var test = _context.Posts.GroupBy("PostDate").Select("new(Key, Sum(NumberOfReads) AS Reads)").ToDynamicArray();

            //Assert
            TestToolsToXunitProxy.Assert.AreEqual(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];
                var testRow = test[i];

                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Key, testRow.Key);
                TestToolsToXunitProxy.Assert.AreEqual(expectedRow.Reads, testRow.Reads);
            }
        }

#endregion

#region Executor Tests

        [TestMethod]
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
            CollectionAssert.AreEqual(firstExpected.ToArray(), firstTest.ToArray());
        }

#endregion
    }
}
#endif