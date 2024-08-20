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
using NSubstitute;
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
    private readonly IEmbeddedResourceLoaderService<Stream?> mockEmbeddedFontResourceService;
    private readonly IItemCache<string, ITexture> mockTextureCache;
    private readonly IFontAtlasService mockFontAtlasService;
    private readonly IContentPathResolver mockContentPathResolver;
    private readonly IContentPathResolver mockFontPathResolver;
    private readonly IFontFactory mockFontFactory;
    private readonly IFontMetaDataParser mockFontMetaDataParser;
    private readonly IPath mockPath;
    private readonly ITexture mockAtlasTexture;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IFileStreamFactory mockFileStream;
    private readonly IFont mockFont;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
    /// </summary>
    public FontLoaderTests()
    {
        this.fontFilePath = $"{ContentDirPath}{FontDirName}/{FontContentName}{FontExtension}";
        this.filePathWithMetaData = $"{this.fontFilePath}|{this.metaData}";
        this.contentNameWithMetaData = $"{FontContentName}|{this.metaData}";

        this.mockAtlasTexture = Substitute.For<ITexture>();

        this.mockFont = Substitute.For<IFont>();

        this.glyphMetricData =
        [
            GenerateMetricData(0),
            GenerateMetricData(10)
        ];

        this.mockFontAtlasService = Substitute.For<IFontAtlasService>();
        this.mockFontAtlasService.CreateAtlas(this.fontFilePath, FontSize).Returns((default(ImageData), this.glyphMetricData));

        this.mockEmbeddedFontResourceService = Substitute.For<IEmbeddedResourceLoaderService<Stream?>>();

        this.mockContentPathResolver = Substitute.For<IContentPathResolver>();
        this.mockContentPathResolver.RootDirectoryPath.Returns(ContentDirPath);

        // Mock for full file paths with metadata
        this.mockFontPathResolver = Substitute.For<IContentPathResolver>();
        this.mockFontPathResolver.RootDirectoryPath.Returns(ContentDirPath);
        this.mockFontPathResolver.ContentDirectoryName.Returns(FontDirName);

        this.mockFontPathResolver.ResolveFilePath(FontContentName).Returns(this.fontFilePath);

        // Mock for both full file paths and content names with metadata
        this.mockTextureCache = Substitute.For<IItemCache<string, ITexture>>();
        this.mockTextureCache.GetItem(this.filePathWithMetaData).Returns(this.mockAtlasTexture);

        // Mock for both full file paths and content names with metadata
        this.mockFontFactory = Substitute.For<IFontFactory>();
        this.mockFontFactory.Create(this.mockAtlasTexture,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    Arg.Any<bool>(),
                    this.glyphMetricData)
            .Returns(this.mockFont);

        this.mockFontMetaDataParser = Substitute.For<IFontMetaDataParser>();
        // Mock for full file paths with metadata
        this.mockFontMetaDataParser.Parse(this.filePathWithMetaData)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = this.fontFilePath,
                MetaData = this.metaData,
                FontSize = FontSize,
            });

        // Mock for content names with metadata
        this.mockFontMetaDataParser.Parse(this.contentNameWithMetaData)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = FontContentName,
                MetaData = this.metaData,
                FontSize = FontSize,
            });

        this.mockDirectory = Substitute.For<IDirectory>();

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(this.fontFilePath).Returns(true);

        this.mockFileStream = Substitute.For<IFileStreamFactory>();

        // Mock for both full file paths and content names with metadata
        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetFileNameWithoutExtension($"{FontContentName}").Returns(FontContentName);
        this.mockPath.GetFileNameWithoutExtension($"{FontContentName}{FontExtension}").Returns(FontContentName);
        this.mockPath.GetFileNameWithoutExtension(this.fontFilePath).Returns(FontContentName);
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
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                null,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                null,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                null,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                null,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                null,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                null,
                this.mockFile,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                null,
                this.mockFileStream,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                null,
                this.mockPath);
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
                this.mockFontAtlasService,
                this.mockEmbeddedFontResourceService,
                this.mockContentPathResolver,
                this.mockFontPathResolver,
                this.mockTextureCache,
                this.mockFontFactory,
                this.mockFontMetaDataParser,
                this.mockDirectory,
                this.mockFile,
                this.mockFileStream,
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

        this.mockDirectory.Exists(ContentDirPath).Returns(false);
        this.mockDirectory.Exists(FontContentDirPath).Returns(false);

        this.mockFile.Exists(defaultRegularFontFilePath).Returns(false);
        this.mockFile.Exists(defaultBoldFontFilePath).Returns(false);
        this.mockFile.Exists(defaultItalicFontFilePath).Returns(true);
        this.mockFile.Exists(defaultBoldItalicFontFilePath).Returns(false);

        this.mockPath.AltDirectorySeparatorChar.Returns('/');

        // Act
        CreateSystemUnderTest();

        // Assert
        // Check for directory existence
        this.mockDirectory.Received(1).Exists(FontContentDirPath);

        // Each file was verified if it exists
        this.mockFile.Received(1).Exists(defaultRegularFontFilePath);
        this.mockFile.Received(1).Exists(defaultBoldFontFilePath);
        this.mockFile.Received(1).Exists(defaultItalicFontFilePath);
        this.mockFile.Received(1).Exists(defaultBoldItalicFontFilePath);

        // Check that each file was created
        this.mockFileStream.Received(1).New(defaultRegularFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStream.Received(1).New(defaultBoldFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStream.DidNotReceive().New(defaultItalicFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStream.Received(1).New(defaultBoldItalicFontFilePath, FileMode.Create, FileAccess.Write);

        mockRegularFontFileStream.Received(1).CopyTo(mockCopyToRegularStream, Arg.Any<int>());
        mockBoldFontFileStream.Received(1).CopyTo(mockCopyToBoldStream, Arg.Any<int>());
        mockItalicFontFileStream.DidNotReceive().CopyTo(mockCopyToItalicStream, Arg.Any<int>());
        mockBoldItalicFontFileStream.Received(1).CopyTo(mockCopyToBoldItalicStream, Arg.Any<int>());
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

        this.mockFontMetaDataParser.Parse(contentName).Returns(new FontMetaDataParseResult
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
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(contentPathOrName);
        this.mockFontPathResolver.ResolveFilePath(Arg.Any<string>()).Returns($"{FontContentDirPath}/{contentPathOrName}.ttf");
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockFontMetaDataParser.Parse(Arg.Any<string>()).Returns(new FontMetaDataParseResult
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
        this.mockFontPathResolver.Received(1).ResolveFilePath(contentPathOrName);
        this.mockFontAtlasService.Received(1).CreateAtlas(Arg.Any<string>(), 12);
        this.mockTextureCache.Received(1).GetItem($"{FontContentDirPath}/{contentPathOrName}.ttf|size:12");
        this.mockFontFactory
            .Received(1).Create(
                Arg.Any<ITexture>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                12,
                Arg.Any<bool>(),
                Arg.Any<GlyphMetrics[]>());
    }

    [Fact]
    public void Load_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Exists(this.fontFilePath).Returns(false);
        this.mockPath.IsPathRooted(this.fontFilePath).Returns(true);

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

        this.mockFontMetaDataParser.Parse(fileNameWithExtAndMetaData).Returns(new FontMetaDataParseResult
                {
                    ContainsMetaData = true,
                    IsValid = true,
                    MetaDataPrefix = fileNameWithExt,
                    MetaData = this.metaData,
                    FontSize = FontSize,
                });
        this.mockFontPathResolver.ResolveFilePath(FontContentName).Returns(this.fontFilePath);
        this.mockPath.GetFileNameWithoutExtension(fileNameWithExt).Returns(FontContentName);
        this.mockPath.GetFileNameWithoutExtension(fileNameWithExtAndMetaData).Returns(FontContentName);
        this.mockFontAtlasService.CreateAtlas(this.fontFilePath, FontSize).Returns((default(ImageData), this.glyphMetricData));

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(fileNameWithExtAndMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(1).Parse(fileNameWithExtAndMetaData);
        this.mockPath.Received(1).GetFileNameWithoutExtension(fileNameWithExt);
        this.mockPath.Received(1).GetFileNameWithoutExtension(this.fontFilePath);
        this.mockFontAtlasService.Received(1).CreateAtlas(this.fontFilePath, FontSize);
        this.mockTextureCache.Received(1).GetItem(this.filePathWithMetaData);
        this.mockFontFactory.Received(1).Create(
                    this.mockAtlasTexture,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    Arg.Any<bool>(),
                    this.glyphMetricData);

        actual.Should().BeEquivalentTo(this.mockFont);
    }

    [Fact]
    public void Load_WhenUsingFullFilePathWithMetaData_LoadsFont()
    {
        // Arrange
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.filePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(1).Parse(this.filePathWithMetaData);
        this.mockPath.Received(1).GetFileNameWithoutExtension(this.fontFilePath);
        this.mockFontAtlasService.Received(1).CreateAtlas(this.fontFilePath, FontSize);
        this.mockTextureCache.Received(1).GetItem(this.filePathWithMetaData);
        this.mockFontFactory.Received(1).Create(
                    this.mockAtlasTexture,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    Arg.Any<bool>(),
                    this.glyphMetricData);

        actual.Should().BeEquivalentTo(this.mockFont);
    }

    [Fact]
    public void Load_WhenUsingContentNameWithMetaData_LoadsFont()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.contentNameWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(1).Parse(this.contentNameWithMetaData);
        this.mockFontPathResolver.Received(1).ResolveFilePath(FontContentName);
        this.mockPath.Received(1).GetFileNameWithoutExtension(this.fontFilePath);
        this.mockFontAtlasService.Received(1).CreateAtlas(this.fontFilePath, FontSize);
        this.mockTextureCache.Received(1).GetItem(this.filePathWithMetaData);

        this.mockFontFactory.Received(1).Create(
                    this.mockAtlasTexture,
                    FontContentName,
                    this.fontFilePath,
                    FontSize,
                    Arg.Any<bool>(),
                    this.glyphMetricData);

        actual.Should().BeEquivalentTo(this.mockFont);
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

        this.mockFontMetaDataParser.Parse(contentName).Returns(new FontMetaDataParseResult
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
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string?>()).Returns(contentNameOrPath);
        this.mockFontPathResolver.ResolveFilePath(Arg.Any<string>()).Returns($"{FontContentDirPath}/{contentNameOrPath}.ttf");
        this.mockPath.GetFileName(Arg.Any<string?>()).Returns($"{contentNameOrPath}.ttf");
        this.mockFontMetaDataParser.Parse(contentNameOrPath).Returns(new FontMetaDataParseResult
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
        this.mockTextureCache.Received(1).Unload(expected);
    }

    [Fact]
    public void Unload_WhenUnloadingWithFullFilePathAndMetaData_UnloadsFonts()
    {
        // Arrange
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(this.filePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(1).Parse(this.filePathWithMetaData);
        this.mockTextureCache.Received(1).Unload(this.filePathWithMetaData);
    }

    [Fact]
    public void Unload_WhenUnloadingWithContentNameAndMetaData_UnloadsFonts()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(this.contentNameWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(1).Parse(this.contentNameWithMetaData);
        this.mockFontPathResolver.Received(1).ResolveFilePath(FontContentName);
        this.mockTextureCache.Received(1).Unload(this.filePathWithMetaData);
    }
    #endregion

    /// <summary>
    /// Generates fake glyph metric data for testing.
    /// </summary>
    /// <param name="start">The start value of all the metric data.</param>
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
        this.mockFontAtlasService,
        this.mockEmbeddedFontResourceService,
        this.mockContentPathResolver,
        this.mockFontPathResolver,
        this.mockTextureCache,
        this.mockFontFactory,
        this.mockFontMetaDataParser,
        this.mockDirectory,
        this.mockFile,
        this.mockFileStream,
        this.mockPath);

    /// <summary>
    /// Mocks the loading of an embedded font resource file using the given name for the purpose of testing.
    /// </summary>
    /// <param name="name">The name of the resource to mock.</param>
    /// <returns>The mock object to verify against.</returns>
    private Stream MockLoadResource(string name)
    {
        var result = Substitute.For<Stream>();
        this.mockEmbeddedFontResourceService.LoadResource(name).Returns(result);

        return result;
    }

    /// <summary>
    /// Mocks the creation of a file stream for the given <paramref name="filePath"/>
    /// for the purpose of testing.
    /// </summary>
    /// <param name="filePath">The file path to mock.</param>
    /// <returns>The mock object to verify against.</returns>
    private FileSystemStreamFake MockCopyToStream(string filePath)
    {
        var result = Substitute.For<FileSystemStreamFake>();
        this.mockFileStream.New(filePath, FileMode.Create, FileAccess.Write).Returns(result);

        return result;
    }
}
