// <copyright file="FontGlyphBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Manages the process of batching up glyphs to be rendered.
    /// </summary>
    internal class FontGlyphBatchingService : IBatchingService<FontGlyphBatchItem>
    {
        private SortedDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)> batchItems = new ();
        private uint currentBatchIndex;
        private uint batchSize;
#if DEBUG
        private uint currentFrame;
#endif
        private bool firstTimeRender = true;
        private uint currentTextureId;
        private uint previousTextureId;

        /// <summary>
        /// Occurs when a batch is full.
        /// </summary>
        /// <remarks>
        /// The batch is ready when the total amount of items to be rendered is equal to the <see cref="BatchSize"/>.
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
        public ReadOnlyDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)> BatchItems
        {
            get => new (this.batchItems);
            set => this.batchItems = new SortedDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)>(value);
        }

        /// <summary>
        /// Adds the given <paramref name="item"/> to the batch.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(FontGlyphBatchItem item)
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
#if DEBUG
            this.currentFrame += 1u;
#endif

            for (var i = 0u; i < this.batchItems.Count; i++)
            {
                if (this.batchItems[i].shouldRender is false)
                {
                    continue;
                }

                (bool shouldRender, FontGlyphBatchItem batchItem) itemToEmpty = this.batchItems[i];

#if DEBUG
                AppStats.RecordFontGlyphRendering(
                    this.currentFrame,
                    itemToEmpty.batchItem.Glyph,
                    itemToEmpty.batchItem.TextureId,
                    itemToEmpty.batchItem.Size,
                    itemToEmpty.batchItem.DestRect);
#endif

                itemToEmpty.shouldRender = false;
                itemToEmpty.batchItem.Empty();

                this.batchItems[i] = itemToEmpty;
            }

            this.currentBatchIndex = 0u;
        }
    }
}
