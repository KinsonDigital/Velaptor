// <copyright file="RendererFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Graphics.Renderers;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due direct interaction with IoC container.")]
public sealed class RendererFactory : IRendererFactory
{
    /// <inheritdoc/>
    public ITextureRenderer CreateTextureRenderer() => IoC.Container.GetInstance<ITextureRenderer>();

    /// <inheritdoc/>
    public IFontRenderer CreateFontRenderer() => IoC.Container.GetInstance<IFontRenderer>();

    /// <inheritdoc/>
    public IShapeRenderer CreateShapeRenderer() => IoC.Container.GetInstance<IShapeRenderer>();

    /// <inheritdoc/>
    public ILineRenderer CreateLineRenderer() => IoC.Container.GetInstance<ILineRenderer>();
}
