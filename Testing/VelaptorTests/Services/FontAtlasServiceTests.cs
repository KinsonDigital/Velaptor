// <copyright file="FontAtlasServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Moq;
using Velaptor;
using Velaptor.Content.Fonts.Services;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Velaptor.Hardware;
using Velaptor.Services;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="FontAtlasService"/> class.
/// </summary>
public class FontAtlasServiceTests
{
    private const string FontFilePath = @"C:\temp\test-font.ttf";
    private readonly Mock<IFontService> mockFontService;
    private readonly Mock<IImageService> mockImageService;
    private readonly Mock<ISystemMonitorService> mockMonitorService;
    private readonly Mock<IPlatform> mockPlatform;
    private readonly char[]? glyphChars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
        '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ', '□',
    };
    private readonly Dictionary<char, uint> glyphIndices = new ();
    private readonly Mock<IFile> mockFile;
    private readonly IntPtr facePtr = new (5678);

    /// <summary>
    /// Initializes a new instance of the <see cref="FontAtlasServiceTests"/> class.
    /// </summary>
    public FontAtlasServiceTests()
    {
        TestHelpers.SetupTestResultDirPath();

        // Set up the glyph indices
        foreach (var glyphChar in this.glyphChars)
        {
            this.glyphIndices.Add(glyphChar, glyphChar);
        }

        this.mockFontService = new Mock<IFontService>();
        this.mockFontService.Setup(m => m.CreateFontFace(FontFilePath))
            .Returns(() => this.facePtr);

        this.mockFontService.Setup(m => m.GetGlyphIndices(this.facePtr, this.glyphChars))
            .Returns(() => this.glyphIndices);

        this.mockFontService.Setup(m => m.CreateGlyphMetrics(
                this.facePtr,
                this.mockFontService.Object.GetGlyphIndices(this.facePtr, this.glyphChars)))
            .Returns(() =>
            {
                var result = new Dictionary<char, GlyphMetrics>();

                for (var i = 0; i < this.glyphChars.Length; i++)
                {
                    GlyphMetrics newMetric = default;

                    newMetric.Glyph = this.glyphChars[i];
                    newMetric.CharIndex = this.glyphIndices[this.glyphChars[i]];
                    newMetric.Ascender = (i + 1) * 10;
                    newMetric.Descender = (i + 2) * 10;
                    newMetric.GlyphWidth = (i + 3) * 10;
                    newMetric.GlyphHeight = (i + 4) * 10;
                    newMetric.HoriBearingX = (i + 5) * 10;
                    newMetric.HoriBearingY = (i + 6) * 10;
                    newMetric.HorizontalAdvance = (i + 7) * 10;
                    newMetric.XMin = (i + 8) * 10;
                    newMetric.XMax = (i + 9) * 10;
                    newMetric.YMin = (i + 10) * 10;
                    newMetric.YMax = (i + 11) * 10;

                    result.Add(this.glyphChars[i], newMetric);
                }

                return result;
            });

        this.mockFontService.Setup(m => m.CreateGlyphImage(this.facePtr, It.IsAny<char>(), It.IsAny<uint>()))
            .Returns(() =>
            {
                return (new byte[]
                {
                    255, 255,
                }, 1, 2);
            });

        this.mockImageService = new Mock<IImageService>();

        this.mockMonitorService = new Mock<ISystemMonitorService>();
        this.mockMonitorService.SetupGet(p => p.MainMonitor)
            .Returns(() => new SystemMonitor(this.mockPlatform.Object)
            {
                HorizontalScale = 1,
                VerticalScale = 1,
            });

        this.mockPlatform = new Mock<IPlatform>();
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(FontFilePath)).Returns(true);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullFontServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                null,
                this.mockImageService.Object,
                this.mockMonitorService.Object,
                this.mockFile.Object);
        }, "The parameter must not be null. (Parameter 'fontService')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService.Object,
                null,
                this.mockMonitorService.Object,
                this.mockFile.Object);
        }, "The parameter must not be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullSystemMonitorServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService.Object,
                this.mockImageService.Object,
                null,
                this.mockFile.Object);
        }, "The parameter must not be null. (Parameter 'systemMonitorService')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService.Object,
                this.mockImageService.Object,
                this.mockMonitorService.Object,
                null);
        }, "The parameter must not be null. (Parameter 'file')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateFontAtlas_WithNullOrEmptyFilePath_ThrowsException(string fontFilePath)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            service.CreateFontAtlas(fontFilePath, It.IsAny<uint>());
        }, "The font file path argument must not be null. (Parameter 'fontFilePath')");
    }

    [Fact]
    public void CreateFontAtlas_WhenFontFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
        var service = CreateService();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            service.CreateFontAtlas(FontFilePath, It.IsAny<uint>());
        }, $"The file '{FontFilePath}' does not exist.");
    }

    [Fact]
    public void CreateFontAtlas_WhenSettingCharacterSizeWithNullMainMonitor_ThrowsException()
    {
        // Arrange
        this.mockMonitorService.SetupGet(p => p.MainMonitor).Returns(() => null);

        var service = CreateService();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<SystemMonitorException>(() =>
        {
            service.CreateFontAtlas(FontFilePath, It.IsAny<uint>());
        }, "The main system monitor must not be null.");
    }

    [Fact]
    public void CreateFontAtlas_WhenInvoked_SetsCharacterSize()
    {
        // Arrange
        const int fontSize = 12;

        var service = CreateService();

        // Act
        service.CreateFontAtlas(FontFilePath, fontSize);

        // Assert
        this.mockFontService.Verify(m => m.SetFontSize(this.facePtr, 12), Times.Once());
    }

    [Fact]
    public void CreateFontAtlas_WhenInvoked_CreatesAllGlyphImages()
    {
        // Arrange
        this.mockImageService.Setup(m => m.Draw(It.IsAny<ImageData>(), It.IsAny<ImageData>(), It.IsAny<Point>()))
            .Returns<ImageData, ImageData, Point>(TestHelpers.Draw);

        var service = CreateService();

        // Act
        var (actualImage, _) = service.CreateFontAtlas(FontFilePath, 12);

        // Save the the results
        TestHelpers.SaveImageForTest(actualImage);

        // Assert
        this.mockFontService.Verify(
            m => m.CreateGlyphImage(
                It.IsAny<IntPtr>(),
                It.IsAny<char>(),
                It.IsAny<uint>()),
            Times.Exactly(95));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontAtlasService"/> for the purposes of testing.
    /// </summary>
    /// <returns>An instance to use for testing.</returns>
    private FontAtlasService CreateService()
    {
        var result = new FontAtlasService(
            this.mockFontService.Object,
            this.mockImageService.Object,
            this.mockMonitorService.Object,
            this.mockFile.Object);

        return result;
    }
}
