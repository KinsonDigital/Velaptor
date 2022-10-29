// <copyright file="TextureBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Velaptor.Content;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Manages the process of batching up the rendering of <see cref="ITexture"/>s.
    /// </summary>
    internal sealed class TextureBatchingService : IBatchingService<TextureBatchItem>
    {
        private SortedDictionary<uint, (bool shouldRender, TextureBatchItem item)> batchItems = new ();
        private uint currentBatchIndex;
        private uint batchSize;
        private bool firstTimeRender = true;
        private uint currentTextureId;
        private uint previousTextureId;

        /// <summary>
        /// Occurs when a batch is full.
        /// </summary>
        /// <remarks>
        /// Scenarios When The Batch Is Ready:
        /// <list type="number">
        ///     <item>The batch is ready when draw calls switch to another texture.</item>
        ///     <item>The batch is ready when the total amount of items to be rendered is equal to the <see cref="BatchSize"/>.</item>
        /// </list>
        /// </remarks>
        public event EventHandler<EventArgs>? BatchFilled;

        /// <inheritdoc/>
        public uint BatchSize
        {
            get => this.batchSize;
            set
            {
                this.batchSize = value;
                this.batchItems.Clear();

                for (var i = 0u; i < this.batchSize; i++)
                {
                    this.batchItems.Add(i, (false, default));
                }
            }
        }

        /// <inheritdoc/>
        public ReadOnlyDictionary<uint, (bool shouldRender, TextureBatchItem item)> BatchItems
        {
            get => new (this.batchItems);
            set => this.batchItems = new SortedDictionary<uint, (bool shouldRender, TextureBatchItem item)>(value);
        }

        /// <summary>
        /// Adds the given <paramref name="item"/> to the batch.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(TextureBatchItem item)
        {
            this.currentTextureId = item.TextureId;

            var hasSwitchedTexture = this.currentTextureId != this.previousTextureId
                && this.firstTimeRender is false;
            var batchIsFull = this.currentBatchIndex >= BatchSize;

            if (hasSwitchedTexture || batchIsFull)
            {
                this.BatchFilled?.Invoke(this, EventArgs.Empty);
            }

            this.batchItems[this.currentBatchIndex] = (true, item);
            this.currentBatchIndex += 1;

            this.previousTextureId = this.currentTextureId;
            this.firstTimeRender = false;
        }

        /// <summary>
        /// Empties the entire batch.
        /// </summary>
        /// <remarks>
        /// An empty batch are items that are set NOT to render and have all values set to default.
        /// </remarks>
        public void EmptyBatch()
        {
            for (var i = 0u; i < this.batchItems.Count; i++)
            {
                if (this.batchItems[i].shouldRender is false)
                {
                    continue;
                }

                (bool shouldRender, TextureBatchItem batchItem) itemToEmpty = this.batchItems[i];

                itemToEmpty.shouldRender = false;
                itemToEmpty.batchItem.Empty();

                this.batchItems[i] = itemToEmpty;
            }

            this.currentBatchIndex = 0u;
            this.previousTextureId = 0u;
        }
    }
}
