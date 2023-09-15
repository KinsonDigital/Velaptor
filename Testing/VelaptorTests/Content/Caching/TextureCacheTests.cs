// <copyright file="TextureCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Abstractions;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Caching;
using Velaptor.Content.Exceptions;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

public class TextureCacheTests
{
    private const int FontSize = 12;
    private const string TextureExtension = ".png";
    private const string TextureDirPath = @"C:/textures";
    private const string TextureName = "text-texture";
    private const string FontAtlasPrefix = "FontAtlas";
    private const string FontDirPath = @"C:/fonts";
    private const string FontName = "test-font";
    private const string FontExtension = ".ttf";
    private const string TextureFilePath = $"{TextureDirPath}/{TextureName}{TextureExtension}";
    private const string FontFilePath = $"{FontDirPath}/{FontName}{FontExtension}";
    private readonly string fontAtlasTextureName = $"{FontAtlasPrefix}|{FontName}|size:{FontSize}";
    private readonly string fontFilePathWithMetaData;
    private readonly Mock<IImageService> mockImageService;
    private readonly Mock<ITextureFactory> mockTextureFactory;
    private readonly Mock<IFontAtlasService> mockFontAtlasService;
    private readonly Mock<IFontMetaDataParser> mockFontMetaDataParser;
    private readonly Mock<IPath> mockPath;
    private readonly ImageData textureImageData;
    private readonly ImageData fontImageData;
    private readonly Mock<IPushReactable<DisposeTextureData>> mockDisposeReactable;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureCacheTests"/> class.
    /// </summary>
    public TextureCacheTests()
    {
        this.fontFilePathWithMetaData = $"{FontFilePath}|size:{FontSize}";

        this.textureImageData = new ImageData(new Color[1, 2], 1, 2);
        this.fontImageData = new ImageData(new Color[2, 1], 2, 1);

        this.mockImageService = new Mock<IImageService>();
        this.mockImageService.Setup(m => m.Load(TextureFilePath))
            .Returns(this.textureImageData);
        this.mockImageService.Setup(m => m.FlipVertically(this.fontImageData))
            .Returns(this.fontImageData);

        this.mockFontAtlasService = new Mock<IFontAtlasService>();
        this.mockFontAtlasService.Setup(m =>
                m.CreateAtlas(FontFilePath, FontSize))
            .Returns((this.fontImageData, Array.Empty<GlyphMetrics>()));

        var mockRegularTexture = new Mock<ITexture>();
        var mockFontAtlasTexture = new Mock<ITexture>();

        this.mockTextureFactory = new Mock<ITextureFactory>();

        // Mock the return of a regular texture if the texture content was a texture file
        this.mockTextureFactory.Setup(m =>
                m.Create(TextureName, TextureFilePath, this.textureImageData))
            .Returns(mockRegularTexture.Object);

        // Mock the return of a font texture atlas if the texture content was a font file
        this.mockTextureFactory.Setup(m =>
                m.Create(this.fontAtlasTextureName, FontFilePath, this.fontImageData))
            .Returns(mockFontAtlasTexture.Object);

        this.mockFontMetaDataParser = new Mock<IFontMetaDataParser>();

        this.mockPath = new Mock<IPath>();
        // Mock getting extension for full texture file path
        this.mockPath.Setup(m => m.GetExtension(TextureFilePath)).Returns(TextureExtension);
        // Mock getting extension for full font file path
        this.mockPath.Setup(m => m.GetExtension(FontFilePath)).Returns(FontExtension);

        // Mock the process of getting the texture name
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(TextureFilePath))
            .Returns(TextureName);

        // Mock the process of getting the font name
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(FontFilePath))
            .Returns(FontName);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.shutDownReactor = reactor;
            });

        this.mockDisposeReactable = new Mock<IPushReactable<DisposeTextureData>>();
        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateDisposeTextureReactable()).Returns(this.mockDisposeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullImageServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                null,
                this.mockTextureFactory.Object,
                this.mockFontAtlasService.Object,
                this.mockFontMetaDataParser.Object,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullTextureFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService.Object,
                null,
                this.mockFontAtlasService.Object,
                this.mockFontMetaDataParser.Object,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService.Object,
                this.mockTextureFactory.Object,
                null,
                this.mockFontMetaDataParser.Object,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullFontMetaDataParserParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService.Object,
                this.mockTextureFactory.Object,
                this.mockFontAtlasService.Object,
                null,
                this.mockPath.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontMetaDataParser')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService.Object,
                this.mockTextureFactory.Object,
                this.mockFontAtlasService.Object,
                this.mockFontMetaDataParser.Object,
                null,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService.Object,
                this.mockTextureFactory.Object,
                this.mockFontAtlasService.Object,
                this.mockFontMetaDataParser.Object,
                this.mockPath.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        var sut = CreateSystemUnderTest();
        sut.GetItem(TextureFilePath);

        // Act
        var actual = sut.TotalCachedItems;

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void CacheKeys_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        var expected = new[] { TextureFilePath }.AsReadOnly();
        var sut = CreateSystemUnderTest();
        sut.GetItem(TextureFilePath);

        // Act
        var actual = sut.CacheKeys;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GetItem_WithNullOrEmptyFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(filePath);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'textureFilePath')");
    }

    [Fact]
    public void GetItem_WhenFileIsNotATextureOrFontWithNoMetaData_ThrowsException()
    {
        // Arrange
        const string invalidFileType = $"{TextureDirPath}/{TextureName}.txt";
        this.mockPath.Setup(m => m.GetExtension(invalidFileType)).Returns(".txt");
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockFontMetaDataParser.Setup(m => m.Parse(invalidFileType))
            .Returns(() => new FontMetaDataParseResult(
                false,
                false,
                string.Empty,
                string.Empty,
                0));
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(invalidFileType);

        // Assert
        act.Should().Throw<CachingException>()
            .WithMessage($"Texture caching must be a '{TextureExtension}' file type.");
    }

    [Fact]
    public void GetItem_WhenPathContainsMetaDataAndIsNotAFontFileType_ThrowsException()
    {
        // Arrange
        const string extension = ".txt";
        const string metaData = "|size:12";
        const string nonFontFilePath = $"{FontDirPath}/{FontName}{extension}";
        const string nonFontFilePathWithMetaData = $"{nonFontFilePath}{metaData}";

        this.mockPath.Setup(m => m.GetExtension(nonFontFilePath)).Returns(extension);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockFontMetaDataParser.Setup(m => m.Parse(nonFontFilePathWithMetaData))
            .Returns(() => new FontMetaDataParseResult(
                true,
                true,
                nonFontFilePath,
                metaData,
                12));
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(nonFontFilePathWithMetaData);

        // Assert
        act.Should().Throw<CachingException>()
            .WithMessage($"Font caching must be a '{FontExtension}' file type.");
    }

    [Fact]
    public void GetItem_WhenFontPathIsNotFullFilePath_ThrowsException()
    {
        // Arrange
        const string metaData = "|size:12";
        const string nonFullFilePath = $"{FontName}{FontExtension}{metaData}";
        this.mockPath.Setup(m => m.GetExtension(nonFullFilePath)).Returns(FontExtension);
        this.mockFontMetaDataParser.Setup(m => m.Parse(nonFullFilePath))
            .Returns(() => new FontMetaDataParseResult(
                true,
                true,
                nonFullFilePath,
                metaData,
                12));
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(nonFullFilePath);

        // Assert
        act.Should().Throw<CachingException>()
            .WithMessage($"The font file path '{nonFullFilePath}' must be a fully qualified file path of type '{FontExtension}'.");
    }

    [Fact]
    public void GetItem_WhenFontPathMetaDataIsInvalid_ThrowsException()
    {
        // Arrange
        const string metaData = "|size12";
        const string fullFilePath = $"{FontDirPath}/{FontName}{FontExtension}{metaData}";
        this.mockPath.Setup(m => m.GetExtension(fullFilePath)).Returns(FontExtension);
        this.mockFontMetaDataParser.Setup(m => m.Parse(fullFilePath))
            .Returns(() => new FontMetaDataParseResult(
                true,
                false,
                fullFilePath,
                metaData,
                12));
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(fullFilePath);

        // Assert
        act.Should().Throw<CachingMetaDataException>()
            .WithMessage($"The metadata '{metaData}' is invalid and is required for font files of type '{FontExtension}'.");
    }

    [Fact]
    public void GetItem_WhenTexturePathIsNotFullFilePath_ThrowsException()
    {
        // Arrange
        const string nonFullFilePath = $"{TextureName}{TextureExtension}";
        this.mockPath.Setup(m => m.GetExtension(nonFullFilePath)).Returns(TextureExtension);
        this.mockFontMetaDataParser.Setup(m => m.Parse(nonFullFilePath))
            .Returns(() => new FontMetaDataParseResult(
                false,
                false,
                string.Empty,
                string.Empty,
                0));
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(nonFullFilePath);

        // Assert
        act.Should().Throw<CachingException>()
            .WithMessage($"The texture file path '{nonFullFilePath}' must be a fully qualified file path of type '{TextureExtension}'.");
    }

    [Fact]
    public void GetItem_WhenGettingTexture_CachesAndReturnsSameTexture()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        MockTextureParseResult();
        var sut = CreateSystemUnderTest();

        // Act
        var actualA = sut.GetItem(TextureFilePath);
        var actualB = sut.GetItem(TextureFilePath);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(TextureFilePath), Times.Exactly(2));
        this.mockPath.Verify(m => m.GetExtension(TextureFilePath), Times.Exactly(2));
        this.mockImageService.Verify(m => m.Load(TextureFilePath), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(TextureFilePath), Times.Exactly(2));
        this.mockTextureFactory.Verify(m =>
            m.Create(TextureName, TextureFilePath, this.textureImageData), Times.Once);

        actualB.Should().BeSameAs(actualA);
    }

    [Fact]
    public void GetItem_WhenGettingFontAtlasTexture_CachesAndReturnsSameAtlasTexture()
    {
        // Arrange
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        MockFontParseResult();
        var sut = CreateSystemUnderTest();

        // Act
        var actualA = sut.GetItem(this.fontFilePathWithMetaData);
        var actualB = sut.GetItem(this.fontFilePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Verify(m => m.Parse(this.fontFilePathWithMetaData), Times.Exactly(2));
        this.mockPath.Verify(m => m.GetExtension(FontFilePath), Times.Exactly(2));

        this.mockFontAtlasService.Verify(m =>
            m.CreateAtlas(FontFilePath, FontSize), Times.Once);

        this.mockImageService.Verify(m => m.FlipVertically(this.fontImageData), Times.Once);
        this.mockPath.Verify(m => m.GetFileNameWithoutExtension(FontFilePath), Times.Exactly(2));

        this.mockTextureFactory.Verify(m =>
            m.Create(this.fontAtlasTextureName, FontFilePath, this.fontImageData), Times.Once);

        actualB.Should().BeSameAs(actualA);
    }

    [Fact]
    public void GetItem_WhenValueUsedIsFontFileWithNoMetaData_ThrowsException()
    {
        // Arrange
        var expected = "Font file paths must include metadata.";
        expected += $"{Environment.NewLine}Font Content Path MetaData Syntax: <file-path>|size:<font-size>";
        expected += $"{Environment.NewLine}Example: C:/Windows/Fonts/my-font.ttf|size:12";

        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        MockTextureParseResult();

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(FontFilePath);

        // Assert
        act.Should().Throw<CachingMetaDataException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Unload_WhenTextureToUnloadExists_RemovesAndDisposesOfTexture()
    {
        // Arrange
        var expected = new DisposeTextureData { TextureId = 123u };
        DisposeTextureData? actual = null;

        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(123u);

        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        this.mockDisposeReactable.Setup(m =>
                m.Push(It.Ref<DisposeTextureData>.IsAny, It.IsAny<Guid>()))
            .Callback((in DisposeTextureData data, Guid _) =>
            {
                data.Should().NotBeNull("it is required for unit testing.");

                actual = data;
            });

        MockImageData();
        MockTextureCreation(mockTexture.Object);

        var sut = CreateSystemUnderTest();
        _ = sut.GetItem(TextureFilePath);

        // Act
        var act = () => sut.Unload(TextureFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable
            .VerifyOnce(m =>
                m.Push(It.Ref<DisposeTextureData>.IsAny, PushNotifications.TextureDisposedId));

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Unload_WhenTextureToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockTexture = new Mock<ITexture>();
        mockTexture.SetupGet(p => p.Id).Returns(123u);
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);
        MockImageData();
        MockTextureCreation(mockTexture.Object);

        var sut = CreateSystemUnderTest();
        sut.GetItem(TextureFilePath);

        // Act
        sut.Unload("non-existing-texture");

        // Assert
        this.mockDisposeReactable
            .Verify(m =>
                m.Push(It.IsAny<DisposeTextureData>(), PushNotifications.TextureDisposedId), Times.Never);
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void Reactable_WithShutDownNotification_ShutsDownCache()
    {
        // Arrange
        const string texturePathA = $"{TextureDirPath}/textureA{TextureExtension}";
        const string texturePathB = $"{TextureDirPath}/textureB{TextureExtension}";

        var mockTextureA = new Mock<ITexture>();
        mockTextureA.SetupGet(p => p.Id).Returns(11u);
        mockTextureA.Name = nameof(mockTextureA);

        var mockTextureB = new Mock<ITexture>();
        mockTextureB.SetupGet(p => p.Id).Returns(22u);
        mockTextureB.Name = nameof(mockTextureB);

        this.mockPath.Setup(m => m.GetExtension(texturePathA)).Returns(TextureExtension);
        this.mockPath.Setup(m => m.GetExtension(texturePathB)).Returns(TextureExtension);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathA)).Returns("textureA");
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathB)).Returns("textureB");
        this.mockPath.Setup(m => m.IsPathRooted(It.IsAny<string?>())).Returns(true);

        MockImageData();
        MockTextureCreation(mockTextureA.Object, "textureA", texturePathA);
        MockTextureCreation(mockTextureB.Object, "textureB", texturePathB);

        var sut = CreateSystemUnderTest();
        sut.GetItem(texturePathA);
        sut.GetItem(texturePathB);

        // Act
        this.shutDownReactor?.OnReceive();
        this.shutDownReactor?.OnReceive();

        // Assert
        this.mockDisposeReactable.VerifyOnce(m => m.Unsubscribe(PushNotifications.TextureDisposedId));
        sut.TotalCachedItems.Should().Be(0);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureCache CreateSystemUnderTest() =>
        new (this.mockImageService.Object,
            this.mockTextureFactory.Object,
            this.mockFontAtlasService.Object,
            this.mockFontMetaDataParser.Object,
            this.mockPath.Object,
            this.mockReactableFactory.Object);

    /// <summary>
    /// Mocks parse result when caching texture file paths.
    /// </summary>
    private void MockTextureParseResult()
    {
        this.mockFontMetaDataParser.Setup(m => m.Parse(TextureFilePath))
            .Returns(() => new FontMetaDataParseResult(
                false,
                true,
                string.Empty,
                string.Empty,
                0));
    }

    /// <summary>
    /// Mocks the parse result when caching font file paths.
    /// </summary>
    private void MockFontParseResult()
    {
        this.mockFontMetaDataParser.Setup(m => m.Parse(this.fontFilePathWithMetaData))
            .Returns(() => new FontMetaDataParseResult(
                true,
                true,
                FontFilePath,
                $"size:{FontSize}",
                FontSize));
    }

    /// <summary>
    /// Mocks the image data result when loading a texture file path.
    /// </summary>
    private void MockImageData()
    {
        var imageData = new ImageData(new Color[3, 1], 3, 1);
        this.mockImageService.Setup(m => m.Load(TextureFilePath))
            .Returns(imageData);
    }

    /// <summary>
    /// Mocks the texture being returned when using the <see cref="TextureFactory"/> mock.
    /// </summary>
    /// <param name="texture">The texture to return.</param>
    /// <param name="textureName">The name of the texture.</param>
    /// <param name="filePath">The texture file path.</param>
    private void MockTextureCreation(ITexture texture, string? textureName = null, string? filePath = null)
    {
        var name = textureName ?? TextureName;
        var path = filePath ?? TextureFilePath;

        this.mockTextureFactory.Setup(m => m.Create(name, path, It.IsAny<ImageData>()))
            .Returns(texture);
    }
}
