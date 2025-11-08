// <copyright file="IStopWatchWrapper.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc cref="Stopwatch"/>
internal interface IStopWatchWrapper
{
    /// <inheritdoc cref="Stopwatch.Frequency"/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test stopwatch property.")]
    long Frequency => Stopwatch.Frequency;

    /// <inheritdoc cref="Stopwatch.GetTimestamp"/>
    long GetTimestamp();
}
