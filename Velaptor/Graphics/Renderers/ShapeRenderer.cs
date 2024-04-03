// <copyright file="ShapeRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using Batching;
using Carbonate.Fluent;
using Factories;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;

/// <inheritdoc cref="IShapeRenderer"/>
internal sealed class ShapeRenderer : IShapeRenderer
{
    private readonly IGLInvoker gl;
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGpuBuffer<ShapeBatchItem> buffer;
    private readonly IShaderProgram shader;
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
        IGpuBuffer<ShapeBatchItem> buffer,
        IShaderProgram shader,
        IBatchingManager batchManager)
    {
        ArgumentNullException.ThrowIfNull(gl);
        ArgumentNullException.ThrowIfNull(openGLService);
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(shader);
        ArgumentNullException.ThrowIfNull(batchManager);

        this.gl = gl;
        this.batchManager = batchManager;
        this.openGLService = openGLService;
        this.buffer = buffer;
        this.shader = shader;

        var beginBatchReactable = reactableFactory.CreateNoDataPushReactable();

        var beginBatchSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchHasBegunId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchHasBegunId)))
            .BuildNonReceiveOrRespond(() => this.hasBegun = true);

        beginBatchReactable.Subscribe(beginBatchSubscription);

        var renderReactable = reactableFactory.CreateRenderShapeReactable();

        var renderSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.RenderShapesId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.RenderShapesId)))
            .BuildOneWayReceive<Memory<RenderItem<ShapeBatchItem>>>(RenderBatch);

        renderReactable.Subscribe(renderSubscription);
    }

    /// <inheritdoc/>
    public void Render(RectShape rect, int layer = 0) => RenderBase(rect.ToBatchItem(), layer);

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
        this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.openGLService.EndGroup();

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
