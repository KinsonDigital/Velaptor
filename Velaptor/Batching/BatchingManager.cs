// <copyright file="BatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Factories;
using Guards;
using OpenGL.Batching;
using ReactableData;

/// <inheritdoc/>
internal sealed class BatchingManager : IBatchingManager
{
    private readonly IDisposable batchSizeUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly IDisposable texturePullUnsubscriber;
    private readonly IDisposable fontPullUnsubscriber;
    private readonly IDisposable rectPullUnsubscriber;
    private readonly IDisposable linePullUnsubscriber;
    private readonly IDisposable emptyBatchUnsubscriber;
    private Memory<RenderItem<TextureBatchItem>> textureItems;
    private Memory<RenderItem<FontGlyphBatchItem>> fontItems;
    private Memory<RenderItem<RectBatchItem>> rectItems;
    private Memory<RenderItem<LineBatchItem>> lineItems;
    private bool isShutDown;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactable objects.</param>
    public BatchingManager(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeReactorName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeSetId));
        this.batchSizeUnsubscriber = batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: PushNotifications.BatchSizeSetId,
            name: batchSizeReactorName,
            onReceiveData: data => SetupItems(data.BatchSize),
            onUnsubscribe: () => this.batchSizeUnsubscriber?.Dispose()));

        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        var shutDownReactorName = this.GetExecutionMemberName(nameof(PushNotifications.SystemShuttingDownId));
        this.shutDownUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.SystemShuttingDownId,
            shutDownReactorName,
            ShutDown));

        var emptyBatchReactorName = this.GetExecutionMemberName(nameof(PushNotifications.EmptyBatchId));
        this.emptyBatchUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.EmptyBatchId,
            emptyBatchReactorName,
            EmptyBatch));

        var texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();
        var texturePullReactorName = this.GetExecutionMemberName(nameof(PullResponses.GetTextureItemsId));
        this.texturePullUnsubscriber = texturePullReactable.Subscribe(new RespondReactor<Memory<RenderItem<TextureBatchItem>>>(
            respondId: PullResponses.GetTextureItemsId,
            name: texturePullReactorName,
            onRespond: () =>
            {
                var lastFullItemIndex = this.textureItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.textureItems
                    : this.textureItems[..lastFullItemIndex];
            }));

        var fontPullReactable = reactableFactory.CreateFontPullBatchReactable();
        var fontPullReactorName = this.GetExecutionMemberName(nameof(PullResponses.GetFontItemsId));
        this.fontPullUnsubscriber = fontPullReactable.Subscribe(new RespondReactor<Memory<RenderItem<FontGlyphBatchItem>>>(
            respondId: PullResponses.GetFontItemsId,
            name: fontPullReactorName,
            onRespond: () =>
            {
                var lastFullItemIndex = this.fontItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.fontItems
                    : this.fontItems[..lastFullItemIndex];
            }));

        var rectPullReactable = reactableFactory.CreateRectPullBatchReactable();
        var rectPullReactorName = this.GetExecutionMemberName(nameof(PullResponses.GetRectItemsId));
        this.rectPullUnsubscriber = rectPullReactable.Subscribe(new RespondReactor<Memory<RenderItem<RectBatchItem>>>(
            respondId: PullResponses.GetRectItemsId,
            name: rectPullReactorName,
            onRespond: () =>
            {
                var lastFullItemIndex = this.rectItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.rectItems
                    : this.rectItems[..lastFullItemIndex];
            }));

        var linePullReactable = reactableFactory.CreateLinePullBatchReactable();
        var linePullReactorName = this.GetExecutionMemberName(nameof(PullResponses.GetLineItemsId));
        this.linePullUnsubscriber = linePullReactable.Subscribe(new RespondReactor<Memory<RenderItem<LineBatchItem>>>(
            respondId: PullResponses.GetLineItemsId,
            name: linePullReactorName,
            onRespond: () =>
            {
                var lastFullItemIndex = this.lineItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.lineItems
                    : this.lineItems[..lastFullItemIndex];
            }));
    }

    /// <summary>
    /// Gets the texture items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<TextureBatchItem>> TextureItems => this.textureItems.Span;

    /// <summary>
    /// Gets the font items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<FontGlyphBatchItem>> FontItems => this.fontItems.Span;

    /// <summary>
    /// Gets the rectangle items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<RectBatchItem>> RectItems => this.rectItems.Span;

    /// <summary>
    /// Gets the line items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<LineBatchItem>> LineItems => this.lineItems.Span;

    /// <inheritdoc/>
    public void AddTextureItem(TextureBatchItem item, int layer)
    {
        var emptyItemIndex = this.textureItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            // TODO: Replace this code and comment once GPU adjustment feature is added
            // NOTE: This exception will eventually be replaced with code to increase the GPU buffer size.
            throw new Exception("The texture batch is full.");
        }

        this.textureItems.Span[emptyItemIndex] = new RenderItem<TextureBatchItem>
        {
            Layer = layer,
            Item = item,
        };
    }

    /// <inheritdoc/>
    public void AddFontItem(FontGlyphBatchItem item, int layer)
    {
        var emptyItemIndex = this.fontItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            // TODO: Replace this code and comment once GPU adjustment feature is added
            // NOTE: This exception will eventually be replaced with code to increase the GPU buffer size.
            throw new Exception("The font batch is full.");
        }

        this.fontItems.Span[emptyItemIndex] = new RenderItem<FontGlyphBatchItem>
        {
            Layer = layer,
            Item = item,
        };
    }

    /// <inheritdoc/>
    public void AddRectItem(RectBatchItem item, int layer)
    {
        var emptyItemIndex = this.rectItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            // TODO: Replace this code and comment once GPU adjustment feature is added
            // NOTE: This exception will eventually be replaced with code to increase the GPU buffer size.
            throw new Exception("The rect batch is full.");
        }

        this.rectItems.Span[emptyItemIndex] = new RenderItem<RectBatchItem>
        {
            Layer = layer,
            Item = item,
        };
    }

    /// <inheritdoc/>
    public void AddLineItem(LineBatchItem item, int layer)
    {
        var emptyItemIndex = this.lineItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            // TODO: Replace this code and comment once GPU adjustment feature is added
            // NOTE: This exception will eventually be replaced with code to increase the GPU buffer size.
            throw new Exception("The line batch is full.");
        }

        this.lineItems.Span[emptyItemIndex] = new RenderItem<LineBatchItem>
        {
            Layer = layer,
            Item = item,
        };
    }

    /// <summary>
    /// Sets up all of the batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    private void SetupItems(uint batchSize)
    {
        this.textureItems = new RenderItem<TextureBatchItem>[batchSize];
        for (var i = 0; i < batchSize; i++)
        {
            this.textureItems.Span[i] = default;
        }

        this.fontItems = new RenderItem<FontGlyphBatchItem>[batchSize];
        for (var i = 0; i < batchSize; i++)
        {
            this.fontItems.Span[i] = default;
        }

        this.rectItems = new RenderItem<RectBatchItem>[batchSize];
        for (var i = 0; i < batchSize; i++)
        {
            this.rectItems.Span[i] = default;
        }

        this.lineItems = new RenderItem<LineBatchItem>[batchSize];
        for (var i = 0; i < batchSize; i++)
        {
            this.lineItems.Span[i] = default;
        }
    }

    /// <summary>
    /// Empties all of the different batch types.
    /// </summary>
    private void EmptyBatch()
    {
        for (var i = 0; i < this.textureItems.Length; i++)
        {
            if (this.textureItems.Span[i].Item.IsEmpty())
            {
                break;
            }

            this.textureItems.Span[i] = default;
        }

        for (var i = 0; i < this.fontItems.Length; i++)
        {
            if (this.fontItems.Span[i].Item.IsEmpty())
            {
                break;
            }

            this.fontItems.Span[i] = default;
        }

        for (var i = 0; i < this.rectItems.Length; i++)
        {
            if (this.rectItems.Span[i].Item.IsEmpty())
            {
                break;
            }

            this.rectItems.Span[i] = default;
        }

        for (var i = 0; i < this.lineItems.Length; i++)
        {
            if (this.lineItems.Span[i].Item.IsEmpty())
            {
                break;
            }

            this.lineItems.Span[i] = default;
        }
    }

    /// <summary>
    /// Shuts down the manager.
    /// </summary>
    private void ShutDown()
    {
        if (this.isShutDown)
        {
            return;
        }

        this.shutDownUnsubscriber.Dispose();
        this.emptyBatchUnsubscriber.Dispose();
        this.texturePullUnsubscriber.Dispose();
        this.fontPullUnsubscriber.Dispose();
        this.rectPullUnsubscriber.Dispose();
        this.linePullUnsubscriber.Dispose();

        this.isShutDown = true;
    }
}
