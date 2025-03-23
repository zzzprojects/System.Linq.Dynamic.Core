using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Entities;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class TypeFinderTests
{
    private readonly ParsingConfig _parsingConfig;

    private readonly TypeFinder _sut;

    public TypeFinderTests()
    {
        var dynamicTypeProviderMock = new Mock<IDynamicLinqCustomTypeProvider>();
        dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(BaseEmployee).FullName!)).Returns(typeof(BaseEmployee));
        dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Boss).FullName!)).Returns(typeof(Boss));
        dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Worker).FullName!)).Returns(typeof(Worker));
        dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("Boss")).Returns(typeof(Boss));

        _parsingConfig = new ParsingConfig
        {
            CustomTypeProvider = dynamicTypeProviderMock.Object
        };

         var keywordsHelperMock = new Mock<IKeywordsHelper>();

        _sut = new TypeFinder(_parsingConfig, keywordsHelperMock.Object);
    }

    [Fact]
    public void TypeFinder_TryFindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_Equals_false()
    {
        // Assign
        _parsingConfig.ResolveTypesBySimpleName = true;

        // Act
        var result = _sut.TryFindTypeByName("Boss", null, forceUseCustomTypeProvider: false, out var type);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TypeFinder_TryFindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_Equals_true()
    {
        // Assign
        _parsingConfig.ResolveTypesBySimpleName = true;

        // Act
        var result = _sut.TryFindTypeByName("Boss", null, forceUseCustomTypeProvider: true, out var type);

        // Assert
        result.Should().BeTrue();
        type.Should().Be<Boss>();
    }

    [Fact]
    public void TypeFinder_TryFindTypeByName_With_SimpleTypeName_BasedOn_it()
    {
        // Assign
        _parsingConfig.ResolveTypesBySimpleName = true;
        var expressions = new[] { Expression.Parameter(typeof(BaseEmployee)) };

        // Act
        var result = _sut.TryFindTypeByName("Boss", expressions, forceUseCustomTypeProvider: false, out var type);

        // Assert
        result.Should().BeTrue();
        type.Should().Be<Boss>();
    }
}