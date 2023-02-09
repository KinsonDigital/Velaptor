// <copyright file="KeyboardKeyStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Input;

/// <summary>
/// Holds the state of a keyboard key.
/// </summary>
internal readonly record struct KeyboardKeyStateData
{
    /// <summary>
    /// Gets the key code.
    /// </summary>
    public KeyCode Key { get; init; }

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="Key"/> is in the down position.
    /// </summary>
    public bool IsDown { get; init; }
}
