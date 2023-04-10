// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
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
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
    /// </summary>
    public TextureLoaderTests()
    {
        this.mockTexturePathResolver = new Mock<IContentPathResolver>();
        this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(TextureFileName))
            .Returns(TextureFilePath);

        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();

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
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureLoader(
                null,
                this.mockTexturePathResolver.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        }, "The parameter must not be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureLoader(
                this.mockTextureCache.Object,
                null,
                this.mockFile.Object,
                this.mockPath.Object);
        }, "The parameter must not be null. (Parameter 'texturePathResolver')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureLoader(
                this.mockTextureCache.Object,
                this.mockTexturePathResolver.Object,
                null,
                this.mockPath.Object);
        }, "The parameter must not be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureLoader(
                this.mockTextureCache.Object,
                this.mockTexturePathResolver.Object,
                this.mockFile.Object,
                null);
        }, "The parameter must not be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
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
        Assert.NotNull(actual);
        Assert.Same(mockTexture.Object, actual);
    }

    [Theory]
    [InlineData(TextureFileName, "")]
    [InlineData(TextureFileName, ".txt")]
    public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();

        this.mockTextureCache.Setup(m => m.GetItem(TextureFilePath))
            .Returns(mockTexture.Object);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}")).Returns(contentName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}")).Returns(contentName);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load($"{contentName}{extension}");

        // Assert
        Assert.NotNull(actual);
        Assert.Same(mockTexture.Object, actual);
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            sut.Load(TextureFilePath);
        }, $"The texture file '{TextureFilePath}' does not exist.");
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

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<LoadTextureException>(() =>
        {
            sut.Load(filePathWithInvalidExtension);
        }, $"The file '{filePathWithInvalidExtension}' must be a texture file with the extension '{TextureExtension}'.");
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
            this.mockFile.Object,
            this.mockPath.Object);
}
