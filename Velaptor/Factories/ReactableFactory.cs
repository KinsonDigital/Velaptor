// <copyright file="ReactableFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using ReactableData;
using Silk.NET.OpenGL;

/// <summary>
/// Creates singleton reactables.
/// </summary>
internal class ReactableFactory : IReactableFactory
{
    /// <inheritdoc/>
    public IPushReactable CreateNoDataPushReactable() => IoC.Container.GetInstance<IPushReactable>();

    /// <inheritdoc/>
    public IPushReactable<GL> CreateGLReactable() => IoC.Container.GetInstance<IPushReactable<GL>>();

    /// <inheritdoc/>
    public IPushReactable<BatchSizeData> CreateBatchSizeReactable() => IoC.Container.GetInstance<IPushReactable<BatchSizeData>>();

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
}
