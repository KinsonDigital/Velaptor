// <copyright file="LineBatchingService.cs" company="KinsonDigital">
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
/// Manages the process of batching the rendering of lines.
/// </summary>
internal sealed class LineBatchingService : IBatchingService<LineBatchItem>
{
    private readonly IDisposable unsubscriber;
    private LineBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchingService"/> class.
    /// </summary>
    /// <param name="batchSizeReactable">Receives push notifications about the batch size.</param>
    public LineBatchingService(IReactable<BatchSizeData> batchSizeReactable)
    {
        EnsureThat.ParamIsNotNull(batchSizeReactable);

        this.unsubscriber = batchSizeReactable.Subscribe(new Reactor<BatchSizeData>(
            onNext: data =>
            {
                var items = new List<LineBatchItem>();

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
    public ReadOnlyCollection<LineBatchItem> BatchItems
    {
        get => new (this.batchItems);
        set => this.batchItems = value.ToArray();
    }

    /// <inheritdoc/>
    public void Add(in LineBatchItem line)
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

        this.batchItems[emptyItemIndex] = line;
    }

    /// <inheritdoc/>
    public void EmptyBatch()
    {
        for (var i = 0u; i < this.batchItems.Length; i++)
        {
            if (this.batchItems[i].IsEmpty())
            {
                continue;
            }

            this.batchItems[i] = default;
        }
    }
}
