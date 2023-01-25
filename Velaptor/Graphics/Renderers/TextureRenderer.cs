// <copyright file="TextureRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Content;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;
using NETRect = System.Drawing.Rectangle;
using NETSizeF = System.Drawing.SizeF;

/// <inheritdoc cref="ITextureRenderer"/>
internal sealed class TextureRenderer : RendererBase, ITextureRenderer
{
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGPUBuffer<TextureBatchItem> buffer;
    private readonly IShaderProgram shader;
    private readonly IDisposable renderBatchBegunUnsubscriber;
    private readonly IDisposable renderUnsubscriber;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public TextureRenderer(
        IGLInvoker gl,
        IReactableFactory reactableFactory,
        IOpenGLService openGLService,
        IGPUBuffer<TextureBatchItem> buffer,
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

        const string renderStateName = $"{nameof(TextureRenderer)}.Ctor - {nameof(PushNotifications.BatchHasBegunId)}";
        this.renderBatchBegunUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.BatchHasBegunId,
            name: renderStateName,
            onReceive: () => this.hasBegun = true));

        // TODO: Create a new class named 'ReceiveTextureItemReactor' to simplify 'new ReceiveReactor<Memory<RenderItem<TextureBatchItem>>'
        // This will reduce the amount of code to type and make things more readable.  This is the same the 'IBatchFullReactable';
        // Do this with other classes as well.  This is a generic condenser.
        // ⚠️Cannot do this unless the reactors are not 'sealed'.  Need a new issue to implement this feature into Carbonate

        var textureRenderBatchReactable = reactableFactory.CreateRenderTextureReactable();

        var renderReactorName = this.GetExecutionMemberName(nameof(PushNotifications.RenderTexturesId));
        this.renderUnsubscriber = textureRenderBatchReactable.Subscribe(new ReceiveReactor<Memory<RenderItem<TextureBatchItem>>>(
            eventId: PushNotifications.RenderTexturesId,
            name: renderReactorName,
            onReceiveData: RenderBatch));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, int layer = 0) =>
        Render(texture, x, y, Color.White, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0) =>
        Render(texture, x, y, Color.White, effects, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, Color color, int layer = 0) =>
        Render(texture, x, y, color, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0)
    {
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect(x, y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, srcRect, destRect, 1, 0, color, effects, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    public void Render(
        ITexture texture,
        NETRect srcRect,
        NETRect destRect,
        float size,
        float angle,
        Color color,
        RenderEffects effects,
        int layer = 0) =>
        RenderBase(texture, srcRect, destRect, size, angle, color, effects, layer);

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

    /// <inheritdoc cref="ITextureRenderer.Render(Velaptor.Content.ITexture,Rectangle,Rectangle,float,float,Color,RenderEffects,int)"/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IRenderer.Begin"/> method has not been called before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    private void RenderBase(
        ITexture texture,
        NETRect srcRect,
        NETRect destRect,
        float size,
        float angle,
        Color color,
        RenderEffects effects,
        int layer = 0)
    {
        if (texture is null)
        {
            throw new ArgumentNullException(nameof(texture), $"Cannot render a null '{nameof(ITexture)}'.");
        }

        if (this.hasBegun is false)
        {
            throw new InvalidOperationException($"The '{nameof(IRenderer.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        if (srcRect.Width <= 0 || srcRect.Height <= 0)
        {
            throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(srcRect));
        }

        var itemToAdd = new TextureBatchItem(
            srcRect,
            destRect,
            size,
            angle,
            color,
            effects,
            texture.Id);

        this.batchManager.AddTextureItem(itemToAdd, layer);
    }

    /// <summary>
    /// Invoked every time a batch of textures is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<TextureBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            this.openGLService.BeginGroup("Render Texture Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Texture Process With {this.shader.Name} Shader");

        this.shader.Use();

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        // Only if items are available to render
        for (var i = 0u; i < itemsToRender.Length; i++)
        {
            var batchItem = itemsToRender.Span[(int)i].Item;

            var isLastItem = i >= itemsToRender.Length - 1;
            var isNotLastItem = !isLastItem;

            var nextTextureIsDifferent = isNotLastItem &&
                                         itemsToRender.Span[(int)(i + 1)].Item.TextureId != batchItem.TextureId;
            var shouldRender = isLastItem || nextTextureIsDifferent;
            var shouldNotRender = !shouldRender;

            gpuDataIndex++;
            totalItemsToRender++;

            this.openGLService.BeginGroup($"Update Texture Data - TextureID({batchItem.TextureId}) - BatchItem({i})");
            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
            this.openGLService.EndGroup();

            if (shouldNotRender)
            {
                continue;
            }

            this.openGLService.BindTexture2D(batchItem.TextureId);

            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Texture Elements");
            GL.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
            this.openGLService.EndGroup();

            totalItemsToRender = 0;
            gpuDataIndex = -1;
        }

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
