using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Tests.TestClasses;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.CustomTypeProviders;

public class DefaultDynamicLinqCustomTypeProviderTests
{
    private readonly IList<Type> _additionalTypes = new List<Type>
    {
        typeof(DirectoryInfo),
        typeof(DefaultDynamicLinqCustomTypeProviderTests)
    };

    private readonly DefaultDynamicLinqCustomTypeProvider _sut;

    public DefaultDynamicLinqCustomTypeProviderTests()
    {
        _sut = new DefaultDynamicLinqCustomTypeProvider(ParsingConfig.Default, _additionalTypes);
    }

    [Fact]
    public void DefaultDynamicLinqCustomTypeProvider_ResolveSystemType()
    {
        // Act
        var type = _sut.ResolveType(typeof(DirectoryInfo).FullName!);

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
        var result = _sut.ResolveType(typeof(DefaultDynamicLinqCustomTypeProviderTests).FullName!);

        // Assert
        Check.That(result).IsNotNull();
    }

    [Fact]
    public void DefaultDynamicLinqCustomTypeProvider_ResolveTypeBySimpleName_UsesAdditionalTypes()
    {
        // Act
        var result = _sut.ResolveTypeBySimpleName(nameof(TestClassWithDynamicLinqAttribute));

        // Assert
        Check.That(result).IsNotNull();
    }

    [Fact]
    public void DefaultDynamicLinqCustomTypeProvider_ResolveTypeBySimpleName_UsesTypesMarkedWithDynamicLinqTypeAttribute()
    {
        // Act
        var result = _sut.ResolveTypeBySimpleName(nameof(DirectoryInfo));

        // Assert
        Check.That(result).IsNotNull();
    }

    [Fact]
    public void DefaultDynamicLinqCustomTypeProvider_ResolveTypeBySimpleName_UnknownReturnsNull()
    {
        // Act
        var result = _sut.ResolveTypeBySimpleName("Dummy123");

        // Assert
        Check.That(result).IsNull();
    }
}