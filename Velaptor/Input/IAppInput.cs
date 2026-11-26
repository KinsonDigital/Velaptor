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
    IReadOnlyCollection<string> ConnectedDeviceNames { get; }

    /// <summary>
    /// Gets the list of connected device IDs.
    /// </summary>
    IReadOnlyCollection<int> ConnectedDeviceIds { get; }

    /// <summary>
    /// Returns the current state of the first connected input.
    /// </summary>
    /// <returns>The state of the input.</returns>
    TState GetState();

    /// <summary>
    /// Returns the current state of the input that matches the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The unique device ID of the input.</param>
    /// <returns>The state of the input.</returns>
    TState GetState(int id);
}
