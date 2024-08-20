// <copyright file="LoggingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using FluentAssertions;
using NSubstitute;
using Serilog;
using Velaptor;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="LoggingService"/> class.
/// </summary>
public class LoggingServiceTests
{
    private readonly IAppSettingsService mockAppSettingsService;
    private readonly IConsoleLoggerService mockConsoleLoggerService;
    private readonly IFileLoggerService mockFileLoggerService;
    private readonly IEventLoggerService mockEventLoggerService;
    private readonly ILogger mockConsoleLogger;
    private readonly ILogger mockFileLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingServiceTests"/> class.
    /// </summary>
    public LoggingServiceTests()
    {
        this.mockAppSettingsService = Substitute.For<IAppSettingsService>();

        this.mockConsoleLogger = Substitute.For<ILogger>();

        this.mockConsoleLoggerService = Substitute.For<IConsoleLoggerService>();
        this.mockConsoleLoggerService.Logger.Returns(this.mockConsoleLogger);

        this.mockFileLogger = Substitute.For<ILogger>();

        this.mockFileLoggerService = Substitute.For<IFileLoggerService>();
        this.mockFileLoggerService.Logger.Returns(this.mockFileLogger);

        this.mockEventLoggerService = Substitute.For<IEventLoggerService>();
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
                this.mockConsoleLoggerService,
                this.mockFileLoggerService,
                this.mockEventLoggerService);
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
                this.mockAppSettingsService,
                null,
                this.mockFileLoggerService,
                this.mockEventLoggerService);
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
                this.mockAppSettingsService,
                this.mockConsoleLoggerService,
                null,
                this.mockEventLoggerService);
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
                this.mockAppSettingsService,
                this.mockConsoleLoggerService,
                this.mockFileLoggerService,
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info(string.Empty);

        // Assert
        VerifyConsoleLogs(0);
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockConsoleLogger.Received(1).Information("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockFileLogger.Received(1).Information("test-info-msg");
        this.mockFileLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyConsoleLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Info("test-info-msg");

        // Assert
        this.mockConsoleLogger.Received(1).Information("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        this.mockFileLogger.Received(1).Information("test-info-msg");
        this.mockFileLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning(string.Empty);

        // Assert
        VerifyConsoleLogs(0);
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockConsoleLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockConsoleLogger.Received(1).Warning("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockFileLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockFileLogger.Received(1).Warning("test-info-msg");
        this.mockFileLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyConsoleLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Warning("test-info-msg");

        // Assert
        this.mockConsoleLogger.Received(1).Warning("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockFileLogger.Received(1).Warning("test-info-msg");
        this.mockFileLogger.DidNotReceive().Error(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error(string.Empty);

        // Assert
        VerifyConsoleLogs(0);
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockConsoleLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockConsoleLogger.Received(1).Error("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyFileLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockFileLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockFileLogger.Received(1).Error("test-info-msg");
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
        VerifyConsoleLogs(0);
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

        this.mockAppSettingsService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Error("test-info-msg");

        // Assert
        this.mockConsoleLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockConsoleLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockConsoleLogger.Received(1).Error("test-info-msg");
        this.mockConsoleLogger.DidNotReceive().Fatal(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Information(Arg.Any<string>());
        this.mockFileLogger.DidNotReceive().Warning(Arg.Any<string>());
        this.mockFileLogger.Received(1).Error("test-info-msg");
        this.mockFileLogger.DidNotReceive().Fatal(Arg.Any<string>());
    }

    [Fact]
    public void Event_WhenInvoked_LogsEvent()
    {
        // Arrange
        var sut = CreateService();

        // Act
        sut.Event("test-event", "test msg");

        // Assert
        this.mockEventLoggerService.Received(1).Event("test-event", "test msg");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LoggingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LoggingService CreateService()
        => new (this.mockAppSettingsService,
            this.mockConsoleLoggerService,
            this.mockFileLoggerService,
            this.mockEventLoggerService);

    /// <summary>
    /// Verifies that logging methods for the file logger were not invoked.
    /// </summary>
    private void VerifyFileLogs(int times)
    {
        this.mockFileLogger.Received(times).Information(Arg.Any<string>());
        this.mockFileLogger.Received(times).Warning(Arg.Any<string>());
        this.mockFileLogger.Received(times).Error(Arg.Any<string>());
        this.mockFileLogger.Received(times).Fatal(Arg.Any<string>());
    }

    /// <summary>
    /// Verifies that logging methods for the console logger were not invoked.
    /// </summary>
    private void VerifyConsoleLogs(int times)
    {
        this.mockConsoleLogger.Received(times).Information(Arg.Any<string>());
        this.mockConsoleLogger.Received(times).Warning(Arg.Any<string>());
        this.mockConsoleLogger.Received(times).Error(Arg.Any<string>());
        this.mockConsoleLogger.Received(times).Fatal(Arg.Any<string>());
    }
}
