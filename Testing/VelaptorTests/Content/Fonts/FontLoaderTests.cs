// <copyright file="FontLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Fakes;
using Shouldly;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Factories;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
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
    private const string FontFileName = $"{FontContentName}{FontExtension}";
    private const string FontContentDirPath = $"{ContentDirPath}/{FontDirName}";
    private const uint TextureAtlasId = 123u;
    private readonly string defaultFontFilePath;
    private readonly IFontAtlasService mockFontAtlasService;
    private readonly IEmbeddedResourceLoaderService<Stream?> mockEmbeddedFontResourceService;
    private readonly IContentPathResolver mockFontPathResolver;
    private readonly ITextureFactory mockTextureFactory;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IFontFactory mockFontFactory;
    private readonly IFileStreamFactory mockFileStreamFactory;
    private readonly IPath mockPath;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPushReactable<DisposeTextureData> mockDisposeTextureReactable;
    private readonly IDisposable mockShutdownUnsubscriber;

    private IReceiveSubscription? mockShutdownSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
    /// </summary>
    public FontLoaderTests()
    {
        this.defaultFontFilePath = $"{ContentDirPath}{FontDirName}/{FontContentName}{FontExtension}";

        var mockAtlasTexture = Substitute.For<ITexture>();
        mockAtlasTexture.Id.Returns(TextureAtlasId);

        GlyphMetrics[] glyphMetricData1 =
        [
            GenerateMetricData(0),
            GenerateMetricData(10)
        ];

        // Mock for full file paths with metadata
        this.mockFontPathResolver = Substitute.For<IContentPathResolver>();
        this.mockFontPathResolver.RootDirectoryPath.Returns(ContentDirPath);
        this.mockFontPathResolver.ContentDirectoryName.Returns(FontDirName);
        this.mockFontPathResolver.ResolveFilePath(FontContentName).Returns(this.defaultFontFilePath);

        this.mockTextureFactory = Substitute.For<ITextureFactory>();
        this.mockTextureFactory
            .Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>())
            .Returns(mockAtlasTexture);

        this.mockShutdownUnsubscriber = Substitute.For<IDisposable>();
        var mockShutdownReactable = Substitute.For<IPushReactable>();
        mockShutdownReactable.Subscribe(Arg.Any<IReceiveSubscription>()).Returns(this.mockShutdownUnsubscriber);
        mockShutdownReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription>();

                if (subscription.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.mockShutdownSubscription = subscription;
                }
            });

        this.mockDisposeTextureReactable = Substitute.For<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(this.mockDisposeTextureReactable);
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockShutdownReactable);

        this.mockFileStreamFactory = Substitute.For<IFileStreamFactory>();

        var mockFont = Substitute.For<IFont>();
        mockFont.Name.Returns(FontContentName);
        mockFont.FilePath.Returns(this.defaultFontFilePath);
        mockFont.Atlas.Returns(mockAtlasTexture);

        this.mockFontFactory = Substitute.For<IFontFactory>();
        this.mockFontFactory.Create(
                Arg.Any<ITexture>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<uint>(),
                Arg.Any<bool>(),
                Arg.Any<GlyphMetrics[]>())
            .Returns(mockFont);

        this.mockEmbeddedFontResourceService = Substitute.For<IEmbeddedResourceLoaderService<Stream?>>();

        this.mockFontAtlasService = Substitute.For<IFontAtlasService>();
        this.mockFontAtlasService.CreateAtlas(this.defaultFontFilePath, FontSize).Returns((default(ImageData), glyphMetricData1));

        this.mockDirectory = Substitute.For<IDirectory>();

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(this.defaultFontFilePath).Returns(true);

        // Mock for both full file paths and content names with metadata
        this.mockPath = Substitute.For<IPath>();
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(FontContentName);
        this.mockPath.GetFileName(Arg.Any<string>()).Returns(FontFileName);
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullFontPathResolver_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                null,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fontPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullTextureFactory_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                null,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactory_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                null,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullFileStreamFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                null,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fileStreamFactory')");
    }

    [Fact]
    public void Ctor_WithNullFontFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                null,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fontFactory')");
    }

    [Fact]
    public void Ctor_WithNullEmbeddedFontResourceService_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                null,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'embeddedFontResourceService')");
    }

    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                null,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                null,
                this.mockPath);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontLoader(
                this.mockFontPathResolver,
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockFileStreamFactory,
                this.mockFontFactory,
                this.mockEmbeddedFontResourceService,
                this.mockFontAtlasService,
                this.mockDirectory,
                this.mockFile,
                null);
        };

        // Arrange
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
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
        this.mockFileStreamFactory.Received(1).New(defaultRegularFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStreamFactory.Received(1).New(defaultBoldFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStreamFactory.DidNotReceive().New(defaultItalicFontFilePath, FileMode.Create, FileAccess.Write);
        this.mockFileStreamFactory.Received(1).New(defaultBoldItalicFontFilePath, FileMode.Create, FileAccess.Write);

        mockRegularFontFileStream.Received(1).CopyTo(mockCopyToRegularStream, Arg.Any<int>());
        mockBoldFontFileStream.Received(1).CopyTo(mockCopyToBoldStream, Arg.Any<int>());
        mockItalicFontFileStream.DidNotReceive().CopyTo(mockCopyToItalicStream, Arg.Any<int>());
        mockBoldItalicFontFileStream.Received(1).CopyTo(mockCopyToBoldItalicStream, Arg.Any<int>());
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Load_WithNullPathOrNameParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(null, 12);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'pathOrName')");
    }

    [Fact]
    public void Load_WithEmptyPathOrName_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(string.Empty, 12);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'pathOrName')");
    }

    [Fact]
    public void Load_WhenContentFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);

        // Act
        var act = () => sut.Load("non-exiting-font.ttf", 12);

        // Assert
        var exception = act.ShouldThrow<FileNotFoundException>();
        exception.Message.ShouldBe("The font content item 'non-exiting-font.ttf' does not exist.");
    }

    [Fact]
    public void Load_WhenInvokedWithRootedPathToFont_LoadsFont()
    {
        // Arrange
        var imageData = default(ImageData);
        var glyphMetrics = Array.Empty<GlyphMetrics>();
        var atlasData = (imageData, glyphMetrics);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);

        this.mockFontAtlasService.CreateAtlas(Arg.Any<string>(), Arg.Any<uint>())
            .Returns(atlasData);
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>());

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.defaultFontFilePath, FontSize);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        this.mockPath.Received(1).IsPathRooted(this.defaultFontFilePath);
        this.mockFontPathResolver.DidNotReceive().ResolveFilePath(Arg.Any<string>());
        this.mockFile.Received(1).Exists(this.defaultFontFilePath);
        this.mockPath.Received(1).GetFileNameWithoutExtension(this.defaultFontFilePath);
        this.mockPath.Received(1).GetFileName(this.defaultFontFilePath);
        this.mockFontAtlasService.Received(1).CreateAtlas(this.defaultFontFilePath, FontSize);
        this.mockTextureFactory.Received(1).Create(FontContentName, this.defaultFontFilePath, imageData);
        actual.Name.ShouldBe(FontContentName);
        actual.FilePath.ShouldBe(this.defaultFontFilePath);
    }

    [Fact]
    public void Load_WhenInvokedWithNoRootedPathToFont_ResolvesPathAndLoadsFont()
    {
        // Arrange
        var imageData = default(ImageData);
        var glyphMetrics = Array.Empty<GlyphMetrics>();
        var atlasData = (imageData, glyphMetrics);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(false);
        this.mockFontPathResolver.ResolveFilePath(Arg.Any<string>()).Returns(this.defaultFontFilePath);

        this.mockFontAtlasService.CreateAtlas(Arg.Any<string>(), Arg.Any<uint>())
            .Returns(atlasData);
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>());

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(this.defaultFontFilePath, FontSize);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        this.mockPath.Received(1).IsPathRooted(this.defaultFontFilePath);
        this.mockFontPathResolver.Received().ResolveFilePath(this.defaultFontFilePath);
        this.mockFile.Received(1).Exists(this.defaultFontFilePath);
        this.mockPath.Received(1).GetFileNameWithoutExtension(this.defaultFontFilePath);
        this.mockPath.Received(1).GetFileName(this.defaultFontFilePath);
        this.mockFontAtlasService.Received(1).CreateAtlas(this.defaultFontFilePath, FontSize);
        this.mockTextureFactory.Received(1).Create(FontContentName, this.defaultFontFilePath, imageData);
        actual.Name.ShouldBe(FontContentName);
        actual.FilePath.ShouldBe(this.defaultFontFilePath);
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsFont()
    {
        // Arrange
        var expectedDisposeTextureData = new DisposeTextureData { TextureId = TextureAtlasId };
        var sut = CreateSystemUnderTest();
        var font = sut.Load(this.defaultFontFilePath, 12);

        // Act
        sut.Unload(font);

        // Assert
        this.mockDisposeTextureReactable.Received(1).Push(PushNotifications.TextureDisposedId, expectedDisposeTextureData);
        sut.TotalCachedItems.ShouldBe(1);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void Reactables_WhenUnsubscribing_DisposesOfSubscription()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.mockShutdownSubscription.OnUnsubscribe();

        // Assert
        this.mockShutdownUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    public void ShutdownProcess_WhenInvoked_ShutsDownFontLoader()
    {
        // Arrange
        var expectedDisposeTextureData = new DisposeTextureData { TextureId = TextureAtlasId };
        var sut = CreateSystemUnderTest();

        // Act
        sut.Load(this.defaultFontFilePath, 12);
        this.mockShutdownSubscription.OnReceive();
        this.mockShutdownSubscription.OnReceive(); // Tests idempotent behavior for the shutdown process

        // Assert
        this.mockDisposeTextureReactable.Received(1).Push(PushNotifications.TextureDisposedId, expectedDisposeTextureData);
        sut.TotalCachedItems.ShouldBe(0);
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
        this.mockFontPathResolver,
        this.mockTextureFactory,
        this.mockReactableFactory,
        this.mockFileStreamFactory,
        this.mockFontFactory,
        this.mockEmbeddedFontResourceService,
        this.mockFontAtlasService,
        this.mockDirectory,
        this.mockFile,
        this.mockPath);

    /// <summary>
    /// Mocks the loading of an embedded font resource file using the given name for testing.
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
        this.mockFileStreamFactory.New(filePath, FileMode.Create, FileAccess.Write).Returns(result);

        return result;
    }
}
