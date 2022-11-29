// <copyright file="FontGlyphBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Guards;
using OpenGL;
using Reactables.Core;
using Reactables.ReactableData;

/// <summary>
/// Manages the process of batching glyphs to be rendered.
/// </summary>
internal sealed class FontGlyphBatchingService : IBatchingService<FontGlyphBatchItem>
{
    private readonly IDisposable unsubscriber;
    private FontGlyphBatchItem[] batchItems = null!;
#if DEBUG
    private uint currentFrame;
#endif

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
                var items = new List<FontGlyphBatchItem>();

                for (var i = 0u; i < data.BatchSize; i++)
                {
                    items.Add(default);
                }

                this.batchItems = items.ToArray();
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? ReadyForRendering;

    /// <inheritdoc/>
    public ReadOnlyCollection<FontGlyphBatchItem> BatchItems
    {
        get => new (this.batchItems);
        set => this.batchItems = value.ToArray();
    }

    /// <summary>
    /// Adds the given <paramref name="item"/> to the batch.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(in FontGlyphBatchItem item)
    {
        var batchIsFull = this.batchItems.All(i => i.IsEmpty() is false);

        if (batchIsFull)
        {
            this.ReadyForRendering?.Invoke(this, EventArgs.Empty);
        }

        var emptyItemIndex = this.batchItems.IndexOf(i => i.IsEmpty());

        if (emptyItemIndex == -1)
        {
            return;
        }

        this.batchItems[emptyItemIndex] = item;
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
            if (this.batchItems[i].IsEmpty())
            {
                continue;
            }

#if DEBUG
            AppStats.RecordFontGlyphRendering(
                this.currentFrame,
                this.batchItems[i].Glyph,
                this.batchItems[i].TextureId,
                this.batchItems[i].Size,
                this.batchItems[i].DestRect);
#endif

            this.batchItems[i] = default;
        }
    }
}
