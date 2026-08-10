// <copyright file="WgpuBatcher.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Batching;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Carbonate;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Graphics.Renderers.Exceptions;
using ReactableData;

/// <summary>
/// Manages the batch rendering lifecycle for the WebGPU backend.
/// Wraps the <see cref="Frame"/> render pass cycle: begin, clear, end, submit.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test it due to direct interaction with the 'IoC' container.")]
internal sealed class WgpuBatcher : IBatcher
{
    private const string RenderExceptionMsg = "The renderer is not initialized.";
    private const uint InitialBatchSize = 1000;
    private readonly IPushReactable pushReactable;
    private readonly Frame frame;
    private readonly GraphicsTexturePipeline texturePipeline;
    private readonly GraphicsShapePipeline shapePipeline;
    private readonly GraphicsLinePipeline linePipeline;
    private readonly IDisposable initUnsubscriber;
    private readonly IDisposable submitUnsubscriber;
    private Color clearColor = Color.FromArgb(255, 16, 29, 36);
    private int frameDepth;
    private bool frameBegun;
    private bool isInitialized;
    private readonly IDisposable reconfigureUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="WgpuBatcher"/> class.
    /// </summary>
    /// <param name="pushReactable">Pushes notifications for batch lifecycle events.</param>
    /// <param name="batchSizeReactable">Pushes batch size data to renderers.</param>
    /// <param name="frame">The per-frame render pass manager.</param>
    /// <param name="texturePipeline">The texture render pipeline.</param>
    /// <param name="shapePipeline">The shape render pipeline.</param>
    /// <param name="linePipeline">The line render pipeline.</param>
    public WgpuBatcher(
        IPushReactable pushReactable,
        IPushReactable<BatchSizeData> batchSizeReactable,
        Frame frame,
        GraphicsTexturePipeline texturePipeline,
        GraphicsShapePipeline shapePipeline,
        GraphicsLinePipeline linePipeline)
    {
        ArgumentNullException.ThrowIfNull(pushReactable);
        ArgumentNullException.ThrowIfNull(batchSizeReactable);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(texturePipeline);
        ArgumentNullException.ThrowIfNull(shapePipeline);
        ArgumentNullException.ThrowIfNull(linePipeline);

        this.pushReactable = pushReactable;
        this.frame = frame;
        this.texturePipeline = texturePipeline;
        this.shapePipeline = shapePipeline;
        this.linePipeline = linePipeline;

        this.initUnsubscriber = this.pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.GLInitializedId,
            () =>
            {
                if (this.isInitialized)
                {
                    return;
                }

                // Initialize the WebGPU surface, adapter, device, and swap chain.
                // Must happen after the window is showing (this notification is pushed
                // after the Silk.NET window has completed its Load/OnLoad callback).
                this.frame.Initialize();

                // Initialize all GPU pipelines now that the device is ready.
                this.texturePipeline.Initialize();
                this.shapePipeline.Initialize();
                this.linePipeline.Initialize();

                foreach (var batchType in Enum.GetValues<BatchType>())
                {
                    batchSizeReactable.Push(
                        PushNotifications.BatchSizeChangedId,
                        new BatchSizeData { BatchSize = InitialBatchSize, TypeOfBatch = batchType });
                }

                this.isInitialized = true;
            },
            () => this.initUnsubscriber?.Dispose());

        // The SubmitRenderPassId notification is pushed once per logical frame from
        // GLWindow_Render AFTER all Begin/End cycles have completed. This ends the
        // render pass, submits the command buffer, and presents the frame so that
        // sequential Begin/End pairs (scene rendering then control rendering) all
        // contribute to the same swap-chain texture.
        this.submitUnsubscriber = this.pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.SubmitRenderPassId,
            () =>
            {
                if (this.frameDepth > 0)
                {
                    return;
                }

                this.frame.Submit();
                this.frameBegun = false;
            },
            () => this.submitUnsubscriber?.Dispose());

        // When the window framebuffer changes size the swap chain textures must be
        // reallocated. Mark the frame as needing reconfiguration so that the next
        // Begin() call calls surface.Configure() with the new dimensions before
        // acquiring a surface texture.
        this.reconfigureUnsubscriber = this.pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.SurfaceReconfigureId,
            () => this.frame.Reconfigure(),
            () => this.reconfigureUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public Color ClearColor
    {
        get => this.clearColor;
        set => this.clearColor = value;
    }

    /// <inheritdoc/>
    public bool HasBegun { get; private set; }

    /// <inheritdoc/>
    public void Begin()
    {
        if (!this.isInitialized)
        {
            throw new RendererException(RenderExceptionMsg);
        }

        // Acquire the render pass from the swap chain if one isn't already
        // active (nested Begin calls reuse the existing pass).
        if (!this.frameBegun)
        {
            this.frame.Begin(this.clearColor);
            this.pushReactable.Push(PushNotifications.FrameHasBegunId);
            this.frameBegun = true;
        }

        this.frameDepth++;
        HasBegun = true;

        this.pushReactable.Push(PushNotifications.BatchHasBegunId);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        // The clear is handled during Frame.Begin via the render pass clear operation.
        // WebGPU does not support ad-hoc clearing once the render pass is active.
    }

    /// <inheritdoc/>
    public void End()
    {
        if (!this.isInitialized)
        {
            throw new RendererException(RenderExceptionMsg);
        }

        HasBegun = false;

        this.pushReactable.Push(PushNotifications.BatchHasEndedId);
        this.frameDepth--;
    }
}
