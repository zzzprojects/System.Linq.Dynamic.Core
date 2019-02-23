using Moq;
using NFluent;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Entities;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class TypeFinderTests
    {
        private readonly ParsingConfig _parsingConfig = new ParsingConfig();
        private readonly Mock<IKeywordsHelper> _keywordsHelperMock;
        private readonly Mock<IDynamicLinkCustomTypeProvider> _dynamicTypeProviderMock;

        private readonly TypeFinder _sut;

        public TypeFinderTests()
        {
            _dynamicTypeProviderMock = new Mock<IDynamicLinkCustomTypeProvider>();
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(BaseEmployee).FullName)).Returns(typeof(BaseEmployee));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Boss).FullName)).Returns(typeof(Boss));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Worker).FullName)).Returns(typeof(Worker));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("Boss")).Returns(typeof(Boss));

            _parsingConfig = new ParsingConfig
            {
                CustomTypeProvider = _dynamicTypeProviderMock.Object
            };

            _keywordsHelperMock = new Mock<IKeywordsHelper>();

            _sut = new TypeFinder(_parsingConfig, _keywordsHelperMock.Object);
        }

        [Fact]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_false()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;

            // Act
            Type result = _sut.FindTypeByName("Boss", null, forceUseCustomTypeProvider: false);

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_true()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;

            // Act
            Type result = _sut.FindTypeByName("Boss", null, forceUseCustomTypeProvider: true);

            // Assert
            Check.That(result).Equals(typeof(Boss));
        }

        [Fact]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_basedon_it()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;
            var expressions = new[] { Expression.Parameter(typeof(BaseEmployee)) };

            // Act
            Type result = _sut.FindTypeByName("Boss", expressions, forceUseCustomTypeProvider: false);

            // Assert
            Check.That(result).Equals(typeof(Boss));
        }
    }
}
