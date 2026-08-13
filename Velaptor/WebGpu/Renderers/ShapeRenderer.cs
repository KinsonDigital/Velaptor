// <copyright file="ShapeRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Renderers;

using System;
using System.Numerics;
using Batching;
using Buffers;
using Carbonate;
using Factories;
using Graphics;
using Graphics.Renderers;
using NativeInterop.WebGpu;
using Velaptor.Batching;

/// <summary>
/// Renders rectangles and circles to the screen using WebGPU.
/// No texture binding is required — shapes are rendered procedurally via fragment shader.
/// </summary>
internal sealed class ShapeRenderer : IDisposable, IShapeRenderer
{
    private readonly IWgpuInvoker wgpu;
    private readonly GraphicsShapePipeline pipeline;
    private readonly ShapeGpuBuffer buffer;
    private readonly IFrame frame;
    private readonly IBatchingManager batchManager;
    private readonly IDisposable frameBeginUnsubscriber;
    private readonly IDisposable batchBeginUnsubscriber;
    private readonly IDisposable renderUnsubscriber;
    private readonly IDisposable viewportUnsubscriber;
    private uint batchOffset;
    private bool hasBegun;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRenderer"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications.</param>
    /// <param name="pipeline">The shape rendering pipeline.</param>
    /// <param name="buffer">Buffers shape data to the GPU.</param>
    /// <param name="frame">The per-frame render pass manager.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    internal ShapeRenderer(
        IWgpuInvoker wgpu,
        IReactableFactory reactableFactory,
        GraphicsShapePipeline pipeline,
        ShapeGpuBuffer buffer,
        IFrame frame,
        IBatchingManager batchManager)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(batchManager);

        this.wgpu = wgpu;
        this.pipeline = pipeline;
        this.buffer = buffer;
        this.frame = frame;
        this.batchManager = batchManager;

        var beginBatchReactable = reactableFactory.CreateNoDataPushReactable();

        this.frameBeginUnsubscriber = beginBatchReactable.CreateNonReceiveOrRespond(
            PushNotifications.FrameHasBegunId,
            () => this.batchOffset = 0,
            () => this.frameBeginUnsubscriber?.Dispose());

        this.batchBeginUnsubscriber = beginBatchReactable.CreateNonReceiveOrRespond(
            PushNotifications.BatchHasBegunId,
            () => this.hasBegun = true,
            () => this.batchBeginUnsubscriber?.Dispose());

        var renderReactable = reactableFactory.CreateRenderShapeReactable();

        this.renderUnsubscriber = renderReactable.CreateOneWayReceive(
            PushNotifications.RenderShapesId,
            RenderBatch,
            () => this.renderUnsubscriber?.Dispose());

        var viewportReactable = reactableFactory.CreateViewPortReactable();

        this.viewportUnsubscriber = viewportReactable.CreateOneWayReceive(
            PushNotifications.ViewPortSizeChangedId,
            data => this.buffer.WindowSize = new Vector2(data.Width, data.Height),
            () => this.viewportUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public void Render(RectShape rect, int layer = 0) => RenderBase(rect.ToBatchItem(), layer);

    /// <summary>
    /// Gets the current window size used by the GPU buffer for NDC conversion.
    /// </summary>
    /// <remarks>For testing purposes.</remarks>
    internal Vector2 BufferWindowSize => this.buffer.WindowSize;

    /// <inheritdoc/>
    public void Render(CircleShape circle, int layer = 0) => RenderBase(circle.ToBatchItem(), layer);

    /// <summary>
    /// Renders the given <paramref name="batchItem"/> to the screen.
    /// </summary>
    /// <param name="batchItem">The batch item to render.</param>
    /// <param name="layer">The layer to render the item.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    private void RenderBase(ShapeBatchItem batchItem, int layer)
    {
        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        this.batchManager.AddShapeItem(batchItem, layer, DateTime.Now);
    }

    /// <summary>
    /// Invoked every time a batch of shapes is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<ShapeBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            return;
        }

        var renderPass = this.frame.RenderPass;

        if (renderPass is null)
        {
            return;
        }

        // Ensure the GPU buffer is large enough before any upload to avoid
        // mid-render-pass resizes that invalidate previously recorded draw commands.
        var requiredCapacity = this.batchOffset + (uint)itemsToRender.Length;
        this.buffer.EnsureCapacity(requiredCapacity);

        this.pipeline.Bind(renderPass);

        var totalItemsToRender = 0u;
        var gpuDataIndex = (int)this.batchOffset - 1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            gpuDataIndex++;
            totalItemsToRender++;

            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
        }

        this.buffer.Draw(renderPass, totalItemsToRender, this.batchOffset);
        this.batchOffset += totalItemsToRender;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.frameBeginUnsubscriber.Dispose();
        this.batchBeginUnsubscriber.Dispose();
        this.renderUnsubscriber.Dispose();
        this.viewportUnsubscriber.Dispose();
    }
}
