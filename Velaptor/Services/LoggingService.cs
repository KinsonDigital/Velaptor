// <copyright file="LoggingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics;
using System.IO.Abstractions;

/// <inheritdoc/>
internal sealed class LoggingService : ILoggingService
{
    private readonly IAppSettingsService appSettingsService;
    private readonly IConsoleLoggerService consoleLoggerService;
    private readonly IFileLoggerService fileLoggerService;
    private readonly IEventLoggerService eventLoggerService;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingService"/> class.
    /// </summary>
    /// <param name="appSettingsService">Provides access to application settings.</param>
    /// <param name="consoleLoggerService">Logs messages to the console.</param>
    /// <param name="fileLoggerService">Logs messages to a file.</param>
    /// <param name="eventLoggerService">Logs events to the console or a file.</param>
    /// <param name="path">Processes directory and file paths.</param>
    public LoggingService(
        IAppSettingsService appSettingsService,
        IConsoleLoggerService consoleLoggerService,
        IFileLoggerService fileLoggerService,
        IEventLoggerService eventLoggerService,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(appSettingsService);
        ArgumentNullException.ThrowIfNull(consoleLoggerService);
        ArgumentNullException.ThrowIfNull(fileLoggerService);
        ArgumentNullException.ThrowIfNull(eventLoggerService);
        ArgumentNullException.ThrowIfNull(path);

        this.appSettingsService = appSettingsService;
        this.consoleLoggerService = consoleLoggerService;
        this.fileLoggerService = fileLoggerService;
        this.eventLoggerService = eventLoggerService;
        this.path = path;
    }

    /// <inheritdoc/>
    public void Info(string msg)
    {
        if (!this.appSettingsService.Settings.LoggingEnabled)
        {
            return;
        }

        if (this.appSettingsService.Settings.ConsoleLoggingEnabled)
        {
            this.consoleLoggerService.Logger.Information(msg);
        }

        if (this.appSettingsService.Settings.FileLoggingEnabled)
        {
            this.fileLoggerService.Logger.Information(msg);
        }
    }

    /// <inheritdoc/>
    public void Warning(string msg)
    {
        if (!this.appSettingsService.Settings.LoggingEnabled)
        {
            return;
        }

        if (this.appSettingsService.Settings.ConsoleLoggingEnabled)
        {
            this.consoleLoggerService.Logger.Warning(msg);
        }

        if (this.appSettingsService.Settings.FileLoggingEnabled)
        {
            this.fileLoggerService.Logger.Warning(msg);
        }
    }

    /// <inheritdoc/>
    public void Error(string msg)
    {
        if (!this.appSettingsService.Settings.LoggingEnabled)
        {
            return;
        }

        if (this.appSettingsService.Settings.ConsoleLoggingEnabled)
        {
            this.consoleLoggerService.Logger.Error(msg);
        }

        if (this.appSettingsService.Settings.FileLoggingEnabled)
        {
            this.fileLoggerService.Logger.Error(msg);
        }
    }

    /// <inheritdoc/>
    public void Error(Exception exception)
    {
        var fileAndLineNumber = string.Empty;

        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            var stackTrace = new StackTrace(exception, true);

            var frame = stackTrace.GetFrame(0);

            if (frame is not null)
            {
                var fileName = this.path.GetFileName(frame.GetFileName());
                fileAndLineNumber = $"{fileName}#{frame.GetFileLineNumber()} - ";
            }
        }

        Error($"{fileAndLineNumber}{exception.Message}");
    }

    /// <inheritdoc/>
    public void Event(string eventName, string msg) => this.eventLoggerService.Event(eventName, msg);
}
