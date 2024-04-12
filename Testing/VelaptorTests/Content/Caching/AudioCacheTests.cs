// <copyright file="AudioCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
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
    private readonly IAudioFactory mockAudioFactory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IPushReactable<DisposeAudioData> mockDisposeReactable;
    private readonly IReactableFactory mockReactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioCacheTests"/> class.
    /// </summary>
    public AudioCacheTests()
    {
        this.mockAudioFactory = Substitute.For<IAudioFactory>();
        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(OggAudioFilePath).Returns(true);
        this.mockFile.Exists(Mp3AudioFilePath).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetExtension(OggAudioFilePath).Returns(OggFileExtension);
        this.mockPath.GetExtension(Mp3AudioFilePath).Returns(Mp3FileExtension);

        var mockPushReactable = Substitute.For<IPushReactable>();

        this.mockDisposeReactable = Substitute.For<IPushReactable<DisposeAudioData>>();
        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateDisposeAudioReactable().Returns(this.mockDisposeReactable);
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
                this.mockFile,
                this.mockPath,
                this.mockReactableFactory);
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
                this.mockAudioFactory,
                null,
                this.mockPath,
                this.mockReactableFactory);
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
                this.mockAudioFactory,
                this.mockFile,
                null,
                this.mockReactableFactory);
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
                this.mockAudioFactory,
                this.mockFile,
                this.mockPath,
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

        this.mockPath.GetExtension(audioFilePath).Returns(invalidExtension);

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
        this.mockPath.GetExtension(OggAudioFilePath).Returns(OggFileExtension);
        this.mockFile.Exists(OggAudioFilePath).Returns(false);
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
        this.mockPath.GetExtension(Mp3AudioFilePath).Returns(Mp3FileExtension);
        this.mockFile.Exists(Mp3AudioFilePath).Returns(false);
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
        var mockMp3Audio = Substitute.For<IAudio>();
        mockMp3Audio.FilePath.Returns(Mp3AudioFilePath);
        mockMp3Audio.Id.Returns(123u);

        var mockOggAudio = Substitute.For<IAudio>();
        mockOggAudio.FilePath.Returns(OggAudioFilePath);
        mockOggAudio.Id.Returns(456u);

        this.mockAudioFactory.Create(Mp3AudioFilePath, AudioBuffer.Stream).Returns(mockMp3Audio);
        this.mockAudioFactory.Create(OggAudioFilePath, AudioBuffer.Full).Returns(mockOggAudio);

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
        var mockAudio = Substitute.For<IAudio>();
        mockAudio.Id.Returns(123u);

        this.mockAudioFactory.Create(OggAudioFilePath, AudioBuffer.Full).Returns(mockAudio);

        var sut = CreateSystemUnderTest();
        _ = sut.GetItem($"{OggAudioFilePath}|Full");

        // Act
        var act = () => sut.Unload(OggAudioFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable.Received(1)
            .Push(PushNotifications.AudioDisposedId, Arg.Is<DisposeAudioData>(data => data.AudioId == 123u));
    }

    [Fact]
    public void Unload_WhenAudioToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockAudio = Substitute.For<IAudio>();
        mockAudio.Id.Returns(123u);

        var sut = CreateSystemUnderTest();
        sut.GetItem($"{OggAudioFilePath}|Stream");

        // Act
        sut.Unload("non-existing-texture");

        // Assert
        this.mockDisposeReactable.DidNotReceive()
            .Push(Arg.Any<Guid>(), Arg.Is<DisposeAudioData>(data => data.AudioId == 123u));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AudioCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AudioCache CreateSystemUnderTest() =>
        new (this.mockAudioFactory,
            this.mockFile,
            this.mockPath,
            this.mockReactableFactory);
}
