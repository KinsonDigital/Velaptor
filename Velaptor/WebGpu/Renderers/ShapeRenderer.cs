// <copyright file="ShapeRenderer.cs" company="KinsonDigital">
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
using NativeInterop.WebGpu.Handles;
using WebGpuBufferUsage = Silk.NET.WebGPU.BufferUsage;
using Velaptor.Batching;

/// <summary>
/// Renders rectangles and circles to the screen using WebGPU.
/// No texture binding is required — shapes are rendered procedurally via fragment shader.
/// </summary>
internal sealed class ShapeRenderer : IShapeRenderer
{
    private readonly IWgpuInvoker wgpu;
    private readonly IGraphicsDevice grfxDevice;
    private readonly IGraphicsSurface surface;
    private readonly IGraphicsShapePipeline shapePipeline;
    private readonly IGraphicsLinePipeline linePipeline;
    private readonly IWebGpuBuffer<ShapeBatchItem> shapeBuffer;
    private readonly IWebGpuBuffer<LineBatchItem> lineBuffer;
    private readonly IFrame frame;
    private readonly IBatchingManager batchManager;
    private readonly IDisposable frameBeginUnsubscriber;
    private readonly IDisposable batchBeginUnsubscriber;
    private readonly IDisposable renderShapesUnsubscriber;
    private readonly IDisposable renderLinesUnsubscriber;
    private readonly IDisposable viewportUnsubscriber;
    private SafeVertexBufferHandle? dpiScaleUniformBuffer;
    private SafeBindGroupHandle? dpiScaleBindGroup;
    private uint shapeBatchOffset;
    private uint lineBatchOffset;
    private bool hasBegun;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRenderer"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="grfxDevice">The graphics device.</param>
    /// <param name="surface">The graphics surface.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications.</param>
    /// <param name="shapePipeline">The shape rendering pipeline.</param>
    /// <param name="linePipeline">The line rendering pipeline.</param>
    /// <param name="shapeBuffer">Manages shape buffer data in the GPU.</param>
    /// <param name="lineBuffer">Manages sine buffer data in the GPU.</param>
    /// <param name="frame">The per-frame render pass manager.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public ShapeRenderer(
        IWgpuInvoker wgpu,
        IGraphicsDevice grfxDevice,
        IGraphicsSurface surface,
        IGraphicsShapePipeline shapePipeline,
        IGraphicsLinePipeline linePipeline,
        IWebGpuBuffer<ShapeBatchItem> shapeBuffer,
        IWebGpuBuffer<LineBatchItem> lineBuffer,
        IFrame frame,
        IBatchingManager batchManager,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(grfxDevice);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(shapePipeline);
        ArgumentNullException.ThrowIfNull(linePipeline);
        ArgumentNullException.ThrowIfNull(shapeBuffer);
        ArgumentNullException.ThrowIfNull(lineBuffer);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(batchManager);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        this.wgpu = wgpu;
        this.grfxDevice = grfxDevice;
        this.surface = surface;
        this.shapePipeline = shapePipeline;
        this.linePipeline = linePipeline;
        this.shapeBuffer = shapeBuffer;
        this.lineBuffer = lineBuffer;
        this.frame = frame;
        this.batchManager = batchManager;

        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        this.frameBeginUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.FrameHasBegunId,
            () =>
            {
                this.shapeBatchOffset = 0;
                this.lineBatchOffset = 0;
            },
            () => this.frameBeginUnsubscriber?.Dispose());

        this.batchBeginUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.BatchHasBegunId,
            () => this.hasBegun = true,
            () => this.batchBeginUnsubscriber?.Dispose());

        var renderShapesReactable = reactableFactory.CreateRenderShapeReactable();
        var renderLinesReactable = reactableFactory.CreateRenderLineReactable();

        this.renderShapesUnsubscriber = renderShapesReactable.CreateOneWayReceive(
            PushNotifications.RenderShapesId,
            RenderShapeBatch,
            () => this.renderShapesUnsubscriber?.Dispose());

        this.renderLinesUnsubscriber = renderLinesReactable.CreateOneWayReceive(
            PushNotifications.RenderLinesId,
            RenderLineBatch,
            () => this.renderLinesUnsubscriber?.Dispose());

        var viewportReactable = reactableFactory.CreateViewPortReactable();

        this.viewportUnsubscriber = viewportReactable.CreateOneWayReceive(
            PushNotifications.ViewPortSizeChangedId,
            data =>
            {
                this.shapeBuffer.WindowSize = new Vector2(data.Width, data.Height);
                this.lineBuffer.WindowSize = new Vector2(data.Width, data.Height);
                UpdateDpiScaleUniform(data.Width, data.Height);
            },
            () => this.viewportUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public void Render(RectShape rect, int layer = 0) => RenderShapeBase(rect.ToBatchItem(), layer);

    /// <inheritdoc/>
    public void Render(CircleShape circle, int layer = 0) => RenderShapeBase(circle.ToBatchItem(), layer);

    /// <inheritdoc/>
    public void Render(Line line, int layer = 0) =>
        RenderLineBase(line.P1, line.P2, line.Color, (uint)line.Thickness, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, int layer = 0) =>
        RenderLineBase(start, end, Color.White, 1u, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, Color color, int layer = 0) =>
        RenderLineBase(start, end, color, 1u, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, uint thickness, int layer = 0) =>
        RenderLineBase(start, end, Color.White, thickness, layer);

    /// <inheritdoc/>
    public void RenderLine(Vector2 start, Vector2 end, Color color, uint thickness, int layer = 0) =>
        RenderLineBase(start, end, color, thickness, layer);

    /// <inheritdoc/>
    public void Dispose() => Dispose(true);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.frameBeginUnsubscriber.Dispose();
            this.batchBeginUnsubscriber.Dispose();
            this.renderShapesUnsubscriber.Dispose();
            this.renderLinesUnsubscriber.Dispose();
            this.viewportUnsubscriber.Dispose();
            this.dpiScaleUniformBuffer?.Dispose();
            this.dpiScaleBindGroup?.Dispose();
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Renders the given <paramref name="batchItem"/> to the screen.
    /// </summary>
    /// <param name="batchItem">The batch item to render.</param>
    /// <param name="layer">The layer to render the item.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    private void RenderShapeBase(ShapeBatchItem batchItem, int layer)
    {
        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        this.batchManager.AddShapeItem(batchItem, layer, DateTime.Now);
    }

    /// <summary>
    /// The main root method for rendering lines.
    /// </summary>
    /// <param name="start">The start of the line.</param>
    /// <param name="end">The end of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    /// <param name="layer">The layer to render the line.</param>
    private void RenderLineBase(Vector2 start, Vector2 end, Color color, uint thickness, int layer)
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
    /// Invoked every time a batch of shapes is ready to be rendered.
    /// </summary>
    private void RenderShapeBatch(Memory<RenderItem<ShapeBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            return;
        }

        if (this.frame.RenderPass is null)
        {
            throw new InvalidOperationException($"The `{nameof(IFrame.RenderPass)}` cannot be null.  Cannot render texture.");
        }

        var renderPass = this.frame.RenderPass;

        this.shapePipeline.Bind(renderPass);

        // Bind the DPI scale uniform buffer
        if (this.dpiScaleBindGroup is not null)
        {
            this.wgpu.RenderPassEncoderSetBindGroup(renderPass, 0, this.dpiScaleBindGroup, 0, 0);
        }

        var totalItemsToRender = 0u;
        var gpuDataIndex = (int)this.shapeBatchOffset - 1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            gpuDataIndex++;
            totalItemsToRender++;

            this.shapeBuffer.UploadData(batchItem, (uint)gpuDataIndex);
        }

        this.shapeBuffer.Draw(renderPass, totalItemsToRender, this.shapeBatchOffset);
        this.shapeBatchOffset += totalItemsToRender;
    }

    /// <summary>
    /// Invoked every time a batch of lines is ready to be rendered.
    /// </summary>
    private void RenderLineBatch(Memory<RenderItem<LineBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            return;
        }

        if (this.frame.RenderPass is null)
        {
            throw new InvalidOperationException($"The `{nameof(IFrame.RenderPass)}` cannot be null.  Cannot render texture.");
        }

        var renderPass = this.frame.RenderPass;

        this.linePipeline.Bind(renderPass);

        var totalItemsToRender = 0u;
        var gpuDataIndex = (int)this.lineBatchOffset - 1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            gpuDataIndex++;
            totalItemsToRender++;

            this.lineBuffer.UploadData(batchItem, (uint)gpuDataIndex);
        }

        this.lineBuffer.Draw(renderPass, totalItemsToRender, this.lineBatchOffset);
        this.lineBatchOffset += totalItemsToRender;
    }

    /// <summary>
    /// Updates the DPI scale uniform buffer based on logical vs. physical framebuffer dimensions.
    /// On macOS HiDPI, the framebuffer is 2× the logical size; on Windows it's typically 1:1.
    /// </summary>
    /// <param name="logicalWidth">The logical window width (in app-visible units).</param>
    /// <param name="logicalHeight">The logical window height (in app-visible units).</param>
    private void UpdateDpiScaleUniform(uint logicalWidth, uint logicalHeight)
    {
        // Lazily create the uniform buffer on first update (device must be initialized)
        // TODO: Check into not doing this in a lazy manner
        if (this.dpiScaleUniformBuffer is null)
        {
            if (this.grfxDevice.Handle is null)
            {
                // TODO: Throw an exception?
                return; // Device not initialized yet
            }

            // TODO: Throw an exception if the 'this.grfxDevice.Queue' is null?

            this.dpiScaleUniformBuffer = this.wgpu.DeviceCreateVertexBuffer(
                this.grfxDevice.Handle,
                "Shape DPI Scale Uniform Buffer",
                8,
                WebGpuBufferUsage.Uniform | WebGpuBufferUsage.CopyDst);
        }

        // Get the physical framebuffer dimensions from the surface
        var framebufferSize = this.surface.FramebufferSize;
        var physicalWidth = (float)framebufferSize.X;
        var physicalHeight = (float)framebufferSize.Y;
        var logicalWidthF = (float)logicalWidth;
        var logicalHeightF = (float)logicalHeight;

        // Calculate scale factors: physical / logical
        var scaleX = logicalWidthF > 0 ? physicalWidth / logicalWidthF : 1.0f;
        var scaleY = logicalHeightF > 0 ? physicalHeight / logicalHeightF : 1.0f;

        // Create and upload the scale factor data
        float[] scaleData = [scaleX, scaleY];
        this.wgpu.QueueWriteBuffer(this.grfxDevice.Queue, this.dpiScaleUniformBuffer.DangerousGetHandle(), 0, scaleData);

        // Lazily create the bind group on first update
        if (this.dpiScaleBindGroup is null)
        {
            this.dpiScaleBindGroup = this.wgpu.DeviceCreateBufferBindGroupHandle(
                this.grfxDevice.Handle,
                "Shape DPI Scale Bind Group",
                this.shapePipeline.BindGroupLayout,
                this.dpiScaleUniformBuffer,
                0,
                8);
        }
    }
}
