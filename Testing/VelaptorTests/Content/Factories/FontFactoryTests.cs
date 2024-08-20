// <copyright file="FontFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Factories;

using System;
using FluentAssertions;
using NSubstitute;
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
    private readonly IFreeTypeService mockFreeTypeService;
    private readonly IFontStatsService mockFontStatsService;
    private readonly IFontAtlasService mockFontAtlasService;
    private readonly IItemCache<string, ITexture> mockTextureCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontFactoryTests"/> class.
    /// </summary>
    public FontFactoryTests()
    {
        this.mockFreeTypeService = Substitute.For<IFreeTypeService>();
        this.mockFontStatsService = Substitute.For<IFontStatsService>();
        this.mockFontAtlasService = Substitute.For<IFontAtlasService>();
        this.mockTextureCache = Substitute.For<IItemCache<string, ITexture>>();
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
                this.mockFontStatsService,
                this.mockFontAtlasService,
                this.mockTextureCache);
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
                this.mockFreeTypeService,
                null,
                this.mockFontAtlasService,
                this.mockTextureCache);
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
                this.mockFreeTypeService,
                this.mockFontStatsService,
                null,
                this.mockTextureCache);
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
                this.mockFreeTypeService,
                this.mockFontStatsService,
                this.mockFontAtlasService,
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
        var mockTexture = Substitute.For<ITexture>();

        var sut = new FontFactory(
            this.mockFreeTypeService,
            this.mockFontStatsService,
            this.mockFontAtlasService,
            this.mockTextureCache);

        IFont? actual = null;

        // Act
        var act = () =>
        {
            actual = sut.Create(
                mockTexture,
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
