// <copyright file="AtlasDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasData"/> class.
/// </summary>
public class AtlasDataTests
{
    private const string DirPath = "C:/Content/Atlas";
    private const string AtlasName = "test-atlas";
    private const string TextureExtension = ".png";
    private const string JSONFileExtension = ".json";
    private const string AtlasImagePath = $"{DirPath}/{AtlasName}{TextureExtension}";
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly Mock<IPath> mockPath;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly AtlasSubTextureData[] atlasData;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasDataTests"/> class.
    /// </summary>
    public AtlasDataTests()
    {
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();

        this.mockDirectory = new Mock<IDirectory>();
        this.mockDirectory.Setup(m => m.Exists(It.IsAny<string?>())).Returns(true);

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetDirectoryName(DirPath)).Returns(DirPath);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AtlasName))
            .Returns(AtlasName);
        this.atlasData = new[]
        {
            new AtlasSubTextureData() // First frame of animating sub texture
            {
                Name = "test-texture",
                FrameIndex = 0,
                Bounds = new Rectangle(11, 22, 33, 44),
            },
            new AtlasSubTextureData() // Second frame of animating sub texture
            {
                Name = "test-texture",
                FrameIndex = 1,
                Bounds = new Rectangle(55, 66, 77, 88),
            },
            new AtlasSubTextureData() // Non animating sub texture
            {
                Name = "other-test-texture",
                FrameIndex = -1,
                Bounds = new Rectangle(111, 222, 333, 444),
            },
        };
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                null,
                this.mockDirectory.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                "atlas-name");
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                null,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                "atlas-name");
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullAtlasSubTextureDataParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                this.mockPath.Object,
                null,
                "dir-path",
                "atlas-name");
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'atlasSubTextureData')");
    }

    [Fact]
    public void Ctor_WithNullDirPath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                null,
                "atlas-name");
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'dirPath')");
    }

    [Fact]
    public void Ctor_WithEmptyDirPath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                string.Empty,
                "atlas-name");
        };

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'dirPath')");
    }

    [Fact]
    public void Ctor_WithNullAtlasName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                null);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'atlasName')");
    }

    [Fact]
    public void Ctor_WithEmptyAtlasName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTextureCache.Object,
                this.mockDirectory.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                string.Empty);
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'atlasName')");
    }

    [Fact]
    public void Ctor_WhenDirPathDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockDirectory.Setup(m => m.Exists(It.IsAny<string?>())).Returns(false);

        // Act
        var act = () => _ = CreateSystemUnderTest();

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage($"The directory '{DirPath}' does not exist.");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void SubTextureNames_WhenGettingValue_ReturnsCorrectFrameNameList()
    {
        // Arrange
        /* NOTE: In the constructor of this test, the items are added by frame index order 0, 1 then -1.
         * In the AtlasData ctor, it auto sorts the data by frame index from lowest to highest.  This is
         * why the order of names comes in the order shown in the expected array
        */
        var expected = new[]
        {
            "other-test-texture",
            "test-texture",
        };

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.SubTextureNames;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Name_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.Should().Be(AtlasName);
    }

    [Theory]
    [InlineData("C:/atlas-dir", "")]
    [InlineData("C:/atlas-dir", ".txt")]
    [InlineData("C:/atlas-dir/", ".txt")]
    [InlineData(@"C:\atlas-dir\", ".txt")]
    public void FilePath_WhenGettingValue_ReturnsCorrectResult(
        string dirPath,
        string extension)
    {
        // Arrange
        const string atlasName = "testAtlas";
        var atlasFileName = $"{atlasName}{extension}";
        const string expected = $"C:/atlas-dir/{atlasName}{TextureExtension}";

        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{atlasName}{extension}"))
            .Returns(atlasName);

        var sut = CreateSystemUnderTest(
            dirPath: dirPath,
            atlasName: atlasFileName);

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("C:/atlas-dir", "")]
    [InlineData("C:/atlas-dir", ".txt")]
    [InlineData("C:/atlas-dir/", ".txt")]
    [InlineData(@"C:\atlas-dir\", ".txt")]
    public void AtlasDataFilePath_WhenGettingValue_ReturnsCorrectResult(
        string dirPath,
        string extension)
    {
        // Arrange
        const string atlasName = "testAtlas";
        var atlasFileName = $"{atlasName}{extension}";
        const string expected = $"C:/atlas-dir/{atlasName}{JSONFileExtension}";

        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{atlasName}{extension}"))
            .Returns(atlasName);
        var sut = CreateSystemUnderTest(
            dirPath: dirPath,
            atlasName: atlasFileName);

        // Act
        var actual = sut.AtlasDataFilePath;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Width_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Width).Returns(123);
        this.mockTextureCache.Setup(m => m.GetItem(AtlasImagePath))
            .Returns(mockTexture.Object);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Width;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void Height_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Height).Returns(123);
        this.mockTextureCache.Setup(m => m.GetItem(AtlasImagePath))
            .Returns(mockTexture.Object);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Height;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Iterator_WhenGettingValueAtIndex_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new AtlasSubTextureData
        {
            Name = "test-texture",
            Bounds = new Rectangle(55, 66, 77, 88),
            FrameIndex = 1,
        };

        var sut = CreateSystemUnderTest();

        // Act
        /* NOTE: In the constructor of this test, the items are added by frame index order 0, 1 then -1.
         * In the AtlasData ctor, it auto sorts the data by frame index from lowest to highest.  This is
         * why the last item in the list of data is the correct item
        */
        var actual = sut[2];

        // Assert
        actual.Name.Should().Be(expected.Name);
        actual.FrameIndex.Should().Be(expected.FrameIndex);
        actual.Bounds.Should().Be(expected.Bounds);
    }

    [Fact]
    public void GetFrames_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetFrames(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'subTextureId')");
    }

    [Fact]
    public void GetFrames_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetFrames(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'subTextureId')");
    }

    [Fact]
    public void GetFrames_WhenSubTextureIdDoesNotExist_ThrowsException()
    {
        // Arrange
        const string subTextureId = "test-id";
        const string dirPath = "C:/atlas-dir";

        var data = new[]
        {
            new AtlasSubTextureData { Name = "itemA" },
        };

        var sut = CreateSystemUnderTest(subTextureData: data, dirPath: dirPath);

        // Act
        var act = () => sut.GetFrames(subTextureId);

        // Assert
        act.Should().Throw<AtlasException>()
            .WithMessage($"The sub-texture id '{subTextureId}' does not exist in the atlas.");
    }

    [Fact]
    public void GetFrames_WhenInvokedWithExistingSubTextureID_ReturnsCorrectFrameRectangle()
    {
        // Arrange
        var expectedItems = new[]
        {
            new AtlasSubTextureData() // First frame of Animating sub texture
            {
                Name = "test-texture",
                FrameIndex = 0,
                Bounds = new Rectangle(11, 22, 33, 44),
            },
            new AtlasSubTextureData() // Second frame of Animating sub texture
            {
                Name = "test-texture",
                FrameIndex = 1,
                Bounds = new Rectangle(55, 66, 77, 88),
            },
        };

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetFrames("test-texture");

        // Assert
        actual.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public void GetFrames_WithExistingSubTexture_ReturnsSubTextureData()
    {
        // Arrange
        var expected = new AtlasSubTextureData
        {
            Name = "test-texture",
            FrameIndex = 0,
            Bounds = new Rectangle(11, 22, 33, 44),
        };

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetFrames("test-texture")[0];

        // Assert
        actual.Name.Should().Be(expected.Name);
        actual.FrameIndex.Should().Be(expected.FrameIndex);
        actual.Bounds.Should().Be(expected.Bounds);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AtlasData"/> for testing purposes.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AtlasData CreateSystemUnderTest(
        AtlasSubTextureData[]? subTextureData = null,
        string? dirPath = DirPath,
        string? atlasName = AtlasName)
    {
        var data = subTextureData ?? this.atlasData;

        return new (
            this.mockTextureCache.Object,
            this.mockDirectory.Object,
            this.mockPath.Object,
            data,
            dirPath,
            atlasName);
    }
}
