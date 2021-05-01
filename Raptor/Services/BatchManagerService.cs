// <copyright file="BatchManagerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using OpenTK.Mathematics;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using SysVector2 = System.Numerics.Vector2;

    /// <summary>
    /// Manages the process of batching textures together when rendering them.
    /// </summary>
    internal class BatchManagerService : IBatchManagerService
    {
        private readonly Dictionary<uint, SpriteBatchItem> batchItems = new ();
        private bool firstRenderMethodInvoke = true;
        private uint currentTextureID;
        private uint previousTextureID;
        private uint currentBatchItem;

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? BatchReady;

        /// <inheritdoc/>
        public uint BatchSize { get; set; } = 10;

        /// <inheritdoc/>
        public ReadOnlyDictionary<uint, SpriteBatchItem> BatchItems
            => new (this.batchItems);

        /// <inheritdoc/>
        public uint TotalItemsToRender => (uint)this.batchItems.Count(i => !i.Value.IsEmpty);

        /// <inheritdoc/>
        public bool EntireBatchEmpty => this.batchItems.All(i => i.Value.IsEmpty);

        /// <inheritdoc/>
        public void UpdateBatch(
            ITexture texture,
            Rectangle srcRect,
            Rectangle destRect,
            float size,
            float angle,
            Color tintColor,
            RenderEffects effects)
        {
            if (texture is null)
            {
                throw new ArgumentNullException(nameof(texture), "The parameter must not be null.");
            }

            // Check that the batch size matches the amount of items.  If not, reinitialize the batch items
            if (this.batchItems.Count != BatchSize)
            {
                this.batchItems.Clear();

                for (uint i = 0; i < BatchSize; i++)
                {
                    this.batchItems.Add(i, SpriteBatchItem.Empty);
                }
            }

            this.currentTextureID = texture.ID;

            var hasSwitchedTexture = this.currentTextureID != this.previousTextureID
                && this.firstRenderMethodInvoke is false;
            var batchIsFull = this.batchItems.Values.ToArray().All(i => !i.IsEmpty);

            // Has the textures switched
            if (hasSwitchedTexture || batchIsFull)
            {
                BatchReady?.Invoke(this, EventArgs.Empty);
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
            batchItem.Effects = effects;

            this.batchItems[this.currentBatchItem] = batchItem;

            this.currentBatchItem += 1;
            this.previousTextureID = this.currentTextureID;
            this.firstRenderMethodInvoke = false;
        }

        /// <inheritdoc/>
        public void EmptyBatch()
        {
            for (uint i = 0; i < this.batchItems.Count; i++)
            {
                this.batchItems[i] = SpriteBatchItem.Empty;
            }

            this.currentBatchItem = 0;
            this.previousTextureID = 0;
        }

        /// <inheritdoc/>
        public Matrix4 BuildTransformationMatrix(SysVector2 viewPortSize, float x, float y, int width, int height, float size, float angle)
        {
            if (viewPortSize.X <= 0)
            {
                throw new ArgumentException("The port size width cannot be a negative or zero value.");
            }

            if (viewPortSize.Y <= 0)
            {
                throw new ArgumentException("The port size height cannot be a negative or zero value.");
            }

            var scaleX = width / viewPortSize.X;
            var scaleY = height / viewPortSize.Y;

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
