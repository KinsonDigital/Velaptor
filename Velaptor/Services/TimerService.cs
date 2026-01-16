// <copyright file="TimerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Runtime.CompilerServices;

/// <inheritdoc/>
internal sealed class TimerService : ITimerService
{
    private const int SampleSize = 1000;
    private readonly IStopWatchWrapper stopWatch;
    private readonly double[] timeSamples = new double[SampleSize];
    private readonly double tickFreqMs;
    private double runningSum;
    private long startTicks;
    private long stopTicks;
    private int index;
    private bool isArrayFull;
    private int divisor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerService"/> class.
    /// </summary>
    /// <param name="stopWatch">Tracks and measures time passed.</param>
    public TimerService(IStopWatchWrapper stopWatch)
    {
        this.stopWatch = stopWatch;
        this.tickFreqMs = 1000.0 / this.stopWatch.Frequency;
    }

    /// <inheritdoc/>
    /// <remarks>This is averaged over 1000 samples.</remarks>
    public float MillisecondsPassed { get; private set; }

    /// <inheritdoc/>
    public void Start() => this.startTicks = this.stopWatch.GetTimestamp();

    /// <inheritdoc/>
    public void Stop()
    {
        this.stopTicks = this.stopWatch.GetTimestamp();
        var sample = (this.stopTicks - this.startTicks) * this.tickFreqMs;
        AddSample(sample);

        // Set the divisor to avoid division by zero and to divide by the correct number of samples,
        // which is important for calculating the average
        this.divisor = this.index == 1 ? 1 : this.index;

        // Calculate average using a running sum
        MillisecondsPassed = (float)(this.runningSum / (this.isArrayFull ? SampleSize : this.divisor));
    }

    /// <inheritdoc/>
    public void Reset()
    {
        this.index = 0;
        this.runningSum = 0;
        this.isArrayFull = false;
        Array.Clear(this.timeSamples, 0, SampleSize);
        MillisecondsPassed = 0;
    }

    /// <summary>
    /// Adds the given <paramref name="sample"/> to the list of samples.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddSample(double sample)
    {
        // Subtract the old value from a running sum before replacing it
        this.runningSum -= this.timeSamples[this.index];
        this.runningSum += sample;
        this.timeSamples[this.index] = sample;

        if (this.index == SampleSize - 1)
        {
            this.isArrayFull = true;
        }

        this.index = this.index >= this.timeSamples.Length - 1
            ? 0
            : this.index + 1;
    }
}
