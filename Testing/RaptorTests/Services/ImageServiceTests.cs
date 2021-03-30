using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Raptor.Graphics;
using Raptor.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;
using NETColor = System.Drawing.Color;

#pragma warning disable SA1202 // Elements should be ordered by access
namespace RaptorTests.Services
{
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
        private readonly string testAssetPath;
        private readonly string testImagePath;
        private readonly string testResultDirPath;
        private Image<Rgba32> testCompareImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageServiceTests"/> class.
        /// </summary>
        public ImageServiceTests()
        {
            this.testAssetPath = $@"{this.basePath}{TestAssetDirName}\";
            this.testImagePath = $"{this.testAssetPath}{TestImageFileName}";
            this.testResultDirPath = $@"{this.basePath}{TestResultDirName}\";

            if (Directory.Exists(this.testResultDirPath))
            {
                Directory.Delete(this.testResultDirPath, true);
            }

            Directory.CreateDirectory(this.testResultDirPath);

            this.testCompareImage = Image.Load<Rgba32>(this.testImagePath);
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
            var imageData = service.Load(this.testImagePath);

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
        #endregion

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
        /// Creates a single row of pixels.
        /// </summary>
        /// <param name="color">The color of all the pixels.</param>
        /// <param name="rowWidth">The width of the rows.</param>
        /// <returns>The 2 dimensional array of pixels to test.</returns>
        [ExcludeFromCodeCoverage]
        private NETColor[] CreatePixelRow(NETColor color, int rowWidth)
        {
            var result = new NETColor[rowWidth];

            for (var x = 0; x < rowWidth; x++)
            {
                result[x] = color;
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
