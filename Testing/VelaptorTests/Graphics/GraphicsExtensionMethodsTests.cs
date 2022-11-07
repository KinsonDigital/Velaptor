// <copyright file="GraphicsExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Velaptor.Graphics;
using Xunit;

namespace VelaptorTests.Graphics;

/// <summary>
/// Tests the <see cref="GraphicsExtensionMethods"/> class.
/// </summary>
public class GraphicsExtensionMethodsTests
{
    [Fact]
    public void MaxHeight_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var metric1 = default(GlyphMetrics);
        metric1.GlyphHeight = 10;

        var metric2 = default(GlyphMetrics);
        metric2.GlyphHeight = 20;

        var metrics = new List<GlyphMetrics[]> { new[] { metric1, metric2 } };

        // Act
        var actual = metrics.MaxHeight(0);

        // Assert
        Assert.Equal(20, actual);
    }

    [Theory]
    [InlineData(200f, 10f, 190f)]
    [InlineData(200f, 210f, 70f)]
    public void MaxVerticalOffset_WhenInvoked_ReturnsCorrectResult(float glyphHeight, float horiBearingY, float expected)
    {
        // Arrange
        var metric1 = default(GlyphMetrics);
        metric1.GlyphHeight = 100;
        metric1.HoriBearingY = 30;

        var metric2 = default(GlyphMetrics);
        metric2.GlyphHeight = glyphHeight;
        metric2.HoriBearingY = horiBearingY;

        var metrics = new List<GlyphMetrics[]> { new[] { metric1, metric2 } };

        // Act
        var actual = metrics.MaxVerticalOffset(0);

        // Assert
        Assert.Equal(expected, actual);
    }
}
