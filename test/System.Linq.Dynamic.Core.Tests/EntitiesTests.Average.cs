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
        public void Entities_Average()
        {
            // Arrange
            PopulateTestData(2, 0);

            // Act
            double expected = _context.Blogs.Select(b => b.BlogId).Average();
            double actual = _context.Blogs.Select("BlogId").Average();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Entities_Average_Selector()
        {
            // Arrange
            PopulateTestData(2, 0);

            double expected = _context.Blogs.Average(b => b.BlogId);

            // Act
            double actual = _context.Blogs.Average("BlogId");

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
