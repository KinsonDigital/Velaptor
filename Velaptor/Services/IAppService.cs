// <copyright file="IAppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Provides application wide services.
/// </summary>
internal interface IAppService
{
    /// <summary>
    /// Gets the directory of the application.
    /// </summary>
    string AppDirectory { get; }

    /// <summary>
    /// Gets a value indicating whether the application is a debug build.
    /// </summary>
    bool IsDebug { get; }

    /// <summary>
    /// Gets a value indicating whether telemetry is enabled.
    /// </summary>
    bool TelemetryEnabled { get; }

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Starts the application initialization process.
    /// </summary>
    void Init();
}
