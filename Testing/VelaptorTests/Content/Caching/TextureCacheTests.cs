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
using NSubstitute;
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
    private const string TextureDirPath = "C:/textures";
    private const string TextureName = "text-texture";
    private const string FontAtlasPrefix = "FontAtlas";
    private const string FontDirPath = "C:/fonts";
    private const string FontName = "test-font";
    private const string FontExtension = ".ttf";
    private const string TextureFilePath = $"{TextureDirPath}/{TextureName}{TextureExtension}";
    private const string FontFilePath = $"{FontDirPath}/{FontName}{FontExtension}";
    private readonly string fontAtlasTextureName = $"{FontAtlasPrefix}|{FontName}|size:{FontSize}";
    private readonly string fontFilePathWithMetaData;
    private readonly IImageService mockImageService;
    private readonly ITextureFactory mockTextureFactory;
    private readonly IFontAtlasService mockFontAtlasService;
    private readonly IFontMetaDataParser mockFontMetaDataParser;
    private readonly IPath mockPath;
    private readonly ImageData textureImageData;
    private readonly ImageData fontImageData;
    private readonly IPushReactable<DisposeTextureData> mockDisposeReactable;
    private readonly IReactableFactory mockReactableFactory;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureCacheTests"/> class.
    /// </summary>
    public TextureCacheTests()
    {
        this.fontFilePathWithMetaData = $"{FontFilePath}|size:{FontSize}";

        this.textureImageData = new ImageData(new Color[1, 2]);
        this.fontImageData = new ImageData(new Color[2, 1]);

        this.mockImageService = Substitute.For<IImageService>();
        this.mockImageService.Load(TextureFilePath)
            .Returns(this.textureImageData);
        this.mockImageService.FlipVertically(this.fontImageData)
            .Returns(this.fontImageData);

        this.mockFontAtlasService = Substitute.For<IFontAtlasService>();
        this.mockFontAtlasService.CreateAtlas(FontFilePath, FontSize)
            .Returns((this.fontImageData, []));

        var mockRegularTexture = Substitute.For<ITexture>();
        var mockFontAtlasTexture = Substitute.For<ITexture>();

        this.mockTextureFactory = Substitute.For<ITextureFactory>();

        // Mock the return of a regular texture if the texture content was a texture file
        this.mockTextureFactory.Create(TextureName, TextureFilePath, this.textureImageData)
            .Returns(mockRegularTexture);

        // Mock the return of a font texture atlas if the texture content was a font file
        this.mockTextureFactory.Create(this.fontAtlasTextureName, FontFilePath, this.fontImageData)
            .Returns(mockFontAtlasTexture);

        this.mockFontMetaDataParser = Substitute.For<IFontMetaDataParser>();

        this.mockPath = Substitute.For<IPath>();
        // Mock getting extension for full texture file path
        this.mockPath.GetExtension(TextureFilePath).Returns(TextureExtension);
        // Mock getting extension for full font file path
        this.mockPath.GetExtension(FontFilePath).Returns(FontExtension);

        // Mock the process of getting the texture name
        this.mockPath.GetFileNameWithoutExtension(TextureFilePath).Returns(TextureName);

        // Mock the process of getting the font name
        this.mockPath.GetFileNameWithoutExtension(FontFilePath).Returns(FontName);

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo => this.shutDownReactor = callInfo.Arg<IReceiveSubscription>());

        this.mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();
        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(this.mockDisposeReactable);
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
                this.mockTextureFactory,
                this.mockFontAtlasService,
                this.mockFontMetaDataParser,
                this.mockPath,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imageService')");
    }

    [Fact]
    public void Ctor_WithNullTextureFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService,
                null,
                this.mockFontAtlasService,
                this.mockFontMetaDataParser,
                this.mockPath,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureFactory')");
    }

    [Fact]
    public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService,
                this.mockTextureFactory,
                null,
                this.mockFontMetaDataParser,
                this.mockPath,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontAtlasService')");
    }

    [Fact]
    public void Ctor_WithNullFontMetaDataParserParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService,
                this.mockTextureFactory,
                this.mockFontAtlasService,
                null,
                this.mockPath,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fontMetaDataParser')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService,
                this.mockTextureFactory,
                this.mockFontAtlasService,
                this.mockFontMetaDataParser,
                null,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureCache(
                this.mockImageService,
                this.mockTextureFactory,
                this.mockFontAtlasService,
                this.mockFontMetaDataParser,
                this.mockPath,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TotalCachedItems_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
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
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
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
    [Fact]
    public void GetItem_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'textureFilePath')");
    }

    [Fact]
    public void GetItem_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.GetItem(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'textureFilePath')");
    }

    [Fact]
    public void GetItem_WhenFileIsNotATextureOrFontWithNoMetaData_ThrowsException()
    {
        // Arrange
        const string invalidFileType = $"{TextureDirPath}/{TextureName}.txt";
        this.mockPath.GetExtension(invalidFileType).Returns(".txt");
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockFontMetaDataParser.Parse(invalidFileType)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = false,
                IsValid = false,
                MetaDataPrefix = string.Empty,
                MetaData = string.Empty,
                FontSize = 0,
            });
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

        this.mockPath.GetExtension(nonFontFilePath).Returns(extension);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockFontMetaDataParser.Parse(nonFontFilePathWithMetaData)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = nonFontFilePath,
                MetaData = metaData,
                FontSize = 12,
            });
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
        this.mockPath.GetExtension(nonFullFilePath).Returns(FontExtension);
        this.mockFontMetaDataParser.Parse(nonFullFilePath)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = nonFullFilePath,
                MetaData = metaData,
                FontSize = 12,
            });
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
        this.mockPath.GetExtension(fullFilePath).Returns(FontExtension);
        this.mockFontMetaDataParser.Parse(fullFilePath)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = false,
                MetaDataPrefix = fullFilePath,
                MetaData = metaData,
                FontSize = 12,
            });
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
        this.mockPath.GetExtension(nonFullFilePath).Returns(TextureExtension);
        this.mockFontMetaDataParser.Parse(nonFullFilePath)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = false,
                IsValid = false,
                MetaDataPrefix = string.Empty,
                MetaData = string.Empty,
                FontSize = 0,
            });
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
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        MockTextureParseResult();
        var sut = CreateSystemUnderTest();

        // Act
        var actualA = sut.GetItem(TextureFilePath);
        var actualB = sut.GetItem(TextureFilePath);

        // Assert
        this.mockFontMetaDataParser.Received(2).Parse(TextureFilePath);
        this.mockPath.Received(2).GetExtension(TextureFilePath);
        this.mockImageService.Received(1).Load(TextureFilePath);
        this.mockPath.Received(2).GetFileNameWithoutExtension(TextureFilePath);
        this.mockTextureFactory.Received(1).Create(TextureName, TextureFilePath, this.textureImageData);

        actualB.Should().BeSameAs(actualA);
    }

    [Fact]
    public void GetItem_WhenGettingFontAtlasTexture_CachesAndReturnsSameAtlasTexture()
    {
        // Arrange
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        MockFontParseResult();
        var sut = CreateSystemUnderTest();

        // Act
        var actualA = sut.GetItem(this.fontFilePathWithMetaData);
        var actualB = sut.GetItem(this.fontFilePathWithMetaData);

        // Assert
        this.mockFontMetaDataParser.Received(2).Parse(this.fontFilePathWithMetaData);
        this.mockPath.Received(2).GetExtension(FontFilePath);

        this.mockFontAtlasService.Received(1).CreateAtlas(FontFilePath, FontSize);

        this.mockImageService.Received(1).FlipVertically(this.fontImageData);
        this.mockPath.Received(2).GetFileNameWithoutExtension(FontFilePath);

        this.mockTextureFactory.Create(this.fontAtlasTextureName, FontFilePath, this.fontImageData);

        actualB.Should().BeSameAs(actualA);
    }

    [Fact]
    public void GetItem_WhenValueUsedIsFontFileWithNoMetaData_ThrowsException()
    {
        // Arrange
        var expected = "Font file paths must include metadata.";
        expected += $"{Environment.NewLine}Font Content Path MetaData Syntax: <file-path>|size:<font-size>";
        expected += $"{Environment.NewLine}Example: C:/Windows/Fonts/my-font.ttf|size:12";

        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
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

        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(123u);

        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        this.mockDisposeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<DisposeTextureData>()))
            .Do(callInfo => actual = callInfo.Arg<DisposeTextureData>());

        MockImageData();
        MockTextureCreation(mockTexture);

        var sut = CreateSystemUnderTest();
        _ = sut.GetItem(TextureFilePath);

        // Act
        var act = () => sut.Unload(TextureFilePath);

        // Assert
        act.Should().NotThrow<NullReferenceException>();

        sut.TotalCachedItems.Should().Be(0);
        this.mockDisposeReactable.Received(1).Push(PushNotifications.TextureDisposedId, Arg.Any<DisposeTextureData>());

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Unload_WhenTextureToUnloadDoesNotExist_DoesNotAttemptToDispose()
    {
        // Arrange
        var mockTexture = Substitute.For<ITexture>();
        mockTexture.Id.Returns(123u);
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);
        MockImageData();
        MockTextureCreation(mockTexture);

        var sut = CreateSystemUnderTest();
        sut.GetItem(TextureFilePath);

        // Act
        sut.Unload("non-existing-texture");

        // Assert
        this.mockDisposeReactable.DidNotReceive().Push(PushNotifications.TextureDisposedId, Arg.Any<DisposeTextureData>());
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void Reactable_WithShutDownNotification_ShutsDownCache()
    {
        // Arrange
        const string texturePathA = $"{TextureDirPath}/textureA{TextureExtension}";
        const string texturePathB = $"{TextureDirPath}/textureB{TextureExtension}";

        var mockTextureA = Substitute.For<ITexture>();
        mockTextureA.Id.Returns(11u);

        var mockTextureB = Substitute.For<ITexture>();
        mockTextureB.Id.Returns(22u);

        this.mockPath.GetExtension(texturePathA).Returns(TextureExtension);
        this.mockPath.GetExtension(texturePathB).Returns(TextureExtension);
        this.mockPath.GetFileNameWithoutExtension(texturePathA).Returns("textureA");
        this.mockPath.GetFileNameWithoutExtension(texturePathB).Returns("textureB");
        this.mockPath.IsPathRooted(Arg.Any<string?>()).Returns(true);

        MockImageData();
        MockTextureCreation(mockTextureA, "textureA", texturePathA);
        MockTextureCreation(mockTextureB, "textureB", texturePathB);

        var sut = CreateSystemUnderTest();
        sut.GetItem(texturePathA);
        sut.GetItem(texturePathB);

        // Act
        this.shutDownReactor?.OnReceive();
        this.shutDownReactor?.OnReceive();

        // Assert
        this.mockDisposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = 11u });
        this.mockDisposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = 22u });
        sut.TotalCachedItems.Should().Be(0);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureCache"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureCache CreateSystemUnderTest() =>
        new (this.mockImageService,
            this.mockTextureFactory,
            this.mockFontAtlasService,
            this.mockFontMetaDataParser,
            this.mockPath,
            this.mockReactableFactory);

    /// <summary>
    /// Mocks parse result when caching texture file paths.
    /// </summary>
    private void MockTextureParseResult()
    {
        this.mockFontMetaDataParser.Parse(TextureFilePath)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = false,
                IsValid = true,
                MetaDataPrefix = string.Empty,
                MetaData = string.Empty,
                FontSize = 0,
            });
    }

    /// <summary>
    /// Mocks the parse result when caching font file paths.
    /// </summary>
    private void MockFontParseResult()
    {
        this.mockFontMetaDataParser.Parse(this.fontFilePathWithMetaData)
            .Returns(new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = FontFilePath,
                MetaData = $"size:{FontSize}",
                FontSize = FontSize,
            });
    }

    /// <summary>
    /// Mocks the image data result when loading a texture file path.
    /// </summary>
    private void MockImageData()
    {
        var imageData = new ImageData(new Color[3, 1]);
        this.mockImageService.Load(TextureFilePath)
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

        this.mockTextureFactory.Create(name, path, Arg.Any<ImageData>()).Returns(texture);
    }
}
