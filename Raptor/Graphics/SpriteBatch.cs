// <copyright file="SpriteBatch.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using System.Xml;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;
    using Raptor.OpenGL;

    /// <summary>
    /// Renders a single or batch of textures.
    /// </summary>
    public class SpriteBatch : IDisposable
    {
        private readonly Dictionary<int, SpriteBatchItem> batchItems = new Dictionary<int, SpriteBatchItem>();
        private readonly IGLInvoker gl;
        private readonly int maxBatchSize = 48;
        private int renderSurfaceWidth;
        private int renderSurfaceHeight;
        private int transDataLocation;
        private GPUBuffer<VertexData>? gpuBuffer;
        private ShaderProgram? shader;
        private bool disposedValue = false;
        private bool hasBegun;
        private int currentBatchItem = 0;
        private int previousTextureID = -1;
        private bool firstRender;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// </summary>
        /// <param name="renderSurfaceWidth">The width of the render surface.</param>
        /// <param name="renderSurfaceHeight">The height of the render surface.</param>
        public SpriteBatch(int renderSurfaceWidth, int renderSurfaceHeight)
        {
            this.gl = new GLInvoker();
            Init(renderSurfaceWidth, renderSurfaceHeight);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatch"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="renderSurfaceWidth">The width of the render surface.</param>
        /// <param name="renderSurfaceHeight">The height of the render surface.</param>
        [ExcludeFromCodeCoverage]
        internal SpriteBatch(IGLInvoker gl, int renderSurfaceWidth, int renderSurfaceHeight)
        {
            this.gl = gl;
            Init(renderSurfaceWidth, renderSurfaceHeight);
        }

        /// <summary>
        /// Starts the batch rendering process.  Must be called before calling
        /// the <see cref="Render()"/> methods.
        /// </summary>
        public void Begin() => this.hasBegun = true;

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <exception cref="Exception">Thrown if the <see cref="Begin"/>() method has not been called.</exception>
        public void Render(ITexture texture, int x, int y) => Render(texture, x, y, Color.White);

        /// <summary>
        /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> location.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The X location of the texture.</param>
        /// <param name="y">The y location of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture.</param>
        /// <exception cref="Exception">Thrown if the <see cref="Begin"/>() method has not been called.</exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside of method.")]
        public void Render(ITexture texture, int x, int y, Color tintColor)
        {
            if (texture is null)
                throw new ArgumentNullException(nameof(texture), "The texture must not be null.");

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

        /// <summary>
        /// Renders the given <see cref="Texture"/> using the given parametters.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="srcRect">The rectangle of the sub texture within the texture to render.</param>
        /// <param name="destRect">The destination rectangle of rendering.</param>
        /// <param name="size">The size to render the texture at. 1 is for 100%/normal size.</param>
        /// <param name="angle">The angle of rotation in degrees of the rendering.</param>
        /// <param name="tintColor">The color to apply to the rendering.</param>
        /// <exception cref="Exception">Thrown if the <see cref="Begin"/>() method has not been called.</exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside method.")]
        public void Render(ITexture texture, Rectangle srcRect, Rectangle destRect, float size, float angle, Color tintColor)
        {
            if (!this.hasBegun)
                throw new Exception("Must call begin() first");

            bool HasSwitchedTexture() => texture.ID != this.previousTextureID && !this.firstRender;

            // var totalBatchItems = _batchItems.Count(i => !i.Value.IsEmpty);
            var totalBatchItems = this.batchItems.Values.ToArray().CountKD(i => !i.IsEmpty);

            // Has the textures switched
            if (HasSwitchedTexture() || totalBatchItems >= this.maxBatchSize)
            {
                RenderBatch();
                this.currentBatchItem = 0;
                this.previousTextureID = 0;
            }

            this.currentBatchItem = this.currentBatchItem >= this.maxBatchSize ? 0 : this.currentBatchItem;

            var batchItem = this.batchItems[this.currentBatchItem];
            batchItem.TextureID = texture.ID;
            batchItem.SrcRect = srcRect;
            batchItem.DestRect = destRect;
            batchItem.Size = size;
            batchItem.Angle = angle;
            batchItem.TintColor = tintColor;

            this.batchItems[this.currentBatchItem] = batchItem;

            this.currentBatchItem += 1;
            this.previousTextureID = texture.ID;
            this.firstRender = true;
        }

        /// <summary>
        /// Ends the sprite batch process.  Calling this will render any textures
        /// still in the batch.
        /// </summary>
        public void End()
        {
            if (this.batchItems.All(i => i.Value.IsEmpty))
                return;

            RenderBatch();
            this.currentBatchItem = 0;
            this.previousTextureID = 0;
            this.hasBegun = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
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
            if (this.disposedValue)
                return;

            if (disposing)
            {
                this.shader?.Dispose();
                this.gpuBuffer?.Dispose();
            }

            this.disposedValue = true;
        }

        /// <summary>
        /// Initializes the sprite batch.
        /// </summary>
        /// <param name="renderSurfaceWidth">The width of the render surface.</param>
        /// <param name="renderSurfaceHeight">The height of the render surface.</param>
        private void Init(int renderSurfaceWidth, int renderSurfaceHeight)
        {
            this.shader = new ShaderProgram(this.maxBatchSize, "shader.vert", "shader.frag");

            for (var i = 0; i < this.maxBatchSize; i++)
            {
                this.batchItems.Add(i, SpriteBatchItem.Empty);
            }

            this.renderSurfaceWidth = renderSurfaceWidth;
            this.renderSurfaceHeight = renderSurfaceHeight;

            this.gl.Enable(EnableCap.Blend);
            this.gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            this.gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f); // TODO: Allow changing of this

            this.gl.ActiveTexture(TextureUnit.Texture0);

            this.shader.UseProgram();

            this.gpuBuffer = new GPUBuffer<VertexData>(this.maxBatchSize);

            this.transDataLocation = this.gl.GetUniformLocation(this.shader.ProgramId, "uTransform");
        }

        /// <summary>
        /// Renders the current batch of textures.
        /// </summary>
        private void RenderBatch()
        {
            var batchAmountToRender = this.batchItems.Count(i => !i.Value.IsEmpty);

            for (var i = 0; i < this.batchItems.Values.Count; i++)
            {
                if (this.batchItems[i].IsEmpty)
                    continue;

                this.gl.BindTexture(TextureTarget.Texture2D, this.batchItems[i].TextureID);

                UpdateGPUTransform(
                    i,
                    this.batchItems[i].DestRect.X,
                    this.batchItems[i].DestRect.Y,
                    this.batchItems[i].SrcRect.Width,
                    this.batchItems[i].SrcRect.Height,
                    this.batchItems[i].Size,
                    this.batchItems[i].Angle);

                this.gpuBuffer?.UpdateQuad(
                    i,
                    this.batchItems[i].SrcRect,
                    this.batchItems[i].DestRect.Width,
                    this.batchItems[i].DestRect.Height,
                    this.batchItems[i].TintColor);

                batchAmountToRender += 1;
            }

            // Only render the amount of elements for the amount of batch items to render.
            // 6 = the number of vertices/quad and each batch is a quad. batchAmontToRender is the total quads to render
            if (batchAmountToRender > 0)
                this.gl.DrawElements(PrimitiveType.Triangles, 6 * batchAmountToRender, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Empty the batch items
            for (var i = 0; i < this.batchItems.Count; i++)
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
        private void UpdateGPUTransform(int quadID, float x, float y, int width, int height, float size, float angle)
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
        /// Builds a complete transformation matrix using the given params.
        /// </summary>
        /// <param name="x">The x position of a texture.</param>
        /// <param name="y">The y position of a texture.</param>
        /// <param name="width">The width of a texture.</param>
        /// <param name="height">The height of a texture.</param>
        /// <param name="size">The size of a texture. 1 represents normal size and 1.5 represents 150%.</param>
        /// <param name="angle">The angle of the texture.</param>
        private Matrix4 BuildTransformationMatrix(float x, float y, int width, int height, float size, float angle)
        {
            var scaleX = (float)width / this.renderSurfaceWidth;
            var scaleY = (float)height / this.renderSurfaceHeight;

            scaleX *= size;
            scaleY *= size;

            var ndcX = x.MapValue(0f, this.renderSurfaceWidth, -1f, 1f);
            var ndcY = y.MapValue(0f, this.renderSurfaceHeight, 1f, -1f);

            // NOTE: (+ degrees) rotates CCW and (- degress) rotates CW
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
