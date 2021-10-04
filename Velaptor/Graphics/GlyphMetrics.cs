// <copyright file="GlyphMetrics.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System.Drawing;

    /// <summary>
    /// Holds all of the various metrics of a glyph for rendering purposes.
    /// </summary>
    public struct GlyphMetrics
    {
        /// <summary>
        /// Gets or sets the glyph character.
        /// </summary>
        public char Glyph { get; set; }

        /// <summary>
        /// Gets or sets the rectangular bounds of where in a font texture
        /// atlas the given <see cref="Glyph"/> resides.
        /// </summary>
        public Rectangle AtlasBounds { get; set; }

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
        public int Ascender { get; set; }

        /// <summary>
        /// Gets or sets the vertical distance from the horizontal baseline to the lowest ‘character’ coordinate in a font face.
        /// </summary>
        /// <remarks>
        ///     Unfortunately, font formats don't define the descender in a uniform way. For some formats,
        ///     it represents the descent of all capital latin characters (without accents), for others it is
        ///     the ascent of the lowest accented character, and finally, other formats define it as being equal
        ///     to Y min value of the global bounding box. This field is negative for values below the baseline.
        /// </remarks>
        public int Descender { get; set; }

        /// <summary>
        /// Gets or sets the horizontal distance from the current cursor position to
        /// the leftmost border of the glyph image's bounding box.
        /// </summary>
        public int HoriBearingX { get; set; }

        /// <summary>
        /// Gets or sets the vertical distance from the current cursor position
        /// (on the baseline) to the topmost border of the glyph image's bounding box.
        /// </summary>
        public int HoriBearingY { get; set; }

        /// <summary>
        /// Gets or sets the horizontal distance to increment the pen position when the glyph
        /// is drawn as part of a string of text.
        /// </summary>
        public int HorizontalAdvance { get; set; }

        /// <summary>
        /// Gets or sets the glyph's width.
        /// </summary>
        public int GlyphWidth { get; set; }

        /// <summary>
        /// Gets or sets the glyph's height.
        /// </summary>
        public int GlyphHeight { get; set; }

        /// <summary>
        /// Gets or sets the horizontal minimum (left-most).
        /// </summary>
        public int XMin { get; set; }

        /// <summary>
        /// Gets or sets the horizontal maximum (right-most).
        /// </summary>
        public int XMax { get; set; }

        /// <summary>
        /// Gets or sets the vertical minimum (bottom-most).
        /// </summary>
        public int YMin { get; set; }

        /// <summary>
        /// Gets or sets the vertical maximum (top-most).
        /// </summary>
        public int YMax { get; set; }

        /// <summary>
        /// Gets or sets the glyph index.
        /// </summary>
        /// <remarks>
        ///     The value of 0 means ‘undefined character code’.
        /// </remarks>
        public uint CharIndex { get; set; }

        /// <inheritdoc/>
        public override string ToString() => $"Name: {Glyph} | Bounds: {AtlasBounds}";
    }
}
