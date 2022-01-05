// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System.Drawing;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.Services;
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
        private const uint OpenGLTextureId = 1234;
        private readonly string textureFilePath = $"{TextureDirPath}{TextureFileName}{TextureExtension}";
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<IPathResolver> mockTexturePathResolver;
        private readonly Mock<IPath> mockPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
        /// </summary>
        public TextureLoaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.GenTexture()).Returns(OpenGLTextureId); // Mock out the OpenGL texture ID

            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();

            var imageData = new ImageData(new Color[3, 1], 3, 1);

            this.mockImageService = new Mock<IImageService>();
            this.mockImageService.Setup(m => m.Load(this.textureFilePath))
                .Returns(imageData);

            this.mockTexturePathResolver = new Mock<IPathResolver>();
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(TextureFileName))
                .Returns(this.textureFilePath);

            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.HasExtension(TextureFileName)).Returns(false);
            this.mockPath.Setup(m => m.HasExtension($"{TextureFileName}{TextureExtension}")).Returns(true);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}")).Returns(TextureFileName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{TextureFileName}{TextureExtension}")).Returns(TextureFileName);
        }

        #region Method Tests
        [Theory]
        [InlineData(TextureFileName, "")]
        [InlineData(TextureFileName, ".txt")]
        public void Load_WhenLoadingAppContentByName_LoadsTexture(string contentName, string extension)
        {
            // Arrange
            this.mockPath.Setup(m => m.HasExtension($"{contentName}{extension}")).Returns(!string.IsNullOrEmpty(extension));
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}")).Returns(contentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{contentName}{extension}")).Returns(contentName);

            var loader = CreateLoader();

            // Act
            var actual = loader.Load($"{contentName}{extension}");

            // Assert
            Assert.NotNull(actual);
            this.mockGL.Verify(m => m.GenTexture(), Times.Once());
            this.mockGL.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, It.IsAny<uint>()), Times.Exactly(2));
        }

        [Fact]
        public void Load_WhenLoadingContentWithFullPath_LoadsTexture()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.textureFilePath);

            // Assert
            Assert.NotNull(actual);
            this.mockImageService.Verify(m => m.Load(this.textureFilePath), Times.Once);
        }

        [Fact]
        public void Load_WhenLoadingSameContentThatIsDisposed_RemovesDataBeforeAdding()
        {
            // Arrange
            var loader = CreateLoader();
            var loadedTexture = loader.Load(TextureFileName);

            // Set the font as not being pooled. This will allow us to dispose of
            // the font to get the font into the disposed state for testing
            loadedTexture.IsPooled = false;
            loadedTexture.Dispose();

            // Act
            var actual = loader.Load(TextureFileName);

            // Assert
            Assert.NotSame(loadedTexture, actual);
        }

        [Fact]
        public void Load_WhenTextureIsAlreadyLoaded_ReturnsAlreadyLoadedTexture()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var textureA = loader.Load(TextureFileName);
            var textureB = loader.Load(TextureFileName);

            // TODO: Verify that this test works

            // Assert
            Assert.Equal(textureA.Name, textureB.Name);
            Assert.Equal(textureA.FilePath, textureB.FilePath);
            Assert.Same(textureA, textureB);
        }

        [Fact]
        public void Load_WhenPathIsInvalidWithUnknownContentFile_ThrowsException()
        {
            // Arrange
            const string invalidPath = "invalid-path.png";
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
                .Returns(invalidPath);

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadContentException>(() =>
            {
                var unused = loader.Load(invalidPath);
            }, $"The content file path '{invalidPath}' is not a valid path.");
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
        public void Load_WhenPathIsInvalidWithFontContentFile_ThrowsException()
        {
            // Arrange
            const string invalidPath = "invalid-path.ttf";
            this.mockPath.Setup(m => m.HasExtension(invalidPath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(invalidPath)).Returns(".ttf");
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(It.IsAny<string>()))
                .Returns(invalidPath);

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadFontException>(() =>
            {
                var unused = loader.Load(invalidPath);
            }, $"The font file path '{invalidPath}' is not a valid path.");
        }

        [Fact]
        public void Unload_WhenInvoked_UnloadsTexture()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(TextureFileName);

            // Act
            loader.Unload(TextureFileName);

            // Assert
            this.mockGL.Verify(m => m.DeleteTexture(OpenGLTextureId), Times.Once());
        }

        [Fact]
        public void Dispose_WhenInvoked_ProperlyDisposesOfTextures()
        {
            // Arrange
            var loader = CreateLoader();
            loader.Load(TextureFileName);

            // Act
            loader.Dispose();
            loader.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteTexture(OpenGLTextureId), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureLoader CreateLoader()
            => new (this.mockGL.Object,
             this.mockGLExtensions.Object,
             this.mockImageService.Object,
             this.mockTexturePathResolver.Object,
             this.mockPath.Object);
    }
}
