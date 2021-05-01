// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using Raptor.Content;
    using Raptor.NativeInterop;
    using Raptor.Services;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureLoader"/> class.
    /// </summary>
    public class TextureLoaderTests
    {
        private const string TextureFileName = "test-texture.png";
        private const uint OpenGLTextureID = 1234;
        private readonly string textureFilePath;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<IPathResolver> mockTexturePathResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
        /// </summary>
        public TextureLoaderTests()
        {
            this.textureFilePath = $@"C:\temp\{TextureFileName}";
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.GenTexture()).Returns(OpenGLTextureID); // Mock out the OpenGL texture ID

            this.mockImageService = new Mock<IImageService>();
            this.mockTexturePathResolver = new Mock<IPathResolver>();
            this.mockTexturePathResolver.Setup(m => m.ResolveFilePath(TextureFileName)).Returns(this.textureFilePath);
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsTexture()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(TextureFileName);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(actual.Path, this.textureFilePath);
            this.mockGL.Verify(m => m.GenTexture(), Times.Once());
            this.mockGL.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<uint>()), Times.Exactly(2));
        }

        [Fact]
        public void Load_WhenTextureIsAlreadyLoaded_ReturnsAlreadyLoadedTexture()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var textureA = loader.Load(TextureFileName);
            var textureB = loader.Load(TextureFileName);

            // Assert
            Assert.Equal(textureA.Name, textureB.Name);
            Assert.Equal(textureA.Path, textureB.Path);
            this.mockGL.Verify(m => m.ObjectLabel(ObjectLabelIdentifier.Texture, It.IsAny<uint>(), -1, TextureFileName), Times.Once());
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
            this.mockGL.Verify(m => m.DeleteTexture(OpenGLTextureID), Times.Once());
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
            this.mockGL.Verify(m => m.DeleteTexture(OpenGLTextureID), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureLoader CreateLoader() => new (this.mockGL.Object, this.mockImageService.Object, this.mockTexturePathResolver.Object);
    }
}
