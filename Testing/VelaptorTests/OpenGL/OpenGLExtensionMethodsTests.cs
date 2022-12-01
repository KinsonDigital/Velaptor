// <copyright file="OpenGLExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Velaptor.OpenGL;
using Velaptor.OpenGL.GPUData;
using Xunit;

/// <summary>
/// Tests the helper methods in the <see cref="OpenGLExtensionMethods"/> class.
/// </summary>
public class OpenGLExtensionMethodsTests
{
    #region Method Tests
    [Fact]
    public void ToNDC_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var vector = new Vector2(75, 150);

        // Act
        var actual = vector.ToNDC(100, 200);

        // Assert
        Assert.Equal(0.5f, actual.X);
        Assert.Equal(-0.5f, actual.Y);
    }

    [Fact]
    public void ToNDCTextureCoordX_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float value = 75f;

        // Act
        var actual = value.ToNDCTextureCoordX(100);

        // Assert
        Assert.Equal(0.75f, actual);
    }

    [Fact]
    public void ToNDCTextureCoordY_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float value = 75f;

        // Act
        var actual = value.ToNDCTextureCoordY(100);

        // Assert
        Assert.Equal(0.25f, actual);
    }

    [Fact]
    public void ToNDCTextureCoords_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var coord = new Vector2(75f, 75f);

        // Act
        var actual = coord.ToNDCTextureCoords(100, 100);

        // Assert
        Assert.Equal(0.75f, actual.X);
        Assert.Equal(0.25f, actual.Y);
    }

    [Fact]
    public void ToArray_WithVector2Overload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 10f, 20f };
        var vector = new Vector2(10, 20);

        // Act
        var actual = vector.ToArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithColorOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 20f, 30f, 40f, 10f };
        var color = Color.FromArgb(10, 20, 30, 40);

        // Act
        var actual = color.ToArray();

        // Assert
        Assert.Equal(4, actual.Length);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithTextureVertexDataOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 1f, 2f, 3f, 4f, 6f, 7f, 8f, 5f };
        var data = new TextureVertexData(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8));

        // Act
        var actual = data.ToArray();

        // Assert
        Assert.Equal(8, actual.Length);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithTextureQuadDataOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[]
        {
            1f,  2f,  3f,  4f,  6f,  7f,  8f,  5f,  // Vertex 1
            9f, 10f, 11f, 12f, 14f, 15f, 16f, 13f,  // Vertex 2
            17f, 18f, 19f, 20f, 22f, 23f, 24f, 21f, // Vertex 3
            25f, 26f, 27f, 28f, 30f, 31f, 32f, 29f, // Vertex 4
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

        var quadData = new TextureGPUData(
            vertex1,
            vertex2,
            vertex3,
            vertex4);

        // Act
        var actual = quadData.ToArray();

        // Assert
        Assert.Equal(32, actual.Length);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithVector2ParamOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 11f, 22f };
        var vector = new Vector2(11, 22);

        // Act
        var actual = vector.ToArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithColorParamOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 22f, 33f, 44f, 11f };
        var clr = Color.FromArgb(11, 22, 33, 44);

        // Act
        var actual = clr.ToArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithTextureVertexDataParamOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { 11f, 22f, 33f, 44f, 66, 77, 88, 55 };
        var vertexData = new TextureVertexData(
            new Vector2(11, 22),
            new Vector2(33, 44),
            Color.FromArgb(55, 66, 77, 88));

        // Act
        var actual = vertexData.ToArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithTextureQuadDataParamOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[]
        {
            1f,   2f,  3f,  4f,  6f,  7f,  8f,  5f, // Vertex 1
            9f,  10f, 11f, 12f, 14f, 15f, 16f, 13f, // Vertex 2
            17f, 18f, 19f, 20f, 22f, 23f, 24f, 21f, // Vertex 3
            25f, 26f, 27f, 28f, 30f, 31f, 32f, 29f, // Vertex 4
        };

        var quad = CreateNewQuad(1);

        // Act
        var actual = quad.ToArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithTextureQuadDataListParamOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[]
        {
            1f,   2f,  3f,  4f,  6f,  7f,  8f,  5f, // Quad 1 Vertex 1
            9f,  10f, 11f, 12f, 14f, 15f, 16f, 13f, // Quad 1 Vertex 2
            17f, 18f, 19f, 20f, 22f, 23f, 24f, 21f, // Quad 1 Vertex 3
            25f, 26f, 27f, 28f, 30f, 31f, 32f, 29f, // Quad 1 Vertex 4
            33f, 34f, 35f, 36f, 38f, 39f, 40f, 37f, // Quad 2 Vertex 1
            41f, 42f, 43f, 44f, 46f, 47f, 48f, 45f, // Quad 2 Vertex 2
            49f, 50f, 51f, 52f, 54f, 55f, 56f, 53f, // Quad 2 Vertex 3
            57f, 58f, 59f, 60f, 62f, 63f, 64f, 61f, // Quad 2 Vertex 4
        };

        var quads = new List<TextureGPUData> { CreateNewQuad(1), CreateNewQuad(33) };

        // Act
        var actual = OpenGLExtensionMethods.ToArray(quads);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToArray_WithVector4Param_ReturnsCorrectResult()
    {
        // Arrange
        var vector = new Vector4(11, 22, 33, 44);

        // Act
        var actual = vector.ToArray();

        // Assert
        Assert.Equal(4f, actual.Length);
        Assert.Equal(11f, actual[0]);
        Assert.Equal(22f, actual[1]);
        Assert.Equal(33f, actual[2]);
        Assert.Equal(44f, actual[3]);
    }
    #endregion

    /// <summary>
    /// Creates a quad with vertex values that are in sequence using the given <paramref name="start"/> value.
    /// </summary>
    /// <param name="start">The starting value to base the values from.</param>
    /// <returns>The texture quad data to test.</returns>
    private static TextureGPUData CreateNewQuad(int start)
    {
        var result = new TextureGPUData(
            new TextureVertexData(
                new Vector2(start, start + 1),
                new Vector2(start + 2, start + 3),
                Color.FromArgb(start + 4, start + 5, start + 6, start + 7)),
            new TextureVertexData(
                new Vector2(start + 8, start + 9),
                new Vector2(start + 10, start + 11),
                Color.FromArgb(start + 12, start + 13, start + 14, start + 15)),
            new TextureVertexData(
                new Vector2(start + 16, start + 17),
                new Vector2(start + 18, start + 19),
                Color.FromArgb(start + 20, start + 21, start + 22, start + 23)),
            new TextureVertexData(
                new Vector2(start + 24, start + 25),
                new Vector2(start + 26, start + 27),
                Color.FromArgb(start + 28, start + 29, start + 30, start + 31)));

        return result;
    }
}
