using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    public class DataRecord
    {
        public List<Dictionary<string, string>> ListOfDictionaries { get; }
        public Dictionary<string, string> FirstDict { get; }
        public List<string> ListOfKeys { get; }

        public DataRecord(List<Dictionary<string, string>> listOfDictionaries, Dictionary<string, string> firstDict, List<string> listOfKeys)
        {
            ListOfDictionaries = listOfDictionaries;
            FirstDict = firstDict;
            ListOfKeys = listOfKeys;
        }
    }

    // #793
    [Theory]
    [InlineData("FirstDict.ContainsKey(\"test\")")]
    [InlineData("ListOfKeys.Any(x => x.Contains(\"test\"))")]
    [InlineData("ListOfDictionaries.Any(it.ContainsKey(\"test\"))")]
    [InlineData("ListOfDictionaries.Any(ContainsKey(\"test\"))")]
    [InlineData("ListOfDictionaries.Any(x => x.ContainsKey(\"test\"))")]
    [InlineData("ListOfDictionaries.Any(x => x.ContainsKey(\"te\" + \"st\"))")]
    public void ContainsKey_Dynamic(string expression)
    {
        // Arrange
        var dataList = new List<Dictionary<string, string>>
        {
            new()
            {
                ["test"] = "value1"
            },
            new()
            {
                ["otherKey"] = "value2"
            },
            new()
            {
                ["test"] = "value3"
            },
        };

        var data = new DataRecord(dataList, dataList[0], dataList.SelectMany(d => d.Keys).ToList());
        var queryableDataRecord = new[] { data }.AsQueryable();

        // Act
        var result = queryableDataRecord.Any(expression);

        // Assert
        result.Should().BeTrue();
    }
}