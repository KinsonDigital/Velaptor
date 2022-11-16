// <copyright file="EventLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.IO.Abstractions;
using Velaptor.Guards;

namespace Velaptor.Services;

/// <inheritdoc/>
internal sealed class EventLoggerService : IEventLoggerService
{
    private const string LogFilePrefix = "event-logs-";
    private const string LogsDirName = "logs";
    private readonly IFile file;
    private readonly IDirectory directory;
    private readonly IConsoleService consoleService;
    private readonly IDateTimeService dateTimeService;
    private readonly IAppSettingsService appSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLoggerService"/> class.
    /// </summary>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="consoleService">Writes to the console.</param>
    /// <param name="dateTimeService">Gets the current date and time.</param>
    /// <param name="appSettingsService">Provides access to application settings.</param>
    public EventLoggerService(
        IDirectory directory,
        IFile file,
        IConsoleService consoleService,
        IDateTimeService dateTimeService,
        IAppSettingsService appSettingsService)
    {
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(consoleService);
        EnsureThat.ParamIsNotNull(dateTimeService);
        EnsureThat.ParamIsNotNull(appSettingsService);

        this.directory = directory;
        this.file = file;
        this.consoleService = consoleService;
        this.dateTimeService = dateTimeService;
        this.appSettingsService = appSettingsService;
    }

    /// <inheritdoc/>
    public void Event(string eventName, string msg)
    {
        if (this.appSettingsService.Settings.LoggingEnabled is false)
        {
            return;
        }

        if (this.appSettingsService.Settings.ConsoleLoggingEnabled)
        {
            LogToConsole(eventName, msg);
        }

        if (this.appSettingsService.Settings.FileLoggingEnabled)
        {
            LogToFile(eventName, msg);
        }
    }

    /// <summary>
    /// Logs the given <paramref name="msg"/> with the given <paramref name="eventName"/> to the console.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="msg">The message to log.</param>
    private void LogToConsole(string eventName, string msg)
    {
        var time = this.dateTimeService.Now().ToString("HH:m:s");

        var clr = this.consoleService.ForegroundColor;

        this.consoleService.ForegroundColor = ConsoleColor.DarkGray;
        this.consoleService.Write("[");

        this.consoleService.ForegroundColor = ConsoleColor.White;
        this.consoleService.Write(time);

        this.consoleService.ForegroundColor = ConsoleColor.Cyan;
        this.consoleService.Write(" EVENT");

        this.consoleService.ForegroundColor = ConsoleColor.DarkGray;
        this.consoleService.Write("(");

        this.consoleService.ForegroundColor = ConsoleColor.DarkCyan;
        this.consoleService.Write(eventName);

        this.consoleService.ForegroundColor = ConsoleColor.DarkGray;
        this.consoleService.Write(")");
        this.consoleService.Write("]");

        this.consoleService.ForegroundColor = ConsoleColor.White;
        this.consoleService.Write($" {msg}");

        this.consoleService.ForegroundColor = clr;
    }

    /// <summary>
    /// Logs the given <paramref name="msg"/> with the given <paramref name="eventName"/> to a file.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="msg">The message to log.</param>
    private void LogToFile(string eventName, string msg)
    {
        var dateAndTime = this.dateTimeService.Now();
        var date = dateAndTime.ToString("yyyyMMdd");
        var baseDirPath = this.directory.GetCurrentDirectory().ToCrossPlatPath();
        var fileName = $"{LogFilePrefix}{date}";
        var filePath = $"{baseDirPath}/{LogsDirName}/{fileName}.txt";
        var isFirstEntry = false;

        if (this.file.Exists(filePath) is false)
        {
            this.file.WriteAllText(filePath, string.Empty);
            isFirstEntry = true;
        }

        var time = dateAndTime.ToString("HH:mm:ss");
        var newLine = isFirstEntry ? string.Empty : Environment.NewLine;
        var textToAppend = $"[{time} EVENT({eventName})] {msg}";

        this.file.AppendAllText(filePath, $"{newLine}{textToAppend}");
    }
}
