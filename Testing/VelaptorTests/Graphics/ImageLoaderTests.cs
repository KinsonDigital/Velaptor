// <copyright file="ImageLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using System.Drawing;
using System.IO.Abstractions;
using Shouldly;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="ImageLoader"/> class.
/// </summary>
public class ImageLoaderTests
{
    private readonly IPath mockPath;
    private readonly IImageService mockImageService;
    private readonly IContentPathResolver mockTexturePathResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLoaderTests"/> class.
    /// </summary>
    public ImageLoaderTests()
    {
        this.mockPath = Substitute.For<IPath>();
        this.mockImageService = Substitute.For<IImageService>();
        this.mockTexturePathResolver = Substitute.For<IContentPathResolver>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(null, this.mockImageService, this.mockTexturePathResolver);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(this.mockPath, null, this.mockTexturePathResolver);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullTexturePathResolverParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImageLoader(this.mockPath, this.mockImageService, null);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'texturePathResolver')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadImage_WithAbsoluteFilePath_LoadsImageData()
    {
        // Arrange
        const string filePath = "test-file-path";
        var expected = new ImageData(new Color[2, 4], filePath);

        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockImageService.Load(filePath)
            .Returns(_ => new ImageData(new Color[2, 4], filePath));
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LoadImage("test-file-path");

        // Assert
        actual.ShouldBe(expected);
        this.mockImageService.Received(1).Load(filePath);
    }

    [Fact]
    public void LoadImage_WithRelativeFilePath_LoadsImageData()
    {
        // Arrange
        const string filePath = "test-file-path";
        var expected = new ImageData(new Color[2, 4], filePath);

        this.mockTexturePathResolver.ResolveFilePath(Arg.Any<string>())
            .Returns(filePath);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(false);
        this.mockImageService.Load(filePath)
            .Returns(_ => new ImageData(new Color[2, 4], filePath));
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LoadImage("test-file-path");

        // Assert
        actual.ShouldBe(expected);
        this.mockImageService.Received(1).Load(filePath);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ImageLoader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ImageLoader CreateSystemUnderTest()
        => new (this.mockPath, this.mockImageService, this.mockTexturePathResolver);
}
