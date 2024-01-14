// <copyright file="IButton.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// Represents a button control.
/// </summary>
public interface IButton : IControl
{
    /// <summary>
    /// Invoked when the button is clicked.
    /// </summary>
    event EventHandler<EventArgs>? Click;

    /// <summary>
    /// Invoked when the mouse is pressed down on the button.
    /// </summary>
    event EventHandler<EventArgs>? MousePressed;

    /// <summary>
    /// Invoked when the mouse is released from the button.
    /// </summary>
    event EventHandler<EventArgs>? MouseReleased;

    /// <summary>
    /// Gets or sets the text of the button.
    /// </summary>
    string Text { get; set; }
}
