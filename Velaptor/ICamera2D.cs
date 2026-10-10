// <copyright file="ICamera2D.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

/// <summary>
/// A simple 2-D camera that applies pan, zoom, and rotation transforms on the CPU for
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
/// screen center so the center of the window stays fixed while zooming.
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
    /// Gets or sets the camera rotation in degrees. Positive values rotate the view
    /// clockwise. Default is <c>0</c> (no rotation). Rotation is applied around the
    /// screen center after zoom.
    /// </summary>
    float Rotation { get; set; }

    /// <summary>
    /// Gets the current window size in pixels. Used to locate the screen center for
    /// zoom calculations. Updated automatically on window resize.
    /// </summary>
    SizeU WindowSize { get; }

    /// <summary>
    /// Gets the axis-aligned bounds of the visible world area in world pixel coordinates.
    /// When the camera is rotated, this is the bounding box of the rotated view frustum.
    /// </summary>
    RectangleF WorldBounds { get; }

    /// <summary>
    /// Determines whether a world-space point is visible on the screen.
    /// </summary>
    /// <param name="worldPos">The position in world pixel coordinates.</param>
    /// <returns><c>true</c> if the point is within the visible screen area; otherwise, <c>false</c>.</returns>
    bool IsVisible(Vector2 worldPos);

    /// <summary>
    /// Determines whether a world-space rectangle intersects the visible screen area.
    /// Use this for culling objects with size (sprites, tiles, etc.).
    /// </summary>
    /// <param name="worldPos">The top-left position of the object in world pixel coordinates.</param>
    /// <param name="worldSize">The size of the object in world pixels.</param>
    /// <returns><c>true</c> if any part of the rectangle is visible; otherwise, <c>false</c>.</returns>
    bool IsVisible(Vector2 worldPos, Vector2 worldSize);

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

    /// <summary>
    /// Transforms a world-space rotation angle (in degrees) into a screen-space
    /// rotation angle, accounting for camera rotation. Rotation is additive:
    /// positive camera rotation (clockwise) adds to the object's rotation.
    /// </summary>
    /// <param name="worldRotation">The rotation angle in world space, in degrees.</param>
    /// <returns>The rotation angle in screen space, in degrees.</returns>
    float TransformRotation(float worldRotation);
}
