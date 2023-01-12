// <copyright file="ITextureRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using Content;

/// <summary>
/// Renders textures to the screen.
/// </summary>
public interface ITextureRenderer
{
    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position are based on the center of the texture.
    ///     </para>
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
    void Render(ITexture texture, int x, int y, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the texture.
    ///     </para>
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
    void Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the texture.
    ///     </para>
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
    void Render(ITexture texture, int x, int y, Color color, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the texture.
    ///     </para>
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
    void Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given <see cref="Texture"/> using the given parameters.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="srcRect">The rectangle of the sub texture within the texture to render.</param>
    /// <param name="destRect">The destination rectangle of rendering.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%/normal size.</param>
    /// <param name="angle">The angle of rotation in degrees of the rendering.</param>
    /// <param name="color">The color to apply to the rendering.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position in the <paramref name="destRect"/> is based on the center of the texture.
    ///     </para>
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
    void Render(ITexture texture, Rectangle srcRect, Rectangle destRect, float size, float angle, Color color, RenderEffects effects, int layer = 0);
}
