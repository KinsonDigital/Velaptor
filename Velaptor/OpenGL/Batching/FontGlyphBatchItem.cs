// <copyright file="FontGlyphBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Graphics;

/// <summary>
/// A single item in a batch of glyph items that can be rendered to the screen.
/// </summary>
internal readonly record struct FontGlyphBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontGlyphBatchItem"/> struct.
    /// </summary>
    /// <param name="srcRect">The rectangular section inside of a texture to render.</param>
    /// <param name="destRect">The destination rectangular area of where to render the glyph on the screen.</param>
    /// <param name="glyph">The font glyph.</param>
    /// <param name="size">The size of the glyph texture to be rendered.</param>
    /// <param name="angle">The angle in degrees of the glyph texture.</param>
    /// <param name="tintColor">The color to apply to the entire glyph texture.</param>
    /// <param name="effects">The type of effects to apply to the glyph texture when rendering.</param>
    /// <param name="textureId">The ID of the font atlas texture.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public FontGlyphBatchItem(
        RectangleF srcRect,
        RectangleF destRect,
        char glyph,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        uint textureId)
    {
        SrcRect = srcRect;
        DestRect = destRect;
        Glyph = glyph;
        Size = size;
        Angle = angle;
        TintColor = tintColor;
        Effects = effects;
        TextureId = textureId;
    }

    /// <summary>
    /// Gets the rectangular section inside of a texture to render.
    /// </summary>
    public RectangleF SrcRect { get; }

    /// <summary>
    /// Gets the destination rectangular area of where to render the glyph on the screen.
    /// </summary>
    public RectangleF DestRect { get; }

    /// <summary>
    /// Gets the font glyph.
    /// </summary>
    public char Glyph { get; }

    /// <summary>
    /// Gets the size of the glyph texture to be rendered.
    /// </summary>
    /// <remarks>This must be a value between 0 and 1.</remarks>
    public float Size { get; }

    /// <summary>
    /// Gets the angle in degrees of the glyph texture.
    /// </summary>
    /// <remarks>Needs to be a value between 0 and 360.</remarks>
    public float Angle { get; }

    /// <summary>
    /// Gets the color to apply to the entire glyph texture.
    /// </summary>
    public Color TintColor { get; }

    /// <summary>
    /// Gets the type of effects to apply to the glyph texture when rendering.
    /// </summary>
    public RenderEffects Effects { get; }

    /// <summary>
    /// Gets the ID of the font atlas texture.
    /// </summary>
    public uint TextureId { get; }

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="FontGlyphBatchItem"/> is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        !(Size > 0f ||
          Angle > 0f ||
          SrcRect.IsEmpty != true ||
          DestRect.IsEmpty != true ||
          Glyph != '\0' ||
          TintColor.IsEmpty != true ||
          Effects != RenderEffects.None ||
          TextureId > 0);
}
