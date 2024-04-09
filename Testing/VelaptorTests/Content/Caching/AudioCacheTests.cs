// <copyright file="AudioCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
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
/// Tests the <see cref="AudioCache"/> class.
/// </summary>
public class AudioCacheTests
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const string AudioDirPath = "C:/audio";
    private const string AudioName = "test-audio";
    private const string OggAudioFilePath = $"{AudioDirPath}/{AudioName}{OggFileExtension}";
    private const string Mp3AudioFilePath = $"{AudioDirPath}/{AudioName}{Mp3FileExtension}";
    private readonly Mock<IAudioFactory> mockAudioFactory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;
    private readonly Mock<IPushReactable<DisposeAudioData>> mockDisposeReactable;
    private readonly Mock<IReactableFactory> mockReactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioCacheTests"/> class.
    /// </summary>
    public AudioCacheTests()
    {
        this.mockAudioFactory = new Mock<IAudioFactory>();
        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(OggAudioFilePath)).Returns(true);
        this.mockFile.Setup(m => m.Exists(Mp3AudioFilePath)).Returns(true);

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetExtension(OggAudioFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.GetExtension(Mp3AudioFilePath)).Returns(Mp3FileExtension);

        var mockShutDownUnsubscriber = new Mock<IDisposable>();
        mockShutDownUnsubscriber.Name = nameof(mockShutDownUnsubscriber);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Name = nameof(mockPushReactable);
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Returns(mockShutDownUnsubscriber.Object)
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
            });

        this.mockDisposeReactable = new Mock<IPushReactable<DisposeAudioData>>();
        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateDisposeAudioReactable()).Returns(this.mockDisposeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullAudioFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioCache(
                null,
                this.mockFile.Object,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'audioFactory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioCache(
                this.mockAudioFactory.Object,
                null,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioCache(
                this.mockAudioFactory.Object,
                this.mockFile.Object,
                null,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioCache(
                this.mockAudioFactory.Object,
                this.mockFile.Object,
                this.mockPath.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.GetItem($"{OggAudioFilePath}|Stream");

        // Act
        var actual = sut.TotalCachedItems;

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void CacheKeys_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { OggAudioFilePath }.AsReadOnly();
        var sut = CreateSystemUnderTest();
        sut.GetItem($"{OggAudioFilePath}|Stream");

        // Act
        var actual = sut.CacheKeys;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetItem_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'audioFilePath')");
    }

    [Fact]
    public void GetItem_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'audioFilePath')");
    }

    [Fact]
    public void GetItem_WithUnsupportedFileType_ThrowsException()
    {
        // Arrange
        const string dirPath = "C:/my-audio";
        const string audioName = "test-audio";
        const string invalidExtension = ".txt";
        const string audioFilePath = $"{dirPath}/{audioName}{invalidExtension}";
        var expected = $"Audio file type '{invalidExtension}' is not supported.";
        expected += $"{Environment.NewLine}Supported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

        this.mockPath.Setup(m => m.GetExtension(audioFilePath)).Returns(invalidExtension);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem($"{audioFilePath}|Stream");

        // Assert
        act.Should().Throw<LoadAudioException>()
            .WithMessage(expected);
    }

    [Fact]
    public void GetItem_WhenOggFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(OggAudioFilePath)).Returns(OggFileExtension);
        this.mockFile.Setup(m => m.Exists(OggAudioFilePath)).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem($"{OggAudioFilePath}|Stream");

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The '{OggFileExtension}' audio file does not exist.");
    }

    [Fact]
    public void GetItem_WhenMp3FileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockPath.Setup(m => m.GetExtension(Mp3AudioFilePath)).Returns(Mp3FileExtension);
        this.mockFile.Setup(m => m.Exists(Mp3AudioFilePath)).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem($"{Mp3AudioFilePath}|Stream");

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The '{Mp3FileExtension}' audio file does not exist.");
    }

    [Fact]
    public void GetItem_WhenGettingAudio_ReturnsAudio()
    {
        // Arrange
        var mockMp3Audio = new Mock<IAudio>();
        mockMp3Audio.Name = nameof(mockMp3Audio);
        mockMp3Audio.SetupGet(p => p.FilePath).Returns(Mp3AudioFilePath);
        mockMp3Audio.SetupGet(p => p.Id).Returns(123u);

        var mockOggAudio = new Mock<IAudio>();
        mockOggAudio.Name = nameof(mockOggAudio);
        mockOggAudio.SetupGet(p => p.FilePath).Returns(OggAudioFilePath);
        mockOggAudio.SetupGet(p => p.Id).Returns(456u);

        this.mockAudioFactory.Setup(m => m.Create(Mp3AudioFilePath, AudioBuffer.Stream))
            .Returns(mockMp3Audio.Object);
        this.mockAudioFactory.Setup(m => m.Create(OggAudioFilePath, AudioBuffer.Full))
            .Returns(mockOggAudio.Object);

        var sut = CreateSystemUnderTest();

        // Act
        var mp3 = sut.GetItem($"{Mp3AudioFilePath}|Stream");
        var ogg = sut.GetItem($"{OggAudioFilePath}|Full");

        // Assert
        mp3.Should().NotBeNull();
        ogg.Should().NotBeNull();

        ogg.Should().NotBeSameAs(mp3);

        mp3.Id.Should().Be(123u);
        ogg.Id.Should().Be(456u);

        mp3.FilePath.Should().Be(Mp3AudioFilePath);
        ogg.FilePath.Should().Be(OggAudioFilePath);
    }

    [Fact]
    public void Unload_WhenAudioToUnloadExists_RemovesAndDisposesOfAudio()
    {
        // Arrange
        var expected = new DisposeAudioData { AudioId = 123u };

        DisposeAudioData? actual = null;

        var mockAudio = new Mock<IAudio>();
        mockAudio.SetupGet(p => p.Id).Returns(123u);

        this.mockAudioFactory.Setup(m => m.Create(OggAudioFilePath, AudioBuffer.Full))
            .Returns(mockAudio.Object);

        this.mockDisposeReactable.Setup(m =>
                m.Push(It.IsAny<Guid>(), It.Ref<DisposeAudioData>.IsAny))
            .Callback((Guid _, in DisposeAudioData data) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");
                actual = data;
            });

        var sut = CreateSystemUnderTest();
        _ = sut.GetItem($"{OggAudioFilePath}|Full");

        // Act
        var act = () => sut.Unload(OggAudioFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable.VerifyOnce(m =>
            m.Push(PushNotifications.AudioDisposedId, It.Ref<DisposeAudioData>.IsAny));

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Unload_WhenAudioToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockAudio = new Mock<IAudio>();
        mockAudio.SetupGet(p => p.Id).Returns(123u);

        var sut = CreateSystemUnderTest();
        sut.GetItem($"{OggAudioFilePath}|Stream");

        // Act
        sut.Unload("non-existing-texture");

        // Assert
        this.mockDisposeReactable.VerifyNever(m =>
            m.Push(It.IsAny<Guid>(), It.Ref<DisposeAudioData>.IsAny));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AudioCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AudioCache CreateSystemUnderTest() =>
        new (this.mockAudioFactory.Object,
            this.mockFile.Object,
            this.mockPath.Object,
            this.mockReactableFactory.Object);
}
