// <copyright file="FontGlyphBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
using Graphics;

/// <summary>
/// A single item in a batch of glyph items that can be rendered to the screen.
/// </summary>
internal readonly struct FontGlyphBatchItem : IEquatable<FontGlyphBatchItem>
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
    /// <param name="effects">The typ of effects to apply tot he glyph texture when rendering.</param>
    /// <param name="viewPortSize">The size of the viewport.</param>
    /// <param name="textureId">The ID of the font atlas texture.</param>
    /// <param name="layer">The layer that the shape will be rendered on.</param>
    public FontGlyphBatchItem(
        RectangleF srcRect,
        RectangleF destRect,
        char glyph,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        SizeF viewPortSize,
        uint textureId,
        int layer)
    {
        SrcRect = srcRect;
        DestRect = destRect;
        Glyph = glyph;
        Size = size;
        Angle = angle;
        TintColor = tintColor;
        Effects = effects;
        ViewPortSize = viewPortSize;
        TextureId = textureId;
        Layer = layer;
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
    /// Gets the size of the viewport.
    /// </summary>
    public SizeF ViewPortSize { get; }

    /// <summary>
    /// Gets the ID of the font atlas texture.
    /// </summary>
    public uint TextureId { get; }

    /// <summary>
    /// Gets the layer that the shape will be rendered on.
    /// </summary>
    public int Layer { get; }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand compared with the right operand.</param>
    /// <param name="right">The right operand compared with the left operand.</param>
    /// <returns>True if both operands are equal.</returns>
    public static bool operator ==(FontGlyphBatchItem left, FontGlyphBatchItem right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand compared with the right operand.</param>
    /// <param name="right">The right operand compared with the left operand.</param>
    /// <returns>True if both operands are not equal.</returns>
    public static bool operator !=(FontGlyphBatchItem left, FontGlyphBatchItem right) => !(left == right);

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="FontGlyphBatchItem"/> is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        TextureId == 0 &&
        Size == 0f &&
        Angle == 0f &&
        Layer == 0 &&
        Effects is 0 or RenderEffects.None &&
        Glyph == '\0' &&
        SrcRect.IsEmpty &&
        DestRect.IsEmpty &&
        TintColor.IsEmpty &&
        ViewPortSize.IsEmpty;

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(FontGlyphBatchItem other) =>
        SrcRect.Equals(other.SrcRect) &&
        DestRect.Equals(other.DestRect) &&
        Size.Equals(other.Size) &&
        Angle.Equals(other.Angle) &&
        TintColor.Equals(other.TintColor) &&
        Effects == other.Effects &&
        ViewPortSize.Equals(other.ViewPortSize) &&
        TextureId == other.TextureId &&
        Glyph == other.Glyph &&
        Layer == other.Layer;

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj) => obj is FontGlyphBatchItem other && Equals(other);

    /// <inheritdoc cref="object.GetHashCode"/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
        => HashCode.Combine(
            HashCode.Combine(
                SrcRect,
                DestRect,
                Size,
                Angle,
                TintColor,
                (int)Effects,
                ViewPortSize,
                TextureId),
            Glyph,
            Layer);

    /// <inheritdoc/>
    public override string ToString()
    {
        var result = new StringBuilder();

        result.AppendLine("Font Batch Item Values:");
        result.AppendLine($"Src Rect: {SrcRect.ToString()}");
        result.AppendLine($"Dest Rect: {DestRect.ToString()}");
        result.AppendLine($"Size: {Size.ToString(CultureInfo.InvariantCulture)}");
        result.AppendLine($"Angle: {Angle.ToString(CultureInfo.InvariantCulture)}");
        result.AppendLine(
            $"Tint Clr: {{A={TintColor.A},R={TintColor.R},G={TintColor.G},B={TintColor.B}}}");
        result.AppendLine($"Effects: {Effects.ToString()}");
        result.AppendLine($"View Port Size: {{W={ViewPortSize.Width},H={ViewPortSize.Height}}}");
        result.AppendLine($"Texture ID: {TextureId.ToString()}");
        result.AppendLine($"Glyph: {Glyph}");
        result.Append($"Layer: {Layer}");

        return result.ToString();
    }
}
