// <copyright file="RectangleRenderer.cs" company="KinsonDigital">
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

/// <inheritdoc cref="IRectangleRenderer"/>
internal sealed class RectangleRenderer : RendererBase, IRectangleRenderer
{
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGPUBuffer<RectBatchItem> buffer;
    private readonly IShaderProgram shader;
    private readonly IDisposable renderUnsubscriber;
    private readonly IDisposable renderBatchBegunUnsubscriber;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public RectangleRenderer(
        IGLInvoker gl,
        IReactableFactory reactableFactory,
        IOpenGLService openGLService,
        IGPUBuffer<RectBatchItem> buffer,
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

        var rectRenderBatchReactable = reactableFactory.CreateRenderRectReactable();

        var renderReactorName = this.GetExecutionMemberName(nameof(PushNotifications.RenderRectsId));
        this.renderUnsubscriber = rectRenderBatchReactable.Subscribe(new ReceiveReactor<Memory<RenderItem<RectBatchItem>>>(
            eventId: PushNotifications.RenderRectsId,
            name: renderReactorName,
            onReceiveData: RenderBatch));
    }

    /// <inheritdoc/>
    public void Render(RectShape rectangle, int layer = 0) => RenderBase(rectangle, layer);

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

    /// <inheritdoc cref="IRectangleRenderer.Render(RectShape,int)"/>
    /// <param name="rectangle">The rectangle to render.</param>
    /// <param name="layer">The layer to render the rectangle.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> method has not been called before rendering.
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If two separate textures have the same <paramref name="layer"/> value, they will
    ///         render in the order that the method was invoked.
    ///     </para>
    ///     <para>Example below:</para>
    ///
    ///     <b>Render Method Invoked Order:</b>
    ///     <list type="number">
    ///         <item>Texture 1 (Layer -10)</item>
    ///         <item>Texture 2 (Layer -20)</item>
    ///         <item>Texture 3 (Layer 0)</item>
    ///         <item>Texture 4 (Layer 0)</item>
    ///         <item>Texture 5 (Layer 4)</item>
    ///         <item>Texture 6 (Layer 3)</item>
    ///     </list>
    ///
    ///     <b>Texture Render Order:</b>
    ///     <list type="bullet">
    ///         <item>Texture 2</item>
    ///         <item>Texture 1</item>
    ///         <item>Texture 3</item>
    ///         <item>Texture 4</item>
    ///         <item>Texture 6</item>
    ///         <item>Texture 5</item>
    ///     </list>
    /// </remarks>
    private void RenderBase(RectShape rectangle, int layer = 0)
    {
        if (this.hasBegun is false)
        {
            throw new InvalidOperationException($"The '{nameof(IRenderer.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var batchItem = new RectBatchItem(
            rectangle.Position,
            rectangle.Width,
            rectangle.Height,
            rectangle.Color,
            rectangle.IsFilled,
            rectangle.BorderThickness,
            rectangle.CornerRadius,
            rectangle.GradientType,
            rectangle.GradientStart,
            rectangle.GradientStop);

        this.batchManager.AddRectItem(batchItem, layer, DateTime.Now);
    }

    /// <summary>
    /// Invoked every time a batch of rectangles is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<RectBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            this.openGLService.BeginGroup("Render Rectangle Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Rectangle Process With {this.shader.Name} Shader");

        this.shader.Use();

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            gpuDataIndex++;
            totalItemsToRender++;

            this.openGLService.BeginGroup($"Update Rectangle Data - BatchItem({i})");
            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
            this.openGLService.EndGroup();
        }

        var totalElements = 6u * totalItemsToRender;

        this.openGLService.BeginGroup($"Render {totalElements} Rectangle Elements");
        GL.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
        this.openGLService.EndGroup();

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
