// <copyright file="IAppInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System.Collections.Generic;

/// <summary>
/// Gets the state of game-specific input such as a mouse or keyboard.
/// </summary>
/// <typeparam name="TState">The state of the input.</typeparam>
public interface IAppInput<out TState>
    where TState : struct
{
    /// <summary>
    /// Gets a value indicating the total number of devices connected.
    /// </summary>
    int TotalDevices { get; }

    /// <summary>
    /// Gets the list of connected devices.
    /// </summary>
    IReadOnlyCollection<string> ConnectedDevices { get; }

    /// <summary>
    /// Returns the current state of the input.
    /// </summary>
    /// <returns>The state of the input.</returns>
    TState GetState();
}
