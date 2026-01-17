// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Shouldly;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasLoader"/> class.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLocalFunction", Justification = "Improves readability")]
public class AtlasLoaderTests
{
    private const string AtlasImageExtension = ".png";
    private const string AtlasDataExtension = ".json";
    private const string AtlasContentName = "test-atlas";
    private const string FakeJSONData = "fake-json-data";
    private const uint AtlasTextureId = 123;
    private static readonly string AtlasDirPath = Path.Combine("C:", "Content", "Atlas");
    private static readonly string AtlasImageFilePath = Path.Combine(AtlasDirPath, AtlasContentName + AtlasImageExtension);
    private static readonly string AtlasDataFilePath = Path.Combine(AtlasDirPath, AtlasContentName + AtlasDataExtension);
    private readonly ITextureFactory mockTextureFactory;
    private readonly IAtlasDataFactory mockAtlasDataFactory;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IContentPathResolver mockAtlasPathResolver;
    private readonly IImageService mockImageService;
    private readonly IJsonService mockJSONService;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IAtlasData mockAtlasData;
    private readonly ITexture mockAtlasTexture;
    private readonly IDisposable mockShutdownUnsubscriber;
    private readonly IPushReactable<DisposeTextureData> mockDisposeTextureReactable;
    private IReceiveSubscription? mockShutdownSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
    /// </summary>
    public AtlasLoaderTests()
    {
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

        this.mockAtlasTexture = Substitute.For<ITexture>();
        this.mockAtlasTexture.Id.Returns(AtlasTextureId);

        this.mockAtlasData = Substitute.For<IAtlasData>();
        this.mockAtlasData.Texture.Returns(this.mockAtlasTexture);

        this.mockAtlasDataFactory = Substitute.For<IAtlasDataFactory>();
        this.mockAtlasDataFactory
            .Create(Arg.Any<ITexture>(),
                Arg.Any<IList<AtlasSubTextureData>>(),
                Arg.Any<string>(),
                Arg.Any<string>())
            .Returns(this.mockAtlasData);

        this.mockTextureFactory = Substitute.For<ITextureFactory>();
        this.mockTextureFactory.Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>())
            .Returns(this.mockAtlasTexture);

        this.mockAtlasPathResolver = Substitute.For<IContentPathResolver>();
        this.mockAtlasPathResolver.ResolveDirPath().Returns(AtlasDirPath);

        this.mockImageService = Substitute.For<IImageService>();

        this.mockJSONService = Substitute.For<IJsonService>();
        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockFile = Substitute.For<IFile>();
        this.mockFile.ReadAllText(AtlasDataFilePath)
            .Returns(FakeJSONData);
        this.mockFile.Exists(AtlasDataFilePath).Returns(true);
        this.mockFile.Exists(AtlasImageFilePath).Returns(true);

        this.mockPath = Substitute.For<IPath>();

        this.mockPath.Combine(AtlasDirPath, AtlasContentName + AtlasDataExtension).Returns(AtlasDataFilePath);
        this.mockPath.Combine(AtlasDirPath, AtlasContentName + AtlasImageExtension).Returns(AtlasImageFilePath);

        this.mockPath.GetDirectoryName(AtlasImageFilePath).Returns(AtlasDirPath);
        this.mockPath.GetFileNameWithoutExtension(AtlasImageFilePath).Returns(AtlasContentName);
        this.mockPath.GetFileNameWithoutExtension(AtlasContentName).Returns(AtlasContentName);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                null,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullAtlasDataFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                null,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlasDataFactory')");
    }

    [Fact]
    public void Ctor_WithNullAtlasDataPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
           _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                null,
                this.mockImageService,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlasDataPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                null,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullJSONServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'jsonService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                this.mockJSONService,
                null,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                this.mockJSONService,
                this.mockDirectory,
                null,
                this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureFactory,
                this.mockAtlasDataFactory,
                this.mockReactableFactory,
                this.mockAtlasPathResolver,
                this.mockImageService,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                null);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
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
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlasPathOrName')");
    }

    [Fact]
    public void Load_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(string.Empty);

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'atlasPathOrName')");
    }

    [Fact]
    public void Load_WithInvalidFullFilePathExtensions_ThrowsException()
    {
        // Arrange
        const string extension = ".txt";
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load($"{AtlasDirPath}/{AtlasContentName}{extension}");

        // Assert
        var exception = act.ShouldThrow<LoadAtlasException>();
        exception.Message.ShouldBe("When loading atlas data with fully qualified paths, the files must be a '.png' or '.json' extension.");
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>()).Returns(".png");
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAtlasPathResolver.ResolveDirPath().Returns(AtlasDirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockAtlasPathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).CreateDirectory(AtlasDirPath);
    }

    [Fact]
    public void Load_WhenUsingRootedFilePath_LoadsAtlasData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasImageExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        var atlasData = MockAtlasJSONData().ToArray();
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(atlasData);

        // Act
        var actual = sut.Load(AtlasImageFilePath);

        // Assert
        actual.ShouldBeSameAs(this.mockAtlasData);
        actual.Texture.ShouldBeSameAs(this.mockAtlasTexture);
        this.mockPath.Received(1).GetFileNameWithoutExtension(AtlasImageFilePath);
        this.mockFile.Received(1).ReadAllText(AtlasDataFilePath);
        this.mockJSONService.Received(1).Deserialize<AtlasSubTextureData[]>(FakeJSONData);
    }

    [Fact]
    public void Load_WhenAtlasJSONDataFileDoesNotExist_ThrowsException()
    {
        // Arrange
        const string missingJsonContentName = "missing-json-file";
        var nonMissingImgFile = Path.Combine(AtlasDirPath, missingJsonContentName + AtlasImageExtension);
        var missingJsonFilePath = Path.Combine(AtlasDirPath, missingJsonContentName + AtlasDataExtension);

        var sut = CreateSystemUnderTest();

        var expected = $"The atlas data directory '{AtlasDirPath}' does not contain the";
        expected += $" required '{missingJsonFilePath}' atlas data file.";

        this.mockFile.Exists(nonMissingImgFile).Returns(true);
        this.mockFile.Exists(missingJsonFilePath).Returns(false);

        this.mockPath.GetDirectoryName(Arg.Any<string>()).Returns(AtlasDirPath);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(missingJsonContentName);
        this.mockPath.Combine(AtlasDirPath, missingJsonContentName + AtlasDataExtension).Returns(missingJsonFilePath);
        this.mockPath.Combine(AtlasDirPath, missingJsonContentName + AtlasImageExtension).Returns(nonMissingImgFile);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasImageExtension);
        this.mockPath.GetDirectoryName(missingJsonFilePath).Returns(AtlasDirPath);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        // Act
        var act = () => sut.Load(missingJsonFilePath);

        // Assert
        var exception = act.ShouldThrow<FileNotFoundException>();
        exception.Message.ShouldBe(expected);
        exception.FileName.ShouldBe(missingJsonFilePath);
    }

    [Fact]
    public void Load_WhenAtlasImageFileDoesNotExist_ThrowsException()
    {
        // Arrange
        const string missingImgContentName = "missing-img-file";
        var nonMissingDataFile = Path.Combine(AtlasDirPath, missingImgContentName + AtlasDataExtension);
        var missingImgFilePath = Path.Combine(AtlasDirPath, missingImgContentName + AtlasImageExtension);

        var sut = CreateSystemUnderTest();

        var expected = $"The atlas data directory '{AtlasDirPath}' does not contain the";
        expected += $" required '{missingImgFilePath}' atlas image file.";

        this.mockFile.Exists(nonMissingDataFile).Returns(true);
        this.mockFile.Exists(missingImgFilePath).Returns(false);

        this.mockPath.GetDirectoryName(Arg.Any<string>()).Returns(AtlasDirPath);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(missingImgContentName);
        this.mockPath.Combine(AtlasDirPath, missingImgContentName + AtlasDataExtension).Returns(nonMissingDataFile);
        this.mockPath.Combine(AtlasDirPath, missingImgContentName + AtlasImageExtension).Returns(missingImgFilePath);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasImageExtension);
        this.mockPath.GetDirectoryName(missingImgFilePath).Returns(AtlasDirPath);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        // Act
        var act = () => sut.Load(missingImgFilePath);

        // Assert
        var exception = act.ShouldThrow<FileNotFoundException>();
        exception.Message.ShouldBe(expected);
        exception.FileName.ShouldBe(missingImgFilePath);
    }

    [Fact]
    public void Load_WhenUsingOnlyContentName_LoadsAtlasData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        var atlasData = MockAtlasJSONData().ToArray();
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(atlasData);

        // Act
        var actual = sut.Load(AtlasContentName);

        // Assert
        this.mockPath.Received(1).GetFileNameWithoutExtension(AtlasContentName);
        this.mockAtlasPathResolver.Received(1).ResolveDirPath();
        this.mockFile.Received(1).ReadAllText(AtlasDataFilePath);
        this.mockJSONService.Received(1).Deserialize<AtlasSubTextureData[]>(FakeJSONData);
        actual.ShouldBeSameAs(this.mockAtlasData);
    }

    [Fact]
    public void Load_WhenNullJSONDataDeserializationResult_ThrowsException()
    {
        // Arrange
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(_ => null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(AtlasContentName);

        // Assert
        var exception = act.ShouldThrow<LoadContentException>();
        exception.Message.ShouldBe($"There was an issue deserializing the JSON atlas data file at '{AtlasDataFilePath}'.");
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsAtlas()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var atlas = sut.Load(AtlasContentName);

        // Act
        sut.Unload(atlas);

        // Assert
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
        var sut = CreateSystemUnderTest();

        // Act
        sut.Load(AtlasContentName);
        this.mockShutdownSubscription.OnReceive();
        this.mockShutdownSubscription.OnReceive(); // Tests idempotent behavior for the shutdown process

        // Assert
        sut.TotalCachedItems.ShouldBe(0);
        this.mockDisposeTextureReactable
            .Received(1).Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = AtlasTextureId });
    }
    #endregion

    /// <summary>
    /// Creates atlas sub texture data for the purpose of testing.
    /// </summary>
    /// <returns>The data to use for testing.</returns>
    private static AtlasSubTextureData[] CreateAtlasSubTextureData()
    {
        var items = new List<AtlasSubTextureData>();

        for (var i = 0; i < 2; i++)
        {
            items.Add(new AtlasSubTextureData
            {
                Name = $"sub-texture{i}",
                Bounds = new Rectangle(i + 1, i + 2, i + 3, i + 4),
                FrameIndex = i,
            });
        }

        return items.ToArray();
    }

    /// <summary>
    /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AtlasLoader CreateSystemUnderTest()
        => new (
            this.mockTextureFactory,
            this.mockAtlasDataFactory,
            this.mockReactableFactory,
            this.mockAtlasPathResolver,
            this.mockImageService,
            this.mockJSONService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

    /// <summary>
    /// Mocks the JSON data deserialization process.
    /// </summary>
    /// <returns>The test data to use if the mock is invoked correctly.</returns>
    private IEnumerable<AtlasSubTextureData> MockAtlasJSONData()
    {
        var data = CreateAtlasSubTextureData();

        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(data);

        return data;
    }
}
