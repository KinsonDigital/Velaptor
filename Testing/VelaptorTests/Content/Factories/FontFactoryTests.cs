// <copyright file="FontFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Factories;

using System;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Factories;
using Velaptor.Content.Fonts;
using Velaptor.Content.Fonts.Services;
using Velaptor.NativeInterop.Services;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FontFactory"/> class.
/// </summary>
public class FontFactoryTests
{
    private readonly Mock<IFreeTypeService> mockFreeTypeService;
    private readonly Mock<IFontStatsService> mockFontStatsService;
    private readonly Mock<IFontAtlasService> mockFontAtlasService;
    private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontFactoryTests"/> class.
    /// </summary>
    public FontFactoryTests()
    {
        this.mockFreeTypeService = new Mock<IFreeTypeService>();
        this.mockFontStatsService = new Mock<IFontStatsService>();
        this.mockFontAtlasService = new Mock<IFontAtlasService>();
        this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullFontServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontFactory(
                null,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'freeTypeService')");
    }

    [Fact]
    public void Ctor_WithNullFontStatsServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontFactory(
                this.mockFreeTypeService.Object,
                null,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontStatsService')");
    }

    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontFactory(
                this.mockFreeTypeService.Object,
                this.mockFontStatsService.Object,
                null,
                this.mockTextureCache.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullTextureCacheParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontFactory(
                this.mockFreeTypeService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureCache')");
    }
    #endregion

    #region Method Tests

    [Fact]
    public void Create_WhenInvoked_ReturnsCorrectResultWithoutThrowingException()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();

        var sut = new FontFactory(
            this.mockFreeTypeService.Object,
            this.mockFontStatsService.Object,
            this.mockFontAtlasService.Object,
            this.mockTextureCache.Object);

        IFont? actual = null;

        // Act
        var act = () =>
        {
            actual = sut.Create(
                mockTexture.Object,
                "test-name",
                "test-path",
                123u,
                true,
                []);
        };

        // Assert
        act.Should().NotThrow();
        actual.Should().NotBeNull();
    }
    #endregion
}
