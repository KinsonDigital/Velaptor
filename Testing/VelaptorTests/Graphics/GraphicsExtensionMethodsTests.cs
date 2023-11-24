// <copyright file="GraphicsExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Collections.Generic;
using FluentAssertions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="GraphicsExtensionMethods"/> class.
/// </summary>
public class GraphicsExtensionMethodsTests
{
    [Fact]
    public void MaxHeight_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var metric1 = default(GlyphMetrics) with { GlyphHeight = 10 };

        var metric2 = default(GlyphMetrics) with { GlyphHeight = 20 };

        var metrics = new List<GlyphMetrics[]> { new[] { metric1, metric2 } };

        // Act
        var actual = metrics.MaxHeight(0);

        // Assert
        actual.Should().Be(20);
    }

    [Theory]
    [InlineData(200f, 10f, 190f)]
    [InlineData(200f, 210f, 70f)]
    public void MaxVerticalOffset_WhenInvoked_ReturnsCorrectResult(float glyphHeight, float horiBearingY, float expected)
    {
        // Arrange
        var metric1 = default(GlyphMetrics) with { GlyphHeight = 100, HoriBearingY = 30, };

        var metric2 = default(GlyphMetrics) with { GlyphHeight = glyphHeight, HoriBearingY = horiBearingY, };

        var metrics = new List<GlyphMetrics[]> { new[] { metric1, metric2 } };

        // Act
        var actual = metrics.MaxVerticalOffset(0);

        // Assert
        actual.Should().Be(expected);
    }
}
