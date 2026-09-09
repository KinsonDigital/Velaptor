// <copyright file="IBatcher.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Batching;

using System.Drawing;

/// <summary>
/// Provides the ability to start and end the batch rendering process.
/// </summary>
public interface IBatcher
{
    /// <summary>
    /// Gets or sets the color of the back buffer when cleared.
    /// </summary>
    Color ClearColor { get; set; }

    /// <summary>
    /// Gets a value indicating whether the batch process has begun.
    /// </summary>
    bool HasBegun { get; }

    /// <summary>
    /// Starts the batch rendering process.  Must be called before invoking any render methods.
    /// </summary>
    void Begin();

    /// <summary>
    /// Ends the batch process.  Calling this will perform the actual GPU render process.
    /// </summary>
    void End();
}
