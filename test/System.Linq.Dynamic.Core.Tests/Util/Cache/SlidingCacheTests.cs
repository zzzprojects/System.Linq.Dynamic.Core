using Moq;
using System.Linq.Dynamic.Core.Util;
using System.Linq.Dynamic.Core.Util.Cache;
using System.Linq.Expressions;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Util.Cache;

public class SlidingCacheTests
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 0, 0, 0);

    [Fact]
    public void SlidingCache_CacheOperations()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        // Configure Mock with SetupGet since SlidingCache can be non-deterministic; don't use SetupSequence
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new SlidingCache<int, string>(
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
        Assert.True(cache.TryGetValue(1, out _), "Expected to find the value, but did not");
        Assert.True(cache.TryGetValue(2, out _), "Expected to find the value, but did not");
        Assert.True(cache.TryGetValue(3, out _), "Expected to find the value, but did not");

        // Test Removal
        cache.Remove(1);
        cache.Count.Should().Be(2, $"Expected 2 items in the cache, only had {cache.Count}");
    }

    [Fact]
    public void SlidingCache_TestExpire()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        // Configure Mock with SetupGet since SlidingCache can be non-deterministic; don't use SetupSequence
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new SlidingCache<int, string>(TimeSpan.FromMinutes(10),
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        // move the time forward 
        var newDateTime = UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(newDateTime);

        // Ensure that the element has expired
        cache.TryGetValue(1, out var value).Should().BeFalse($"Expected to not find the value, but found {value}");
    }

    [Fact]
    public void SlidingCache_TestReturnExpiredItems()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        // Configure Mock with SetupGet since SlidingCache can be non-deterministic; don't use SetupSequence
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new SlidingCache<int, string>(TimeSpan.FromMinutes(10),
            dateTimeProvider: dateTimeUtilsMock.Object, returnExpiredItems: true);

        // Act
        cache.AddOrUpdate(1, "one");

        // move the time forward
        var newDateTime = UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(newDateTime);

        // Ensure the expired item is returned from the cache
        cache.TryGetValue(1, out _).Should().BeTrue("Expected to return expired item");
    }

    [Fact]
    public void SlidingCache_TestAutoExpire()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        // Configure Mock with SetupGet since SlidingCache can be non-deterministic; don't use SetupSequence
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new SlidingCache<int, string>(
            TimeSpan.FromMinutes(10),
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, had {cache.Count}");

        // move the time forward
        var newDateTime = UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(newDateTime);

        // Trigger the cleanup, asking for non-existing key
        cache.TryGetValue(10, out _);

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        Sleep();

        // Ensure no item is in the cache
        cache.Count.Should().Be(0, $"Expected 0 items in the cache, had {cache.Count}");
    }

    [Fact]
    public void SlidingCache_TestNull()
    {
        // Arrange
        var cache = new SlidingCache<Expression, string>(TimeSpan.FromMinutes(10));

        // Expect an ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => { cache.AddOrUpdate(null, "one"); });
    }

    [Fact]
    public void SlidingCache_TestMinNumberBeforeTests()
    {
        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        // Configure Mock with SetupGet since SlidingCache can be non-deterministic; don't use SetupSequence
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        // Arrange
        var cache = new SlidingCache<int, string>(
            TimeSpan.FromMinutes(10),
            minCacheItemsBeforeCleanup: 2,
            dateTimeProvider: dateTimeUtilsMock.Object);

        // Act
        cache.AddOrUpdate(1, "one");

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, only had {cache.Count}");

        // move the time forward
        var newDateTime = UtcNow.AddMinutes(11);
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(newDateTime);

        // Trigger the cleanup, asking for non-existing key
        cache.TryGetValue(10, out _);

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        Sleep();

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, only had {cache.Count}");

        // Act
        cache.AddOrUpdate(2, "two");

        // Since the cache cleanup is triggered by a Task and not on the same thread, 
        // give it a moment for the cleanup to happen
        Sleep();

        // Ensure one item is in the cache
        cache.Count.Should().Be(1, $"Expected 1 items in the cache, had {cache.Count}");
    }

    private static void Sleep()
    {
        Thread.Sleep(1000);
    }
}