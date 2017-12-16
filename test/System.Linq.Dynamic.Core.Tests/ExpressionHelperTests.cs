using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid()
        {
            // Assign
            string guidAsString = Guid.NewGuid().ToString();

            // Act
            Expression result = ExpressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsInstanceOf<ConstantExpression>();
            Check.That(result.ToString()).Equals(guidAsString);
        }

        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid_Invalid()
        {
            // Assign
            string guidAsString = "x";

            // Act
            Expression result = ExpressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsNull();
        }
    }
}
