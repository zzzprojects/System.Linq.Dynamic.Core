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
        public async Task Entities_AverageAsync()
        {
            // Arrange
            PopulateTestData(2, 0);

            double expected = await _context.Blogs.Select(b => b.BlogId).AverageAsync();

            // Act
            double actual = await _context.Blogs.Select("BlogId").AverageAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Entities_AverageAsync_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            double expected = await _context.Blogs.AverageAsync(b => b.BlogId);

            // Act
            double actual = await _context.Blogs.AverageAsync("BlogId");

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
