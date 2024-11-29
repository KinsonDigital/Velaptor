// <copyright file="EventLoggerServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.IO.Abstractions;
using FluentAssertions;
using NSubstitute;
using Velaptor;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="EventLoggerService"/> class.
/// </summary>
public class EventLoggerServiceTests
{
    private readonly IDirectory mockDir;
    private readonly IFile mockFile;
    private readonly IConsoleService mockConsoleService;
    private readonly IDateTimeService mockDateTimeService;
    private readonly IAppSettingsService mockAppSettingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLoggerServiceTests"/> class.
    /// </summary>
    public EventLoggerServiceTests()
    {
        this.mockDir = Substitute.For<IDirectory>();
        this.mockFile = Substitute.For<IFile>();
        this.mockConsoleService = Substitute.For<IConsoleService>();
        this.mockDateTimeService = Substitute.For<IDateTimeService>();
        this.mockAppSettingService = Substitute.For<IAppSettingsService>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new EventLoggerService(
                null,
                this.mockFile,
                this.mockConsoleService,
                this.mockDateTimeService,
                this.mockAppSettingService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new EventLoggerService(
                this.mockDir,
                null,
                this.mockConsoleService,
                this.mockDateTimeService,
                this.mockAppSettingService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullConsoleServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new EventLoggerService(
                this.mockDir,
                this.mockFile,
                null,
                this.mockDateTimeService,
                this.mockAppSettingService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'consoleService')");
    }

    [Fact]
    public void Ctor_WithNullDateTimeServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new EventLoggerService(
                this.mockDir,
                this.mockFile,
                this.mockConsoleService,
                null,
                this.mockAppSettingService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'dateTimeService')");
    }

    [Fact]
    public void Ctor_WithNullAppSettingsServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new EventLoggerService(
                this.mockDir,
                this.mockFile,
                this.mockConsoleService,
                this.mockDateTimeService,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'appSettingsService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Event_WhenAllLoggingIsDisabled_DoesNotLogAnything()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            LoggingEnabled = false,
        };

        this.mockAppSettingService.Settings.Returns(appSettings);

        var sut = CreateService();

        // Act
        sut.Event("test-event", "event msg");

        // Assert
        this.mockDateTimeService.DidNotReceive().Now();
        this.mockConsoleService.DidNotReceive().Write(Arg.Any<string>());
        this.mockConsoleService.DidNotReceive().WriteLine(Arg.Any<string>());
        this.mockDir.DidNotReceive().GetCurrentDirectory();
        this.mockFile.DidNotReceive().Exists(Arg.Any<string>());
        this.mockFile.DidNotReceive().WriteAllText(Arg.Any<string>(), Arg.Any<string>());
        this.mockFile.DidNotReceive().AppendAllText(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void Event_WithConsoleLoggingEnabled_LogsToConsole()
    {
        // Arrange
        MockTime(18, 36, 12);

        var sut = CreateService();

        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = true,
            FileLoggingEnabled = false,
        };

        this.mockAppSettingService.Settings.Returns(appSettings);

        // Act
        sut.Event("test-event", "event msg");

        // Assert
        this.mockConsoleService.Received(3).ForegroundColor = ConsoleColor.DarkGray;
        this.mockConsoleService.Received(1).ForegroundColor = ConsoleColor.Cyan;
        this.mockConsoleService.Received(1).ForegroundColor = ConsoleColor.DarkCyan;
        this.mockConsoleService.Received(2).ForegroundColor = ConsoleColor.White;

        this.mockConsoleService.Received(1).Write("[");
        this.mockConsoleService.Received(1).Write("18:36:12");
        this.mockConsoleService.Received(1).Write(" EVENT");
        this.mockConsoleService.Received(1).Write("(");
        this.mockConsoleService.Received(1).Write("test-event");
        this.mockConsoleService.Received(1).Write(")");
        this.mockConsoleService.Received(1).Write("]");

        this.mockConsoleService.Received(1).Write(" event msg");
    }

    [Fact]
    public void Event_WithFileLoggingEnabledAndFileDoesExist_LogsToFile()
    {
        // Arrange
        const string expectedTextToAppend = "[09:15:57 EVENT(test-event)] event msg";
        const string logsDirName = "logs";
        const string baseDirPath = "C:/app-dir";
        const string logFileName = "event-logs-20221024.txt";
        const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

        MockDateAndTime(2022, 10, 24, 9, 15, 57);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        this.mockDir.GetCurrentDirectory().Returns(baseDirPath);

        var sut = CreateService();
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingService.Settings.Returns(appSettings);

        // Act
        sut.Event("test-event", "event msg");

        // Assert
        this.mockFile.Received(1).Exists(logFilePath);
        this.mockFile.Received(1).AppendAllText(logFilePath, expectedTextToAppend);
    }

    [Fact]
    public void Event_WithFileLoggingEnabledAndFileAlreadyExists_LogsToFile()
    {
        // Arrange
        var expectedTextToAppend = $"{Environment.NewLine}[09:15:57 EVENT(test-event)] event msg";
        const string logsDirName = "logs";
        const string baseDirPath = "C:/app-dir";
        const string logFileName = "event-logs-20221024.txt";
        const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

        MockDateAndTime(2022, 10, 24, 9, 15, 57);
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockDir.GetCurrentDirectory().Returns(baseDirPath);

        var sut = CreateService();
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingService.Settings.Returns(appSettings);

        // Act
        sut.Event("test-event", "event msg");

        // Assert
        this.mockFile.Received(1).Exists(logFilePath);
        this.mockFile.Received(1).AppendAllText(logFilePath, expectedTextToAppend);
    }

    [Fact]
    public void Event_WithFileLoggingEnabledAndFileDoesNotExist_LogsToFile()
    {
        // Arrange
        const string expectedTextToAppend = "[14:08:23 EVENT(test-event)] event msg";
        const string logsDirName = "logs";
        const string baseDirPath = "C:/app-dir";
        const string logFileName = "event-logs-20220902.txt";
        const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

        MockDateAndTime(2022, 09, 02, 14, 08, 23);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);
        this.mockDir.GetCurrentDirectory().Returns(baseDirPath);

        var sut = CreateService();
        var appSettings = new AppSettings
        {
            LoggingEnabled = true,
            ConsoleLoggingEnabled = false,
            FileLoggingEnabled = true,
        };

        this.mockAppSettingService.Settings.Returns(appSettings);

        // Act
        sut.Event("test-event", "event msg");

        // Assert
        this.mockFile.Received(1).Exists(logFilePath);
        this.mockFile.Received(1).WriteAllText(logFilePath, string.Empty);
        this.mockFile.Received(1).AppendAllText(logFilePath, expectedTextToAppend);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="EventLoggerService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private EventLoggerService CreateService()
        => new (this.mockDir,
            this.mockFile,
            this.mockConsoleService,
            this.mockDateTimeService,
            this.mockAppSettingService);

    /// <summary>
    /// Mocks the time using the given <paramref name="hour"/>, <paramref name="minute"/>, and <paramref name="second"/>.
    /// </summary>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    private void MockTime(int hour, int minute, int second) =>
        this.mockDateTimeService.Now().Returns(new DateTime(2022, 1, 2, hour, minute, second, 0));

    /// <summary>
    /// Mocks the date and time using the given values.
    /// </summary>
    private void MockDateAndTime(int year, int month, int day, int hour, int minute, int second) =>
        this.mockDateTimeService.Now().Returns(new DateTime(year, month, day, hour, minute, second, 0));
}
