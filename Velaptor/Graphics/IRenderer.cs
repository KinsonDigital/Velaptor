// <copyright file="IRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;

namespace Velaptor.Graphics;

/// <summary>
/// Renders a single or batch of textures.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public facing API required for library users.")]
public interface IRenderer
{
    /// <summary>
    /// Gets the total number of items rendered for each batch.
    /// </summary>
    public const uint BatchSize = 1000;

    /// <summary>
    /// Gets or sets the render surface width.
    /// </summary>
    /// <remarks>This is the width of the viewport.</remarks>
    uint RenderSurfaceWidth { get; set; }

    /// <summary>
    /// Gets or sets the render surface height.
    /// </summary>
    /// <remarks>This is the height of the viewport.</remarks>
    uint RenderSurfaceHeight { get; set; }

    /// <summary>
    /// Gets or sets the color that the back buffer will be cleared to.
    /// </summary>
    Color ClearColor { get; set; }

    /// <summary>
    /// Starts the batch rendering process.  Must be called before invoking any render methods.
    /// </summary>
    void Begin();

    /// <summary>
    /// Clears the buffers.
    /// </summary>
    /// <remarks>
    ///     It is best to clear the buffer before rendering all of the textures.
    ///     This is to make sure smearing does not occur during texture
    ///     movement or animation.
    /// </remarks>
    void Clear();

    /// <summary>
    /// Ends the batch process.  Calling this will render any textures
    /// still in the batch.
    /// </summary>
    void End();

    /// <summary>
    /// Updates the view port size.
    /// </summary>
    /// <param name="size">The size to set the view port to.</param>
    void OnResize(SizeU size);

    /// <summary>
    /// Renders the given texture at the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="texture">The texture to render.</param>
    /// <param name="x">The X location of the texture.</param>
    /// <param name="y">The y location of the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position is based on the center of the text.  The center of the text is based on the
    ///         furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If 2 separate textures have the same <paramref name="layer"/> value, they will
    ///         rendered in the order that the render method was invoked.
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
    /// <param name="y">The y location of the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position is based on the center of the text.  The center of the text is based on the
    ///         furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If 2 separate textures have the same <paramref name="layer"/> value, they will
    ///         rendered in the order that the render method was invoked.
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
    /// <param name="y">The y location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position is based on the center of the text.  The center of the text is based on the
    ///         furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If 2 separate textures have the same <paramref name="layer"/> value, they will
    ///         rendered in the order that the render method was invoked.
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
    /// <param name="y">The y location of the texture.</param>
    /// <param name="color">The color to apply to the texture.</param>
    /// <param name="effects">The rendering effects to apply to the texture when rendering.</param>
    /// <param name="layer">The layer to render the texture.</param>
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position is based on the center of the text.  The center of the text is based on the
    ///         furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If 2 separate textures have the same <paramref name="layer"/> value, they will
    ///         rendered in the order that the render method was invoked.
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
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The position is based on the center of the text.  The center of the text is based on the
    ///         furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         Lower <paramref name="layer"/> values will render before higher <paramref name="layer"/> values.
    ///         If 2 separate textures have the same <paramref name="layer"/> value, they will
    ///         rendered in the order that the render method was invoked.
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
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <remarks>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </remarks>
    void Render(IFont font, string text, int x, int y);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// and <paramref name="position"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <remarks>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </remarks>
    void Render(IFont font, string text, Vector2 position);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <remarks>
    /// <para>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </para>
    ///
    /// <para>
    ///     The size is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///     at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
    /// </para>
    /// </remarks>
    void Render(IFont font, string text, int x, int y, float renderSize, float angle);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/>, with the given <paramref name="size"/>, and <paramref name="angle"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="size">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <remarks>
    /// <para>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </para>
    ///
    /// <para>
    ///     The size is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///     at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
    /// </para>
    /// </remarks>
    void Render(IFont font, string text, Vector2 position, float size, float angle);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates
    /// and <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <remarks>
    /// <para>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </para>
    ///
    /// <para>
    ///     The size is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///     at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
    /// </para>
    /// </remarks>
    void Render(IFont font, string text, int x, int y, Color color);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/> with the given <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <remarks>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </remarks>
    void Render(IFont font, string text, Vector2 position, Color color);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/>, <paramref name="angle"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="color">The color of the text.</param>
    /// <remarks>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </remarks>
    void Render(IFont font, string text, Vector2 position, float angle, Color color);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates,
    /// with the given <paramref name="angle"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="color">The color to apply to the rendering.</param>
    /// <remarks>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </remarks>
    void Render(IFont font, string text, int x, int y, float angle, Color color);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates,
    /// with the given <paramref name="angle"/>, <paramref name="renderSize"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="color">The color to apply to the rendering.</param>
    /// <remarks>
    /// <para>
    ///     The position is based on the center of the text.  The center of the text is based on the
    ///     furthest most left, right, top, and bottom edges of the text.
    /// </para>
    ///
    /// <para>
    ///     The size is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///     at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
    /// </para>
    /// </remarks>
    void Render(IFont font, string text, int x, int y, float renderSize, float angle, Color color);

    /// <summary>
    /// Renders the given <paramref name="rectangle"/>.
    /// </summary>
    /// <param name="rectangle">The rectangle to render.</param>
    /// <exception cref="Exception">Thrown if the <see cref="Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     The <see cref="RectShape.Position"/> is the center of the rectangle.
    /// </remarks>
    void Render(RectShape rectangle);
}
