// <copyright file="IDrawable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

/// <summary>
/// Provides the ability for an object to be rendered.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Renders the object.
    /// </summary>
    void Render();
}
