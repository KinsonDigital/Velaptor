// <copyright file="IFontRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using System.Numerics;
using Content.Fonts;

/// <summary>
/// Renders text to the screen using a particular font.
/// </summary>
public interface IFontRenderer
{
    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
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
    void Render(IFont font, string text, int x, int y, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// and <paramref name="position"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="position"/> is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
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
    void Render(IFont font, string text, Vector2 position, int layer = 0);

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
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         The <paramref name="renderSize"/> is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///         at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
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
    void Render(IFont font, string text, int x, int y, float renderSize, float angle, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/>, with the given <paramref name="renderSize"/>, and <paramref name="angle"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="position"/> is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         The <paramref name="renderSize"/> is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///         at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
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
    void Render(IFont font, string text, Vector2 position, float renderSize, float angle, int layer = 0);

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
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         The size is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///         at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
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
    void Render(IFont font, string text, int x, int y, Color color, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/> with the given <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="position"/> is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
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
    void Render(IFont font, string text, Vector2 position, Color color, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="text"/> using the given <paramref name="font"/>
    /// at the given <paramref name="position"/>, <paramref name="angle"/>, and <paramref name="color"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="text"/>.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="position">The position to render the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="position"/> is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
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
    void Render(IFont font, string text, Vector2 position, float angle, Color color, int layer = 0);

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
    /// <param name="layer">The layer to render the text.</param>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
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
    void Render(IFont font, string text, int x, int y, float angle, Color color, int layer = 0);

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
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         The <paramref name="renderSize"/> is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///         at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
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
    void Render(IFont font, string text, int x, int y, float renderSize, float angle, Color color, int layer = 0);

    /// <summary>
    /// Renders the given <paramref name="charMetrics"/> using the given <paramref name="font"/>
    /// at the position determined by the given <paramref name="x"/> and <paramref name="y"/> coordinates,
    /// with the given <paramref name="angle"/>, and <paramref name="renderSize"/>.
    /// </summary>
    /// <param name="font">The font to use for rendering the <paramref name="charMetrics"/>.</param>
    /// <param name="charMetrics">The text to render.</param>
    /// <param name="x">The X coordinate location to render the text.</param>
    /// <param name="y">The Y coordinate location to render the text.</param>
    /// <param name="renderSize">The size of the text.</param>
    /// <param name="angle">The angle of the text in degrees.</param>
    /// <param name="layer">The layer to render the text.</param>
    /// <exception cref="Exception">Thrown if the <see cref="IRenderer.Begin"/> method has not been called.</exception>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="x"/> and <paramref name="y"/> position is based on the center of the text.
    ///         The center of the text is based on the furthest most left, right, top, and bottom edges of the text.
    ///     </para>
    ///     <para>
    ///         The <paramref name="renderSize"/> is a value between 0 and 1.  Using the value 1 represents the text being rendered
    ///         at the standard size of 100%.  Example: Using 1.5 would represent 150% or 50% larger than the normal size.
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
    void Render(IFont font, Span<(GlyphMetrics metrics, Color clr)> charMetrics, int x, int y, float renderSize, float angle, int layer = 0);
}
