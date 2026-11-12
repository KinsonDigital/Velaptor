// <copyright file="GameCtrlThumbstickState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

/// <summary>
/// Holds data about a game controllers thumbstick.
/// </summary>
internal readonly record struct GameCtrlThumbstickState
{
    /// <summary>
    /// Gets the current direction of the stick, from 0 to 360.
    /// </summary>
    public float Direction { get; init; }

    /// <summary>
    /// Gets the current position of the stick from 0.0 to 1.0.
    /// </summary>
    /// <remarks>
    /// This represents this distance of the stick from center position to the maximum
    /// outside position.
    /// </remarks>
    public float Position { get; init; }

    /// <summary>
    /// Gets the x-axis of the stick from -1.0 to 1.0.
    /// </summary>
    public float X { get; init; }

    /// <summary>
    /// Gets the y-axis of the stick from -1.0 to 1.0.
    /// </summary>
    public float Y { get; init; }

    /// <summary>
    /// Gets a value indicating whether if the thumbstick is the left stick.
    /// </summary>
    public bool IsLeftStick { get; init; }

    /// <summary>
    /// Gets a value indicating whether if the thumbstick is the right stick.
    /// </summary>
    public bool IsRightStick { get; init; }
}
