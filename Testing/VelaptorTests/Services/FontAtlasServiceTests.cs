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
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.Hardware;
using Velaptor.NativeInterop.Services;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FontAtlasService"/> class.
/// </summary>
public class FontAtlasServiceTests
{
    private const string FontFilePath = @"C:\temp\test-font.ttf";
    private readonly IFreeTypeService mockFontService;
    private readonly IImageService mockImageService;
    private readonly ISystemDisplayService mockDisplayService;
    private readonly IPlatform mockPlatform;
    private readonly char[]? glyphChars =
    [
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
        '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ', '□'
    ];
    private readonly Dictionary<char, uint> glyphIndices = new ();
    private readonly IFile mockFile;
    private readonly nint facePtr;

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

        this.mockFontService = Substitute.For<IFreeTypeService>();
        this.mockFontService.CreateFontFace(FontFilePath).Returns((_) => this.facePtr);

        this.mockFontService.GetGlyphIndices(Arg.Any<nint>(), Arg.Any<char[]>())
            .Returns((_) => this.glyphIndices);

        this.mockFontService.CreateGlyphMetrics(
            Arg.Any<nint>(),
            Arg.Any<Dictionary<char, uint>>()).Returns((_) =>
            {
                var result = new Dictionary<char, GlyphMetrics>();

                for (var i = 0; i < this.glyphChars.Length; i++)
                {
                    GlyphMetrics newMetric = new GlyphMetrics
                    {
                        Glyph = this.glyphChars[i],
                        CharIndex = this.glyphIndices[this.glyphChars[i]],
                        Ascender = (i + 1) * 10,
                        Descender = (i + 2) * 10,
                        GlyphWidth = (i + 3) * 10,
                        GlyphHeight = (i + 4) * 10,
                        HoriBearingX = (i + 5) * 10,
                        HoriBearingY = (i + 6) * 10,
                        HorizontalAdvance = (i + 7) * 10,
                        XMin = (i + 8) * 10,
                        XMax = (i + 9) * 10,
                        YMin = (i + 10) * 10,
                        YMax = (i + 11) * 10,
                    };
                    result.Add(this.glyphChars[i], newMetric);
                }

                return result;
            });

        this.mockFontService.CreateGlyphImage(this.facePtr, Arg.Any<uint>())
            .Returns<(byte[], uint, uint)>((_) =>
            {
                return (new byte[]
                {
                    255, 255,
                }, 1, 2);
            });

        this.mockImageService = Substitute.For<IImageService>();

        this.mockDisplayService = Substitute.For<ISystemDisplayService>();
        this.mockDisplayService.MainDisplay.Returns((_) => new SystemDisplay(this.mockPlatform)
            {
                HorizontalScale = 1,
                VerticalScale = 1,
            });

        this.mockPlatform = Substitute.For<IPlatform>();
        this.mockPlatform.CurrentPlatform.Returns((_) => OSPlatform.Windows);

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(FontFilePath).Returns((_) => true);
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
                this.mockImageService,
                this.mockDisplayService,
                this.mockFile);
        }, "Value cannot be null. (Parameter 'freeTypeService')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService,
                null,
                this.mockDisplayService,
                this.mockFile);
        }, "Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService,
                this.mockImageService,
                null,
                this.mockFile);
        }, "Value cannot be null. (Parameter 'systemDisplayService')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new FontAtlasService(
                this.mockFontService,
                this.mockImageService,
                this.mockDisplayService,
                null);
        }, "Value cannot be null. (Parameter 'file')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateAtlas_WithNullOrEmptyFilePath_ThrowsException(string? fontFilePath)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            service.CreateAtlas(fontFilePath, 12);
        }, "The font file path argument must not be null. (Parameter 'fontFilePath')");
    }

    [Fact]
    public void CreateAtlas_WhenFontFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        var service = CreateService();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            service.CreateAtlas(FontFilePath, 12);
        }, $"The file '{FontFilePath}' does not exist.");
    }

    [Fact]
    public void CreateAtlas_WhenInvoked_SetsCharacterSize()
    {
        // Arrange
        const int fontSize = 12;

        var service = CreateService();

        // Act
        service.CreateAtlas(FontFilePath, fontSize);

        // Assert
        this.mockFontService.Received(1).SetFontSize(this.facePtr, 12);
    }

    [Fact]
    public void CreateAtlas_WhenInvoked_CreatesAllGlyphImages()
    {
        // Arrange
        ImageData src = default;
        ImageData dest = default;
        Point location = default;

        this.mockImageService
            .When(x => x.Draw(Arg.Any<ImageData>(), Arg.Any<ImageData>(), Arg.Any<Point>()))
            .Do(callInfo =>
            {
                src = callInfo.ArgAt<ImageData>(0);
                dest = callInfo.ArgAt<ImageData>(1);
                location = callInfo.Arg<Point>();
            });

        this.mockImageService.Draw(Arg.Any<ImageData>(), Arg.Any<ImageData>(), Arg.Any<Point>())
            .Returns((_) => TestHelpers.Draw(src, dest, location));

        var service = CreateService();

        // Act
        (ImageData actualImage, _) = service.CreateAtlas(FontFilePath, 12);

        // Save the results
        TestHelpers.SaveImageForTest(actualImage);

        // Assert
        this.mockFontService.Received(1).CreateFontFace(FontFilePath);
        this.mockFontService.Received(95).CreateGlyphImage(
                Arg.Any<nint>(),
                Arg.Any<uint>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontAtlasService"/> for the purposes of testing.
    /// </summary>
    /// <returns>An instance to use for testing.</returns>
    private FontAtlasService CreateService()
    {
        var result = new FontAtlasService(
            this.mockFontService,
            this.mockImageService,
            this.mockDisplayService,
            this.mockFile);

        return result;
    }
}
