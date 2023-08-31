// <copyright file="IDisplays.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

using System;
using Hardware;

/// <summary>
/// Represents multiple monitors in a system.
/// </summary>
internal interface IDisplays : IDisposable
{
    /// <summary>
    /// Gets a list of all the monitors currently in the system.
    /// </summary>
    SystemDisplay[] SystemDisplays { get; }

    /// <summary>
    /// Refreshes the display information.
    /// </summary>
    void Refresh();
}
