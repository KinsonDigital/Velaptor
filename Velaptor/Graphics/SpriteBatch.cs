// <copyright file="SpriteBatch.cs" company="KinsonDigital">
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
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using NETRect = System.Drawing.Rectangle;
    using NETSizeF = System.Drawing.SizeF;
    using VelObservable = Velaptor.Observables.Core.IObservable<bool>;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    internal sealed class SpriteBatch : ISpriteBatch
    {
        private readonly Dictionary<string, CachedValue<uint>> cachedUIntProps = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IShaderProgram textureShader;
        private readonly IShaderProgram fontShader;
        private readonly IGPUBuffer<SpriteBatchItem> textureBuffer;
        private readonly IGPUBuffer<SpriteBatchItem> fontBuffer;
        private readonly IBatchManagerService<SpriteBatchItem> textureBatchService;
        private readonly IBatchManagerService<SpriteBatchItem> fontBatchService;
        private readonly IDisposable glInitUnsubscriber;
        private readonly IDisposable shutDownUnsubscriber;

        // ReSharper disable once MemberInitializerValueIgnored
        private CachedValue<Color> cachedClearColor = null!;
        private bool isDisposed;
        private bool hasBegun;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes OpenGL extensions methods.</param>
        /// <param name="textureShader">The shader used for rendering textures.</param>
        /// <param name="fontShader">The shader used for rendering text.</param>
        /// <param name="textureBuffer">Updates the data in the GPU related to rendering textures.</param>
        /// <param name="fontBuffer">Updates the data in the GPU related to rendering text.</param>
        /// <param name="textureBatchService">Manages the batch of textures to render textures.</param>
        /// <param name="fontBatchService">Manages the batch of textures to render text.</param>
        /// <param name="glInitObservable">Provides push notifications to OpenGL related events.</param>
        /// <param name="shutDownObservable">Sends out a notification that the application is shutting down.</param>
        /// <remarks>
        ///     <paramref name="glInitObservable"/> is subscribed to in this class.  <see cref="GLWindow"/>
        ///     pushes the notification that OpenGL has been initialized.
        /// </remarks>
        public SpriteBatch(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderProgram textureShader,
            IShaderProgram fontShader,
            IGPUBuffer<SpriteBatchItem> textureBuffer,
            IGPUBuffer<SpriteBatchItem> fontBuffer,
            IBatchManagerService<SpriteBatchItem> textureBatchService,
            IBatchManagerService<SpriteBatchItem> fontBatchService,
            VelObservable glInitObservable,
            VelObservable shutDownObservable)
        {
            this.gl = gl ?? throw new ArgumentNullException(nameof(gl), $"The parameter must not be null.");

            this.glExtensions = glExtensions ?? throw new ArgumentNullException(nameof(glExtensions), "The parameter must not be null.");
            this.textureShader = textureShader ?? throw new ArgumentNullException(nameof(textureShader), "The parameter must not be null.");
            this.fontShader = fontShader ?? throw new ArgumentNullException(nameof(fontShader), "The parameter must not be null.");
            this.textureBuffer = textureBuffer ?? throw new ArgumentNullException(nameof(textureBuffer), "The parameter must not be null.");
            this.fontBuffer = fontBuffer ?? throw new ArgumentNullException(nameof(fontBuffer), "The parameter must not be null.");

            this.textureBatchService = textureBatchService ?? throw new ArgumentNullException(nameof(textureBatchService), "The parameter must not be null.");
            this.textureBatchService.BatchSize = ISpriteBatch.BatchSize;
            this.textureBatchService.BatchFilled += TextureBatchService_BatchFilled;

            this.fontBatchService = fontBatchService ?? throw new ArgumentNullException(nameof(fontBatchService), "The parameter must not be null.");
            this.fontBatchService.BatchSize = ISpriteBatch.BatchSize;
            this.fontBatchService.BatchFilled += FontBatchService_BatchFilled;

            if (glInitObservable is null)
            {
                throw new ArgumentNullException(nameof(glInitObservable), "The parameter must not be null.");
            }

            // Receive a push notification that OpenGL has initialized
            this.glInitUnsubscriber = glInitObservable.Subscribe(new Observer<bool>(
                _ =>
                {
                    this.cachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

                    this.cachedClearColor.IsCaching = false;

                    Init();
                }));

            if (shutDownObservable is null)
            {
                throw new ArgumentNullException(nameof(shutDownObservable), "The parameter must not be null.");
            }

            this.shutDownUnsubscriber = shutDownObservable.Subscribe(new Observer<bool>(_ => ShutDown()));

            SetupPropertyCaches();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpriteBatch"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~SpriteBatch()
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
        public void BeginBatch() => this.hasBegun = true;

        /// <inheritdoc/>
        public void Clear() => this.gl.Clear(GLClearBufferMask.ColorBufferBit);

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
        ///     Thrown if the <see cref="BeginBatch"/>() method is not called before calling this method.
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
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");
            }

            if (texture.IsDisposed)
            {
                throw new InvalidOperationException($"Cannot render texture.  The texture '{texture.Name}' has been disposed.");
            }

            if (!this.hasBegun)
            {
                throw new InvalidOperationException($"The '{nameof(BeginBatch)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
            }

            if (srcRect.Width <= 0 || srcRect.Height <= 0)
            {
                throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(srcRect));
            }

            var itemToAdd = default(SpriteBatchItem);

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
        public void Render(IFont font, string text, int x, int y, float size, float angle)
            => Render(font, text, x, y, size, angle, Color.White);

        /// <inheritdoc/>
        public void Render(IFont font, string text, Vector2 position, float size, float angle)
            => Render(font, text, (int)position.X, (int)position.Y, size, angle, Color.White);

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
        ///     Thrown if the <see cref="BeginBatch"/>() method is not called before calling this method.
        /// </exception>
        public void Render(IFont font, string text, int x, int y, float size, float angle, Color color)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (font.IsDisposed)
            {
                throw new InvalidOperationException($"Cannot render font.  The font '{font.Name}' has been disposed.");
            }

            size = size < 0f ? 0f : size;

            if (!this.hasBegun)
            {
                throw new InvalidOperationException($"The '{nameof(BeginBatch)}()' method must be invoked first before any '{nameof(Render)}()' methods.");
            }

            var normalizedSize = size - 1f;
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
                // Not need to apply a size to waist compute time if the size is 0 which is no size change.
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
                    var lastLineVerticalOffset = glyphLines.MaxVerticalOffset(glyphLines.Count - 1);
                    var textHalfHeight = textSize.Height / 2f;

                    characterY = textTop - textHalfHeight - (lastLineVerticalOffset / 2f);
                }
                else
                {
                    characterY += lineSpacing;
                }

                var characterX = originalX - textHalfWidth + firstLineFirstCharBearingX;
                var textLinePos = new Vector2(characterX, characterY);

                // Convert all of the glyphs to sprite batch items to be rendered
                var batchItems = ToSpriteBatchItems(
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
        public void EndBatch()
        {
            TextureBatchService_BatchFilled(this.textureBatchService, EventArgs.Empty);
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
            this.cachedUIntProps.Clear();
            this.glInitUnsubscriber.Dispose();
            this.shutDownUnsubscriber.Dispose();

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the sprite batch.
        /// </summary>
        private void Init()
        {
            this.gl.Enable(GLEnableCap.Blend);
            this.gl.BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

            this.isDisposed = false;
        }

        /// <summary>
        /// Invoked every time the batch of textures is ready to be rendered.
        /// </summary>
        private void TextureBatchService_BatchFilled(object? sender, EventArgs e)
        {
            var textureIsBound = false;

            this.textureShader.Use();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.textureBatchService.BatchItems.Count; i++)
            {
                var (shouldRender, batchItem) = this.textureBatchService.BatchItems[i];

                if (shouldRender is false || batchItem.IsEmpty())
                {
                    continue;
                }

                if (!textureIsBound)
                {
                    this.gl.ActiveTexture(GLTextureUnit.Texture0);
                    this.gl.BindTexture(GLTextureTarget.Texture2D, batchItem.TextureId);
                    textureIsBound = true;
                }

                this.textureBuffer.UploadData(batchItem, i);
                totalItemsToRender += 1;
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices per quad and each batch is a quad. batchAmountToRender is the total quads to render
            if (totalItemsToRender > 0)
            {
                var totalElements = 6u * totalItemsToRender;

                this.glExtensions.BeginGroup($"Render {totalElements} Texture Elements");
                this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
                this.glExtensions.EndGroup();
            }

            // Empty the batch items
            this.textureBatchService.EmptyBatch();
        }

        /// <summary>
        /// Invoked every time the batch of fonts is ready to be rendered.
        /// </summary>
        private void FontBatchService_BatchFilled(object? sender, EventArgs e)
        {
            var fontTextureIsBound = false;

            this.glExtensions.BeginGroup($"Render Text Process With {this.fontShader.Name} Shader");

            this.fontShader.Use();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.fontBatchService.BatchItems.Count; i++)
            {
                var (shouldRender, batchItem) = this.fontBatchService.BatchItems[i];

                if (shouldRender is false || batchItem.IsEmpty())
                {
                    continue;
                }

                this.glExtensions.BeginGroup($"Update Character Data - TextureID({batchItem.TextureId}) - BatchItem({i})");

                if (!fontTextureIsBound)
                {
                    this.gl.ActiveTexture(GLTextureUnit.Texture1);
                    this.gl.BindTexture(GLTextureTarget.Texture2D, batchItem.TextureId);
                    fontTextureIsBound = true;
                }

                this.fontBuffer.UploadData(batchItem, i);

                totalItemsToRender += 1;

                this.glExtensions.EndGroup();
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices per quad and each batch is a quad. batchAmountToRender is the total quads to render
            if (totalItemsToRender > 0)
            {
                var totalElements = 6u * totalItemsToRender;

                this.glExtensions.BeginGroup($"Render {totalElements} Font Elements");
                this.gl.DrawElements(GLPrimitiveType.Triangles, totalElements, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
                this.glExtensions.EndGroup();
            }

            // Empty the batch items
            this.fontBatchService.EmptyBatch();

            this.glExtensions.EndGroup();
        }

        /// <summary>
        /// Constructs a list of sprite batch items from the given
        /// <paramref name="charMetrics"/> to be rendered.
        /// </summary>
        /// <param name="textPos">The position to render the text.</param>
        /// <param name="charMetrics">The glyph metrics of the characters in the text.</param>
        /// <param name="font">The font being used.</param>
        /// <param name="origin">The origin to rotate the text around.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="angle">The angle of the text.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="atlasWidth">The width of the font texture atlas.</param>
        /// <param name="atlasHeight">The height of the font texture atlas.</param>
        /// <returns>The list of glyphs that make up the string as sprite batch items.</returns>
        private IEnumerable<SpriteBatchItem> ToSpriteBatchItems(
            Vector2 textPos,
            IEnumerable<GlyphMetrics> charMetrics,
            IFont font,
            Vector2 origin,
            float size,
            float angle,
            Color color,
            float atlasWidth,
            float atlasHeight)
        {
            var result = new List<SpriteBatchItem>();

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
                    var itemToAdd = default(SpriteBatchItem);

                    itemToAdd.SrcRect = srcRect;
                    itemToAdd.DestRect = destRect;
                    itemToAdd.Size = size;
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
                    () => (uint)this.glExtensions.GetViewPortSize().Width,
                    (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size((int)value, viewPortSize.Height));
                    }));

            this.cachedUIntProps.Add(
                nameof(RenderSurfaceHeight),
                new CachedValue<uint>(
                    0,
                    () => (uint)this.glExtensions.GetViewPortSize().Height,
                    (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size(viewPortSize.Width, (int)value));
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
