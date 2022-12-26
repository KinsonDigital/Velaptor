// <copyright file="SoundCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching;

using System;
using System.IO;
using System.IO.Abstractions;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.ReactableData;
using Helpers;
using Velaptor;
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
    private readonly Mock<IReactable> mockReactable;
    private IReactor? shutDownReactor;

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

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Name = nameof(this.mockReactable);
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Returns(this.mockShutDownUnsubscriber.Object)
            .Callback<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.shutDownReactor = reactor;
            });
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullSoundFactoryParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new SoundCache(
                null,
                this.mockFile.Object,
                this.mockPath.Object,
                this.mockReactable.Object);
        }, "The parameter must not be null. (Parameter 'soundFactory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new SoundCache(
                this.mockSoundFactory.Object,
                null,
                this.mockPath.Object,
                this.mockReactable.Object);
        }, "The parameter must not be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new SoundCache(
                this.mockSoundFactory.Object,
                this.mockFile.Object,
                null,
                this.mockReactable.Object);
        }, "The parameter must not be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullReactorParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new SoundCache(
                this.mockSoundFactory.Object,
                this.mockFile.Object,
                this.mockPath.Object,
                null);
        }, "The parameter must not be null. (Parameter 'reactable')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var cache = CreateCache();
        cache.GetItem(OggSoundFilePath);

        // Act
        var actual = cache.TotalCachedItems;

        // Assert
        Assert.Equal(1, actual);
    }

    [Fact]
    public void CacheKeys_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var cache = CreateCache();
        cache.GetItem(OggSoundFilePath);

        // Act
        var actual = cache.CacheKeys;

        // Assert
        Assert.Single(actual);
        Assert.Equal(OggSoundFilePath, actual[0]);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetItem_WithNullOrEmptySoundFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var cache = CreateCache();

        // Act, & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            cache.GetItem(filePath);
        }, "The string parameter must not be null or empty. (Parameter 'soundFilePath')");
    }

    [Fact]
    public void GetItem_WithUnsupportedFileType_ThrowsException()
    {
        // Arrange
        const string dirPath = @"C:/my-sounds";
        const string soundName = "test-sound";
        const string invalidExtension = ".txt";
        const string soundFilePath = $"{dirPath}/{soundName}{invalidExtension}";
        var exceptionMsg = $"Sound file type '{invalidExtension}' is not supported.";
        exceptionMsg += $"{Environment.NewLine}Supported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

        this.mockPath.Setup(m => m.GetExtension(soundFilePath)).Returns(invalidExtension);

        var cache = CreateCache();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<LoadSoundException>(() =>
        {
            cache.GetItem(soundFilePath);
        }, exceptionMsg);
    }

    [Fact]
    public void GetItem_WhenOggFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(OggSoundFilePath)).Returns(OggFileExtension);
        this.mockFile.Setup(m => m.Exists(OggSoundFilePath)).Returns(false);
        var cache = CreateCache();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            cache.GetItem(OggSoundFilePath);
        }, $"The '{OggFileExtension}' sound file does not exist.");
    }

    [Fact]
    public void GetItem_WhenMp3FileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(Mp3SoundFilePath)).Returns(Mp3FileExtension);
        this.mockFile.Setup(m => m.Exists(Mp3SoundFilePath)).Returns(false);
        var cache = CreateCache();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            cache.GetItem(Mp3SoundFilePath);
        }, $"The '{Mp3FileExtension}' sound file does not exist.");
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

        var cache = CreateCache();

        // Act
        var mp3Sound = cache.GetItem(Mp3SoundFilePath);
        var oggSound = cache.GetItem(OggSoundFilePath);

        // Assert
        Assert.NotNull(mp3Sound);
        Assert.NotNull(oggSound);

        Assert.NotSame(mp3Sound, oggSound);

        Assert.Equal(123u, mp3Sound.Id);
        Assert.Equal(456u, oggSound.Id);

        Assert.Equal(Mp3SoundFilePath, mp3Sound.FilePath);
        Assert.Equal(OggSoundFilePath, oggSound.FilePath);
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

        this.mockReactable.Setup(m => m.PushData(It.Ref<DisposeSoundData>.IsAny, It.IsAny<Guid>()))
            .Callback((in DisposeSoundData data, Guid _) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");
                actual = data;
            });

        var cache = CreateCache();
        var unused = cache.GetItem(OggSoundFilePath);

        // Act
        var act = () => cache.Unload(OggSoundFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        cache.TotalCachedItems.Should().Be(0);
        this.mockReactable.VerifyOnce(m => m.PushData(It.Ref<DisposeSoundData>.IsAny, NotificationIds.SoundDisposedId));

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Unload_WhenSoundToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockSound = new Mock<ISound>();
        mockSound.SetupGet(p => p.Id).Returns(123u);

        var cache = CreateCache();
        cache.GetItem(OggSoundFilePath);

        // Act
        cache.Unload("non-existing-texture");

        // Assert
        this.mockReactable.VerifyNever(m => m.PushData(It.Ref<DisposeSoundData>.IsAny, It.IsAny<Guid>()));
    }

    [Fact]
    public void ShutDownNotification_WhenReceived_ShutsDownCache()
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

        var cache = CreateCache();

        cache.GetItem(soundPathA);
        cache.GetItem(soundPathB);

        // Act
        this.shutDownReactor?.OnNext();
        this.shutDownReactor?.OnNext();

        // Assert
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockReactable.VerifyOnce(m => m.Unsubscribe(NotificationIds.SoundDisposedId));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SoundCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SoundCache CreateCache() =>
        new (this.mockSoundFactory.Object,
            this.mockFile.Object,
            this.mockPath.Object,
            this.mockReactable.Object);
}
