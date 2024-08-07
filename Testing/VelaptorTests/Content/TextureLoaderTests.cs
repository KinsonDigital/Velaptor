// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
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
using Xunit;

/// <summary>
/// Tests the <see cref="TextureLoader"/> class.
/// </summary>
public class TextureLoaderTests
{
    private const string TextureExtension = ".png";
    private const string TextureFileName = "test-texture";
    private static readonly string AppDirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:/" : "/";
    private static readonly string ContentDirPath = $"{AppDirPath}/Content";
    private static readonly string TextureDirPath = $"{ContentDirPath}/Graphics";
    private static readonly string TextureFilePath = $"{TextureDirPath}/{TextureFileName}{TextureExtension}";
    private readonly IItemCache<string, ITexture> mockTextureCache;
    private readonly IContentPathResolver mockTexturePathResolver;
    private readonly IDirectory mockDirectory;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
    /// </summary>
    public TextureLoaderTests()
    {
        this.mockTexturePathResolver = Substitute.For<IContentPathResolver>();
        this.mockTextureCache = Substitute.For<IItemCache<string, ITexture>>();
        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockPath = Substitute.For<IPath>();
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
                this.mockPath);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new TextureLoader(
                this.mockTextureCache,
                this.mockTexturePathResolver,
                this.mockDirectory,
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
    public void Load_WhenLoadingContent_LoadsTexture()
    {
        // Arrange
        this.mockTexturePathResolver.ResolveFilePath(Arg.Any<string>()).Returns(TextureFilePath);

        var mockTexture = Substitute.For<ITexture>();

        this.mockTextureCache.GetItem(TextureFilePath).Returns(mockTexture);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Load("test-content.png");

        // Assert
        actual.Should().Be(mockTexture);
        this.mockTexturePathResolver.Received(1).ResolveFilePath("test-content.png");
        this.mockTextureCache.Received(1).GetItem(TextureFilePath);
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockDirectory.Exists(Arg.Any<string>()).Returns(false);
        this.mockPath.AltDirectorySeparatorChar.Returns(Path.AltDirectorySeparatorChar);
        this.mockTexturePathResolver.RootDirectoryPath.Returns(ContentDirPath);
        this.mockTexturePathResolver.ContentDirectoryName.Returns("Graphics");

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content.png");

        // Assert
        this.mockTexturePathResolver.Received(1).ResolveFilePath("test-content.png");
        this.mockDirectory.Received(1).CreateDirectory(TextureDirPath);
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
    private TextureLoader CreateSystemUnderTest() => new (this.mockTextureCache, this.mockTexturePathResolver, this.mockDirectory, this.mockPath);
}
