// <copyright file="StopWatchWrapper.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

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
    /// <inheritdoc />
    public long Frequency => Stopwatch.Frequency;

    /// <inheritdoc/>
    public long GetTimestamp() => Stopwatch.GetTimestamp();
}
