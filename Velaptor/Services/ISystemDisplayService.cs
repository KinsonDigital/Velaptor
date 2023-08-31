// <copyright file="ISystemDisplayService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Collections.Generic;
using Hardware;

/// <summary>
/// Gets information about all of the monitors in the system.
/// </summary>
internal interface ISystemDisplayService
{
    /// <summary>
    /// Gets a list of all of the monitors in the system.
    /// </summary>
    IReadOnlyCollection<SystemDisplay> Displays { get; }

    /// <summary>
    /// Gets the primary display in the system.
    /// </summary>
    /// <remarks>Will return null if no monitors are hooked up to the system.</remarks>
    public SystemDisplay MainDisplay { get; }

    /// <summary>
    /// Refreshes the display information.
    /// </summary>
    void Refresh();
}
