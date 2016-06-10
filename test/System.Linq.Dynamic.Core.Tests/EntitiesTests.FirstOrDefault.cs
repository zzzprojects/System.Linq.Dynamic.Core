using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
#if EFCORE
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using EntityFramework.DynamicLinq;
#endif
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_FirstOrDefault_Dynamic()
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
    }
}