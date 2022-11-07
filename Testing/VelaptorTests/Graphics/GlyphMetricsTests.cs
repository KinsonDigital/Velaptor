// <copyright file="GlyphMetricsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Drawing;
using Velaptor.Graphics;
using Xunit;

namespace VelaptorTests.Graphics;

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
        var metrics = default(GlyphMetrics);
        metrics.GlyphBounds = new RectangleF(11, 22, 33, 44);
        metrics.Glyph = 'Z';

        // Act
        var actual = metrics.ToString();

        // Assert
        Assert.Equal("Name: Z | Bounds: {X=11,Y=22,Width=33,Height=44}", actual);
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
        var metricsA = default(GlyphMetrics);
        metricsA.Ascender = 1;
        metricsA.Descender = 2;
        metricsA.Glyph = 'c';
        metricsA.CharIndex = 3;
        metricsA.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsA.GlyphHeight = 8;
        metricsA.GlyphWidth = 9;
        metricsA.HorizontalAdvance = 10;
        metricsA.XMax = 11;
        metricsA.XMin = 12;
        metricsA.YMax = 13;
        metricsA.YMin = 14;
        metricsA.HoriBearingX = 15;
        metricsA.HoriBearingY = 16;

        var metricsB = default(GlyphMetrics);
        metricsB.Ascender = ascender;
        metricsB.Descender = descender;
        metricsB.Glyph = glyph;
        metricsB.CharIndex = charIndex;
        metricsB.GlyphBounds = glyphBounds;
        metricsB.GlyphHeight = glyphHeight;
        metricsB.GlyphWidth = glyphWidth;
        metricsB.HorizontalAdvance = horizontalAdvance;
        metricsB.XMax = xMax;
        metricsB.XMin = xMin;
        metricsB.YMax = yMax;
        metricsB.YMin = yMin;
        metricsB.HoriBearingX = horiBearingX;
        metricsB.HoriBearingY = horiBearingY;

        // Act
        var actual = metricsA.Equals(metricsB);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Equal_WithObjectParamNotOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = default(GlyphMetrics);
        metricsA.Ascender = 1;
        metricsA.Descender = 2;
        metricsA.Glyph = 'c';
        metricsA.CharIndex = 3;
        metricsA.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsA.GlyphHeight = 8;
        metricsA.GlyphWidth = 9;
        metricsA.HorizontalAdvance = 10;
        metricsA.XMax = 11;
        metricsA.XMin = 12;
        metricsA.YMax = 13;
        metricsA.YMin = 14;
        metricsA.HoriBearingX = 15;
        metricsA.HoriBearingY = 16;

        var metricsB = new object();

        // Act
        var actual = metricsA.Equals(metricsB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equal_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = default(GlyphMetrics);
        metricsA.Ascender = 1;
        metricsA.Descender = 2;
        metricsA.Glyph = 'c';
        metricsA.CharIndex = 3;
        metricsA.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsA.GlyphHeight = 8;
        metricsA.GlyphWidth = 9;
        metricsA.HorizontalAdvance = 10;
        metricsA.XMax = 11;
        metricsA.XMin = 12;
        metricsA.YMax = 13;
        metricsA.YMin = 14;
        metricsA.HoriBearingX = 15;
        metricsA.HoriBearingY = 16;

        object metricsB = new GlyphMetrics()
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
        Assert.True(actual);
    }

    [Fact]
    public void EqualsOperator_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = default(GlyphMetrics);
        metricsA.Ascender = 1;
        metricsA.Descender = 2;
        metricsA.Glyph = 'c';
        metricsA.CharIndex = 3;
        metricsA.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsA.GlyphHeight = 8;
        metricsA.GlyphWidth = 9;
        metricsA.HorizontalAdvance = 10;
        metricsA.XMax = 11;
        metricsA.XMin = 12;
        metricsA.YMax = 13;
        metricsA.YMin = 14;
        metricsA.HoriBearingX = 15;
        metricsA.HoriBearingY = 16;

        var metricsB = default(GlyphMetrics);
        metricsB.Ascender = 1;
        metricsB.Descender = 2;
        metricsB.Glyph = 'c';
        metricsB.CharIndex = 3;
        metricsB.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsB.GlyphHeight = 8;
        metricsB.GlyphWidth = 9;
        metricsB.HorizontalAdvance = 10;
        metricsB.XMax = 11;
        metricsB.XMin = 12;
        metricsB.YMax = 13;
        metricsB.YMin = 14;
        metricsB.HoriBearingX = 15;
        metricsB.HoriBearingY = 16;

        // Act
        var actual = metricsA == metricsB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualsOperator_WithObjectParamOfSameType_ReturnsCorrectResult()
    {
        // Arrange
        var metricsA = default(GlyphMetrics);
        metricsA.Ascender = 1;
        metricsA.Descender = 2;
        metricsA.Glyph = 'c';
        metricsA.CharIndex = 3;
        metricsA.GlyphBounds = new RectangleF(4, 5, 6, 7);
        metricsA.GlyphHeight = 8;
        metricsA.GlyphWidth = 9;
        metricsA.HorizontalAdvance = 10;
        metricsA.XMax = 11;
        metricsA.XMin = 12;
        metricsA.YMax = 13;
        metricsA.YMin = 14;
        metricsA.HoriBearingX = 15;
        metricsA.HoriBearingY = 16;

        var metricsB = default(GlyphMetrics);

        // Act
        var actual = metricsA != metricsB;

        // Assert
        Assert.True(actual);
    }
    #endregion
}
