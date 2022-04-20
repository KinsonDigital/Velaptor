// <copyright file="BatchServiceManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.ObjectModel;
    using Velaptor.Graphics;
    using Velaptor.Guards;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc />
    internal class BatchServiceManager : IBatchServiceManager
    {
        private readonly IBatchingService<TextureBatchItem> textureBatchingService;
        private readonly IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService;
        private readonly IBatchingService<RectShape> rectBatchingService;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchServiceManager"/> class.
        /// </summary>
        /// <param name="textureBatchingService">Manages a batch of textures.</param>
        /// <param name="fontGlyphBatchingService">Manages a batch of font glyphs.</param>
        /// <param name="rectBatchingService">Manages the batch of rectangles.</param>
        public BatchServiceManager(
            IBatchingService<TextureBatchItem> textureBatchingService,
            IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService,
            IBatchingService<RectShape> rectBatchingService)
        {
            EnsureThat.ParamIsNotNull(textureBatchingService);
            EnsureThat.ParamIsNotNull(fontGlyphBatchingService);
            EnsureThat.ParamIsNotNull(rectBatchingService);

            this.textureBatchingService = textureBatchingService;
            this.textureBatchingService.BatchFilled += TextureBatchingService_BatchFilled;

            this.fontGlyphBatchingService = fontGlyphBatchingService;
            this.fontGlyphBatchingService.BatchFilled += FontGlyphBatchingService_BatchFilled;

            this.rectBatchingService = rectBatchingService;
            this.rectBatchingService.BatchFilled += RectBatchingService_BatchFilled;
        }

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? TextureBatchFilled;

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? FontGlyphBatchFilled;

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? RectBatchFilled;

        /// <inheritdoc/>
        public ReadOnlyDictionary<uint, (bool shouldRender, TextureBatchItem item)> TextureBatchItems
        {
            get => this.textureBatchingService.BatchItems;
            set => this.textureBatchingService.BatchItems = value;
        }

        /// <inheritdoc/>
        public ReadOnlyDictionary<uint, (bool shouldRender, FontGlyphBatchItem item)> FontGlyphBatchItems
        {
            get => this.fontGlyphBatchingService.BatchItems;
            set => this.fontGlyphBatchingService.BatchItems = value;
        }

        /// <inheritdoc/>
        public ReadOnlyDictionary<uint, (bool shouldRender, RectShape item)> RectBatchItems
        {
            get => this.rectBatchingService.BatchItems;
            set => this.rectBatchingService.BatchItems = value;
        }

        /// <inheritdoc/>
        public uint GetBatchSize(BatchServiceType serviceType) =>
            serviceType switch
            {
                BatchServiceType.Texture => this.textureBatchingService.BatchSize,
                BatchServiceType.FontGlyph => this.fontGlyphBatchingService.BatchSize,
                BatchServiceType.Rectangle => this.rectBatchingService.BatchSize,
                _ => throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, $"The enum '{nameof(BatchServiceType)}' value is invalid.")
            };

        /// <inheritdoc/>
        public void SetBatchSize(BatchServiceType serviceType, uint batchSize)
        {
            switch (serviceType)
            {
                case BatchServiceType.Texture:
                    this.textureBatchingService.BatchSize = batchSize;
                    break;
                case BatchServiceType.FontGlyph:
                    this.fontGlyphBatchingService.BatchSize = batchSize;
                    break;
                case BatchServiceType.Rectangle:
                    this.rectBatchingService.BatchSize = batchSize;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, $"The enum '{nameof(BatchServiceType)}' value is invalid.");
            }
        }

        /// <inheritdoc/>
        public void AddTextureBatchItem(TextureBatchItem batchItem) => this.textureBatchingService.Add(batchItem);

        /// <inheritdoc/>
        public void AddFontGlyphBatchItem(FontGlyphBatchItem batchItem) => this.fontGlyphBatchingService.Add(batchItem);

        /// <inheritdoc/>
        public void AddRectBatchItem(RectShape batchItem) => this.rectBatchingService.Add(batchItem);

        /// <inheritdoc/>
        public void EmptyBatch(BatchServiceType serviceType)
        {
            switch (serviceType)
            {
                case BatchServiceType.Texture:
                    this.textureBatchingService.EmptyBatch();
                    break;
                case BatchServiceType.FontGlyph:
                    this.fontGlyphBatchingService.EmptyBatch();
                    break;
                case BatchServiceType.Rectangle:
                    this.rectBatchingService.EmptyBatch();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(serviceType),
                        serviceType,
                        $"The enum '{nameof(BatchServiceType)}' value is invalid.");
            }
        }

        /// <inheritdoc/>
        public void EndBatch(BatchServiceType serviceType)
        {
            switch (serviceType)
            {
                case BatchServiceType.Texture:
                    this.TextureBatchFilled?.Invoke(this, EventArgs.Empty);
                    break;
                case BatchServiceType.FontGlyph:
                    this.FontGlyphBatchFilled?.Invoke(this, EventArgs.Empty);
                    break;
                case BatchServiceType.Rectangle:
                    this.RectBatchFilled?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(serviceType),
                        serviceType,
                        $"The enum '{nameof(BatchServiceType)}' value is invalid.");
            }
        }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureBatchingService.BatchFilled -= TextureBatchingService_BatchFilled;
                this.fontGlyphBatchingService.BatchFilled -= FontGlyphBatchingService_BatchFilled;
                this.rectBatchingService.BatchFilled -= RectBatchingService_BatchFilled;
            }

            this.disposed = true;
        }

        /// <summary>
        /// Invoked the texture batch filled event.
        /// </summary>
        private void TextureBatchingService_BatchFilled(object? sender, EventArgs e)
            => this.TextureBatchFilled?.Invoke(sender, e);

        /// <summary>
        /// Invoked the font glyph batch filled event.
        /// </summary>
        private void FontGlyphBatchingService_BatchFilled(object? sender, EventArgs e)
            => this.FontGlyphBatchFilled?.Invoke(sender, e);

        /// <summary>
        /// Invoked the rectangle batch filled event.
        /// </summary>
        private void RectBatchingService_BatchFilled(object? sender, EventArgs e)
            => this.RectBatchFilled?.Invoke(sender, e);
    }
}
