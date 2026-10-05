// <copyright file="FrameMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Holds performance metrics for a frame.
/// </summary>
internal readonly struct FrameMetrics
{
    /// <summary>
    /// Gets the average frames per second over the recent display window.
    /// </summary>
    /// <remarks>
    /// This is calculated from the average frame time over the display interval (typically 0.5 seconds),
    /// providing a smooth, responsive FPS value that updates frequently.
    /// </remarks>
    public double AverageFps { get; init; }

    /// <summary>
    /// Gets the average frame time in milliseconds over the recent display window.
    /// </summary>
    /// <remarks>
    /// This represents the typical time taken to render a frame over the display interval.
    /// Lower values indicate better performance.
    /// </remarks>
    public double AverageFrameTimeMs { get; init; }

    /// <summary>
    /// Gets the FPS value representing the worst 1% of frames.
    /// </summary>
    /// <remarks>
    /// This is calculated by averaging the frame times of the slowest 1% of frames in the history buffer,
    /// then converting to FPS. It indicates the typical performance during stuttery moments.
    /// With a 1000-frame buffer, this averages the 10 worst frames.
    /// </remarks>
    public double OnePercentLowFps { get; init; }

    /// <summary>
    /// Gets the FPS value representing the worst 0.1% of frames.
    /// </summary>
    /// <remarks>
    /// This is calculated by averaging the frame times of the slowest 0.1% of frames in the history buffer,
    /// then converting to FPS. It highlights extreme outliers in performance.
    /// With a 1000-frame buffer, this effectively represents the single worst frame.
    /// </remarks>
    public double ZeroPointOnePercentLowFps { get; init; }

    /// <summary>
    /// Gets the maximum frame time in milliseconds recorded in the history buffer.
    /// </summary>
    /// <remarks>
    /// This represents the slowest frame over the entire history window (typically ~16.7 seconds).
    /// It shows the worst-case rendering time.
    /// </remarks>
    public double MaxFrameTimeMs { get; init; }

    /// <summary>
    /// Gets the minimum frame time in milliseconds recorded in the history buffer.
    /// </summary>
    /// <remarks>
    /// This represents the fastest frame over the entire history window (typically ~16.7 seconds).
    /// It shows the best-case rendering time.
    /// </remarks>
    public double MinFrameTimeMs { get; init; }
}
