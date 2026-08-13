// <copyright file="LineRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Renderers;

using System;
using System.Drawing;
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
/// Renders lines to the screen using WebGPU.
/// No texture binding is required — lines are rendered procedurally via fragment shader.
/// </summary>
internal sealed class LineRenderer : IDisposable, ILineRenderer
{
    private readonly IWgpuInvoker wgpu;
    private readonly GraphicsLinePipeline pipeline;
    private readonly LineGpuBuffer buffer;
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
    /// Initializes a new instance of the <see cref="LineRenderer"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="reactableFactory">Creates reactables.</param>
    /// <param name="pipeline">The line rendering pipeline.</param>
    /// <param name="buffer">Buffers line data to the GPU.</param>
    /// <param name="frame">The per-frame render pass manager.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    internal LineRenderer(
        IWgpuInvoker wgpu,
        IReactableFactory reactableFactory,
        GraphicsLinePipeline pipeline,
        LineGpuBuffer buffer,
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

        var renderReactable = reactableFactory.CreateRenderLineReactable();

        this.renderUnsubscriber = renderReactable.CreateOneWayReceive(
            PushNotifications.RenderLinesId,
            RenderBatch,
            () => this.renderUnsubscriber?.Dispose());

        var viewportReactable = reactableFactory.CreateViewPortReactable();

        this.viewportUnsubscriber = viewportReactable.CreateOneWayReceive(
            PushNotifications.ViewPortSizeChangedId,
            data => this.buffer.WindowSize = new Vector2(data.Width, data.Height),
            () => this.viewportUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public void Render(Line line, int layer = 0) =>
        RenderBase(line.P1, line.P2, line.Color, (uint)line.Thickness, layer);

    /// <summary>
    /// Gets the current window size used by the GPU buffer for NDC conversion.
    /// </summary>
    /// <remarks>For testing purposes.</remarks>
    internal Vector2 BufferWindowSize => this.buffer.WindowSize;

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, int layer = 0) =>
        RenderBase(start, end, Color.White, 1u, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, Color color, int layer = 0) =>
        RenderBase(start, end, color, 1u, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, uint thickness, int layer = 0) =>
        RenderBase(start, end, Color.White, thickness, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, Color color, uint thickness, int layer = 0) =>
        RenderBase(start, end, color, thickness, layer);

    /// <summary>
    /// The main root method for rendering lines.
    /// </summary>
    /// <param name="start">The start of the line.</param>
    /// <param name="end">The end of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    /// <param name="layer">The layer to render the line.</param>
    private void RenderBase(Vector2 start, Vector2 end, Color color, uint thickness, int layer)
    {
        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var batchItem = new LineBatchItem(
            start,
            end,
            color,
            thickness);

        this.batchManager.AddLineItem(batchItem, layer, DateTime.Now);
    }

    /// <summary>
    /// Invoked every time a batch of lines is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<LineBatchItem>> itemsToRender)
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
