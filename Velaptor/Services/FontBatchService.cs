namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Manages the process of batching up the rendering of text.
    /// </summary>
    internal class FontBatchService : IBatchManagerService<FontBatchItem>
    {
        private SortedDictionary<uint, (bool shouldRender, FontBatchItem item)> batchItems = new ();
        private uint currentBatchIndex;
        private uint batchSize;
        private uint currentFrame;

        /// <summary>
        /// Occurs when a batch is full.
        /// </summary>
        /// <remarks>
        /// Scenarios When The Batch Is Ready:
        /// <list type="number">
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
        public ReadOnlyDictionary<uint, (bool shouldRender, FontBatchItem item)> BatchItems
        {
            get => new (this.batchItems);
            set => this.batchItems = new SortedDictionary<uint, (bool shouldRender, FontBatchItem item)>(value);
        }

        /// <summary>
        /// Adds the given <paramref name="rect"/> to the batch.
        /// </summary>
        /// <param name="rect">The item to be added.</param>
        public void Add(FontBatchItem rect)
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
        public void AddRange(IEnumerable<FontBatchItem> rects)
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
        /// An empty batch are items that are set NOT to render and have all values set to default.
        /// </remarks>
        public void EmptyBatch()
        {
            this.currentFrame += 1u;

            for (var i = 0u; i < this.batchItems.Count; i++)
            {
                if (this.batchItems[i].shouldRender is false)
                {
                    continue;
                }

                (bool shouldRender, FontBatchItem spriteItem) itemToEmpty = this.batchItems[i];

#if DEBUG
                AppStats.RecordFontGlyphRendering(
                    this.currentFrame,
                    itemToEmpty.spriteItem.Glyph,
                    itemToEmpty.spriteItem.TextureId,
                    itemToEmpty.spriteItem.DestRect);
#endif

                itemToEmpty.shouldRender = false;
                itemToEmpty.spriteItem.Empty();

                this.batchItems[i] = itemToEmpty;
            }

            this.currentBatchIndex = 0u;
        }
    }
}
