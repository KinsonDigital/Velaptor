// <copyright file="ReactableFactoryPerf.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RenderMediatorPerf;

using System.Diagnostics.CodeAnalysis;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.ReactableData;
using Velaptor.WebGpu.Batching;

/// <summary>
/// Creates singleton reactables.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal class ReactableFactoryPerf : IReactableFactory
{
    /// <summary>
    /// Gets the shared no-data push reactable used to coordinate batch events.
    /// </summary>
    public IPushReactable CoordinateReactable { get; } = new PushReactable();

    /// <summary>
    /// Gets the shared reactable used to request buffer resizes.
    /// </summary>
    public IPushReactable<RequiredBufferCapacityData> BufferResizeReactable { get; } =
        new PushReactable<RequiredBufferCapacityData>();

    /// <summary>
    /// Gets the shared reactable used to pull texture batch items.
    /// </summary>
    public IBatchPullReactable<TextureBatchItem> TexturePullReactable { get; } =
        new BatchPullReactable<TextureBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to pull font batch items.
    /// </summary>
    public IBatchPullReactable<FontGlyphBatchItem> FontPullReactable { get; } =
        new BatchPullReactable<FontGlyphBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to pull shape batch items.
    /// </summary>
    public IBatchPullReactable<ShapeBatchItem> ShapePullReactable { get; } =
        new BatchPullReactable<ShapeBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to pull line batch items.
    /// </summary>
    public IBatchPullReactable<LineBatchItem> LinePullReactable { get; } =
        new BatchPullReactable<LineBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to render texture batch items.
    /// </summary>
    public IRenderBatchReactable<TextureBatchItem> RenderTextureReactable { get; } =
        new RenderBatchReactable<TextureBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to render font batch items.
    /// </summary>
    public IRenderBatchReactable<FontGlyphBatchItem> RenderFontReactable { get; } =
        new RenderBatchReactable<FontGlyphBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to render shape batch items.
    /// </summary>
    public IRenderBatchReactable<ShapeBatchItem> RenderShapeReactable { get; } =
        new RenderBatchReactable<ShapeBatchItem>();

    /// <summary>
    /// Gets the shared reactable used to render line batch items.
    /// </summary>
    public IRenderBatchReactable<LineBatchItem> RenderLineReactable { get; } =
        new RenderBatchReactable<LineBatchItem>();

    /// <inheritdoc/>
    public IPushReactable CreateNoDataPushReactable() =>
        CoordinateReactable;

    /// <inheritdoc/>
    public IPushReactable<RequiredBufferCapacityData> CreateResizeBufferReactable() =>
        BufferResizeReactable;

    /// <inheritdoc/>
    public IBatchPullReactable<TextureBatchItem> CreateTexturePullBatchReactable() =>
        TexturePullReactable;

    /// <inheritdoc/>
    public IBatchPullReactable<FontGlyphBatchItem> CreateFontPullBatchReactable() =>
        FontPullReactable;

    /// <inheritdoc/>
    public IBatchPullReactable<ShapeBatchItem> CreateShapePullBatchReactable() =>
        ShapePullReactable;

    /// <inheritdoc/>
    public IBatchPullReactable<LineBatchItem> CreateLinePullBatchReactable() =>
        LinePullReactable;

    /// <inheritdoc/>
    public IRenderBatchReactable<TextureBatchItem> CreateRenderTextureReactable() =>
        RenderTextureReactable;

    /// <inheritdoc/>
    public IRenderBatchReactable<FontGlyphBatchItem> CreateRenderFontReactable() =>
        RenderFontReactable;

    /// <inheritdoc/>
    public IRenderBatchReactable<ShapeBatchItem> CreateRenderShapeReactable() =>
        RenderShapeReactable;

    /// <inheritdoc/>
    public IRenderBatchReactable<LineBatchItem> CreateRenderLineReactable() =>
        RenderLineReactable;

    /// <inheritdoc/>
    public IPushReactable<BatchSizeData> CreateBatchSizeReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<WindowSizeData> CreatePushWindowSizeReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPullReactable<WindowSizeData> CreatePullWindowSizeReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<ViewPortSizeData> CreateViewPortReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<MouseStateData> CreateMouseReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<KeyboardKeyStateData> CreateKeyboardReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<DisposeTextureData> CreateDisposeTextureReactable() =>
        throw new NotImplementedException("Not needed for perf testing");

    /// <inheritdoc/>
    public IPushReactable<DisposeAudioData> CreateDisposeAudioReactable() =>
        throw new NotImplementedException("Not needed for perf testing");
}
