// <copyright file="LoggingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Guards;

namespace Velaptor.Services;

/// <inheritdoc/>
internal sealed class LoggingService : ILoggingService
{
    private readonly IAppSettingsService appSettingsService;
    private readonly IConsoleLoggerService consoleLoggerService;
    private readonly IFileLoggerService fileLoggerService;
    private readonly IEventLoggerService eventLoggerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingService"/> class.
    /// </summary>
    /// <param name="appSettingsService">Provides access to application settings.</param>
    /// <param name="consoleLoggerService">Logs messages to the console.</param>
    /// <param name="fileLoggerService">Logs messages to a file.</param>
    /// <param name="eventLoggerService">Logs events to the console or a file.</param>
    public LoggingService(
        IAppSettingsService appSettingsService,
        IConsoleLoggerService consoleLoggerService,
        IFileLoggerService fileLoggerService,
        IEventLoggerService eventLoggerService)
    {
        EnsureThat.ParamIsNotNull(appSettingsService);
        EnsureThat.ParamIsNotNull(consoleLoggerService);
        EnsureThat.ParamIsNotNull(fileLoggerService);
        EnsureThat.ParamIsNotNull(eventLoggerService);

        this.appSettingsService = appSettingsService;
        this.consoleLoggerService = consoleLoggerService;
        this.fileLoggerService = fileLoggerService;
        this.eventLoggerService = eventLoggerService;
    }

    /// <inheritdoc/>
    public void Info(string msg)
    {
        if (this.appSettingsService.Settings.LoggingEnabled is false)
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
        if (this.appSettingsService.Settings.LoggingEnabled is false)
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
        if (this.appSettingsService.Settings.LoggingEnabled is false)
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
    public void Event(string eventName, string msg) => this.eventLoggerService.Event(eventName, msg);
}
