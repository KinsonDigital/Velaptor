// <copyright file="IReactableFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
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
    /// Creates a batch size reactable.
    /// </summary>
    /// <returns>The reactable.</returns>
    IPushReactable<BatchSizeData> CreateBatchSizeReactable();

    /// <summary>
    /// Creates a view port reactable.
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
    IPushReactable<DisposeSoundData> CreateDisposeSoundReactable();
}
