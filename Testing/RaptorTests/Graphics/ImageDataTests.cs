// <copyright file="ImageDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System.Drawing;
    using Raptor.Graphics;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ImageData"/> struct.
    /// </summary>
    public class ImageDataTests
    {
        #region Method Tests
        [Fact]
        public void Equals_WhenBothAreSameTypeAndPixelLengthsAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndIsEqual_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndHasDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.Blue,  2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.White, 0, 0);
            var imageDataB = CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndPixelLengthsAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenObjectIsNotCorrectType_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = new object();

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndIsEqual_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndHasDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.White, 0, 0);
            object imageDataB = CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothPixelLengthsAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.White, 0, 0);
            var imageDataB = CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothPixelLengthsAreNotEqual_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothHaveNoPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = CreateImageData(Color.White, 0, 0);
            var imageDataB = CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.False(actual);
        }
        #endregion

        /// <summary>
        /// Creates a new <see cref="ImageData"/> struct for the purpose of testing.
        /// </summary>
        /// <param name="color">The color to set all of the <see cref="ImageData.Pixels"/>.</param>
        /// <param name="width">The width of the image that the pixels represent.</param>
        /// <param name="height">The heightof the image that the pixels represent.</param>
        /// <returns>The struct to test.</returns>
        private ImageData CreateImageData(Color color, int width, int height)
        {
            ImageData result = default;

            result.Pixels = CreatePixels(color, width, height);
            result.Width = width;
            result.Height = height;

            return result;
        }

        /// <summary>
        /// Creates a new 2 dimensional array of pixel colors using the given <paramref name="color"/>
        /// with enough pixels to fill and image that has the given <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="color">The color of all the pixels.</param>
        /// <param name="width">The width of the image represented by the <see cref="ImageData.Pixels"/>.</param>
        /// <param name="height">The height of the image represented by the <see cref="ImageData.Pixels"/>.</param>
        /// <returns>The 2 dimensional array of pixels to test.</returns>
        private Color[,] CreatePixels(Color color, int width, int height)
        {
            var result = new Color[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    result[x, y] = color;
                }
            }

            return result;
        }
    }
}
