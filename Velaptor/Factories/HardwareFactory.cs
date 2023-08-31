﻿// <copyright file="HardwareFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Hardware;
using Input;
using Services;

/// <summary>
/// Generates input type objects for processing input such as the keyboard and mouse.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
public static class HardwareFactory
{
    /// <summary>
    /// Gets a keyboard object.
    /// </summary>
    /// <returns>The keyboard singleton object.</returns>
    public static IAppInput<KeyboardState> GetKeyboard() => IoC.Container.GetInstance<IAppInput<KeyboardState>>();

    /// <summary>
    /// Gets a mouse object.
    /// </summary>
    /// <returns>The keyboard singleton object.</returns>
    public static IAppInput<MouseState> GetMouse() => IoC.Container.GetInstance<IAppInput<MouseState>>();

    /// <summary>
    /// Gets all of the monitors in the system.
    /// </summary>
    /// <returns>The list of monitors.</returns>
    public static ImmutableArray<SystemMonitor> GetMonitors()
    {
        var monitorService = IoC.Container.GetInstance<ISystemMonitorService>();

        return monitorService.Monitors.ToImmutableArray();
    }

    /// <summary>
    /// Gets the primary monitor in the system.
    /// </summary>
    /// <returns>The system's primary monitor.</returns>
    public static SystemMonitor GetMainMonitor()
    {
        var monitorService = IoC.Container.GetInstance<ISystemMonitorService>();

        return monitorService.MainMonitor;
    }
}
