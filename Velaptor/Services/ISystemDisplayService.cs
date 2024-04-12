// <copyright file="ISystemDisplayService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Collections.Generic;
using Hardware;

/// <summary>
/// Gets information about all the displays in the system.
/// </summary>
internal interface ISystemDisplayService
{
    /// <summary>
    /// Gets a list of all the displays in the system.
    /// </summary>
    IReadOnlyCollection<SystemDisplay> Displays { get; }

    /// <summary>
    /// Gets the primary display in the system.
    /// </summary>
    /// <remarks>Will return null if no displays are hooked up to the system.</remarks>
    public SystemDisplay MainDisplay { get; }

    /// <summary>
    /// Refreshes the display information.
    /// </summary>
    void Refresh();
}
