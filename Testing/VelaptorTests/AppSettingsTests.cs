// <copyright file="AppSettingsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Velaptor;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests;

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
    public void ConsoleLoggingEnabled_WhenGettingValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.ConsoleLoggingEnabled.Should().BeTrue();
    }

    [Fact]
    public void FileLoggingEnabled_WhenGettingValue_ReturnsTrue()
    {
        // Arrange
        var settings = new AppSettings();

        // Act & Assert
        settings.FileLoggingEnabled.Should().BeTrue();
    }
    #endregion
}
