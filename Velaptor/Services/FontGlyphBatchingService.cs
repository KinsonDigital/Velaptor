// <copyright file="FontGlyphBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Velaptor.Guards;
using Velaptor.OpenGL;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.Services;

/// <summary>
/// Manages the process of batching up glyphs to be rendered.
/// </summary>
internal sealed class FontGlyphBatchingService : IBatchingService<FontGlyphBatchItem>
{
    private readonly IDisposable unsubscriber;
    private (bool shouldRender, FontGlyphBatchItem item)[] batchItems = null!;
#if DEBUG
    private uint currentFrame;
#endif
    private bool firstTimeRender = true;
    private uint currentTextureId;
    private uint previousTextureId;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGlyphBatchingService"/> class.
    /// </summary>
    /// <param name="batchSizeReactable">Receives push notifications about the batch size.</param>
    public FontGlyphBatchingService(IReactable<BatchSizeData> batchSizeReactable)
    {
        EnsureThat.ParamIsNotNull(batchSizeReactable);

        this.unsubscriber = batchSizeReactable.Subscribe(new Reactor<BatchSizeData>(
            onNext: data =>
            {
                var items = new List<(bool, FontGlyphBatchItem)>();

                for (var i = 0u; i < data.BatchSize; i++)
                {
                    items.Add((false, default));
                }

                this.batchItems = items.ToArray();
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? ReadyForRendering;

    /// <inheritdoc/>
    public ReadOnlyCollection<(bool shouldRender, FontGlyphBatchItem item)> BatchItems
    {
        get => new (this.batchItems);
        set => this.batchItems = value.ToArray();
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

        var batchIsFull = this.batchItems.All(i => i.item.IsEmpty() is false);

        if (hasSwitchedTexture || batchIsFull)
        {
            this.ReadyForRendering?.Invoke(this, EventArgs.Empty);
        }

        var emptyItemIndex = this.batchItems.IndexOf(i => i.item.IsEmpty());

        if (emptyItemIndex != -1)
        {
            this.batchItems[emptyItemIndex] = (true, item);
        }

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

        for (var i = 0u; i < this.batchItems.Length; i++)
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
    }
}
