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
        /// <summary>
        /// Test for https://github.com/StefH/System.Linq.Dynamic.Core/issues/28
        /// </summary>
        [Fact]
        public void Entities_Where_Include()
        {
            // Arrange
            PopulateTestData(5, 2);

            var expected = _context.Blogs.Include(b => b.Posts).Where(b => b.BlogId > 2000).ToArray();

            // Act
            var test = _context.Blogs.Include(b => b.Posts).Where("BlogId > 2000").ToArray();

            // Assert
            Assert.Equal(expected, test);
        }
    }
}