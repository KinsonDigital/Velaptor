// <copyright file="AudioLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
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
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="AudioLoader"/> class.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLocalFunction", Justification = "Improves readability")]
[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1514: Element documentation header requires it be preceded by a blank line", Justification = "Improves readability")]
public class AudioLoaderTests
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const string AudioContentName = "test-audio";
    private const string OggFileName = $"{AudioContentName}{OggFileExtension}";
    private const string Mp3FileName = $"{AudioContentName}{Mp3FileExtension}";
    private const uint AudioId = 123;
    private static readonly string BaseDirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:" : "/";
    private static readonly string ContentDirPath = $"{BaseDirPath}/Content";
    private static readonly string AudioDirPath = $"{ContentDirPath}/Audio";
    private static readonly string OggFilePath = $"{AudioDirPath}/{OggFileName}";
    private static readonly string Mp3FilePath = $"{AudioDirPath}/{Mp3FileName}";
    private readonly IAudioFactory mockAudioFactory;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IContentPathResolver mockAudioPathResolver;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IAudio mockAudio;
    private readonly IDisposable mockShutdownUnsubscriber;
    private readonly IPushReactable<DisposeAudioData> mockDisposeAudioReactable;
    private IReceiveSubscription? mockShutdownSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLoaderTests"/> class.
    /// </summary>
    public AudioLoaderTests()
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

        this.mockDisposeAudioReactable = Substitute.For<IPushReactable<DisposeAudioData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeAudioReactable().Returns(this.mockDisposeAudioReactable);
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockShutdownReactable);

        this.mockAudio = Substitute.For<IAudio>();
        this.mockAudio.Id.Returns(AudioId);
        this.mockAudio.FilePath.Returns(OggFilePath);

        this.mockAudioFactory = Substitute.For<IAudioFactory>();
        this.mockAudioFactory.Create(Arg.Any<string>(), Arg.Any<AudioBuffer>()).Returns(this.mockAudio);

        this.mockAudioPathResolver = Substitute.For<IContentPathResolver>();
        this.mockAudioPathResolver.ResolveDirPath().Returns(AudioDirPath);
        this.mockAudioPathResolver.ResolveFilePath(Arg.Any<string>()).Returns(OggFilePath);

        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(true);

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(OggFileExtension);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
    }

    #region Test Data
    /// <summary>
    /// Provides test data for the <see cref="AudioLoader.Load"/> method unit test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> LoadRootedPathTestData() =>
        new ()
        {
            { OggFilePath, OggFilePath },
            { Mp3FilePath, Mp3FilePath },
        };

    /// <summary>
    /// Provides test data for the <see cref="AudioLoader.Load"/> method unit test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string, string> LoadUnrootedPathTestData() =>
        new ()
        {
            { AudioContentName, OggFileExtension, OggFilePath },
            { AudioContentName, Mp3FileExtension, Mp3FilePath },
        };
    #endregion

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullAudioFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                null,
                this.mockReactableFactory,
                this.mockAudioPathResolver,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'audioFactory')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioFactory,
                null,
                this.mockAudioPathResolver,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullAudioPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioFactory,
                this.mockReactableFactory,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'audioPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioFactory,
                this.mockReactableFactory,
                this.mockAudioPathResolver,
                null,
                this.mockFile,
                this.mockPath);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioFactory,
                this.mockReactableFactory,
                this.mockAudioPathResolver,
                this.mockDirectory,
                null,
                this.mockPath);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioFactory,
                this.mockReactableFactory,
                this.mockAudioPathResolver,
                this.mockDirectory,
                this.mockFile,
                null);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Load_WithNullPathOrNameParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(null, AudioBuffer.Full);

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'pathOrName')");
    }

    [Fact]
    public void Load_WithEmptyPathOrNameParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(string.Empty, AudioBuffer.Full);

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'pathOrName')");
    }

    [Theory]
    [MemberData(nameof(LoadRootedPathTestData))]
    public void Load_WithRootedPath_LoadsContent(string rootedFilePath, string expectedFilePath)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(rootedFilePath, AudioBuffer.Full);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        actual.ShouldBeSameAs(this.mockAudio);
        this.mockPath.Received(1).IsPathRooted(expectedFilePath);

        // Is a rooted path so should not invoke these
        this.mockAudioPathResolver.DidNotReceive().ResolveDirPath();
        this.mockDirectory.DidNotReceive().Exists(Arg.Any<string>());
        this.mockDirectory.DidNotReceive().CreateDirectory(Arg.Any<string>());
        this.mockAudioPathResolver.DidNotReceive().ResolveFilePath(expectedFilePath);

        this.mockFile.Received(1).Exists(expectedFilePath);
        this.mockPath.Received(1).GetExtension(expectedFilePath);
        this.mockAudioFactory.Received(1).Create(expectedFilePath, AudioBuffer.Full);
    }

    [Theory]
    [MemberData(nameof(LoadUnrootedPathTestData))]
    public void Load_WithUnrootedPathAndWhenContentDirDoesNotExist_LoadsContent(
        string unrootedContentName,
        string fileExtension,
        string expectedFilePath)
    {
        // Arrange
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(false);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(fileExtension);
        this.mockAudioPathResolver.ResolveFilePath(Arg.Any<string>()).Returns(expectedFilePath);
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(false);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(unrootedContentName, AudioBuffer.Full);

        // Assert
        sut.TotalCachedItems.ShouldBe(1);
        actual.ShouldBeSameAs(this.mockAudio);
        this.mockPath.Received(1).IsPathRooted(AudioContentName);

        this.mockAudioPathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).Exists(AudioDirPath);
        this.mockAudioPathResolver.DidNotReceive().ResolveFilePath(expectedFilePath);
        this.mockDirectory.Received(1).Exists(AudioDirPath);
        this.mockFile.Received(1).Exists(expectedFilePath);
        this.mockPath.Received(1).GetExtension(expectedFilePath);
        this.mockAudioFactory.Received(1).Create(expectedFilePath, AudioBuffer.Full);
    }

    [Fact]
    public void Load_WhenContentFileDoesNotExist_ThrowException()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(OggFilePath, AudioBuffer.Full);

        // Assert
        var exception = act.ShouldThrow<FileNotFoundException>();
        exception.Message.ShouldBe($"The audio file does not exist.");
        exception.FileName.ShouldBe(OggFilePath);
    }

    [Fact]
    public void Load_WithInvalidContentFileNameExtension_ThrowException()
    {
        // Arrange
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(".txt");

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(OggFilePath, AudioBuffer.Full);

        // Assert
        var exception = act.ShouldThrow<LoadAudioException>();
        exception.Message.ShouldBe($"The file '{OggFilePath}' must be an audio file with the extension '{OggFileExtension}' or '{Mp3FileExtension}'.");
    }

    [Fact]
    public void Load_WithInvalidExtension_ThrowsException()
    {
        // Arrange
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(".txt");

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(OggFilePath, AudioBuffer.Full);

        // Assert
        var exception = act.ShouldThrow<LoadAudioException>();
        exception.Message.ShouldBe($"The file '{OggFilePath}' must be an audio file with the extension '{OggFileExtension}' or '{Mp3FileExtension}'.");
    }

    [Fact]
    public void Unload_WhenUnloadingUsingContentName_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Exists(OggFilePath).Returns(true);
        this.mockPath.GetExtension(OggFilePath).Returns(OggFileExtension);
        this.mockPath.GetFileNameWithoutExtension(AudioContentName).Returns(AudioContentName);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAudioPathResolver.ResolveFilePath(AudioContentName).Returns(OggFilePath);

        var sut = CreateSystemUnderTest();
        var audio = sut.Load(AudioContentName, AudioBuffer.Full);

        // Act
        sut.Unload(audio);

        // Assert
        sut.TotalCachedItems.ShouldBe(0);
    }

    [Fact]
    public void Unload_WhenUnloadingUsingFullDirectPath_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Exists(OggFilePath).Returns(true);
        this.mockPath.GetExtension(OggFilePath).Returns(OggFileExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockAudioPathResolver.ResolveFilePath(OggFilePath).Returns(OggFilePath);

        var sut = CreateSystemUnderTest();
        var audio = sut.Load(OggFilePath, AudioBuffer.Full);

        // Act
        sut.Unload(audio);

        // Assert
        sut.TotalCachedItems.ShouldBe(0);
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
        sut.Load(OggFileName, AudioBuffer.Full);
        this.mockShutdownSubscription.OnReceive();
        this.mockShutdownSubscription.OnReceive(); // Tests idempotent behavior for the shutdown process

        // Assert
        sut.TotalCachedItems.ShouldBe(0);
        this.mockDisposeAudioReactable.Received(1).Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = AudioId });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of a <see cref="AudioLoader"/> for testing purposes.
    /// </summary>
    /// <returns>The mocked audio loader instance used for testing.</returns>
    private AudioLoader CreateSystemUnderTest() => new (
        this.mockAudioFactory,
        this.mockReactableFactory,
        this.mockAudioPathResolver,
        this.mockDirectory,
        this.mockFile,
        this.mockPath);
}
