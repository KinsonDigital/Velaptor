// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Batching;
using Graphics.Renderers;

/// <summary>
/// Creates renderers for rendering different types of graphics.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due direct interaction with IoC container.")]
public static class RendererFactory
{
    /// <summary>
    /// Creates an instance of the <see cref="ITextureRenderer"/>.
    /// </summary>
    /// <returns>The texture renderer.</returns>
    public static ITextureRenderer CreateTextureRenderer() => IoC.Container.GetInstance<ITextureRenderer>();

    /// <summary>
    /// Creates an instance of the <see cref="IFontRenderer"/>.
    /// </summary>
    /// <returns>The font renderer.</returns>
    public static IFontRenderer CreateFontRenderer() => IoC.Container.GetInstance<IFontRenderer>();

    /// <summary>
    /// Creates an instance of the <see cref="IShapeRenderer"/>.
    /// </summary>
    /// <returns>The rectangle renderer.</returns>
    public static IShapeRenderer CreateShapeRenderer() => IoC.Container.GetInstance<IShapeRenderer>();

    /// <summary>
    /// Creates an instance of the <see cref="ILineRenderer"/>.
    /// </summary>
    /// <returns>The line renderer.</returns>
    public static ILineRenderer CreateLineRenderer() => IoC.Container.GetInstance<ILineRenderer>();

    /// <summary>
    /// Creates an instance of <see cref="IBatcher"/> to start and stop batching.
    /// </summary>
    /// <returns>The batcher instance.</returns>
    public static IBatcher CreateBatcher() => IoC.Container.GetInstance<IBatcher>();
}
