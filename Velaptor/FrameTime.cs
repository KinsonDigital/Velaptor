// <copyright file="FrameTime.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds timing information for a loop iteration.
/// </summary>
public struct FrameTime : IEquatable<FrameTime>
{
    /// <summary>
    /// Gets or sets the total time that the entire application has been running.
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// Gets or sets the total time that has passed for the current frame.
    /// </summary>
    public TimeSpan ElapsedTime { get; set; }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns>True if both operators are equal.</returns>
    public static bool operator ==(FrameTime left, FrameTime right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operator.</param>
    /// <param name="right">The right operator.</param>
    /// <returns>True if both operators are not equal.</returns>
    public static bool operator !=(FrameTime left, FrameTime right) => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FrameTime frameTime && Equals(frameTime);

    /// <inheritdoc/>
    public bool Equals(FrameTime other)
        => other.TotalTime == TotalTime && other.ElapsedTime == ElapsedTime;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => ElapsedTime.GetHashCode();
}
