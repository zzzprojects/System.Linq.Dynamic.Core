﻿using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class DynamicClassFactoryTests
{
    public DynamicClassFactoryTests()
    {
        DynamicClassFactory.ClearGeneratedTypes();
    }

    [Fact]
    public void CreateGenericComparerTypeForInt()
    {
        // Assign
        var comparer = new CustomCaseInsensitiveComparer();
        var comparerGenericType = typeof(IComparer<>).MakeGenericType(typeof(int));
        var a = 1;
        var b = 2;

        // Act
        var type = DynamicClassFactory.CreateGenericComparerType(comparerGenericType, comparer.GetType());

        // Assert
        var instance = (IComparer<int>)Activator.CreateInstance(type)!;
        int greaterThan = instance.Compare(a, b);
        greaterThan.Should().Be(1);

        int equal = instance.Compare(a, a);
        equal.Should().Be(0);

        int lessThan = instance.Compare(b, a);
        lessThan.Should().Be(-1);
    }

    [Fact]
    public void CreateGenericComparerTypeForString()
    {
        // Assign
        var comparer = new CustomCaseInsensitiveComparer();
        var comparerGenericType = typeof(IComparer<>).MakeGenericType(typeof(string));
        var a = "a";
        var b = "b";

        // Act
        var type = DynamicClassFactory.CreateGenericComparerType(comparerGenericType, comparer.GetType());

        // Assert
        var instance = (IComparer<string>)Activator.CreateInstance(type);
        int greaterThan = instance.Compare(a, b);
        greaterThan.Should().Be(1);

        int equal = instance.Compare(a, a);
        equal.Should().Be(0);

        int lessThan = instance.Compare(b, a);
        lessThan.Should().Be(-1);
    }

    [Fact]
    public void CreateGenericComparerTypeForDateTime()
    {
        // Assign
        var comparer = new CustomCaseInsensitiveComparer();
        var comparerGenericType = typeof(IComparer<>).MakeGenericType(typeof(DateTime));
        var a = new DateTime(2022, 1, 1);
        var b = new DateTime(2023, 1, 1);

        // Act
        var type = DynamicClassFactory.CreateGenericComparerType(comparerGenericType, comparer.GetType());

        // Assert
        var instance = (IComparer<DateTime>)Activator.CreateInstance(type);
        int greaterThan = instance.Compare(a, b);
        greaterThan.Should().Be(1);

        int equal = instance.Compare(a, a);
        equal.Should().Be(0);

        int lessThan = instance.Compare(b, a);
        lessThan.Should().Be(-1);
    }
}

public class CustomCaseInsensitiveComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return new CaseInsensitiveComparer().Compare(y, x);
    }
}
