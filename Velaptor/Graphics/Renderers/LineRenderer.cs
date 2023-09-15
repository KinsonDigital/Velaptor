// <copyright file="LineRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using System.Numerics;
using Batching;
using Carbonate.Fluent;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;

/// <inheritdoc cref="ILineRenderer"/>
internal sealed class LineRenderer : RendererBase, ILineRenderer
{
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGpuBuffer<LineBatchItem> buffer;
    private readonly IShaderProgram shader;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public LineRenderer(
        IGLInvoker gl,
        IReactableFactory reactableFactory,
        IOpenGLService openGLService,
        IGpuBuffer<LineBatchItem> buffer,
        IShaderProgram shader,
        IBatchingManager batchManager)
            : base(gl, reactableFactory)
    {
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(buffer);
        EnsureThat.ParamIsNotNull(shader);
        EnsureThat.ParamIsNotNull(batchManager);

        this.batchManager = batchManager;
        this.openGLService = openGLService;
        this.buffer = buffer;
        this.shader = shader;

        var beginBatchReactable = reactableFactory.CreateNoDataPushReactable();

        var beginBatchSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchHasBegunId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchHasBegunId)))
            .BuildNonReceive(() => this.hasBegun = true);

        beginBatchReactable.Subscribe(beginBatchSubscription);

        var renderReactable = reactableFactory.CreateRenderLineReactable();

        var renderSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.RenderLinesId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.RenderLinesId)))
            .BuildOneWayReceive<Memory<RenderItem<LineBatchItem>>>(RenderBatch);

        renderReactable.Subscribe(renderSubscription);
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
            this.openGLService.BeginGroup("Render Line Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Line Process With {this.shader.Name} Shader");

        this.shader.Use();

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

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

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
