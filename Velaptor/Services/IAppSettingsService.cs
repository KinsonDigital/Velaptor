// <copyright file="IAppSettingsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Provides application settings management.
/// </summary>
internal interface IAppSettingsService
{
    /// <summary>
    /// Gets the application settings.
    /// </summary>
    AppSettings Settings { get; }
}
