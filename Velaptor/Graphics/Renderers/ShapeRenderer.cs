// <copyright file="ShapeRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;

/// <inheritdoc cref="IShapeRenderer"/>
internal sealed class ShapeRenderer : RendererBase, IShapeRenderer
{
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGPUBuffer<ShapeBatchItem> buffer;
    private readonly IShaderProgram shader;
    private readonly IDisposable renderUnsubscriber;
    private readonly IDisposable renderBatchBegunUnsubscriber;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public ShapeRenderer(
        IGLInvoker gl,
        IReactableFactory reactableFactory,
        IOpenGLService openGLService,
        IGPUBuffer<ShapeBatchItem> buffer,
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

        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        var renderStateName = this.GetExecutionMemberName(nameof(PushNotifications.BatchHasBegunId));
        this.renderBatchBegunUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.BatchHasBegunId,
            name: renderStateName,
            onReceive: () => this.hasBegun = true));

        var shapeRenderBatchReactable = reactableFactory.CreateRenderShapeReactable();

        var renderReactorName = this.GetExecutionMemberName(nameof(PushNotifications.RenderShapesId));
        this.renderUnsubscriber = shapeRenderBatchReactable.Subscribe(new ReceiveReactor<Memory<RenderItem<ShapeBatchItem>>>(
            eventId: PushNotifications.RenderShapesId,
            name: renderReactorName,
            onReceiveData: RenderBatch));
    }

    /// <inheritdoc/>
    public void Render(RectShape rect, int layer = 0) => RenderBase(rect.ToBatchItem(), layer);

    /// <inheritdoc/>
    public void Render(CircleShape circle, int layer = 0) => RenderBase(circle.ToBatchItem(), layer);

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
    /// Renders the given <paramref name="batchItem"/> to the screen.
    /// </summary>
    /// <param name="batchItem">The batch item to render.</param>
    /// <param name="layer">The layer to render the item.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    private void RenderBase(ShapeBatchItem batchItem, int layer)
    {
        if (this.hasBegun is false)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        this.batchManager.AddShapeItem(batchItem, layer, DateTime.Now);
    }

    /// <summary>
    /// Invoked every time a batch of shapes are ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<ShapeBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            this.openGLService.BeginGroup("Render Shape Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Shape Process With {this.shader.Name} Shader");

        this.shader.Use();

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            gpuDataIndex++;
            totalItemsToRender++;

            this.openGLService.BeginGroup($"Update Shape Data - BatchItem({i})");
            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
            this.openGLService.EndGroup();
        }

        var totalElements = 6u * totalItemsToRender;

        this.openGLService.BeginGroup($"Render {totalElements} Shape Elements");
        GL.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.openGLService.EndGroup();

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
