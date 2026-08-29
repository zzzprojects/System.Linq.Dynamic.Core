using System.Linq.Dynamic.Core.Tests.Helpers.Entities;

#if EFCORE
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    /// <summary>
    /// Test for https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/524
    /// </summary>
    [Fact]
    public void Entities_Where_In_And()
    {
        // Arrange
        var expected = _context.Blogs.Include(b => b.Posts).Where(b => new[] { 1000, 1001, 1002 }.Contains(b.BlogId) && new[] { "Blog1", "Blog2" }.Contains(b.Name)).ToArray();

        // Act
        var test = _context.Blogs.Include(b => b.Posts).Where(@"BlogId in (1000, 1001, 1002) and Name in (""Blog1"", ""Blog2"")").ToArray();

        // Assert
        Assert.Equal(expected, test);
    }

    [Fact]
    public void Entities_Where_In_DifferentTypes()
    {
        // Arrange
        var expected = _context.Blogs.Include(b => b.Posts).Where(b => new long[] { 1000, 1001, 1002 }.Contains(b.BlogLongId)).ToArray();

        // Act
        var test = _context.Blogs.Include(b => b.Posts).Where(@"BlogLongId in (1000, 1001, 1002)").ToArray();

        // Assert
        Assert.Equal(expected, test);
    }

    [Fact]
    public void Entities_Where_In_Issue987()
    {
        // Arrange
        for (int i = 0; i < 10000; i++)
        {
            var blogText = new BlogText
            {
                Id = i
            };
            _context.BlogTexts.Add(blogText);   
        }
        _context.SaveChanges();

        // Act
        var test = _context.BlogTexts
            .Where("Id in (9495, 9496, 9498, 9500, 9501, 9503, 9505, 9508, 9509, 9510, 9511, 9514, 9515, 9517, 9518, 9519, 9520, 9521, 9523, 9524, 9525, 9526, 9527, 9528, 9529, 9530, 9531, 9532, 9533, 9534, 9535, 9536, 9538, 9539, 9540, 9541, 9542, 9543, 9544, 9545, 9546, 9547, 9548, 9549, 9550, 9552, 9554, 9556, 9557, 9558, 9559, 9560, 9561, 9562, 9563, 9565, 9567, 9569, 9570, 9575, 9576, 9577, 9578, 9579, 9580, 9581, 9582, 9583, 9584, 9585, 9586, 9587, 9588, 9589, 9590, 9591, 9592, 9593, 9594, 9595, 9596, 9597, 9598, 9599, 9600, 9601, 9602, 9603, 9604, 9605, 9606, 9607, 9608, 9609, 9610, 9611, 9612, 9613, 9614, 9615, 9616, 9617, 9618, 9619, 9620, 9621, 9622, 9623, 9624, 9625, 9626, 9627, 9628, 9629)")
            .ToList();

        // Assert
        Assert.Equal(114, test.Count);
    }
}