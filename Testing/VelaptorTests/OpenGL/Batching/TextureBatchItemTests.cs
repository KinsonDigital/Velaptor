// <copyright file="TextureBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System;
using System.Drawing;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Test the <see cref="TextureBatchItem"/> struct.
/// </summary>
public class TextureBatchItemTests
{
    #region Method Tests
    [Fact]
    public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue()
    {
        // Arrange
        var sut = default(TextureBatchItem);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        Assert.True(actual);
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

        var sut = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);

        // Act
        var actual = sut.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
}
