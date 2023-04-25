// <copyright file="RenderMediator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Collections.Generic;
using Batching;
using Carbonate.NonDirectional;
using Factories;
using Guards;
using OpenGL.Batching;

/// <inheritdoc/>
internal sealed class RenderMediator : IRenderMediator
{
    private readonly IPushReactable pushReactable;
    private readonly IBatchPullReactable<TextureBatchItem> texturePullReactable;
    private readonly IBatchPullReactable<FontGlyphBatchItem> fontPullReactable;
    private readonly IBatchPullReactable<RectEllipseBatchItem> rectPullReactable;
    private readonly IBatchPullReactable<LineBatchItem> linePullReactable;
    private readonly IRenderBatchReactable<TextureBatchItem> textureRenderBatchReactable;
    private readonly IRenderBatchReactable<FontGlyphBatchItem> fontRenderBatchReactable;
    private readonly IRenderBatchReactable<RectEllipseBatchItem> rectRenderBatchReactable;
    private readonly IRenderBatchReactable<LineBatchItem> lineRenderBatchReactable;
    private readonly IComparer<RenderItem<TextureBatchItem>> textureItemComparer;
    private readonly IComparer<RenderItem<FontGlyphBatchItem>> fontItemComparer;
    private readonly IComparer<RenderItem<RectEllipseBatchItem>> rectItemComparer;
    private readonly IComparer<RenderItem<LineBatchItem>> lineItemComparer;
    private readonly IDisposable endBatchUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;

    // The total amount of layers supported
    private readonly Memory<int> allLayers = new (new int[1000]);

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediator"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="textureItemComparer">Compares two texture batch items for the purpose of sorting.</param>
    /// <param name="fontItemComparer">Compares two font batch items for the purpose of sorting.</param>
    /// <param name="rectItemComparer">Compares two rect batch items for the purpose of sorting.</param>
    /// <param name="lineItemComparer">Compares two line batch items for the purpose of sorting.</param>
    public RenderMediator(
        IReactableFactory reactableFactory,
        IComparer<RenderItem<TextureBatchItem>> textureItemComparer,
        IComparer<RenderItem<FontGlyphBatchItem>> fontItemComparer,
        IComparer<RenderItem<RectEllipseBatchItem>> rectItemComparer,
        IComparer<RenderItem<LineBatchItem>> lineItemComparer)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);
        EnsureThat.ParamIsNotNull(textureItemComparer);
        EnsureThat.ParamIsNotNull(fontItemComparer);
        EnsureThat.ParamIsNotNull(rectItemComparer);
        EnsureThat.ParamIsNotNull(lineItemComparer);

        this.pushReactable = reactableFactory.CreateNoDataPushReactable();

        var batchEndName = this.GetExecutionMemberName(nameof(PushNotifications.BatchHasEndedId));
        this.endBatchUnsubscriber = this.pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.BatchHasEndedId,
            name: batchEndName,
            onReceive: CoordinateRenders));

        var shutDownName = this.GetExecutionMemberName(nameof(PushNotifications.SystemShuttingDownId));
        this.shutDownUnsubscriber = this.pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.SystemShuttingDownId,
            name: shutDownName,
            onReceive: ShutDown));

        this.texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();
        this.fontPullReactable = reactableFactory.CreateFontPullBatchReactable();
        this.rectPullReactable = reactableFactory.CreateRectPullBatchReactable();
        this.linePullReactable = reactableFactory.CreateLinePullBatchReactable();

        this.textureRenderBatchReactable = reactableFactory.CreateRenderTextureReactable();
        this.fontRenderBatchReactable = reactableFactory.CreateRenderFontReactable();
        this.rectRenderBatchReactable = reactableFactory.CreateRenderRectReactable();
        this.lineRenderBatchReactable = reactableFactory.CreateRenderLineReactable();

        this.textureItemComparer = textureItemComparer;
        this.fontItemComparer = fontItemComparer;
        this.rectItemComparer = rectItemComparer;
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
        var rectItems = this.rectPullReactable.Pull(PullResponses.GetRectItemsId);
        var lineItems = this.linePullReactable.Pull(PullResponses.GetLineItemsId);

        textureItems.Span.Sort(this.textureItemComparer);
        fontItems.Span.Sort(this.fontItemComparer);
        rectItems.Span.Sort(this.rectItemComparer);
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

        for (var i = 0; i < rectItems.Length; i++)
        {
            var rectLayer = rectItems.Span[i].Layer;
            if (this.allLayers.Span.Contains(rectLayer))
            {
                continue;
            }

            this.allLayers.Span[layerIndex] = rectLayer;
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
            var totalRectsOnCurrentLayer = rectItems.TotalOnLayer(currentLayer);
            var totalLinesOnCurrentLayer = lineItems.TotalOnLayer(currentLayer);

            if (totalTexturesOnCurrentLayer > 0)
            {
                var textureLayerStart = textureItems.FirstLayerIndex(currentLayer);

                this.textureRenderBatchReactable.Push(
                    textureItems.Slice(textureLayerStart, totalTexturesOnCurrentLayer),
                    PushNotifications.RenderTexturesId);
            }

            if (totalFontOnCurrentLayer > 0)
            {
                var fontLayerStart = fontItems.FirstLayerIndex(currentLayer);

                this.fontRenderBatchReactable.Push(
                    fontItems.Slice(fontLayerStart, totalFontOnCurrentLayer),
                    PushNotifications.RenderFontsId);
            }

            if (totalRectsOnCurrentLayer > 0)
            {
                var rectLayerStart = rectItems.FirstLayerIndex(currentLayer);

                this.rectRenderBatchReactable.Push(
                    rectItems.Slice(rectLayerStart, totalRectsOnCurrentLayer),
                    PushNotifications.RenderRectsId);
            }

            if (totalLinesOnCurrentLayer > 0)
            {
                var lineLayerStart = lineItems.FirstLayerIndex(currentLayer);

                this.lineRenderBatchReactable.Push(
                    lineItems.Slice(lineLayerStart, totalLinesOnCurrentLayer),
                    PushNotifications.RenderLinesId);
            }

            // Resets the item back to the default value
            this.allLayers.Span[i] = int.MaxValue;
        }

        this.pushReactable.Push(PushNotifications.EmptyBatchId);
    }

    /// <summary>
    /// Shuts down the <see cref="RenderMediator"/>.
    /// </summary>
    private void ShutDown()
    {
        this.endBatchUnsubscriber.Dispose();
        this.shutDownUnsubscriber.Dispose();
    }
}
