// <copyright file="LineBatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Exceptions;
using Factories;
using Guards;
using OpenGL.Batching;
using ReactableData;

/// <summary>
/// Manages the process of batching the rendering of lines.
/// </summary>
internal sealed class LineBatchingManager : IBatchingManager<LineBatchItem>
{
    private readonly IDisposable unsubscriber;
    private readonly IPushReactable pushReactable;
    private LineBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public LineBatchingManager(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.pushReactable = reactableFactory.CreateNoDataReactable();

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(NotificationIds.BatchSizeSetId));
        this.unsubscriber = batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: NotificationIds.BatchSizeSetId,
            name: batchSizeName,
            onReceiveMsg: msg =>
            {
                var batchSize = msg.GetData()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException(
                        $"{nameof(LineBatchingManager)}.Constructor()",
                        NotificationIds.BatchSizeSetId);
                }

                var items = new List<LineBatchItem>();

                for (var i = 0u; i < batchSize; i++)
                {
                    items.Add(default);
                }

                this.batchItems = items.ToArray();
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }

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
            this.pushReactable.Push(NotificationIds.RenderLinesId);
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
