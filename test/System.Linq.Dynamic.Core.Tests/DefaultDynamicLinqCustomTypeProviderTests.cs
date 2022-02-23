using System.IO;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DefaultDynamicLinqCustomTypeProviderTests
    {
        private readonly IDynamicLinkCustomTypeProvider _sut;

        public DefaultDynamicLinqCustomTypeProviderTests()
        {
            _sut = new DefaultDynamicLinqCustomTypeProvider();
        }

        [Fact]
        public void DefaultDynamicLinqCustomTypeProvider_ResolveSystemType()
        {
            // Act
            var type = _sut.ResolveType(typeof(DirectoryInfo).FullName);

            // Assert
            type.Should().Be(typeof(DirectoryInfo));
        }

        [Fact]
        public void DefaultDynamicLinqCustomTypeProvider_GetCustomTypes()
        {
            // Act
            var types = _sut.GetCustomTypes();

            // Assert
            Check.That(types.Count).IsStrictlyGreaterThan(0);
        }

        [Fact]
        public void DefaultDynamicLinqCustomTypeProvider_ResolveType_UnknownReturnsNull()
        {
            // Act
            var result = _sut.ResolveType("dummy");

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void DefaultDynamicLinqCustomTypeProvider_ResolveType_DefinedReturnsType()
        {
            // Act
            var result = _sut.ResolveType(typeof(DefaultDynamicLinqCustomTypeProviderTests).FullName);

            // Assert
            Check.That(result).IsNotNull();
        }
    }
}
