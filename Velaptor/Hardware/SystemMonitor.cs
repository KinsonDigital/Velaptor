// <copyright file="SystemMonitor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Guards;

/// <summary>
/// Holds information about a single monitor in the system.
/// </summary>
public class SystemMonitor : IEquatable<SystemMonitor>
{
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitor"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public SystemMonitor() => this.platform = IoC.Container.GetInstance<IPlatform>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitor"/> class.
    /// </summary>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <exception cref="ArgumentNullException">
    ///     Occurs if the <paramref name="platform"/> parameter is null.
    /// </exception>
    internal SystemMonitor(IPlatform platform)
    {
        EnsureThat.ParamIsNotNull(platform);
        this.platform = platform;
    }

    /// <summary>
    /// Gets a value indicating whether or not the monitor is the primary monitor in the system.
    /// </summary>
    public bool IsMain { get; init; }

    /// <summary>
    /// Gets the width in screen coordinates in the current video mode.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// Gets the height in screen coordinates in the current video mode.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// Gets the bit depth of the red color channel in the current video mode.
    /// </summary>
    public int RedBitDepth { get; init; }

    /// <summary>
    /// Gets the bit depth of the green color channel in the current video mode.
    /// </summary>
    public int GreenBitDepth { get; init; }

    /// <summary>
    /// Gets the bit depth of the blue color channel in the current video mode.
    /// </summary>
    public int BlueBitDepth { get; init; }

    /// <summary>
    /// Gets the refresh rate in Hz in the current video mode.
    /// </summary>
    public int RefreshRate { get; init; }

    /// <summary>
    /// Gets the scale of the monitor on the horizontal axis.
    /// </summary>
    /// <remarks>
    ///     If the monitor was set to a scale of 150%, this value of 1.5 should be used.
    /// </remarks>
    public float HorizontalScale { get; init; }

    /// <summary>
    /// Gets the scale of the monitor on the vertical axis.
    /// </summary>
    /// <remarks>
    ///     If the monitor was set to a scale of 150%, this value of 1.5 should be used.
    /// </remarks>
    public float VerticalScale { get; init; }

    /// <summary>
    /// Gets the center location of the monitor.
    /// </summary>
    public Vector2 Center => new (Width / 2f, Height / 2f);

    /// <summary>
    /// Gets the approximate dpi of the monitor on the horizontal axis.
    /// </summary>
    public float HorizontalDPI => GetPlatformDefaultDpi() * HorizontalScale;

    /// <summary>
    /// Gets the approximate dpi of the monitor on the vertical axis.
    /// </summary>
    public float VerticalDPI => GetPlatformDefaultDpi() * VerticalScale;

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if both operands are equal.</returns>
    public static bool operator ==(SystemMonitor? left, SystemMonitor right) => left is not null && left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if both operands are not equal.</returns>
    public static bool operator !=(SystemMonitor left, SystemMonitor right) => !(left == right);

    /// <inheritdoc/>
    public bool Equals(SystemMonitor? other)
    {
        if (other is null)
        {
            return false;
        }

        return IsMain == other.IsMain &&
               RedBitDepth == other.RedBitDepth &&
               GreenBitDepth == other.GreenBitDepth &&
               BlueBitDepth == other.BlueBitDepth &&
               Width == other.Width &&
               Height == other.Height &&
               RefreshRate == other.RefreshRate &&
               Math.Abs(HorizontalScale - other.HorizontalScale) == 0f &&
               Math.Abs(VerticalScale - other.VerticalScale) == 0f;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not SystemMonitor monitor)
        {
            return false;
        }

        return Equals(monitor);
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
        => HashCode.Combine(IsMain, RedBitDepth, GreenBitDepth, BlueBitDepth, Width) +
           HashCode.Combine(Height, RefreshRate, HorizontalScale, VerticalScale);

    /// <summary>
    /// Gets the default DPI value of the current platform.
    /// </summary>
    /// <returns>The current platform's DPI setting.</returns>
    [ExcludeFromCodeCoverage]
    private float GetPlatformDefaultDpi() => this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f;
}
