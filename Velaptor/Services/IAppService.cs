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
    /// Starts the application initialization process.
    /// </summary>
    void Init();
}
