// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.UI;

namespace Velaptor;

/// <summary>
/// The different kinds of borders that a <see cref="IWindow"/> can have.
/// </summary>
public enum WindowBorder
{
    /// <summary>
    /// The window has a resizable border. A window with a resizable border can be resized
    /// by the user or programmatically.
    /// </summary>
    Resizable = 0,

    /// <summary>
    /// The window has a fixed border. A window with a fixed border can only be resized
    /// programmatically.
    /// </summary>
    Fixed = 1,

    /// <summary>
    ///  The window does not have a border. A window with a hidden border can only be
    ///  resized programmatically.
    /// </summary>
    Hidden = 2,
}

/// <summary>
/// The different states that a <see cref="Window"/> can be in.
/// </summary>
public enum StateOfWindow
{
    /// <summary>
    /// The window is in the normal state.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The window is minimized to the taskbar.
    /// </summary>
    /// <remarks>This is also known as .iconified'.</remarks>
    Minimized = 1,

    /// <summary>
    /// The window covers the whole working area, which includes the desktop but not the taskbar and/or panels.
    /// </summary>
    Maximized = 2,

    /// <summary>
    /// The window covers the whole screen, including all task bars and/or panels.
    /// </summary>
    FullScreen = 3,
}
