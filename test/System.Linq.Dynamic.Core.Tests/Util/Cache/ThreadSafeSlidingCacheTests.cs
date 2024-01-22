using Moq;
using System.Linq.Dynamic.Core.Util;
using System.Linq.Dynamic.Core.Util.Cache;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Util.Cache;

public class ThreadSafeSlidingCacheTests
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 0, 0, 0);

    [Fact]
    public void ThreadSafeSlidingCache_CacheOperations()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new ThreadSafeSlidingCache<int, string>(
            TimeSpan.FromSeconds(1),
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Add
        cache.AddOrUpdate(1, "one");
        cache.AddOrUpdate(2, "two");
        cache.AddOrUpdate(3, "three");

        // Replace
        cache.AddOrUpdate(1, "oneone");

        cache.Count.Should().Be(3, $"Expected 3 items in the cache, only had {cache.Count}");

        // Test retrieval
        Assert.True(cache.TryGetValue(1, out var value1), "Expected to find the value, but did not");
        Assert.True(cache.TryGetValue(2, out var value2), "Expected to find the value, but did not");
        Assert.True(cache.TryGetValue(3, out var value3), "Expected to find the value, but did not");

        // Test Removal
        cache.Remove(1);
        cache.Count.Should().Be(2, $"Expected 2 items in the cache, only had {cache.Count}");
    }

    [Fact]
    public void ThreadSafeSlidingCache_TestExpire()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        var cache = new ThreadSafeSlidingCache<int, string>(TimeSpan.FromMinutes(10),
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        var r = dateTimeUtilsMock.Object.UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(r);

        if (cache.TryGetValue(1, out var value))
        {
            Assert.True(false, $"Expected to not find the value, but found {value}");
        }
    }

    [Fact]
    public void ThreadSafeSlidingCache_TestAutoExpire()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new ThreadSafeSlidingCache<int, string>(
            TimeSpan.FromMinutes(10),
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, only had {cache.Count}");

        // move the time forward
        var r = dateTimeUtilsMock.Object.UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(r);

        // Trigger the cleanup, asking for non-existing key
        cache.TryGetValue(10, out var _);

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        System.Threading.Thread.Sleep(10);

        // Ensure one item is in the cache
        cache.Count.Should().Be(0, $"Expected 0 items in the cache, only had {cache.Count}");
    }

    [Fact]
    public void ThreadSafeSlidingCache_TestNull()
    {
        // Arrange
        var cache = new ThreadSafeSlidingCache<Expression, string>(TimeSpan.FromMinutes(10));

        // Expect an ArgumentNullException
        var exception = Assert.Throws<ArgumentNullException>(() => { cache.AddOrUpdate(null, "one"); });
    }

    [Fact]
    public void ThreadSafeSlidingCache_TestMinNumberBeforeTests()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new ThreadSafeSlidingCache<int, string>(
            TimeSpan.FromMinutes(10),
            minCacheItemsBeforeCleanup: 2,
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, only had {cache.Count}");

        // move the time forward
        var r = dateTimeUtilsMock.Object.UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(r);

        // Trigger the cleanup, asking for non-existing key
        cache.TryGetValue(10, out var _);

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        System.Threading.Thread.Sleep(10);

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, only had {cache.Count}");

        // Act
        cache.AddOrUpdate(2, "two");

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        System.Threading.Thread.Sleep(10);

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, had {cache.Count}");
    }

}
