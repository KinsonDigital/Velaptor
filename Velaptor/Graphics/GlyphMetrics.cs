// <copyright file="GlyphMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;

/// <summary>
/// Holds all the various metrics of a glyph for rendering purposes.
/// </summary>
public readonly record struct GlyphMetrics
{
    /// <summary>
    /// Gets the glyph character.
    /// </summary>
    public char Glyph { get; init; }

    /// <summary>
    /// Gets the rectangular bounds of where in a font texture
    /// atlas the given <see cref="Glyph"/> resides.
    /// </summary>
    public RectangleF GlyphBounds { get; init; }

    /// <summary>
    /// Gets the vertical distance from the horizontal baseline to the highest 'character'
    /// coordinate in a font face.
    /// </summary>
    /// <remarks>
    ///     Unfortunately, font formats don't define the ascender in a
    ///     uniform way. For some formats, it represents the ascent of all capital Latin characters
    ///     (without accents). For others, it is the ascent of the highest accented character. Finally,
    ///     other formats define it as being equal to Y max value of the global bounding box.
    /// </remarks>
    public float Ascender { get; init; }

    /// <summary>
    /// Gets the vertical distance from the horizontal baseline to the lowest ‘character’ coordinate in a font face.
    /// </summary>
    /// <remarks>
    ///     Unfortunately, font formats don't define the descender in a uniform way. For some formats,
    ///     it represents the descent of all capital Latin characters (without accents). For others, it is
    ///     the ascent of the lowest accented character. Finally, other formats define it as being equal
    ///     to the Y min value of the global bounding box. This field is negative for values below the baseline.
    /// </remarks>
    public float Descender { get; init; }

    /// <summary>
    /// Gets the horizontal distance from the current cursor position to
    /// the leftmost border of the glyph image's bounding box.
    /// </summary>
    public float HoriBearingX { get; init; }

    /// <summary>
    /// Gets the vertical distance from the current cursor position
    /// (on the baseline) to the top most border of the glyph image's bounding box.
    /// </summary>
    public float HoriBearingY { get; init; }

    /// <summary>
    /// Gets the horizontal distance to increment the pen position when the glyph
    /// is drawn as part of a string of text.
    /// </summary>
    public float HorizontalAdvance { get; init; }

    /// <summary>
    /// Gets the glyph's width.
    /// </summary>
    public float GlyphWidth { get; init; }

    /// <summary>
    /// Gets the glyph's height.
    /// </summary>
    public float GlyphHeight { get; init; }

    /// <summary>
    /// Gets the horizontal minimum (left-most).
    /// </summary>
    public float XMin { get; init; }

    /// <summary>
    /// Gets the horizontal maximum (right-most).
    /// </summary>
    public float XMax { get; init; }

    /// <summary>
    /// Gets the vertical minimum (bottom-most).
    /// </summary>
    public float YMin { get; init; }

    /// <summary>
    /// Gets the vertical maximum (top-most).
    /// </summary>
    public float YMax { get; init; }

    /// <summary>
    /// Gets the glyph index.
    /// </summary>
    /// <remarks>
    ///     The value of 0 means ‘undefined character code’.
    /// </remarks>
    public uint CharIndex { get; init; }
}
