using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Min()
        {
            // Arrange
            var incomes = User.GenerateSampleModels(100).Select(u => u.Income);

            // Act
            var expected = incomes.Min();
            var actual = incomes.AsQueryable().Min();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Min_Selector()
        {
            // Arrange
            var users = User.GenerateSampleModels(100);

            // Act
            var expected = users.Min(u => u.Income);
            var result = users.AsQueryable().Min("Income");

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
