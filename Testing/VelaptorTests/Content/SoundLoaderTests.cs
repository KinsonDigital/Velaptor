// <copyright file="SoundLoaderTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SoundLoader"/> class.
/// </summary>
public class SoundLoaderTests
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const string SoundDirPath = @"C:\temp\Content\Sounds\";
    private const string SoundName = "test-sound";
    private readonly string oggSoundFilePath;
    private readonly string mp3SoundFilePath;
    private readonly Mock<IItemCache<string, IAudio>> mockSoundCache;
    private readonly Mock<IContentPathResolver> mockSoundPathResolver;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundLoaderTests"/> class.
    /// </summary>
    public SoundLoaderTests()
    {
        this.oggSoundFilePath = $"{SoundDirPath}{SoundName}{OggFileExtension}";
        this.mp3SoundFilePath = $"{SoundDirPath}{SoundName}{Mp3FileExtension}";

        this.mockSoundCache = new Mock<IItemCache<string, IAudio>>();
        this.mockSoundPathResolver = new Mock<IContentPathResolver>();

        this.mockDirectory = new Mock<IDirectory>();
        this.mockFile = new Mock<IFile>();
        this.mockPath = new Mock<IPath>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullSoundCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundLoader(
                null,
                this.mockSoundPathResolver.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Act
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'soundCache')");
    }

    [Fact]
    public void Ctor_WithNullSoundPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundLoader(
                this.mockSoundCache.Object,
                null,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'soundPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundLoader(
                this.mockSoundCache.Object,
                this.mockSoundPathResolver.Object,
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
            _ = new SoundLoader(
                this.mockSoundCache.Object,
                this.mockSoundPathResolver.Object,
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
            _ = new SoundLoader(
                this.mockSoundCache.Object,
                this.mockSoundPathResolver.Object,
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
        var sut = CreateSoundLoader();

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
        var sut = CreateSoundLoader();

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
        var invalidSoundFilePath = $"{SoundDirPath}{SoundName}{invalidExtension}";
        this.mockFile.Setup(m => m.Exists(invalidSoundFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(invalidSoundFilePath)).Returns(invalidExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var expectedMsg = $"The file '{invalidSoundFilePath}' must be a sound file with";
        expectedMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

        var loader = CreateSoundLoader();

        // Act
        var act = () => loader.Load(invalidSoundFilePath);

        // Assert
        act.Should()
            .Throw<LoadSoundException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void Load_WhenSoundFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(false);

        var expectedMsg = "The sound file does not exist.";

        var loader = CreateSoundLoader();

        // Act
        var act = () => loader.Load(this.oggSoundFilePath);

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
        this.mockSoundPathResolver.Setup(m => m.ResolveDirPath()).Returns(SoundDirPath);

        var sut = CreateSoundLoader();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockSoundPathResolver.VerifyOnce(m => m.ResolveDirPath());
        this.mockDirectory.VerifyOnce(m => m.CreateDirectory(SoundDirPath));
    }

    [Theory]
    [InlineData(SoundName, "")]
    [InlineData(SoundName, ".txt")]
    public void Load_WhenLoadingOggSoundByFileNameOnly_LoadsOggSound(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(this.oggSoundFilePath);

        var loader = CreateSoundLoader();

        // Act
        loader.Load($"{contentName}{extension}");

        // Assert
        this.mockSoundCache.Verify(m => m.GetItem(this.oggSoundFilePath), Times.Once);
        this.mockPath.VerifyOnce(m => m.IsPathRooted($"{contentName}{extension}"));
        this.mockSoundPathResolver.VerifyOnce(m => m.ResolveFilePath($"{contentName}{extension}"));
        this.mockFile.VerifyOnce(m => m.Exists(this.oggSoundFilePath));
        this.mockPath.VerifyOnce(m => m.GetExtension(this.oggSoundFilePath));
    }

    [Theory]
    [InlineData(SoundName, "")]
    [InlineData(SoundName, ".txt")]
    public void Load_WhenLoadingMp3SoundByFileNameOnly_LoadsMp3Sound(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(Mp3FileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(this.mp3SoundFilePath);

        var loader = CreateSoundLoader();

        // Act
        loader.Load($"{contentName}{extension}");

        // Assert
        this.mockSoundCache.Verify(m => m.GetItem(this.mp3SoundFilePath), Times.Once);
        this.mockPath.VerifyOnce(m => m.IsPathRooted($"{contentName}{extension}"));
        this.mockSoundPathResolver.VerifyOnce(m => m.ResolveFilePath($"{contentName}{extension}"));
        this.mockFile.VerifyOnce(m => m.Exists(this.mp3SoundFilePath));
        this.mockPath.VerifyOnce(m => m.GetExtension(this.mp3SoundFilePath));
    }

    [Fact]
    public void Unload_WhenUnloadingUsingContentName_UnloadsSound()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(SoundName)).Returns(SoundName);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(SoundName)).Returns(this.oggSoundFilePath);

        var loader = CreateSoundLoader();
        loader.Load(SoundName);

        // Act
        loader.Unload(SoundName);

        // Assert
        this.mockSoundCache.Verify(m => m.Unload(this.oggSoundFilePath), Times.Once);
    }

    [Fact]
    public void Unload_WhenUnloadingUsingFullDirectPath_UnloadsSound()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(this.oggSoundFilePath)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(this.oggSoundFilePath)).Returns(OggFileExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockSoundPathResolver.Setup(m => m.ResolveFilePath(this.oggSoundFilePath)).Returns(this.oggSoundFilePath);

        var loader = CreateSoundLoader();
        loader.Load(this.oggSoundFilePath);

        // Act
        loader.Unload(this.oggSoundFilePath);

        // Assert
        this.mockSoundCache.Verify(m => m.Unload(this.oggSoundFilePath), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of a <see cref="SoundLoader"/> for testing purposes.
    /// </summary>
    /// <returns>The mockSound loader instance used for testing.</returns>
    private SoundLoader CreateSoundLoader() => new (
        this.mockSoundCache.Object,
        this.mockSoundPathResolver.Object,
        this.mockDirectory.Object,
        this.mockFile.Object,
        this.mockPath.Object);
}
