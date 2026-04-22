// <copyright file="IAppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Provides application-wide services.
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
    /// Gets a value indicating whether the consumer of the Velaptor library is a debug build.
    /// </summary>
    bool ConsumerIsDebug { get; }

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    string Version { get; }
}
