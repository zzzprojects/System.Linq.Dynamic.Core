#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using System.Data.Entity;
using EntityFramework.DynamicLinq;
#endif
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public async Task Entities_SumAsync_Integer()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = await _context.Blogs.Select(b => b.BlogId).SumAsync();

            // Act
            var actual = await _context.Blogs.Select("BlogId").SumAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Entities_SumAsync_Double()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = await _context.Blogs.Select(b => b.BlogId * 1.0d).SumAsync();

            // Act
            var actual = await _context.Blogs.Select("BlogId").SumAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Entities_SumAsync_Integer_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = await _context.Blogs.SumAsync(b => b.BlogId);

            // Act
            var actual = await _context.Blogs.SumAsync("BlogId");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Entities_SumAsync_Double_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = await _context.Blogs.SumAsync(b => b.BlogId * 1.0d);

            // Act
            var actual = await _context.Blogs.SumAsync("BlogId * 1.0d");

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
