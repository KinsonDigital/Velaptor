// <copyright file="TextureRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Renderers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Batching;
using Buffers;
using Carbonate;
using Content;
using Graphics;
using Graphics.Renderers;
using Graphics.Renderers.Exceptions;
using NativeInterop.WebGpu;
using Velaptor.Batching;
using Factories;
using NETRect = System.Drawing.Rectangle;

/// <inheritdoc cref="ITextureRenderer"/>
internal sealed class TextureRenderer : ITextureRenderer, IDisposable
{
    private readonly IWgpuInvoker wgpu;
    private readonly IBatchingManager batchManager;
    private readonly IGraphicsTexturePipeline pipeline;
    private readonly IWebGpuBuffer<TextureBatchItem> buffer;
    private readonly IFrame frame;
    private readonly TextureBindGroupRegistry bindGroupRegistry;
    private readonly IDisposable frameBeginUnsubscriber;
    private readonly IDisposable batchBeginUnsubscriber;
    private readonly IDisposable renderTexturesUnsubscriber;
    private readonly IDisposable viewPortUnsubscriber;
    private uint batchOffset;
    private bool hasBegun;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRenderer"/> class.
    /// </summary>
    /// <param name="wgpu">Invokes WebGPU functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="pipeline">The texture render pipeline.</param>
    /// <param name="buffer">Buffers texture quad data to the GPU.</param>
    /// <param name="frame">The per-frame render pass manager.</param>
    /// <param name="bindGroupRegistry">Resolves texture IDs to bind groups.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists",
        Justification = "Reactable subscribe call requires 'this' prefix.")]
    public TextureRenderer(
        IWgpuInvoker wgpu,
        IGraphicsTexturePipeline pipeline,
        IWebGpuBuffer<TextureBatchItem> buffer,
        IFrame frame,
        TextureBindGroupRegistry bindGroupRegistry,
        IBatchingManager batchManager,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(bindGroupRegistry);
        ArgumentNullException.ThrowIfNull(batchManager);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        this.wgpu = wgpu;
        this.batchManager = batchManager;
        this.pipeline = pipeline;
        this.buffer = buffer;
        this.frame = frame;
        this.bindGroupRegistry = bindGroupRegistry;

        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        this.frameBeginUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.FrameHasBegunId,
            () => this.batchOffset = 0,
            () => this.frameBeginUnsubscriber?.Dispose());

        this.batchBeginUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.BatchHasBegunId,
            () => this.hasBegun = true,
            () => this.batchBeginUnsubscriber?.Dispose());

        var renderReactable = reactableFactory.CreateRenderTextureReactable();

        this.renderTexturesUnsubscriber = renderReactable.CreateOneWayReceive(PushNotifications.RenderTexturesId,
            RenderBatch,
            () => this.renderTexturesUnsubscriber?.Dispose());

        var viewportReactable = reactableFactory.CreateViewPortReactable();

        this.viewPortUnsubscriber = viewportReactable.CreateOneWayReceive(PushNotifications.ViewPortSizeChangedId,
            data => this.buffer.WindowSize = new Vector2(data.Width, data.Height),
            () => this.viewPortUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, int layer = 0) =>
        Render(texture, x, y, Color.White, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, float angle, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect(x, y, width, height);

        RenderBase(texture, srcRect, destRect, angle, 1, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, float angle, float size, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect(x, y, width, height);

        RenderBase(texture, srcRect, destRect, angle, size, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, float angle, float size, Color color, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect(x, y, width, height);

        RenderBase(texture, srcRect, destRect, angle, size, color, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0) =>
        Render(texture, x, y, Color.White, effects, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, Color color, int layer = 0) =>
        Render(texture, x, y, color, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect(x, y, width, height);

        RenderBase(texture, srcRect, destRect, 0, 1, color, effects, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, int layer = 0) =>
        Render(texture, (int)pos.X, (int)pos.Y, Color.White, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, float angle, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, width, height);

        RenderBase(texture, srcRect, destRect, angle, 1, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, float angle, float size, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, width, height);

        RenderBase(texture, srcRect, destRect, angle, size, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, float angle, float size, Color color, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, width, height);

        RenderBase(texture, srcRect, destRect, angle, size, color, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, RenderEffects effects, int layer = 0) =>
        Render(texture, (int)pos.X, (int)pos.Y, Color.White, effects, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, Color color, int layer = 0) =>
        Render(texture, (int)pos.X, (int)pos.Y, color, RenderEffects.None, layer);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, Color color, RenderEffects effects, int layer = 0)
    {
        var (width, height) = GetTextureSize(texture);

        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = width,
            Height = height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, width, height);

        RenderBase(texture, srcRect, destRect, 0, 1, color, effects, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
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
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(texture);

        RenderBase(texture, srcRect, destRect, angle, size, color, effects, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(IAtlasData atlas, string subTextureName, Vector2 pos, int frameNumber = 0, int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            0f,
            1f,
            Color.White,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(IAtlasData atlas, string subTextureName, Vector2 pos, Color color, int frameNumber = 0, int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            0f,
            1f,
            color,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, int frameNumber = 0, int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            angle,
            1f,
            Color.White,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, int frameNumber = 0, int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            angle,
            size,
            Color.White,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, Color color, int frameNumber = 0, int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            angle,
            1f,
            color,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(
        IAtlasData atlas,
        string subTextureName,
        Vector2 pos,
        float angle,
        float size,
        Color color,
        int frameNumber = 0,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            angle,
            size,
            color,
            RenderEffects.None,
            layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlas"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    /// <exception cref="RendererException">
    ///     Thrown if the <paramref name="frameNumber"/> refers to a frame that does not exist for a sub-texture in an atlas.
    /// </exception>
    public void Render(
        IAtlasData atlas,
        string subTextureName,
        Vector2 pos,
        float angle,
        float size,
        Color color,
        RenderEffects effects,
        int frameNumber = 0,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(atlas);

        var frames = atlas.GetFrames(subTextureName);

        ValidateFrameNumber(frameNumber, frames.Length, atlas.Name, subTextureName);

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            subTextureData.Bounds,
            new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Texture.Width, (int)atlas.Texture.Height),
            angle,
            size,
            color,
            effects,
            layer);
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
        this.renderTexturesUnsubscriber.Dispose();
        this.viewPortUnsubscriber.Dispose();
    }

    /// <summary>
    /// Gets the size of the texture from the given <paramref name="texture"/>.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <returns>The size of the texture.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the texture is null.</exception>
    private static (int width, int height) GetTextureSize(ITexture texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        return ((int)texture.Width, (int)texture.Height);
    }

    /// <summary>
    /// Validates the given <paramref name="frameNumber"/> checking if it is within a range. Throws an exception
    /// if the frame is not within range.
    /// </summary>
    /// <param name="frameNumber">The number of the frame.</param>
    /// <param name="totalFrames">The total number of frames.</param>
    /// <param name="atlasName">The name of the atlas.</param>
    /// <param name="subTextureName">the name of the subtexture.</param>
    /// <exception cref="RendererException">Thrown if the <paramref name="frameNumber"/> is not withing range.</exception>
    private static void ValidateFrameNumber(int frameNumber, int totalFrames, string atlasName, string subTextureName)
    {
        if (frameNumber < 0 || frameNumber >= totalFrames)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlasName}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }
    }

    /// <inheritdoc cref="ITextureRenderer.Render(Velaptor.Content.ITexture,Rectangle,Rectangle,float,float,Color,RenderEffects,int)"/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> method has not been called before rendering.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the source rectangle width or height is less than or equal to 0.
    /// </exception>
    private void RenderBase(
        ITexture texture,
        NETRect srcRect,
        NETRect destRect,
        float angle,
        float size,
        Color color,
        RenderEffects effects,
        int layer = 0)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
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

        this.batchManager.AddTextureItem(itemToAdd, layer, DateTime.Now);
    }

    /// <summary>
    /// Invoked every time a batch of textures is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<TextureBatchItem>> itemsToRender)
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

        this.pipeline.Bind(renderPass);

        var totalItemsToRender = 0u;
        var gpuDataIndex = (int)this.batchOffset - 1;

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

            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);

            if (shouldNotRender)
            {
                continue;
            }

            var bindGroup = this.bindGroupRegistry.GetBindGroup(batchItem.TextureId);

            if (bindGroup is not null)
            {
                this.wgpu.RenderPassEncoderSetBindGroup(renderPass, 0, bindGroup, 0, 0);
                this.buffer.Draw(renderPass, totalItemsToRender, this.batchOffset);
            }

            this.batchOffset += totalItemsToRender;
            totalItemsToRender = 0;
            gpuDataIndex = (int)this.batchOffset - 1;
        }

        this.hasBegun = false;
    }
}
