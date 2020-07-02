namespace RaptorTests.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using Raptor.OpenGL;
    using Xunit;

    public class SpriteBatchItemTests
    {
        [Fact]
        public void Empty_WhenGettingValue_ReturnsCorrectResult()
        {
            // Act
            var actual = SpriteBatchItem.Empty;

            // Assert
            Assert.Equal(0u, actual.TextureID);
            Assert.Equal(Rectangle.Empty, actual.SrcRect);
            Assert.Equal(Rectangle.Empty, actual.DestRect);
            Assert.Equal(0f, actual.Size);
            Assert.Equal(0f, actual.Angle);
            Assert.Equal(Color.Empty, actual.TintColor);
        }

        [Fact]
        public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue()
        {
            // Arrange
            var item = new SpriteBatchItem();

            // Act
            var actual = item.IsEmpty;

            // Assert
            Assert.True(actual);
        }
    }
}
