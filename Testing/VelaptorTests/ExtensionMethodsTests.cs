// <copyright file="ExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using OpenTK.Mathematics;
    using Velaptor;
    using Velaptor.Graphics;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Xunit;
    using NETColor = System.Drawing.Color;

    /// <summary>
    /// Tests the <see cref="ExtensionMethods"/> class.
    /// </summary>
    public class ExtensionMethodsTests
    {
        #region Method Tests
        [Fact]
        public void ForcePositive_WhenUsingNegativeValue_ReturnsPositiveResult()
        {
            // Act & Assert
            Assert.Equal(123f, (-123f).ForcePositive());
        }

        [Fact]
        public void ForcePositive_WhenUsingPositiveValue_ReturnsPositiveResult()
        {
            // Act & Assert
            Assert.Equal(123f, 123f.ForcePositive());
        }

        [Fact]
        public void ForceNegative_WhenUsingPositiveValue_ReturnsNegativeResult()
        {
            // Act & Assert
            Assert.Equal(-123f, 123f.ForceNegative());
        }

        [Fact]
        public void ForceNegative_WhenUsingNegativeValue_ReturnsNegativeResult()
        {
            // Act & Assert
            Assert.Equal(-123f, (-123f).ForceNegative());
        }

        [Fact]
        public void ToRadians_WhenInvoking_ReturnsCorrectResult()
        {
            // Act & Assert
            Assert.Equal(70710.06f, 1234.1234f.ToDegrees());
        }

        [Fact]
        public void RotateAround_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            var angle = 13f;
            var expected = new Vector2(1.25290489f, -0.996605873f);

            // Act
            var actual = vectorToRotate.RotateAround(origin, angle);

            // Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }

        [Fact]
        public void RotateAround_WhenInvokedWithClockwiseFalse_ReturnsCorrectResult()
        {
            // Arrange
            var vectorToRotate = new Vector2(0, 0);
            var origin = new Vector2(5, 5);
            var angle = 45f;
            var expected = new Vector2(-2.07106781f, 5f);

            // Act
            var actual = vectorToRotate.RotateAround(origin, angle, false);

            // Assert
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
        }

        [Fact]
        public void ToVector4_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var color = NETColor.FromArgb(11, 22, 33, 44);
            var expected = new Vector4(22, 33, 44, 11);

            // Act
            var actual = color.ToVector4();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MapValue_WhenUsingFloatType_ReturnsCorrectResult()
        {
            // Arrange
            var testValue = 5f;

            // Act
            var actual = testValue.MapValue(0f, 10f, 0f, 21f);

            // Assert
            Assert.Equal(10.5f, actual);
        }

        [Fact]
        public void MapValue_WhenUsingByteValues_ReturnsCorrectResult()
        {
            // Arrange
            byte testValue = 5;

            // Act
            var actual = testValue.MapValue(0, 10, 0, 20);

            // Assert
            Assert.Equal(10, actual);
        }

        [Fact]
        public void MapValue_WhenInvokedWithIntegerType_ReturnsCorrectResult()
        {
            // Arrange
            var testValue = 500;

            // Act
            var actual = testValue.MapValue(0, 1_000, 0, 100_000);

            // Assert
            Assert.Equal(50_000, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(@"C:\", true)]
        [InlineData(@"C:", false)]
        [InlineData(@"C\", false)]
        [InlineData(@"C:\test-file.txt", true)]
        public void IsDirectoryRootDrive_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
        {
            // Act
            var actual = value.IsDirectoryRootDrive();

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
        #endregion

        /// <summary>
        /// Creates a Six Labors image type of <see cref="Image{Rgba32}"/> with the given <paramref name="width"/>
        /// and <paramref name="height"/> with each row having its own colors described by the given
        /// <paramref name="rowColors"/> dictionary.
        /// </summary>
        /// <param name="width">The width of the iamge.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="rowColors">The color for each row.</param>
        /// <returns>An image with the given row colors.</returns>
        /// <remarks>
        ///     The <paramref name="rowColors"/> dictionary key is the zero based row index and the
        ///     value is the color to make the entire row.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        private Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
        {
            if (height != rowColors.Count)
            {
                Assert.True(false, $"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
            }

            var availableRows = rowColors.Keys.ToArray();

            foreach (var row in availableRows)
            {
                if (row < 0 && row > height - 1)
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

        private NETColor[,] CreateImageDataPixels(int width, int height, Dictionary<uint, NETColor> rowColors)
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
        private Rgba32[,] GetSixLaborPixels(Image<Rgba32> sixLaborsImage)
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
