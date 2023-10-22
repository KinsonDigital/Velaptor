// <copyright file="BatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using System.ComponentModel;
using System.Linq;
using Carbonate.Fluent;
using Carbonate.OneWay;
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
    private readonly IDisposable requestTexturesUnsubscriber;
    private readonly IDisposable requestFontsUnsubscriber;
    private readonly IDisposable requestShapesUnsubscriber;
    private readonly IDisposable requestLinesUnsubscriber;
    private readonly IDisposable emptyBatchUnsubscriber;
    private readonly IPushReactable<BatchSizeData> batchSizeReactable;
    private Memory<RenderItem<TextureBatchItem>> textureItems;
    private Memory<RenderItem<FontGlyphBatchItem>> fontItems;
    private Memory<RenderItem<ShapeBatchItem>> shapeItems;
    private Memory<RenderItem<LineBatchItem>> lineItems;
    private bool isShutDown;
    private bool firstTimeSettingBatchSize = true;
    private uint textureBatchSize;
    private uint fontBatchSize;
    private uint shapeBatchSize;
    private uint lineBatchSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingManager"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactable objects.</param>
    public BatchingManager(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        // Subscribe to batch size changes
        this.batchSizeReactable = reactableFactory.CreateBatchSizeReactable();
        var batchSizeSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchSizeChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId)))
            .WhenUnsubscribing(() => this.batchSizeUnsubscriber?.Dispose())
            .BuildOneWayReceive<BatchSizeData>(data =>
            {
                if (this.firstTimeSettingBatchSize)
                {
                    InitBatchItems(data.BatchSize);
                    return;
                }

                SetNewBatchSize(data.BatchSize, data.TypeOfBatch);
            });

        this.batchSizeUnsubscriber = this.batchSizeReactable.Subscribe(batchSizeSubscription);

        var signalReactable = reactableFactory.CreateNoDataPushReactable();

        // Subscribe to shutdown messages
        var shutDownSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.SystemShuttingDownId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.SystemShuttingDownId)))
            .BuildNonReceive(ShutDown);

        this.shutDownUnsubscriber = signalReactable.Subscribe(shutDownSubscription);

        // Subscribe to empty batch messages
        var emptyBatchSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.EmptyBatchId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.EmptyBatchId)))
            .BuildNonReceive(EmptyBatch);

        this.emptyBatchUnsubscriber = signalReactable.Subscribe(emptyBatchSubscription);

        // Subscribe to texture batch requests
        var texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();
        var textureRequestSubscription = ISubscriptionBuilder.Create()
            .WithId(PullResponses.GetTextureItemsId)
            .WithName(this.GetExecutionMemberName(nameof(PullResponses.GetTextureItemsId)))
            .BuildOneWayRespond(() =>
            {
                var lastFullItemIndex = this.textureItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.textureItems
                    : this.textureItems[..lastFullItemIndex];
            });

        this.requestTexturesUnsubscriber = texturePullReactable.Subscribe(textureRequestSubscription);

        // Subscribe to font batch requests
        var fontPullReactable = reactableFactory.CreateFontPullBatchReactable();
        var fontRequestSubscription = ISubscriptionBuilder.Create()
            .WithId(PullResponses.GetFontItemsId)
            .WithName(this.GetExecutionMemberName(nameof(PullResponses.GetFontItemsId)))
            .BuildOneWayRespond(() =>
            {
                var lastFullItemIndex = this.fontItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.fontItems
                    : this.fontItems[..lastFullItemIndex];
            });

        this.requestFontsUnsubscriber = fontPullReactable.Subscribe(fontRequestSubscription);

        // Subscribe to shape batch requests
        var shapePullReactable = reactableFactory.CreateShapePullBatchReactable();
        var shapeRequestSubscription = ISubscriptionBuilder.Create()
            .WithId(PullResponses.GetShapeItemsId)
            .WithName(this.GetExecutionMemberName(nameof(PullResponses.GetShapeItemsId)))
            .BuildOneWayRespond(() =>
            {
                var lastFullItemIndex = this.shapeItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.shapeItems
                    : this.shapeItems[..lastFullItemIndex];
            });

        this.requestShapesUnsubscriber = shapePullReactable.Subscribe(shapeRequestSubscription);

        // Subscribe to line batch requests
        var linePullReactable = reactableFactory.CreateLinePullBatchReactable();
        var lineRequestSubscription = ISubscriptionBuilder.Create()
            .WithId(PullResponses.GetLineItemsId)
            .WithName(this.GetExecutionMemberName(nameof(PullResponses.GetLineItemsId)))
            .BuildOneWayRespond(() =>
            {
                var lastFullItemIndex = this.lineItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.lineItems
                    : this.lineItems[..lastFullItemIndex];
            });

        this.requestLinesUnsubscriber = linePullReactable.Subscribe(lineRequestSubscription);
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
    /// Gets the shape items.
    /// </summary>
    /// <remarks>USED FOR UNIT TESTING.</remarks>
    public Span<RenderItem<ShapeBatchItem>> ShapeItems => this.shapeItems.Span;

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
    public void AddShapeItem(ShapeBatchItem item, int layer, DateTime renderStamp)
    {
        var emptyItemIndex = this.shapeItems.
            FirstItemIndex(i => i.Item.IsEmpty());

        if (emptyItemIndex == -1)
        {
            emptyItemIndex = this.shapeItems.Length;

            var newBatchSize = CalcNewBatchSize(BatchType.Rect);
            this.batchSizeReactable.Push(
                new BatchSizeData { BatchSize = newBatchSize, TypeOfBatch = BatchType.Rect },
                PushNotifications.BatchSizeChangedId);
        }

        this.shapeItems.Span[emptyItemIndex] = new RenderItem<ShapeBatchItem>
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
        this.shapeBatchSize = size;
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

        this.shapeItems = new RenderItem<ShapeBatchItem>[size];
        for (var i = 0; i < size; i++)
        {
            this.shapeItems.Span[i] = default;
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
    /// <exception cref="InvalidEnumArgumentException">
    ///     Occurs if the given <paramref name="batchType"/> is an invalid value.
    /// </exception>
    private uint CalcNewBatchSize(BatchType batchType) =>
        batchType switch
        {
            BatchType.Texture => (uint)(this.textureBatchSize + (this.textureBatchSize * BatchIncreasePercentage)),
            BatchType.Font => (uint)(this.fontBatchSize + (this.fontBatchSize * BatchIncreasePercentage)),
            BatchType.Rect => (uint)(this.shapeBatchSize + (this.shapeBatchSize * BatchIncreasePercentage)),
            BatchType.Line => (uint)(this.lineBatchSize + (this.lineBatchSize * BatchIncreasePercentage)),
            _ => throw new InvalidEnumArgumentException(nameof(batchType), (int)batchType, typeof(BatchType))
        };

    /// <summary>
    /// Sets the size of the batch for the given <paramref name="batchType"/> to the given <paramref name="newBatchSize"/>.
    /// </summary>
    /// <param name="newBatchSize">The new batch size.</param>
    /// <param name="batchType">The type of batch.</param>
    /// <exception cref="InvalidEnumArgumentException">
    ///     Occurs if the given <paramref name="batchType"/> is an invalid value.
    /// </exception>
    private void SetNewBatchSize(uint newBatchSize, BatchType batchType)
    {
        var increaseAmount = batchType switch
        {
            BatchType.Texture => newBatchSize - this.textureBatchSize,
            BatchType.Font => newBatchSize - this.fontBatchSize,
            BatchType.Rect => newBatchSize - this.shapeBatchSize,
            BatchType.Line => newBatchSize - this.lineBatchSize,
            _ => throw new InvalidEnumArgumentException(nameof(batchType), (int)batchType, typeof(BatchType))
        };

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
                this.shapeBatchSize = newBatchSize;
                this.shapeItems.IncreaseBy(increaseAmount);
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

        for (var i = 0; i < this.shapeItems.Length; i++)
        {
            if (this.shapeItems.Span[i].Item.IsEmpty())
            {
                break;
            }

            this.shapeItems.Span[i] = default;
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
        this.requestTexturesUnsubscriber.Dispose();
        this.requestFontsUnsubscriber.Dispose();
        this.requestShapesUnsubscriber.Dispose();
        this.requestLinesUnsubscriber.Dispose();

        this.isShutDown = true;
    }
}
