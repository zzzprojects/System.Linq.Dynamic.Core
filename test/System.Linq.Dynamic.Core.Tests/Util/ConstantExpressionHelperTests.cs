using System.Linq.Dynamic.Core.Parser;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        //[Fact]
        //public async Task TestConstantExpressionLeak()
        //{
        //    //Arrange
        //    PopulateTestData(1, 0);

        //    var populateExpression = _context.Blogs.All("BlogId > 2000");

        //    var expressions = ConstantExpressionHelper.Expressions;

        //    // Should contain
        //    if (!expressions.TryGetValue(2000, out _))
        //    {
        //        Assert.Fail("Cache was missing constant expression for 2000");
        //    }

        //    // wait half the expiry time
        //    await Task.Delay(TimeSpan.FromSeconds(ConstantExpressionHelper.Expressions.TimeToLive.TotalSeconds/2));
        //    if (!expressions.TryGetValue(2000, out _))
        //    {
        //        Assert.Fail("Cache was missing constant expression for 2000 (1)");
        //    }

        //    // wait another half the expiry time, plus one second
        //    await Task.Delay(TimeSpan.FromSeconds((ConstantExpressionHelper.Expressions.TimeToLive.TotalSeconds / 2)+1));
        //    if (!expressions.TryGetValue(2000, out _))
        //    {
        //        Assert.Fail("Cache was missing constant expression for 2000 (2)");
        //    }

        //    // Wait for the slide cache to expire, check on second later
        //    await Task.Delay(ConstantExpressionHelper.Expressions.TimeToLive.Add(TimeSpan.FromSeconds(1)));

        //    if (expressions.TryGetValue(2000, out _))
        //    {
        //        Assert.Fail("Expected constant to be expired 2000");
        //    }
        //}
    }
}
