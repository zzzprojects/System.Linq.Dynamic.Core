using System.Linq.Dynamic.Core.Exceptions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

/// <summary>
/// Tests for Issue #973: Multiple unhandled exceptions from malformed expression strings (5 crash sites found via fuzzing).
/// All 5 bugs should throw <see cref="ParseException"/> instead of unhandled runtime exceptions.
/// </summary>
public partial class QueryableTests
{

    /// <summary>
    /// Bug 1: ParseStringLiteral IndexOutOfRangeException for empty single-quoted string literal ''.
    /// </summary>
    [Fact]
    public void Issue973_Bug1_EmptyCharLiteral_ShouldThrowParseException()
    {
        // Arrange
        var items = new[] { new { Id = 1, Name = "Alice" } }.AsQueryable();

        // Act
        Action act = () => items.Where("''");

        // Assert
        act.Should().Throw<ParseException>();
    }

    /// <summary>
    /// Bug 2: TryGenerateAndAlsoNotNullExpression IndexOutOfRangeException for malformed np() call.
    /// </summary>
    [Fact]
    public void Issue973_Bug2_MalformedNpCall_ShouldThrowParseException()
    {
        // Arrange
        var items = new[] { new { Id = 1, Name = "Alice" } }.AsQueryable();

        // Act
        Action act = () => items.Where("-np(--9999999)9999T--99999999999)99999");

        // Assert
        act.Should().Throw<ParseException>();
    }

    /// <summary>
    /// Bug 3: Compiled expression IndexOutOfRangeException for string indexer with out-of-bounds constant index.
    /// </summary>
    [Fact]
    public void Issue973_Bug3_StringIndexerOutOfBounds_ShouldThrowParseException()
    {
        // Arrange
        var items = new[] { new { Id = 1, Name = "Alice" } }.AsQueryable();

        // Act
        Action act = () => items.OrderBy("\"ab\"[3]").ToList();

        // Assert
        act.Should().Throw<ParseException>();
    }

    /// <summary>
    /// Bug 3 (valid case): String indexer with in-bounds index should work correctly.
    /// </summary>
    [Fact]
    public void Issue973_Bug3_StringIndexerInBounds_ShouldWork()
    {
        // Arrange
        var items = new[] { new { Id = 1, Name = "Alice" } }.AsQueryable();

        // Act - "ab"[0] = 'a', "ab"[1] = 'b' are valid
        var result0 = items.OrderBy("\"ab\"[0]").ToList();
        var result1 = items.OrderBy("\"ab\"[1]").ToList();

        // Assert
        result0.Should().HaveCount(1);
        result1.Should().HaveCount(1);
    }

    /// <summary>
    /// Bug 4: NullReferenceException in compiled Select projection with mismatched brace/paren in new().
    /// </summary>
    [Fact]
    public void Issue973_Bug4_MismatchedBraceInNewExpression_ShouldThrowParseException()
    {
        // Arrange
        var items = new[] { new { Id = 1, Name = "Alice", Age = 30, Score = 85.5, Active = true } }.AsQueryable();

        // Act
        Action act = () => items.Select("new(Name }. AgAkQ & 1111555555+55555-5555555+555555+55555-55555").ToDynamicList();

        // Assert
        act.Should().Throw<ParseException>();
    }

    /// <summary>
    /// Bug 5: MethodAccessException from $ identifier in GroupBy due to duplicate property names.
    /// </summary>
    [Fact]
    public void Issue973_Bug5_DollarIdentifierWithDuplicatePropertyNames_ShouldThrowParseException()
    {
        // Arrange
        var items = new[]
        {
            new { Id = 1, Name = "Alice", Age = 30, Score = 85.5, Active = true },
            new { Id = 2, Name = "Bob", Age = 25, Score = 92.0, Active = false },
        }.AsQueryable();

        // Act
        Action act = () => items.GroupBy("new($ as DoubleAge, Age + 2 as DoubleAge)").ToDynamicList();

        // Assert - duplicate property name 'DoubleAge' should cause a ParseException
        act.Should().Throw<ParseException>();
    }
}
