// <copyright file="FontBatchService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using OpenGL;

    // TODO: This file needs to be updated in Velaptor
    // Process todo items

    internal class FontBatchService : IBatchManagerService<SpriteBatchItem>
    {
        private readonly Dictionary<uint, (bool shouldRender, SpriteBatchItem rect)> batchItems = new();
        private uint currentBatchIndex;
        private uint batchSize;
        private bool firstTimeRender = true;
        private uint currentTextureId = 0u;
        private uint previousTextureId = 0u;

        public event EventHandler<EventArgs>? BatchFilled;

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

        public ReadOnlyDictionary<uint, (bool shouldRender, SpriteBatchItem item)> AllBatchItems => new(this.batchItems);

        public void Add(SpriteBatchItem rect)
        {
            this.currentTextureId = rect.TextureId;

            var hasSwitchedTexture = this.currentTextureId != this.previousTextureId
                && this.firstTimeRender is false;
            var batchIsFull = this.currentBatchIndex >= BatchSize;

            if (hasSwitchedTexture || batchIsFull)
            {
                this.BatchFilled?.Invoke(this, EventArgs.Empty);
            }

            this.batchItems[this.currentBatchIndex] = (true, rect);
            this.currentBatchIndex += 1;

            this.previousTextureId = this.currentTextureId;
            this.firstTimeRender = false;
        }

        public void EmptyBatch()
        {
            for (var i = 0u; i < this.batchItems.Count; i++)
            {
                if (this.batchItems[i].shouldRender is false)
                {
                    continue;
                }

                (bool shouldRender, SpriteBatchItem spriteItem) itemToEmpty = this.batchItems[i];
                itemToEmpty.shouldRender = false;
                itemToEmpty.spriteItem.Empty();

                this.batchItems[i] = itemToEmpty;
            }

            this.currentBatchIndex = 0u;
            this.previousTextureId = 0u;
        }
    }
}
