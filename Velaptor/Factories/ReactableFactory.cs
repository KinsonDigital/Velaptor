// <copyright file="ReactableFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Graphics;
using OpenGL.Batching;
using ReactableData;
using Silk.NET.OpenGL;

/// <summary>
/// Creates singleton reactables.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal class ReactableFactory : IReactableFactory
{
    /// <inheritdoc/>
    public IPushReactable CreateNoDataPushReactable() => IoC.Container.GetInstance<IPushReactable>();

    /// <inheritdoc/>
    public IPushReactable<GL> CreateGLReactable() => IoC.Container.GetInstance<IPushReactable<GL>>();

    /// <inheritdoc/>
    public IPushReactable<GLObjectsData> CreateGLObjectsReactable()
        => IoC.Container.GetInstance<IPushReactable<GLObjectsData>>();

    /// <inheritdoc/>
    public IPushReactable<BatchSizeData> CreateBatchSizeReactable() => IoC.Container.GetInstance<IPushReactable<BatchSizeData>>();

    /// <inheritdoc/>
    public IPushReactable<WindowSizeData> CreatePushWindowSizeReactable() => IoC.Container.GetInstance<IPushReactable<WindowSizeData>>();

    /// <inheritdoc/>
    public IPushReactable<ViewPortSizeData> CreateViewPortReactable() => IoC.Container.GetInstance<IPushReactable<ViewPortSizeData>>();

    /// <inheritdoc/>
    public IPushReactable<MouseStateData> CreateMouseReactable() => IoC.Container.GetInstance<IPushReactable<MouseStateData>>();

    /// <inheritdoc/>
    public IPushReactable<KeyboardKeyStateData> CreateKeyboardReactable() => IoC.Container.GetInstance<IPushReactable<KeyboardKeyStateData>>();

    /// <inheritdoc/>
    public IPushReactable<DisposeTextureData> CreateDisposeTextureReactable() => IoC.Container.GetInstance<IPushReactable<DisposeTextureData>>();

    /// <inheritdoc/>
    public IPushReactable<DisposeSoundData> CreateDisposeSoundReactable() => IoC.Container.GetInstance<IPushReactable<DisposeSoundData>>();

    /// <inheritdoc/>
    public IBatchPullReactable<TextureBatchItem> CreateTexturePullBatchReactable() =>
        IoC.Container.GetInstance<IBatchPullReactable<TextureBatchItem>>();

    /// <inheritdoc/>
    public IBatchPullReactable<FontGlyphBatchItem> CreateFontPullBatchReactable() =>
        IoC.Container.GetInstance<IBatchPullReactable<FontGlyphBatchItem>>();

    /// <inheritdoc/>
    public IBatchPullReactable<ShapeBatchItem> CreateShapePullBatchReactable() =>
        IoC.Container.GetInstance<IBatchPullReactable<ShapeBatchItem>>();

    /// <inheritdoc/>
    public IBatchPullReactable<LineBatchItem> CreateLinePullBatchReactable() =>
        IoC.Container.GetInstance<IBatchPullReactable<LineBatchItem>>();

    /// <inheritdoc/>
    public IRenderBatchReactable<TextureBatchItem> CreateRenderTextureReactable() =>
        IoC.Container.GetInstance<IRenderBatchReactable<TextureBatchItem>>();

    /// <inheritdoc/>
    public IRenderBatchReactable<FontGlyphBatchItem> CreateRenderFontReactable() =>
        IoC.Container.GetInstance<IRenderBatchReactable<FontGlyphBatchItem>>();

    /// <inheritdoc/>
    public IRenderBatchReactable<ShapeBatchItem> CreateRenderShapeReactable() =>
        IoC.Container.GetInstance<IRenderBatchReactable<ShapeBatchItem>>();

    /// <inheritdoc/>
    public IRenderBatchReactable<LineBatchItem> CreateRenderLineReactable() =>
        IoC.Container.GetInstance<IRenderBatchReactable<LineBatchItem>>();
}
