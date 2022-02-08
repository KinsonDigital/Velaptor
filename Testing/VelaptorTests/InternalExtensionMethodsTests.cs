// <copyright file="InternalExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Numerics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Velaptor;
    using Velaptor.Graphics;
    using Velaptor.OpenGL.GPUData;
    using Xunit;
    using NETColor = System.Drawing.Color;
    using NETRectF = System.Drawing.RectangleF;
    using NETSizeF = System.Drawing.SizeF;

    /// <summary>
    /// Tests the <see cref="Velaptor.InternalExtensionMethods"/> class.
    /// </summary>
    public class InternalExtensionMethodsTests
    {
        #region Method Tests
        [Theory]
        [InlineData('x', true)]
        [InlineData('k', false)]
        public void DoesNotStartWidth_WhenCheckingForCharacters_ReturnsCorrectResult(char character, bool expected)
        {
            // Arrange
            const string stringToCheck = "kinson";

            // Act
            var actual = stringToCheck.DoesNotStartWith(character);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("digital", true)]
        [InlineData("kinson", false)]
        public void DoesNotStartWith_WhenCheckingForStrings_ReturnsCorrectResult(string stringValue, bool expected)
        {
            // Arrange
            const string stringToCheck = "kinson digital";

            // Act
            var actual = stringToCheck.DoesNotStartWith(stringValue);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData('x', true)]
        [InlineData('n', false)]
        public void DoesNotEndWith_WhenCheckingForCharacters_ReturnsCorrectResult(char character, bool expected)
        {
            // Arrange
            const string stringToCheck = "kinson";

            // Act
            var actual = stringToCheck.DoesNotEndWith(character);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("kinson", true)]
        [InlineData("digital", false)]
        public void DoesNotEndWith_WhenCheckingForStrings_ReturnsCorrectResult(string stringValue, bool expected)
        {
            // Arrange
            const string stringToCheck = "kinson digital";

            // Act
            var actual = stringToCheck.DoesNotEndWith(stringValue);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(@"C:\", true)]
        [InlineData(@"C:", false)]
        [InlineData(@"C\", false)]
        [InlineData(@"C:\test-file.txt", false)]
        public void OnlyContainsDrive_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
        {
            // Act
            var actual = value.OnlyContainsDrive();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(".txt", "")]
        [InlineData("test-dir", "test-dir")]
        [InlineData(@"C:\", @"C:\")]
        [InlineData(@"C:\temp", @"temp")]
        [InlineData(@"C:\temp\", @"temp")]
        [InlineData(@"C:\test-file.txt", @"C:\")]
        [InlineData(@"C:\temp\test-file.txt", @"temp")]
        public void GetLastDirName_WhenInvoked_ReturnsCorrectResult(string value, string expected)
        {
            // Act
            var actual = value.GetLastDirName();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToSixLaborImage_WhenInvoked_CorrectlyConvertsToSixLaborImage()
        {
            // Arrange
            var imageData = new ImageData(new NETColor[2, 3], 2, 3);

            var expectedPixels = new Rgba32[2, 3];

            // Act
            var sixLaborsImage = imageData.ToSixLaborImage();
            var actualPixels = GetSixLaborPixels(sixLaborsImage);

            // Assert
            Assert.Equal(expectedPixels, actualPixels);
        }

        [Fact]
        public void ToImageData_WhenInvoked_CorrectlyConvertsToSixLaborImage()
        {
            // Arrange
            var rowColors = new Dictionary<uint, NETColor>()
            {
                { 0, NETColor.Red },
                { 1, NETColor.Green },
                { 2, NETColor.Blue },
            };

            var sixLaborsImage = CreateSixLaborsImage(2, 3, rowColors);
            var expectedPixels = CreateImageDataPixels(2, 3, rowColors);

            // Act
            var actual = sixLaborsImage.ToImageData();

            // Assert
            Assert.Equal(expectedPixels, actual.Pixels);
        }

        [Fact]
        public void ToVertexArray_WithVector2ParamOverload_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { 11f, 22f };
            var vector = new Vector2(11, 22);

            // Act
            var actual = vector.ToVertexArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToVertexArray_WithColorParamOverload_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { 22f, 33f, 44f, 11f };
            var clr = NETColor.FromArgb(11, 22, 33, 44);

            // Act
            var actual = clr.ToVertexArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToVertexArray_WithTextureVertexDataParamOverload_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new[] { 11f, 22f, 33f, 44f, 66, 77, 88, 55 };
            var vertexData = default(TextureVertexData);
            vertexData.VertexPos = new Vector2(11, 22);
            vertexData.TextureCoord = new Vector2(33, 44);
            vertexData.TintColor = NETColor.FromArgb(55, 66, 77, 88);

            // Act
            var actual = vertexData.ToVertexArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToVertexArray_WithTextureQuadDataParamOverload_ReturnsCorrectResult()
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
            var actual = quad.ToVertexArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToVertexArray_WithTextureQuadDataListParamOverload_ReturnsCorrectResult()
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

            var quads = new List<TextureQuadData> { CreateNewQuad(1), CreateNewQuad(33) };

            // Act
            var actual = quads.ToVertexArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPosition_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var rect = new NETRectF(11f, 22f, 33f, 44f);

            // Act
            var actual = rect.GetPosition();

            // Assert
            Assert.Equal(11f, actual.X);
            Assert.Equal(22f, actual.Y);
        }

        [Fact]
        public void TrimAllEnds_WhenUsingDefaultParamValue_TrimsEndsOfAllStrings()
        {
            // Arrange
            var values = new[] { "item ", "item " };

            // Act
            var actual = values.TrimAllEnds();

            // Assert
            Assert.All(actual, i => Assert.Equal("item", i));
        }

        [Fact]
        public void TrimAllEnds_WhenTrimmingSpecificCharacter_TrimsEndsOfAllStrings()
        {
            // Arrange
            var values = new[] { "item~", "item~" };

            // Act
            var actual = values.TrimAllEnds('~');

            // Assert
            Assert.All(actual, i => Assert.Equal("item", i));
        }

        [Fact]
        public void ToArray_WithVector4Param_ReturnsCorrectResult()
        {
            // Arrange
            var vector = new Vector4(11, 22, 33, 44);

            // Act
            var actual = vector.ToArray();

            // Assert
            Assert.Equal(4, actual.Length);
            Assert.Equal(11, actual[0]);
            Assert.Equal(22, actual[1]);
            Assert.Equal(33, actual[2]);
            Assert.Equal(44, actual[3]);
        }
        #endregion

        /// <summary>
        /// Creates a quad with vertex values that are in sequence using the given <paramref name="start"/> value.
        /// </summary>
        /// <param name="start">The starting value to base the values from.</param>
        /// <returns>The texture quad data to test.</returns>
        private static TextureQuadData CreateNewQuad(int start)
        {
            var result = default(TextureQuadData);

            result.Vertex1 = new TextureVertexData()
            {
                VertexPos = new Vector2(start, start + 1),
                TextureCoord = new Vector2(start + 2, start + 3),
                TintColor = NETColor.FromArgb(start + 4, start + 5, start + 6, start + 7),
            };

            result.Vertex2 = new TextureVertexData()
            {
                VertexPos = new Vector2(start + 8, start + 9),
                TextureCoord = new Vector2(start + 10, start + 11),
                TintColor = NETColor.FromArgb(start + 12, start + 13, start + 14, start + 15),
            };

            result.Vertex3 = new TextureVertexData()
            {
                VertexPos = new Vector2(start + 16, start + 17),
                TextureCoord = new Vector2(start + 18, start + 19),
                TintColor = NETColor.FromArgb(start + 20, start + 21, start + 22, start + 23),
            };

            result.Vertex4 = new TextureVertexData()
            {
                VertexPos = new Vector2(start + 24, start + 25),
                TextureCoord = new Vector2(start + 26, start + 27),
                TintColor = NETColor.FromArgb(start + 28, start + 29, start + 30, start + 31),
            };

            return result;
        }

        /// <summary>
        /// Creates a Six Labors image type of <see cref="Image{Rgba32}"/> with the given <paramref name="width"/>
        /// and <paramref name="height"/> with each row having its own colors described by the given
        /// <paramref name="rowColors"/> dictionary.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="rowColors">The color for each row.</param>
        /// <returns>An image with the given row colors.</returns>
        /// <remarks>
        ///     The <paramref name="rowColors"/> dictionary key is the zero based row index and the
        ///     value is the color to make the entire row.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        private static Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
        {
            if (height != rowColors.Count)
            {
                Assert.True(false, $"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
            }

            var availableRows = rowColors.Keys.ToArray();

            foreach (var row in availableRows)
            {
                if (row > height - 1)
                {
                    Assert.True(false, $"The row '{row}' is not within the range of rows for the image height '{height}' for the definition of row colors.");
                }
            }

            var result = new Image<Rgba32>(width, height);

            for (var y = 0; y < height; y++)
            {
                var rowSpan = result.GetPixelRowSpan(y);

                for (var x = 0; x < width; x++)
                {
                    rowSpan[x] = new Rgba32(rowColors[(uint)y].R, rowColors[(uint)y].G, rowColors[(uint)y].B, rowColors[(uint)y].A);
                }
            }

            return result;
        }

        private static NETColor[,] CreateImageDataPixels(int width, int height, Dictionary<uint, NETColor> rowColors)
        {
            var result = new NETColor[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    result[x, y] = NETColor.FromArgb(rowColors[(uint)y].A, rowColors[(uint)y].R, rowColors[(uint)y].G, rowColors[(uint)y].B);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the <see cref="Rgba32"/> pixels from the given <paramref name="sixLaborsImage"/>.
        /// </summary>
        /// <param name="sixLaborsImage">The six labors image.</param>
        /// <returns>The 2 dimensional pixel colors of the image.</returns>
        private static Rgba32[,] GetSixLaborPixels(Image<Rgba32> sixLaborsImage)
        {
            var result = new Rgba32[sixLaborsImage.Width, sixLaborsImage.Height];

            for (var y = 0; y < sixLaborsImage.Height; y++)
            {
                var pixelRow = sixLaborsImage.GetPixelRowSpan(y);

                for (var x = 0; x < sixLaborsImage.Width; x++)
                {
                    result[x, y] = pixelRow[x];
                }
            }

            return result;
        }
    }
}
