using System.Collections;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using FluentAssertions;
using Newtonsoft.Json;
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
        public void Entities_Select_SingleColumn_NullCoalescing()
        {
            //A rrange
            var blog1 = new Blog { BlogId = 1000, Name = "Blog1", Created = DateTime.Now, NullableInt = null };
            var blog2 = new Blog { BlogId = 2000, Name = "Blog2", Created = DateTime.Now, NullableInt = 5 };
            _context.Blogs.Add(blog1);
            _context.Blogs.Add(blog2);
            _context.SaveChanges();

            var expected1 = _context.Blogs.Select(x => (int?)(x.NullableInt ?? 10)).ToArray();
            var expected2 = _context.Blogs.Select(x => (int?)(x.NullableInt ?? 9 + x.BlogId)).ToArray();

            // Act
            var test1 = _context.Blogs.Select<int?>("NullableInt ?? 10").ToArray();
            var test2 = _context.Blogs.Select<int?>("NullableInt ?? 9 + BlogId").ToArray();

            // Assert
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
        public void Entities_Select_EmptyObject()
        {
            ParsingConfig config = ParsingConfig.Default;
            config.EvaluateGroupByAtDatabase = true;

            // Arrange
            PopulateTestData(5, 0);

            var expected = _context.Blogs.Select(x => new {}).ToList();

            // Act
            var test = _context.Blogs.GroupBy(config, "BlogId", "new()").Select<object>("new()").ToList();

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(test));
        }

        [Fact]
        public void Entities_Select_BrokenObject()
        {
            ParsingConfig config = ParsingConfig.Default;
            config.DisableMemberAccessToIndexAccessorFallback = false;

            // Silently creates something that will later fail on materialization
            var test = _context.Blogs.Select(config, "new(~.BlogId)");
            test = test.Select(config, "new(nonexistentproperty as howcanthiswork)");

            // Will fail when creating the expression
            config.DisableMemberAccessToIndexAccessorFallback = true;
            Assert.ThrowsAny<ParseException>(() =>
            {
                test = test.Select(config, "new(nonexistentproperty as howcanthiswork)");
            });
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

        // fixed : EF issue !!! https://github.com/aspnet/EntityFramework/issues/4968
        [Fact]
        public void Entities_Select_BlogAndPosts()
        {
            //Arrange
            PopulateTestData(5, 5);

            var expected = _context.Blogs.Select(x => new { x.BlogId, x.Name, x.Posts }).ToArray();

            //Act
            var test = _context.Blogs.Select("new (BlogId, Name, Posts)").ToDynamicArray();

            //Assert
            Assert.Equal(expected.Length, test.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var expectedRow = expected[i];
                var testRow = test[i];

                Assert.Equal(expectedRow.BlogId, testRow.BlogId);
                Assert.Equal(expectedRow.Name, testRow.Name);

                Assert.True(expectedRow.Posts != null);
                Assert.Equal(expectedRow.Posts.ToList(), testRow.Posts);
            }
        }

        [Fact]
        public void Entities_Select_Blog_And_Call_Where()
        {
            // Arrange
            PopulateTestData(5, 0);

            // Act
            var result = _context.Blogs
                .Select("new (BlogId, Name)")
                .Where("Name == \"Blog2\"")
                .ToDynamicArray();

            // Assert
            Assert.Equal(1, result.Length);
        }
    }
}