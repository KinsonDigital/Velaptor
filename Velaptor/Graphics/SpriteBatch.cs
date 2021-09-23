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
    using System.Numerics;
    using FreeTypeSharp.Native;
    using Velaptor.Exceptions;
    using Velaptor.NativeInterop;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL;
    using Velaptor.Services;

    /// <inheritdoc/>
    internal class SpriteBatch : ISpriteBatch
    {
        private const char InvalidCharacter = '□';
        private readonly Dictionary<uint, SpriteBatchItem> batchItems = new ();
        private readonly Dictionary<string, CachedValue<int>> cachedIntProps = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IShaderProgram shader;
        private readonly IGPUBuffer gpuBuffer;
        private readonly IBatchManagerService batchManagerService;
        private CachedValue<Color> cachedClearColor;
        private int transDataLocation;
        private bool isDisposed;
        private bool hasBegun;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes OpenGL extentions methods.</param>
        /// <param name="freeTypeInvoker">Loads and manages fonts.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="gpuBuffer">The GPU buffer that holds the data for a batch of sprites.</param>
        /// <param name="batchManagerService">Manages the batch of textures to render.</param>
        /// <param name="glObservable">Provides push notifications to OpenGL related events.</param>
        /// <remarks>
        ///     <paramref name="glObservable"/> is subscribed to in this class.  <see cref="GLWindow"/>
        ///     pushes the notification that OpenGL has been intialized.
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
            IGPUBuffer gpuBuffer,
            IBatchManagerService batchManagerService,
            OpenGLInitObservable glObservable)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (gl is null)
            {
                throw new ArgumentNullException(nameof(gl), $"The '{nameof(IGLInvoker)}' must not be null.");
            }

            if (glExtensions is null)
            {
                throw new ArgumentNullException(nameof(glExtensions), $"The '{nameof(IGLInvokerExtensions)}' must not be null.");
            }

            if (shader is null)
            {
                throw new ArgumentNullException(nameof(shader), $"The '{nameof(IShaderProgram)}' must not be null.");
            }

            if (gpuBuffer is null)
            {
                throw new ArgumentNullException(nameof(gpuBuffer), $"The '{nameof(IGPUBuffer)}' must not be null.");
            }

            this.gl = gl;
            this.glExtensions = glExtensions;
            this.freeTypeInvoker = freeTypeInvoker;
            this.shader = shader;
            this.gpuBuffer = gpuBuffer;
            this.batchManagerService = batchManagerService;

            this.batchManagerService.BatchReady += BatchManagerService_BatchReady;

            // Receive a push notification that OpenGL has initialized
            GLObservableUnsubscriber = glObservable.Subscribe(new Observer<bool>(
                onNext: (isInitialized) =>
                {
                    this.cachedIntProps.Values.ToList().ForEach(i => i.IsCaching = false);

                    if (this.cachedClearColor is not null)
                    {
                        this.cachedClearColor.IsCaching = false;
                    }

                    Init();
                }));

            SetupPropertyCaches();
        }

        /// <inheritdoc/>
        public int RenderSurfaceWidth
        {
            get => this.cachedIntProps[nameof(RenderSurfaceWidth)].GetValue();
            set => this.cachedIntProps[nameof(RenderSurfaceWidth)].SetValue(value);
        }

        /// <inheritdoc/>
        public int RenderSurfaceHeight
        {
            get => this.cachedIntProps[nameof(RenderSurfaceHeight)].GetValue();
            set => this.cachedIntProps[nameof(RenderSurfaceHeight)].SetValue(value);
        }

        /// <inheritdoc/>
        public Color ClearColor
        {
            get => this.cachedClearColor is null ? Color.Empty : this.cachedClearColor.GetValue();
            set => this.cachedClearColor.SetValue(value);
        }

        /// <inheritdoc/>
        public uint BatchSize
        {
            get => this.batchManagerService.BatchSize;
            set => this.batchManagerService.BatchSize = value;
        }

        /// <summary>
        /// Gets the unsubscriber for the subcription
        /// to the <see cref="OpenGLInitObservable"/>.
        /// </summary>
        internal IDisposable GLObservableUnsubscriber { get; private set; }

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
            var srcRect = new Rectangle()
            {
                X = 0,
                Y = 0,
                Width = texture.Width,
                Height = texture.Height,
            };

            var destRect = new Rectangle(x, y, texture.Width, texture.Height);

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
                    // TODO: Check the perf for curiousity reasons
                    FT_Vector delta = this.freeTypeInvoker.FT_Get_Kerning(
                        facePtr,
                        leftGlyghIndex,
                        glyphMetrics.CharIndex,
                        (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

                    x += delta.x.ToInt32() >> 6;
                }

                Rectangle srcRect = default;
                srcRect.X = glyphMetrics.AtlasBounds.X;
                srcRect.Y = glyphMetrics.AtlasBounds.Y;
                srcRect.Width = glyphMetrics.AtlasBounds.Width;
                srcRect.Height = glyphMetrics.AtlasBounds.Height;

                var verticalOffset = glyphMetrics.AtlasBounds.Height - glyphMetrics.HoriBearingY;

                Rectangle destRect = default;
                destRect.X = x + (glyphMetrics.AtlasBounds.Width / 2);
                destRect.Y = y - (glyphMetrics.AtlasBounds.Height / 2) + verticalOffset;
                destRect.Width = font.FontTextureAtlas.Width;
                destRect.Height = font.FontTextureAtlas.Height;

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
            Rectangle srcRect,
            Rectangle destRect,
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

            this.batchManagerService.UpdateBatch(
                texture,
                srcRect,
                destRect,
                size,
                angle,
                tintColor,
                effects);
        }

        /// <inheritdoc/>
        public void EndBatch()
        {
            if (this.batchManagerService.EntireBatchEmpty)
            {
                return;
            }

            RenderBatch();

            this.hasBegun = false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if managed resources should be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.batchManagerService.BatchReady -= BatchManagerService_BatchReady;
                this.batchItems.Clear();
                this.cachedIntProps.Clear();
                this.shader.Dispose();
                this.gpuBuffer.Dispose();
                GLObservableUnsubscriber.Dispose();
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Invoked every time the batch is ready to be rendered.
        /// </summary>
        private void BatchManagerService_BatchReady(object? sender, EventArgs e) => RenderBatch();

        /// <summary>
        /// Initializes the sprite batch.
        /// </summary>
        private void Init()
        {
            this.shader.Init();
            this.gpuBuffer.TotalQuads = BatchSize;
            this.gpuBuffer.Init();

            this.gl.Enable(GLEnableCap.Blend);
            this.gl.BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

            this.gl.ActiveTexture(GLTextureUnit.Texture0);

            this.shader.UseProgram();

            this.transDataLocation = this.gl.GetUniformLocation(this.shader.ProgramId, "uTransform");
            this.isDisposed = false;
        }

        /// <summary>
        /// Setup all of the caching for the properties that need caching.
        /// </summary>
        private void SetupPropertyCaches()
        {
            // ReSharper disable ArgumentsStyleLiteral
            // ReSharper disable ArgumentsStyleNamedExpression
            // ReSharper disable ArgumentsStyleAnonymousFunction
            this.cachedIntProps.Add(
                nameof(RenderSurfaceWidth),
                new CachedValue<int>(
                    defaultValue: 0,
                    getterWhenNotCaching: () => this.glExtensions.GetViewPortSize().Width,
                    setterWhenNotCaching: (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size(value, viewPortSize.Height));
                    }));

            this.cachedIntProps.Add(
                nameof(RenderSurfaceHeight),
                new CachedValue<int>(
                    defaultValue: 0,
                    getterWhenNotCaching: () => this.glExtensions.GetViewPortSize().Height,
                    setterWhenNotCaching: (value) =>
                    {
                        var viewPortSize = this.glExtensions.GetViewPortSize();

                        this.glExtensions.SetViewPortSize(new Size(viewPortSize.Width, value));
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

            // ReSharper restore ArgumentsStyleLiteral
            // ReSharper restore ArgumentsStyleNamedExpression
            // ReSharper restore ArgumentsStyleAnonymousFunction
        }

        /// <summary>
        /// Renders the current batch of textures.
        /// </summary>
        private void RenderBatch()
        {
            var batchAmountToRender = this.batchManagerService.TotalItemsToRender;
            var textureIsBound = false;

            for (var i = 0; i < this.batchManagerService.BatchItems.Values.Count; i++)
            {
                var quadID = i;
                var batchItem = this.batchManagerService.BatchItems[(uint)quadID];

                if (batchItem.IsEmpty)
                {
                    continue;
                }

                if (!textureIsBound)
                {
                    // TODO: Verify that this is being invoked with proper values with unit tests
                    this.gl.BindTexture(GLTextureTarget.Texture2D, batchItem.TextureID);
                    textureIsBound = true;
                }

                var srcRectWidth = batchItem.SrcRect.Width;
                var srcRectHeight = batchItem.SrcRect.Height;

                // Set the source rectangle width and height based on the render effects
                srcRectWidth = batchItem.Effects switch
                {
                    RenderEffects.None => batchItem.SrcRect.Width,
                    RenderEffects.FlipHorizontally => batchItem.SrcRect.Width * -1,
                    RenderEffects.FlipVertically => batchItem.SrcRect.Width,
                    RenderEffects.FlipBothDirections => batchItem.SrcRect.Width * -1,
                    _ => throw new InvalidRenderEffectsException($"The '{nameof(RenderEffects)}' value of '{(int)batchItem.Effects}' is not valid."),
                };

                srcRectHeight = batchItem.Effects switch
                {
                    RenderEffects.None => batchItem.SrcRect.Height,
                    RenderEffects.FlipHorizontally => batchItem.SrcRect.Height,
                    RenderEffects.FlipVertically => batchItem.SrcRect.Height * -1,
                    RenderEffects.FlipBothDirections => batchItem.SrcRect.Height * -1,
                    _ => throw new InvalidRenderEffectsException($"The '{nameof(RenderEffects)}' value of '{(int)batchItem.Effects}' is not valid."),
                };

                var viewPortSize = this.glExtensions.GetViewPortSize();
                var transMatrix = this.batchManagerService.BuildTransformationMatrix(
                    new Vector2(viewPortSize.Width, viewPortSize.Height),
                    batchItem.DestRect.X,
                    batchItem.DestRect.Y,
                    srcRectWidth,
                    srcRectHeight,
                    batchItem.Size,
                    batchItem.Angle);

                this.gl.UniformMatrix4(this.transDataLocation + quadID, 1u, true, transMatrix);

                this.gpuBuffer.UpdateQuad(
                    (uint)quadID,
                    batchItem.SrcRect,
                    batchItem.DestRect.Width,
                    batchItem.DestRect.Height,
                    batchItem.TintColor);
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices per quad and each batch is a quad. batchAmountToRender is the total quads to render
            if (batchAmountToRender > 0)
            {
                this.gl.DrawElements(GLPrimitiveType.Triangles, 6 * batchAmountToRender, GLDrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            // Empty the batch items
            this.batchManagerService.EmptyBatch();
        }
    }
}
