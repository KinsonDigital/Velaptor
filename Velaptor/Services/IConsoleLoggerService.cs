// <copyright file="IConsoleLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using Serilog;

/// <summary>
/// Logs messages of different types to the console.
/// </summary>
internal interface IConsoleLoggerService
{
    /// <summary>
    /// Gets the logger that logs to the console.
    /// </summary>
    public ILogger Logger { get; }
}
