using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DynamicClassFactoryTests
    {
        [Fact]
        public void CreateGenericComparerType()
        {
            // Assign
            var comparer = new IntComparer();
            var comparerGenericType = typeof(IComparer<>).MakeGenericType(typeof(int));

            // Act
            var type = DynamicClassFactory.CreateGenericComparerType(comparerGenericType, comparer.GetType());

            // Assert
            var instance = (IComparer<int>)Activator.CreateInstance(type);
            int greaterThan = instance.Compare(1, 2);
            greaterThan.Should().Be(1);

            int equal = instance.Compare(1, 1);
            equal.Should().Be(0);

            int lessThan = instance.Compare(2, 1);
            lessThan.Should().Be(-1);
        }
    }

    public class IntComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return new CaseInsensitiveComparer().Compare(y, x);
        }
    }
}