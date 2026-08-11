// <copyright file="IFrameMetricsTracker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Tracks frame performance metrics.
/// </summary>
internal interface IFrameMetricsTracker
{
    /// <summary>
    /// Gets or sets how often the UI readout recalculates metrics.
    /// </summary>
    double DisplayUpdateIntervalSeconds { get; set; }

    /// <summary>
    /// Gets current calculated metrics ready for display.
    /// </summary>
    FrameMetrics CurrentMetrics { get; }

    /// <summary>
    /// Records a frame and calculates the metrics for the current frame.
    /// </summary>
    /// <param name="deltaTimeSeconds">Elapsed frame time from timing API (e.g., Stopwatch).</param>
    /// <remarks>Call this once at the end of every frame pass.</remarks>
    void RecordFrame(double deltaTimeSeconds);
}
