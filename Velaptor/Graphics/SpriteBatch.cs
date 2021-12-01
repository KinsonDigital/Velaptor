// <copyright file="SpriteBatch.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using FreeTypeSharp.Native;
    using Silk.NET.Maths;
    using Velaptor.Content;
    using Velaptor.Exceptions;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using NETRect = System.Drawing.Rectangle;
    using NETSizeF = System.Drawing.SizeF;

    /// <inheritdoc/>
    internal sealed class SpriteBatch : ISpriteBatch
    {
        private const char InvalidCharacter = '□';
        private readonly Dictionary<string, CachedValue<uint>> cachedUIntProps = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IShaderProgram shader;
        private readonly IGPUBuffer<SpriteBatchItem, NETSizeF> textureBuffer;
        private readonly IBatchManagerService<SpriteBatchItem> textureBatchService;
        private CachedValue<Color> cachedClearColor;
        private bool isDisposed;
        private bool hasBegun;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes OpenGL extensions methods.</param>
        /// <param name="freeTypeInvoker">Loads and manages fonts.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="textureBuffer">The GPU buffer that holds the data for a batch of sprites.</param>
        /// <param name="textureBatchService">Manages the batch of textures to render.</param>
        /// <param name="glObservable">Provides push notifications to OpenGL related events.</param>
        /// <remarks>
        ///     <paramref name="glObservable"/> is subscribed to in this class.  <see cref="GLWindow"/>
        ///     pushes the notification that OpenGL has been initialized.
        /// </remarks>
        [ExcludeFromCodeCoverage]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// The reason for ignoring this warning for the `cachedClearColor` not being set in constructor while
// it is set to not be null is due to the fact that we do not want warnings expressing an issue that
// does not exist.  The SetupPropertyCaches() method takes care of making sure it is not null.
        public SpriteBatch(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IFreeTypeInvoker freeTypeInvoker,
            IShaderProgram shader,
            IGPUBuffer<SpriteBatchItem, NETSizeF> textureBuffer,
            IBatchManagerService<SpriteBatchItem> textureBatchService,
            OpenGLInitObservable glObservable)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.gl = gl ?? throw new ArgumentNullException(nameof(gl), $"The '{nameof(IGLInvoker)}' must not be null.");
            this.glExtensions = glExtensions ?? throw new ArgumentNullException(nameof(glExtensions), $"The '{nameof(IGLInvokerExtensions)}' must not be null.");
            this.freeTypeInvoker = freeTypeInvoker;
            this.shader = shader ?? throw new ArgumentNullException(nameof(shader), $"The '{nameof(IShaderProgram)}' must not be null.");
            this.textureBuffer = textureBuffer ?? throw new ArgumentNullException(nameof(textureBuffer), $"The '{nameof(IGPUBuffer<SpriteBatchItem, NETSizeF>)}' must not be null.");

            this.shader.BatchSize = ISpriteBatch.BatchSize;
            this.textureBatchService = textureBatchService;
            this.textureBatchService.BatchSize = ISpriteBatch.BatchSize;
            this.textureBatchService.BatchFilled += TextureBatchService_BatchFilled;

            // Receive a push notification that OpenGL has initialized
            GLObservableUnsubscriber = glObservable.Subscribe(new Observer<bool>(
                _ =>
                {
                    this.cachedUIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

                    if (this.cachedClearColor is not null)
                    {
                        this.cachedClearColor.IsCaching = false;
                    }

                    Init();
                }));

            SetupPropertyCaches();
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

        /// <summary>
        /// Gets the unsubscriber for the subscription
        /// to the <see cref="OpenGLInitObservable"/>.
        /// </summary>
        private IDisposable GLObservableUnsubscriber { get; }

        /// <inheritdoc/>
        public void BeginBatch() => this.hasBegun = true;

        /// <inheritdoc/>
        public void Clear() => this.gl.Clear(GLClearBufferMask.ColorBufferBit);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y) => Render(texture, x, y, Color.White);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, RenderEffects effects) => Render(texture, x, y, Color.White, effects);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, Color tintColor) => Render(texture, x, y, tintColor, RenderEffects.None);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, Color tintColor, RenderEffects effects)
        {
            if (!this.hasBegun)
            {
                throw new Exception($"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
            }

            if (texture is null)
            {
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");
            }

            // Render the entire texture
            var srcRect = new NETRect()
            {
                X = 0,
                Y = 0,
                Width = (int)texture.Width,
                Height = (int)texture.Height,
            };

            var destRect = new NETRect(x, y, (int)texture.Width, (int)texture.Height);

            Render(texture, srcRect, destRect, 1, 0, tintColor, effects);
        }

        /// <inheritdoc/>
        public void Render(IFont font, string text, int x, int y) => Render(font, text, x, y, Color.White);

        /// <inheritdoc/>
        public void Render(IFont font, string text, int x, int y, Color tintColor)
        {
            var leftGlyghIndex = 0u;

            var facePtr = this.freeTypeInvoker.GetFace();

            var availableCharacters = font.GetAvailableGlyphCharacters();

            foreach (var character in text)
            {
                var charToRender = character;

                if (availableCharacters.Contains(character) is false)
                {
                    charToRender = InvalidCharacter;
                }

                var glyphMetrics = (from f in font.Metrics
                                    where f.Glyph == charToRender
                                    select f).FirstOrDefault();

                if (font.HasKerning && leftGlyghIndex != 0 && glyphMetrics.CharIndex != 0)
                {
                    // TODO: Check the perf for curiosity reasons
                    FT_Vector delta = this.freeTypeInvoker.FT_Get_Kerning(
                        facePtr,
                        leftGlyghIndex,
                        glyphMetrics.CharIndex,
                        (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

                    x += delta.x.ToInt32() >> 6;
                }

                NETRect srcRect = default;
                srcRect.X = glyphMetrics.AtlasBounds.X;
                srcRect.Y = glyphMetrics.AtlasBounds.Y;
                srcRect.Width = glyphMetrics.AtlasBounds.Width;
                srcRect.Height = glyphMetrics.AtlasBounds.Height;

                var verticalOffset = glyphMetrics.AtlasBounds.Height - glyphMetrics.HoriBearingY;

                NETRect destRect = default;
                destRect.X = x + (glyphMetrics.AtlasBounds.Width / 2);
                destRect.Y = y - (glyphMetrics.AtlasBounds.Height / 2) + verticalOffset;
                destRect.Width = (int)font.FontTextureAtlas.Width;
                destRect.Height = (int)font.FontTextureAtlas.Height;

                // Only render characters that are not a space (32 char code)
                if (character != ' ')
                {
                    Render(
                        font.FontTextureAtlas,
                        srcRect: srcRect,
                        destRect: destRect,
                        size: 1,
                        angle: 0,
                        tintColor: tintColor,
                        effects: RenderEffects.None);
                }

                // Horizontally advance the current glyph
                x += glyphMetrics.HorizontalAdvance;
                leftGlyghIndex = glyphMetrics.CharIndex;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidRenderEffectsException">
        ///     Thrown if the given <paramref name="effects"/> is invalid.
        /// </exception>
        public void Render(
            ITexture texture,
            NETRect srcRect,
            NETRect destRect,
            float size,
            float angle,
            Color tintColor,
            RenderEffects effects)
        {
            if (!this.hasBegun)
            {
                throw new Exception($"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
            }

            if (srcRect.Width <= 0 || srcRect.Height <= 0)
            {
                throw new ArgumentException("The source rectangle must have a width and height greater than zero.", nameof(srcRect));
            }

            if (texture is null)
            {
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");
            }

            var itemToAdd = default(SpriteBatchItem);

            itemToAdd.SrcRect = srcRect;
            itemToAdd.DestRect = destRect;
            itemToAdd.Size = size;
            itemToAdd.Angle = angle;
            itemToAdd.TintColor = tintColor;
            itemToAdd.Effects = effects;
            itemToAdd.TextureId = texture.Id;

            this.textureBatchService.Add(itemToAdd);
        }

        /// <inheritdoc/>
        public void EndBatch()
        {
            TextureBatchService_BatchFilled(this.textureBatchService, EventArgs.Empty);

            this.hasBegun = false;
        }

        public void OnResize(Vector2D<int> size)
        {
            var viewPortSize = new SizeF(size.X <= 0f ? 1f : size.X, size.Y <= 0f ? 1f : size.Y);

            RenderSurfaceWidth = (uint)viewPortSize.Width;
            RenderSurfaceHeight = (uint)viewPortSize.Height;

            this.textureBuffer.SetState(viewPortSize);
            // _lineBuffer.ViewPortSize = viewPortSize;
            // _rectBuffer.ViewPortSize = viewPortSize;
            // _ellipseBuffer.ViewPortSize = viewPortSize;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if managed resources should be disposed of.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureBatchService.BatchFilled -= TextureBatchService_BatchFilled;
                this.cachedUIntProps.Clear();
                this.shader.Dispose();
                this.textureBuffer.Dispose();
                GLObservableUnsubscriber.Dispose();
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Invoked every time the batch is ready to be rendered.
        /// </summary>
        private void TextureBatchService_BatchFilled(object? sender, EventArgs e)
        {
            var textureIsBound = false;

            this.shader.UseProgram();

            var totalItemsToRender = 0u;

            for (var i = 0u; i < this.textureBatchService.AllBatchItems.Count; i++)
            {
                (var shouldRender, SpriteBatchItem batchItem) = this.textureBatchService.AllBatchItems[i];

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

                this.textureBuffer.UpdateData(batchItem, i);
                totalItemsToRender += 1;
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices per quad and each batch is a quad. batchAmountToRender is the total quads to render
            if (totalItemsToRender > 0)
            {
                var formattedCallStack = string.Empty;
                foreach (var call in GLInvoker.GLCallStack)
                {
                    formattedCallStack += $"{call}|";
                }

                this.gl.DrawElements(GLPrimitiveType.Triangles, 6u * totalItemsToRender, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            // Empty the batch items
            this.textureBatchService.EmptyBatch();
        }

        /// <summary>
        /// Initializes the sprite batch.
        /// </summary>
        private void Init()
        {
            this.shader.Init();

            this.gl.Enable(GLEnableCap.Blend);
            this.gl.BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

            // TODO: Remove this and setup an OpenGLInitObservable in the ctor in the GPUBufferBase class
            this.textureBuffer.Init();
            this.shader.UseProgram();

            this.textureBuffer.SetState(new (RenderSurfaceWidth, RenderSurfaceHeight));

            this.isDisposed = false;
        }

        /// <summary>
        /// Setup all of the caching for the properties that need caching.
        /// </summary>
        private void SetupPropertyCaches()
        {
            this.cachedUIntProps.Add(
                nameof(RenderSurfaceWidth),
                new CachedValue<uint>(
                    defaultValue: 0,
                    getterWhenNotCaching: () => (uint)this.glExtensions.GetViewPortSize().Width,
                    setterWhenNotCaching: (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size((int)value, viewPortSize.Height));
                        this.textureBuffer.SetState(new SizeF(value, this.textureBuffer.GetState().Height));
                    }));

            this.cachedUIntProps.Add(
                nameof(RenderSurfaceHeight),
                new CachedValue<uint>(
                    defaultValue: 0,
                    getterWhenNotCaching: () => (uint)this.glExtensions.GetViewPortSize().Height,
                    setterWhenNotCaching: (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size(viewPortSize.Width, (int)value));
                        this.textureBuffer.SetState(new SizeF(this.textureBuffer.GetState().Width, value));
                    }));

            this.cachedClearColor = new CachedValue<Color>(
                defaultValue: Color.CornflowerBlue,
                getterWhenNotCaching: () =>
                {
                    var colorValues = new float[4];
                    this.gl.GetFloat(GLGetPName.ColorClearValue, colorValues);

                    var red = colorValues[0].MapValue(0, 1, 0, 255);
                    var green = colorValues[1].MapValue(0, 1, 0, 255);
                    var blue = colorValues[2].MapValue(0, 1, 0, 255);
                    var alpha = colorValues[3].MapValue(0, 1, 0, 255);

                    return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
                },
                setterWhenNotCaching: (value) =>
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
