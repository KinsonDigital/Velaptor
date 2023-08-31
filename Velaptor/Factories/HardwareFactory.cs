// <copyright file="HardwareFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Input;

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
}
