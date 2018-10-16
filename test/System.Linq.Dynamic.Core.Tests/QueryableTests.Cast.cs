using Newtonsoft.Json.Linq;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Cast_Explicit()
        {
            // Assign
            var test1 = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test 9") }
            };

            var test2 = new JObject
            {
                { "Id", new JValue(10) },
                { "Name", new JValue("Test 10") }
            };

            var queryable = new[] { test1, test2 }.AsQueryable();

            // Act
            var result = queryable.Select(x => new { Id = (int)x["Id"], Name = (string)x["Name"] }).OrderBy(x => x.Id).ToArray();
            var resultDynamic = queryable.Select("new (int(Id) as Id, string(Name) as Name)").OrderBy("Id").ToDynamicArray();

            // Assert
            Check.That(resultDynamic.Count()).Equals(result.Count());
            Check.That(resultDynamic[0].Id).Equals(result[0].Id);
            Check.That(resultDynamic[0].Name).Equals(result[0].Name);
        }
    }
}
