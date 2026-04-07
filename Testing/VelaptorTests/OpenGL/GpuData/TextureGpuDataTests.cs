// <copyright file="TextureGpuDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GpuData;

using System.Drawing;
using System.Numerics;
using Shouldly;
using Velaptor.OpenGL.GpuData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureGpuData"/> struct.
/// </summary>
public class TextureGpuDataTests
{
    #region Overloaded Operator Tests
    [Fact]
    public void EqualsOperator_WithBothOperandsEqual_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        var sutB = default(TextureGpuData);

        // Act
        var actual = sutA == sutB;

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNotEqual_ReturnsFalse()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        var sutB = new TextureGpuData(
            new TextureVertexData(
                new Vector2(11, 22),
                default,
                default),
            default,
            default,
            default);

        // Act
        var actual = sutA != sutB;

        // Assert
        actual.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetTotalBytes_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureGpuData.GetTotalBytes();

        // Assert
        actual.ShouldBe(128u);
    }

    [Fact]
    public void ToArray_WithTextureQuadDataOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[]
        {
            // ReSharper disable MultipleSpaces
            1f,  2f,  3f,  4f,  6f,  7f,  8f,  5f,  // Vertex 1
            9f, 10f, 11f, 12f, 14f, 15f, 16f, 13f,  // Vertex 2
            17f, 18f, 19f, 20f, 22f, 23f, 24f, 21f, // Vertex 3
            25f, 26f, 27f, 28f, 30f, 31f, 32f, 29f, // Vertex 4
            // ReSharper restore MultipleSpaces
        };
        var vertex1 = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8));

        var vertex2 = new TextureVertexData(
            new Vector2(9, 10),
            new Vector2(11, 12),
            Color.FromArgb(13, 14, 15, 16));

        var vertex3 = new TextureVertexData(
            new Vector2(17, 18),
            new Vector2(19, 20),
            Color.FromArgb(21, 22, 23, 24));

        var vertex4 = new TextureVertexData(
            new Vector2(25, 26),
            new Vector2(27, 28),
            Color.FromArgb(29, 30, 31, 32));

        var quadData = new TextureGpuData(
            vertex1,
            vertex2,
            vertex3,
            vertex4);

        // Act
        var actual = quadData.ToArray();

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void Equals_WithEqualParam_ReturnsTrue()
    {
        // Arrange
        var sut = default(TextureGpuData);
        var dataB = default(TextureGpuData);

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void Equals_WhenInvokedWithParamOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var sut = default(TextureGpuData);
        var dataB = new object();

        // Act
        var actual = sut.Equals(dataB);

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_WhenInvokedWithEqualParamOfSameType_ReturnsTrue()
    {
        // Arrange
        var sutA = default(TextureGpuData);
        object sutB = default(TextureGpuData);

        // Act
        var actual = sutA.Equals(sutB);

        // Assert
        actual.ShouldBeTrue();
    }
    #endregion
}
