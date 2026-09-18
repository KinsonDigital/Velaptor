// <copyright file="ICamera2D.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

/// <summary>
/// A simple 2-D camera that applies pan and zoom transforms on the CPU for
/// world-to-screen coordinate conversion.
/// </summary>
/// <remarks>
/// <para>
/// Works in screen pixel coordinates: top-left origin, Y increases downward.
/// <see cref="Position"/> is a pan offset — positive X shifts the view right, which
/// makes scene content appear to move left (standard game-camera convention).
/// </para>
/// <para>
/// Unlike GPU-uniform cameras, transforms are applied entirely on the CPU
/// before vertex data is submitted. Call <see cref="TransformPosition"/> and
/// <see cref="TransformSize"/> when building render data each frame.
/// </para>
/// <para>
/// <b>Zoom:</b> 1.0 = one world pixel maps to exactly one screen pixel. Values above
/// 1.0 magnify the scene; values below 1.0 shrink it. Zoom is applied relative to the
/// screen centre so the centre of the window stays fixed while zooming.
/// </para>
/// </remarks>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
public interface ICamera2D
{
    /// <summary>
    /// Gets or sets the camera pan offset in screen pixel units.
    /// Moving the camera right (positive X) shifts all scene content leftward.
    /// Default is <see cref="Vector2.Zero"/> (no pan).
    /// </summary>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the zoom factor. <c>1.0</c> = native pixel size.
    /// Values above <c>1.0</c> magnify; values below shrink. Default is <c>1.0</c>.
    /// </summary>
    float Zoom { get; set; }

    /// <summary>
    /// Gets or sets the minimum zoom level. Default is <c>0.1</c>.
    /// </summary>
    float ZoomMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum zoom level. Default is <c>4.0</c>.
    /// </summary>
    float ZoomMax { get; set; }

    /// <summary>
    /// Gets the current window size in pixels. Used to locate the screen centre for
    /// zoom calculations. Updated automatically on window resize.
    /// </summary>
    SizeU WindowSize { get; }

    /// <summary>
    /// Transforms a world-space pixel position into a screen render position,
    /// accounting for camera pan and zoom.
    /// </summary>
    /// <param name="worldPos">The position in world pixel coordinates.</param>
    /// <returns>The position in screen pixel coordinates.</returns>
    Vector2 TransformPosition(Vector2 worldPos);

    /// <summary>
    /// Scales a size value by the current zoom level.
    /// Use this for width, height, radius, and other size-related values.
    /// </summary>
    /// <param name="size">The size in world pixels.</param>
    /// <returns>The scaled size in screen pixels.</returns>
    float TransformSize(float size);
}
