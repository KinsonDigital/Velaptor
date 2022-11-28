// <copyright file="GlyphMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

/// <summary>
/// Holds all of the various metrics of a glyph for rendering purposes.
/// </summary>
public struct GlyphMetrics : IEquatable<GlyphMetrics>
{
    /// <summary>
    /// Gets or sets the glyph character.
    /// </summary>
    public char Glyph { get; set; }

    /// <summary>
    /// Gets or sets the rectangular bounds of where in a font texture
    /// atlas the given <see cref="Glyph"/> resides.
    /// </summary>
    public RectangleF GlyphBounds { get; set; }

    /// <summary>
    /// Gets or sets the vertical distance from the horizontal baseline to the highest 'character'
    /// coordinate in a font face.
    /// </summary>
    /// <remarks>
    ///     Unfortunately, font formats don't define the ascender in a
    ///     uniform way. For some formats, it represents the ascent of all capital Latin characters
    ///     (without accents). For others it is the ascent of the highest accented character. Finally,
    ///     other formats define it as being equal to Y max value of the global bounding box.
    /// </remarks>
    public float Ascender { get; set; }

    /// <summary>
    /// Gets or sets the vertical distance from the horizontal baseline to the lowest ‘character’ coordinate in a font face.
    /// </summary>
    /// <remarks>
    ///     Unfortunately, font formats don't define the descender in a uniform way. For some formats,
    ///     it represents the descent of all capital Latin characters (without accents). For others it is
    ///     the ascent of the lowest accented character. Finally, other formats define it as being equal
    ///     to the Y min value of the global bounding box. This field is negative for values below the baseline.
    /// </remarks>
    public float Descender { get; set; }

    /// <summary>
    /// Gets or sets the horizontal distance from the current cursor position to
    /// the leftmost border of the glyph image's bounding box.
    /// </summary>
    public float HoriBearingX { get; set; }

    /// <summary>
    /// Gets or sets the vertical distance from the current cursor position
    /// (on the baseline) to the topmost border of the glyph image's bounding box.
    /// </summary>
    public float HoriBearingY { get; set; }

    /// <summary>
    /// Gets or sets the horizontal distance to increment the pen position when the glyph
    /// is drawn as part of a string of text.
    /// </summary>
    public float HorizontalAdvance { get; set; }

    /// <summary>
    /// Gets or sets the glyph's width.
    /// </summary>
    public float GlyphWidth { get; set; }

    /// <summary>
    /// Gets or sets the glyph's height.
    /// </summary>
    public float GlyphHeight { get; set; }

    /// <summary>
    /// Gets or sets the horizontal minimum (left-most).
    /// </summary>
    public float XMin { get; set; }

    /// <summary>
    /// Gets or sets the horizontal maximum (right-most).
    /// </summary>
    public float XMax { get; set; }

    /// <summary>
    /// Gets or sets the vertical minimum (bottom-most).
    /// </summary>
    public float YMin { get; set; }

    /// <summary>
    /// Gets or sets the vertical maximum (top-most).
    /// </summary>
    public float YMax { get; set; }

    /// <summary>
    /// Gets or sets the glyph index.
    /// </summary>
    /// <remarks>
    ///     The value of 0 means ‘undefined character code’.
    /// </remarks>
    public uint CharIndex { get; set; }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand compared with the right operand.</param>
    /// <param name="right">The right operand compared with the left operand.</param>
    /// <returns>True if both operands are equal.</returns>
    public static bool operator ==(GlyphMetrics left, GlyphMetrics right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand compared with the right operand.</param>
    /// <param name="right">The right operand compared with the left operand.</param>
    /// <returns>True if both operands are not equal.</returns>
    public static bool operator !=(GlyphMetrics left, GlyphMetrics right) => !left.Equals(right);

    /// <inheritdoc/>
    public override string ToString() => $"Name: {Glyph} | Bounds: {GlyphBounds}";

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj) => obj is GlyphMetrics metric && Equals(metric);

    /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
    public bool Equals(GlyphMetrics other) =>
        Glyph == other.Glyph &&
        GlyphBounds.Equals(other.GlyphBounds) &&
        Ascender.Equals(other.Ascender) &&
        Descender.Equals(other.Descender) &&
        HoriBearingX.Equals(other.HoriBearingX) &&
        HoriBearingY.Equals(other.HoriBearingY) &&
        HorizontalAdvance.Equals(other.HorizontalAdvance) &&
        GlyphWidth.Equals(other.GlyphWidth) &&
        GlyphHeight.Equals(other.GlyphHeight) &&
        XMin.Equals(other.XMin) &&
        XMax.Equals(other.XMax) &&
        YMin.Equals(other.YMin) &&
        YMax.Equals(other.YMax) &&
        CharIndex == other.CharIndex;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        var hashCode = default(HashCode);
        hashCode.Add(Glyph);
        hashCode.Add(GlyphBounds);
        hashCode.Add(Ascender);
        hashCode.Add(Descender);
        hashCode.Add(HoriBearingX);
        hashCode.Add(HoriBearingY);
        hashCode.Add(HorizontalAdvance);
        hashCode.Add(GlyphWidth);
        hashCode.Add(GlyphHeight);
        hashCode.Add(XMin);
        hashCode.Add(XMax);
        hashCode.Add(YMin);
        hashCode.Add(YMax);
        hashCode.Add(CharIndex);

        return hashCode.ToHashCode();
    }
}
