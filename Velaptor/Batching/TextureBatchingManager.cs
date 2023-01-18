// <copyright file="TextureBatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Content;
using Factories;
using Guards;
using OpenGL.Batching;
using ReactableData;

/// <summary>
/// Manages the process of batching up the rendering of <see cref="ITexture"/>s.
/// </summary>
internal sealed class TextureBatchingManager : IBatchingManager<TextureBatchItem>
{
    private readonly IDisposable unsubscriber;
    private readonly IPushReactable pushReactable;
    private TextureBatchItem[] batchItems = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public TextureBatchingManager(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.pushReactable = reactableFactory.CreateNoDataReactable();

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeSetId));
        this.unsubscriber = batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: PushNotifications.BatchSizeSetId,
            name: batchSizeName,
            onReceiveData: data =>
            {
                var items = new List<TextureBatchItem>();

                for (var i = 0u; i < data.BatchSize; i++)
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
            this.pushReactable.Push(PushNotifications.RenderTexturesId);
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
