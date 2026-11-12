// <copyright file="GameCtrlButtonStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Input;

/// <summary>
/// Holds data about the state of a game controller button.
/// </summary>
internal readonly record struct GameCtrlButtonStateData
{
    /// <summary>
    /// Gets the button.
    /// </summary>
    public ControllerButton Button { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="ControllerButton"/> is in the down position.
    /// </summary>
    public bool IsDown { get; init; }
}
