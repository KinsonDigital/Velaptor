// <copyright file="IFont.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Raptor.Content;

    /// <summary>
    /// The font to use when rendering text to the screen.
    /// </summary>
    public interface IFont : IContent, IDisposable
    {
        /// <summary>
        /// Gets the font atlas texture that contains all of the bitmap data for all of the available glyphs for the font.
        /// </summary>
        ITexture FontTextureAtlas { get; }

        /// <summary>
        /// Gets the size of the font in points.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the style of the font.
        /// </summary>
        FontStyle Style { get; }

        /// <summary>
        /// Gets a value indicating whether the font has kerning for text rendering layout.
        /// </summary>
        bool HasKerning { get; }

        /// <summary>
        /// Gets the list of metrics for all of the glyphs supported by the font.
        /// </summary>
        /// <returns>The glyph metrics.</returns>
        GlyphMetrics[] Metrics { get; }

        /// <summary>
        /// Returns a list of all the available glyph characters for the font.
        /// </summary>
        /// <returns>The list of glyph characters that can be rendered.</returns>
        char[] GetAvailableGlyphCharacters();
    }
}
