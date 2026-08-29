using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class ParsingConfigTests
{
    class TestQueryableAnalyzer : IQueryableAnalyzer
    {
        public bool SupportsLinqToObjects(IQueryable query, IQueryProvider? provider = null)
        {
            return true;
        }
    }

    [Fact]
    public void ParsingConfig_QueryableAnalyzer_Set_Null()
    {
        // Assign
        var config = ParsingConfig.Default;

        // Assert
        Assert.NotNull(config.QueryableAnalyzer);
    }

    [Fact]
    public void ParsingConfig_QueryableAnalyzer_Set_Custom()
    {
        // Assign
        var config = ParsingConfig.Default;
        var analyzer = new TestQueryableAnalyzer();

        // Act
        config.QueryableAnalyzer = analyzer;

        // Assert
        Assert.Equal(analyzer, config.QueryableAnalyzer);
    }
}