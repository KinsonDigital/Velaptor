// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using FluentAssertions;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureLoader"/> class.
/// </summary>
public class TextureLoaderTests
{
    private const string TextureExtension = ".png";
    private const string TextureDirPath = "C:/textures";
    private const string TextureFileName = "test-texture";
    private const string TextureFilePath = $"{TextureDirPath}/{TextureFileName}{TextureExtension}";
    private readonly IItemCache<string, ITexture> mockTextureCache;
    private readonly IContentPathResolver mockTexturePathResolver;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
    /// </summary>
    public TextureLoaderTests()
    {
        this.mockTexturePathResolver = Substitute.For<IContentPathResolver>();
        this.mockTextureCache = Substitute.For<IItemCache<string, ITexture>>();
        this.mockDirectory = Substitute.For<IDirectory>();

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(TextureFilePath).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetExtension(TextureFilePath)
            .Returns(TextureExtension);
        this.mockPath.GetFileNameWithoutExtension($"{TextureFileName}")
            .Returns(TextureFileName);
        this.mockPath.GetFileNameWithoutExtension($"{TextureFileName}{TextureExtension}")
            .Returns(TextureFileName);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            null,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureCache,
            null,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'texturePathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
                this.mockTextureCache,
                this.mockTexturePathResolver,
                null,
                this.mockFile,
                this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureCache,
            this.mockTexturePathResolver,
            this.mockDirectory,
            null,
            this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new TextureLoader(
            this.mockTextureCache,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockFile,
            null);

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
        act.Should().Throw<ArgumentNullException>()
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
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void Load_WhenLoadingContentWithFullPath_LoadsTexture()
    {
        // Arrange
        var mockTexture = Substitute.For<ITexture>();
        this.mockTextureCache.GetItem(TextureFilePath)
            .Returns(mockTexture);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(TextureFilePath);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(mockTexture);
    }

    [Theory]
    [InlineData(TextureFileName, "")]
    [InlineData(TextureFileName, ".txt")]
    public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
    {
        // Arrange
        this.mockTexturePathResolver.ResolveFilePath(Arg.Any<string>())
            .Returns(TextureFilePath);
        var mockTexture = Substitute.For<ITexture>();

        this.mockTextureCache.GetItem(TextureFilePath)
            .Returns(mockTexture);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load($"{contentName}{extension}");

        // Assert
        actual.Should().Be(mockTexture);
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(TextureFilePath);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The texture file '{TextureFilePath}' does not exist.");
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string?>())
            .Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>())
            .Returns(".png");
        this.mockPath.IsPathRooted(Arg.Any<string?>())
            .Returns(false);
        this.mockTexturePathResolver.ResolveDirPath()
            .Returns(TextureDirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockTexturePathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).CreateDirectory(TextureDirPath);
    }

    [Fact]
    public void Load_WhenFileExistsButIsNotATextureContentFile_ThrowsException()
    {
        // Arrange
        const string extension = ".txt";
        const string filePathWithInvalidExtension = $"{TextureDirPath}/{TextureFileName}{extension}";
        this.mockFile.Exists(filePathWithInvalidExtension)
            .Returns(true);
        this.mockPath.GetExtension(filePathWithInvalidExtension)
            .Returns(extension);
        this.mockPath.IsPathRooted(Arg.Any<string?>())
            .Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(filePathWithInvalidExtension);

        // Assert
        act.Should().ThrowExactly<LoadTextureException>()
            .WithMessage($"The file '{filePathWithInvalidExtension}' must be a texture file with the extension '{TextureExtension}'.");
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsCachedTextures()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(TextureFilePath);

        // Assert
        this.mockTextureCache.Received(1).Unload(TextureFilePath);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureLoader CreateSystemUnderTest()
        => new (this.mockTextureCache,
            this.mockTexturePathResolver,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);
}
