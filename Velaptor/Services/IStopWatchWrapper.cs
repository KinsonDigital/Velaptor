// <copyright file="IStopWatchWrapper.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics;

/// <inheritdoc cref="Stopwatch"/>
internal interface IStopWatchWrapper
{
    /// <inheritdoc cref="Stopwatch.Elapsed"/>
    TimeSpan Elapsed { get; }

    /// <inheritdoc cref="Stopwatch.IsRunning"/>
    bool IsRunning { get; }

    /// <inheritdoc cref="Stopwatch.Start"/>
    void Start();

    /// <inheritdoc cref="Stopwatch.Stop"/>
    void Stop();

    /// <inheritdoc cref="Stopwatch.Reset"/>
    void Reset();
}
