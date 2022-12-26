// <copyright file="MouseStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Input;

/// <summary>
/// Holds the state of the mouse.
/// </summary>
internal sealed record MouseStateData
{
    /// <summary>
    /// Gets or sets the X position of the mouse.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y position of the mouse.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Gets or sets the mouse button.
    /// </summary>
    public MouseButton Button { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the mouse button is in the down position.
    /// </summary>
    public bool ButtonIsDown { get; set; }

    /// <summary>
    /// Gets or sets the scroll wheel direction.
    /// </summary>
    public MouseScrollDirection ScrollDirection { get; set; }

    /// <summary>
    /// Gets or sets the scroll wheel value.
    /// </summary>
    public int ScrollWheelValue { get; set; }
}
