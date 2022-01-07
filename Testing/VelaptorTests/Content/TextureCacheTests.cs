// <copyright file="TextureCacheTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    public class TextureCacheTests
    {
        private const string TextureExtension = ".png";
        private const string TextureName = "texture";
        private const string TextureDirPath = @"C:\content\";
        private readonly string textureFilePath = $"{TextureDirPath}{TextureName}{TextureExtension}";
        private readonly Mock<IPath> mockPath;
        private readonly Mock<ITextureFactory> mockTextureFactory;
        private readonly Mock<IImageService> mockImageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCacheTests"/> class.
        /// </summary>
        public TextureCacheTests()
        {
            this.mockImageService = new Mock<IImageService>();
            this.mockTextureFactory = new Mock<ITextureFactory>();

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.textureFilePath))
                .Returns(TextureName);
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
        public void GetItem_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            MockImageData();
            MockTexture(mockTexture.Object);

            var cache = CreateCache();

            // Act
            var firstItem = cache.GetItem(this.textureFilePath);
            var secondItem = cache.GetItem(this.textureFilePath);

            // Assert
            Assert.Same(mockTexture.Object, firstItem);
            Assert.Same(mockTexture.Object, secondItem);
        }

        [Fact]
        public void Unload_WhenItemToUnloadExists_RemovesAndDisposesOfTexture()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();

            MockImageData();
            MockTexture(mockTexture.Object);

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

            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathA)).Returns("textureA");
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(texturePathB)).Returns("textureB");

            MockImageData();
            MockTexture(mockTextureA.Object, "textureA", texturePathA);
            MockTexture(mockTextureB.Object, "textureB", texturePathB);

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
                this.mockPath.Object);

        private void MockImageData()
        {
            var imageData = new ImageData(new Color[3, 1], 3, 1);
            this.mockImageService.Setup(m => m.Load(this.textureFilePath))
                .Returns(imageData);
        }

        private void MockTexture(ITexture texture, string? textureName = null, string? filePath = null)
        {
            var name = textureName ?? TextureName;
            var path = filePath ?? this.textureFilePath;

            this.mockTextureFactory.Setup(m => m.Create(name, path, It.IsAny<ImageData>(), true))
                .Returns(texture);
        }
    }
}
