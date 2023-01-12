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
    private readonly IPushReactable reactable;
    private TextureBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchingService"/> class.
    /// </summary>
    /// <param name="reactable">Sends and receives push notifications.</param>
    public TextureBatchingService(IPushReactable reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.reactable = reactable;

        var batchSizeName = this.GetExecutionMemberName(nameof(NotificationIds.BatchSizeSetId));
        this.unsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.BatchSizeSetId,
            name: batchSizeName,
            onReceiveMsg: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException(
                        $"{nameof(TextureBatchingService)}.Constructor()",
                        NotificationIds.BatchSizeSetId);
                }

                var items = new List<TextureBatchItem>();

                for (var i = 0u; i < batchSize; i++)
                {
                    items.Add(default);
                }

                this.batchItems = items.ToArray();
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }

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
            this.reactable.Push(NotificationIds.RenderTexturesId);
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
