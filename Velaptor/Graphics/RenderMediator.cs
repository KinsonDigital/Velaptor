﻿// <copyright file="RenderMediator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Collections.Generic;
using Batching;
using Carbonate;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Factories;
using ReactableData;
using WebGpu.Batching;

/// <inheritdoc/>
internal sealed class RenderMediator : IRenderMediator
{
    private readonly IPushReactable coordinateReactable;
    private readonly IBatchPullReactable<TextureBatchItem> texturePullReactable;
    private readonly IBatchPullReactable<FontGlyphBatchItem> fontPullReactable;
    private readonly IBatchPullReactable<ShapeBatchItem> shapePullReactable;
    private readonly IBatchPullReactable<LineBatchItem> linePullReactable;
    private readonly IPushReactable<RequiredBufferCapacityData> bufferResizeReactable;
    private readonly IRenderBatchReactable<TextureBatchItem> textureRenderBatchReactable;
    private readonly IRenderBatchReactable<FontGlyphBatchItem> fontRenderBatchReactable;
    private readonly IRenderBatchReactable<ShapeBatchItem> shapeRenderBatchReactable;
    private readonly IRenderBatchReactable<LineBatchItem> lineRenderBatchReactable;
    private readonly IComparer<RenderItem<TextureBatchItem>> textureItemComparer;
    private readonly IComparer<RenderItem<FontGlyphBatchItem>> fontItemComparer;
    private readonly IComparer<RenderItem<ShapeBatchItem>> shapeItemComparer;
    private readonly IComparer<RenderItem<LineBatchItem>> lineItemComparer;

    // The total amount of layers supported
    private readonly Memory<int> allLayers = new (new int[1000]);

    // Holds all the layers used for the current frame for quick lookups to stay performant
    // during the layer processing and sorting phase
    private readonly HashSet<int> usedLayers = [];

    private readonly IDisposable endBatchUnsubscriber;
    private RequiredBufferCapacityData bufferCapacityData;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediator"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="textureItemComparer">Compares two texture batch items for the purpose of sorting.</param>
    /// <param name="fontItemComparer">Compares two font batch items for the purpose of sorting.</param>
    /// <param name="shapeItemComparer">Compares two shape batch items for the purpose of sorting.</param>
    /// <param name="lineItemComparer">Compares two line batch items for the purpose of sorting.</param>
    public RenderMediator(
        IReactableFactory reactableFactory,
        IComparer<RenderItem<TextureBatchItem>> textureItemComparer,
        IComparer<RenderItem<FontGlyphBatchItem>> fontItemComparer,
        IComparer<RenderItem<ShapeBatchItem>> shapeItemComparer,
        IComparer<RenderItem<LineBatchItem>> lineItemComparer)
    {
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(textureItemComparer);
        ArgumentNullException.ThrowIfNull(fontItemComparer);
        ArgumentNullException.ThrowIfNull(shapeItemComparer);
        ArgumentNullException.ThrowIfNull(lineItemComparer);

        this.coordinateReactable = reactableFactory.CreateNoDataPushReactable();

        this.endBatchUnsubscriber = this.coordinateReactable.CreateNonReceiveOrRespond(
            PushNotifications.BatchHasEndedId,
            CoordinateRenders,
            () => this.endBatchUnsubscriber?.Dispose());

        this.texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();
        this.fontPullReactable = reactableFactory.CreateFontPullBatchReactable();
        this.shapePullReactable = reactableFactory.CreateShapePullBatchReactable();
        this.linePullReactable = reactableFactory.CreateLinePullBatchReactable();

        this.bufferResizeReactable = reactableFactory.CreateResizeBufferReactable();
        this.textureRenderBatchReactable = reactableFactory.CreateRenderTextureReactable();
        this.fontRenderBatchReactable = reactableFactory.CreateRenderFontReactable();
        this.shapeRenderBatchReactable = reactableFactory.CreateRenderShapeReactable();
        this.lineRenderBatchReactable = reactableFactory.CreateRenderLineReactable();

        this.textureItemComparer = textureItemComparer;
        this.fontItemComparer = fontItemComparer;
        this.shapeItemComparer = shapeItemComparer;
        this.lineItemComparer = lineItemComparer;

        // Sets all the layers to a default value of the max value of int
        for (var i = 0; i < this.allLayers.Length; i++)
        {
            this.allLayers.Span[i] = int.MaxValue;
        }
    }

    /// <summary>
    /// Coordinates the renders between all the different layers of each type of thing to render.
    /// </summary>
    private void CoordinateRenders()
    {
        var textureItems = this.texturePullReactable.Pull(PullResponses.GetTextureItemsId);
        var fontItems = this.fontPullReactable.Pull(PullResponses.GetFontItemsId);
        var shapeItems = this.shapePullReactable.Pull(PullResponses.GetShapeItemsId);
        var lineItems = this.linePullReactable.Pull(PullResponses.GetLineItemsId);

        this.bufferCapacityData.TotalTextureItems = (uint)textureItems.Length;
        this.bufferCapacityData.TotalFontItems = (uint)fontItems.Length;
        this.bufferCapacityData.TotalShapeItems = (uint)shapeItems.Length;
        this.bufferCapacityData.TotalLineItems = (uint)lineItems.Length;

        // Resize the buffers before drawing any calls
        this.bufferResizeReactable.Push(PushNotifications.ResizeBufferId, this.bufferCapacityData);

        // Sort all the item layers
        textureItems.Span.Sort(this.textureItemComparer);
        fontItems.Span.Sort(this.fontItemComparer);
        shapeItems.Span.Sort(this.shapeItemComparer);
        lineItems.Span.Sort(this.lineItemComparer);

        var layerIndex = 0;
        this.usedLayers.Clear();

        var textureItemsSpan = textureItems.Span;

        // Collect all existing layers that exist before sorting them from the farthest back to the front
        for (var i = 0; i < textureItems.Length; i++)
        {
            var textureLayer = textureItemsSpan[i].Layer;
            if (!this.usedLayers.Add(textureLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = textureLayer;
            layerIndex++;
        }

        var fontItemsSpan = fontItems.Span;

        for (var i = 0; i < fontItems.Length; i++)
        {
            var fontLayer = fontItemsSpan[i].Layer;
            if (!this.usedLayers.Add(fontLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = fontLayer;
            layerIndex++;
        }

        var shapeItemsSpan = shapeItems.Span;

        for (var i = 0; i < shapeItems.Length; i++)
        {
            var shapeLayer = shapeItemsSpan[i].Layer;
            if (!this.usedLayers.Add(shapeLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = shapeLayer;
            layerIndex++;
        }

        var lineItemsSpan = lineItems.Span;

        for (var i = 0; i < lineItems.Length; i++)
        {
            var lineLayer = lineItemsSpan[i].Layer;
            if (!this.usedLayers.Add(lineLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = lineLayer;
            layerIndex++;
        }

        // Sort all the existing layers from the farthest back to the front.
        // this.allLayers.Span.Sort();
        var actualLayerCount = layerIndex;
        this.allLayers.Span[..actualLayerCount].Sort();

        var allLayersSpan = this.allLayers.Span;

        // Renders all the items in a coordinated fashion
        for (var i = 0; i < actualLayerCount; i++)
        {
            var currentLayer = allLayersSpan[i];

            var totalTexturesOnCurrentLayer = textureItems.TotalOnLayer(currentLayer);
            var totalFontOnCurrentLayer = fontItems.TotalOnLayer(currentLayer);
            var totalShapesOnCurrentLayer = shapeItems.TotalOnLayer(currentLayer);
            var totalLinesOnCurrentLayer = lineItems.TotalOnLayer(currentLayer);

            if (totalTexturesOnCurrentLayer > 0)
            {
                var textureLayerStartIndex = textureItems.FirstLayerIndex(currentLayer);

                this.textureRenderBatchReactable.Push(
                    PushNotifications.RenderTexturesId,
                    textureItems.Slice(textureLayerStartIndex, totalTexturesOnCurrentLayer));
            }

            if (totalFontOnCurrentLayer > 0)
            {
                var fontLayerStartIndex = fontItems.FirstLayerIndex(currentLayer);

                this.fontRenderBatchReactable.Push(
                    PushNotifications.RenderFontsId,
                    fontItems.Slice(fontLayerStartIndex, totalFontOnCurrentLayer));
            }

            if (totalShapesOnCurrentLayer > 0)
            {
                var shapeLayerStartIndex = shapeItems.FirstLayerIndex(currentLayer);

                this.shapeRenderBatchReactable.Push(
                    PushNotifications.RenderShapesId,
                    shapeItems.Slice(shapeLayerStartIndex, totalShapesOnCurrentLayer));
            }

            if (totalLinesOnCurrentLayer > 0)
            {
                var lineLayerStartIndex = lineItems.FirstLayerIndex(currentLayer);

                this.lineRenderBatchReactable.Push(
                    PushNotifications.RenderLinesId,
                    lineItems.Slice(lineLayerStartIndex, totalLinesOnCurrentLayer));
            }

            // Resets the item back to the default value
            this.allLayers.Span[i] = int.MaxValue;
        }

        this.coordinateReactable.Push(PushNotifications.EmptyBatchId);
    }
}
