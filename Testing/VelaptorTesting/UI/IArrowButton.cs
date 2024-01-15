// <copyright file="IArrowButton.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// An arrow button control.
/// </summary>
public interface IArrowButton : IControl
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
}
