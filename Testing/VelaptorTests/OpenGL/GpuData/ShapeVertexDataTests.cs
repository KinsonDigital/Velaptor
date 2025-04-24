// <copyright file="ShapeVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Drawing;
using System.Linq;
using System.Numerics;
using Shouldly;
using Velaptor.OpenGL.GpuData;
using Xunit;

/// <summary>
/// Tests the <see cref="ShapeVertexData"/> struct.
/// </summary>
public class ShapeVertexDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsProperties()
    {
        // Arrange & Act
        var data = new ShapeVertexData(
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
        data.IsSolid.ShouldBeTrue();
        data.VertexPos.ShouldBe(new Vector2(1, 2));
        data.BoundingBox.ShouldBe(new Vector4(3, 4, 5, 6));
        data.Color.ShouldBe(Color.FromArgb(7, 8, 9, 10));
        data.BorderThickness.ShouldBe(11);
        data.TopLeftCornerRadius.ShouldBe(12);
        data.BottomLeftCornerRadius.ShouldBe(13);
        data.BottomRightCornerRadius.ShouldBe(14);
        data.TopRightCornerRadius.ShouldBe(15);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Empty_WhenInvoked_ReturnsEmptyInstance()
    {
        // Arrange & Act
        var actual = ShapeVertexData.Empty();

        // Assert
        actual.IsSolid.ShouldBeFalse();
        actual.VertexPos.ShouldBe(Vector2.Zero);
        actual.BoundingBox.ShouldBe(Vector4.Zero);
        actual.Color.ShouldBe(Color.Empty);
        actual.BorderThickness.ShouldBe(0f);
        actual.TopLeftCornerRadius.ShouldBe(0f);
        actual.BottomLeftCornerRadius.ShouldBe(0f);
        actual.BottomRightCornerRadius.ShouldBe(0f);
        actual.TopRightCornerRadius.ShouldBe(0f);
    }

    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var actual = ShapeVertexData.GetStride();

        // Assert
        actual.ShouldBe(64u);
    }

    [Fact]
    public void ToArray_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var data = new ShapeVertexData(
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
        actual.Length.ShouldBe(16);
        actual[0].ShouldBe(1f, "the Vertex Pos X should be correct.");
        actual[1].ShouldBe(2f, "the Vertex Pos Y should be correct.");
        actual[2].ShouldBe(3f, "the Rectangle X should be correct.");
        actual[3].ShouldBe(4f, "the Rectangle Y should be correct.");
        actual[4].ShouldBe(5f, "the Rectangle Width should be correct.");
        actual[5].ShouldBe(6f, "the Rectangle Height should be correct.");
        actual[6].ShouldBe(8, "the Color R should be correct.");
        actual[7].ShouldBe(9, "the Color G should be correct.");
        actual[8].ShouldBe(10, "the Color B should be correct.");
        actual[9].ShouldBe(7, "the Color A should be correct.");
        actual[10].ShouldBe(1, "the IsSolid should be correct.");
        actual[11].ShouldBe(11f, "the Border Thickness should be correct.");
        actual[12].ShouldBe(12f, "the Top Left Radius should be correct.");
        actual[13].ShouldBe(13f, "the Bottom Left Radius should be correct.");
        actual[14].ShouldBe(14f, "the Bottom Right Radius should be correct.");
        actual[15].ShouldBe(15f, "the Top Right Radius should be correct.");
    }
    #endregion
}
