// <copyright file="SystemDisplay.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

/// <summary>
/// Holds information about a single display in the system.
/// </summary>
public readonly record struct SystemDisplay
{
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplay"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required for testing.")]
    public SystemDisplay() => this.platform = new Platform();

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplay"/> class.
    /// </summary>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <exception cref="ArgumentNullException">
    ///     Occurs if the <paramref name="platform"/> parameter is null.
    /// </exception>
    internal SystemDisplay(IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        this.platform = platform;
    }

    /// <summary>
    /// Gets a value indicating whether the display is the primary display in the system.
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
    /// Gets the scale of the display on the horizontal axis.
    /// </summary>
    /// <remarks>
    ///     If the display was set to a scale of 150%, this value of 1.5 should be used.
    /// </remarks>
    public float HorizontalScale { get; init; }

    /// <summary>
    /// Gets the scale of the display on the vertical axis.
    /// </summary>
    /// <remarks>
    ///     If the display was set to a scale of 150%, this value of 1.5 should be used.
    /// </remarks>
    public float VerticalScale { get; init; }

    /// <summary>
    /// Gets the center location of the display.
    /// </summary>
    public Vector2 Center => new (Width / 2f, Height / 2f);

    /// <summary>
    /// Gets the approximate dpi of the display on the horizontal axis.
    /// </summary>
    public float HorizontalDPI => GetPlatformDefaultDpi() * HorizontalScale;

    /// <summary>
    /// Gets the approximate dpi of the display on the vertical axis.
    /// </summary>
    public float VerticalDPI => GetPlatformDefaultDpi() * VerticalScale;

    /// <summary>
    /// Gets the default DPI value of the current platform.
    /// </summary>
    /// <returns>The current platform's DPI setting.</returns>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    private float GetPlatformDefaultDpi() => this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f;
}
