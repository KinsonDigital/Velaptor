// <copyright file="SpriteBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System.Drawing;
    using Velaptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Test the <see cref="SpriteBatchItem"/> struct.
    /// </summary>
    public class SpriteBatchItemTests
    {
        #region Prop Tests
        [Fact]
        public void Empty_WhenGettingValue_ReturnsCorrectResult()
        {
            // Act
            var actual = SpriteBatchItem.Empty;

            // Assert
            Assert.Equal(0u, actual.TextureId);
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
            var item = default(SpriteBatchItem);

            // Act
            var actual = item.IsEmpty;

            // Assert
            Assert.True(actual);
        }
        #endregion
    }
}
