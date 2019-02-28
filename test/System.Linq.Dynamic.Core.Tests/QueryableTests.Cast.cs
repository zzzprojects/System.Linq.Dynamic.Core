using Newtonsoft.Json.Linq;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        // [Fact] --> System.Linq.Dynamic.Core.Exceptions.ParseException : No property or field 'Id' exists in type 'JObject'
        public void Cast_Explicit()
        {
            // Assign
            var test1 = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test 9") },
                { "Items", new JArray(new JValue(1), new JValue("one")) }
            };

            var test2 = new JObject
            {
                { "Id", new JValue(10) },
                { "Name", new JValue("Test 10") },
                { "Items", new JArray(new JValue(2), new JValue("two")) }
            };

            var queryable = new[] { test1, test2 }.AsQueryable();

            // Act
            var result = queryable.Select(x => new { Id = (int)x["Id"], Name = (string)x["Name"], Item0 = (int) x["Items"].Values().ToArray()[0], Item1 = (string)x["Items"].Values().ToArray()[1] }).OrderBy(x => x.Id).ToArray();
            var resultDynamic = queryable.Select("new (int(Id) as Id, string(Name) as Name, int(Items[0]) as Item0, string(Items[1]) as Item1)").OrderBy("Id").ToDynamicArray();

            // Assert
            Check.That(resultDynamic.Count()).Equals(result.Count());
            Check.That(resultDynamic[0].Id).Equals(result[0].Id);
            Check.That(resultDynamic[0].Name).Equals(result[0].Name);
            Check.That(resultDynamic[0].Item0).Equals(result[0].Item0);
            Check.That(resultDynamic[0].Item1).Equals(result[0].Item1);

            Check.That(resultDynamic[1].Id).Equals(result[1].Id);
            Check.That(resultDynamic[1].Name).Equals(result[1].Name);
            Check.That(resultDynamic[1].Item0).Equals(result[1].Item0);
            Check.That(resultDynamic[1].Item1).Equals(result[1].Item1);
        }
    }
}
