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
        [Fact]
        public void Entities_GroupBy_SingleKey()
        {
            // "memory leak" warning exception starting from EF Core 3.x
#if !EFCORE_3X
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
#endif
        }

        [Fact]
        public void Entities_GroupBy_MultiKey()
        {
            // "memory leak" warning exception starting from EF Core 3.x
#if !EFCORE_3X
            //Arrange
            PopulateTestData(5, 15);

            var expected = _context.Posts.GroupBy(x => new { x.BlogId, x.PostDate }).OrderBy(x => x.Key.PostDate).ToArray();

            //Act
            var test = _context.Posts.GroupBy("new (BlogId, PostDate)").OrderBy("Key.PostDate").ToDynamicArray();


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
#endif
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_SingleResult()
        {
            // "memory leak" warning exception starting from EF Core 3.x
#if !EFCORE_3X
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
                var testRow = (IGrouping<DateTime, string>)test[i];

                Assert.Equal(expectedRow.Key, testRow.Key);
                Assert.Equal(expectedRow.ToArray(), testRow.ToArray());
            }
#endif
        }

        [Fact]
        public void Entities_GroupBy_SingleKey_MultiResult()
        {
            // "memory leak" warning exception starting from EF Core 3.x
#if !EFCORE_3X
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
#endif
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
    }
}
