// <copyright file="TextureBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Test the <see cref="TextureBatchItem"/> struct.
/// </summary>
public class TextureBatchItemTests
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            18, // Layer
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
            Color.FromArgb(12, 13, 14, 15), // Tint Color
            180, // Layer <-- THIS ONE IS DIFFERENT
            false, // Expected Equal Result
        };
    }

    #region Method Tests
    [Fact]
    public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue()
    {
        // Arrange
        var item = default(TextureBatchItem);

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
        int layer,
        bool expected)
    {
        // Arrange
        var batchItemA = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        var batchItemB = new TextureBatchItem(
            srcRect,
            destRect,
            size,
            angle,
            tintColor,
            effects,
            textureId,
            layer);

        // Act
        var actual = batchItemA.Equals(batchItemB);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void EqualsOperator_WithEqualOperands_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        var batchItemB = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        // Act
        var actual = batchItemA == batchItemB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualsOperator_WithUnequalOperands_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        var batchItemB = new TextureBatchItem(
            new RectangleF(77, 88, 99, 100),
            new RectangleF(33, 44, 55, 66),
            22,
            11,
            Color.FromArgb(120, 130, 140, 150),
            RenderEffects.None,
            110,
            180);

        // Act
        var actual = batchItemA != batchItemB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOverloadWithMatchingType_ReturnsTrue()
    {
        // Arrange
        var batchItemA = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            12);

        object batchItemB = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            12);

        // Act
        var actual = batchItemA.Equals(batchItemB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenUsingObjectParamOverloadWithDifferentType_ReturnsFalse()
    {
        // Arrange
        var batchItemA = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            12);

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
        var expected = "Texture Batch Item Values:";
        expected += $"{Environment.NewLine}Src Rect: {{X=7,Y=8,Width=9,Height=10}}";
        expected += $"{Environment.NewLine}Dest Rect: {{X=3,Y=4,Width=5,Height=6}}";
        expected += $"{Environment.NewLine}Size: 2";
        expected += $"{Environment.NewLine}Angle: 1";
        expected += $"{Environment.NewLine}Tint Clr: {{A=12,R=13,G=14,B=15}}";
        expected += $"{Environment.NewLine}Effects: None";
        expected += $"{Environment.NewLine}Texture ID: 11";
        expected += $"{Environment.NewLine}Layer: 18";

        var batchItem = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
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
