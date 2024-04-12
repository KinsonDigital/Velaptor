﻿// <copyright file="IFont.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Text;
using Graphics;
using VelFontStyle = FontStyle;

/// <summary>
/// The font to use when rendering text to the screen.
/// </summary>
public interface IFont : IContent
{
    /// <summary>
    /// Gets the source of where the font was loaded.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    FontSource Source { get; }

    /// <summary>
    /// Gets the font atlas texture that contains all the bitmap data for all available glyphs for the font.
    /// </summary>
    ITexture Atlas { get; }

    /// <summary>
    /// Gets or sets the size of the font in points.
    /// </summary>
    uint Size { get; set; }

    /// <summary>
    /// Gets or sets the style of the font.
    /// </summary>
    VelFontStyle Style { get; set; }

    /// <summary>
    /// Gets a value indicating whether the font is a default font.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    bool IsDefaultFont { get; }

    /// <summary>
    /// Gets a list of all the available font styles for the current font <see cref="FamilyName"/>.
    /// </summary>
    IEnumerable<FontStyle> AvailableStylesForFamily { get; }

    /// <summary>
    /// Gets the name of the font family.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    string FamilyName { get; }

    /// <summary>
    /// Gets a value indicating whether the font has kerning for text rendering layout.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    bool HasKerning { get; }

    /// <summary>
    /// Gets the spacing between lines of text in pixels.
    /// </summary>
    float LineSpacing { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to cache the measurements of the text.
    /// </summary>
    bool CacheEnabled { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of measurements to cache.
    /// </summary>
    int MaxCacheSize { get; set; }

    /// <summary>
    /// Gets the list of metrics for all the glyphs supported by the font.
    /// </summary>
    /// <returns>The glyph metrics.</returns>
    IReadOnlyCollection<GlyphMetrics> Metrics { get; }

    /// <summary>
    /// Measures the width and height bounds of the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The width and height of the text in pixels.</returns>
    SizeF Measure(string text);

    /// <summary>
    /// Gets the glyph metrics using the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to get the metrics for.</param>
    /// <returns>The metrics of each individual glyph/character.</returns>
    GlyphMetrics[] ToGlyphMetrics(string text);

    /// <summary>
    /// Gets the kerning between two glyphs using the given <paramref name="leftGlyphIndex"/> and <paramref name="rightGlyphIndex"/>.
    /// </summary>
    /// <param name="leftGlyphIndex">The index of the left glyph.</param>
    /// <param name="rightGlyphIndex">The index of the right glyph.</param>
    /// <returns>The kerning result between the glyphs.</returns>
    /// <remarks>Refer to https://freetype.org/freetype2/docs/glyphs/glyphs-4.html for more info.</remarks>
    float GetKerning(uint leftGlyphIndex, uint rightGlyphIndex);

    /// <summary>
    /// Returns the bounds of each character in the given <paramref name="text"/> based on the
    /// given <paramref name="textPos"/>.
    /// </summary>
    /// <param name="text">The text to get the bounds data.</param>
    /// <param name="textPos">The position of the text as a whole.</param>
    /// <returns>The bounds for each character.</returns>
    IEnumerable<(char character, RectangleF bounds)> GetCharacterBounds(string text, Vector2 textPos);

    /// <summary>
    /// Returns the bounds of each character in the given <paramref name="text"/> based on the
    /// given <paramref name="textPos"/>.
    /// </summary>
    /// <param name="text">The text to get the bounds data.</param>
    /// <param name="textPos">The position of the text as a whole.</param>
    /// <returns>The bounds for each character.</returns>
    IEnumerable<(char character, RectangleF bounds)> GetCharacterBounds(StringBuilder text, Vector2 textPos);
}
