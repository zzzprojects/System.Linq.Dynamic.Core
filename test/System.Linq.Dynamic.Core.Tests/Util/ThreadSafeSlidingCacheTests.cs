using System.Linq.Dynamic.Core.Util;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Util
{
    public class ThreadSafeSlidingCacheTests
    {
        [Fact]
        public void ThreadSafeSlidingCache_CacheOperations()
        {
            var mockDateTime = new MockDateTimeProvider();

            // Arrange
            var cache = new ThreadSafeSlidingCache<int, string>(
                TimeSpan.FromSeconds(1), 
                dateTimeProvider: mockDateTime);

            // Add
            cache.AddOrUpdate(1, "one");
            cache.AddOrUpdate(2, "two");
            cache.AddOrUpdate(3, "three");

            // Replace
            cache.AddOrUpdate(1, "oneone");

            Assert.True(cache.Count == 3, $"Expected 3 items in the cache, only had {cache.Count}");


            // Test retrieval
            Assert.True(cache.TryGetValue(1, out var value1), $"Expected to find the value, but did not");
            Assert.True(cache.TryGetValue(2, out var value2), $"Expected to find the value, but did not");
            Assert.True(cache.TryGetValue(3, out var value3), $"Expected to find the value, but did not");

            // Test Removal
            cache.Remove(1);
            Assert.True(cache.Count == 2, $"Expected 2 items in the cache, only had {cache.Count}");

        }

        [Fact]
        public void ThreadSafeSlidingCache_TestExpire()
        {
            var mockDateTime = new MockDateTimeProvider();

            // Arrange
            var cache = new ThreadSafeSlidingCache<int, string>(TimeSpan.FromMinutes(10),
                dateTimeProvider: mockDateTime);

            // Act
            cache.AddOrUpdate(1, "one");
            mockDateTime.UtcNow = mockDateTime.UtcNow.AddMinutes(11);
            if (cache.TryGetValue(1, out var value))
            {
                Assert.True(false, $"Expected to not find the value, but found {value}");
            }

        }

        [Fact]
        public async Task ThreadSafeSlidingCache_TestAutoExpire()
        {
            var mockDateTime = new MockDateTimeProvider();

            // Arrange
            var cache = new ThreadSafeSlidingCache<int, string>(TimeSpan.FromMinutes(10),
                dateTimeProvider: mockDateTime);

            // Act
            cache.AddOrUpdate(1, "one");

            // Ensure one item is in the cache
            Assert.True(cache.Count == 1, $"Expected 1 items in the cache, only had {cache.Count}");

            // move the time forward
            mockDateTime.UtcNow = mockDateTime.UtcNow.AddMinutes(11);
            
            // Trigger the cleanup, asking for non-existing key
            cache.TryGetValue(10, out var _);

            // Since the cache cleanup is triggered by a Task and not on the same thread, 
            // give it a moment for the cleanup to happen
            await Task.Delay(10);

            // Ensure one item is in the cache
            Assert.True(cache.Count == 0, $"Expected 0 items in the cache, only had {cache.Count}");

        }

        [Fact]
        public async Task ThreadSafeSlidingCache_TestNull()
        {
            // Arrange
            var cache = new ThreadSafeSlidingCache<Expression, string>(TimeSpan.FromMinutes(10));

            // Expect an ArgumentNullException
            var exception = Assert.Throws<ArgumentNullException>(() => {
                cache.AddOrUpdate(null, "one");
            });

        }

        [Fact]
        public async Task ThreadSafeSlidingCache_TestMinNumberBeforeTests()
        {
            // Arrange
            var mockDateTime = new MockDateTimeProvider();

            // Arrange
            var cache = new ThreadSafeSlidingCache<int, string>(
                TimeSpan.FromMinutes(10),
                minCacheItemsBeforeCleanup: 2,
                dateTimeProvider: mockDateTime);

            // Act
            cache.AddOrUpdate(1, "one");

            // Ensure one item is in the cache
            Assert.True(cache.Count == 1, $"Expected 1 items in the cache, only had {cache.Count}");

            // move the time forward
            mockDateTime.UtcNow = mockDateTime.UtcNow.AddMinutes(11);

            // Trigger the cleanup, asking for non-existing key
            cache.TryGetValue(10, out var _);

            // Since the cache cleanup is triggered by a Task and not on the same thread, 
            // give it a moment for the cleanup to happen
            await Task.Delay(10);

            // Ensure one item is in the cache
            Assert.True(cache.Count == 1, $"Expected 1 items in the cache, only had {cache.Count}");

            // Act
            cache.AddOrUpdate(2, "two");

            // Since the cache cleanup is triggered by a Task and not on the same thread, 
            // give it a moment for the cleanup to happen
            await Task.Delay(10);

            // Ensure one item is in the cache
            Assert.True(cache.Count == 1, $"Expected 1 items in the cache, had {cache.Count}");
        }

        private class MockDateTimeProvider : IDateTimeUtils
        {
            public DateTime UtcNow { get; set; } = DateTime.UtcNow;

        }
    }
}
