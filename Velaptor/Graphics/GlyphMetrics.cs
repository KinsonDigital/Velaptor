// <copyright file="GlyphMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Holds all of the various metrics of a glyph for rendering purposes.
    /// </summary>
    public struct GlyphMetrics : IEqualityComparer<GlyphMetrics>
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
        ///     uniform way. For some formats, it represents the ascent of all capital latin characters
        ///     (without accents), for others it is the ascent of the highest accented character, and finally,
        ///     other formats define it as being equal to Y max value of the global bounding box.
        /// </remarks>
        public float Ascender { get; set; }

        /// <summary>
        /// Gets or sets the vertical distance from the horizontal baseline to the lowest ‘character’ coordinate in a font face.
        /// </summary>
        /// <remarks>
        ///     Unfortunately, font formats don't define the descender in a uniform way. For some formats,
        ///     it represents the descent of all capital latin characters (without accents), for others it is
        ///     the ascent of the lowest accented character, and finally, other formats define it as being equal
        ///     to Y min value of the global bounding box. This field is negative for values below the baseline.
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

        /// <inheritdoc/>
        public override string ToString() => $"Name: {Glyph} | Bounds: {GlyphBounds}";

        // TODO: Code docs
        public override bool Equals(object obj)
        {
            if (obj is not GlyphMetrics metric)
            {
                return false;
            }

            return Equals(metric, this);
        }

        public bool Equals(GlyphMetrics x, GlyphMetrics y)
        {
            return x.Glyph == y.Glyph &&
                   x.GlyphBounds == y.GlyphBounds &&//
                   x.Ascender == y.Ascender &&//
                   x.Descender == y.Descender &&//
                   x.HoriBearingX == y.HoriBearingX &&
                   x.HoriBearingY == y.HoriBearingY &&//
                   x.HorizontalAdvance == y.HorizontalAdvance &&
                   x.GlyphWidth == y.GlyphWidth &&//
                   x.GlyphHeight == y.GlyphHeight &&//
                   x.XMin == y.XMin &&//
                   x.XMax == y.XMax &&//
                   x.YMin == y.YMin &&//
                   x.YMax == y.YMax &&//
                   x.CharIndex == y.CharIndex;
        }

        public int GetHashCode(GlyphMetrics obj)
        {
            return HashCode.Combine(
                Glyph,
                GlyphBounds,
                Ascender,
                Descender,
                HoriBearingX,
                HoriBearingY,
                HorizontalAdvance,
                HashCode.Combine(GlyphWidth, GlyphHeight, XMin, XMax, YMin, YMax, CharIndex));
        }

        public static bool operator==(GlyphMetrics a, GlyphMetrics b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GlyphMetrics a, GlyphMetrics b)
        {
            return !a.Equals(b);
        }
    }
}
