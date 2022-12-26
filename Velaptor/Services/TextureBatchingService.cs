// <copyright file="TextureBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Carbonate;
using Content;
using Exceptions;
using Guards;
using OpenGL;
using ReactableData;

/// <summary>
/// Manages the process of batching up the rendering of <see cref="ITexture"/>s.
/// </summary>
internal sealed class TextureBatchingService : IBatchingService<TextureBatchItem>
{
    private readonly IDisposable unsubscriber;
    private TextureBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchingService"/> class.
    /// </summary>
    /// <param name="reactable">Sends and receives push notifications.</param>
    public TextureBatchingService(IReactable reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.unsubscriber = reactable.Subscribe(new Reactor(
            eventId: NotificationIds.BatchSizeId,
            onNextMsg: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException(
                        $"{nameof(TextureBatchingService)}.Constructor()",
                        NotificationIds.BatchSizeId);
                }

                var items = new List<TextureBatchItem>();

                for (var i = 0u; i < batchSize; i++)
                {
                    items.Add(default);
                }

                this.batchItems = items.ToArray();
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <remarks>
    /// Invoked when the scenarios below occur:
    /// <list type="bullet">
    ///     <item>When the item to be called is a different texture.</item>
    ///     <item>When all of the items are ready to be rendered.</item>
    /// </list>
    /// </remarks>
    public event EventHandler<EventArgs>? ReadyForRendering;

    /// <inheritdoc/>
    public ReadOnlyCollection<TextureBatchItem> BatchItems
    {
        get => new (this.batchItems);
        set => this.batchItems = value.ToArray();
    }

    /// <summary>
    /// Adds the given <paramref name="item"/> to the batch.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(in TextureBatchItem item)
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
