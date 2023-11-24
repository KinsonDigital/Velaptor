// <copyright file="GlyphMetricsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="GlyphMetrics"/> struct.
/// </summary>
public class GlyphMetricsTests
{
    /// <summary>
    /// Gets all of the test data for testing equality of <see cref="GlyphMetrics"/> instances.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> GetMetricData()
    {
        yield return new object[] // Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            true, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            11, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 22, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'd', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            33u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(44, 55, 66, 77), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 88, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            99, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 100, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 110, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            120, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 130, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 140, /* Y Min */
            15, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            150, /* Horizontal Bearing X */ 16, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
        yield return new object[] // Not Equal Result
        {
            1, /* Ascender */ 2, /* Descender */ 'c', /* Glyph */
            3u, /* Char Index */ new RectangleF(4, 5, 6, 7), /* Glyph Bounds */ 8, /* Glyph Height */
            9, /* Glyph Width */ 10, /* Horizontal Advance */ 11, /* X Max */
            12, /* X Min */ 13, /* Y Max */ 14, /* Y Min */
            15, /* Horizontal Bearing X */ 160, /* Horizontal Bearing Y */
            false, /* Expected Result */
        };
    }

    #region Method Tests
    [Fact]
    public void ToString_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var metrics = new GlyphMetrics
        {
            GlyphBounds = new RectangleF(11, 22, 33, 44),
            Glyph = 'Z'
        };

        // Act
        var actual = metrics.ToString();

        // Assert
        actual.Should().Be("Name: Z | Bounds: {X=11,Y=22,Width=33,Height=44}");
    }

    [Theory]
    [MemberData(nameof(GetMetricData))]
    public void Equal_WithSameParamType_ReturnsCorrectResult(
        float ascender,
        float descender,
        char glyph,
        uint charIndex,
        RectangleF glyphBounds,
        float glyphHeight,
        float glyphWidth,
        float horizontalAdvance,
        float xMax,
        float xMin,
        float yMax,
        float yMin,
        float horiBearingX,
        float horiBearingY,
        bool expected)
    {
        // Arrange
        var metricsA = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        var metricsB = new GlyphMetrics
        {
            Ascender = ascender,
            Descender = descender,
            Glyph = glyph,
            CharIndex = charIndex,
            GlyphBounds = glyphBounds,
            GlyphHeight = glyphHeight,
            GlyphWidth = glyphWidth,
            HorizontalAdvance = horizontalAdvance,
            XMax = xMax,
            XMin = xMin,
            YMax = yMax,
            YMin = yMin,
            HoriBearingX = horiBearingX,
            HoriBearingY = horiBearingY,
        };
        // Act
        var actual = metricsA.Equals(metricsB);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Equal_WithObjectParamNotOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        var metricsB = new object();

        // Act
        var actual = metricsA.Equals(metricsB);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equal_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        object metricsB = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        // Act
        var actual = metricsA.Equals(metricsB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        var metricsB = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        // Act
        var actual = metricsA == metricsB;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = new GlyphMetrics
        {
            Ascender = 1,
            Descender = 2,
            Glyph = 'c',
            CharIndex = 3,
            GlyphBounds = new RectangleF(4, 5, 6, 7),
            GlyphHeight = 8,
            GlyphWidth = 9,
            HorizontalAdvance = 10,
            XMax = 11,
            XMin = 12,
            YMax = 13,
            YMin = 14,
            HoriBearingX = 15,
            HoriBearingY = 16,
        };

        var metricsB = default(GlyphMetrics);

        // Act
        var actual = metricsA != metricsB;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion
}
