// <copyright file="Camera2D.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Carbonate;
using Factories;

/// <summary>
/// A simple 2-D camera that applies pan, zoom, and rotation to rendered objects on the CPU.
/// </summary>
/// <inheritdoc cref="ICamera2D"/>
/// <remarks>
/// <para>
/// Works in screen pixel coordinates: top-left origin, Y increases downward.
/// <see cref="Position"/> is a pan offset — positive X shifts the view right, which
/// makes scene content appear to move left (standard game-camera convention).
/// </para>
/// <para>
/// Unlike a GPU-uniform camera, transforms are applied entirely on the CPU
/// before vertex data is uploaded. Call <see cref="TransformPosition"/> and
/// <see cref="TransformSize"/> when building render data each frame.
/// </para>
/// <para>
/// <b>Zoom:</b> 1.0 = one world pixel maps to exactly one screen pixel. Values above
/// 1.0 magnify the scene; values below 1.0 shrink it. Zoom is applied relative to the
/// screen center so the center of the window stays fixed while zooming.
/// </para>
/// <para>
/// The <see cref="WindowSize"/> is automatically kept in sync with the window
/// via the internal reactable system. No manual resize handling is required.
/// </para>
/// </remarks>
public sealed class Camera2D : ICamera2D
{
    private readonly IDisposable? unsubscriber;
    private float zoom = 0.1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera2D"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public Camera2D()
        : this(IoC.Container.GetInstance<IReactableFactory>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera2D"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications.</param>
    internal Camera2D(IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(reactableFactory);

        var pushWinSizeReactable = reactableFactory.CreatePushWindowSizeReactable();

        this.unsubscriber = pushWinSizeReactable.CreateOneWayReceive(
            PushNotifications.WindowSizeChangedId,
            (data) => WindowSize = new SizeU(data.Width, data.Height),
            () => this.unsubscriber?.Dispose());

        // Get the initial window size in case the camera is created before
        // the window and GL initialization have completed.
        var pullWinSizeReactable = reactableFactory.CreatePullWindowSizeReactable();
        var winSizeData = pullWinSizeReactable.Pull(PullNotifications.GetWindowSizeId);
        WindowSize = new SizeU(winSizeData.Width, winSizeData.Height);
    }

    /// <inheritdoc/>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <inheritdoc/>
    public float Zoom
    {
        get => this.zoom;
        set
        {
            if (value < ZoomMin)
            {
                this.zoom = ZoomMin;
            }
            else if (value > ZoomMax)
            {
                this.zoom = ZoomMax;
            }
            else
            {
                this.zoom = value;
            }
        }
    }

    /// <inheritdoc/>
    public SizeU WindowSize { get; private set; }

    /// <inheritdoc/>
    public float ZoomMin { get; set; } = 0.1f;

    /// <inheritdoc/>
    public float ZoomMax { get; set; } = 4f;

    /// <inheritdoc/>
    public float Rotation { get; set; }

    /// <inheritdoc/>
    public RectangleF WorldBounds
    {
        get
        {
            // Inverse-transform all 4 screen corners to world space
            var topLeft = InverseTransformPosition(Vector2.Zero);
            var topRight = InverseTransformPosition(new Vector2(WindowSize.Width, 0));
            var bottomLeft = InverseTransformPosition(new Vector2(0, WindowSize.Height));
            var bottomRight = InverseTransformPosition(new Vector2(WindowSize.Width, WindowSize.Height));

            var minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            var maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            var minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
            var maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }

    /// <inheritdoc/>
    public Vector2 TransformPosition(Vector2 worldPos)
    {
        var center = new Vector2(WindowSize.Width / 2f, WindowSize.Height / 2f);
        var offset = (worldPos - center - Position) * Zoom;

        if (Rotation != 0f)
        {
            var radians = Rotation * (float)Math.PI / 180f;
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            offset = new Vector2(
                (offset.X * cos) - (offset.Y * sin),
                (offset.X * sin) + (offset.Y * cos));
        }

        return center + offset;
    }

    /// <inheritdoc/>
    public float TransformSize(float size) => size * Zoom;

    /// <inheritdoc/>
    public float TransformRotation(float worldRotation) => worldRotation + Rotation;

    /// <inheritdoc/>
    public bool IsVisible(Vector2 worldPos)
    {
        var screenPos = TransformPosition(worldPos);

        return screenPos.X >= 0 && screenPos.X <= WindowSize.Width &&
               screenPos.Y >= 0 && screenPos.Y <= WindowSize.Height;
    }

    /// <inheritdoc/>
    public bool IsVisible(Vector2 worldPos, Vector2 worldSize)
    {
        var screenPos = TransformPosition(worldPos);
        var screenWidth = TransformSize(worldSize.X);
        var screenHeight = TransformSize(worldSize.Y);

        // Object's screen-space bounds
        var left = screenPos.X;
        var top = screenPos.Y;
        var right = left + screenWidth;
        var bottom = top + screenHeight;

        // Check intersection with screen bounds
        return left < WindowSize.Width && right > 0 &&
               top < WindowSize.Height && bottom > 0;
    }

    /// <summary>
    /// Transforms a screen-space position back to world-space coordinates.
    /// This is the inverse of <see cref="TransformPosition"/>.
    /// </summary>
    /// <param name="screenPos">The position in screen pixel coordinates.</param>
    /// <returns>The position in world pixel coordinates.</returns>
    private Vector2 InverseTransformPosition(Vector2 screenPos)
    {
        var center = new Vector2(WindowSize.Width / 2f, WindowSize.Height / 2f);
        var offset = (screenPos - center) / Zoom;

        if (Rotation != 0f)
        {
            // Inverse rotation (negate the angle)
            var radians = -Rotation * (float)Math.PI / 180f;
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            offset = new Vector2(
                (offset.X * cos) - (offset.Y * sin),
                (offset.X * sin) + (offset.Y * cos));
        }

        return center + offset + Position;
    }
}
