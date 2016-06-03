using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void In()
        {
            //Arrange
            var testRange = Enumerable.Range(1, 100).ToArray();
            var testModels = User.GenerateSampleModels(10);
            var testModelByUsername = string.Format("Username in (\"{0}\",\"{1}\",\"{2}\")", testModels[0].UserName, testModels[1].UserName, testModels[2].UserName);
            var testInExpression = new int[] { 2, 4, 6, 8 };

            //Act
            var result1a = testRange.AsQueryable().Where("it in (2,4,6,8)").ToArray();
            var result1b = testRange.AsQueryable().Where("it in (2, 4,  6, 8)").ToArray();
            // https://github.com/NArnott/System.Linq.Dynamic/issues/52
            var result2 = testModels.AsQueryable().Where(testModelByUsername).ToArray();
            var result3 =
                testModels.AsQueryable()
                    .Where("Id in (@0, @1, @2)", testModels[0].Id, testModels[1].Id, testModels[2].Id)
                    .ToArray();
            var result4 = testRange.AsQueryable().Where("it in @0", testInExpression).ToArray();

            //Assert
            Assert.Equal(new int[] { 2, 4, 6, 8 }, result1a);
            Assert.Equal(new int[] { 2, 4, 6, 8 }, result1b);
            Assert.Equal(testModels.Take(3).ToArray(), result2);
            Assert.Equal(testModels.Take(3).ToArray(), result3);
            Assert.Equal(new int[] { 2, 4, 6, 8 }, result4);
        }
    }
}