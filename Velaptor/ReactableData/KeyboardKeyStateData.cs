// <copyright file="KeyboardKeyStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Input;

/// <summary>
/// Holds the state of a keyboard key.
/// </summary>
public sealed record KeyboardKeyStateData
{
    /// <summary>
    /// Gets or sets the key code.
    /// </summary>
    public KeyCode Key { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current <see cref="Key"/> is in the down position.
    /// </summary>
    public bool IsDown { get; set; }
}
