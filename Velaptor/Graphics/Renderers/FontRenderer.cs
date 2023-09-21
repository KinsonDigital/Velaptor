// <copyright file="FontRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Batching;
using Carbonate.Fluent;
using Content.Fonts;
using ExtensionMethods;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Shaders;
using NETRect = System.Drawing.Rectangle;
using NETSizeF = System.Drawing.SizeF;

/// <inheritdoc cref="IFontRenderer"/>
internal sealed class FontRenderer : IFontRenderer
{
    private readonly IGLInvoker gl;
    private readonly IBatchingManager batchManager;
    private readonly IOpenGLService openGLService;
    private readonly IGpuBuffer<FontGlyphBatchItem> buffer;
    private readonly IShaderProgram shader;
    private bool hasBegun;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontRenderer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="buffer">Buffers data to the GPU.</param>
    /// <param name="shader">A shader program in the GPU.</param>
    /// <param name="batchManager">Batches items for rendering.</param>
    public FontRenderer(
        IGLInvoker gl,
        IReactableFactory reactableFactory,
        IOpenGLService openGLService,
        IGpuBuffer<FontGlyphBatchItem> buffer,
        IShaderProgram shader,
        IBatchingManager batchManager)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(buffer);
        EnsureThat.ParamIsNotNull(shader);
        EnsureThat.ParamIsNotNull(batchManager);

        this.gl = gl;
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

        var renderReactable = reactableFactory.CreateRenderFontReactable();

        var renderSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.RenderFontsId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.RenderFontsId)))
            .BuildOneWayReceive<Memory<RenderItem<FontGlyphBatchItem>>>(RenderBatch);

        renderReactable.Subscribe(renderSubscription);
    }

        /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, int layer = 0)
        => RenderBase(font, text, x, y, 1f, 0f, Color.White, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, int layer = 0)
        => RenderBase(font, text, (int)position.X, (int)position.Y, 1f, 0f, Color.White, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, float renderSize, float angle, int layer = 0)
        => RenderBase(font, text, x, y, renderSize, angle, Color.White, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, float renderSize, float angle, int layer = 0)
        => RenderBase(font, text, (int)position.X, (int)position.Y, renderSize, angle, Color.White, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, Color color, int layer = 0)
        => RenderBase(font, text, x, y, 1f, 0f, color, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, Color color, int layer = 0)
        => RenderBase(font, text, (int)position.X, (int)position.Y, 1f, 0f, color, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, float angle, Color color, int layer = 0)
        => RenderBase(font, text, x, y, 1f, angle, color, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, Vector2 position, float angle, Color color, int layer = 0)
        => RenderBase(font, text, (int)position.X, (int)position.Y, 1f, angle, color, layer);

    /// <inheritdoc/>
    public void Render(IFont font, string text, int x, int y, float renderSize, float angle, Color color, int layer = 0)
        => RenderBase(font, text, x, y, renderSize, angle, color, layer);

    /// <inheritdoc/>
    public void Render(IFont font, Span<(GlyphMetrics metrics, Color clr)> glyphs, int x, int y, float renderSize, float angle, int layer = 0)
    {
        if (font is null)
        {
            throw new ArgumentNullException(nameof(font), $"Cannot render a null '{nameof(IFont)}'.");
        }

        if (font.Size == 0u)
        {
            return;
        }

        renderSize = renderSize < 0f ? 0f : renderSize;

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var normalizedSize = renderSize - 1f;
        var originalX = (float)x;
        var originalY = (float)y;

        if (glyphs.Length <= 0)
        {
            return;
        }

        var glyphItems = glyphs.ToArray();
        var text = string.Join(string.Empty, glyphItems.Select(g => g.metrics.Glyph));

        var textSize = font.Measure(text).ApplySize(normalizedSize);
        var textHalfWidth = textSize.Width / 2f;

        var atlasWidth = font.Atlas.Width.ApplySize(normalizedSize);
        var atlasHeight = font.Atlas.Height.ApplySize(normalizedSize);

        var firstLineFirstCharBearingX = glyphs[0].metrics.HoriBearingX;
        var firstLineHeight = glyphItems.Max(g => g.metrics.GlyphHeight);
        var textTop = originalY + firstLineHeight;
        var textHalfHeight = textSize.Height / 2f;

        var characterY = textTop - textHalfHeight;

        var characterX = originalX - textHalfWidth + firstLineFirstCharBearingX;
        var textLinePos = new Vector2(characterX, characterY);

        // Convert all of the glyphs to batch items to be rendered
        var batchItems = ToFontBatchItems(
            textLinePos,
            glyphItems,
            font,
            new Vector2(x, y),
            normalizedSize,
            angle,
            atlasWidth,
            atlasHeight);

        var renderStamp = DateTime.Now;

        foreach (var item in batchItems)
        {
            this.batchManager.AddFontItem(item, layer, renderStamp);
        }
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
    private static IEnumerable<FontGlyphBatchItem> ToFontBatchItems(
        Vector2 textPos,
        GlyphMetrics[] charMetrics,
        IFont font,
        Vector2 origin,
        float renderSize,
        float angle,
        Color color,
        float atlasWidth,
        float atlasHeight)
    {
        var metricsWithClr = new (GlyphMetrics, Color)[charMetrics.Length];

        for (var i = 0; i < charMetrics.Length; i++)
        {
            metricsWithClr[i] = (charMetrics[i], color);
        }

        return ToFontBatchItems(
            textPos,
            metricsWithClr.ToArray(),
            font,
            origin,
            renderSize,
            angle,
            atlasWidth,
            atlasHeight);
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
    /// <param name="atlasWidth">The width of the font texture atlas.</param>
    /// <param name="atlasHeight">The height of the font texture atlas.</param>
    /// <returns>The list of glyphs that make up the string as font batch items.</returns>
    private static IEnumerable<FontGlyphBatchItem> ToFontBatchItems(
        Vector2 textPos,
        (GlyphMetrics metric, Color clr)[] charMetrics,
        IFont font,
        Vector2 origin,
        float renderSize,
        float angle,
        float atlasWidth,
        float atlasHeight)
    {
        var result = new List<FontGlyphBatchItem>();

        var leftGlyphIndex = 0u;

        foreach (var (charMetric, clr) in charMetrics)
        {
            textPos.X += font.GetKerning(leftGlyphIndex, charMetric.CharIndex);

            // Creates the source rect
            var srcRect = charMetric.GlyphBounds;
            srcRect.Width = srcRect.Width <= 0 ? 0 : srcRect.Width;
            srcRect.Height = srcRect.Height <= 0 ? 0 : srcRect.Height;

            // Calculate the height offset
            var heightOffset = charMetric.GlyphHeight - charMetric.HoriBearingY;

            // Adjust for characters that have a negative horizontal bearing Y
            // For example, the '_' character
            if (charMetric.HoriBearingY < 0)
            {
                heightOffset += charMetric.HoriBearingY;
            }

            // Create the destination rect
            RectangleF destRect = default;
            destRect.X = textPos.X;
            destRect.Y = textPos.Y + heightOffset;
            destRect.Width = atlasWidth;
            destRect.Height = atlasHeight;

            // Translate the position for rotated text. This also calculates the position
            // of the current glyph relative to the origin which is the center of the text
            var newPosition = destRect.GetPosition().RotateAround(origin, angle);
            destRect.X = newPosition.X;
            destRect.Y = newPosition.Y;

            // Only render characters that are not a space (32 char code)
            if (charMetric.Glyph != ' ')
            {
                var itemToAdd = new FontGlyphBatchItem(
                    srcRect,
                    destRect,
                    charMetric.Glyph,
                    renderSize,
                    angle,
                    clr,
                    RenderEffects.None,
                    font.Atlas.Id);

                result.Add(itemToAdd);
            }

            // Horizontally advance to the next glyph
            // Get the difference between the old glyph width
            // and the glyph width with the size applied
            textPos.X += charMetric.HorizontalAdvance;

            leftGlyphIndex = charMetric.CharIndex;
        }

        return result.ToArray();
    }

    /// <summary>
    /// The main root method for rendering text.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="color">The color to apply to the rendering.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <remarks>This takes new line characters into account to render multiple lines of text.</remarks>
    /// <exception cref="ArgumentNullException">Thrown if the font object is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="IBatcher.Begin"/> method has not been called before rendering.
    /// </exception>
    private void RenderBase(IFont font, string text, int x, int y, float renderSize, float angle, Color color, int layer = 0)
    {
        if (font is null)
        {
            throw new ArgumentNullException(nameof(font), $"Cannot render a null '{nameof(IFont)}'.");
        }

        if (font.Size == 0u)
        {
            return;
        }

        renderSize = renderSize < 0f ? 0f : renderSize;

        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
        }

        var normalizedSize = renderSize - 1f;
        var originalX = (float)x;
        var originalY = (float)y;
        var characterY = (float)y;

        text = text.TrimNewLineFromEnd();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        // Normalize the line endings
        if (text.Contains("\r\n"))
        {
            text = text.Replace("\r\n", "\n");
        }

        var lines = text.Split("\n");

        var lineSpacing = font.LineSpacing.ApplySize(normalizedSize);
        var textSize = font.Measure(text).ApplySize(normalizedSize);

        var textHalfWidth = textSize.Width / 2f;
        var textHalfHeight = textSize.Height / 2f;

        var atlasWidth = font.Atlas.Width.ApplySize(normalizedSize);
        var atlasHeight = font.Atlas.Height.ApplySize(normalizedSize);

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

            var renderStamp = DateTime.Now;

            foreach (var item in batchItems)
            {
                this.batchManager.AddFontItem(item, layer, renderStamp);
            }
        }
    }

    /// <summary>
    /// Invoked every time a batch of fonts is ready to be rendered.
    /// </summary>
    private void RenderBatch(Memory<RenderItem<FontGlyphBatchItem>> itemsToRender)
    {
        if (itemsToRender.Length <= 0)
        {
            this.openGLService.BeginGroup("Render Text Process - Nothing To Render");
            this.openGLService.EndGroup();

            return;
        }

        this.openGLService.BeginGroup($"Render Text Process With {this.shader.Name} Shader");

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

            this.openGLService.BeginGroup($"Update Character Data - TextureID({batchItem.TextureId}) - BatchItem({i})");
            this.buffer.UploadData(batchItem, (uint)gpuDataIndex);
            this.openGLService.EndGroup();

            if (shouldNotRender)
            {
                continue;
            }

            this.openGLService.BindTexture2D(batchItem.TextureId);

            var totalElements = 6u * totalItemsToRender;

            this.openGLService.BeginGroup($"Render {totalElements} Font Elements");
            this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, nint.Zero);
            this.openGLService.EndGroup();

            totalItemsToRender = 0;
            gpuDataIndex = -1;
        }

        this.openGLService.EndGroup();
        this.hasBegun = false;
    }
}
