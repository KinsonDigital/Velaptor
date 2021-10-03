// <copyright file="ExtensionMethodTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics
{
    using Velaptor.Content;
    using Xunit;

    /// <summary>
    /// Tests the various helper extension methods.
    /// </summary>
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

        [Theory]
        [InlineData("1234", 1234)]
        [InlineData("The number 5678", 5678)]
        [InlineData("No number here", -1)]
        public void GetFirstOccurenceOfNumber_WhenInvokedWithNumberInString_ReturnsCorrectIndex(string value, int expected)
        {
            // Act
            var actual = value.GetFirstOccurenceOfNumber();

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
        public void IsAlphaNumeric_WhenInvoked_ReturnsCorrectResult(string data, bool expected)
        {
            // Act
            var actual = data.IsAlphaNumeric();

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
