using FluentAssertions;
using System.Linq.Dynamic.Core.TypeConverters;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.TypeConvertors
{
    public class CustomDateTimeConverterTests
    {
        private readonly CustomDateTimeConverter _sut;

        public CustomDateTimeConverterTests()
        {
            _sut = new CustomDateTimeConverter();
        }

        [Fact]
        public void ConvertFromInvariantString_WithGMT_ReturnsCorrectDateTime()
        {
            // Arrange
            string value = "Fri, 10 May 2019 11:03:17 GMT";

            // Act
            DateTime? result = _sut.ConvertFromInvariantString(value) as DateTime?;

            // Assert
            result.Should().Be(new DateTime(2019, 5, 10, 11, 3, 17));
        }

        [Fact]
        public void ConvertFromInvariantString_WithPST_ReturnsCorrectDateTime()
        {
            // Arrange
            string value = "Fri, 10 May 2019 11:03:17 -07:00";

            // Act
            DateTime? result = _sut.ConvertFromInvariantString(value) as DateTime?;

            // Assert
            result.Should().Be(new DateTime(2019, 5, 10, 18, 3, 17));
        }
    }
}
