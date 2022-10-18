// <copyright file="FontStatsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using Velaptor.Content.Fonts;
    using Xunit;

    public class FontStatsTests
    {
        #region Method Tests
        [Theory]
        [InlineData(@"C:/Windows/Fonts/test-font.ttf", "Times New Roman", FontStyle.Regular, true)]
        [InlineData(@"C:/Windows/Fonts/test-font.ttf", "Times New Roman", FontStyle.Bold | FontStyle.Italic, false)]
        [InlineData(@"C:/other-file.ttf", "Times New Roman", FontStyle.Regular, false)]
        [InlineData(@"C:/Windows/Fonts/test-font.ttf", "Arial", FontStyle.Regular, false)]
        [InlineData(@"C:/Windows/Fonts/test-font.ttf", "Times New Roman", FontStyle.Bold, false)]
        public void Equals_WhenInvoked_ReturnsCorrectResult(string filePath, string family, FontStyle style, bool expected)
        {
            // Arrange
            var statsA = default(FontStats);
            statsA.FontFilePath = @"C:/Windows/Fonts/test-font.ttf";
            statsA.FamilyName = "Times New Roman";
            statsA.Style = FontStyle.Regular;

            var statsB = default(FontStats);
            statsB.FontFilePath = filePath;
            statsB.FamilyName = family;
            statsB.Style = style;

            // Act
            var actual = statsA.Equals(statsB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equals_WithObjParamWithMatchingType_ReturnsTrue()
        {
            // Arrange
            var statsA = default(FontStats);
            statsA.FontFilePath = @"C:/Windows/Fonts/test-font.ttf";
            statsA.FamilyName = "Times New Roman";
            statsA.Style = FontStyle.Regular;

            object statsB = new FontStats()
            {
                FontFilePath = @"C:/Windows/Fonts/test-font.ttf",
                FamilyName = "Times New Roman",
                Style = FontStyle.Regular,
            };

            // Act
            var actual = statsA.Equals(statsB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WithObjParamWithNonMatchingType_ReturnsFalse()
        {
            // Arrange
            var statsA = default(FontStats);
            statsA.FontFilePath = @"C:/Windows/Fonts/test-font.ttf";
            statsA.FamilyName = "Times New Roman";
            statsA.Style = FontStyle.Regular;

            var statsB = new object();

            // Act
            var actual = statsA.Equals(statsB);

            // Assert
            Assert.False(actual);
        }
        #endregion
    }
}
