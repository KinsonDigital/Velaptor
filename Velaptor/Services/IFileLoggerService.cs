// <copyright file="IFileLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using Serilog;

/// <summary>
/// Logs messages of different types to a file.
/// </summary>
internal interface IFileLoggerService
{
    /// <summary>
    /// Gets the logger that logs to a file.
    /// </summary>
    public ILogger Logger { get; }
}
