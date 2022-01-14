// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureLoader"/> class.
    /// </summary>
    public class TextureLoaderTests
    {
        private const string TextureExtension = ".png";
        private const string TextureDirPath = @"C:\textures\";
        private const string TextureFileName = "test-texture";
        private readonly string textureFilePath = $"{TextureDirPath}{TextureFileName}{TextureExtension}";
        private readonly Mock<IDisposableItemCache<string, ITexture>> mockTextureCache;
        private readonly Mock<IPathResolver> mockTexturePathResolver;
        private readonly Mock<IPath> mockPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
        /// </summary>
        public TextureLoaderTests()
        {
            this.mockTexturePathResolver = new Mock<IPathResolver>();
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(TextureFileName))
                .Returns(this.textureFilePath);

            this.mockTextureCache = new Mock<IDisposableItemCache<string, ITexture>>();

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.HasExtension(TextureFileName)).Returns(false);
            this.mockPath.Setup(m => m.HasExtension($"{TextureFileName}{TextureExtension}")).Returns(true);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}")).Returns(TextureFileName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}{TextureExtension}")).Returns(TextureFileName);
        }

        #region Method Tests
        [Fact]
        public void Load_WhenLoadingContentWithFullPath_LoadsTexture()
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();
            this.mockTextureCache.Setup(m => m.GetItem(this.textureFilePath))
                .Returns(mockTexture.Object);

            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.textureFilePath);

            // Assert
            Assert.NotNull(actual);
            Assert.Same(mockTexture.Object, actual);
        }

        [Theory]
        [InlineData(TextureFileName, "")]
        // [InlineData(TextureFileName, ".txt")]
        public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
        {
            // Arrange
            var mockTexture = new Mock<ITexture>();

            this.mockTextureCache.Setup(m => m.GetItem(this.textureFilePath))
                .Returns(mockTexture.Object);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}")).Returns(contentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}")).Returns(contentName);

            var loader = CreateLoader();

            // Act
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            Assert.NotNull(actual);
            Assert.Same(mockTexture.Object, actual);
        }

        [Fact]
        public void Load_WhenPathIsInvalidWithTextureContentFile_ThrowsException()
        {
            // Arrange
            const string invalidPath = "invalid-path.png";
            this.mockPath.Setup(m => m.HasExtension(invalidPath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(invalidPath)).Returns(".png");
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
                .Returns(invalidPath);

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadTextureException>(() =>
            {
                var unused = loader.Load(invalidPath);
            }, $"The texture file path '{invalidPath}' is not a valid path.");
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsCachedTextures()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.textureFilePath);

            // Assert
            this.mockTextureCache.Verify(m => m.Unload(this.textureFilePath), Times.Once);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfCachedTextures()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.mockTextureCache.Verify(m => m.Dispose(), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureLoader CreateLoader()
            => new (this.mockTextureCache.Object,
             this.mockTexturePathResolver.Object,
             this.mockPath.Object);
    }
}
