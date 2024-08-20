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
using NSubstitute;
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
    private const string DirPath = "C:/Content/Atlas";
    private const string AtlasContentName = "test-atlas";
    private const string FakeJSONData = "fake-json-data";
    private const string AtlasImageFilePath = $"{DirPath}/{AtlasContentName}{TextureExtension}";
    private const string AtlasDataFilePath = $"{DirPath}/{AtlasContentName}{AtlasDataExtension}";
    private readonly IItemCache<string, ITexture> mockTextureCache;
    private readonly IAtlasDataFactory mockAtlasDataFactory;
    private readonly IContentPathResolver mockAtlasPathResolver;
    private readonly IJsonService mockJSONService;
    private readonly IDirectory mockDirectory;
    private readonly IFile mockFile;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasLoaderTests"/> class.
    /// </summary>
    public AtlasLoaderTests()
    {
        this.mockTextureCache = Substitute.For<IItemCache<string, ITexture>>();
        this.mockAtlasDataFactory = Substitute.For<IAtlasDataFactory>();

        this.mockAtlasPathResolver = Substitute.For<IContentPathResolver>();
        this.mockAtlasPathResolver.ResolveDirPath().Returns(DirPath);

        this.mockJSONService = Substitute.For<IJsonService>();
        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockFile = Substitute.For<IFile>();
        this.mockFile.ReadAllText(AtlasDataFilePath)
            .Returns(FakeJSONData);
        this.mockFile.Exists(AtlasDataFilePath).Returns(true);
        this.mockFile.Exists(AtlasImageFilePath).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetDirectoryName(AtlasImageFilePath).Returns(DirPath);
        this.mockPath.GetFileNameWithoutExtension(AtlasImageFilePath).Returns(AtlasContentName);
        this.mockPath.GetFileNameWithoutExtension(AtlasContentName).Returns(AtlasContentName);
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
                this.mockAtlasDataFactory,
                this.mockAtlasPathResolver,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockTextureCache,
                null,
                this.mockAtlasPathResolver,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockTextureCache,
                this.mockAtlasDataFactory,
                null,
                this.mockJSONService,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockTextureCache,
                this.mockAtlasDataFactory,
                this.mockAtlasPathResolver,
                null,
                this.mockDirectory,
                this.mockFile,
                this.mockPath);
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
                this.mockTextureCache,
                this.mockAtlasDataFactory,
                this.mockAtlasPathResolver,
                this.mockJSONService,
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
            _ = new AtlasLoader(
                this.mockTextureCache,
                this.mockAtlasDataFactory,
                this.mockAtlasPathResolver,
                this.mockJSONService,
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
           _ = new AtlasLoader(
                this.mockTextureCache,
                this.mockAtlasDataFactory,
                this.mockAtlasPathResolver,
                this.mockJSONService,
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
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

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
        this.mockFile.Exists(Arg.Any<string?>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string?>()).Returns(".png");
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockAtlasPathResolver.ResolveDirPath().Returns(DirPath);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Load("test-content");

        // Assert
        this.mockAtlasPathResolver.Received(1).ResolveDirPath();
        this.mockDirectory.Received(1).CreateDirectory(DirPath);
    }

    [Fact]
    public void Load_WhenUsingFullFilePath_LoadsAtlasData()
    {
        // Arrange
        var mockAtlasData = Substitute.For<IAtlasData>();

        var sut = CreateSystemUnderTest();

        this.mockPath.GetExtension(Arg.Any<string>()).Returns(TextureExtension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        var atlasData = MockAtlasJSONData().ToArray();
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(atlasData);

        this.mockAtlasDataFactory.Create(Arg.Any<AtlasSubTextureData[]>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(mockAtlasData);

        // Act
        var actual = sut.Load(AtlasImageFilePath);

        // Assert
        this.mockPath.Received(1).GetFileNameWithoutExtension(AtlasImageFilePath);
        this.mockFile.Received(1).ReadAllText(AtlasDataFilePath);
        this.mockJSONService.Received(1).Deserialize<AtlasSubTextureData[]>(FakeJSONData);
        this.mockAtlasDataFactory.Received(1).Create(atlasData, DirPath, AtlasContentName);
        actual.Should().BeSameAs(mockAtlasData);
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

        this.mockFile.Exists(invalidFilePath).Returns(false);
        this.mockPath.GetFileNameWithoutExtension(invalidFilePath).Returns(contentName);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(AtlasDataExtension);
        this.mockPath.GetDirectoryName(invalidFilePath).Returns(DirPath);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

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

        this.mockFile.Exists(validJSONFilePath).Returns(true);
        this.mockFile.Exists(invalidImageFilePath).Returns(false);
        this.mockPath.GetFileNameWithoutExtension(invalidImageFilePath).Returns(imageContentName);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(TextureExtension);
        this.mockPath.GetDirectoryName(invalidImageFilePath).Returns(DirPath);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

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
        var mockAtlasData = Substitute.For<IAtlasData>();

        var sut = CreateSystemUnderTest();

        var atlasData = MockAtlasJSONData().ToArray();
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(atlasData);

        this.mockAtlasDataFactory.Create(Arg.Any<AtlasSubTextureData[]>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(mockAtlasData);

        // Act
        var actual = sut.Load(AtlasContentName);

        // Assert
        this.mockPath.Received(1).GetFileNameWithoutExtension(AtlasContentName);
        this.mockAtlasPathResolver.Received(1).ResolveDirPath();
        this.mockFile.Received(1).ReadAllText(AtlasDataFilePath);
        this.mockJSONService.Received(1).Deserialize<AtlasSubTextureData[]>(FakeJSONData);
        this.mockAtlasDataFactory.Received(1).Create(atlasData, DirPath, AtlasContentName);
        actual.Should().BeSameAs(mockAtlasData);
    }

    [Fact]
    public void Load_WhenNullJSONDataDeserializationResult_ThrowsException()
    {
        // Arrange
        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(_ => null);

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
        this.mockTextureCache.Received(1).Unload(AtlasContentName);
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
            this.mockTextureCache,
            this.mockAtlasDataFactory,
            this.mockAtlasPathResolver,
            this.mockJSONService,
            this.mockDirectory,
            this.mockFile,
            this.mockPath);

    /// <summary>
    /// Mocks the JSON data deserialization process.
    /// </summary>
    /// <returns>The test data to use if the mock is invoked correctly.</returns>
    private IEnumerable<AtlasSubTextureData> MockAtlasJSONData()
    {
        var data = CreateAtlasSubTextureData();

        this.mockJSONService.Deserialize<AtlasSubTextureData[]>(Arg.Any<string>()).Returns(data);

        return data;
    }
}
