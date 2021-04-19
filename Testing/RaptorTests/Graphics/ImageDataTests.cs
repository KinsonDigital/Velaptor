// <copyright file="ImageDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System.Drawing;
    using Raptor.Graphics;
    using RaptorTests.Helpers;
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
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndIsEqual_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndHasDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.Blue,  2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenBothAreSameTypeAndBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.White, 0, 0);
            var imageDataB = TestHelpers.CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndPixelLengthsAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenObjectIsNotCorrectType_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
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
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndHasDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            object imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenParamIsObjectTypeAndBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.White, 0, 0);
            object imageDataB = TestHelpers.CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA.Equals(imageDataB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothPixelLengthsAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void EqualsOperator_WhenBothHaveNoPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.White, 0, 0);
            var imageDataB = TestHelpers.CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA == imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothPixelLengthsAreNotEqual_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsTrue()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
            var imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NotEqualsOperator_WhenBothHaveNoPixels_ReturnsFalse()
        {
            // Arrange
            var imageDataA = TestHelpers.CreateImageData(Color.White, 0, 0);
            var imageDataB = TestHelpers.CreateImageData(Color.White, 0, 0);

            // Act
            var actual = imageDataA != imageDataB;

            // Assert
            Assert.False(actual);
        }
        #endregion
    }
}
