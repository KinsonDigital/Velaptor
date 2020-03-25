using Moq;
using RaptorCore.Graphics;
using Xunit;

namespace KDScorpionCoreTests.Graphics
{
    /// <summary>
    /// Unit tests to test the <see cref="Texture"/> class.
    /// </summary>
    public class TextureTests
    {
        [Fact]
        public void Width_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(m => m.Width).Returns(23);
            var texture = new Texture(mockTexture.Object);
            var expected = 23;

            //Act
            var actual = texture.Width;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Height_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var mockTexture = new Mock<ITexture>();
            mockTexture.SetupGet(m => m.Height).Returns(56);
            var texture = new Texture(mockTexture.Object);

            var expected = 56;

            //Act
            var actual = texture.Height;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
