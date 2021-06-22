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
        /// Test for https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/524
        /// </summary>
        [Fact]
        public void Entities_Where_In_And()
        {
            // Arrange
            PopulateTestData();

            var expected = _context.Blogs.Include(b => b.Posts).Where(b => new[] { 1, 3, 5 }.Contains(b.BlogId) && new [] { "Blog3", "Blog4" }.Contains(b.Name)).ToArray();

            // Act
            var test = _context.Blogs.Include(b => b.Posts).Where(@"BlogId in (1, 3, 5) and Name in (""Blog3"", ""Blog4"")").ToArray();

            // Assert
            Assert.NotEmpty(test);
            Assert.Equal(expected, test);
        }
    }
}
