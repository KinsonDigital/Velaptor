// <copyright file="RenderMediatorBenchmarks.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RenderMediatorPerf;

using System.Drawing;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using Carbonate;
using Carbonate.NonDirectional;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.WebGpu.Batching;

/// <summary>
/// Runs performance benchmarks against the <see cref="RenderMediator"/> class.
/// </summary>
[MemoryDiagnoser]
public class RenderMediatorBenchmarks
{
    // ReSharper disable once NotAccessedField.Local
    private readonly RenderMediator renderMediator;
    private readonly IDisposable requestTexturesUnsubscriber;
    private readonly IDisposable requestFontsUnsubscriber;
    private readonly IDisposable requestShapesUnsubscriber;
    private readonly IDisposable requestLinesUnsubscriber;
    private readonly IPushReactable coordinateReactable;
    private Memory<RenderItem<TextureBatchItem>> textureItems;
    private Memory<RenderItem<FontGlyphBatchItem>> fontItems;
    private Memory<RenderItem<ShapeBatchItem>> shapeItems;
    private Memory<RenderItem<LineBatchItem>> lineItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorBenchmarks"/> class.
    /// </summary>
    public RenderMediatorBenchmarks()
    {
        var reactableFactory = new ReactableFactoryPerf();
        this.coordinateReactable = reactableFactory.CoordinateReactable;

        // Subscribe to texture batch requests
        var texturePullReactable = reactableFactory.CreateTexturePullBatchReactable();

        this.requestTexturesUnsubscriber = texturePullReactable.CreateOneWayRespond(
            PullResponses.GetTextureItemsId,
            () =>
            {
                var lastFullItemIndex = this.textureItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.textureItems
                    : this.textureItems[..lastFullItemIndex];
            },
            () => this.requestTexturesUnsubscriber?.Dispose());

        // Subscribe to font batch requests
        var fontPullReactable = reactableFactory.CreateFontPullBatchReactable();

        this.requestFontsUnsubscriber = fontPullReactable.CreateOneWayRespond(
            PullResponses.GetFontItemsId,
            () =>
            {
                var lastFullItemIndex = this.fontItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.fontItems
                    : this.fontItems[..lastFullItemIndex];
            },
            () => this.requestFontsUnsubscriber?.Dispose());

        // Subscribe to shape batch requests
        var shapePullReactable = reactableFactory.CreateShapePullBatchReactable();

        this.requestShapesUnsubscriber = shapePullReactable.CreateOneWayRespond(
            PullResponses.GetShapeItemsId,
            () =>
            {
                var lastFullItemIndex = this.shapeItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.shapeItems
                    : this.shapeItems[..lastFullItemIndex];
            },
            () => this.requestShapesUnsubscriber?.Dispose());

        // Subscribe to line batch requests
        var linePullReactable = reactableFactory.CreateLinePullBatchReactable();

        this.requestLinesUnsubscriber = linePullReactable.CreateOneWayRespond(
            PullResponses.GetLineItemsId,
            () =>
            {
                var lastFullItemIndex = this.lineItems.IndexOf(i => i.IsEmpty());

                return lastFullItemIndex < 0
                    ? this.lineItems
                    : this.lineItems[..lastFullItemIndex];
            },
            () => this.requestLinesUnsubscriber?.Dispose());

        var textureItemComparer = new RenderItemComparer<TextureBatchItem>();
        var fontItemComparer = new RenderItemComparer<FontGlyphBatchItem>();
        var shapeItemComparer = new RenderItemComparer<ShapeBatchItem>();
        var lineItemComparer = new RenderItemComparer<LineBatchItem>();

        this.renderMediator = new RenderMediator(reactableFactory, textureItemComparer, fontItemComparer, shapeItemComparer, lineItemComparer);
    }

    [Params(500_000)]
    public int BatchSize { get; set; }

    /// <summary>
    /// Sets up the test for each iteration.
    /// </summary>
    [IterationSetup]
    public void IterationSetup()
    {
        this.textureItems = new RenderItem<TextureBatchItem>[BatchSize];
        for (var i = 0; i < BatchSize; i++)
        {
            this.textureItems.Span[i] = new RenderItem<TextureBatchItem>
            {
                Item = new TextureBatchItem(
                    new RectangleF(11, 22, 33, 44),
                    new RectangleF(44, 55, 66, 77),
                    88,
                    99,
                    Color.FromArgb(101, 102, 103, 104),
                    RenderEffects.None,
                    555),
            };
        }

        this.fontItems = new RenderItem<FontGlyphBatchItem>[BatchSize];
        for (var i = 0; i < BatchSize; i++)
        {
            this.fontItems.Span[i] = new RenderItem<FontGlyphBatchItem>
            {
                Item = new FontGlyphBatchItem(
                    new RectangleF(11, 22, 33, 44),
                    new RectangleF(44, 55, 66, 77),
                    'v',
                    88,
                    99,
                    Color.FromArgb(101, 102, 103, 104),
                    RenderEffects.None,
                    555),
            };
        }

        this.shapeItems = new RenderItem<ShapeBatchItem>[BatchSize];
        for (var i = 0; i < BatchSize; i++)
        {
            this.shapeItems.Span[i] = new RenderItem<ShapeBatchItem>
            {
                Item = new ShapeBatchItem(new Vector2(11, 22),
                    33,
                    44,
                    Color.FromArgb(55, 66, 77, 88),
                    true,
                    99,
                    CornerRadius.Empty(),
                    ColorGradient.None,
                    Color.FromArgb(101, 102, 103, 104),
                    Color.FromArgb(105, 106, 107, 108)),
            };
        }

        this.lineItems = new RenderItem<LineBatchItem>[BatchSize];
        for (var i = 0; i < BatchSize; i++)
        {
            this.lineItems.Span[i] = new RenderItem<LineBatchItem>
            {
                Item = new LineBatchItem(new Vector2(11, 22),
                    new Vector2(33, 44),
                    Color.FromArgb(101, 102, 103, 104),
                    99),
            };
        }
    }

    /// <summary>
    /// Cleans up the test for each iteration.
    /// </summary>
    [IterationCleanup]
    public void IterationCleanup()
    {
        this.textureItems = null;
        this.fontItems = null;
        this.shapeItems = null;
        this.lineItems = null;
    }

    /// <summary>
    /// Runs the performance check.
    /// </summary>
    [Benchmark(Description = "Coordinate Renders")]
    public void CoordinateRenders() => this.coordinateReactable.Push(PushNotifications.BatchHasEndedId);
}
