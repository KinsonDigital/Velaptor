// <copyright file="BatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using System.Linq;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Exceptions;
using Factories;
using Guards;
using OpenGL.Batching;
using ReactableData;

/// <inheritdoc/>
internal sealed class BatchingManager : IBatchingManager
{
    private const float BatchIncreasePercentage = 0.5f;
    private readonly IDisposable batchSizeUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;
    private readonly IDisposable texturePullUnsubscriber;
    private readonly IDisposable fontPullUnsubscriber;
    private readonly IDisposable rectPullUnsubscriber;
    private readonly IDisposable linePullUnsubscriber;
    private readonly IDisposable emptyBatchUnsubscriber;
    private readonly IPushReactable<BatchSizeData> batchSizeReactable;
    private readonly BatchType[] batchTypes = Enum.GetValues<BatchType>();
    private Memory<RenderItem<TextureBatchItem>> textureItems;
    private Memory<RenderItem<FontGlyphBatchItem>> fontItems;
    private Memory<RenderItem<ShapeBatchItem>> rectItems;
    private Memory<RenderItem<LineBatchItem>> lineItems;
    private bool isShutDown;
    private bool firstTimeSettingBatchSize = true;
    private uint textureBatchSize;
    private uint fontBatchSize;
    private uint rectBatchSize;
    private uint lineBatchSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactable objects.</param>
    public BatchingManager(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeReactorName = this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId));
        this.batchSizeUnsubscriber = this.batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: PushNotifications.BatchSizeChangedId,
            name: batchSizeReactorName,
            onReceiveData: data =>
            {
                if (this.firstTimeSettingBatchSize)
                {
                    InitBatchItems(data.BatchSize);
                    return;
                }

                SetNewBatchSize(data.BatchSize, data.TypeOfBatch);
            },
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
        this.rectPullUnsubscriber = rectPullReactable.Subscribe(new RespondReactor<Memory<RenderItem<ShapeBatchItem>>>(
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
    public Span<RenderItem<ShapeBatchItem>> RectItems => this.rectItems.Span;

    /// <summary>
    /// Gets the line items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<LineBatchItem>> LineItems => this.lineItems.Span;

    /// <inheritdoc/>
    public void AddTextureItem(TextureBatchItem item, int layer, DateTime renderStamp)
    {
        var emptyItemIndex = this.textureItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            emptyItemIndex = this.textureItems.Length;

            var newBatchSize = CalcNewBatchSize(BatchType.Texture);
            this.batchSizeReactable.Push(
                new BatchSizeData { BatchSize = newBatchSize, TypeOfBatch = BatchType.Texture },
                PushNotifications.BatchSizeChangedId);
        }

        this.textureItems.Span[emptyItemIndex] = new RenderItem<TextureBatchItem>
        {
            Layer = layer,
            Item = item,
            RenderStamp = renderStamp,
        };
    }

    /// <inheritdoc/>
    public void AddFontItem(FontGlyphBatchItem item, int layer, DateTime renderStamp)
    {
        var emptyItemIndex = this.fontItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            emptyItemIndex = this.fontItems.Length;

            var newBatchSize = CalcNewBatchSize(BatchType.Font);
            this.batchSizeReactable.Push(
                new BatchSizeData { BatchSize = newBatchSize, TypeOfBatch = BatchType.Font },
                PushNotifications.BatchSizeChangedId);
        }

        this.fontItems.Span[emptyItemIndex] = new RenderItem<FontGlyphBatchItem>
        {
            Layer = layer,
            Item = item,
            RenderStamp = renderStamp,
        };
    }

    /// <inheritdoc/>
    public void AddRectItem(ShapeBatchItem item, int layer, DateTime renderStamp)
    {
        var emptyItemIndex = this.rectItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            emptyItemIndex = this.rectItems.Length;

            var newBatchSize = CalcNewBatchSize(BatchType.Rect);
            this.batchSizeReactable.Push(
                new BatchSizeData { BatchSize = newBatchSize, TypeOfBatch = BatchType.Rect },
                PushNotifications.BatchSizeChangedId);
        }

        this.rectItems.Span[emptyItemIndex] = new RenderItem<ShapeBatchItem>
        {
            Layer = layer,
            Item = item,
            RenderStamp = renderStamp,
        };
    }

    /// <inheritdoc/>
    public void AddLineItem(LineBatchItem item, int layer, DateTime renderStamp)
    {
        var emptyItemIndex = this.lineItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            emptyItemIndex = this.lineItems.Length;

            var newBatchSize = CalcNewBatchSize(BatchType.Line);
            this.batchSizeReactable.Push(
                new BatchSizeData { BatchSize = newBatchSize, TypeOfBatch = BatchType.Line },
                PushNotifications.BatchSizeChangedId);
        }

        this.lineItems.Span[emptyItemIndex] = new RenderItem<LineBatchItem>
        {
            Layer = layer,
            Item = item,
            RenderStamp = renderStamp,
        };
    }

    /// <summary>
    /// Sets up all of the batches.
    /// </summary>
    /// <param name="size">The size of each batch.</param>
    private void InitBatchItems(uint size)
    {
        this.textureBatchSize = size;
        this.fontBatchSize = size;
        this.rectBatchSize = size;
        this.lineBatchSize = size;

        this.textureItems = new RenderItem<TextureBatchItem>[size];
        for (var i = 0; i < size; i++)
        {
            this.textureItems.Span[i] = default;
        }

        this.fontItems = new RenderItem<FontGlyphBatchItem>[size];
        for (var i = 0; i < size; i++)
        {
            this.fontItems.Span[i] = default;
        }

        this.rectItems = new RenderItem<ShapeBatchItem>[size];
        for (var i = 0; i < size; i++)
        {
            this.rectItems.Span[i] = default;
        }

        this.lineItems = new RenderItem<LineBatchItem>[size];
        for (var i = 0; i < size; i++)
        {
            this.lineItems.Span[i] = default;
        }

        this.firstTimeSettingBatchSize = false;
    }

    /// <summary>
    /// Calculates the new batch size based on the given <paramref name="batchType"/>.
    /// </summary>
    /// <param name="batchType">The type of batch.</param>
    /// <returns>The new batch size for a particular batch type.</returns>
    /// <exception cref="EnumOutOfRangeException{T}">
    ///     Occurs if the given <paramref name="batchType"/> is an invalid value.
    /// </exception>
    private uint CalcNewBatchSize(BatchType batchType) =>
#pragma warning disable CS8524
        batchType switch
        {
            BatchType.Texture => (uint)(this.textureBatchSize + (this.textureBatchSize * BatchIncreasePercentage)),
            BatchType.Font => (uint)(this.fontBatchSize + (this.fontBatchSize * BatchIncreasePercentage)),
            BatchType.Rect => (uint)(this.rectBatchSize + (this.rectBatchSize * BatchIncreasePercentage)),
            BatchType.Line => (uint)(this.lineBatchSize + (this.lineBatchSize * BatchIncreasePercentage)),
        };
#pragma warning restore CS8524

    /// <summary>
    /// Sets the size of the batch for the given <paramref name="batchType"/> to the given <paramref name="newBatchSize"/>.
    /// </summary>
    /// <param name="newBatchSize">The new batch size.</param>
    /// <param name="batchType">The type of batch.</param>
    /// <exception cref="EnumOutOfRangeException{T}">
    ///     Occurs if the given <paramref name="batchType"/> is an invalid value.
    /// </exception>
    private void SetNewBatchSize(uint newBatchSize, BatchType batchType)
    {
        if (this.batchTypes.Contains(batchType) is false)
        {
            throw new EnumOutOfRangeException<BatchType>(nameof(BatchingManager), nameof(SetNewBatchSize));
        }

#pragma warning disable CS8524
        var increaseAmount = batchType switch
        {
            BatchType.Texture => newBatchSize - this.textureBatchSize,
            BatchType.Font => newBatchSize - this.fontBatchSize,
            BatchType.Rect => newBatchSize - this.rectBatchSize,
            BatchType.Line => newBatchSize - this.lineBatchSize,
        };
#pragma warning restore CS8524

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (batchType)
        {
            case BatchType.Texture:
                this.textureBatchSize = newBatchSize;
                this.textureItems.IncreaseBy(increaseAmount);
                break;
            case BatchType.Font:
                this.fontBatchSize = newBatchSize;
                this.fontItems.IncreaseBy(increaseAmount);
                break;
            case BatchType.Rect:
                this.rectBatchSize = newBatchSize;
                this.rectItems.IncreaseBy(increaseAmount);
                break;
            case BatchType.Line:
                this.lineBatchSize = newBatchSize;
                this.lineItems.IncreaseBy(increaseAmount);
                break;
        }

// ReSharper restore SwitchStatementHandlesSomeKnownEnumValuesWithDefault
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
