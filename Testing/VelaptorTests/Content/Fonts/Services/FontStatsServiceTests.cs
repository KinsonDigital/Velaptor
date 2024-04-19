// <copyright file="FontStatsServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts.Services;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Content.Fonts.Services;
using Velaptor.NativeInterop.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FontStatsService"/> class.
/// </summary>
public class FontStatsServiceTests
{
    private const string RootContentDirPath = @"C:\content-dir\";
    private const string DirNameForContentPath = "content-fonts";
    private const string RootSystemDirPath = @"C:\system-dir\";
    private const string DirNameForSystemPath = "system-fonts";
    private readonly string fullContentFontDirPath = $@"{RootContentDirPath}{DirNameForContentPath}\";
    private readonly string fullSystemFontDirPath = $@"{RootSystemDirPath}{DirNameForSystemPath}\";
    private readonly Mock<IFreeTypeService> mockFreeTypeService;
    private readonly Mock<IContentPathResolver> mockSystemFontPathResolver;
    private readonly Mock<IContentPathResolver> mockContentPathResolver;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontStatsServiceTests"/> class.
    /// </summary>
    public FontStatsServiceTests()
    {
        this.mockFreeTypeService = new Mock<IFreeTypeService>();

        this.mockContentPathResolver = new Mock<IContentPathResolver>();
        this.mockContentPathResolver.Setup(m => m.ResolveDirPath())
            .Returns(this.fullContentFontDirPath);
        this.mockContentPathResolver.SetupGet(p => p.ContentDirectoryName)
            .Returns(DirNameForContentPath);

        this.mockSystemFontPathResolver = new Mock<IContentPathResolver>();
        this.mockSystemFontPathResolver.Setup(m => m.ResolveDirPath())
            .Returns(this.fullSystemFontDirPath);
        this.mockSystemFontPathResolver.SetupGet(p => p.ContentDirectoryName)
            .Returns(DirNameForSystemPath);

        this.mockDirectory = new Mock<IDirectory>();

        this.mockPath = new Mock<IPath>();
    }

    #region Constructor Tests

    [Fact]
    public void Ctor_WithNullFontServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontStatsService(
                null,
                this.mockContentPathResolver.Object,
                this.mockSystemFontPathResolver.Object,
                this.mockDirectory.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'freeTypeService')");
    }

    [Fact]
    public void Ctor_WithNullContentFontPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontStatsService(
                this.mockFreeTypeService.Object,
                null,
                this.mockSystemFontPathResolver.Object,
                this.mockDirectory.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'contentPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullSysFontPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontStatsService(
                this.mockFreeTypeService.Object,
                this.mockContentPathResolver.Object,
                null,
                this.mockDirectory.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'sysFontPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontStatsService(
                this.mockFreeTypeService.Object,
                this.mockContentPathResolver.Object,
                this.mockSystemFontPathResolver.Object,
                null,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontStatsService(
                this.mockFreeTypeService.Object,
                this.mockContentPathResolver.Object,
                this.mockSystemFontPathResolver.Object,
                this.mockDirectory.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetContentStatsForFontFamily_WhenInvoked_ReturnsCorrectListOfFontStyles()
    {
        // Arrange
        const string fontFamily = "Times New Roman";
        var fontTimesRegular = BuildContentFontPath("TimesNewRoman-Regular.ttf");
        var fontTimesBold = BuildContentFontPath("TimesNewRoman-Bold.ttf");
        var fontTimesItalic = BuildContentFontPath("TimesNewRoman-Italic.ttf");
        var fontTimesBoldItalic = BuildContentFontPath("TimesNewRoman-BoldItalic.ttf");

        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesRegular)).Returns(this.fullContentFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBold)).Returns(this.fullContentFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesItalic)).Returns(this.fullContentFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBoldItalic)).Returns(this.fullContentFontDirPath);

        var fontFiles = new[]
        {
            fontTimesRegular,
            fontTimesBold,
            fontTimesItalic,
            fontTimesBoldItalic,
        };

        MockAllFontFamilies(fontFiles, fontFamily);

        MockFontStyle(fontTimesRegular, FontStyle.Regular);
        MockFontStyle(fontTimesBold, FontStyle.Bold);
        MockFontStyle(fontTimesItalic, FontStyle.Italic);
        MockFontStyle(fontTimesBoldItalic, FontStyle.Bold | FontStyle.Italic);

        this.mockDirectory.Setup(m => m.GetFiles(this.fullContentFontDirPath, "*.ttf"))
            .Returns(() => fontFiles);

        var expected = new[]
        {
            new FontStats { FontFilePath = fontTimesRegular, FamilyName = fontFamily, Style = FontStyle.Regular, Source = FontSource.AppContent },
            new FontStats { FontFilePath = fontTimesBold, FamilyName = fontFamily, Style = FontStyle.Bold, Source = FontSource.AppContent },
            new FontStats { FontFilePath = fontTimesItalic, FamilyName = fontFamily, Style = FontStyle.Italic, Source = FontSource.AppContent },
            new FontStats { FontFilePath = fontTimesBoldItalic, FamilyName = fontFamily, Style = FontStyle.Bold | FontStyle.Italic, Source = FontSource.AppContent },
        };

        var sut = CreateSystemUnderTest();

        // Act
        sut.GetContentStatsForFontFamily(fontFamily); // Executed 2 times to check for caching
        var actual = sut.GetContentStatsForFontFamily(fontFamily);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetSystemStatsForFontFamily_WhenInvoked_ReturnsCorrectListOfFontStyles()
    {
        // Arrange
        const string fontFamily = "Times New Roman";
        var fontTimesRegular = BuildSystemFontPath("TimesNewRoman-Regular.ttf");
        var fontTimesBold = BuildSystemFontPath("TimesNewRoman-Bold.ttf");
        var fontTimesItalic = BuildSystemFontPath("TimesNewRoman-Italic.ttf");
        var fontTimesBoldItalic = BuildSystemFontPath("TimesNewRoman-BoldItalic.ttf");

        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesRegular)).Returns(this.fullSystemFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBold)).Returns(this.fullSystemFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesItalic)).Returns(this.fullSystemFontDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBoldItalic)).Returns(this.fullSystemFontDirPath);

        var fontFiles = new[]
        {
            fontTimesRegular,
            fontTimesBold,
            fontTimesItalic,
            fontTimesBoldItalic,
        };

        MockAllFontFamilies(fontFiles, fontFamily);

        MockFontStyle(fontTimesRegular, FontStyle.Regular);
        MockFontStyle(fontTimesBold, FontStyle.Bold);
        MockFontStyle(fontTimesItalic, FontStyle.Italic);
        MockFontStyle(fontTimesBoldItalic, FontStyle.Bold | FontStyle.Italic);

        this.mockDirectory.Setup(m => m.GetFiles(this.fullSystemFontDirPath, "*.ttf"))
            .Returns(() => fontFiles);

        var expected = new[]
        {
            new FontStats { FontFilePath = fontTimesRegular, FamilyName = fontFamily, Style = FontStyle.Regular, Source = FontSource.System },
            new FontStats { FontFilePath = fontTimesBold, FamilyName = fontFamily, Style = FontStyle.Bold, Source = FontSource.System },
            new FontStats { FontFilePath = fontTimesItalic, FamilyName = fontFamily, Style = FontStyle.Italic, Source = FontSource.System },
            new FontStats { FontFilePath = fontTimesBoldItalic, FamilyName = fontFamily, Style = FontStyle.Bold | FontStyle.Italic, Source = FontSource.System },
        };

        var sut = CreateSystemUnderTest();

        // Act
        sut.GetSystemStatsForFontFamily(fontFamily); // Executed 2 times to check for caching
        var actual = sut.GetSystemStatsForFontFamily(fontFamily);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetSystemStatsForFontFamily_WithNonContentOrSystemFontPaths_ReturnsCorrectListOfFontStyles()
    {
        // Arrange
        const string fontFamily = "Times New Roman";
        const string unknownDirPath = @"C:\Unknown\";
        var fontTimesRegular = $"{unknownDirPath}TimesNewRoman-Regular.ttf";
        var fontTimesBold = $"{unknownDirPath}TimesNewRoman-Bold.ttf";
        var fontTimesItalic = $"{unknownDirPath}TimesNewRoman-Italic.ttf";
        var fontTimesBoldItalic = $"{unknownDirPath}TimesNewRoman-BoldItalic.ttf";

        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesRegular)).Returns(unknownDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBold)).Returns(unknownDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesItalic)).Returns(unknownDirPath);
        this.mockPath.Setup(m => m.GetDirectoryName(fontTimesBoldItalic)).Returns(unknownDirPath);

        var fontFiles = new[]
        {
            fontTimesRegular,
            fontTimesBold,
            fontTimesItalic,
            fontTimesBoldItalic,
        };

        MockAllFontFamilies(fontFiles, fontFamily);

        MockFontStyle(fontTimesRegular, FontStyle.Regular);
        MockFontStyle(fontTimesBold, FontStyle.Bold);
        MockFontStyle(fontTimesItalic, FontStyle.Italic);
        MockFontStyle(fontTimesBoldItalic, FontStyle.Bold | FontStyle.Italic);

        this.mockDirectory.Setup(m => m.GetFiles(this.fullSystemFontDirPath, "*.ttf"))
            .Returns(() => fontFiles);

        var expected = new[]
        {
            new FontStats { FontFilePath = fontTimesRegular, FamilyName = fontFamily, Style = FontStyle.Regular, Source = FontSource.Unknown },
            new FontStats { FontFilePath = fontTimesBold, FamilyName = fontFamily, Style = FontStyle.Bold, Source = FontSource.Unknown },
            new FontStats { FontFilePath = fontTimesItalic, FamilyName = fontFamily, Style = FontStyle.Italic, Source = FontSource.Unknown },
            new FontStats { FontFilePath = fontTimesBoldItalic, FamilyName = fontFamily, Style = FontStyle.Bold | FontStyle.Italic, Source = FontSource.Unknown },
        };

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetSystemStatsForFontFamily(fontFamily);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new service for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontStatsService CreateSystemUnderTest() => new (
        this.mockFreeTypeService.Object,
        this.mockContentPathResolver.Object,
        this.mockSystemFontPathResolver.Object,
        this.mockDirectory.Object,
        this.mockPath.Object);

    /// <summary>
    /// Builds a path to a application font directory location.
    /// </summary>
    /// <param name="fileName">The file name to include in the path.</param>
    /// <returns>The application file path.</returns>
    private string BuildContentFontPath(string fileName) => $"{this.fullContentFontDirPath}{fileName}";

    /// <summary>
    /// Builds a path to a system font location.
    /// </summary>
    /// <param name="fileName">The file name to include in the path.</param>
    /// <returns>The system file path.</returns>
    private string BuildSystemFontPath(string fileName) => $"{this.fullSystemFontDirPath}{fileName}";

    /// <summary>
    /// Mocks the font at the given <paramref name="filePath"/> with the given font <paramref name="familyName"/>.
    /// </summary>
    /// <param name="filePath">The file path to the font file.</param>
    /// <param name="familyName">The family of the font to mock.</param>
    private void MockFontFamilyName(string filePath, string familyName)
    {
        this.mockFreeTypeService.Setup(m => m.GetFamilyName(filePath))
            .Returns(familyName);
    }

    /// <summary>
    /// Mocks all the font families at the given <paramref name="filePaths"/> with the given font <paramref name="familyName"/>.
    /// </summary>
    /// <param name="filePaths">All the file paths to the fonts.</param>
    /// <param name="familyName">The family of the font to mock.</param>
    private void MockAllFontFamilies(IEnumerable<string> filePaths, string familyName)
    {
        foreach (var path in filePaths)
        {
            MockFontFamilyName(path, familyName);
        }
    }

    /// <summary>
    /// Mocks the given font style of the font at the given <paramref name="filePath"/> using the given font <paramref name="style"/>.
    /// </summary>
    /// <param name="filePath">The path to the font file.</param>
    /// <param name="style">The style to mock.</param>
    private void MockFontStyle(string filePath, FontStyle style)
    {
        this.mockFreeTypeService.Setup(m => m.GetFontStyle(filePath))
            .Returns(style);
    }
}
