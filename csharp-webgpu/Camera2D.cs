// <copyright file="Camera2D.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Numerics;

/// <summary>
/// A simple 2-D camera that applies pan and zoom to rendered objects on the CPU.
/// </summary>
/// <remarks>
/// <para>
/// Works in screen pixel coordinates: top-left origin, Y increases downward.
/// <see cref="Position"/> is a pan offset — positive X shifts the view right, which
/// makes scene content appear to move left (standard game-camera convention).
/// </para>
/// <para>
/// Unlike the previous GPU-uniform camera, transforms are applied entirely on the CPU
/// before vertex data is uploaded. Call <see cref="TransformPosition"/> and
/// <see cref="TransformSize"/> when building <see cref="TextureQuad"/> or
/// <see cref="RectShape"/> data each frame.
/// </para>
/// <para>
/// <b>Zoom:</b> 1.0 = one world pixel maps to exactly one screen pixel. Values above
/// 1.0 magnify the scene; values below 1.0 shrink it. Zoom is applied relative to the
/// screen centre so the centre of the window stays fixed while zooming.
/// </para>
/// </remarks>
public sealed class Camera2D
{
    /// <summary>
    /// Gets or sets the camera pan offset in screen pixel units.
    /// Moving the camera right (positive X) shifts all scene content leftward.
    /// Default is <see cref="Vector2.Zero"/> (no pan).
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the zoom factor. <c>1.0</c> = native pixel size.
    /// Values above <c>1.0</c> magnify; values below shrink. Default is <c>1.0</c>.
    /// </summary>
    public float Zoom { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the window size in pixels. Used to locate the screen centre for
    /// zoom calculations. Update this on every window resize.
    /// </summary>
    public Vector2 WindowSize { get; set; }

    /// <summary>
    /// Transforms a world-space pixel position into a screen render position,
    /// accounting for camera pan and zoom. Pass the result as
    /// <see cref="TextureQuad.Position"/> or <see cref="RectShape.Position"/>.
    /// </summary>
    /// <param name="worldPos">The position in world pixel coordinates.</param>
    /// <returns>The position in screen pixel coordinates to pass to the render buffer.</returns>
    public Vector2 TransformPosition(Vector2 worldPos)
    {
        var center = WindowSize / 2f;
        return center + ((worldPos - center - Position) * Zoom);
    }

    /// <summary>
    /// Scales a size value by the current zoom level.
    /// Use this for <see cref="TextureQuad.Size"/>, and for rect width/height/radii.
    /// </summary>
    /// <param name="size">The size in world pixels.</param>
    /// <returns>The scaled size in screen pixels.</returns>
    public float TransformSize(float size) => size * Zoom;

    /// <summary>
    /// No-op kept for API compatibility with call sites that previously triggered
    /// a GPU uniform buffer upload. The CPU-based camera needs no explicit flush.
    /// </summary>
    public void Update() { }
}
