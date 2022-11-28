// <copyright file="RectVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor.OpenGL.GPUData;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="RectVertexData"/> struct.
/// </summary>
public class RectVertexDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperties()
    {
        // Arrange & Act
        var data = new RectVertexData(
            new Vector2(1, 2),
            new Vector4(3, 4, 5, 6),
            Color.FromArgb(7, 8, 9, 10),
            true,
            11,
            12,
            13,
            14,
            15);

        // Assert
        Assert.Equal(new Vector2(1, 2), data.VertexPos);
        Assert.Equal(new Vector4(3, 4, 5, 6), data.Rectangle);
        Assert.Equal(Color.FromArgb(7, 8, 9, 10), data.Color);
        Assert.True(data.IsFilled);
        Assert.Equal(11, data.BorderThickness);
        Assert.Equal(12, data.TopLeftCornerRadius);
        Assert.Equal(13, data.BottomLeftCornerRadius);
        Assert.Equal(14, data.BottomRightCornerRadius);
        Assert.Equal(15, data.TopRightCornerRadius);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Empty_WhenInvoked_ReturnsEmptyInstance()
    {
        // Arrange & Act
        var actual = RectVertexData.Empty();

        // Assert
        Assert.Equal(Vector2.Zero, actual.VertexPos);
        Assert.Equal(Vector4.Zero, actual.Rectangle);
        Assert.Equal(Color.Empty, actual.Color);
        Assert.False(actual.IsFilled);
        Assert.Equal(0f, actual.BorderThickness);
        Assert.Equal(0f, actual.TopLeftCornerRadius);
        Assert.Equal(0f, actual.BottomLeftCornerRadius);
        Assert.Equal(0f, actual.BottomRightCornerRadius);
        Assert.Equal(0f, actual.TopRightCornerRadius);
    }

    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = RectVertexData.GetTotalBytes();

        // Assert
        Assert.Equal(64u, actual);
    }

    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var data = new RectVertexData(
            new Vector2(1f, 2f),
            new Vector4(3f, 4f, 5f, 6f),
            Color.FromArgb(7, 8, 9, 10),
            true,
            11f,
            12f,
            13f,
            14f,
            15);

        // Act
        var actual = data.ToArray().ToArray();

        // Assert
        Assert.Equal(16, actual.Length);
        AssertExtensions.EqualWithMessage(1f, actual[0], "Vertex Pos X");
        AssertExtensions.EqualWithMessage(2f, actual[1], "Vertex Pos Y");
        AssertExtensions.EqualWithMessage(3f, actual[2], "Rectangle X");
        AssertExtensions.EqualWithMessage(4f, actual[3], "Rectangle Y");
        AssertExtensions.EqualWithMessage(5f, actual[4], "Rectangle Width");
        AssertExtensions.EqualWithMessage(6f, actual[5], "Rectangle Height");
        AssertExtensions.EqualWithMessage(8, actual[6], "Color R");
        AssertExtensions.EqualWithMessage(9, actual[7], "Color G");
        AssertExtensions.EqualWithMessage(10, actual[8], "Color B");
        AssertExtensions.EqualWithMessage(7, actual[9], "Color A");
        AssertExtensions.EqualWithMessage(1, actual[10], "IsFilled");
        AssertExtensions.EqualWithMessage(11f, actual[11], "Border Thickness");
        AssertExtensions.EqualWithMessage(12f, actual[12], "Top Left Radius");
        AssertExtensions.EqualWithMessage(13f, actual[13], "Bottom Left Radius");
        AssertExtensions.EqualWithMessage(14f, actual[14], "Bottom Right Radius");
        AssertExtensions.EqualWithMessage(15f, actual[15], "Top Right Radius");
    }
    #endregion
}
