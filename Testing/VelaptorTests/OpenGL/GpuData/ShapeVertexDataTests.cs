// <copyright file="ShapeVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Drawing;
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
    #endregion
}
