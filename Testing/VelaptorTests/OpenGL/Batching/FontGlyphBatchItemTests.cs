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
    /// Gets all of the test data related to testing the end result
    /// values for the batch items to be rendered.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> GetBatchItemData()
    {
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            true, // Expected Equal Result
        };
        yield return new object[]
        {
            45, // Angle <-- THIS ONE IS DIFFERENT
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.FlipHorizontally, //Render Effects <-- THIS ONE IS DIFFERENT
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            20, // Size <-- THIS ONE IS DIFFERENT
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(33, 44, 55, 66), // Dest Rect <-- THIS ONE IS DIFFERENT
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(77, 88, 99, 100), // Src Rect <-- THIS ONE IS DIFFERENT
            11, // TextureId
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            111, // TextureId <-- THIS ONE IS DIFFERENT
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            false, // Expected Equal Result
        };
        yield return new object[]
        {
            1, // Angle
            RenderEffects.None, //Render Effects
            2, // Size
            new RectangleF(3, 4, 5, 6), // Dest Rect
            new RectangleF(7, 8, 9, 10), // Src Rect
            11, // TextureId
            Color.FromArgb(120, 130, 140, 150), // Tint Color <-- THIS ONE IS DIFFERENT
            false, // Expected Equal Result
        };
    }

    #region Method Tests
    [Fact]
    public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue()
    {
        // Arrange
        var item = default(FontGlyphBatchItem);

        // Act
        var actual = item.IsEmpty();

        // Assert
        Assert.True(actual);
    }

    [Theory]
    [MemberData(nameof(GetBatchItemData))]
    public void Equals_WhenUsingBatchItemParamOverload_ReturnsCorrectResult(
        float angle,
        RenderEffects effects,
        float size,
        RectangleF destRect,
        RectangleF srcRect,
        uint textureId,
        Color tintColor,
        bool expected)
    {
        // Arrange
        var batchItemA = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        var batchItemB = new FontGlyphBatchItem(
            srcRect,
            destRect,
            'g',
            size,
            angle,
            tintColor,
            effects,
            textureId,
            0);

        // Act
        var actual = batchItemA.Equals(batchItemB);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void EqualsOperator_WithEqualOperands_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        var batchItemB = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        // Act
        var actual = batchItemA == batchItemB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualsOperator_WithUnequalOperands_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        var batchItemB = new FontGlyphBatchItem(
            new RectangleF(77, 88, 99, 100),
            new RectangleF(33, 44, 55, 66),
            'g',
            22,
            11,
            Color.FromArgb(120, 130, 140, 150),
            RenderEffects.None,
            110,
            0);

        // Act
        var actual = batchItemA != batchItemB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOverloadWithMatchingType_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        object batchItemB = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        // Act
        var actual = batchItemA.Equals(batchItemB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOverloadWithDifferentType_ReturnsFalse()
    {
        // Arrange
        var batchItemA = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            0);

        var batchItemB = new object();

        // Act
        var actual = batchItemA.Equals(batchItemB);

        // Assert
        Assert.False(actual);
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
