// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Shouldly;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureLoader"/> class.
/// </summary>
public class TextureLoaderTests
{
    private const string TextureExtension = ".png";
    private const string TextureContentName = "test-texture";
    private const string TextureFileName = $"{TextureContentName}{TextureExtension}";
    private const uint TextureId = 123u;
    private static readonly string AppDirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:" : "/";
    private static readonly string ContentDirPath = $"{AppDirPath}/Content";
    private static readonly string TextureDirPath = $"{ContentDirPath}/Graphics";
    private static readonly string TextureFilePath = $"{TextureDirPath}/{TextureFileName}";
    private readonly ITextureFactory mockTextureFactory;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IImageService mockImageService;
    private readonly IContentPathResolver mockTexturePathResolver;
    private readonly IDirectory mockDirectory;
    private readonly IPath mockPath;
    private readonly ITexture mockTexture;
    private readonly IPushReactable<DisposeTextureData> mockDisposeTextureReactable;
    private readonly IDisposable mockShutdownUnsubscriber;
    private IReceiveSubscription? mockShutdownSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
    /// </summary>
    public TextureLoaderTests()
    {
        this.mockTextureFactory = Substitute.For<ITextureFactory>();

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

        this.mockImageService = Substitute.For<IImageService>();

        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();

        this.mockTexturePathResolver = Substitute.For<IContentPathResolver>();
        this.mockTexturePathResolver.ResolveDirPath().Returns(TextureDirPath);
        this.mockTexturePathResolver.ResolveFilePath(Arg.Any<string>()).Returns(TextureFilePath);
        this.mockPath.GetFileNameWithoutExtension(Arg.Any<string>()).Returns(TextureContentName);
        this.mockImageService.Load(Arg.Any<string>()).Returns(default(ImageData));

        this.mockTexture = Substitute.For<ITexture>();
        this.mockTexture.Id.Returns(TextureId);

        this.mockTextureFactory
            .Create(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ImageData>())
            .Returns(this.mockTexture);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            null,
            this.mockReactableFactory,
            this.mockImageService,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockPath);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureFactory,
            null,
            this.mockImageService,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockPath);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            null,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockPath);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
                this.mockTextureFactory,
                this.mockReactableFactory,
                this.mockImageService,
                this.mockTexturePathResolver,
                null,
                this.mockPath);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureFactory,
            this.mockReactableFactory,
            this.mockImageService,
            this.mockTexturePathResolver,
            this.mockDirectory,
            null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Load_WithNullPathOrNameParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'pathOrName')");
    }

    [Fact]
    public void Load_WithEmptyPathOrNameParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(string.Empty);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'pathOrName')");
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(false);
        this.mockTexturePathResolver.ResolveDirPath().Returns($"{ContentDirPath}/Graphics");

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content.png");

        // Assert
        this.mockTexturePathResolver.Received(1).ResolveFilePath("test-content.png");
        this.mockDirectory.Received(1).CreateDirectory(TextureDirPath);
    }

    [Fact]
    public void Load_WhenLoadingUncachedContent_CachesAndLoadsTexture()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(TextureFileName);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        actual.ShouldBeSameAs(this.mockTexture);
        this.mockTexturePathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).Exists(TextureDirPath);
        this.mockDirectory.DidNotReceive().CreateDirectory(Arg.Any<string>());
        this.mockTexturePathResolver.Received(1).ResolveFilePath(TextureFileName);
        this.mockImageService.Received(1).Load(TextureFilePath);
        this.mockPath.Received(1).GetFileNameWithoutExtension(TextureFilePath);
        this.mockTextureFactory.Received(1).Create(TextureContentName, TextureFilePath, default);
    }

    [Fact]
    public void Load_WhenLoadingCachedContent_DoesNotCacheAndLoadsTexture()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actualA = sut.Load(TextureFileName);
        var actualB = sut.Load(TextureFileName);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        actualA.ShouldBeSameAs(this.mockTexture);
        actualB.ShouldBeSameAs(this.mockTexture);
        this.mockTexturePathResolver.Received(2).ResolveDirPath();
        this.mockDirectory.Received(2).Exists(TextureDirPath);
        this.mockDirectory.DidNotReceive().CreateDirectory(Arg.Any<string>());
        this.mockTexturePathResolver.Received(2).ResolveFilePath(TextureFileName);
        this.mockImageService.Received(1).Load(TextureFilePath);
        this.mockPath.Received(1).GetFileNameWithoutExtension(TextureFilePath);
        this.mockTextureFactory.Received(1).Create(TextureContentName, TextureFilePath, default);
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsCachedTextures()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var actual = sut.Load(TextureFileName);

        // Act
        sut.Unload(actual);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        this.mockDisposeTextureReactable.Received(1).Push(
            PushNotifications.TextureDisposedId,
            Arg.Is<DisposeTextureData>(data => data.TextureId == TextureId));
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
        sut.Load(TextureFileName);
        this.mockShutdownSubscription.OnReceive();
        this.mockShutdownSubscription.OnReceive(); // Tests idempotent behavior for the shutdown process

        // Assert
        sut.TotalCachedItems.ShouldBe(0);
        this.mockDisposeTextureReactable.Received(1).Push(
            PushNotifications.TextureDisposedId,
            Arg.Is<DisposeTextureData>(data => data.TextureId == TextureId));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureLoader CreateSystemUnderTest() => new (
        this.mockTextureFactory,
        this.mockReactableFactory,
        this.mockImageService,
        this.mockTexturePathResolver,
        this.mockDirectory,
        this.mockPath);
}
