namespace RaptorTests.Content
{
    using FileIO.Core;
    using Raptor.Content;
    using Raptor.OpenGL;
    using Moq;
    using Xunit;
    using OpenToolkit.Graphics.OpenGL4;

    public class TextureLoaderTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IImageFile> mockImageFile;

        /// <summary>
        /// Initializes a new instance of <see cref="TextureLoaderTests"/>.
        /// </summary>
        public TextureLoaderTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockImageFile = new Mock<IImageFile>();
        }


        [Fact]
        public void Load_WhenInvoked_LoadsTexture()
        {
            //Arrange
            var loader = new TextureLoader(this.mockGL.Object, this.mockImageFile.Object);

            //Act
            var actual = loader.Load("test-file");

            //Assert
            Assert.NotNull(actual);
            this.mockGL.Verify(m => m.GenTexture(), Times.Once());
            this.mockGL.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.IsAny<int>()), Times.Exactly(2));
        }
    }
}
