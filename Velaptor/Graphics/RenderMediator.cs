// <copyright file="RenderMediator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Collections.Generic;
using Batching;
using Carbonate.Fluent;
using Carbonate.NonDirectional;
using Factories;
using OpenGL.Batching;

/// <inheritdoc/>
internal sealed class RenderMediator : IRenderMediator
{
    private readonly IPushReactable endBatchReactable;
    private readonly IBatchPullReactable<TextureBatchItem> texturePullReactable;
    private readonly IBatchPullReactable<FontGlyphBatchItem> fontPullReactable;
    private readonly IBatchPullReactable<ShapeBatchItem> shapePullReactable;
    private readonly IBatchPullReactable<LineBatchItem> linePullReactable;
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

        this.endBatchReactable = reactableFactory.CreateNoDataPushReactable();

        var endBatchSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchHasEndedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchHasEndedId)))
            .BuildNonReceiveOrRespond(CoordinateRenders);

        this.endBatchReactable.Subscribe(endBatchSubscription);

        this.texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();
        this.fontPullReactable = reactableFactory.CreateFontPullBatchReactable();
        this.shapePullReactable = reactableFactory.CreateShapePullBatchReactable();
        this.linePullReactable = reactableFactory.CreateLinePullBatchReactable();

        this.textureRenderBatchReactable = reactableFactory.CreateRenderTextureReactable();
        this.fontRenderBatchReactable = reactableFactory.CreateRenderFontReactable();
        this.shapeRenderBatchReactable = reactableFactory.CreateRenderShapeReactable();
        this.lineRenderBatchReactable = reactableFactory.CreateRenderLineReactable();

        this.textureItemComparer = textureItemComparer;
        this.fontItemComparer = fontItemComparer;
        this.shapeItemComparer = shapeItemComparer;
        this.lineItemComparer = lineItemComparer;

        // Sets all of the layers to a default value of the max value of int
        for (var i = 0; i < this.allLayers.Length; i++)
        {
            this.allLayers.Span[i] = int.MaxValue;
        }
    }

    /// <summary>
    /// Coordinates the rendering between each of the renderers.
    /// </summary>
    private void CoordinateRenders()
    {
        var textureItems = this.texturePullReactable.Pull(PullResponses.GetTextureItemsId);
        var fontItems = this.fontPullReactable.Pull(PullResponses.GetFontItemsId);
        var shapeItems = this.shapePullReactable.Pull(PullResponses.GetShapeItemsId);
        var lineItems = this.linePullReactable.Pull(PullResponses.GetLineItemsId);

        textureItems.Span.Sort(this.textureItemComparer);
        fontItems.Span.Sort(this.fontItemComparer);
        shapeItems.Span.Sort(this.shapeItemComparer);
        lineItems.Span.Sort(this.lineItemComparer);

        var layerIndex = 0;

        for (var i = 0; i < textureItems.Length; i++)
        {
            var textureLayer = textureItems.Span[i].Layer;
            if (this.allLayers.Span.Contains(textureLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = textureLayer;
            layerIndex++;
        }

        for (var i = 0; i < fontItems.Length; i++)
        {
            var fontLayer = fontItems.Span[i].Layer;
            if (this.allLayers.Span.Contains(fontLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = fontLayer;
            layerIndex++;
        }

        for (var i = 0; i < shapeItems.Length; i++)
        {
            var shapeLayer = shapeItems.Span[i].Layer;
            if (this.allLayers.Span.Contains(shapeLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = shapeLayer;
            layerIndex++;
        }

        for (var i = 0; i < lineItems.Length; i++)
        {
            var lineLayer = lineItems.Span[i].Layer;
            if (this.allLayers.Span.Contains(lineLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = lineLayer;
            layerIndex++;
        }

        this.allLayers.Span.Sort();

        // Renders all of the items in a coordinated fashion
        for (var i = 0; i < this.allLayers.Length; i++)
        {
            if (this.allLayers.Span[i] == int.MaxValue)
            {
                break;
            }

            var currentLayer = this.allLayers.Span[i];

            var totalTexturesOnCurrentLayer = textureItems.TotalOnLayer(currentLayer);
            var totalFontOnCurrentLayer = fontItems.TotalOnLayer(currentLayer);
            var totalShapesOnCurrentLayer = shapeItems.TotalOnLayer(currentLayer);
            var totalLinesOnCurrentLayer = lineItems.TotalOnLayer(currentLayer);

            if (totalTexturesOnCurrentLayer > 0)
            {
                var textureLayerStart = textureItems.FirstLayerIndex(currentLayer);

                this.textureRenderBatchReactable.Push(
                    PushNotifications.RenderTexturesId,
                    textureItems.Slice(textureLayerStart, totalTexturesOnCurrentLayer));
            }

            if (totalFontOnCurrentLayer > 0)
            {
                var fontLayerStart = fontItems.FirstLayerIndex(currentLayer);

                this.fontRenderBatchReactable.Push(
                    PushNotifications.RenderFontsId,
                    fontItems.Slice(fontLayerStart, totalFontOnCurrentLayer));
            }

            if (totalShapesOnCurrentLayer > 0)
            {
                var shapeLayerStart = shapeItems.FirstLayerIndex(currentLayer);

                this.shapeRenderBatchReactable.Push(
                    PushNotifications.RenderShapesId,
                    shapeItems.Slice(shapeLayerStart, totalShapesOnCurrentLayer));
            }

            if (totalLinesOnCurrentLayer > 0)
            {
                var lineLayerStart = lineItems.FirstLayerIndex(currentLayer);

                this.lineRenderBatchReactable.Push(
                    PushNotifications.RenderLinesId,
                    lineItems.Slice(lineLayerStart, totalLinesOnCurrentLayer));
            }

            // Resets the item back to the default value
            this.allLayers.Span[i] = int.MaxValue;
        }

        this.endBatchReactable.Push(PushNotifications.EmptyBatchId);
    }
}
