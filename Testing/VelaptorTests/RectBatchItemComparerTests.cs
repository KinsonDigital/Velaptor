// <copyright file="RectBatchItemComparerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using FluentAssertions;
using Velaptor;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="RectBatchItemComparer"/> class.
/// </summary>
public class RectBatchItemComparerTests
{
    #region Method Tests
    [Theory]
    [InlineData(10, 20, -1)]
    [InlineData(30, 30, 0)]
    [InlineData(50, 40, 1)]
    public void Compare_WhenInvoked_ReturnsCorrectResult(int layerA, int layerB, int result)
    {
        // Arrange
        var itemA = new RectBatchItem { Layer = layerA };
        var itemB = new RectBatchItem { Layer = layerB };

        var sut = new RectBatchItemComparer();

        // Act
        var actual = sut.Compare(itemA, itemB);

        // Assert
        actual.Should().Be(result);
    }
    #endregion
}
