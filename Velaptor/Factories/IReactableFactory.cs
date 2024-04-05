// <copyright file="IReactableFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Batching;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Graphics;
using OpenGL.Batching;
using ReactableData;
using Silk.NET.OpenGL;

/// <summary>
/// Generates reactable instances.
/// </summary>
internal interface IReactableFactory
{
    /// <summary>
    /// Creates a non-directional reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable CreateNoDataPushReactable();

    /// <summary>
    /// Creates an OpenGL reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<GL> CreateGLReactable();

    /// <summary>
    /// Creates an OpenGL objects reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<GLObjectsData> CreateGLObjectsReactable();

    /// <summary>
    /// Creates a batch size reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<BatchSizeData> CreateBatchSizeReactable();

    /// <summary>
    /// Creates a push window size reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<WindowSizeData> CreatePushWindowSizeReactable();

    /// <summary>
    /// Creates a pull window size reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPullReactable<WindowSizeData> CreatePullWindowSizeReactable();

    /// <summary>
    /// Creates a viewport reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<ViewPortSizeData> CreateViewPortReactable();

    /// <summary>
    /// Creates a mouse reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<MouseStateData> CreateMouseReactable();

    /// <summary>
    /// Creates a keyboard reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<KeyboardKeyStateData> CreateKeyboardReactable();

    /// <summary>
    /// Creates a dispose texture reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<DisposeTextureData> CreateDisposeTextureReactable();

    /// <summary>
    /// Creates a dispose texture reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<DisposeAudioData> CreateDisposeSoundReactable();

    /// <summary>
    /// Creates a reactable for pulling texture batch data.
    /// </summary>
    /// <returns>The reactable.</returns>
    IBatchPullReactable<TextureBatchItem> CreateTexturePullBatchReactable();

    /// <summary>
    /// Creates a reactable for pulling font batch data.
    /// </summary>
    /// <returns>The reactable.</returns>
    IBatchPullReactable<FontGlyphBatchItem> CreateFontPullBatchReactable();

    /// <summary>
    /// Creates a reactable for pulling shape batch data.
    /// </summary>
    /// <returns>The reactable.</returns>
    IBatchPullReactable<ShapeBatchItem> CreateShapePullBatchReactable();

    /// <summary>
    /// Creates a reactable for pulling line batch data.
    /// </summary>
    /// <returns>The reactable.</returns>
    IBatchPullReactable<LineBatchItem> CreateLinePullBatchReactable();

    /// <summary>
    /// Creates a reactable for pushing texture batch data to the texture renderer.
    /// </summary>
    /// <returns>The reactable.</returns>
    IRenderBatchReactable<TextureBatchItem> CreateRenderTextureReactable();

    /// <summary>
    /// Creates a reactable for pushing font batch data to the font renderer.
    /// </summary>
    /// <returns>The reactable.</returns>
    IRenderBatchReactable<FontGlyphBatchItem> CreateRenderFontReactable();

    /// <summary>
    /// Creates a reactable for pushing shape batch data to the shape renderer.
    /// </summary>
    /// <returns>The reactable.</returns>
    IRenderBatchReactable<ShapeBatchItem> CreateRenderShapeReactable();

    /// <summary>
    /// Creates a reactable for pushing line batch data to the line renderer.
    /// </summary>
    /// <returns>The reactable.</returns>
    IRenderBatchReactable<LineBatchItem> CreateRenderLineReactable();
}
