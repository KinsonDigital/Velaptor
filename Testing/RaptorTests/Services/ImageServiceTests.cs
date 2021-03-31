// <copyright file="ImageServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1202 // Elements should be ordered by access
namespace RaptorTests.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Raptor.Graphics;
    using Raptor.Services;
    using RaptorTests.Helpers;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using Xunit;
    using NETColor = System.Drawing.Color;
    using NETRectangle = System.Drawing.Rectangle;

    /// <summary>
    /// Tests the <see cref="ImageService"/>.
    /// </summary>
    public class ImageServiceTests : IDisposable
    {
        private const string TestAssetDirName = "TestAssets";
        private const string TestResultDirName = "ImageTestResults";
        private const string TestResultImageFileName = "SaveResultImage.png";
        private const string TestImageFileName = "TestCompareImage.png";
        private readonly string basePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly string testAssetDirPath;
        private readonly string testAssetFilePath;
        private readonly string testResultDirPath;
        private Image<Rgba32> testCompareImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageServiceTests"/> class.
        /// </summary>
        public ImageServiceTests()
        {
            this.testAssetDirPath = $@"{this.basePath}{TestAssetDirName}\";
            this.testAssetFilePath = $"{this.testAssetDirPath}{TestImageFileName}";
            this.testResultDirPath = $@"{this.basePath}{TestResultDirName}\";

            if (Directory.Exists(this.testResultDirPath))
            {
                Directory.Delete(this.testResultDirPath, true);
            }

            Directory.CreateDirectory(this.testResultDirPath);

            this.testCompareImage = Image.Load<Rgba32>(this.testAssetFilePath);
            this.testCompareImage.Mutate(context => context.Flip(FlipMode.Vertical));
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_ProperlyLoadsImage()
        {
            // Arrange
            var service = new ImageService();
            var expected = GetTestCompareImagePixels();

            // Act
            var imageData = service.Load(this.testAssetFilePath);

            // Assert
            Assert.Equal(expected, imageData.Pixels);
        }

        [Fact]
        public void Save_WhenInvoked_CorrectlySavesImage()
        {
            // Arrange
            var width = 8;
            var height = 8;
            var expectedPixelData = new NETColor[width, height];

            // Fill the pixel data with 3 rows of pixels
            // Row 0: RED
            // Row 1: GREEN
            // Row 2: BLUE
            for (var y = 0; y < height; y++)
            {
                NETColor pixelClr = default;

                for (var x = 0; x < width; x++)
                {
                    if (x >= 0 && x <= 3 && y >= 0 && y <= 3)
                    {
                        pixelClr = NETColor.FromArgb(255, 128, 128, 128); // Gray | Top Left Corner
                    }

                    if (x >= 4 && x <= 7 && y >= 0 && y <= 3)
                    {
                        pixelClr = NETColor.FromArgb(255, 255, 0, 0); // Red | Top Right Corner
                    }

                    if (x >= 0 && x <= 3 && y >= 4 && y <= 7)
                    {
                        pixelClr = NETColor.FromArgb(255, 0, 255, 0); // Green | Bottom Left Corner
                    }

                    if (x >= 4 && x <= 7 && y >= 4 && y <= 7)
                    {
                        pixelClr = NETColor.FromArgb(255, 0, 0, 255); // Blue | Bottom Right Corner
                    }

                    expectedPixelData[x, y] = pixelClr;
                }
            }

            var imageData = new ImageData()
            {
                Pixels = expectedPixelData,
                Width = width,
                Height = height,
            };

            var service = new ImageService();
            var saveResultImageFilePath = $"{this.testResultDirPath}{nameof(Save_WhenInvoked_CorrectlySavesImage)}.png";

            // Act
            service.Save(saveResultImageFilePath, imageData);
            var actualSavedPixelData = LoadSaveResultImage(saveResultImageFilePath);

            // Assert
            Assert.Equal(expectedPixelData, actualSavedPixelData);
        }

        [Fact]
        public void FlipVertically_WhenInvoked_FlipsImageVertically()
        {
            // Arrange
            var service = new ImageService();
            var testCompareImage = ToImageData(Image.Load<Rgba32>(this.testAssetFilePath));

            // Act
            var flippedImage = service.FlipVertically(testCompareImage);

            var flippedImageResultPath = $"{this.testResultDirPath}{nameof(FlipVertically_WhenInvoked_FlipsImageVertically)}.png";
            ToSixLaborImage(flippedImage).SaveAsPng(flippedImageResultPath);

            // Assert
            // Check that all pixels in the top left section are green
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(0, 0, 3, 3),
                NETColor.FromArgb(255, 0, 255, 0));

            // Check that all pixels in the top right section are blue
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(4, 0, 3, 3),
                NETColor.FromArgb(255, 0, 0, 255));

            // Check that all pixels in the bottom left section are gray
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(0, 4, 3, 3),
                NETColor.FromArgb(255, 128, 128, 128));

            // Check that all pixels in the bottom right section are red
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(4, 4, 3, 3),
                NETColor.FromArgb(255, 255, 0, 0));
        }

        [Fact]
        public void FlipHorizontally_WhenInvoked_FlipsImageVertically()
        {
            // Arrange
            var service = new ImageService();
            var testCompareImage = ToImageData(Image.Load<Rgba32>(this.testAssetFilePath));

            // Act
            var flippedImage = service.FlipHorizontally(testCompareImage);

            var flippedImageResultPath = $"{this.testResultDirPath}{nameof(FlipHorizontally_WhenInvoked_FlipsImageVertically)}.png";
            ToSixLaborImage(flippedImage).SaveAsPng(flippedImageResultPath);

            // Assert
            // Check that all pixels in the top left section are red
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(0, 0, 3, 3),
                NETColor.FromArgb(255, 255, 0, 0));

            // Check that all pixels in the top right section are gray
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(4, 0, 3, 3),
                NETColor.FromArgb(255, 128, 128, 128));

            // Check that all pixels in the bottom left section are blue
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(0, 4, 3, 3),
                NETColor.FromArgb(255, 0, 0, 255));

            // Check that all pixels in the bottom right section are green
            AssertThatPixelsMatch(
                flippedImage.Pixels,
                flippedImage.Width,
                flippedImage.Height,
                new NETRectangle(4, 4, 3, 3),
                NETColor.FromArgb(255, 0, 255, 0));
        }
        #endregion

        /// <summary>
        /// Asserts that all of the given <paramref name="pixels"/> with the dimensions given by
        /// <paramref name="width"/> and <paramref name="height"/> match the given <paramref name="clr"/>
        /// inside of the given <paramref name="assertRect"/>.
        /// </summary>
        /// <param name="pixels">The pixels to possibly assert.</param>
        /// <param name="width">The width of the 1st array dimension.</param>
        /// <param name="height">The height of the 2nd array dimension.</param>
        /// <param name="assertRect">The rectangle that might contain any pixels to assert against.</param>
        /// <param name="clr">The color that the pixels must be.</param>
        /// <remarks>
        ///     As long as the pixel is inside of the given <paramref name="assertRect"/>, the pixel
        ///     color will be asserted againt the given <paramref name="clr"/>.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        private void AssertThatPixelsMatch(NETColor[,] pixels, int width, int height, NETRectangle assertRect, NETColor clr)
        {
            AssertHelpers.All(pixels, width, height, (pixel, x, y) =>
            {
                if (assertRect.Contains(x, y))
                {
                    var message = $"The pixel at location '{x},{y}' is incorrect with the ARGB value of '{pixel}'.";
                    AssertHelpers.True(
                        condition: pixel.A == clr.A,
                        message: message,
                        expected: $"Alpha {clr.A}",
                        actual: $"Alpha {pixel.A}");

                    AssertHelpers.True(
                        condition: pixel.R == clr.R,
                        message: message,
                        expected: $"Red {clr.R}",
                        actual: $"Red {pixel.R}");

                    AssertHelpers.True(
                        condition: pixel.G == clr.G,
                        message: message,
                        expected: $"Green {clr.G}",
                        actual: $"Green {pixel.G}");

                    AssertHelpers.True(
                        condition: pixel.B == clr.B,
                        message: message,
                        expected: $"Blue {clr.B}",
                        actual: $"Red {pixel.B}");
                }
            });
        }

        /// <summary>
        /// Loads all of the pixel data into a 2 dimensional array of <see cref="NETColor"/>.
        /// </summary>
        /// <returns>The pixel data from the test comparison image.</returns>
        [ExcludeFromCodeCoverage]
        private NETColor[,] LoadSaveResultImage(string filePath)
        {
            if (File.Exists(filePath) is false)
            {
                Assert.True(false, $"The resulting image '{filePath}' from unit test '{nameof(Save_WhenInvoked_CorrectlySavesImage)}()' does not exist.");
            }

            var image = Image.Load<Rgba32>(filePath);

            var result = new NETColor[image.Width, image.Height];

            for (var y = 0; y < image.Height; y++)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);

                for (var x = 0; x < image.Width; x++)
                {
                    result[x, y] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            }

            return result;
        }

        /// <summary>
        /// Loads all of the pixel data into a 2 dimensional array of <see cref="NETColor"/>.
        /// </summary>
        /// <returns>The pixel data from the test comparison image.</returns>
        [ExcludeFromCodeCoverage]
        private NETColor[,] GetTestCompareImagePixels()
        {
            var result = new NETColor[this.testCompareImage.Width, this.testCompareImage.Height];

            for (var y = 0; y < this.testCompareImage.Height; y++)
            {
                var pixelRowSpan = this.testCompareImage.GetPixelRowSpan(y);

                for (var x = 0; x < this.testCompareImage.Width; x++)
                {
                    result[x, y] = NETColor.FromArgb(pixelRowSpan[x].A, pixelRowSpan[x].R, pixelRowSpan[x].G, pixelRowSpan[x].B);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given <paramref name="imageData"/> of type <see cref="ImageData"/>
        /// to the type of <see cref="Image{Rgba32}"/>.
        /// </summary>
        /// <param name="imageData">The image data to convert.</param>
        /// <returns>The image data of type <see cref="Image{Rgba32}"/>.</returns>
        [ExcludeFromCodeCoverage]
        public Image<Rgba32> ToSixLaborImage(ImageData imageData)
        {
            var result = new Image<Rgba32>(imageData.Width, imageData.Height);

            for (var y = 0; y < result.Height; y++)
            {
                var pixelRowSpan = result.GetPixelRowSpan(y);

                for (var x = 0; x < result.Width; x++)
                {
                    pixelRowSpan[x] = new Rgba32(
                        imageData.Pixels[x, y].R,
                        imageData.Pixels[x, y].G,
                        imageData.Pixels[x, y].B,
                        imageData.Pixels[x, y].A);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the given <paramref name="image"/> of type <see cref="Image{Rgba32}"/>
        /// to the type of <see cref="ImageData"/>.
        /// </summary>
        /// <param name="image">The image to convert.</param>
        /// <returns>The image data of type <see cref="ImageData"/>.</returns>
        [ExcludeFromCodeCoverage]
        public ImageData ToImageData(Image<Rgba32> image)
        {
            ImageData result = default;

            result.Pixels = new NETColor[image.Width, image.Height];
            result.Width = image.Width;
            result.Height = image.Height;

            for (var y = 0; y < image.Height; y++)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);

                for (var x = 0; x < image.Width; x++)
                {
                    result.Pixels[x, y] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.testCompareImage.Dispose();
            this.testCompareImage = null;
        }
    }
}
