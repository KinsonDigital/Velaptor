// <copyright file="AudioLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="AudioLoader"/> class.
/// </summary>
public class AudioLoaderTests
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const string AudioDirPath = @"C:\temp\Content\audio\";
    private const string AudioName = "test-audio";
    private readonly string oggFilePath;
    private readonly string mp3FilePath;
    private readonly Mock<IItemCache<string, IAudio>> mockAudioCache;
    private readonly Mock<IContentPathResolver> mockAudioPathResolver;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLoaderTests"/> class.
    /// </summary>
    public AudioLoaderTests()
    {
        this.oggFilePath = $"{AudioDirPath}{AudioName}{OggFileExtension}";
        this.mp3FilePath = $"{AudioDirPath}{AudioName}{Mp3FileExtension}";

        this.mockAudioCache = new Mock<IItemCache<string, IAudio>>();
        this.mockAudioPathResolver = new Mock<IContentPathResolver>();

        this.mockDirectory = new Mock<IDirectory>();
        this.mockFile = new Mock<IFile>();
        this.mockPath = new Mock<IPath>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullAudioCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                null,
                this.mockAudioPathResolver.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Act
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'audioCache')");
    }

    [Fact]
    public void Ctor_WithNullAudioPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioCache.Object,
                null,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'audioPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioCache.Object,
                this.mockAudioPathResolver.Object,
                null,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioLoader(
                this.mockAudioCache.Object,
                this.mockAudioPathResolver.Object,
                this.mockDirectory.Object,
                null,
                this.mockPath.Object);
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
            _ = new AudioLoader(
                this.mockAudioCache.Object,
                this.mockAudioPathResolver.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
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
        act.Should()
            .Throw<ArgumentNullException>()
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
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void Load_WithInvalidExtensionForFullFilePath_ThrowsException()
    {
        // Arrange
        const string invalidExtension = ".txt";
        const string invalidFilePath = $"{AudioDirPath}{AudioName}{invalidExtension}";
        this.mockFile.Setup(m => m.Exists(invalidFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(invalidFilePath)).Returns(invalidExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var expectedMsg = $"The file '{invalidFilePath}' must be a audio file with";
        expectedMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

        var loader = CreateSystemUnderTest();

        // Act
        var act = () => loader.Load(invalidFilePath);

        // Assert
        act.Should()
            .Throw<LoadAudioException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void Load_WhenAudioFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggFilePath)).Returns(false);

        const string expectedMsg = "The audio file does not exist.";

        var loader = CreateSystemUnderTest();

        // Act
        var act = () => loader.Load(this.oggFilePath);

        // Assert
        act.Should()
            .Throw<FileNotFoundException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(".ogg");
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockAudioPathResolver.Setup(m => m.ResolveDirPath()).Returns(AudioDirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockAudioPathResolver.VerifyOnce(m => m.ResolveDirPath());
        this.mockDirectory.VerifyOnce(m => m.CreateDirectory(AudioDirPath));
    }

    [Theory]
    [InlineData(AudioName, "")]
    [InlineData(AudioName, ".txt")]
    public void Load_WhenLoadingOggAudioByFileNameOnly_LoadsOggAudio(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockAudioPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();

        // Act
        loader.Load($"{contentName}{extension}");

        // Assert
        this.mockAudioCache.Verify(m => m.GetItem(this.oggFilePath), Times.Once);
        this.mockPath.VerifyOnce(m => m.IsPathRooted($"{contentName}{extension}"));
        this.mockAudioPathResolver.VerifyOnce(m => m.ResolveFilePath($"{contentName}{extension}"));
        this.mockFile.VerifyOnce(m => m.Exists(this.oggFilePath));
        this.mockPath.VerifyOnce(m => m.GetExtension(this.oggFilePath));
    }

    [Theory]
    [InlineData(AudioName, "")]
    [InlineData(AudioName, ".txt")]
    public void Load_WhenLoadingMp3AudioByFileNameOnly_LoadsMp3Audio(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(Mp3FileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockAudioPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(this.mp3FilePath);

        var loader = CreateSystemUnderTest();

        // Act
        loader.Load($"{contentName}{extension}");

        // Assert
        this.mockAudioCache.Verify(m => m.GetItem(this.mp3FilePath), Times.Once);
        this.mockPath.VerifyOnce(m => m.IsPathRooted($"{contentName}{extension}"));
        this.mockAudioPathResolver.VerifyOnce(m => m.ResolveFilePath($"{contentName}{extension}"));
        this.mockFile.VerifyOnce(m => m.Exists(this.mp3FilePath));
        this.mockPath.VerifyOnce(m => m.GetExtension(this.mp3FilePath));
    }

    [Fact]
    public void Unload_WhenUnloadingUsingContentName_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(this.oggFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AudioName)).Returns(AudioName);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockAudioPathResolver.Setup(m => m.ResolveFilePath(AudioName)).Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();
        loader.Load(AudioName);

        // Act
        loader.Unload(AudioName);

        // Assert
        this.mockAudioCache.Verify(m => m.Unload(this.oggFilePath), Times.Once);
    }

    [Fact]
    public void Unload_WhenUnloadingUsingFullDirectPath_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(this.oggFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockAudioPathResolver.Setup(m => m.ResolveFilePath(this.oggFilePath)).Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();
        loader.Load(this.oggFilePath);

        // Act
        loader.Unload(this.oggFilePath);

        // Assert
        this.mockAudioCache.Verify(m => m.Unload(this.oggFilePath), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of a <see cref="AudioLoader"/> for testing purposes.
    /// </summary>
    /// <returns>The mocked audio loader instance used for testing.</returns>
    private AudioLoader CreateSystemUnderTest() => new (
        this.mockAudioCache.Object,
        this.mockAudioPathResolver.Object,
        this.mockDirectory.Object,
        this.mockFile.Object,
        this.mockPath.Object);
}
