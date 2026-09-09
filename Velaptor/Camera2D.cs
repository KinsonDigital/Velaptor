// <copyright file="Camera2D.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Carbonate;
using Factories;

/// <summary>
/// A simple 2-D camera that applies pan and zoom to rendered objects on the CPU.
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
/// screen centre so the centre of the window stays fixed while zooming.
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
    public Vector2 TransformPosition(Vector2 worldPos)
    {
        var center = new Vector2(WindowSize.Width / 2f, WindowSize.Height / 2f);

        return center + ((worldPos - center - Position) * Zoom);
    }

    /// <inheritdoc/>
    public float TransformSize(float size) => size * Zoom;
}
