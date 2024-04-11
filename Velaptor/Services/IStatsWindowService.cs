// <copyright file="IStatsWindowService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Drawing;
using ImGuiNET;

/// <summary>
/// Various run time stats that can be displayed on the screen using <see cref="ImGui"/>.
/// </summary>
internal interface IStatsWindowService : IDisposable, IUpdatable, IDrawable
{
    /// <summary>
    /// Invoked when the stats window has been initialized.
    /// </summary>
    event EventHandler Initialized;

    /// <summary>
    /// Gets or sets the position of the stats window.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets the size of the window.
    /// </summary>
    Size Size { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the window is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Updates the frames per second stat.
    /// </summary>
    /// <param name="fpsStat">The frames per second.</param>
    void UpdateFpsStat(float fpsStat);
}
