// <copyright file="AppSettingsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests
{
    using FluentAssertions;
    using Velaptor;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="AppSettings"/> class.
    /// </summary>
    public class AppSettingsTests
    {
        #region Property Tests
        [Fact]
        public void WindowWidth_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var settings = new AppSettings();

            // Act & Assert
            settings.WindowWidth.Should().Be(1500);
        }

        [Fact]
        public void WindowHeight_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var settings = new AppSettings();

            // Act & Assert
            settings.WindowHeight.Should().Be(800);
        }
        #endregion
    }
}
