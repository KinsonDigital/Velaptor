// <copyright file="MouseStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Input;

/// <summary>
/// Holds the state of the mouse.
/// </summary>
internal readonly record struct MouseStateData
{
    /// <summary>
    /// Gets the X position of the mouse.
    /// </summary>
    public int X { get; init; }

    /// <summary>
    /// Gets the Y position of the mouse.
    /// </summary>
    public int Y { get; init; }

    /// <summary>
    /// Gets the mouse button.
    /// </summary>
    public MouseButton Button { get; init; }

    /// <summary>
    /// Gets a value indicating whether or not the mouse button is in the down position.
    /// </summary>
    public bool ButtonIsDown { get; init; }

    /// <summary>
    /// Gets the scroll wheel direction.
    /// </summary>
    public MouseScrollDirection ScrollDirection { get; init; }

    /// <summary>
    /// Gets the scroll wheel value.
    /// </summary>
    public int ScrollWheelValue { get; init; }
}
