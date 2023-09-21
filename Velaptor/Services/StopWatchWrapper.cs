// <copyright file="StopWatchWrapper.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
///     This is just a thin wrapper around the dotnet <see cref="Stopwatch"/> type.
///     <br/>
///     This is primarily used for unit testing purposes.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around the Stopwatch class.")]
internal class StopWatchWrapper : IStopWatchWrapper
{
    private readonly Stopwatch stopWatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="StopWatchWrapper"/> class.
    /// </summary>
    public StopWatchWrapper() => this.stopWatch = new Stopwatch();

    /// <inheritdoc/>
    public TimeSpan Elapsed => this.stopWatch.Elapsed;

    /// <inheritdoc/>
    public bool IsRunning => this.stopWatch.IsRunning;

    /// <inheritdoc/>
    public void Start() => this.stopWatch.Start();

    /// <inheritdoc/>
    public void Stop() => this.stopWatch.Stop();

    /// <inheritdoc/>
    public void Reset() => this.stopWatch.Reset();
}
