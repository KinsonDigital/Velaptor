// <copyright file="FrameTime.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds timing information for a loop iteration.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Not worth covering.")]
public readonly record struct FrameTime
{
    /// <summary>
    /// Gets the total time that the entire application has been running.
    /// </summary>
    public TimeSpan TotalTime { get; init; }

    /// <summary>
    /// Gets the total time that has passed for the current frame.
    /// </summary>
    public TimeSpan ElapsedTime { get; init; }
}
