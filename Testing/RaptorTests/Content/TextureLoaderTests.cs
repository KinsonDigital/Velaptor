// <copyright file="TextureLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using Raptor.Content;
    using Raptor.OpenGL;
    using Raptor.Services;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="TextureLoader"/> class.
    /// </summary>
    public class TextureLoaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IImageFileService> mockImageFileService;
        private readonly Mock<IContentSource> mockContentSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoaderTests"/> class.
        /// </summary>
        public TextureLoaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockImageFileService = new Mock<IImageFileService>();
            this.mockContentSource = new Mock<IContentSource>();
        }

        #region Method Tests
        [Fact]
        public void Load_WhenInvoked_LoadsTexture()
        {
            // Arrange
            var loader = new TextureLoader(this.mockGL.Object, this.mockImageFileService.Object, this.mockContentSource.Object);

            // Act
            var actual = loader.Load("test-file");

            // Assert
            Assert.NotNull(actual);
            this.mockGL.Verify(m => m.GenTexture(), Times.Once());
            this.mockGL.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<uint>()), Times.Exactly(2));
        }
        #endregion
    }
}
