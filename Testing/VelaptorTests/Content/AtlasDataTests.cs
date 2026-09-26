// <copyright file="AtlasDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using NSubstitute;
using Shouldly;
using Velaptor.Content;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasData"/> class.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLocalFunction", Justification = "Improves readability")]
public class AtlasDataTests
{
    private const string AtlasName = "test-atlas";
    private const string AtlasImgExtension = ".png";
    private const string AtlasDataFileExtension = ".json";
    private static readonly string DirPath = Path.Combine("C:", "Content", "Atlas");
    private static readonly string AtlasImgFilePath = Path.Combine(DirPath, $"{AtlasName}{AtlasImgExtension}");
    private static readonly string AtlasDataFilePath = Path.Combine(DirPath, $"{AtlasName}{AtlasDataFileExtension}");
    private readonly ITexture mockTexture;
    private readonly IPath mockPath;
    private readonly IDirectory mockDirectory;
    private readonly AtlasSubTextureData[] atlasData;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasDataTests"/> class.
    /// </summary>
    public AtlasDataTests()
    {
        this.mockTexture = Substitute.For<ITexture>();
        this.mockDirectory = Substitute.For<IDirectory>();
        this.mockDirectory.Exists(Arg.Any<string?>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.GetDirectoryName(DirPath).Returns(DirPath);
        this.mockPath.GetFileNameWithoutExtension(AtlasName).Returns(AtlasName);
        this.mockPath.Combine(DirPath, AtlasName + AtlasDataFileExtension).Returns(AtlasDataFilePath);
        this.mockPath.Combine(DirPath, AtlasName + AtlasImgExtension).Returns(AtlasImgFilePath);

        this.atlasData =
        [
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
        ];
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                null,
                this.mockDirectory,
                this.mockPath,
                [],
                "dir-path",
                "atlas-name");
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                null,
                [],
                "dir-path",
                "atlas-name");
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                null,
                this.mockPath,
                [],
                "dir-path",
                "atlas-name");
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullAtlasSubTextureDataParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                this.mockPath,
                null,
                "dir-path",
                "atlas-name");
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlasSubTextureData')");
    }

    [Fact]
    public void Ctor_WithNullDirPath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                this.mockPath,
                [],
                null,
                "atlas-name");
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'dirPath')");
    }

    [Fact]
    public void Ctor_WithEmptyDirPath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                this.mockPath,
                [],
                string.Empty,
                "atlas-name");
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'dirPath')");
    }

    [Fact]
    public void Ctor_WithNullAtlasName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                this.mockPath,
                [],
                "dir-path",
                null);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'atlasName')");
    }

    [Fact]
    public void Ctor_WithEmptyAtlasName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasData(
                this.mockTexture,
                this.mockDirectory,
                this.mockPath,
                [],
                "dir-path",
                string.Empty);
        };

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'atlasName')");
    }

    [Fact]
    public void Ctor_WhenDirPathDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockDirectory.Exists(Arg.Any<string?>()).Returns(false);

        // Act
        var act = () => _ = CreateSystemUnderTest();

        // Assert
        var exception = Should.Throw<DirectoryNotFoundException>(act);
        exception.Message.ShouldBe($"The directory '{DirPath}' does not exist.");
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Name_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.ShouldBe(AtlasName);
    }

    [Fact]
    public void FilePath_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.ShouldBe(AtlasImgFilePath);
    }

    [Fact]
    public void AtlasDataFilePath_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AtlasDataFilePath;

        // Assert
        actual.ShouldBe(AtlasDataFilePath);
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
        actual.Name.ShouldBe(expected.Name);
        actual.FrameIndex.ShouldBe(expected.FrameIndex);
        actual.Bounds.ShouldBe(expected.Bounds);
    }

    [Fact]
    public void GetFrames_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetFrames(null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'subTextureId')");
    }

    [Fact]
    public void GetFrames_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetFrames(string.Empty);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'subTextureId')");
    }

    [Fact]
    public void GetFrames_WhenSubTextureIdDoesNotExist_ThrowsException()
    {
        // Arrange
        const string subTextureId = "test-id";

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetFrames(subTextureId);

        // Assert
        var exception = Should.Throw<AtlasException>(act);
        exception.Message.ShouldBe($"The sub-texture id '{subTextureId}' does not exist in the atlas.");
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
        actual.ShouldBeEquivalentTo(expectedItems);
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
        actual.Name.ShouldBe(expected.Name);
        actual.FrameIndex.ShouldBe(expected.FrameIndex);
        actual.Bounds.ShouldBe(expected.Bounds);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AtlasData"/> for testing purposes.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AtlasData CreateSystemUnderTest() =>
        new (this.mockTexture,
            this.mockDirectory,
            this.mockPath,
            this.atlasData,
            DirPath,
            AtlasName);
}
