// <copyright file="IDrawable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Graphics;

namespace Velaptor;

/// <summary>
/// Provides the ability for an object to be rendered.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Renders the object.
    /// </summary>
    /// <param name="renderer">Renders textures, primitives, and text.</param>
    void Render(IRenderer renderer);
}
