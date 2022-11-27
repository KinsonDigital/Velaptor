// <copyright file="TextureVertexDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.GPUData;

using System.Drawing;
using System.Numerics;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureVertexData"/> struct.
/// </summary>
public class TextureVertexDataTests
{
    #region Overloaded Operator Tests
    [Fact]
    public void NotEqualsOperator_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var dataA = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));
        var dataB = new TextureVertexData(
            new Vector2(11, 22),
            new Vector2(44, 55),
            Color.FromArgb(66, 77, 88, 99));

        // Act
        var actual = dataA != dataB;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void EqualsOperator_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var dataA = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));
        var dataB = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));

        // Act
        var actual = dataA == dataB;

        // Assert
        Assert.True(actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void TotalElements_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureVertexData.TotalElements();

        // Assert
        Assert.Equal(8u, actual);
    }

    [Fact]
    public void Stride_WhenInvoked_ReturnsCorrectResult()
    {
        // Act
        var actual = TextureVertexData.Stride();

        // Assert
        Assert.Equal(32u, actual);
    }

    [Theory]
    [ClassData(typeof(VertexDataTestData))]
    public void Equals_WhenInvoked_ReturnsCorrectResult(Vector2 vertex, Vector2 textureCoord, Color tintClr, bool expected)
    {
        // Arrange
        var dataA = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));

        var dataB = new TextureVertexData(
            vertex,
            textureCoord,
            tintClr);

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Equals_WhenUsingOverloadWithObjectParamWithObjectOfDifferentType_ReturnsFalse()
    {
        // Arrange
        var dataA = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));
        var dataB = new object();

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenUsingOverloadWithObjectParamWithObjectOfSameType_ReturnsTrue()
    {
        // Arrange
        var dataA = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));
        object dataB = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(4, 5),
            Color.FromArgb(6, 7, 8, 9));

        // Act
        var actual = dataA.Equals(dataB);

        // Assert
        Assert.True(actual);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0, 0, true)]
    [InlineData(1, 1, 0, 0, 0, 0, 0, 0, false)]
    [InlineData(0, 0, 1, 1, 0, 0, 0, 0, false)]
    [InlineData(0, 0, 0, 0, 1, 1, 1, 1, false)]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        float vertexPosX,
        float vertexPosY,
        float textureCoordX,
        float textureCoordY,
        byte clrA,
        byte clrR,
        byte clrG,
        byte clrB,
        bool expected)
    {
        // Arrange
        var clr = clrA == 0 && clrR == 0 && clrG == 0 && clrB == 0
            ? Color.Empty
            : Color.FromArgb(clrA, clrR, clrG, clrB);
        var data = new TextureVertexData(
            new Vector2(vertexPosX, vertexPosY),
            new Vector2(textureCoordX, textureCoordY),
            clr);

        // Act
        var actual = data.IsEmpty();

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
}
