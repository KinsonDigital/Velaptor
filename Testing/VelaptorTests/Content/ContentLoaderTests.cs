// <copyright file="ContentLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Xunit;

namespace VelaptorTests.Content;

/// <summary>
/// Tests the <see cref="ContentLoader"/> class.
/// </summary>
public class ContentLoaderTests
{
    private const string TextureContentName = "test-texture";
    private const string AtlasContentName = "test-atlas";
    private const string SoundContentName = "test-sound";
    private const string FontContentName = "test-font";
    private readonly Mock<ILoader<ITexture>> mockTextureLoader;
    private readonly Mock<ILoader<ISound>> mockSoundLoader;
    private readonly Mock<ILoader<IAtlasData>> mockAtlasLoader;
    private readonly Mock<ILoader<IFont>> mockFontLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentLoaderTests"/> class.
    /// </summary>
    public ContentLoaderTests()
    {
        this.mockTextureLoader = new Mock<ILoader<ITexture>>();
        this.mockSoundLoader = new Mock<ILoader<ISound>>();
        this.mockSoundLoader = new Mock<ILoader<ISound>>();
        this.mockAtlasLoader = new Mock<ILoader<IAtlasData>>();
        this.mockFontLoader = new Mock<ILoader<IFont>>();
    }

    #region Method Tests
    [Fact]
    public void LoadTexture_WhenLoadingTextures_LoadsTexture()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.LoadTexture(TextureContentName);

        // Assert
        this.mockTextureLoader.Verify(m => m.Load(TextureContentName), Times.Once());
    }

    [Fact]
    public void LoadSound_WhenLoadingSounds_LoadsSound()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.LoadSound(SoundContentName);

        // Assert
        this.mockSoundLoader.Verify(m => m.Load(SoundContentName), Times.Once());
    }

    [Fact]
    public void LoadAtlas_WhenLoadingAtlasData_LoadsAtlasData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.LoadAtlas(AtlasContentName);

        // Assert
        this.mockAtlasLoader.Verify(m => m.Load(AtlasContentName), Times.Once());
    }

    [Fact]
    public void LoadFont_WhenLoadingFonts_LoadsFont()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.LoadFont(FontContentName, 12);

        // Assert
        this.mockFontLoader.Verify(m => m.Load($"{FontContentName}|size:12"), Times.Once());
    }

    [Fact]
    public void UnloadTexture_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UnloadTexture(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'content')");
    }

    [Fact]
    public void UnloadTexture_WhenUnloadingTextures_UnloadsTexture()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.FilePath).Returns(TextureContentName);

        this.mockTextureLoader.Setup(m => m.Load(TextureContentName)).Returns(mockTexture.Object);

        var sut = CreateSystemUnderTest();
        var texture = sut.LoadTexture(TextureContentName);

        // Act
        sut.UnloadTexture(texture);

        // Assert
        this.mockTextureLoader.Verify(m => m.Unload(TextureContentName), Times.Once());
    }

    [Fact]
    public void UnloadSound_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UnloadSound(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'content')");
    }

    [Fact]
    public void UnloadSound_WhenUnloadingSounds_UnloadsSound()
    {
        // Arrange
        var mockSound = new Mock<ISound>();
        mockSound.SetupGet(p => p.FilePath).Returns(SoundContentName);

        this.mockSoundLoader.Setup(m => m.Load(SoundContentName)).Returns(mockSound.Object);

        var sut = CreateSystemUnderTest();
        var sound = sut.LoadSound(SoundContentName);

        // Act
        sut.UnloadSound(sound);

        // Assert
        this.mockSoundLoader.Verify(m => m.Unload(SoundContentName), Times.Once());
    }

    [Fact]
    public void UnloadAtlas_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UnloadAtlas(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'content')");
    }

    [Fact]
    public void UnloadAtlas_WhenUnloadingAtlasData_UnloadsAtlasData()
    {
        // Arrange
        var mockAtlasData = new Mock<IAtlasData>();
        mockAtlasData.SetupGet(p => p.FilePath).Returns(AtlasContentName);

        this.mockAtlasLoader.Setup(m => m.Load(AtlasContentName)).Returns(mockAtlasData.Object);

        var sut = CreateSystemUnderTest();
        var atlasData = sut.LoadAtlas(AtlasContentName);

        // Act
        sut.UnloadAtlas(atlasData);

        // Assert
        this.mockAtlasLoader.Verify(m => m.Unload(AtlasContentName), Times.Once());
    }

    [Fact]
    public void UnloadFont_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UnloadFont(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'content')");
    }

    [Fact]
    public void UnloadFont_WhenUnloadingFonts_UnloadsFont()
    {
        // Arrange
        const int fontSize = 12;
        var mockFont = new Mock<IFont>();
        mockFont.SetupGet(p => p.FilePath).Returns(FontContentName);
        mockFont.SetupGet(p => p.Size).Returns(fontSize);

        this.mockFontLoader.Setup(m => m.Load($"{FontContentName}|size:{fontSize}")).Returns(mockFont.Object);

        var sut = CreateSystemUnderTest();
        var font = sut.LoadFont(FontContentName, fontSize);

        // Act
        sut.UnloadFont(font);

        // Assert
        this.mockFontLoader.Verify(m => m.Unload($"{FontContentName}|size:{fontSize}"), Times.Once());
    }
    #endregion

    /// <summary>
    /// Returns a new instance of a content sut.
    /// </summary>
    /// <returns>A content sut instance to use for testing.</returns>
    private ContentLoader CreateSystemUnderTest()
        => new (this.mockTextureLoader.Object,
            this.mockSoundLoader.Object,
            this.mockAtlasLoader.Object,
            this.mockFontLoader.Object);
}
