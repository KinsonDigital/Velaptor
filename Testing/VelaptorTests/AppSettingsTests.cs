// <copyright file="AppSettingsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using FluentAssertions;
using Velaptor;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="AppSettings"/> class.
/// </summary>
public class AppSettingsTests
{
    #region Property Tests
    [Fact]
    public void WindowWidth_WhenGettingInitializedValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings
        {
            WindowWidth = 1111,
        };

        // Act & Assert
        settings.WindowWidth.Should().Be(1111);
    }

    [Fact]
    public void WindowHeight_WhenGettingInitializedValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings
        {
            WindowHeight = 1111,
        };

        // Act & Assert
        settings.WindowHeight.Should().Be(1111);
    }

    [Fact]
    public void WindowWidth_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.WindowWidth.Should().Be(1500);
    }

    [Fact]
    public void WindowHeight_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.WindowHeight.Should().Be(800);
    }

    [FactForDebug]
    public void LoggingEnabled_WhenGettingValueInDebugMode_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.LoggingEnabled.Should().BeTrue();
    }

    [FactForRelease]
    public void LoggingEnabled_WhenGettingValueInReleaseMode_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.LoggingEnabled.Should().BeFalse();
    }

    [Fact]
    public void ConsoleLoggingEnabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.ConsoleLoggingEnabled.Should().BeTrue();
    }

    [Fact]
    public void FileLoggingEnabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.FileLoggingEnabled.Should().BeTrue();
    }
    #endregion
}
