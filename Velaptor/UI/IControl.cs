// <copyright file="IControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using Velaptor.Content;

namespace Velaptor.UI;

/// <summary>
/// A user interface object that can be updated and rendered to the screen.
/// </summary>
public interface IControl : IUpdatable, IDrawable, IContentLoadable, ISizable
{
    /// <summary>
    /// Occurs when the button has been clicked.
    /// </summary>
    event EventHandler<EventArgs>? Click;

    /// <summary>
    /// Occurs when the left mouse button is in the down position over the button.
    /// </summary>
    event EventHandler<EventArgs>? MouseDown;

    /// <summary>
    /// Occurs when the left mouse button is in the up position over the button
    /// after the mouse has been in the down position.
    /// </summary>
    event EventHandler<EventArgs>? MouseUp;

    /// <summary>
    /// Occurs when the mouse moves over the button.
    /// </summary>
    event EventHandler<MousePositionEventArgs>? MouseMove;

    // ReSharper disable UnusedMemberInSuper.Global

    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the position of the <see cref="IControl"/> on the screen.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets or sets the position of the left side of the control.
    /// </summary>
    int Left { get; set; }

    /// <summary>
    /// Gets or sets the position of the right side of the control.
    /// </summary>
    int Right { get; set; }

    /// <summary>
    /// Gets or sets the position of the top of the control.
    /// </summary>
    int Top { get; set; }

    /// <summary>
    /// Gets or sets the position of the bottom of the control.
    /// </summary>
    int Bottom { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the control is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the control is enabled.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the mouse is hovering over the button.
    /// </summary>
    bool IsMouseOver { get; }

    // ReSharper restore UnusedMemberInSuper.Global
}
