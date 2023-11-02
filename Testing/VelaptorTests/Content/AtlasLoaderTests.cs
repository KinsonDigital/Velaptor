// <copyright file="AtlasLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Graphics;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasLoader"/> class.
/// </summary>
public class AtlasLoaderTests
{
    private const string TextureExtension = ".png";
    private const string AtlasDataExtension = ".json";
    private const string DirPath = @"C:/Content/Atlas";
    private const string AtlasContentName = "test-atlas";
    private const string FakeJSONData = "fake-json-data";
    private const string AtlasImageFilePath = $"{DirPath}/{AtlasContentName}{TextureExtension}";
    private const string AtlasDataFilePath = $"{DirPath}/{AtlasContentName}{AtlasDataExtension}";
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly Mock<IAtlasDataFactory> mockAtlasDataFactory;
    private readonly Mock<IContentPathResolver> mockAtlasPathResolver;
    private readonly Mock<IJSONService> mockJSONService;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
    /// </summary>
    public AtlasLoaderTests()
    {
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
        this.mockAtlasDataFactory = new Mock<IAtlasDataFactory>();

        this.mockAtlasPathResolver = new Mock<IContentPathResolver>();
        this.mockAtlasPathResolver.Setup(m => m.ResolveDirPath()).Returns(DirPath);

        this.mockJSONService = new Mock<IJSONService>();

        this.mockDirectory = new Mock<IDirectory>();

        this.mockFile = new Mock<IFile>();
        this.mockFile.Setup(m => m.ReadAllText(AtlasDataFilePath))
            .Returns(FakeJSONData);
        this.mockFile.Setup(m => m.Exists(AtlasDataFilePath)).Returns(true);
        this.mockFile.Setup(m => m.Exists(AtlasImageFilePath)).Returns(true);

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetDirectoryName(AtlasImageFilePath)).Returns(DirPath);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AtlasImageFilePath)).Returns(AtlasContentName);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AtlasContentName)).Returns(AtlasContentName);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                null,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullAtlasDataFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
       _ = new AtlasLoader(
                this.mockTextureCache.Object,
                null,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'atlasDataFactory')");
    }

    [Fact]
    public void Ctor_WithNullAtlasDataPathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
       _ = new AtlasLoader(
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                null,
                this.mockJSONService.Object,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'atlasDataPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullJSONServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                null,
                this.mockDirectory.Object,
                this.mockFile.Object,
                this.mockPath.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'jsonService')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasLoader(
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
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
            _ = new AtlasLoader(
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
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
           _ = new AtlasLoader(
                this.mockTextureCache.Object,
                this.mockAtlasDataFactory.Object,
                this.mockAtlasPathResolver.Object,
                this.mockJSONService.Object,
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
    public void Load_WithInvalidFullFilePathExtensions_ThrowsException()
    {
        // Arrange
        const string extension = ".txt";
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load($"{DirPath}/{AtlasContentName}{extension}");

        // Assert
        act.Should().Throw<LoadAtlasException>()
            .WithMessage("When loading atlas data with fully qualified paths, the files must be a '.png' or '.json' extension.");
    }

    [Fact]
    public void Load_WhenContentDirPathDoesNotExist_CreateDirectory()
    {
        // Arrange
        this.mockFile.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string?>())).Returns(".png");
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockAtlasPathResolver.Setup(m => m.ResolveDirPath()).Returns(DirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockAtlasPathResolver.VerifyOnce(m => m.ResolveDirPath());
        this.mockDirectory.VerifyOnce(m => m.CreateDirectory(DirPath));
    }

    [Fact]
    public void Load_WhenUsingFullFilePath_LoadsAtlasData()
    {
        // Arrange
        var mockAtlasData = new Mock<IAtlasData>();

        var sut = CreateSystemUnderTest();

        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(TextureExtension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        var atlasData = MockAtlasJSONData().ToArray();
        MockAtlasDataFactory(mockAtlasData.Object, atlasData, DirPath, $"{AtlasContentName}");

        // Act
        var actual = sut.Load(AtlasImageFilePath);

        // Assert
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(AtlasImageFilePath), Times.Once);
        this.mockFile.Verify(m => m.ReadAllText(AtlasDataFilePath), Times.Once());
        this.mockJSONService.Verify(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData), Times.Once);
        this.mockAtlasDataFactory.Verify(m => m.Create(atlasData, DirPath, $"{AtlasContentName}"));
        actual.Should().BeSameAs(mockAtlasData.Object);
    }

    [Fact]
    public void Load_WhenAtlasJSONDataFileDoesNotExist_ThrowsException()
    {
        // Arrange
        const string contentName = "missing-json-file";
        const string invalidFilePath = $"{DirPath}/{contentName}{AtlasDataExtension}";
        var sut = CreateSystemUnderTest();

        var expected = $"The atlas data directory '{DirPath}' does not contain the";
        expected += $" required '{DirPath}/{contentName}{AtlasDataExtension}' atlas data file.";

        this.mockFile.Setup(m => m.Exists(invalidFilePath)).Returns(false);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(invalidFilePath)).Returns(contentName);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(AtlasDataExtension);
        this.mockPath.Setup(m => m.GetDirectoryName(invalidFilePath)).Returns(DirPath);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        // Act
        var act = () => sut.Load(invalidFilePath);

        // Assert
        act.Should().Throw<LoadAtlasException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Load_WhenAtlasImageFileDoesNotExist_ThrowsException()
    {
        // Arrange
        const string jsonContentName = "missing-file";
        const string imageContentName = "missing-file";
        const string validJSONFilePath = $"{DirPath}/{jsonContentName}{AtlasDataExtension}";
        const string invalidImageFilePath = $"{DirPath}/{imageContentName}{TextureExtension}";
        var sut = CreateSystemUnderTest();

        var expected = $"The atlas data directory '{DirPath}' does not contain the";
        expected += $" required '{DirPath}/{imageContentName}{TextureExtension}' atlas image file.";

        this.mockFile.Setup(m => m.Exists(validJSONFilePath)).Returns(true);
        this.mockFile.Setup(m => m.Exists(invalidImageFilePath)).Returns(false);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(invalidImageFilePath)).Returns(imageContentName);
        this.mockPath.Setup(m => m.GetExtension(It.IsAny<string>())).Returns(TextureExtension);
        this.mockPath.Setup(m => m.GetDirectoryName(invalidImageFilePath)).Returns(DirPath);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        // Act
        var act = () => sut.Load(invalidImageFilePath);

        // Assert
        act.Should().Throw<LoadAtlasException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Load_WhenUsingJustContentName_LoadsAtlasData()
    {
        // Arrange
        var mockAtlasData = new Mock<IAtlasData>();

        var sut = CreateSystemUnderTest();

        var atlasData = MockAtlasJSONData().ToArray();
        MockAtlasDataFactory(mockAtlasData.Object, atlasData, DirPath, AtlasContentName);

        // Act
        var actual = sut.Load(AtlasContentName);

        // Assert
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(AtlasContentName), Times.Once);
        this.mockAtlasPathResolver.Verify(m => m.ResolveDirPath(), Times.Once);
        this.mockFile.Verify(m => m.ReadAllText(AtlasDataFilePath), Times.Once());
        this.mockJSONService.Verify(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData), Times.Once);
        this.mockAtlasDataFactory.Verify(m => m.Create(atlasData, DirPath, AtlasContentName));
        actual.Should().BeSameAs(mockAtlasData.Object);
    }

    [Fact]
    public void Load_WhenNullJSONDataDeserializationResult_ThrowsException()
    {
        // Arrange
        this.mockJSONService.Setup(m => m.Deserialize<AtlasSubTextureData[]>(It.IsAny<string>()))
            .Returns(() => null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Load(AtlasContentName);

        // Assert
        act.Should().Throw<LoadContentException>()
            .WithMessage($"There was an issue deserializing the JSON atlas data file at '{AtlasDataFilePath}'.");
    }

    [Fact]
    public void Unload_WhenInvoked_UnloadsAtlas()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Unload(AtlasContentName);

        // Assert
        this.mockTextureCache.Verify(m => m.Unload(AtlasContentName));
    }
    #endregion

    /// <summary>
    /// Creates atlas sub texture data for the purpose of testing.
    /// </summary>
    /// <returns>The data to use for testing.</returns>
    private static AtlasSubTextureData[] CreateAtlasSubTextureData()
    {
        var items = new List<AtlasSubTextureData>();

        for (var i = 0; i < 2; i++)
        {
            items.Add(new AtlasSubTextureData
            {
                Name = $"sub-texture{i}",
                Bounds = new Rectangle(i + 1, i + 2, i + 3, i + 4),
                FrameIndex = i,
            });
        }

        return items.ToArray();
    }

    /// <summary>
    /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AtlasLoader CreateSystemUnderTest()
        => new (
            this.mockTextureCache.Object,
            this.mockAtlasDataFactory.Object,
            this.mockAtlasPathResolver.Object,
            this.mockJSONService.Object,
            this.mockDirectory.Object,
            this.mockFile.Object,
            this.mockPath.Object);

    /// <summary>
    /// Mocks the atlas data factory for the purpose of testing.
    /// </summary>
    /// <param name="dataIfInvokedCorrectly">The mock data to return if the mock is invoked correctly.</param>
    /// <param name="subTextureData">The sub texture data to return if the mock is invoked correctly.</param>
    /// <param name="dirPath">The test directory path.</param>
    /// <param name="atlasName">The test atlas name.</param>
    private void MockAtlasDataFactory(
        IAtlasData dataIfInvokedCorrectly,
        AtlasSubTextureData[] subTextureData,
        string dirPath,
        string atlasName)
    {
        this.mockAtlasDataFactory.Setup(m =>
                m.Create(subTextureData,
                    dirPath,
                    atlasName))
            .Returns(dataIfInvokedCorrectly);
    }

    /// <summary>
    /// Mocks the JSON data deserialization process.
    /// </summary>
    /// <returns>The test data to use if the mock is invoked correctly.</returns>
    private IEnumerable<AtlasSubTextureData> MockAtlasJSONData()
    {
        var data = CreateAtlasSubTextureData();

        this.mockJSONService.Setup(m => m.Deserialize<AtlasSubTextureData[]>(FakeJSONData))
            .Returns(data);

        return data;
    }
}
