// <copyright file="FontGlyphBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System;
using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Test the <see cref="FontGlyphBatchItem"/> struct.
/// </summary>
public class FontGlyphBatchItemTests
{
    /// <summary>
    /// Gets all of the test data related to testing the <see cref="IsEmptyData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyData()
    {
        yield return new object[]
        {
            // FULLY EMPTY
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            true, // Expected Result
        };
        yield return new object[]
        {
            new RectangleF(10, 20, 30, 40), // Dest Rect - NON-EMPTY VALUE
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            new RectangleF(10, 20, 30, 40), // Src Rect - NON-EMPTY VALUE
            'V', // Glyph - NON-EMPTY VALUE
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            'V', // Glyph - NON-EMPTY VALUE
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            10, // Size - NON-EMPTY VALUE
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            10, // Angle - NON-EMPTY VALUE
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.FromArgb(10, 20, 30, 40), // Tint Color - NON-EMPTY VALUE
            RenderEffects.None, //Render Effects
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.FlipHorizontally, //Render Effects - NON-EMPTY VALUE
            0, // TextureId
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            10, // TextureId - NON-EMPTY VALUE
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            10, // Layer - NON-EMPTY VALUE
            false, // Expected Result
        };
    }

    #region Method Tests
    [Theory]
    [MemberData(nameof(IsEmptyData))]
    public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue(
        RectangleF srcRect,
        RectangleF destRect,
        char glyph,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        uint textureId,
        int layer,
        bool expected)
    {
        // Arrange
        var sut = new FontGlyphBatchItem(
            srcRect,
            destRect,
            glyph,
            size,
            angle,
            tintColor,
            effects,
            textureId,
            layer);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToString_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = "Font Batch Item Values:";
        expected += $"{Environment.NewLine}Src Rect: {{X=7,Y=8,Width=9,Height=10}}";
        expected += $"{Environment.NewLine}Dest Rect: {{X=3,Y=4,Width=5,Height=6}}";
        expected += $"{Environment.NewLine}Size: 2";
        expected += $"{Environment.NewLine}Angle: 1";
        expected += $"{Environment.NewLine}Tint Clr: {{A=12,R=13,G=14,B=15}}";
        expected += $"{Environment.NewLine}Effects: None";
        expected += $"{Environment.NewLine}Texture ID: 11";
        expected += $"{Environment.NewLine}Glyph: V";
        expected += $"{Environment.NewLine}Layer: 18";

        var batchItem = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'V',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        // Act
        var actual = batchItem.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
}
