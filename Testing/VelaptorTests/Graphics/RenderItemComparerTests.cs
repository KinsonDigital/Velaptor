// <copyright file="RenderItemComparerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
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
    public void Compare_WhenItemsAreOnDifferentLayers_ReturnsCorrectResult(
        int layerA,
        int layerB,
        int expected)
    {
        // Arrange
        var itemA = new RenderItem<string>
        {
            Layer = layerA,
            Item = "itemA",
            RenderStamp = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 1)),
        };
        var itemB = new RenderItem<string>
        {
            Layer = layerB,
            Item = "itemB",
            RenderStamp = DateTime.Now,
        };

        var sut = new RenderItemComparer<string>();

        // Act
        var actual = sut.Compare(itemA, itemB);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 4, -1)]
    [InlineData(4, 2, 1)]
    [InlineData(6, 6, 0)]
    public void Compare_WhenItemsAreOnTheSameLayer_ReturnsCorrectResult(
        int timeA,
        int timeB,
        int expected)
    {
        // Arrange
        var itemA = new RenderItem<string>
        {
            Layer = 1,
            Item = "itemA",
            RenderStamp = DateTime.Today.Add(new TimeSpan(0, 0, 0, timeA)),
        };
        var itemB = new RenderItem<string>
        {
            Layer = 1,
            Item = "itemB",
            RenderStamp = DateTime.Today.Add(new TimeSpan(0, 0, 0, timeB)),
        };

        var sut = new RenderItemComparer<string>();

        // Act
        var actual = sut.Compare(itemA, itemB);

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}
