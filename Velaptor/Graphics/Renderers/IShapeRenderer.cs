// <copyright file="IShapeRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using Batching;

/// <summary>
/// Renders rectangles to the screen.
/// </summary>
public interface IShapeRenderer
{
    /// <summary>
    /// Renders the given <paramref name="rect"/>.
    /// </summary>
    /// <param name="rect">The rectangle to render.</param>
    /// <param name="layer">The layer to render the rectangle.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If two separate textures have the same <paramref name="layer"/> value, they will
    ///         render in the order that the method was invoked.
    ///     </para>
    ///     <para>Example below:</para>
    ///
    ///     <b>Render Method Invoked Order:</b>
    ///     <list type="number">
    ///         <item>Texture 1 (Layer -10)</item>
    ///         <item>Texture 2 (Layer -20)</item>
    ///         <item>Texture 3 (Layer 0)</item>
    ///         <item>Texture 4 (Layer 0)</item>
    ///         <item>Texture 5 (Layer 4)</item>
    ///         <item>Texture 6 (Layer 3)</item>
    ///     </list>
    ///
    ///     <b>Texture Render Order:</b>
    ///     <list type="bullet">
    ///         <item>Texture 2</item>
    ///         <item>Texture 1</item>
    ///         <item>Texture 3</item>
    ///         <item>Texture 4</item>
    ///         <item>Texture 6</item>
    ///         <item>Texture 5</item>
    ///     </list>
    /// </remarks>
    void Render(RectShape rect, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="circle"/>.
    /// </summary>
    /// <param name="circle">The circle to render.</param>
    /// <param name="layer">The layer to render the circle.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If two separate textures have the same <paramref name="layer"/> value, they will
    ///         render in the order that the method was invoked.
    ///     </para>
    ///     <para>Example below:</para>
    ///
    ///     <b>Render Method Invoked Order:</b>
    ///     <list type="number">
    ///         <item>Texture 1 (Layer -10)</item>
    ///         <item>Texture 2 (Layer -20)</item>
    ///         <item>Texture 3 (Layer 0)</item>
    ///         <item>Texture 4 (Layer 0)</item>
    ///         <item>Texture 5 (Layer 4)</item>
    ///         <item>Texture 6 (Layer 3)</item>
    ///     </list>
    ///
    ///     <b>Texture Render Order:</b>
    ///     <list type="bullet">
    ///         <item>Texture 2</item>
    ///         <item>Texture 1</item>
    ///         <item>Texture 3</item>
    ///         <item>Texture 4</item>
    ///         <item>Texture 6</item>
    ///         <item>Texture 5</item>
    ///     </list>
    /// </remarks>
    void Render(CircleShape circle, int layer = 0);
}
