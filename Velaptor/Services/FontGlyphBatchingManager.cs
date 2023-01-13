// <copyright file="FontGlyphBatchingManager.cs" company="KinsonDigital">
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
using OpenGL;
using ReactableData;

/// <summary>
/// Manages the process of batching glyphs to be rendered.
/// </summary>
internal sealed class FontGlyphBatchingManager : IBatchingManager<FontGlyphBatchItem>
{
    private readonly IDisposable unsubscriber;
    private readonly IPushReactable pushReactable;
    private FontGlyphBatchItem[] batchItems = null!;
#if DEBUG
    private uint currentFrame;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGlyphBatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public FontGlyphBatchingManager(IReactableFactory reactableFactory)
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
                        $"{nameof(FontGlyphBatchingManager)}.Constructor()",
                        NotificationIds.BatchSizeSetId);
                }

                var items = new List<FontGlyphBatchItem>();

                for (var i = 0u; i < batchSize; i++)
                {
                    items.Add(default);
                }

                this.batchItems = items.ToArray();
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }

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
            this.pushReactable.Push(NotificationIds.RenderFontsId);
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
