// <copyright file="IWindowActions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;

/// <summary>
/// Provides the various actions of a window.
/// </summary>
public interface IWindowActions
{
    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate to be invoked one time to initialize the window.
    /// </summary>
    Action? Initialize { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for updating.
    /// </summary>
    Action<FrameTime>? Update { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for rendering.
    /// </summary>
    Action<FrameTime>? Draw { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate to be invoked one time to uninitialize the window.
    /// </summary>
    Action? Uninitialize { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked every time the window is resized.
    /// </summary>
    Action<SizeU>? WinResize { get; set; }
}
