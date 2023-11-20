using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        // Not supported : https://msdn.microsoft.com/en-in/library/bb738474%28en-us%29.aspx
        [Fact(Skip = "not supported")]
        public void Entities_TakeWhile()
        {
            //Arrange
            const int total = 33;
            PopulateTestData(total, 0);

            //Act
            var expected = _context.Blogs.OrderBy(b => b.BlogId).TakeWhile(b => b.BlogId > 5).ToArray();
            var result = _context.Blogs.OrderBy("BlogId").TakeWhile("b.BlogId > 5").ToDynamicArray<Blog>();

            //Assert
            Assert.Equal(expected, result);
        }
    }
}