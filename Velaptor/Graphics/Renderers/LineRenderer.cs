// <copyright file="LineRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Carbonate;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Buffers;
using OpenGL.Shaders;
using Services;

/// <inheritdoc cref="ILineRenderer"/>
internal sealed class LineRenderer : RendererBase, ILineRenderer
{
    private readonly IBatchingService<LineBatchItem> batchService;
    private readonly IOpenGLService openGLService;
    private readonly IGPUBuffer<LineBatchItem> buffer;
    private readonly IShaderProgram shader;
    private readonly IDisposable renderUnsubscriber;
    private readonly IDisposable renderBatchBegunUnsubscriber;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchService">Batches items for rendering.</param>
    public LineRenderer(
        IGLInvoker gl,
        IPushReactable reactable,
        IOpenGLService openGLService,
        IGPUBuffer<LineBatchItem> buffer,
        IShaderProgram shader,
        IBatchingService<LineBatchItem> batchService)
            : base(gl, reactable)
    {
        EnsureThat.ParamIsNotNull(batchService);

        this.batchService = batchService;
        this.openGLService = openGLService;
        this.buffer = buffer;
        this.shader = shader;

        var batchEndName = this.GetExecutionMemberName(nameof(NotificationIds.RenderLinesId));
        this.renderUnsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderLinesId,
            name: batchEndName,
            onReceive: RenderBatch));

        const string renderStateName = $"{nameof(LineRenderer)}.Ctor - {nameof(NotificationIds.RenderBatchBegunId)}";
        this.renderBatchBegunUnsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderBatchBegunId,
            name: renderStateName,
            onReceive: () => this.hasBegun = true));
    }

    /// <inheritdoc/>
    public void Render(Line line, int layer = 0) =>
        RenderBase(line.P1, line.P2, line.Color, (uint)line.Thickness, layer);

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
    /// Shuts down the application by disposing resources.
    /// </summary>
    protected override void ShutDown()
    {
        if (IsDisposed)
        {
            return;
        }

        this.renderUnsubscriber.Dispose();
        this.renderBatchBegunUnsubscriber.Dispose();

        base.ShutDown();
    }

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
        if (this.hasBegun is false)
        {
            throw new InvalidOperationException($"The '{nameof(IRenderer.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var batchItem = new LineBatchItem(
            start,
            end,
            color,
            thickness,
            layer);

        this.batchService.Add(batchItem);
    }

    /// <summary>
    /// Invoked every time a batch of lines is ready to be rendered.
    /// </summary>
    private void RenderBatch()
    {
        if (this.batchService.BatchItems.Count <= 0)
        {
            this.openGLService.BeginGroup("Render Line Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Line Process With {this.shader.Name} Shader");

        this.shader.Use();

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        var itemsToRender = this.batchService.BatchItems
            .Where(i => i.IsEmpty() is false)
            .Select(i => i)
            .OrderBy(i => i.Layer)
            .ToArray();

        // Only if items are available to render
        if (itemsToRender.Length > 0)
        {
            for (var i = 0u; i < itemsToRender.Length; i++)
            {
                var batchItem = itemsToRender[(int)i];

                gpuDataIndex++;
                totalItemsToRender++;

                this.openGLService.BeginGroup($"Update Line Data - BatchItem({i})");
                this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
                this.openGLService.EndGroup();
            }

            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Line Elements");
            GL.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
            this.openGLService.EndGroup();

            // Empties the batch
            this.batchService.EmptyBatch();
        }

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
