// <copyright file="LoggingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using FluentAssertions;
using Moq;
using Serilog;
using Velaptor;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="LoggingService"/> class.
/// </summary>
public class LoggingServiceTests
{
    private readonly Mock<IAppSettingsService> mockAppSettingsService;
    private readonly Mock<IConsoleLoggerService> mockConsoleLoggerService;
    private readonly Mock<IFileLoggerService>  mockFileLoggerService;
    private readonly Mock<IEventLoggerService> mockEventLoggerService;
    private readonly Mock<ILogger> mockConsoleLogger;
    private readonly Mock<ILogger> mockFileLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingServiceTests"/> class.
    /// </summary>
    public LoggingServiceTests()
    {
        this.mockAppSettingsService = new Mock<IAppSettingsService>();

        this.mockConsoleLogger = new Mock<ILogger>();

        this.mockConsoleLoggerService = new Mock<IConsoleLoggerService>();
        this.mockConsoleLoggerService.SetupGet(p => p.Logger).Returns(this.mockConsoleLogger.Object);

        this.mockFileLogger = new Mock<ILogger>();

        this.mockFileLoggerService = new Mock<IFileLoggerService>();
        this.mockFileLoggerService.SetupGet(p => p.Logger).Returns(this.mockFileLogger.Object);

        this.mockEventLoggerService = new Mock<IEventLoggerService>();
    }

    #region Constructor Tests

    [Fact]
    public void Ctor_WithNullAppSettingsServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LoggingService(
                null,
                this.mockConsoleLoggerService.Object,
                this.mockFileLoggerService.Object,
                this.mockEventLoggerService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'appSettingsService')");
    }

    [Fact]
    public void Ctor_WithNullConsoleLoggerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LoggingService(
                this.mockAppSettingsService.Object,
                null,
                this.mockFileLoggerService.Object,
                this.mockEventLoggerService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'consoleLoggerService')");
    }

    [Fact]
    public void Ctor_WithNullFileLoggerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LoggingService(
                this.mockAppSettingsService.Object,
                this.mockConsoleLoggerService.Object,
                null,
                this.mockEventLoggerService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'fileLoggerService')");
    }

    [Fact]
    public void Ctor_WithNullEventLoggerServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LoggingService(
                this.mockAppSettingsService.Object,
                this.mockConsoleLoggerService.Object,
                this.mockFileLoggerService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'eventLoggerService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Info_WithAllLoggingDisabledButConsoleAndFileLoggingEnabled_DoesNotLogAnything()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = false,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info(It.IsAny<string>());

        // Assert
        VerifyConsoleLogs(Times.Never());
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Info_WithOnlyConsoleLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = false,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Info_WithOnlyFileLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockFileLogger.Verify(m => m.Information("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyConsoleLogs(Times.Never());
    }

    [Fact]
    public void Info_WithFileAndConsoleLoggingEnabled_LogsBothToConsoleAndFile()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Information("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Warning_WithAllLoggingDisabledButConsoleAndFileLoggingEnabled_DoesNotLogAnything()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = false,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning(It.IsAny<string>());

        // Assert
        VerifyConsoleLogs(Times.Never());
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Warning_WithOnlyConsoleLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = false,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Warning("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Warning_WithOnlyFileLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockFileLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Warning("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyConsoleLogs(Times.Never());
    }

    [Fact]
    public void Warning_WithFileAndConsoleLoggingEnabled_LogsBothToConsoleAndFile()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Warning("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Warning("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Error(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Error_WithAllLoggingDisabledButConsoleAndFileLoggingEnabled_DoesNotLogAnything()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = false,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error(It.IsAny<string>());

        // Assert
        VerifyConsoleLogs(Times.Never());
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Error_WithOnlyConsoleLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = false,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Error("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyFileLogs(Times.Never());
    }

    [Fact]
    public void Error_WithOnlyFileLoggingEnabled_LogsToConsole()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockFileLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Error("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        VerifyConsoleLogs(Times.Never());
    }

    [Fact]
    public void Error_WithFileAndConsoleLoggingEnabled_LogsBothToConsoleAndFile()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingsService.SetupGet(p => p.Settings).Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockConsoleLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockConsoleLogger.Verify(m => m.Error("test-info-msg"), Times.Once);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Information(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Warning(It.IsAny<string>()), Times.Never);
        this.mockFileLogger.Verify(m => m.Error("test-info-msg"), Times.Once);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Event_WhenInvoked_LogsEvent()
    {
        // Arrange
        var sut = CreateService();

        // Act
        sut.Event("test-event", "test msg");

        // Assert
        this.mockEventLoggerService.Verify(m => m.Event("test-event", "test msg"), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LoggingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LoggingService CreateService()
        => new (this.mockAppSettingsService.Object,
            this.mockConsoleLoggerService.Object,
            this.mockFileLoggerService.Object,
            this.mockEventLoggerService.Object);

    /// <summary>
    /// Verifies that logging methods for the file logger were not invoked.
    /// </summary>
    private void VerifyFileLogs(Times times)
    {
        this.mockFileLogger.Verify(m => m.Information(It.IsAny<string>()), times);
        this.mockFileLogger.Verify(m => m.Warning(It.IsAny<string>()), times);
        this.mockFileLogger.Verify(m => m.Error(It.IsAny<string>()), times);
        this.mockFileLogger.Verify(m => m.Fatal(It.IsAny<string>()), times);
    }

    /// <summary>
    /// Verifies that logging methods for the console logger were not invoked.
    /// </summary>
    private void VerifyConsoleLogs(Times times)
    {
        this.mockConsoleLogger.Verify(m => m.Information(It.IsAny<string>()), times);
        this.mockConsoleLogger.Verify(m => m.Warning(It.IsAny<string>()), times);
        this.mockConsoleLogger.Verify(m => m.Error(It.IsAny<string>()), times);
        this.mockConsoleLogger.Verify(m => m.Fatal(It.IsAny<string>()), times);
    }
}
