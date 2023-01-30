// <copyright file="RenderItemComparerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Tests the <see cref="RenderItemComparer{T}"/> class.
/// </summary>
public class RenderItemComparerTests
{
    #region Method Tests
    [Theory]
    [InlineData(2, 4, -1)]
    [InlineData(4, 2, 1)]
    [InlineData(6, 6, 0)]
    public void Compare_WhenInvoked_ReturnsCorrectResult(
        int layerOne,
        int layerTwo,
        int expected)
    {
        // Arrange
        var itemA = new RenderItem<string> { Layer = layerOne, Item = "itemA", };
        var itemB = new RenderItem<string> { Layer = layerTwo, Item = "itemB", };

        var sut = new RenderItemComparer<string>();

        // Act
        var actual = sut.Compare(itemA, itemB);

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}
