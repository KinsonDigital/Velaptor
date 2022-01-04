// <copyright file="IFont.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.Collections.ObjectModel;
    using System.Drawing;
    using Velaptor.Graphics;
    using VelFontStyle = Velaptor.Content.Fonts.FontStyle;

    /// <summary>
    /// The font to use when rendering text to the screen.
    /// </summary>
    public interface IFont : IContent
    {
        /// <summary>
        /// Gets the font atlas texture that contains all of the bitmap data for all of the available glyphs for the font.
        /// </summary>
        ITexture FontTextureAtlas { get; }

        /// <summary>
        /// Gets or sets the size of the font in points.
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// Gets the style of the font.
        /// </summary>
        VelFontStyle Style { get; }

        /// <summary>
        /// Gets the name of the font family.
        /// </summary>
        string FamilyName { get; }

        /// <summary>
        /// Gets a value indicating whether or not the font has kerning for text rendering layout.
        /// </summary>
        bool HasKerning { get; }

        // TODO: Add code docs
        float LineSpacing { get; }

        /// <summary>
        /// Gets the list of metrics for all of the glyphs supported by the font.
        /// </summary>
        /// <returns>The glyph metrics.</returns>
        ReadOnlyCollection<GlyphMetrics> Metrics { get; }

        /// <summary>
        /// Returns a list of all the available glyph characters for the font.
        /// </summary>
        /// <returns>The list of glyph characters that can be rendered.</returns>
        char[] GetAvailableGlyphCharacters();

        // TODO: Add code docs
        SizeF Measure(string text);

        GlyphMetrics[] ToGlyphMetrics(string text);

        float GetKerning(uint leftGlyphIndex, uint rightGlyphIndex);
    }
}
