// <copyright file="IControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Drawing;
using Content;
using Input;

/// <summary>
/// A user interface object that can be updated and rendered to the screen.
/// </summary>
[Obsolete("This interface is deprecated and will be removed in a future release.")]
public interface IControl : IUpdatable, IDrawable, IContentLoadable, ISizable
{
    /// <summary>
    /// Occurs when the button has been clicked.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<EventArgs>? Click;

    /// <summary>
    /// Occurs when the left mouse button is in the down position over the button.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<EventArgs>? MouseDown;

    /// <summary>
    /// Occurs when the left mouse button is in the up position over the button
    /// after the mouse has been in the down position.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<EventArgs>? MouseUp;

    /// <summary>
    /// Occurs when the mouse moves over the button.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<MouseMoveEventArgs>? MouseMove;

    /// <summary>
    /// Occurs when a keyboard key is pressed into the down position.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<KeyEventArgs>? KeyDown;

    /// <summary>
    /// Occurs when a keyboard key is released into the up position.
    /// </summary>
    [Obsolete("This event is deprecated and will be removed in a future release.")]
    event EventHandler<KeyEventArgs>? KeyUp;

    // ReSharper disable UnusedMemberInSuper.Global

    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the position of the <see cref="IControl"/> on the screen.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    Point Position { get; set; }

    /// <summary>
    /// Gets or sets the position of the left side of the control.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    int Left { get; set; }

    /// <summary>
    /// Gets or sets the position of the right side of the control.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    int Right { get; set; }

    /// <summary>
    /// Gets or sets the position of the top of the control.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    int Top { get; set; }

    /// <summary>
    /// Gets or sets the position of the bottom of the control.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    int Bottom { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is visible.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    bool Enabled { get; set; }

    /// <summary>
    /// Gets a value indicating whether the mouse is hovering over the button.
    /// </summary>
    [Obsolete("This property is deprecated and will be removed in a future release.")]
    bool IsMouseOver { get; }

    // ReSharper restore UnusedMemberInSuper.Global
}
