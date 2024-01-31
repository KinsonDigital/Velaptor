// <copyright file="ReactableFactoryFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace MousePerf;

using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Silk.NET.OpenGL;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;

/// <summary>
/// Used for the purpose of performance testing.
/// </summary>
internal sealed class ReactableFactoryFake : IReactableFactory
{
    /// <inheritdoc/>
    public IPushReactable CreateNoDataPushReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<GL> CreateGLReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<GLObjectsData> CreateGLObjectsReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<BatchSizeData> CreateBatchSizeReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<WindowSizeData> CreatePushWindowSizeReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPullReactable<WindowSizeData> CreatePullWindowSizeReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<ViewPortSizeData> CreateViewPortReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<MouseStateData> CreateMouseReactable() => new PushReactableFake();

    /// <inheritdoc/>
    public IPushReactable<KeyboardKeyStateData> CreateKeyboardReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<DisposeTextureData> CreateDisposeTextureReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IPushReactable<DisposeSoundData> CreateDisposeSoundReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IBatchPullReactable<TextureBatchItem> CreateTexturePullBatchReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IBatchPullReactable<FontGlyphBatchItem> CreateFontPullBatchReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IBatchPullReactable<ShapeBatchItem> CreateShapePullBatchReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IBatchPullReactable<LineBatchItem> CreateLinePullBatchReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IRenderBatchReactable<TextureBatchItem> CreateRenderTextureReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IRenderBatchReactable<FontGlyphBatchItem> CreateRenderFontReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IRenderBatchReactable<ShapeBatchItem> CreateRenderShapeReactable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public IRenderBatchReactable<LineBatchItem> CreateRenderLineReactable() => throw new NotImplementedException();
}
