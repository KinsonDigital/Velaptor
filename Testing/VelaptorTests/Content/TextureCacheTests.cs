using System.Drawing;
using System.IO.Abstractions;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Exceptions;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Services;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.Content
{
    public class TextureCacheTests
    {
        private const string TextureExtension = ".png";
        private const string TextureName = "texture";
        private const string TextureDirPath = @"C:\content\";
        private readonly string textureFilePath = $"{TextureDirPath}{TextureName}{TextureExtension}";
        private readonly Mock<IPath> mockPath;
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<ITextureFactory> mockTextureFactory;
        private readonly Mock<IImageService> mockImageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCacheTests"/> class.
        /// </summary>
        public TextureCacheTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
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
            var mockTexture = new Mock<ITexture>();
            MockTexture(mockTexture.Object);

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

            // Act

            // Assert
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="TextureCache"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private TextureCache CreateCache() =>
            new (this.mockGL.Object,
                this.mockGLExtensions.Object,
                this.mockImageService.Object,
                this.mockTextureFactory.Object,
                this.mockPath.Object);


        private void MockTexture(ITexture texture)
        {
            var imageData = new ImageData(new Color[3, 1], 3, 1);
            this.mockTextureFactory.Setup(m => m.Create(TextureName, this.textureFilePath, imageData, true))
                .Returns(texture);
        }
    }
}
