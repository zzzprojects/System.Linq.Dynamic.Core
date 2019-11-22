#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using System.Data.Entity;
using EntityFramework.DynamicLinq;
#endif
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_Sum_Integer()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = _context.Blogs.Select(b => b.BlogId).Sum();

            // Act
            var actual = _context.Blogs.Select("BlogId").Sum();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Entities_Sum_Double()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = _context.Blogs.Select(b => b.BlogId * 1.0d).Sum();

            // Act
            var actual = _context.Blogs.Select("BlogId * 1.0").Sum();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Entities_Sum_Integer_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = _context.Blogs.Sum(b => b.BlogId);

            // Act
            var actual = _context.Blogs.Sum("BlogId");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Entities_Sum_Double_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            var expected = _context.Blogs.Sum(b => b.BlogId * 1.0d);

            // Act
            var actual = _context.Blogs.Sum("BlogId * 1.0d");

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
