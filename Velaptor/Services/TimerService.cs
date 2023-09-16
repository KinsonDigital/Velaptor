// <copyright file="TimerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <inheritdoc/>
internal sealed class TimerService : ITimerService
{
    private readonly IStopWatchWrapper timer;
    private readonly double[] timeSamples = new double[1000];
    private int index;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerService"/> class.
    /// </summary>
    /// <param name="stopWatch">Tracks and measures time passed.</param>
    public TimerService(IStopWatchWrapper stopWatch) => this.timer = stopWatch;

    /// <inheritdoc/>
    /// <remarks>This is averaged over 1000 samples.</remarks>
    public float MillisecondsPassed { get; private set; }

    /// <inheritdoc/>
    public void Start() => this.timer.Start();

    /// <inheritdoc/>
    public void Stop()
    {
        if (!this.timer.IsRunning)
        {
            return;
        }

        this.timer.Stop();

        AddSample(this.timer.Elapsed.TotalMilliseconds);

        // Get the average of the samples and save it
        MillisecondsPassed = Average();
    }

    /// <inheritdoc/>
    public void Reset() => this.timer.Reset();

    /// <summary>
    /// Adds the given <paramref name="sample"/> to the list of samples.
    /// </summary>
    /// <param name="sample">The sample to add.</param>
    private void AddSample(double sample)
    {
        this.timeSamples[this.index] = sample;

        if (this.index >= this.timeSamples.Length - 1)
        {
            this.index = 0;
        }
        else
        {
            this.index += 1;
        }
    }

    /// <summary>
    /// Calculates the average of all the samples.
    /// </summary>
    /// <returns>The sample average.</returns>
    /// <remarks>If any of the samples are 0, it is not counted towards the average.</remarks>
    private float Average()
    {
        var sum = 0.0;
        var count = 0;

        for (var i = 0; i < this.timeSamples.Length; i++)
        {
            if (this.timeSamples[i] != 0)
            {
                sum += this.timeSamples[i];
                count += 1;
            }
        }

        return count == 0 ? 0 : (float)sum / count;
    }
}
