// <copyright file="SoundCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="SoundCache"/> class.
/// </summary>
public class SoundCacheTests
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const string SoundDirPath = @"C:/sounds";
    private const string SoundName = "test-sound";
    private const string OggSoundFilePath = $"{SoundDirPath}/{SoundName}{OggFileExtension}";
    private const string Mp3SoundFilePath = $"{SoundDirPath}/{SoundName}{Mp3FileExtension}";
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<ISoundFactory> mockSoundFactory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;
    private readonly Mock<IPushReactable<DisposeSoundData>> mockDisposeReactable;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveReactor? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundCacheTests"/> class.
    /// </summary>
    public SoundCacheTests()
    {
        this.mockSoundFactory = new Mock<ISoundFactory>();
        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(OggSoundFilePath)).Returns(true);
        this.mockFile.Setup(m => m.Exists(Mp3SoundFilePath)).Returns(true);

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetExtension(OggSoundFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.GetExtension(Mp3SoundFilePath)).Returns(Mp3FileExtension);

        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber.Name = nameof(this.mockShutDownUnsubscriber);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Name = nameof(mockPushReactable);
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns(this.mockShutDownUnsubscriber.Object)
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.shutDownReactor = reactor;
            });

        this.mockDisposeReactable = new Mock<IPushReactable<DisposeSoundData>>();
        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateDisposeSoundReactable()).Returns(this.mockDisposeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullSoundFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundCache(
                null,
                this.mockFile.Object,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'soundFactory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundCache(
                this.mockSoundFactory.Object,
                null,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundCache(
                this.mockSoundFactory.Object,
                this.mockFile.Object,
                null,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundCache(
                this.mockSoundFactory.Object,
                this.mockFile.Object,
                this.mockPath.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.GetItem(OggSoundFilePath);

        // Act
        var actual = sut.TotalCachedItems;

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void CacheKeys_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { OggSoundFilePath }.AsReadOnly();
        var sut = CreateSystemUnderTest();
        sut.GetItem(OggSoundFilePath);

        // Act
        var actual = sut.CacheKeys;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetItem_WithNullOrEmptySoundFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(filePath);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'soundFilePath')");
    }

    [Fact]
    public void GetItem_WithUnsupportedFileType_ThrowsException()
    {
        // Arrange
        const string dirPath = @"C:/my-sounds";
        const string soundName = "test-sound";
        const string invalidExtension = ".txt";
        const string soundFilePath = $"{dirPath}/{soundName}{invalidExtension}";
        var expected = $"Sound file type '{invalidExtension}' is not supported.";
        expected += $"{Environment.NewLine}Supported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

        this.mockPath.Setup(m => m.GetExtension(soundFilePath)).Returns(invalidExtension);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(soundFilePath);

        // Assert
        act.Should().Throw<LoadSoundException>()
            .WithMessage(expected);
    }

    [Fact]
    public void GetItem_WhenOggFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(OggSoundFilePath)).Returns(OggFileExtension);
        this.mockFile.Setup(m => m.Exists(OggSoundFilePath)).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(OggSoundFilePath);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The '{OggFileExtension}' sound file does not exist.");
    }

    [Fact]
    public void GetItem_WhenMp3FileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(Mp3SoundFilePath)).Returns(Mp3FileExtension);
        this.mockFile.Setup(m => m.Exists(Mp3SoundFilePath)).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(Mp3SoundFilePath);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The '{Mp3FileExtension}' sound file does not exist.");
    }

    [Fact]
    public void GetItem_WhenGettingSound_ReturnsSound()
    {
        // Arrange
        var mockMp3Sound = new Mock<ISound>();
        mockMp3Sound.Name = nameof(mockMp3Sound);
        mockMp3Sound.SetupGet(p => p.FilePath).Returns(Mp3SoundFilePath);
        mockMp3Sound.SetupGet(p => p.Id).Returns(123u);

        var mockOggSound = new Mock<ISound>();
        mockOggSound.Name = nameof(mockOggSound);
        mockOggSound.SetupGet(p => p.FilePath).Returns(OggSoundFilePath);
        mockOggSound.SetupGet(p => p.Id).Returns(456u);

        this.mockSoundFactory.Setup(m => m.Create(Mp3SoundFilePath))
            .Returns(mockMp3Sound.Object);
        this.mockSoundFactory.Setup(m => m.Create(OggSoundFilePath))
            .Returns(mockOggSound.Object);

        var sut = CreateSystemUnderTest();

        // Act
        var mp3Sound = sut.GetItem(Mp3SoundFilePath);
        var oggSound = sut.GetItem(OggSoundFilePath);

        // Assert
        mp3Sound.Should().NotBeNull();
        oggSound.Should().NotBeNull();

        oggSound.Should().NotBeSameAs(mp3Sound);

        mp3Sound.Id.Should().Be(123u);
        oggSound.Id.Should().Be(456u);

        mp3Sound.FilePath.Should().Be(Mp3SoundFilePath);
        oggSound.FilePath.Should().Be(OggSoundFilePath);
    }

    [Fact]
    public void Unload_WhenSoundToUnloadExists_RemovesAndDisposesOfSound()
    {
        // Arrange
        var expected = new DisposeSoundData { SoundId = 123u };

        DisposeSoundData? actual = null;

        var mockSound = new Mock<ISound>();
        mockSound.SetupGet(p => p.Id).Returns(123u);

        this.mockSoundFactory.Setup(m => m.Create(OggSoundFilePath))
            .Returns(mockSound.Object);

        this.mockDisposeReactable.Setup(m =>
                m.Push(It.Ref<DisposeSoundData>.IsAny, It.IsAny<Guid>()))
            .Callback((in DisposeSoundData data, Guid _) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");
                actual = data;
            });

        var sut = CreateSystemUnderTest();
        _ = sut.GetItem(OggSoundFilePath);

        // Act
        var act = () => sut.Unload(OggSoundFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable.VerifyOnce(m =>
            m.Push(It.Ref<DisposeSoundData>.IsAny, PushNotifications.SoundDisposedId));

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Unload_WhenSoundToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockSound = new Mock<ISound>();
        mockSound.SetupGet(p => p.Id).Returns(123u);

        var sut = CreateSystemUnderTest();
        sut.GetItem(OggSoundFilePath);

        // Act
        sut.Unload("non-existing-texture");

        // Assert
        this.mockDisposeReactable.VerifyNever(m =>
            m.Push(It.Ref<DisposeSoundData>.IsAny, It.IsAny<Guid>()));
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void PushReactable_WithShutDownNotification_ShutsDownCache()
    {
        // Arrange
        var mockSoundA = new Mock<ISound>();
        mockSoundA.SetupGet(p => p.Id).Returns(11u);
        mockSoundA.Name = nameof(mockSoundA);

        var mockSoundB = new Mock<ISound>();
        mockSoundB.SetupGet(p => p.Id).Returns(22u);
        mockSoundB.Name = nameof(mockSoundB);

        const string soundPathA = $"{SoundDirPath}/soundA{OggFileExtension}";
        const string soundPathB = $"{SoundDirPath}/soundB{OggFileExtension}";

        this.mockSoundFactory.Setup(m => m.Create(soundPathA))
            .Returns(mockSoundA.Object);

        this.mockSoundFactory.Setup(m => m.Create(soundPathB))
            .Returns(mockSoundB.Object);

        this.mockPath.Setup(m => m.GetExtension(soundPathA)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.GetExtension(soundPathB)).Returns(OggFileExtension);

        this.mockFile.Setup(m => m.Exists(soundPathA)).Returns(true);
        this.mockFile.Setup(m => m.Exists(soundPathB)).Returns(true);

        var sut = CreateSystemUnderTest();

        sut.GetItem(soundPathA);
        sut.GetItem(soundPathB);

        // Act
        this.shutDownReactor?.OnReceive();
        this.shutDownReactor?.OnReceive();

        // Assert
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockDisposeReactable.VerifyOnce(m => m.Unsubscribe(PushNotifications.SoundDisposedId));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SoundCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SoundCache CreateSystemUnderTest() =>
        new (this.mockSoundFactory.Object,
            this.mockFile.Object,
            this.mockPath.Object,
            this.mockReactableFactory.Object);
}
