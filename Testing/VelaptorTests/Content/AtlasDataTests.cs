﻿// <copyright file="AtlasDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Drawing;
using System.IO.Abstractions;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasData"/> class.
/// </summary>
public class AtlasDataTests
{
    private const string DirPath = @"C:/Content/Atlas";
    private const string AtlasName = "test-atlas";
    private const string TextureExtension = ".png";
    private const string JSONFileExtension = ".json";
    private const string AtlasImagePath = $"{DirPath}/{AtlasName}{TextureExtension}";
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
    private readonly Mock<IPath> mockPath;
    private readonly AtlasSubTextureData[] subTextureData;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasDataTests"/> class.
    /// </summary>
    public AtlasDataTests()
    {
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.GetDirectoryName(DirPath)).Returns(DirPath);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(AtlasName))
            .Returns(AtlasName);
        this.subTextureData = new[]
        {
            new AtlasSubTextureData() // First frame of Animating sub texture
            {
                Name = "sub-texture",
                FrameIndex = 0,
                Bounds = new Rectangle(11, 22, 33, 44),
            },
            new AtlasSubTextureData() // Second frame of Animating sub texture
            {
                Name = "sub-texture",
                FrameIndex = 1,
                Bounds = new Rectangle(55, 66, 77, 88),
            },
            new AtlasSubTextureData() // Non animating sub texture
            {
                Name = "other-sub-texture",
                FrameIndex = -1,
                Bounds = new Rectangle(111, 222, 333, 444),
            },
        };
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureCache_ThrowsException()
    {
        // Act
        Action action = () =>
        {
            var unused = new AtlasData(
                null,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                "atlas-name");
        };

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameters must not be null or empty. (Parameter 'textureCache')");
    }

    [Fact]
    public void Ctor_WithNullPath_ThrowsException()
    {
        // Act
        Action action = () =>
        {
            var unused = new AtlasData(
                this.mockTextureCache.Object,
                null,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                "atlas-name");
        };

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameters must not be null or empty. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullSubTextureData_ThrowsException()
    {
        // Act
        Action action = () =>
        {
            var unused = new AtlasData(
                this.mockTextureCache.Object,
                this.mockPath.Object,
                null,
                "dir-path",
                "atlas-name");
        };

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameters must not be null or empty. (Parameter 'atlasSubTextureData')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullOrEmptyDirPath_ThrowsException(string dirPath)
    {
        // Act
        Action action = () =>
        {
            var unused = new AtlasData(
                this.mockTextureCache.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                dirPath,
                "atlas-name");
        };

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'dirPath')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullOrEmptyAtlasName_ThrowsException(string atlasName)
    {
        // Act
        Action action = () =>
        {
            var unused = new AtlasData(
                this.mockTextureCache.Object,
                this.mockPath.Object,
                Array.Empty<AtlasSubTextureData>(),
                "dir-path",
                atlasName);
        };

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameters must not be null or empty. (Parameter 'atlasName')");
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
            "other-sub-texture",
            "sub-texture",
        };

        var data = CreateAtlasData();

        // Act
        var actual = data.SubTextureNames;

        // Assert
        actual.Should().Equal(expected);
    }

    [Fact]
    public void Name_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var data = CreateAtlasData();

        // Act
        var actual = data.Name;

        // Assert
        actual.Should().Be(AtlasName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(".txt")]
    public void FilePath_WhenGettingValue_ReturnsCorrectResult(string extension)
    {
        // Arrange
        const string atlasName = "atlasWithExtension";
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{atlasName}{extension}"))
            .Returns(atlasName);
        var data = CreateAtlasData($"{atlasName}{extension}");

        // Act
        var actual = data.FilePath;

        // Assert
        actual.Should().Be($"{DirPath}/{atlasName}{TextureExtension}");
    }

    [Theory]
    [InlineData("")]
    [InlineData(".txt")]
    public void AtlasDataFilePath_WhenGettingValue_ReturnsCorrectResult(string extension)
    {
        // Arrange
        const string atlasName = "atlasWithExtension";
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{atlasName}{extension}"))
            .Returns(atlasName);
        var data = CreateAtlasData($"{atlasName}{extension}");

        // Act
        var actual = data.AtlasDataFilePath;

        // Assert
        actual.Should().Be($"{DirPath}/{atlasName}{JSONFileExtension}");
    }

    [Fact]
    public void Width_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Width).Returns(123);
        this.mockTextureCache.Setup(m => m.GetItem(AtlasImagePath))
            .Returns(mockTexture.Object);

        var data = CreateAtlasData();

        // Act
        var actual = data.Width;

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

        var data = CreateAtlasData();

        // Act
        var actual = data.Height;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Iterator_WhenGettingValueAtIndex_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new AtlasSubTextureData()
        {
            Name = "sub-texture",
            Bounds = new Rectangle(55, 66, 77, 88),
            FrameIndex = 1,
        };

        var data = CreateAtlasData();

        // Act
        /* NOTE: In the constructor of this test, the items are added by frame index order 0, 1 then -1.
         * In the AtlasData ctor, it auto sorts the data by frame index from lowest to highest.  This is
         * why the last item in the list of data is the correct item
        */
        var actual = data[2];

        // Assert
        actual.Name.Should().Be(expected.Name);
        actual.FrameIndex.Should().Be(expected.FrameIndex);
        actual.Bounds.Should().Be(expected.Bounds);
    }

    [Fact]
    public void GetFrames_WhenInvokedWithExistingSubTextureID_ReturnsCorrectFrameRectangle()
    {
        // Arrange
        var expectedItems = new[]
        {
            new AtlasSubTextureData() // First frame of Animating sub texture
            {
                Name = "sub-texture",
                FrameIndex = 0,
                Bounds = new Rectangle(11, 22, 33, 44),
            },
            new AtlasSubTextureData() // Second frame of Animating sub texture
            {
                Name = "sub-texture",
                FrameIndex = 1,
                Bounds = new Rectangle(55, 66, 77, 88),
            },
        };

        var data = CreateAtlasData();

        // Act
        var actual = data.GetFrames("sub-texture");

        // Assert
        actual.Length.Should().Be(expectedItems.Length);

        for (var i = 0; i < actual.Length; i++)
        {
            actual[i].Name.Should().Be(expectedItems[i].Name);
            actual[i].FrameIndex.Should().Be(expectedItems[i].FrameIndex);
            actual[i].Bounds.Should().Be(expectedItems[i].Bounds);
        }
    }

    [Fact]
    public void GetFrame_WhenSubTextureIDDoesNotExist_ThrowsException()
    {
        // Arrange
        var data = CreateAtlasData();

        // Act & Assert
        Action action = () => data.GetFrame("missing-texture");

        action.Should().Throw<Exception>()
            .WithMessage("The frame 'missing-texture' was not found in the atlas 'test-atlas'.");
    }

    [Fact]
    public void GetFrame_WithExistingSubTexture_ReturnsSubTextureData()
    {
        // Arrange
        var expected = new AtlasSubTextureData()
        {
            Name = "sub-texture",
            FrameIndex = 0,
            Bounds = new Rectangle(11, 22, 33, 44),
        };

        var data = CreateAtlasData();

        // Act
        var actual = data.GetFrame("sub-texture");

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
    private AtlasData CreateAtlasData(string? atlasName = null)
        => new (this.mockTextureCache.Object, this.mockPath.Object, this.subTextureData, DirPath, atlasName ?? AtlasName);
}
