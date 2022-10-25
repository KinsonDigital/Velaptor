// <copyright file="RectBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Manages the process of batching the rendering of rectangles.
    /// </summary>
    internal sealed class RectBatchingService : IBatchingService<RectShape>
    {
        private SortedDictionary<uint, (bool shouldRender, RectShape item)> batchItems = new ();
        private uint currentBatchIndex;
        private uint batchSize;

        /// <summary>
        /// Occurs when a batch is full.
        /// </summary>
        /// <remarks>
        ///     The batch is ready when the total amount of items to be rendered is equal to the <see cref="BatchSize"/>.
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
        public ReadOnlyDictionary<uint, (bool shouldRender, RectShape item)> BatchItems
        {
            get => new (this.batchItems);
            set => this.batchItems = new SortedDictionary<uint, (bool shouldRender, RectShape item)>(value);
        }

        /// <summary>
        /// Adds the given <paramref name="rect"/> to the batch.
        /// </summary>
        /// <param name="rect">The item to be added.</param>
        public void Add(RectShape rect)
        {
            var batchIsFull = this.currentBatchIndex >= BatchSize;

            if (batchIsFull)
            {
                this.BatchFilled?.Invoke(this, EventArgs.Empty);
            }

            this.batchItems[this.currentBatchIndex] = (true, rect);
            this.currentBatchIndex += 1;
        }

        /// <summary>
        /// Adds the given list of <paramref name="rects"/> to batch.
        /// </summary>
        /// <param name="rects">The items to be added.</param>
        public void AddRange(IEnumerable<RectShape> rects)
        {
            foreach (var rect in rects)
            {
                Add(rect);
            }
        }

        /// <summary>
        /// Empties the entire batch.
        /// </summary>
        /// <remarks>
        /// An empty batch are items that are set NOT set to render and have all values set to default.
        /// </remarks>
        public void EmptyBatch()
        {
            for (var i = 0u; i < this.batchItems.Count; i++)
            {
                if (this.batchItems[i].shouldRender is false)
                {
                    continue;
                }

                (bool shouldRender, RectShape rectItem) itemToEmpty = this.batchItems[i];
                itemToEmpty.shouldRender = false;
                itemToEmpty.rectItem.Empty();

                this.batchItems[i] = itemToEmpty;
            }

            this.currentBatchIndex = 0u;
        }
    }
}
