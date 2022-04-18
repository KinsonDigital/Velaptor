// <copyright file="Renderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor;
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

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    internal sealed class Renderer : IRenderer
    {
        private readonly Dictionary<string, CachedValue<uint>> cachedUIntProps = new ();
        private readonly IGLInvoker gl;
        private readonly IOpenGLService openGLService;
        private readonly IShaderProgram textureShader;
        private readonly IShaderProgram fontShader;
        private readonly IShaderProgram rectShader;
        private readonly IGPUBuffer<TextureBatchItem> textureBuffer;
        private readonly IGPUBuffer<FontGlyphBatchItem> fontBuffer;
        private readonly IGPUBuffer<RectShape> rectBuffer;
        private readonly IBatchingService<TextureBatchItem> textureBatchService;
        private readonly IBatchingService<FontGlyphBatchItem> fontBatchService;
        private readonly IBatchingService<RectShape> rectBatchService;
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
        /// <param name="textureShader">The shader used for rendering textures.</param>
        /// <param name="fontShader">The shader used for rendering text.</param>
        /// <param name="rectShader">The shader used for rendering rectangles.</param>
        /// <param name="textureBuffer">Updates the data in the GPU related to rendering textures.</param>
        /// <param name="fontBuffer">Updates the data in the GPU related to rendering text.</param>
        /// <param name="rectBuffer">Updates the data in the GPU related to rendering rectangles.</param>
        /// <param name="textureBatchingService">Manages the batch of textures to render textures.</param>
        /// <param name="fontBatchingService">Manages the batch of textures to render text.</param>
        /// <param name="rectBatchingService">Manages the batch of rectangles to render.</param>
        /// <param name="glInitReactable">Provides push notifications that OpenGL has been initialized.</param>
        /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
        /// <remarks>
        ///     <paramref name="glInitReactable"/> is subscribed to in this class.  <see cref="GLWindow"/>
        ///     pushes the notification that OpenGL has been initialized.
        /// </remarks>
        public Renderer(
            IGLInvoker gl,
            IOpenGLService openGLService,
            IShaderProgram textureShader,
            IShaderProgram fontShader,
            IShaderProgram rectShader,
            IGPUBuffer<TextureBatchItem> textureBuffer,
            IGPUBuffer<FontGlyphBatchItem> fontBuffer,
            IGPUBuffer<RectShape> rectBuffer,
            IBatchingService<TextureBatchItem> textureBatchingService,
            IBatchingService<FontGlyphBatchItem> fontBatchingService,
            IBatchingService<RectShape> rectBatchingService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
        {
            EnsureThat.ParamIsNotNull(gl);
            EnsureThat.ParamIsNotNull(openGLService);
            EnsureThat.ParamIsNotNull(textureShader);
            EnsureThat.ParamIsNotNull(fontShader);
            EnsureThat.ParamIsNotNull(rectShader);
            EnsureThat.ParamIsNotNull(textureBuffer);
            EnsureThat.ParamIsNotNull(fontBuffer);
            EnsureThat.ParamIsNotNull(rectBuffer);

            this.gl = gl;
            this.openGLService = openGLService;
            this.textureShader = textureShader;
            this.fontShader = fontShader;
            this.rectShader = rectShader;
            this.textureBuffer = textureBuffer;
            this.fontBuffer = fontBuffer;
            this.rectBuffer = rectBuffer;

            this.textureBatchService = textureBatchingService ?? throw new ArgumentNullException(nameof(textureBatchingService), "The parameter must not be null.");
            this.textureBatchService.BatchSize = IRenderer.BatchSize;
            this.textureBatchService.BatchFilled += TextureBatchService_BatchFilled;

            this.fontBatchService = fontBatchingService ?? throw new ArgumentNullException(nameof(fontBatchingService), "The parameter must not be null.");
            this.fontBatchService.BatchSize = IRenderer.BatchSize;
            this.fontBatchService.BatchFilled += FontBatchService_BatchFilled;

            this.rectBatchService = rectBatchingService ?? throw new ArgumentNullException(nameof(rectBatchingService), "The parameter must not be null.");
            this.rectBatchService.BatchSize = IRenderer.BatchSize;
            this.rectBatchService.BatchFilled += RectBatchService_BatchFilled;

            if (glInitReactable is null)
            {
                throw new ArgumentNullException(nameof(glInitReactable), "The parameter must not be null.");
            }

            // Receive a push notification that OpenGL has initialized
            this.glInitUnsubscriber = glInitReactable.Subscribe(new Reactor<GLInitData>(
                _ =>
                {
                    this.cachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

                    this.cachedClearColor.IsCaching = false;

                    Init();
                }));

            if (shutDownReactable is null)
            {
                throw new ArgumentNullException(nameof(shutDownReactable), "The parameter must not be null.");
            }

            this.shutDownUnsubscriber = shutDownReactable.Subscribe(new Reactor<ShutDownData>(_ => ShutDown()));

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
            this.textureBuffer.ViewPortSize = size;
            this.fontBuffer.ViewPortSize = size;
            this.rectBuffer.ViewPortSize = size;
        }

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y) => Render(texture, x, y, Color.White);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, RenderEffects effects) => Render(texture, x, y, Color.White, effects);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, Color color) => Render(texture, x, y, color, RenderEffects.None);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, Color color, RenderEffects effects)
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

            Render(texture, srcRect, destRect, 1, 0, color, effects);
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
            RenderEffects effects)
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

            this.textureBatchService.Add(itemToAdd);
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

            text = text.TrimEnd('\n');
            var lines = text.Split('\n').TrimAllEnds();

            var lineSpacing = font.LineSpacing.ApplySize(normalizedSize);
            var textSize = font.Measure(text).ApplySize(normalizedSize);

            var textHalfWidth = textSize.Width / 2f;

            var atlasWidth = font.FontTextureAtlas.Width.ApplySize(normalizedSize);
            var atlasHeight = font.FontTextureAtlas.Height.ApplySize(normalizedSize);

            var glyphLines = lines.Select(l =>
            {
                /* ⚙️ Perf Optimization️ ⚙️ */
                // No need to apply a size to waste compute time if the size
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

                this.fontBatchService.AddRange(batchItems);
            }
        }

        /// <inheritdoc/>
        public void Render(RectShape rectangle) => this.rectBatchService.Add(rectangle);

        /// <inheritdoc/>
        public void End()
        {
            TextureBatchService_BatchFilled(this.textureBatchService, EventArgs.Empty);
            RectBatchService_BatchFilled(this.rectBatchService, EventArgs.Empty);
            FontBatchService_BatchFilled(this.fontBatchService, EventArgs.Empty);

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

            this.textureBatchService.BatchFilled -= TextureBatchService_BatchFilled;
            this.fontBatchService.BatchFilled -= FontBatchService_BatchFilled;
            this.rectBatchService.BatchFilled -= RectBatchService_BatchFilled;
            this.cachedUIntProps.Clear();
            this.glInitUnsubscriber.Dispose();
            this.shutDownUnsubscriber.Dispose();

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
        private void TextureBatchService_BatchFilled(object? sender, EventArgs e)
        {
            var textureIsBound = false;

            if (this.textureBatchService.BatchItems.Count <= 0)
            {
                this.openGLService.BeginGroup("Render Texture Process - Nothing To Render");
                this.openGLService.EndGroup();

                return;
            }

            this.openGLService.BeginGroup($"Render Texture Process With {this.textureShader.Name} Shader");

            this.textureShader.Use();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.textureBatchService.BatchItems.Count; i++)
            {
                var (shouldRender, batchItem) = this.textureBatchService.BatchItems[i];

                if (shouldRender is false || batchItem.IsEmpty())
                {
                    continue;
                }

                this.openGLService.BeginGroup($"Update Texture Data - TextureID({batchItem.TextureId}) - BatchItem({i})");

                if (!textureIsBound)
                {
                    this.gl.ActiveTexture(GLTextureUnit.Texture0);
                    this.openGLService.BindTexture2D(batchItem.TextureId);
                    textureIsBound = true;
                }

                this.textureBuffer.UploadData(batchItem, i);
                totalItemsToRender += 1;

                this.openGLService.EndGroup();
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices per quad and each batch is a quad. batchAmountToRender is the total quads to render
            if (totalItemsToRender > 0)
            {
                var totalElements = 6u * totalItemsToRender;

                this.openGLService.BeginGroup($"Render {totalElements} Texture Elements");
                this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
                this.openGLService.EndGroup();
            }

            // Empty the batch
            this.textureBatchService.EmptyBatch();

            this.openGLService.EndGroup();
        }

        /// <summary>
        /// Invoked every time a batch of fonts is ready to be rendered.
        /// </summary>
        private void FontBatchService_BatchFilled(object? sender, EventArgs e)
        {
            var fontTextureIsBound = false;

            if (this.fontBatchService.BatchItems.Count <= 0)
            {
                this.openGLService.BeginGroup("Render Text Process - Nothing To Render");
                this.openGLService.EndGroup();

                return;
            }

            this.openGLService.BeginGroup($"Render Text Process With {this.fontShader.Name} Shader");

            this.fontShader.Use();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.fontBatchService.BatchItems.Count; i++)
            {
                var (shouldRender, batchItem) = this.fontBatchService.BatchItems[i];

                if (shouldRender is false || batchItem.IsEmpty())
                {
                    continue;
                }

                this.openGLService.BeginGroup($"Update Character Data - TextureID({batchItem.TextureId}) - BatchItem({i})");

                if (!fontTextureIsBound)
                {
                    this.openGLService.BindTexture2D(batchItem.TextureId);
                    fontTextureIsBound = true;
                }

                this.fontBuffer.UploadData(batchItem, i);

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
            this.fontBatchService.EmptyBatch();

            this.openGLService.EndGroup();
        }

        /// <summary>
        /// Invoked every time a batch of rectangles is ready to be rendered.
        /// </summary>
        private void RectBatchService_BatchFilled(object? sender, EventArgs e)
        {
            if (this.rectBatchService.BatchItems.Count <= 0)
            {
                this.openGLService.BeginGroup("Render Rectangle Process - Nothing To Render");
                this.openGLService.EndGroup();

                return;
            }

            this.openGLService.BeginGroup($"Render Rectangle Process With {this.rectShader.Name} Shader");

            this.rectShader.Use();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.rectBatchService.BatchItems.Count; i++)
            {
                var (shouldRender, batchItem) = this.rectBatchService.BatchItems[i];

                if (shouldRender is false || batchItem.IsEmpty())
                {
                    continue;
                }

                this.openGLService.BeginGroup($"Update Rectangle Data - BatchItem({i})");

                this.rectBuffer.UploadData(batchItem, i);
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
            this.rectBatchService.EmptyBatch();

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
}
