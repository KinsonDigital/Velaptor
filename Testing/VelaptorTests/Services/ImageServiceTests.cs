// <copyright file="ImageServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using FluentAssertions;
using Helpers;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Velaptor.ExtensionMethods;
using Velaptor.Graphics;
using Velaptor.Services;
using Xunit;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectangle = System.Drawing.Rectangle;

/// <summary>
/// Tests the <see cref="ImageService"/>.
/// </summary>
public class ImageServiceTests : IDisposable
{
    private const string TestAssetDirName = "TestAssets";
    private const string TestImageFileName = "TestCompareImage.png";
    private readonly string basePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .ToCrossPlatPath();
    private readonly string testAssetFilePath;
    private readonly Mock<IFile> mockFile;
    private Image<Rgba32> testCompareImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageServiceTests"/> class.
    /// </summary>
    public ImageServiceTests()
    {
        this.testAssetFilePath = $@"{this.basePath}/{TestAssetDirName}/{TestImageFileName}";

        TestHelpers.SetupTestResultDirPath();

        this.testCompareImage = Image.Load<Rgba32>(this.testAssetFilePath);
        this.testCompareImage.Mutate(context => context.Flip(FlipMode.Vertical));
        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(this.testAssetFilePath)).Returns(true);
    }

    #region Constructor Tests

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Load_WithNullParam_ThrowsException()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        var act = () => service.Load(null);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage("The image file was not found.");
    }

    [Fact]
    public void Load_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        var act = () => service.Load(string.Empty);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage("The image file was not found.");
    }

    [Fact]
    public void Load_WhenInvoked_ProperlyLoadsImage()
    {
        // Arrange
        var service = CreateSystemUnderTest();
        var expected = TestHelpers.ToPixelColors(this.testCompareImage);

        // Act
        var imageData = service.Load(this.testAssetFilePath);

        // Assert
        imageData.Pixels.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Save_WhenInvoked_CorrectlySavesImage()
    {
        // Arrange
        const uint width = 8u;
        const uint height = 8u;
        var expectedPixelData = new NETColor[width, height];

        // Fill the pixel data with 3 rows of pixels
        for (var y = 0; y < height; y++)
        {
            NETColor pixelClr = default;

            for (var x = 0; x < width; x++)
            {
                pixelClr = x switch
                {
                    >= 0 and <= 3 when y is >= 0 and <= 3 => NETColor.FromArgb(255, 128, 128, 128),
                    _ => pixelClr
                };

                switch (x)
                {
                    case >= 0 and <= 3 when y is >= 4 and <= 7:
                        pixelClr = NETColor.FromArgb(255, 0, 255, 0); // Green | Bottom Left Corner
                        break;
                }

                expectedPixelData[x, y] = pixelClr;
            }
        }

        var imageData = new ImageData(expectedPixelData, width, height);

        var service = CreateSystemUnderTest();
        var saveResultImageFilePath = $"{TestHelpers.GetTestResultDirPath()}{nameof(Save_WhenInvoked_CorrectlySavesImage)}.png";

        // Act
        service.Save(saveResultImageFilePath, imageData);
        var actualSavedPixelData = LoadSaveResultImage(saveResultImageFilePath);

        // Assert
        actualSavedPixelData.Should().BeEquivalentTo(expectedPixelData);
    }

    [Fact]
    public void FlipVertically_WhenInvoked_FlipsImageVertically()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        var comparisonSample = Image.Load<Rgba32>(this.testAssetFilePath).ToImageData();

        // Act
        var flippedImage = service.FlipVertically(comparisonSample);

        TestHelpers.SaveImageForTest(flippedImage);

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
        var service = CreateSystemUnderTest();
        var comparisonSample = Image.Load<Rgba32>(this.testAssetFilePath).ToImageData();

        // Act
        var flippedImage = service.FlipHorizontally(comparisonSample);

        TestHelpers.SaveImageForTest(flippedImage);

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

    [Fact]
    public void Draw_WhenInvoked_CorrectlyDrawsSrcImageOntoDestination()
    {
        /*NOTE:
         * The location of '3,3' where the srcImage is drawn onto the destImage
         * is purposely not centered on the destImage so that way any horizontal
         * or vertically flipping of the drawn image can be detected.
         */

        // Arrange
        var srcImage = TestHelpers.CreateImageData(NETColor.FromArgb(255, 0, 255, 0), 4, 4);
        TestHelpers.SaveImageForTest(srcImage, $"{nameof(Draw_WhenInvoked_CorrectlyDrawsSrcImageOntoDestination)}-SrcImage");

        var destImage = TestHelpers.CreateImageData(NETColor.FromArgb(255, 255, 255, 255), 8, 8);
        TestHelpers.SaveImageForTest(destImage, $"{nameof(Draw_WhenInvoked_CorrectlyDrawsSrcImageOntoDestination)}-DestImage");

        var service = CreateSystemUnderTest();

        // Act
        var actual = service.Draw(srcImage, destImage, new NETPoint(3, 3));
        TestHelpers.SaveImageForTest(actual, $"{nameof(Draw_WhenInvoked_CorrectlyDrawsSrcImageOntoDestination)}-ResultImage");

        // Assert
        AssertThatPixelsMatch(
            actual.Pixels,
            actual.Width,
            actual.Height,
            new NETRectangle(3, 3, 4, 4),
            NETColor.FromArgb(255, 0, 255, 0));
    }
    #endregion

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        this.testCompareImage.Dispose();
        this.testCompareImage = null;
    }

    /// <summary>
    /// Asserts that all of the given <paramref name="pixels"/> with the dimensions given by
    /// <paramref name="width"/> and <paramref name="height"/> match the given <paramref name="expectedClr"/>
    /// inside of the given <paramref name="assertRect"/>.
    /// </summary>
    /// <param name="pixels">The pixels to possibly assert.</param>
    /// <param name="width">The width of the 1st array dimension.</param>
    /// <param name="height">The height of the 2nd array dimension.</param>
    /// <param name="assertRect">The rectangle that might contain any pixels to assert against.</param>
    /// <param name="expectedClr">The color that the pixels must be.</param>
    /// <remarks>
    ///     As long as the pixel is inside of the given <paramref name="assertRect"/>, the pixel
    ///     color will be asserted against the given <paramref name="expectedClr"/>.
    /// </remarks>
    [ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
    private static void AssertThatPixelsMatch(NETColor[,] pixels, uint width, uint height, NETRectangle assertRect, NETColor expectedClr)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (!assertRect.Contains(x, y))
                {
                    continue;
                }

                var pixel = pixels[x, y];

                expectedClr.A.Should().Be(pixel.A);
                expectedClr.R.Should().Be(pixel.R);
                expectedClr.G.Should().Be(pixel.G);
                expectedClr.B.Should().Be(pixel.B);
            }
        }
    }

    /// <summary>
    /// Loads all of the pixel data into a two dimensional array of <see cref="NETColor"/>.
    /// </summary>
    /// <returns>The pixel data from the test comparison image.</returns>
    [ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
    private static NETColor[,] LoadSaveResultImage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Assert.Fail($"The resulting image '{filePath}' from unit test '{nameof(Save_WhenInvoked_CorrectlySavesImage)}()' does not exist.");
        }

        var image = Image.Load<Rgba32>(filePath);

        var result = new NETColor[image.Width, image.Height];

        for (var y = 0; y < image.Height; y++)
        {
            var row = y;
            image.ProcessPixelRows(accessor =>
            {
                var pixelRowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < image.Width; x++)
                {
                    result[x, row] = NETColor.FromArgb(
                        pixelRowSpan[x].A,
                        pixelRowSpan[x].R,
                        pixelRowSpan[x].G,
                        pixelRowSpan[x].B);
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ImageService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ImageService CreateSystemUnderTest() => new (this.mockFile.Object);
}
