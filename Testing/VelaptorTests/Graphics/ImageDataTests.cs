// <copyright file="ImageDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using System.Drawing;
    using Velaptor.Graphics;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ImageData"/> struct.
    /// </summary>
    public class ImageDataTests
    {
        #region Method Tests
        [Fact]
        public void DrawImage_WhenDrawingWithingBounds_DrawsWholeImageOntoOther()
        {
            // Arrange
            var targetImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 0, 255), 10, 10);
            var srcImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 128, 0), 6, 6);

            // Act
            var actual = targetImgData.DrawImage(srcImgData, new Point(2, 2));

            bool ClrMatches(Color clrA, Color clrB)
            {
                return clrA.A == clrB.A &&
                       clrA.R == clrB.R &&
                       clrA.G == clrB.G &&
                       clrA.B == clrB.B;
            }

            // First 2 rows
            var row0 = TestHelpers.GetRow(actual, 0);
            var row1 = TestHelpers.GetRow(actual, 1);

            // Middle rows
            var row2 = TestHelpers.GetRow(actual, 2, 2, actual.Width - (2 + 1));
            var row3 = TestHelpers.GetRow(actual, 3, 2, actual.Width - (2 + 1));
            var row4 = TestHelpers.GetRow(actual, 4, 2, actual.Width - (2 + 1));
            var row5 = TestHelpers.GetRow(actual, 5, 2, actual.Width - (2 + 1));
            var row6 = TestHelpers.GetRow(actual, 6, 2, actual.Width - (2 + 1));
            var row7 = TestHelpers.GetRow(actual, 7, 2, actual.Width - (2 + 1));

            // Last 2 rows
            var row8 = TestHelpers.GetRow(actual, 8);
            var row9 = TestHelpers.GetRow(actual, 9);

            // First 2 columns
            var col0 = TestHelpers.GetColumn(actual, 0);
            var col1 = TestHelpers.GetColumn(actual, 1);

            // Last 2 columns
            var col8 = TestHelpers.GetColumn(actual, 8);
            var col9 = TestHelpers.GetColumn(actual, 9);

            // Assert
            // First 2 rows
            Assert.All(row0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(row1, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(row2, clr => Assert.True(ClrMatches(clr, Color.Green)));

            // Middle rows
            Assert.All(row3, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row4, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row5, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row6, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row7, clr => Assert.True(ClrMatches(clr, Color.Green)));

            // Last 2 rows
            Assert.All(row8, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(row9, clr => Assert.True(ClrMatches(clr, Color.Blue)));

            // First 2 columns
            Assert.All(col0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(col1, clr => Assert.True(ClrMatches(clr, Color.Blue)));

            // Last 2 columns
            Assert.All(col8, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(col9, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        }

        [Fact]
        public void DrawImage_WithWidthAndHeightLargerThenTarget_DrawsPartialSourceImageOntoTarget()
        {
            // Arrange
            var targetImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 0, 255), 6, 6);
            var srcImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 128, 0), 10, 10);

            // Act
            var actual = targetImgData.DrawImage(srcImgData, new Point(2, 2));

            bool ClrMatches(Color clrA, Color clrB)
            {
                return clrA.A == clrB.A &&
                       clrA.R == clrB.R &&
                       clrA.G == clrB.G &&
                       clrA.B == clrB.B;
            }

            // First top 2 blue rows
            var row0 = TestHelpers.GetRow(actual, 0);
            var row1 = TestHelpers.GetRow(actual, 1);

            // Green rows below top 2 blue rows
            var row2 = TestHelpers.GetRow(actual, 2, 2, 5);
            var row3 = TestHelpers.GetRow(actual, 3, 2, 5);
            var row4 = TestHelpers.GetRow(actual, 4, 2, 5);
            var row5 = TestHelpers.GetRow(actual, 5, 2, 5);

            // First 2 blue columns
            var col0 = TestHelpers.GetColumn(actual, 0);
            var col1 = TestHelpers.GetColumn(actual, 1);

            // Assert
            // First top 2 blue rows
            Assert.All(row0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(row1, clr => Assert.True(ClrMatches(clr, Color.Blue)));

            // Green rows below top 2 blue rows
            Assert.All(row2, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row3, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row4, clr => Assert.True(ClrMatches(clr, Color.Green)));
            Assert.All(row5, clr => Assert.True(ClrMatches(clr, Color.Green)));

            // First 2 blue columns columns
            Assert.All(col0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
            Assert.All(col1, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        }

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
