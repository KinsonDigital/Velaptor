// <copyright file="AudioLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using FluentAssertions;
using NSubstitute;
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
    private readonly IItemCache<string, IAudio> mockAudioCache;
    private readonly IContentPathResolver mockAudioPathResolver;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLoaderTests"/> class.
    /// </summary>
    public AudioLoaderTests()
    {
        this.oggFilePath = $"{AudioDirPath}{AudioName}{OggFileExtension}";
        this.mp3FilePath = $"{AudioDirPath}{AudioName}{Mp3FileExtension}";

        this.mockAudioCache = Substitute.For<IItemCache<string, IAudio>>();
        this.mockAudioPathResolver = Substitute.For<IContentPathResolver>();

        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockFile = Substitute.For<IFile>();
        this.mockPath = Substitute.For<IPath>();
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
                this.mockAudioPathResolver,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockAudioCache,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockAudioCache,
                this.mockAudioPathResolver,
                null,
                this.mockFile,
                this.mockPath);
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
                this.mockAudioCache,
                this.mockAudioPathResolver,
                this.mockDirectory,
                null,
                this.mockPath);
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
                this.mockAudioCache,
                this.mockAudioPathResolver,
                this.mockDirectory,
                this.mockFile,
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
    public void Load_WithMissingDataSignifier_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load("test-content");

        // Assert
        act.Should().Throw<LoadAudioException>()
            .WithMessage("The audio file path must contain metadata.");
    }

    [Fact]
    public void Load_WithInvalidMetaData_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load("test-content|Invalid");

        // Assert
        act.Should().Throw<LoadAudioException>()
            .WithMessage("The audio buffer type could not be determined.");
    }

    [Fact]
    public void Load_WithInvalidExtensionForFullFilePath_ThrowsException()
    {
        // Arrange
        const string invalidExtension = ".txt";
        const string invalidFilePath = $"{AudioDirPath}{AudioName}{invalidExtension}";
        const string invalidFilePathWithMetaData = $"{invalidFilePath}|Stream";
        this.mockFile.Exists(invalidFilePath).Returns(true);
        this.mockPath.GetExtension(invalidFilePath).Returns(invalidExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        var expectedMsg = $"The file '{invalidFilePath}' must be a audio file with";
        expectedMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

        var loader = CreateSystemUnderTest();

        // Act
        var act = () => loader.Load(invalidFilePathWithMetaData);

        // Assert
        act.Should()
            .Throw<LoadAudioException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void Load_WhenAudioFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Exists(this.oggFilePath).Returns(false);

        const string expectedMsg = "The audio file does not exist.";

        var loader = CreateSystemUnderTest();

        // Act
        var act = () => loader.Load($"{this.oggFilePath}|Stream");

        // Assert
        act.Should()
            .Throw<FileNotFoundException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>()).Returns(".ogg");
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAudioPathResolver.ResolveDirPath().Returns(AudioDirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content|Stream");

        // Assert
        this.mockAudioPathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).CreateDirectory(AudioDirPath);
    }

    [Theory]
    [InlineData(AudioName, "")]
    [InlineData(AudioName, ".txt")]
    public void Load_WhenLoadingOggAudioByFileNameOnly_LoadsOggAudio(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>()).Returns(OggFileExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAudioPathResolver.ResolveFilePath(Arg.Any<string>())
            .Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();

        // Act
        loader.Load($"{contentName}{extension}|Stream");

        // Assert
        this.mockAudioCache.Received(1).GetItem($"{this.oggFilePath}|Stream");
        this.mockPath.Received(1).IsPathRooted($"{contentName}{extension}");
        this.mockAudioPathResolver.Received(1).ResolveFilePath($"{contentName}{extension}");
        this.mockFile.Received(1).Exists(this.oggFilePath);
        this.mockPath.Received(1).GetExtension(this.oggFilePath);
    }

    [Theory]
    [InlineData(AudioName, "")]
    [InlineData(AudioName, ".txt")]
    public void Load_WhenLoadingMp3AudioByFileNameOnly_LoadsMp3Audio(string contentName, string extension)
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>()).Returns(Mp3FileExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAudioPathResolver.ResolveFilePath(Arg.Any<string>())
            .Returns(this.mp3FilePath);

        var loader = CreateSystemUnderTest();

        // Act
        loader.Load($"{contentName}{extension}|Stream");

        // Assert
        this.mockAudioCache.Received(1).GetItem($"{this.mp3FilePath}|Stream");
        this.mockPath.Received(1).IsPathRooted($"{contentName}{extension}");
        this.mockAudioPathResolver.Received(1).ResolveFilePath($"{contentName}{extension}");
        this.mockFile.Received(1).Exists(this.mp3FilePath);
        this.mockPath.Received(1).GetExtension(this.mp3FilePath);
    }

    [Fact]
    public void Unload_WhenUnloadingUsingContentName_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Exists(this.oggFilePath).Returns(true);
        this.mockPath.GetExtension(this.oggFilePath).Returns(OggFileExtension);
        this.mockPath.GetFileNameWithoutExtension(AudioName).Returns(AudioName);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAudioPathResolver.ResolveFilePath(AudioName).Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();
        loader.Load($"{AudioName}|Stream");

        // Act
        loader.Unload(AudioName);

        // Assert
        this.mockAudioCache.Received(1).Unload(this.oggFilePath);
    }

    [Fact]
    public void Unload_WhenUnloadingUsingFullDirectPath_UnloadsAudio()
    {
        // Arrange
        this.mockFile.Exists(this.oggFilePath).Returns(true);
        this.mockPath.GetExtension(this.oggFilePath).Returns(OggFileExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockAudioPathResolver.ResolveFilePath(this.oggFilePath).Returns(this.oggFilePath);

        var loader = CreateSystemUnderTest();
        loader.Load($"{this.oggFilePath}|Stream");

        // Act
        loader.Unload(this.oggFilePath);

        // Assert
        this.mockAudioCache.Received(1).Unload(this.oggFilePath);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of a <see cref="AudioLoader"/> for testing purposes.
    /// </summary>
    /// <returns>The mocked audio loader instance used for testing.</returns>
    private AudioLoader CreateSystemUnderTest() => new (
        this.mockAudioCache,
        this.mockAudioPathResolver,
        this.mockDirectory,
        this.mockFile,
        this.mockPath);
}
