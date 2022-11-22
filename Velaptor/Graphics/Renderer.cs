// <copyright file="Renderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Guards;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.OpenGL.Shaders;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Velaptor.Services;
using NETRect = System.Drawing.Rectangle;
using NETSizeF = System.Drawing.SizeF;

namespace Velaptor.Graphics;

/// <inheritdoc/>
internal sealed class Renderer : IRenderer
{
    private const uint BatchSize = 1000;
    private readonly Dictionary<string, CachedValue<uint>> cachedUIntProps = new ();
    private readonly IGLInvoker gl;
    private readonly IOpenGLService openGLService;
    private readonly IShaderManager shaderManager;
    private readonly IBufferManager bufferManager;
    private readonly IBatchServiceManager batchServiceManager;
    private readonly IDisposable glInitUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;

    // ReSharper disable once MemberInitializerValueIgnored
    private CachedValue<Color> cachedClearColor = null!;
    private bool isDisposed;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="Renderer"/> class.
    /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderManager">Manages various shader operations.</param>
    /// <param name="bufferManager">Manages various buffer operations.</param>
    /// <param name="batchServiceManager">Manages the batching of various items to be rendered.</param>
    /// <param name="glInitReactable">Provides push notifications that OpenGL has been initialized.</param>
    /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
    /// <param name="batchSizeReactable">Sends a push notification of the batch size.</param>
    /// <remarks>
    ///     <paramref name="glInitReactable"/> is subscribed to in this class.  <see cref="GLWindow"/>
    ///     pushes the notification that OpenGL has been initialized.
    /// </remarks>
    public Renderer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderManager shaderManager,
        IBufferManager bufferManager,
        IBatchServiceManager batchServiceManager,
        IReactable<GLInitData> glInitReactable,
        IReactable<ShutDownData> shutDownReactable,
        IReactable<BatchSizeData> batchSizeReactable)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(shaderManager);
        EnsureThat.ParamIsNotNull(bufferManager);
        EnsureThat.ParamIsNotNull(batchServiceManager);
        EnsureThat.ParamIsNotNull(glInitReactable);
        EnsureThat.ParamIsNotNull(shutDownReactable);
        EnsureThat.ParamIsNotNull(batchSizeReactable);

        this.gl = gl;
        this.openGLService = openGLService;
        this.shaderManager = shaderManager;
        this.bufferManager = bufferManager;

        this.batchServiceManager = batchServiceManager;
        this.batchServiceManager.TextureBatchReadyForRendering += TextureBatchService_BatchReadyForRendering;
        this.batchServiceManager.FontGlyphBatchReadyForRendering += FontGlyphBatchService_BatchReadyForRendering;
        this.batchServiceManager.RectBatchReadyForRendering += RectBatchService_BatchReadyForRendering;

        // Receive a push notification that OpenGL has initialized
        this.glInitUnsubscriber = glInitReactable.Subscribe(new Reactor<GLInitData>(
            _ =>
            {
                this.cachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

                this.cachedClearColor.IsCaching = false;

                Init();
            }, onCompleted: () =>
            {
                this.glInitUnsubscriber?.Dispose();
            }));

        this.shutDownUnsubscriber = shutDownReactable.Subscribe(new Reactor<ShutDownData>(
            _ => ShutDown(),
            onCompleted: () =>
            {
                this.shutDownUnsubscriber?.Dispose();
            }));

        var batchSizeData = new BatchSizeData(BatchSize);
        batchSizeReactable.PushNotification(batchSizeData);
        batchSizeReactable.EndNotifications();
        batchSizeReactable.UnsubscribeAll();

        SetupPropertyCaches();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Renderer"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~Renderer()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        ShutDown();
    }

    /// <inheritdoc/>
    public uint RenderSurfaceWidth
    {
        get => this.cachedUIntProps[nameof(RenderSurfaceWidth)].GetValue();
        set => this.cachedUIntProps[nameof(RenderSurfaceWidth)].SetValue(value);
    }

    /// <inheritdoc/>
    public uint RenderSurfaceHeight
    {
        get => this.cachedUIntProps[nameof(RenderSurfaceHeight)].GetValue();
        set => this.cachedUIntProps[nameof(RenderSurfaceHeight)].SetValue(value);
    }

    /// <inheritdoc/>
    public Color ClearColor
    {
        get => this.cachedClearColor.GetValue();
        set => this.cachedClearColor.SetValue(value);
    }

    /// <inheritdoc/>
    public void Begin() => this.hasBegun = true;

    /// <inheritdoc/>
    public void Clear() => this.gl.Clear(GLClearBufferMask.ColorBufferBit);

    /// <inheritdoc/>
    public void OnResize(SizeU size)
    {
        this.bufferManager.SetViewPortSize(VelaptorBufferType.Texture, size);
        this.bufferManager.SetViewPortSize(VelaptorBufferType.Font, size);
        this.bufferManager.SetViewPortSize(VelaptorBufferType.Rectangle, size);
    }

    /// <inheritdoc/>
    public void Render(ITexture texture, int x, int y, int layer = 0) => Render(texture, x, y, Color.White, layer);

    /// <inheritdoc/>
    public void Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0) => Render(texture, x, y, Color.White, effects, layer);

    /// <inheritdoc/>
    public void Render(ITexture texture, int x, int y, Color color, int layer = 0) => Render(texture, x, y, color, RenderEffects.None, layer);

    /// <inheritdoc/>
    public void Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0)
    {
        // Render the entire texture
        var srcRect = new NETRect()
        {
            X = 0,
            Y = 0,
            Width = (int)texture.Width,
            Height = (int)texture.Height,
        };

        var destRect = new NETRect(x, y, (int)texture.Width, (int)texture.Height);

        Render(texture, srcRect, destRect, 1, 0, color, effects, layer);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="Begin"/>() method is not called before calling this method.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the <see cref="Rectangle.Width"/> or <see cref="Rectangle.Height"/> property
    ///     values for the <paramref name="srcRect"/> argument are less than or equal to 0.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="texture"/> argument is null.
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
        if (texture is null)
        {
            throw new ArgumentNullException(nameof(texture), $"Cannot render a null '{nameof(ITexture)}'.");
        }

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        if (srcRect.Width <= 0 || srcRect.Height <= 0)
        {
            throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(srcRect));
        }

        var itemToAdd = default(TextureBatchItem);

        itemToAdd.SrcRect = srcRect;
        itemToAdd.DestRect = destRect;
        itemToAdd.Size = size;
        itemToAdd.Angle = angle;
        itemToAdd.TintColor = color;
        itemToAdd.Effects = effects;
        itemToAdd.ViewPortSize = new SizeF(RenderSurfaceWidth, RenderSurfaceHeight);
        itemToAdd.TextureId = texture.Id;
        itemToAdd.Layer = layer;

        this.batchServiceManager.AddTextureBatchItem(itemToAdd);
    }

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y)
        => Render(font, text, x, y, 1f, 0f, Color.White);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position)
        => Render(font, text, (int)position.X, (int)position.Y, 1f, 0f, Color.White);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, float renderSize, float angle)
        => Render(font, text, x, y, renderSize, angle, Color.White);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, float renderSize, float angle)
        => Render(font, text, (int)position.X, (int)position.Y, renderSize, angle, Color.White);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, Color color)
        => Render(font, text, x, y, 1f, 0f, color);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, Color color)
        => Render(font, text, (int)position.X, (int)position.Y, 0f, 0f, color);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, float angle, Color color)
        => Render(font, text, x, y, 1f, angle, color);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, float angle, Color color)
        => Render(font, text, (int)position.X, (int)position.Y, 1f, angle, color);

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="Begin"/>() method is not called before calling this method.
    /// </exception>
    /// <remarks>
    ///     If <paramref name="font"/> is null, nothing will be rendered.
    ///     <para>A null reference exception will not be thrown.</para>
    /// </remarks>
    public void Render(IFont font, string text, int x, int y, float renderSize, float angle, Color color)
    {
        if (font is null)
        {
            throw new ArgumentNullException(nameof(font), $"Cannot render a null '{nameof(IFont)}'.");
        }

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (font.Size == 0u)
        {
            return;
        }

        renderSize = renderSize < 0f ? 0f : renderSize;

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var normalizedSize = renderSize - 1f;
        var originalX = (float)x;
        var originalY = (float)y;
        var characterY = (float)y;

        text = text.TrimNewLineFromEnd();

        var lines = text.Split(Environment.NewLine).TrimAllEnds();

        var lineSpacing = font.LineSpacing.ApplySize(normalizedSize);
        var textSize = font.Measure(text).ApplySize(normalizedSize);

        var textHalfWidth = textSize.Width / 2f;

        var atlasWidth = font.FontTextureAtlas.Width.ApplySize(normalizedSize);
        var atlasHeight = font.FontTextureAtlas.Height.ApplySize(normalizedSize);

        var glyphLines = lines.Select(l =>
        {
            /* ⚙️ Perf Optimization️ ⚙️ */
            // No need to apply a size to waste compute time if the size is equal to 0
            return normalizedSize == 0f
                ? font.ToGlyphMetrics(l)
                : font.ToGlyphMetrics(l).Select(g => g.ApplySize(normalizedSize)).ToArray();
        }).ToList();

        var firstLineFirstCharBearingX = glyphLines[0][0].HoriBearingX;

        for (var i = 0; i < glyphLines.Count; i++)
        {
            if (i == 0)
            {
                var firstLineHeight = glyphLines.MaxHeight(i);
                var textTop = originalY + firstLineHeight;
                var textHalfHeight = textSize.Height / 2f;

                characterY = textTop - textHalfHeight;
            }
            else
            {
                characterY += lineSpacing;
            }

            var characterX = originalX - textHalfWidth + firstLineFirstCharBearingX;
            var textLinePos = new Vector2(characterX, characterY);

            // Convert all of the glyphs to batch items to be rendered
            var batchItems = ToFontBatchItems(
                textLinePos,
                glyphLines.ToArray()[i],
                font,
                new Vector2(x, y),
                normalizedSize,
                angle,
                color,
                atlasWidth,
                atlasHeight);

            foreach (var item in batchItems)
            {
                this.batchServiceManager.AddFontGlyphBatchItem(item);
            }
        }
    }

    /// <inheritdoc/>
    public void Render(RectShape rectangle) => this.batchServiceManager.AddRectBatchItem(rectangle);

    /// <inheritdoc/>
    public void End()
    {
        this.batchServiceManager.EndBatch(BatchServiceType.Texture);
        this.batchServiceManager.EndBatch(BatchServiceType.Rectangle);
        this.batchServiceManager.EndBatch(BatchServiceType.FontGlyph);

        this.hasBegun = false;
    }

    /// <summary>
    /// Shuts down the application by disposing of resources.
    /// </summary>
    private void ShutDown()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.batchServiceManager.TextureBatchReadyForRendering -= TextureBatchService_BatchReadyForRendering;
        this.batchServiceManager.FontGlyphBatchReadyForRendering -= FontGlyphBatchService_BatchReadyForRendering;
        this.batchServiceManager.RectBatchReadyForRendering -= RectBatchService_BatchReadyForRendering;
        this.batchServiceManager.Dispose();
        this.cachedUIntProps.Clear();

        this.isDisposed = true;
    }

    /// <summary>
    /// Initializes the renderer.
    /// </summary>
    private void Init()
    {
        this.gl.Enable(GLEnableCap.Blend);
        this.gl.BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

        this.isDisposed = false;
    }

    /// <summary>
    /// Invoked every time a batch of textures is ready to be rendered.
    /// </summary>
    private void TextureBatchService_BatchReadyForRendering(object? sender, EventArgs e)
    {
        if (this.batchServiceManager.TextureBatchItems.Count <= 0)
        {
            this.openGLService.BeginGroup("Render Texture Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Texture Process With {this.shaderManager.GetShaderName(ShaderType.Texture)} Shader");

        this.shaderManager.Use(ShaderType.Texture);

        var totalItemsToRender = 0u;
        var gpuDataIndex = -1;

        for (var i = 0u; i < this.batchServiceManager.TextureBatchItems.Count; i++)
        {
            if (this.batchServiceManager.TextureBatchItems[(int)i].IsEmpty())
            {
                // TODO: Because batch items are sorted by layers from lease to highest in the batching service,
                // If the batch item is empty, then the rest should be empty, we can break out of the loop.
                // Currently the batch items are only sorted by layer.  Look into sorting by layer AND if they are empty.
                // This way, all of the empty items are always after the non empty items.
                continue;
            }

            var batchItem = this.batchServiceManager.TextureBatchItems[(int)i];

            var isLastItem = i >= this.batchServiceManager.TextureBatchItems.Count - 1;
            var shouldRender = isLastItem ||
                               this.batchServiceManager.TextureBatchItems[(int)(i + 1)].TextureId != batchItem.TextureId;

            gpuDataIndex++;
            totalItemsToRender++;
            this.bufferManager.UploadTextureData(batchItem, (uint)gpuDataIndex);

            if (shouldRender)
            {
                this.openGLService.BindTexture2D(batchItem.TextureId);

                // Only render the amount of elements for the amount of batch items to render.
                // 6 = the number of vertices per quad and each batch item is a quad. totalItemsToRender is the total quads to render
                if (totalItemsToRender > 0)
                {
                    var totalElements = 6u * totalItemsToRender;

                    this.openGLService.BeginGroup($"Render {totalElements} Texture Elements");
                    this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
                    this.openGLService.EndGroup();

                    totalItemsToRender = 0;
                    gpuDataIndex = -1;
                }
            }

            this.openGLService.BeginGroup($"Update Texture Data - TextureID({batchItem.TextureId}) - BatchItem({i})");

            this.openGLService.EndGroup();
        }

        // Empty the batch
        this.batchServiceManager.EmptyBatch(BatchServiceType.Texture);

        this.openGLService.EndGroup();
    }

    /// <summary>
    /// Invoked every time a batch of fonts is ready to be rendered.
    /// </summary>
    private void FontGlyphBatchService_BatchReadyForRendering(object? sender, EventArgs e)
    {
        var fontTextureIsBound = false;

        if (this.batchServiceManager.FontGlyphBatchItems.Count <= 0)
        {
            this.openGLService.BeginGroup("Render Text Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Text Process With {this.shaderManager.GetShaderName(ShaderType.Font)} Shader");

        this.shaderManager.Use(ShaderType.Font);

        var totalItemsToRender = 0u;

        for (var i = 0u; i < this.batchServiceManager.FontGlyphBatchItems.Count; i++)
        {
            var batchItem = this.batchServiceManager.FontGlyphBatchItems[(int)i];

            if (batchItem.IsEmpty())
            {
                continue;
            }

            this.openGLService.BeginGroup($"Update Character Data - TextureID({batchItem.TextureId}) - BatchItem({i})");

            if (!fontTextureIsBound)
            {
                this.openGLService.BindTexture2D(batchItem.TextureId);
                fontTextureIsBound = true;
            }

            this.bufferManager.UploadFontGlyphData(batchItem, i);
            totalItemsToRender += 1;

            this.openGLService.EndGroup();
        }

        // Only render the amount of elements for the amount of batch items to render.
        // 6 = the number of vertices per quad and each batch is a quad. totalItemsToRender is the total quads to render
        if (totalItemsToRender > 0)
        {
            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Font Elements");
            this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
            this.openGLService.EndGroup();
        }

        // Empty the batch
        this.batchServiceManager.EmptyBatch(BatchServiceType.FontGlyph);

        this.openGLService.EndGroup();
    }

    /// <summary>
    /// Invoked every time a batch of rectangles is ready to be rendered.
    /// </summary>
    private void RectBatchService_BatchReadyForRendering(object? sender, EventArgs e)
    {
        if (this.batchServiceManager.RectBatchItems.Count <= 0)
        {
            this.openGLService.BeginGroup("Render Rectangle Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Rectangle Process With {this.shaderManager.GetShaderName(ShaderType.Rectangle)} Shader");

        this.shaderManager.Use(ShaderType.Rectangle);

        var totalItemsToRender = 0u;

        for (var i = 0u; i < this.batchServiceManager.RectBatchItems.Count; i++)
        {
            var batchItem = this.batchServiceManager.RectBatchItems[(int)i];

            if (batchItem.IsEmpty())
            {
                continue;
            }

            this.openGLService.BeginGroup($"Update Rectangle Data - BatchItem({i})");

            this.bufferManager.UploadRectData(batchItem, i);
            totalItemsToRender += 1;

            this.openGLService.EndGroup();
        }

        // Only render the amount of elements for the amount of batch items to render.
        // 6 = the number of vertices per quad and each batch is a quad. totalItemsToRender is the total number of  quads to render
        if (totalItemsToRender > 0)
        {
            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Rectangle Elements");
            this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
            this.openGLService.EndGroup();
        }

        // Empty the batch
        this.batchServiceManager.EmptyBatch(BatchServiceType.Rectangle);

        this.openGLService.EndGroup();
    }

    /// <summary>
    /// Constructs a list of batch items from the given
    /// <paramref name="charMetrics"/> to be rendered.
    /// </summary>
    /// <param name="textPos">The position to render the text.</param>
    /// <param name="charMetrics">The glyph metrics of the characters in the text.</param>
    /// <param name="font">The font being used.</param>
    /// <param name="origin">The origin to rotate the text around.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="atlasWidth">The width of the font texture atlas.</param>
    /// <param name="atlasHeight">The height of the font texture atlas.</param>
    /// <returns>The list of glyphs that make up the string as font batch items.</returns>
    private IEnumerable<FontGlyphBatchItem> ToFontBatchItems(
        Vector2 textPos,
        IEnumerable<GlyphMetrics> charMetrics,
        IFont font,
        Vector2 origin,
        float renderSize,
        float angle,
        Color color,
        float atlasWidth,
        float atlasHeight)
    {
        var result = new List<FontGlyphBatchItem>();

        var leftGlyphIndex = 0u;

        foreach (var currentCharMetric in charMetrics)
        {
            textPos.X += font.GetKerning(leftGlyphIndex, currentCharMetric.CharIndex);

            // Create the source rect
            var srcRect = currentCharMetric.GlyphBounds;
            srcRect.Width = srcRect.Width <= 0 ? 1 : srcRect.Width;
            srcRect.Height = srcRect.Height <= 0 ? 1 : srcRect.Height;

            // Calculate the height offset
            var heightOffset = currentCharMetric.GlyphHeight - currentCharMetric.HoriBearingY;

            // Adjust for characters that have a negative horizontal bearing Y
            // For example, the '_' character
            if (currentCharMetric.HoriBearingY < 0)
            {
                heightOffset += currentCharMetric.HoriBearingY;
            }

            // Create the destination rect
            RectangleF destRect = default;
            destRect.X = textPos.X;
            destRect.Y = textPos.Y + heightOffset;
            destRect.Width = atlasWidth;
            destRect.Height = atlasHeight;

            var newPosition = destRect.GetPosition().RotateAround(origin, angle);

            destRect.X = newPosition.X;
            destRect.Y = newPosition.Y;

            // Only render characters that are not a space (32 char code)
            if (currentCharMetric.Glyph != ' ')
            {
                var itemToAdd = default(FontGlyphBatchItem);

                itemToAdd.Glyph = currentCharMetric.Glyph;
                itemToAdd.SrcRect = srcRect;
                itemToAdd.DestRect = destRect;
                itemToAdd.Size = renderSize;
                itemToAdd.Angle = angle;
                itemToAdd.TintColor = color;
                itemToAdd.ViewPortSize = new SizeF(RenderSurfaceWidth, RenderSurfaceHeight);
                itemToAdd.Effects = RenderEffects.None;
                itemToAdd.TextureId = font.FontTextureAtlas.Id;

                result.Add(itemToAdd);
            }

            // Horizontally advance to the next glyph
            // Get the difference between the old glyph width
            // and the glyph width with the size applied
            textPos.X += currentCharMetric.HorizontalAdvance;

            leftGlyphIndex = currentCharMetric.CharIndex;
        }

        return result.ToArray();
    }

    /// <summary>
    /// Setup all of the caching for the properties that need caching.
    /// </summary>
    private void SetupPropertyCaches()
    {
        this.cachedUIntProps.Add(
            nameof(RenderSurfaceWidth),
            new CachedValue<uint>(
                0,
                () => (uint)this.openGLService.GetViewPortSize().Width,
                (value) =>
                {
                    var viewPortSize = this.openGLService.GetViewPortSize();

                    this.openGLService.SetViewPortSize(new Size((int)value, viewPortSize.Height));
                }));

        this.cachedUIntProps.Add(
            nameof(RenderSurfaceHeight),
            new CachedValue<uint>(
                0,
                () => (uint)this.openGLService.GetViewPortSize().Height,
                (value) =>
                {
                    var viewPortSize = this.openGLService.GetViewPortSize();

                    this.openGLService.SetViewPortSize(new Size(viewPortSize.Width, (int)value));
                }));

        this.cachedClearColor = new CachedValue<Color>(
            Color.CornflowerBlue,
            () =>
            {
                var colorValues = new float[4];
                this.gl.GetFloat(GLGetPName.ColorClearValue, colorValues);

                var red = colorValues[0].MapValue(0, 1, 0, 255);
                var green = colorValues[1].MapValue(0, 1, 0, 255);
                var blue = colorValues[2].MapValue(0, 1, 0, 255);
                var alpha = colorValues[3].MapValue(0, 1, 0, 255);

                return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
            },
            (value) =>
            {
                var red = value.R.MapValue(0f, 255f, 0f, 1f);
                var green = value.G.MapValue(0f, 255f, 0f, 1f);
                var blue = value.B.MapValue(0f, 255f, 0f, 1f);
                var alpha = value.A.MapValue(0f, 255f, 0f, 1f);

                this.gl.ClearColor(red, green, blue, alpha);
            });
    }
}
