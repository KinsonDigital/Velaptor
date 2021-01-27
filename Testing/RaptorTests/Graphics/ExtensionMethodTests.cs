// <copyright file="ExtensionMethodTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using Raptor.Graphics;
    using Xunit;

    public class ExtensionMethodTests
    {
        #region Method Tests
        [Fact]
        public void IsLetter_WhenInvokedWithUpperCaseLetter_ReturnsTrue()
        {
            // Arrange
            var letter = 'Z';
            var expected = true;

            // Act
            var actual = letter.IsLetter();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsLetter_WhenInvokedWithLowerCaseLetter_ReturnsTrue()
        {
            // Arrange
            var letter = 'z';
            var expected = true;

            // Act
            var actual = letter.IsLetter();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsLetter_WhenInvokedWithNumber_ReturnsFalse()
        {
            // Arrange
            var digit = '3';
            var expected = false;

            // Act
            var actual = digit.IsLetter();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsNumber_WhenInvokedWithNumber_ReturnsTrue()
        {
            // Arrange
            var digit = '4';
            var expected = true;

            // Act
            var actual = digit.IsNumber();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsNumber_WhenInvokedWithLetter_ReturnsFalse()
        {
            // Arrange
            var letter = 'T';
            var expected = false;

            // Act
            var actual = letter.IsNumber();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFirstOccurenceOfNumber_WhenInvokedWithNumberInString_ReturnsCorrectIndex()
        {
            // Arrange
            var data = "This number 1234 is my favorite number!";
            var expected = 1234;

            // Act
            var actual = data.GetFirstOccurenceOfNumber();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFirstOccurenceOfNumber_WhenInvokedWithNoNumbersInString_ReturnsCorrectResult()
        {
            // Arrange
            var data = "No number is my favorite number!";
            var expected = -1;

            // Act
            var actual = data.GetFirstOccurenceOfNumber();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HasNumbers_WhenInvokedWithNumbers_ReturnsTrue()
        {
            // Arrange
            var data = "The number 1234 is my worst number!!";
            var expected = true;

            // Act
            var actual = data.HasNumbers();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("TheNumber445IsANumber!", false)]
        [InlineData("TheNumber445IsANumber", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsAlphaNumeric_WhenInvoked_ReturnsCorrectValue(string data, bool expected)
        {
            // Act
            var actual = data.IsAlphaNumeric();

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
