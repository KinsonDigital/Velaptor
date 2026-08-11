// <copyright file="FrameMetricsTracker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;

/// <inheritdoc/>
internal class FrameMetricsTracker : IFrameMetricsTracker
{
    private readonly double[] frameBuffer;
    private readonly double[] sortScratchBuffer; // Sorted so the original buffer is not disturbed
    private int headIndex;
    private int totalRecordedFrames;
    private double accumulatedDisplayTimeSeconds;
    private int framesSinceLastDisplayUpdate;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameMetricsTracker"/> class.
    /// </summary>
    /// <param name="historyCapacity">
    /// Ring buffer capacity. At 60 FPS, 1000 frames store ~16.7 seconds of history.
    /// This ensures the 1% low metric covers ~10 frames and the 0.1% low covers at least 1 frame.
    /// </param>
    public FrameMetricsTracker(int historyCapacity = 1000)
    {
        if (historyCapacity < 10)
        {
            throw new ArgumentOutOfRangeException(nameof(historyCapacity), "Capacity must be at least 10 frames.");
        }

        this.frameBuffer = new double[historyCapacity];
        this.sortScratchBuffer = new double[historyCapacity];
    }

    /// <inheritdoc/>
    public double DisplayUpdateIntervalSeconds { get; set; } = 0.5;

    /// <inheritdoc/>
    public FrameMetrics CurrentMetrics { get; private set; }

    /// <inheritdoc/>
    public void RecordFrame(double deltaTimeSeconds)
    {
        // Guard against non-positive delta time
        if (deltaTimeSeconds <= 0.000001)
        {
            deltaTimeSeconds = 0.000001;
        }

        var frameTimeMs = deltaTimeSeconds * 1000.0;

        // Write to circular ring buffer
        this.frameBuffer[this.headIndex] = frameTimeMs;
        this.headIndex = (this.headIndex + 1) % this.frameBuffer.Length;

        if (this.totalRecordedFrames < this.frameBuffer.Length)
        {
            this.totalRecordedFrames++;
        }

        this.framesSinceLastDisplayUpdate++;
        this.accumulatedDisplayTimeSeconds += deltaTimeSeconds;

        // Recalculate metrics on the display interval boundary
        if (this.accumulatedDisplayTimeSeconds >= DisplayUpdateIntervalSeconds)
        {
            CalculateMetrics();
            this.accumulatedDisplayTimeSeconds = 0.0;
            this.framesSinceLastDisplayUpdate = 0;
        }
    }

    /// <summary>
    /// Calculates the metrics for the current frame.
    /// </summary>
    private void CalculateMetrics()
    {
        var sampleCount = this.totalRecordedFrames;

        if (sampleCount == 0)
        {
            return;
        }

        // Copy active window slice into scratch buffer to avoid mutating ring buffer order
        var samples = this.sortScratchBuffer.AsSpan(0, sampleCount);

        this.frameBuffer.AsSpan(0, sampleCount).CopyTo(samples);

        // Sort frame times in ascending order (fastest frames first, slowest frames last)
        samples.Sort();

        // 1. Average throughput over the display window
        var avgFrameTimeMs = this.accumulatedDisplayTimeSeconds * 1000.0 / this.framesSinceLastDisplayUpdate;
        var avgFps = 1000.0 / avgFrameTimeMs;

        // 2. Extreme
        var minFrameTimeMs = samples[0];
        var maxFrameTimeMs = samples[sampleCount - 1];

        // 3. 1% Lows (Average of the worst 1% frame times)
        var onePercentCount = Math.Max(1, (int)Math.Ceiling(sampleCount * 0.01));
        var onePercentWorstMsSum = 0.0;

        for (var i = sampleCount - onePercentCount; i < sampleCount; i++)
        {
            onePercentWorstMsSum += samples[i];
        }

        var avgOnePercentWorstMs = onePercentWorstMsSum / onePercentCount;
        var onePercentLowFps = 1000.0 / avgOnePercentWorstMs;

        // 4. 0.1% Lows (Average of the worst 0.1% frame times, or at least the single worst frame)
        var pointOnePercentCount = Math.Max(1, (int)Math.Ceiling(sampleCount * 0.001));
        var pointOnePercentWorstMsSum = 0.0;
        for (var i = sampleCount - pointOnePercentCount; i < sampleCount; i++)
        {
            pointOnePercentWorstMsSum += samples[i];
        }

        var avgPointOnePercentWorstMs = pointOnePercentWorstMsSum / pointOnePercentCount;
        var zeroPointOnePercentLowFps = 1000.0 / avgPointOnePercentWorstMs;

        CurrentMetrics = new FrameMetrics
        {
            AverageFps = avgFps,
            AverageFrameTimeMs = avgFrameTimeMs,
            OnePercentLowFps = onePercentLowFps,
            ZeroPointOnePercentLowFps = zeroPointOnePercentLowFps,
            MaxFrameTimeMs = maxFrameTimeMs,
            MinFrameTimeMs = minFrameTimeMs,
        };
    }
}
