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

        [Theory]
        [InlineData("Fri, 10 May 2019 11:03:17 GMT", 11)]
        [InlineData("Fri, 10 May 2019 11:03:17 -07:00", 18)]
        public void ConvertFromInvariantString_ReturnsCorrectDateTime(string value, int hours)
        {
            // Act
            DateTime? result = _sut.ConvertFromInvariantString(value) as DateTime?;

            // Assert
            result.Should().Be(new DateTime(2019, 5, 10, hours, 3, 17));
        }
    }
}
