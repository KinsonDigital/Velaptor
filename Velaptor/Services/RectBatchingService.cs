// <copyright file="RectBatchingService.cs" company="KinsonDigital">
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
/// Manages the process of batching the rendering of rectangles.
/// </summary>
internal sealed class RectBatchingService : IBatchingService<RectBatchItem>
{
    private readonly IDisposable unsubscriber;
    private RectShape[] batchItems = null!;
    private RectBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectBatchingService"/> class.
    /// </summary>
    /// <param name="batchSizeReactable">Receives push notifications about the batch size.</param>
    public RectBatchingService(IReactable<BatchSizeData> batchSizeReactable)
    {
        EnsureThat.ParamIsNotNull(batchSizeReactable);

        this.unsubscriber = batchSizeReactable.Subscribe(new Reactor<BatchSizeData>(
            onNext: data =>
            {
                var items = new List<RectBatchItem>();

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
    public ReadOnlyCollection<RectBatchItem> BatchItems
    {
        get => new (this.batchItems);
        set => this.batchItems = value.ToArray();
    }

    /// <summary>
    /// Adds the given <paramref name="rect"/> to the batch.
    /// </summary>
    /// <param name="rect">The item to be added.</param>
    public void Add(RectBatchItem rect)
    {
        var batchIsFull = this.batchItems.All(i => i.IsEmpty() is false);

        if (batchIsFull)
        {
            this.ReadyForRendering?.Invoke(this, EventArgs.Empty);
        }

        var emptyItemIndex = this.batchItems.IndexOf(i => i.IsEmpty());

        if (emptyItemIndex != -1)
        {
            this.batchItems[emptyItemIndex] = rect;
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
        for (var i = 0u; i < this.batchItems.Length; i++)
        {
            if (this.batchItems[i].IsEmpty())
            {
                continue;
            }

            // TODO: This will probably not work anymore once the struct is readonly
            // TODO: This is probably going to have to be a completely new item that writes over top
            RectBatchItem itemToEmpty = this.batchItems[i];
            itemToEmpty.Empty();

            this.batchItems[i] = itemToEmpty;
        }
    }
}
