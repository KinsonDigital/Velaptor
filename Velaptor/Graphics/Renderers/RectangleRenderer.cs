// <copyright file="RectangleRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Linq;
using Carbonate;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Buffers;
using OpenGL.Shaders;
using Services;

/// <inheritdoc cref="IRectangleRenderer"/>
internal sealed class RectangleRenderer : RendererBase, IRectangleRenderer
{
    private readonly IBatchingService<RectBatchItem> batchService;
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
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchService">Batches items for rendering.</param>
    public RectangleRenderer(
        IGLInvoker gl,
        IPushReactable reactable,
        IOpenGLService openGLService,
        IGPUBuffer<RectBatchItem> buffer,
        IShaderProgram shader,
        IBatchingService<RectBatchItem> batchService)
            : base(gl, reactable)
    {
        EnsureThat.ParamIsNotNull(batchService);

        this.batchService = batchService;
        this.openGLService = openGLService;
        this.buffer = buffer;
        this.shader = shader;

        var batchEndName = this.GetExecutionMemberName(nameof(NotificationIds.RenderRectsId));
        this.renderUnsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderRectsId,
            name: batchEndName,
            onReceive: RenderBatch));

        const string renderStateName = $"{nameof(RectangleRenderer)}.Ctor - {nameof(NotificationIds.RenderBatchBegunId)}";
        this.renderBatchBegunUnsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderBatchBegunId,
            name: renderStateName,
            onReceive: () => this.hasBegun = true));
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
            rectangle.GradientStop,
            layer);

        this.batchService.Add(batchItem);
    }

    // TODO: Create more overloads as features

    /// <summary>
    /// Invoked every time a batch of rectangles is ready to be rendered.
    /// </summary>
    private void RenderBatch()
    {
        if (this.batchService.BatchItems.Count <= 0)
        {
            this.openGLService.BeginGroup("Render Rectangle Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Rectangle Process With {this.shader.Name} Shader");

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

                this.openGLService.BeginGroup($"Update Rectangle Data - BatchItem({i})");
                this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
                this.openGLService.EndGroup();
            }

            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Rectangle Elements");
            GL.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
            this.openGLService.EndGroup();

            // Empties the batch
            this.batchService.EmptyBatch();
        }

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
