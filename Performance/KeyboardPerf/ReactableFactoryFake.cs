// <copyright file="ReactableFactoryFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace KeyboardPerf;

using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Silk.NET.OpenGL;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;

internal class ReactableFactoryFake : IReactableFactory
{
    private readonly IPushReactable<KeyboardKeyStateData> keyboardReactable;

    public ReactableFactoryFake(IPushReactable<KeyboardKeyStateData> keyboardReactable) => this.keyboardReactable = keyboardReactable;

    public IPushReactable CreateNoDataPushReactable() => throw new NotImplementedException();

    public IPushReactable<GL> CreateGLReactable() => throw new NotImplementedException();

    public IPushReactable<BatchSizeData> CreateBatchSizeReactable() => throw new NotImplementedException();

    public IPushReactable<WindowSizeData> CreateWindowSizeReactable() => throw new NotImplementedException();

    public IPushReactable<ViewPortSizeData> CreateViewPortReactable() => throw new NotImplementedException();

    public IPushReactable<MouseStateData> CreateMouseReactable() => throw new NotImplementedException();

    public IPushReactable<KeyboardKeyStateData> CreateKeyboardReactable() => this.keyboardReactable;

    public IPushReactable<DisposeTextureData> CreateDisposeTextureReactable() => throw new NotImplementedException();

    public IPushReactable<DisposeSoundData> CreateDisposeSoundReactable() => throw new NotImplementedException();

    public IBatchPullReactable<TextureBatchItem> CreateTexturePullBatchReactable() => throw new NotImplementedException();

    public IBatchPullReactable<FontGlyphBatchItem> CreateFontPullBatchReactable() => throw new NotImplementedException();

    public IBatchPullReactable<ShapeBatchItem> CreateShapePullBatchReactable() => throw new NotImplementedException();

    public IBatchPullReactable<LineBatchItem> CreateLinePullBatchReactable() => throw new NotImplementedException();

    public IRenderBatchReactable<TextureBatchItem> CreateRenderTextureReactable() => throw new NotImplementedException();

    public IRenderBatchReactable<FontGlyphBatchItem> CreateRenderFontReactable() => throw new NotImplementedException();

    public IRenderBatchReactable<ShapeBatchItem> CreateRenderShapeReactable() => throw new NotImplementedException();

    public IRenderBatchReactable<LineBatchItem> CreateRenderLineReactable() => throw new NotImplementedException();
}
