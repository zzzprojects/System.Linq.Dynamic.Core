using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Max()
        {
            // Arrange
            var incomes = User.GenerateSampleModels(100).Select(u => u.Income);

            // Act
            var expected = incomes.Max();
            var actual = incomes.AsQueryable().Max();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Max_Selector()
        {
            // Arrange
            var users = User.GenerateSampleModels(100);

            // Act
            var expected = users.Max(u => u.Income);
            var result = users.AsQueryable().Max("Income");

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
