// <copyright file="OpenGLExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System.Drawing;
    using System.Numerics;
    using Velaptor.OpenGL;
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
            var data = default(TextureVertexData);
            data.VertexPos = new Vector2(1, 2);
            data.TextureCoord = new Vector2(3, 4);
            data.TintColor = Color.FromArgb(5, 6, 7, 8);

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
            var vertex1 = default(TextureVertexData);
            vertex1.VertexPos = new Vector2(1, 2);
            vertex1.TextureCoord = new Vector2(3, 4);
            vertex1.TintColor = Color.FromArgb(5, 6, 7, 8);

            var vertex2 = default(TextureVertexData);
            vertex2.VertexPos = new Vector2(9, 10);
            vertex2.TextureCoord = new Vector2(11, 12);
            vertex2.TintColor = Color.FromArgb(13, 14, 15, 16);

            var vertex3 = default(TextureVertexData);
            vertex3.VertexPos = new Vector2(17, 18);
            vertex3.TextureCoord = new Vector2(19, 20);
            vertex3.TintColor = Color.FromArgb(21, 22, 23, 24);

            var vertex4 = default(TextureVertexData);
            vertex4.VertexPos = new Vector2(25, 26);
            vertex4.TextureCoord = new Vector2(27, 28);
            vertex4.TintColor = Color.FromArgb(29, 30, 31, 32);

            var quadData = default(TextureQuadData);
            quadData.Vertex1 = vertex1;
            quadData.Vertex2 = vertex2;
            quadData.Vertex3 = vertex3;
            quadData.Vertex4 = vertex4;

            // Act
            var actual = quadData.ToArray();

            // Assert
            Assert.Equal(32, actual.Length);
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
