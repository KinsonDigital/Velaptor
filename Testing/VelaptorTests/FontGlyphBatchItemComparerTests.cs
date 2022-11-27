﻿// <copyright file="FontGlyphBatchItemComparerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Velaptor;
using Velaptor.OpenGL;
using Xunit;

namespace VelaptorTests;

/// <summary>
/// Tests the <see cref="FontGlyphBatchItemComparer"/> class.
/// </summary>
public class FontGlyphBatchItemComparerTests
{
    #region Method Tests
    [Theory]
    [InlineData(10, 20, -1)]
    [InlineData(30, 30, 0)]
    [InlineData(50, 40, 1)]
    public void Compare_WhenInvoked_ReturnsCorrectResult(int layerA, int layerB, int result)
    {
        // Arrange
        var itemA = new FontGlyphBatchItem { Layer = layerA };
        var itemB = new FontGlyphBatchItem { Layer = layerB };

        var sut = new FontGlyphBatchItemComparer();

        // Act
        var actual = sut.Compare(itemA, itemB);

        // Assert
        actual.Should().Be(result);
    }
    #endregion
}