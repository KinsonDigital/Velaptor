// <copyright file="TextureCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Caching
{
    using System;
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    public class TextureCacheTests
    {
        private const string TextureExtension = ".png";
        private const string FontExtension = ".ttf";
        private const string TextureName = "texture";
        private const string TextureDirPath = @"C:\content\";
        private readonly string textureFilePath = $"{TextureDirPath}{TextureName}{TextureExtension}";
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<ITextureFactory> mockTextureFactory;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly Mock<IPath> mockPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCacheTests"/> class.
        /// </summary>
        public TextureCacheTests()
        {
            this.mockImageService = new Mock<IImageService>();
            this.mockTextureFactory = new Mock<ITextureFactory>();

            this.mockFontAtlasService = new Mock<IFontAtlasService>();

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.textureFilePath))
                .Returns(TextureName);
            this.mockPath.Setup(m => m.GetExtension(this.textureFilePath)).Returns(TextureExtension);
        }

        #region Method Tests
        [Fact]
        public void GetItem_WithInvalidPath_ThrowsException()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            AssertExtensions.ThrowsWithMessage<LoadTextureException>(() =>
            {
                var unused = cache.GetItem("invalid-path");
            }, $"The texture file path 'invalid-path' is not a valid path.");
        }

        [Fact]
        public void GetItem_WhenCachingStandardTexture_ReturnsCorrectResult()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            MockImageData();
            MockTextureCreation(mockTexture.Object);

            var cache = CreateCache();

            // Act
            var firstItem = cache.GetItem(this.textureFilePath);
            var secondItem = cache.GetItem(this.textureFilePath);

            // Assert
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.mockImageService.Verify(m => m.Load(this.textureFilePath), Times.Once);
            Assert.Same(mockTexture.Object, firstItem);
            Assert.Same(mockTexture.Object, secondItem);
        }

        [Theory]
        [InlineData("size22")]
        public void GetItem_WithMissingFontPathMetaData_ThrowsException(string fontMetaData)
        {
            // Arrange
            const string dirPath = @"C:\content\fonts\";
            const string contentName = "TimesNewRoman-Regular";
            var fontFilePath = $@"{dirPath}{contentName}{FontExtension}";
            var fontFilePathWithMetaData = $@"{fontFilePath}{fontMetaData}";

            var expected = "Font metadata required when caching fonts.";
            expected += "Required metadata syntax: '|size:<number-here>'";
            expected += "\nIf the '|' character is missing, it signifies that no metadata exists.";

            this.mockPath.Setup(m => m.GetExtension(fontFilePath)).Returns($"{FontExtension}size22");
            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                cache.GetItem(fontFilePathWithMetaData);
            }, expected);
        }

        [Theory]
        [InlineData("|size22", FontExtension, "\nCurrent metadata: '|size22'")]
        [InlineData("size:22|", FontExtension, "")]
        [InlineData("|size:NAN", FontExtension, "\nCurrent metadata: '|size:NAN'")]
        [InlineData("||size:22", FontExtension, "")]
        [InlineData("|size:22|", FontExtension, "")]
        public void GetItem_WithInvalidFontPathMetaData_ThrowsException(string fontMetaData, string extension, string exceptionMsg)
        {
            // Arrange
            const string dirPath = @"C:\content\fonts\";
            const string contentName = "TimesNewRoman-Regular";
            var fontFilePath = $@"{dirPath}{contentName}{extension}";
            var fontFilePathWithMetaData = $@"{fontFilePath}{fontMetaData}";

            var expected = "Font metadata required when caching fonts.";
            expected += exceptionMsg;
            expected += "Required metadata syntax: '|size:<number-here>'";

            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                cache.GetItem(fontFilePathWithMetaData);
            }, expected);
        }

        [Fact]
        public void GetItem_WhenCachingFontTextureAtlas_ReturnsCorrectResult()
        {
            // Arrange
            const string dirPath = @"C:\content\fonts\";
            const string contentName = "TimesNewRoman-Regular";
            const string fontExtension = ".ttf";
            const int fontSize = 22;
            var metaData = $"|size:{fontSize}";
            var fontFilePath = $@"{dirPath}{contentName}{fontExtension}";
            var fontFilePathWithMetaData = $@"{dirPath}{contentName}{fontExtension}{metaData}";
            var mockTexture = new Mock<ITexture>();
            var imageData = new ImageData(new Color[3, 1], 3, 1);
            this.mockPath.Setup(m => m.GetExtension(fontFilePath)).Returns(fontExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fontFilePath)).Returns(contentName);
            MockFontAtlasData(imageData, fontFilePath, fontSize);
            MockTextureCreation(mockTexture.Object, contentName, fontFilePath);

            var cache = CreateCache();

            // Act
            var firstItem = cache.GetItem(fontFilePathWithMetaData);
            var secondItem = cache.GetItem(fontFilePathWithMetaData);

            // Assert
            this.mockFontAtlasService.Verify(m =>
                m.CreateFontAtlas(fontFilePath, 22), Times.Once);
            this.mockImageService.Verify(m => m.Load(It.IsAny<string>()), Times.Never);
            Assert.Same(mockTexture.Object, firstItem);
            Assert.Same(mockTexture.Object, secondItem);
        }

        [Fact]
        public void GetItem_WhenNotAnImageOrFontFile_ThrowsException()
        {
            // Arrange
            const string invalidFilePath = @"C:\Content\Graphics\invalid-extension.txt";
            var expected = "Texture Caching Error:";
            expected += $"\nWhen caching textures, the only file types allowed";
            expected += $" are '{TextureExtension}' and '{FontExtension}' files.";
            expected += "\nFont files are converted into texture atlases for the font glyphs.";

            var mockTexture = new Mock<ITexture>();
            MockImageData();
            MockTextureCreation(mockTexture.Object);
            this.mockPath.Setup(m => m.GetExtension(invalidFilePath)).Returns(".txt");

            var cache = CreateCache();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingException>(() =>
            {
                cache.GetItem(invalidFilePath);
            }, expected);
        }

        [Fact]
        public void Unload_WhenItemToUnloadExists_RemovesAndDisposesOfTexture()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();

            MockImageData();
            MockTextureCreation(mockTexture.Object);

            var cache = CreateCache();
            var unused = cache.GetItem(this.textureFilePath);

            // Act
            cache.Unload(this.textureFilePath);

            // Assert
            AssertExtensions.DoesNotThrowNullReference(() =>
            {
                cache.Unload(this.textureFilePath);
            });

            Assert.Equal(0, cache.TotalCachedItems);
            mockTexture.Verify(m => m.Dispose(), Times.Once);
        }

        [Fact]
        public void Unload_WhenInvoked_DisposesOfTextures()
        {
            // Arrange
            var mockTextureA = new Mock<ITexture>();
            mockTextureA.Name = nameof(mockTextureA);
            var mockTextureB = new Mock<ITexture>();
            mockTextureB.Name = nameof(mockTextureB);

            var texturePathA = $"{TextureDirPath}textureA{TextureExtension}";
            var texturePathB = $"{TextureDirPath}textureB{TextureExtension}";

            this.mockPath.Setup(m => m.GetExtension(texturePathA)).Returns(TextureExtension);
            this.mockPath.Setup(m => m.GetExtension(texturePathB)).Returns(TextureExtension);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathA)).Returns("textureA");
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathB)).Returns("textureB");

            MockImageData();
            MockTextureCreation(mockTextureA.Object, "textureA", texturePathA);
            MockTextureCreation(mockTextureB.Object, "textureB", texturePathB);

            var cache = CreateCache();
            cache.GetItem(texturePathA);
            cache.GetItem(texturePathB);

            // Act
            cache.Dispose();
            cache.Dispose();

            // Assert
            mockTextureA.Verify(m => m.Dispose(), Times.Once);
            mockTextureB.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureCache"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureCache CreateCache() =>
            new (this.mockImageService.Object,
                this.mockTextureFactory.Object,
                this.mockFontAtlasService.Object,
                this.mockPath.Object);

        private void MockImageData()
        {
            var imageData = new ImageData(new Color[3, 1], 3, 1);
            this.mockImageService.Setup(m => m.Load(this.textureFilePath))
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
            var path = filePath ?? this.textureFilePath;

            this.mockTextureFactory.Setup(m => m.Create(name, path, It.IsAny<ImageData>(), true))
                .Returns(texture);
        }

        /// <summary>
        /// Mocks the font atlas data being returned when using the <see cref="FontAtlasService"/> mock.
        /// </summary>
        /// <param name="data">The image data to return if the mock is successful.</param>
        /// <param name="filePath">The path to use.</param>
        /// <param name="fontSize">The font size to use.</param>
        private void MockFontAtlasData(ImageData data, string filePath, int fontSize)
        {
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(filePath, fontSize))
                .Returns((data, Array.Empty<GlyphMetrics>()));
        }
    }
}
