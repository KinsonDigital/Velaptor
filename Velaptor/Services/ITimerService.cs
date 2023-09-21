// <copyright file="ITimerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Measures the time it takes to perform an operation.
/// </summary>
internal interface ITimerService
{
    /// <summary>
    /// Gets the time in milliseconds that was measured.
    /// </summary>
    float MillisecondsPassed { get; }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the timer.
    /// </summary>
    void Stop();

    /// <summary>
    /// Resets the timer.
    /// </summary>
    void Reset();
}
