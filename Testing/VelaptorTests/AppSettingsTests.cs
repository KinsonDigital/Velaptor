// <copyright file="AppSettingsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using Helpers;
using Shouldly;
using Velaptor;
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
        settings.WindowWidth.ShouldBe(1111u);
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
        settings.WindowHeight.ShouldBe(1111u);
    }

    [Fact]
    public void WindowWidth_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.WindowWidth.ShouldBe(1280u);
    }

    [Fact]
    public void WindowHeight_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.WindowHeight.ShouldBe(720u);
    }

    [FactForDebug]
    public void LoggingEnabled_WhenGettingValueInDebugMode_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.LoggingEnabled.ShouldBeTrue();
    }

    [FactForRelease]
    public void LoggingEnabled_WhenGettingValueInReleaseMode_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.LoggingEnabled.ShouldBeFalse();
    }

    [Fact]
    public void ConsoleLoggingEnabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.ConsoleLoggingEnabled.ShouldBeTrue();
    }

    [Fact]
    public void FileLoggingEnabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.FileLoggingEnabled.ShouldBeTrue();
    }
    #endregion
}
