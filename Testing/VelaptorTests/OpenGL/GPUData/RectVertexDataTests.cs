// <copyright file="RectVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Velaptor.OpenGL.GPUData;
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
        data.IsSolid.Should().BeTrue();
        data.VertexPos.Should().Be(new Vector2(1, 2));
        data.Rectangle.Should().Be(new Vector4(3, 4, 5, 6));
        data.Color.Should().Be(Color.FromArgb(7, 8, 9, 10));
        data.BorderThickness.Should().Be(11);
        data.TopLeftCornerRadius.Should().Be(12);
        data.BottomLeftCornerRadius.Should().Be(13);
        data.BottomRightCornerRadius.Should().Be(14);
        data.TopRightCornerRadius.Should().Be(15);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Empty_WhenInvoked_ReturnsEmptyInstance()
    {
        // Arrange & Act
        var actual = RectVertexData.Empty();

        // Assert
        actual.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(Vector2.Zero);
        actual.Rectangle.Should().Be(Vector4.Zero);
        actual.Color.Should().Be(Color.Empty);
        actual.BorderThickness.Should().Be(0f);
        actual.TopLeftCornerRadius.Should().Be(0f);
        actual.BottomLeftCornerRadius.Should().Be(0f);
        actual.BottomRightCornerRadius.Should().Be(0f);
        actual.TopRightCornerRadius.Should().Be(0f);
    }

    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = RectVertexData.GetStride();

        // Assert
        actual.Should().Be(64u);
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
        actual.Length.Should().Be(16);
        actual[0].Should().Be(1f, "the Vertex Pos X should be correct.");
        actual[1].Should().Be(2f, "the Vertex Pos Y should be correct.");
        actual[2].Should().Be(3f, "the Rectangle X should be correct.");
        actual[3].Should().Be(4f, "the Rectangle Y should be correct.");
        actual[4].Should().Be(5f, "the Rectangle Width should be correct.");
        actual[5].Should().Be(6f, "the Rectangle Height should be correct.");
        actual[6].Should().Be(8, "the Color R should be correct.");
        actual[7].Should().Be(9, "the Color G should be correct.");
        actual[8].Should().Be(10, "the Color B should be correct.");
        actual[9].Should().Be(7, "the Color A should be correct.");
        actual[10].Should().Be(1, "the IsSolid should be correct.");
        actual[11].Should().Be(11f, "the Border Thickness should be correct.");
        actual[12].Should().Be(12f, "the Top Left Radius should be correct.");
        actual[13].Should().Be(13f, "the Bottom Left Radius should be correct.");
        actual[14].Should().Be(14f, "the Bottom Right Radius should be correct.");
        actual[15].Should().Be(15f, "the Top Right Radius should be correct.");
    }
    #endregion
}
