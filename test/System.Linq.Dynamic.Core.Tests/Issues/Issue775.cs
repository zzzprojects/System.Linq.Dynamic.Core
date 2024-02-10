using NFluent;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Issue775()
    {
        // Arrange
        var users = User.GenerateSampleModels(10);
        var pets = new[] { new Pet() }.AsQueryable();

        // Act
        var realResult = users.Where(x => x.Income == pets.Select(p => p.Id).FirstOrDefault()).Select(x => x.Id).ToArray();
        var result = users.AsQueryable().Where("Income == @0.Select(Id).FirstOrDefault()", pets).Select("Id");

        // Assert
        Check.That(result.ToDynamicArray().Cast<Guid>()).ContainsExactly(realResult);
    }
}