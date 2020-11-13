// <copyright file="SpriteBatch.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Raptor.OpenGL;

    /// <inheritdoc/>
    internal class SpriteBatch : ISpriteBatch
    {
        private readonly Dictionary<uint, SpriteBatchItem> batchItems = new Dictionary<uint, SpriteBatchItem>();
        private readonly IGLInvoker gl;
        private readonly IShaderProgram shader;
        private readonly IGPUBuffer gpuBuffer;
        private uint transDataLocation;
        private bool isDisposed;
        private bool hasBegun;
        private uint batchSize = 10;
        private uint currentBatchItem;
        private uint currentTextureID;
        private uint previousTextureID;
        private bool firstRenderMethodInvoke = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="gpuBuffer">The GPU buffer that holds the data for a batch of sprites.</param>
        [ExcludeFromCodeCoverage]
        public SpriteBatch(IGLInvoker gl, IShaderProgram shader, IGPUBuffer gpuBuffer)
        {
            if (gl is null)
            {
                throw new ArgumentNullException(nameof(gl), $"The '{nameof(IGLInvoker)}' must not be null.");
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
            this.shader = shader;
            this.gpuBuffer = gpuBuffer;
            Init();
        }

        /// <inheritdoc/>
        public uint BatchSize
        {
            get => this.batchSize;
            set
            {
                this.batchSize = value;
                Init();
            }
        }

        /// <inheritdoc/>
        public int RenderSurfaceWidth
        {
            get => (int)this.gl.GetViewPortSize().X;
            set
            {
                var data = new int[4];

                this.gl.GetInteger(GetPName.Viewport, data);

                this.gl.Viewport(data[0], data[1], value, data[3]);
            }
        }

        /// <inheritdoc/>
        public int RenderSurfaceHeight
        {
            get => (int)this.gl.GetViewPortSize().Y;
            set
            {
                var data = new int[4];

                this.gl.GetInteger(GetPName.Viewport, data);

                this.gl.Viewport(data[0], data[1], data[2], value);
            }
        }

        /// <inheritdoc/>
        public void BeginBatch() => this.hasBegun = true;

        /// <inheritdoc/>
        public void Clear() => this.gl.Clear(ClearBufferMask.ColorBufferBit);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y) => Render(texture, x, y, Color.White);

        /// <inheritdoc/>
        public void Render(ITexture texture, int x, int y, Color tintColor)
        {
            if (!this.hasBegun)
            {
                throw new Exception($"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
            }

            if (texture is null)
            {
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");
            }

            var srcRect = new Rectangle()
            {
                X = 0,
                Y = 0,
                Width = texture.Width,
                Height = texture.Height,
            };

            var destRect = new Rectangle(x, y, texture.Width, texture.Height);

            Render(texture, srcRect, destRect, 1, 0, tintColor);
        }

        /// <inheritdoc/>
        public void Render(ITexture texture, Rectangle srcRect, Rectangle destRect, float size, float angle, Color tintColor)
        {
            if (!this.hasBegun)
            {
                throw new Exception($"The '{nameof(SpriteBatch.BeginBatch)}()' method must be invoked first before the '{nameof(SpriteBatch.Render)}()' method.");
            }

            if (texture is null)
            {
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");
            }

            this.currentTextureID = texture.ID;

            var hasSwitchedTexture = this.currentTextureID != this.previousTextureID && !this.firstRenderMethodInvoke;
            var batchIsFull = this.batchItems.Values.ToArray().All(i => !i.IsEmpty);

            // Has the textures switched
            if (hasSwitchedTexture || batchIsFull)
            {
                RenderBatch();
                this.currentBatchItem = 0u;
                this.previousTextureID = 0u;
            }

            var batchItem = this.batchItems[this.currentBatchItem];
            batchItem.TextureID = texture.ID;
            batchItem.SrcRect = srcRect;
            batchItem.DestRect = destRect;
            batchItem.Size = size;
            batchItem.Angle = angle;
            batchItem.TintColor = tintColor;

            this.batchItems[this.currentBatchItem] = batchItem;

            this.currentBatchItem += 1;
            this.previousTextureID = this.currentTextureID;
            this.firstRenderMethodInvoke = false;
        }

        /// <inheritdoc/>
        public void EndBatch()
        {
            if (this.batchItems.All(i => i.Value.IsEmpty))
            {
                return;
            }

            RenderBatch();

            this.currentBatchItem = 0;
            this.previousTextureID = 0;
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
        /// <param name="disposing">True if managed resources should be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.batchItems.Clear();
                this.shader.Dispose();
                this.gpuBuffer.Dispose();
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the sprite batch.
        /// </summary>
        private void Init()
        {
            this.batchItems.Clear();
            for (uint i = 0; i < BatchSize; i++)
            {
                this.batchItems.Add(i, SpriteBatchItem.Empty);
            }

            this.gl.Enable(EnableCap.Blend);
            this.gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            this.gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f); // TODO: Allow changing of this

            this.gl.ActiveTexture(TextureUnit.Texture0);

            this.shader.UseProgram();

            this.transDataLocation = this.gl.GetUniformLocation(this.shader.ProgramId, "uTransform");
        }

        /// <summary>
        /// Renders the current batch of textures.
        /// </summary>
        private void RenderBatch()
        {
            var batchAmountToRender = (uint)this.batchItems.Count(i => !i.Value.IsEmpty);
            var textureIsBound = false;

            for (uint i = 0; i < this.batchItems.Values.Count; i++)
            {
                if (this.batchItems[i].IsEmpty)
                {
                    continue;
                }

                if (!textureIsBound)
                {
                    this.gl.BindTexture(TextureTarget.Texture2D, this.batchItems[i].TextureID);
                    textureIsBound = true;
                }

                UpdateGPUTransform(
                    i,
                    this.batchItems[i].DestRect.X,
                    this.batchItems[i].DestRect.Y,
                    this.batchItems[i].SrcRect.Width,
                    this.batchItems[i].SrcRect.Height,
                    this.batchItems[i].Size,
                    this.batchItems[i].Angle);

                this.gpuBuffer.UpdateQuad(
                    i,
                    this.batchItems[i].SrcRect,
                    this.batchItems[i].DestRect.Width,
                    this.batchItems[i].DestRect.Height,
                    this.batchItems[i].TintColor);
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices/quad and each batch is a quad. batchAmontToRender is the total quads to render
            if (batchAmountToRender > 0)
            {
                this.gl.DrawElements(PrimitiveType.Triangles, 6 * batchAmountToRender, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            // Empty the batch items
            for (uint i = 0; i < this.batchItems.Count; i++)
            {
                this.batchItems[i] = SpriteBatchItem.Empty;
            }
        }

        /// <summary>
        /// Updates the transform for the quad using the given data that matches the given <paramref name="quadID"/>.
        /// </summary>
        /// <param name="quadID">The ID of the quad to update.</param>
        /// <param name="x">The X location of the quad.</param>
        /// <param name="y">The Y location of the quad.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="height">The height of the quad.</param>
        /// <param name="size">The size of the quad. 1 represents normal size of 100%.</param>
        /// <param name="angle">The angle of the quad in degrees.</param>
        private void UpdateGPUTransform(uint quadID, float x, float y, int width, int height, float size, float angle)
        {
            // Create and send the transformation data to the GPU
            var transMatrix = BuildTransformationMatrix(
                x,
                y,
                width,
                height,
                size,
                angle);

            this.gl.UniformMatrix4(this.transDataLocation + quadID, true, ref transMatrix);
        }

        /// <summary>
        /// Builds a complete transformation matrix using the given parameters.
        /// </summary>
        /// <param name="x">The x position of a texture.</param>
        /// <param name="y">The y position of a texture.</param>
        /// <param name="width">The width of a texture.</param>
        /// <param name="height">The height of a texture.</param>
        /// <param name="size">The size of a texture. 1 represents normal size and 1.5 represents 150%.</param>
        /// <param name="angle">The angle of the texture.</param>
        private Matrix4 BuildTransformationMatrix(float x, float y, int width, int height, float size, float angle)
        {
            var viewPortSize = this.gl.GetViewPortSize();

            var scaleX = (float)width / viewPortSize.X;
            var scaleY = (float)height / viewPortSize.Y;

            scaleX *= size;
            scaleY *= size;

            var ndcX = x.MapValue(0f, viewPortSize.X, -1f, 1f);
            var ndcY = y.MapValue(0f, viewPortSize.Y, 1f, -1f);

            // NOTE: (+ degrees) rotates CCW and (- degrees) rotates CW
            var angleRadians = MathHelper.DegreesToRadians(angle);

            // Invert angle to rotate CW instead of CCW
            angleRadians *= -1;

            var rotation = Matrix4.CreateRotationZ(angleRadians);
            var scaleMatrix = Matrix4.CreateScale(scaleX, scaleY, 1f);
            var positionMatrix = Matrix4.CreateTranslation(new Vector3(ndcX, ndcY, 0));

            return rotation * scaleMatrix * positionMatrix;
        }
    }
}
