// <copyright file="HardwareFactory.cs" company="KinsonDigital">
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
    /// Gets all the displays in the system.
    /// </summary>
    /// <returns>The list of displays.</returns>
    public static ImmutableArray<SystemDisplay> GetDisplays()
    {
        var displayService = IoC.Container.GetInstance<ISystemDisplayService>();

        return displayService.Displays.ToImmutableArray();
    }

    /// <summary>
    /// Gets the primary display in the system.
    /// </summary>
    /// <returns>The system's primary display.</returns>
    public static SystemDisplay GetMainDisplay()
    {
        var displayService = IoC.Container.GetInstance<ISystemDisplayService>();

        return displayService.MainDisplay;
    }
}
