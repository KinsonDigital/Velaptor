// <copyright file="IStopWatchWrapper.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics;

/// <inheritdoc cref="Stopwatch"/>
internal interface IStopWatchWrapper
{
    /// <inheritdoc cref="Stopwatch.Frequency"/>
    long Frequency => Stopwatch.Frequency;

    /// <inheritdoc cref="Stopwatch.GetTimestamp"/>
    long GetTimestamp();
}
