// <copyright file="ITextureRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Batching;
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
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position are based in the center of the texture.
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
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates, and the given <paramref name="angle"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, int x, int y, float angle, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates, <paramref name="angle"/>,
    /// and <paramref name="size"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, int x, int y, float angle, float size, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates, <paramref name="angle"/>,
    /// <paramref name="size"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, int x, int y, float angle, float size, Color color, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The Y location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
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
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    void Render(ITexture texture, Vector2 pos, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates and the given <paramref name="angle"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, float angle, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates, <paramref name="angle"/>, and
    /// the given <paramref name="size"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, float angle, float size, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates, <paramref name="angle"/>,
    /// <paramref name="size"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, float angle, float size, Color color, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, Color color, int layer = 0);

    /// <summary>
    /// Renders the given texture at the given <paramref name="pos"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="pos">The location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="pos"/> position are based in the center of the texture.
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(ITexture texture, Vector2 pos, Color color, RenderEffects effects, int layer = 0);

    /// <summary>
    /// Renders the given <see cref="Texture"/> using the given parameters.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="srcRect">The rectangle of the sub texture within the texture to render.</param>
    /// <param name="destRect">The destination rectangle of rendering.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="color">The color to apply to the rendering.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IBatcher.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position in the <paramref name="destRect"/> is based in the center of the texture.
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

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, Color color, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, Color color, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API for users.")]
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, Color color, int frameNumber = 0, int layer = 0);

    /// <summary>
    /// Renders the <see cref="Texture"/> in the given <paramref name="atlas"/> using the given parameters.
    /// </summary>
    /// <param name="atlas">The texture atlas data.</param>
    /// <param name="subTextureName">The name of the sub-texture in the atlas.</param>
    /// <param name="pos">The position of where to render the sub-texture.</param>
    /// <param name="angle">The angle of rotation in degrees.</param>
    /// <param name="size">The size to render the texture. 1 is for 100%, which is the normal size.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="frameNumber">The number of the frame if an animation.</param>
    /// <param name="layer">The layer to render the texture.</param>
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
    void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, Color color, RenderEffects effects, int frameNumber = 0, int layer = 0);
}
