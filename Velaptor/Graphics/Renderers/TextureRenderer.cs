﻿// <copyright file="TextureRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using System.Numerics;
using Batching;
using Carbonate;
using Content;
using Exceptions;
using Factories;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;
using NETRect = System.Drawing.Rectangle;

/// <inheritdoc cref="ITextureRenderer"/>
internal sealed class TextureRenderer : ITextureRenderer
{
    private readonly IGLInvoker gl;
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGpuBuffer<TextureBatchItem> buffer;
    private readonly IShaderProgram shader;
    private readonly IDisposable batchBeginUnsubscriber;
    private readonly IDisposable renderTexturesUnsubscriber;
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
        IGpuBuffer<TextureBatchItem> buffer,
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

        this.batchBeginUnsubscriber = beginBatchReactable.CreateNonReceiveOrRespond(
            PushNotifications.BatchHasBegunId,
            () => this.hasBegun = true,
            () => this.batchBeginUnsubscriber?.Dispose());

        var renderReactable = reactableFactory.CreateRenderTextureReactable();

        this.renderTexturesUnsubscriber = renderReactable.CreateOneWayReceive(
            PushNotifications.RenderTexturesId,
            RenderBatch,
            () => this.renderTexturesUnsubscriber?.Dispose());
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
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect(x, y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), angle, 1, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, float angle, float size, int layer = 0)
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

        RenderBase(texture, (srcRect, destRect), angle, size, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, int x, int y, float angle, float size, Color color, int layer = 0)
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

        RenderBase(texture, (srcRect, destRect), angle, size, color, RenderEffects.None, layer);
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
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect(x, y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), 0, 1, color, effects, layer);
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
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), angle, 1, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, float angle, float size, int layer = 0)
    {
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), angle, size, Color.White, RenderEffects.None, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texture"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> has not been invoked before rendering.
    /// </exception>
    public void Render(ITexture texture, Vector2 pos, float angle, float size, Color color, int layer = 0)
    {
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), angle, size, color, RenderEffects.None, layer);
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
        // Render the entire texture
        var srcRect = new NETRect
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect((int)pos.X, (int)pos.Y, (int)texture.Width, (int)texture.Height);

        RenderBase(texture, (srcRect, destRect), 0, 1, color, effects, layer);
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
        if (srcRect.Width <= 0 || srcRect.Height <= 0)
        {
            throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(srcRect));
        }

        RenderBase(texture, (srcRect, destRect), angle, size, color, effects, layer);
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
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

        if (frameNumber < 0 || frameNumber >= frames.Length)
        {
            var exMsg =
                $"The frame number '{frameNumber}' is invalid for atlas '{atlas.Name}' and sub-texture '{subTextureName}'." +
                "\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
            throw new RendererException(exMsg);
        }

        var subTextureData = frames[frameNumber];

        RenderBase(
            atlas.Texture,
            (subTextureData.Bounds, new NETRect((int)pos.X, (int)pos.Y, (int)atlas.Width, (int)atlas.Height)),
            angle,
            size,
            color,
            effects,
            layer);
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
        (NETRect srcRect, NETRect destRect) rects,
        float angle,
        float size,
        Color color,
        RenderEffects effects,
        int layer = 0)
    {
        if (texture is null)
        {
            throw new ArgumentNullException(nameof(texture), $"Cannot render a null '{nameof(ITexture)}'.");
        }

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        (NETRect srcRect, NETRect destRect) = rects;

        if (srcRect.Width <= 0 || srcRect.Height <= 0)
        {
            throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(rects));
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
            this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
            this.openGLService.EndGroup();

            totalItemsToRender = 0;
            gpuDataIndex = -1;
        }

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
