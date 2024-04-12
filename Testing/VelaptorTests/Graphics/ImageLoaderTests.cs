// <copyright file="ImageLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using System.Drawing;
using System.IO.Abstractions;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="ImageLoader"/> class.
/// </summary>
public class ImageLoaderTests
{
    private readonly Mock<IPath> mockPath;
    private readonly Mock<IImageService> mockImageService;
    private readonly Mock<IContentPathResolver> mockTexturePathResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLoaderTests"/> class.
    /// </summary>
    public ImageLoaderTests()
    {
        this.mockPath = new Mock<IPath>();
        this.mockImageService = new Mock<IImageService>();
        this.mockTexturePathResolver = new Mock<IContentPathResolver>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(null, this.mockImageService.Object, this.mockTexturePathResolver.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(this.mockPath.Object, null, this.mockTexturePathResolver.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(this.mockPath.Object, this.mockImageService.Object, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'texturePathResolver')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadImage_WithAbsoluteFilePath_LoadsImageData()
    {
        // Arrange
        const string filePath = "test-file-path";
        var expected = new ImageData(new Color[2, 4], filePath);

        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockImageService.Setup(m => m.Load(filePath))
            .Returns<string>(_ => new ImageData(new Color[2, 4], filePath));
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LoadImage("test-file-path");

        // Assert
        actual.Should().Be(expected);
        this.mockImageService.VerifyOnce(m => m.Load(filePath));
    }

    [Fact]
    public void LoadImage_WithRelativeFilePath_LoadsImageData()
    {
        // Arrange
        const string filePath = "test-file-path";
        var expected = new ImageData(new Color[2, 4], filePath);

        this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
            .Returns(filePath);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(false);
        this.mockImageService.Setup(m => m.Load(filePath))
            .Returns<string>(_ => new ImageData(new Color[2, 4], filePath));
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LoadImage("test-file-path");

        // Assert
        actual.Should().Be(expected);
        this.mockImageService.VerifyOnce(m => m.Load(filePath));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ImageLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ImageLoader CreateSystemUnderTest()
        => new (this.mockPath.Object, this.mockImageService.Object, this.mockTexturePathResolver.Object);
}
