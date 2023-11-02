﻿// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="TextureLoader"/> class.
/// </summary>
public class TextureLoaderTests
{
    private const string TextureExtension = ".png";
    private const string TextureDirPath = @"C:/textures";
    private const string TextureFileName = "test-texture";
    private const string TextureFilePath = $"{TextureDirPath}/{TextureFileName}{TextureExtension}";
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly Mock<IContentPathResolver> mockTexturePathResolver;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
    /// </summary>
    public TextureLoaderTests()
    {
        this.mockTexturePathResolver = new Mock<IContentPathResolver>();
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
        this.mockDirectory = new Mock<IDirectory>();

        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.Exists(TextureFilePath)).Returns(true);

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetExtension(TextureFilePath)).Returns(TextureExtension);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}")).Returns(TextureFileName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}{TextureExtension}")).Returns(TextureFileName);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            null,
            this.mockTexturePathResolver.Object,
            this.mockDirectory.Object,
            this.mockFile.Object,
            this.mockPath.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
            this.mockTextureCache.Object,
            null,
            this.mockDirectory.Object,
            this.mockFile.Object,
            this.mockPath.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'texturePathResolver')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
                this.mockTextureCache.Object,
                this.mockTexturePathResolver.Object,
                null,
                this.mockFile.Object,
                this.mockPath.Object);

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
            this.mockTextureCache.Object,
            this.mockTexturePathResolver.Object,
            this.mockDirectory.Object,
            null,
            this.mockPath.Object);

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
            this.mockTextureCache.Object,
            this.mockTexturePathResolver.Object,
            this.mockDirectory.Object,
            this.mockFile.Object,
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
        var mockTexture = new Mock<ITexture>();
        this.mockTextureCache.Setup(m => m.GetItem(TextureFilePath))
            .Returns(mockTexture.Object);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load(TextureFilePath);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(mockTexture.Object);
    }

    [Theory]
    [InlineData(TextureFileName, "")]
    [InlineData(TextureFileName, ".txt")]
    public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
    {
        // Arrange
        this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(TextureFilePath);
        var mockTexture = new Mock<ITexture>();

        this.mockTextureCache.Setup(m => m.GetItem(TextureFilePath))
            .Returns(mockTexture.Object);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load($"{contentName}{extension}");

        // Assert
        actual.Should().Be(mockTexture.Object);
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

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
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(".png");
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockTexturePathResolver.Setup(m => m.ResolveDirPath()).Returns(TextureDirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockTexturePathResolver.VerifyOnce(m => m.ResolveDirPath());
        this.mockDirectory.VerifyOnce(m => m.CreateDirectory(TextureDirPath));
    }

    [Fact]
    public void Load_WhenFileExistsButIsNotATextureContentFile_ThrowsException()
    {
        // Arrange
        const string extension = ".txt";
        const string filePathWithInvalidExtension = $"{TextureDirPath}/{TextureFileName}{extension}";
        this.mockFile.Setup(m => m.Exists(filePathWithInvalidExtension)).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(filePathWithInvalidExtension)).Returns(extension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

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
        this.mockTextureCache.Verify(m => m.Unload(TextureFilePath), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureLoader CreateSystemUnderTest()
        => new (this.mockTextureCache.Object,
            this.mockTexturePathResolver.Object,
            this.mockDirectory.Object,
            this.mockFile.Object,
            this.mockPath.Object);
}
