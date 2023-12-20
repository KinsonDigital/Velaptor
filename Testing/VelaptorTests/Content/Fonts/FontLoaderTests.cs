// <copyright file="FontLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using Fakes;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Content.Fonts;
using Velaptor.Graphics;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FontLoader"/> class.
/// </summary>
public class FontLoaderTests
{
    private const int FontSize = 12;
    private const string FontExtension = ".ttf";
    private const string FontDirName = "fonts";
    private const string AppDirPath = "C:/app";
    private const string ContentDirPath = $"{AppDirPath}/content";
    private const string FontContentName = "test-font";
    private const string FontContentDirPath = $"{ContentDirPath}/{FontDirName}";
    private readonly string metaData = $"size:{FontSize}";
    private readonly string fontFilePath;
    private readonly string filePathWithMetaData;
    private readonly string contentNameWithMetaData;
    private readonly GlyphMetrics[] glyphMetricData;
    private readonly Mock<IFontAtlasService> mockFontAtlasService;
    private readonly Mock<IEmbeddedResourceLoaderService<Stream?>> mockEmbeddedFontResourceService;
    private readonly Mock<IContentPathResolver> mockContentPathResolver;
    private readonly Mock<IContentPathResolver> mockFontPathResolver;
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly Mock<IFontFactory> mockFontFactory;
    private readonly Mock<IFontMetaDataParser> mockFontMetaDataParser;
    private readonly Mock<IPath> mockPath;
    private readonly Mock<ITexture> mockAtlasTexture;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IFileStreamFactory> mockFileStream;
    private readonly Mock<IFont> mockFont;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
    /// </summary>
    public FontLoaderTests()
    {
        this.fontFilePath = $"{ContentDirPath}{FontDirName}/{FontContentName}{FontExtension}";
        this.filePathWithMetaData = $"{this.fontFilePath}|{this.metaData}";
        this.contentNameWithMetaData = $"{FontContentName}|{this.metaData}";

        this.mockAtlasTexture = new Mock<ITexture>();

        this.mockFont = new Mock<IFont>();

        this.glyphMetricData = new[]
        {
            GenerateMetricData(0),
            GenerateMetricData(10),
        };

        this.mockFontAtlasService = new Mock<IFontAtlasService>();
        this.mockFontAtlasService.Setup(m => m.CreateAtlas(this.fontFilePath, FontSize))
            .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

        this.mockEmbeddedFontResourceService = new Mock<IEmbeddedResourceLoaderService<Stream?>>();

        this.mockContentPathResolver = new Mock<IContentPathResolver>();
        this.mockContentPathResolver.SetupGet(p => p.RootDirectoryPath).Returns(ContentDirPath);

        // Mock for full file paths with metadata
        this.mockFontPathResolver = new Mock<IContentPathResolver>();
        this.mockFontPathResolver.SetupGet(p => p.RootDirectoryPath).Returns(ContentDirPath);
        this.mockFontPathResolver.SetupGet(p => p.ContentDirectoryName).Returns(FontDirName);

        this.mockFontPathResolver.Setup(p => p.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);

        // Mock for both full file paths and content names with metadata
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
        this.mockTextureCache.Setup(m => m.GetItem(this.filePathWithMetaData))
            .Returns(this.mockAtlasTexture.Object);

        // Mock for both full file paths and content names with metadata
        this.mockFontFactory = new Mock<IFontFactory>();
        this.mockFontFactory.Setup(m =>
                m.Create(this.mockAtlasTexture.Object,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    It.IsAny<bool>(),
                    this.glyphMetricData))
            .Returns(this.mockFont.Object);

        this.mockFontMetaDataParser = new Mock<IFontMetaDataParser>();
        // Mock for full file paths with metadata
        this.mockFontMetaDataParser.Setup(m => m.Parse(this.filePathWithMetaData))
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = this.fontFilePath,
                MetaData = this.metaData,
                FontSize = FontSize,
            });

        // Mock for content names with metadata
        this.mockFontMetaDataParser.Setup(m => m.Parse(this.contentNameWithMetaData))
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = FontContentName,
                MetaData = this.metaData,
                FontSize = FontSize,
            });

        this.mockDirectory = new Mock<IDirectory>();

        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(this.fontFilePath)).Returns(true);

        this.mockFileStream = new Mock<IFileStreamFactory>();

        // Mock for both full file paths and content names with metadata
        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}"))
            .Returns(FontContentName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}{FontExtension}"))
            .Returns(FontContentName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.fontFilePath))
            .Returns(FontContentName);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                null,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullEmbeddedFontResourceService_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                null,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'embeddedFontResourceService')");
    }

    [Fact]
    public void Ctor_WithNullContentPathResolver_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                null,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'contentPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullFontPathResolver_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                null,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullTextureCache_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                null,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullFontFactory_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                null,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontFactory')");
    }

    [Fact]
    public void Ctor_WithNullFontMetaDataParser_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                null,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontMetaDataParser')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                null,
                this.mockFile.Object,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                null,
                this.mockFileStream.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                null,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fileStream')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontAtlasService.Object,
                this.mockEmbeddedFontResourceService.Object,
                this.mockContentPathResolver.Object,
                this.mockFontPathResolver.Object,
                this.mockTextureCache.Object,
                this.mockFontFactory.Object,
                this.mockFontMetaDataParser.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockFileStream.Object,
                null);
        };

        // Arrange
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WhenFontContentDirectoryDoesNotExist_CreatesFontContentDirectory()
    {
        // Arrange
        const string defaultRegularFontName = $"TimesNewRoman-Regular{FontExtension}";
        const string defaultBoldFontName = $"TimesNewRoman-Bold{FontExtension}";
        const string defaultItalicFontName = $"TimesNewRoman-Italic{FontExtension}";
        const string defaultBoldItalicFontName = $"TimesNewRoman-BoldItalic{FontExtension}";

        const string defaultRegularFontFilePath = $"{FontContentDirPath}/{defaultRegularFontName}";
        const string defaultBoldFontFilePath = $"{FontContentDirPath}/{defaultBoldFontName}";
        const string defaultItalicFontFilePath = $"{FontContentDirPath}/{defaultItalicFontName}";
        const string defaultBoldItalicFontFilePath = $"{FontContentDirPath}/{defaultBoldItalicFontName}";

        var mockRegularFontFileStream = MockLoadResource(defaultRegularFontName);
        var mockBoldFontFileStream = MockLoadResource(defaultBoldFontName);
        var mockItalicFontFileStream = MockLoadResource(defaultItalicFontName);
        var mockBoldItalicFontFileStream = MockLoadResource(defaultBoldItalicFontName);

        var mockCopyToRegularStream = MockCopyToStream(defaultRegularFontFilePath);
        var mockCopyToBoldStream = MockCopyToStream(defaultBoldFontFilePath);
        var mockCopyToItalicStream = MockCopyToStream(defaultItalicFontFilePath);
        var mockCopyToBoldItalicStream = MockCopyToStream(defaultBoldItalicFontFilePath);

        this.mockDirectory.Setup(m => m.Exists(ContentDirPath)).Returns(false);
        this.mockDirectory.Setup(m => m.Exists(FontContentDirPath)).Returns(false);

        this.mockFile.Setup(m => m.Exists(defaultRegularFontFilePath)).Returns(false);
        this.mockFile.Setup(m => m.Exists(defaultBoldFontFilePath)).Returns(false);
        this.mockFile.Setup(m => m.Exists(defaultItalicFontFilePath)).Returns(true);
        this.mockFile.Setup(m => m.Exists(defaultBoldItalicFontFilePath)).Returns(false);

        this.mockPath.SetupGet(p => p.AltDirectorySeparatorChar).Returns('/');

        // Act
        CreateSystemUnderTest();

        // Assert
        this.mockFontPathResolver.VerifyGet(p => p.ContentDirectoryName, Times.Once);

        // Check for directory existence
        this.mockDirectory.Verify(m => m.Exists(FontContentDirPath), Times.Once);

        // Each file was verified if it exists
        this.mockFile.Verify(m => m.Exists(defaultRegularFontFilePath), Times.Once);
        this.mockFile.Verify(m => m.Exists(defaultBoldFontFilePath), Times.Once);
        this.mockFile.Verify(m => m.Exists(defaultItalicFontFilePath), Times.Once);
        this.mockFile.Verify(m => m.Exists(defaultBoldItalicFontFilePath), Times.Once);

        // Check that each file was created
        this.mockFileStream.Verify(m =>
                m.New(defaultRegularFontFilePath, FileMode.Create, FileAccess.Write),
            Times.Once);
        this.mockFileStream.Verify(m =>
                m.New(defaultBoldFontFilePath, FileMode.Create, FileAccess.Write),
            Times.Once);
        this.mockFileStream.Verify(m =>
                m.New(defaultItalicFontFilePath, FileMode.Create, FileAccess.Write),
            Times.Never);
        this.mockFileStream.Verify(m =>
                m.New(defaultBoldItalicFontFilePath, FileMode.Create, FileAccess.Write),
            Times.Once);

        mockRegularFontFileStream.VerifyOnce(m => m.CopyTo(mockCopyToRegularStream.Object, It.IsAny<int>()));
        mockBoldFontFileStream.VerifyOnce(m => m.CopyTo(mockCopyToBoldStream.Object, It.IsAny<int>()));
        mockItalicFontFileStream.VerifyNever(m => m.CopyTo(mockCopyToItalicStream.Object, It.IsAny<int>()));
        mockBoldItalicFontFileStream.VerifyOnce(m => m.CopyTo(mockCopyToBoldItalicStream.Object, It.IsAny<int>()));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Load_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void Load_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void Load_WithInvalidMetaData_ThrowsException()
    {
        // Arrange
        const string contentName = "invalid-metadata";
        const string invalidMetaData = "size-12";

        var expected = $"The metadata '{invalidMetaData}' is invalid when loading '{contentName}'.";
        expected += $"{Environment.NewLine}\tExpected MetaData Syntax: size:<font-size>";
        expected += $"{Environment.NewLine}\tExample: size:12";

        this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = false,
                MetaDataPrefix = string.Empty,
                MetaData = invalidMetaData,
                FontSize = FontSize,
            });
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(contentName);

        // Assert
        act.Should().Throw<CachingMetaDataException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Load_WithNoMetaData_UsesDefaultMetaData()
    {
        // Arrange
        const string contentPathOrName = "no-metadata";
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(It.IsAny<string>())).Returns(contentPathOrName);
        this.mockFontPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns($"{FontContentDirPath}/{contentPathOrName}.ttf");
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockFontMetaDataParser.Setup(m => m.Parse(It.IsAny<string>()))
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = false,
                IsValid = false,
                MetaData = string.Empty,
                MetaDataPrefix = string.Empty,
                FontSize = 0,
            });

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load(contentPathOrName);

        // Assert
        this.mockFontPathResolver.VerifyOnce(m => m.ResolveFilePath(contentPathOrName));
        this.mockFontAtlasService.VerifyOnce(m => m.CreateAtlas(It.IsAny<string>(), 12));
        this.mockTextureCache
            .VerifyOnce(m => m.GetItem($"{FontContentDirPath}/{contentPathOrName}.ttf|size:12"));
        this.mockFontFactory
            .VerifyOnce(m => m.Create(
                It.IsAny<ITexture>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                12,
                It.IsAny<bool>(),
                It.IsAny<GlyphMetrics[]>()));
    }

    [Fact]
    public void Load_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.fontFilePath)).Returns(false);
        this.mockPath.Setup(m => m.IsPathRooted(this.fontFilePath)).Returns(true);

        var expected = $"The font content item '{this.fontFilePath}' does not exist.";

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(this.filePathWithMetaData);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Load_WithFileNameAndExtensionOnly_LoadsFontFromContentDirectory()
    {
        // Arrange
        const string fileNameWithExt = $"{FontContentName}{FontExtension}";
        const string fileNameWithExtAndMetaData = $"{fileNameWithExt}|size:12";

        this.mockFontMetaDataParser.Setup(m => m.Parse(fileNameWithExtAndMetaData))
            .Returns(new FontMetaDataParseResult
                {
                    ContainsMetaData = true,
                    IsValid = true,
                    MetaDataPrefix = fileNameWithExt,
                    MetaData = this.metaData,
                    FontSize = FontSize,
                });
        this.mockFontPathResolver.Setup(m => m.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExt)).Returns(FontContentName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExtAndMetaData)).Returns(FontContentName);
        this.mockFontAtlasService.Setup(m => m.CreateAtlas(this.fontFilePath, FontSize))
            .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(fileNameWithExtAndMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(fileNameWithExtAndMetaData), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(fileNameWithExt), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
        this.mockFontAtlasService.Verify(m => m.CreateAtlas(this.fontFilePath, FontSize), Times.Once);
        this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
        this.mockFontFactory.Verify(m =>
                m.Create(
                    this.mockAtlasTexture.Object,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    It.IsAny<bool>(),
                    this.glyphMetricData),
            Times.Once);

        actual.Should().BeEquivalentTo(this.mockFont.Object);
    }

    [Fact]
    public void Load_WhenUsingFullFilePathWithMetaData_LoadsFont()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string>())).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.filePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
        this.mockFontAtlasService.Verify(m => m.CreateAtlas(this.fontFilePath, FontSize), Times.Once);
        this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
        this.mockFontFactory.Verify(m =>
                m.Create(
                    this.mockAtlasTexture.Object,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    It.IsAny<bool>(),
                    this.glyphMetricData),
            Times.Once);

        actual.Should().BeEquivalentTo(this.mockFont.Object);
    }

    [Fact]
    public void Load_WhenUsingContentNameWithMetaData_LoadsFont()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.contentNameWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
        this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
        this.mockFontAtlasService.Verify(m => m.CreateAtlas(this.fontFilePath, FontSize), Times.Once);
        this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);

        this.mockFontFactory.Verify(m =>
                m.Create(
                    this.mockAtlasTexture.Object,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    It.IsAny<bool>(),
                    this.glyphMetricData),
            Times.Once);

        actual.Should().BeEquivalentTo(this.mockFont.Object);
    }

    [Fact]
    public void Unload_WithInvalidMetaData_ThrowsException()
    {
        // Arrange
        const string contentName = "invalid-metadata";
        const string invalidMetaData = "size-12";

        var expected = $"The metadata '{invalidMetaData}' is invalid when unloading '{contentName}'.";
        expected += $"{Environment.NewLine}\tExpected MetaData Syntax: size:<font-size>";
        expected += $"{Environment.NewLine}\tExample: size:12";

        this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
            .Returns(new FontMetaDataParseResult
                {
                    ContainsMetaData = true,
                    IsValid = false,
                    MetaDataPrefix = string.Empty,
                    MetaData = invalidMetaData,
                    FontSize = FontSize,
                });
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Unload(contentName);

        // Assert
        act.Should().Throw<CachingMetaDataException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData("no-metadata", $"{FontContentDirPath}/no-metadata.ttf|size:12")]
    [InlineData("TimesNewRoman-Regular", $"[DEFAULT]{FontContentDirPath}/TimesNewRoman-Regular.ttf|size:12")]
    public void Unload_WithNoMetaData_UnloadsFont(string contentNameOrPath, string expected)
    {
        // Arrange
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(It.IsAny<string?>()))
            .Returns(contentNameOrPath);
        this.mockFontPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns($"{FontContentDirPath}/{contentNameOrPath}.ttf");
        this.mockPath.Setup(m => m.GetFileName(It.IsAny<string?>())).Returns($"{contentNameOrPath}.ttf");
        this.mockFontMetaDataParser.Setup(m => m.Parse(contentNameOrPath))
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = false,
                IsValid = false,
                MetaDataPrefix = string.Empty,
                MetaData = string.Empty,
                FontSize = FontSize,
            });
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(contentNameOrPath);

        // Assert
        this.mockTextureCache.VerifyOnce(m => m.Unload(expected));
    }

    [Fact]
    public void Unload_WhenUnloadingWithFullFilePathAndMetaData_UnloadsFonts()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string>())).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(this.filePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
        this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
    }

    [Fact]
    public void Unload_WhenUnloadingWithContentNameAndMetaData_UnloadsFonts()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(this.contentNameWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
        this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
        this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
    }
    #endregion

    /// <summary>
    /// Generates fake glyph metric data for testing.
    /// </summary>
    /// <param name="start">The start value of all of the metric data.</param>
    /// <returns>The glyph metric data to be tested.</returns>
    /// <remarks>
    ///     The start value is a metric value start and incremented for each metric.
    /// </remarks>
    private static GlyphMetrics GenerateMetricData(int start)
    {
        return new GlyphMetrics
        {
            Ascender = start,
            Descender = start + 1,
            CharIndex = (uint)start + 2,
            GlyphWidth = start + 3,
            GlyphHeight = start + 4,
            HoriBearingX = start + 5,
            HoriBearingY = start + 6,
            XMin = start + 7,
            XMax = start + 8,
            YMin = start + 9,
            YMax = start + 10,
            HorizontalAdvance = start + 11,
            Glyph = (char)(start + 12),
            GlyphBounds = new RectangleF(start + 13, start + 14, start + 15, start + 16),
        };
    }

    /// <summary>
    /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontLoader CreateSystemUnderTest() => new (
        this.mockFontAtlasService.Object,
        this.mockEmbeddedFontResourceService.Object,
        this.mockContentPathResolver.Object,
        this.mockFontPathResolver.Object,
        this.mockTextureCache.Object,
        this.mockFontFactory.Object,
        this.mockFontMetaDataParser.Object,
        this.mockDirectory.Object,
        this.mockFile.Object,
        this.mockFileStream.Object,
        this.mockPath.Object);

    /// <summary>
    /// Mocks the loading of an embedded font resource file using the given name for the purpose of testing.
    /// </summary>
    /// <param name="name">The name of the resource to mock.</param>
    /// <returns>The mock object to verify against.</returns>
    private Mock<Stream> MockLoadResource(string name)
    {
        var result = new Mock<Stream>();
        this.mockEmbeddedFontResourceService.Setup(m => m.LoadResource(name))
            .Returns(result.Object);

        return result;
    }

    /// <summary>
    /// Mocks the creation of a file stream for the given <paramref name="filePath"/>
    /// for the purpose of testing.
    /// </summary>
    /// <param name="filePath">The file path to mock.</param>
    /// <returns>The mock object to verify against.</returns>
    private Mock<FileSystemStreamFake> MockCopyToStream(string filePath)
    {
        var result = new Mock<FileSystemStreamFake>();
        this.mockFileStream.Setup(m => m.New(filePath, FileMode.Create, FileAccess.Write))
            .Returns(result.Object);

        return result;
    }
}
