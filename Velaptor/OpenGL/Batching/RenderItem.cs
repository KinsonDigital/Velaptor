// <copyright file="RenderItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System;

/// <summary>
/// A single batch item with associated meta-data about how it should be rendered.
/// </summary>
/// <typeparam name="T">The <c>struct</c> that represents the batch item.</typeparam>
internal readonly record struct RenderItem<T>
{
    /// <summary>
    /// Gets the layer that the <see cref="Item"/> should be rendered on.
    /// </summary>
    public int Layer { get; init; }

    /// <summary>
    /// Gets the date and time that the <see cref="Item"/> was added for rendering.
    /// </summary>
    public DateTime DateTime { get; init; }

    /// <summary>
    /// Gets the item to render.
    /// </summary>
    public T Item { get; init; }
}
