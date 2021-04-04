// <copyright file="IFont.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System.Collections.Generic;
    using Raptor.Content;

    /// <summary>
    /// The font to use when rendering text to the screen.
    /// </summary>
    public interface IFont : IContent, IEnumerable<GlyphMetrics>
    {
        /// <summary>
        /// Gets the font atlas texture that contains all of the glyphs for the font for rendering.
        /// </summary>
        ITexture FontTextureAtlas { get; }

        /// <summary>
        /// Gets the size of the font in points.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the total number of glyph metrics in the font.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The iterator for the atlas sub glyph texture data.
        /// </summary>
        /// <param name="index">The index of the glyph data to retrieve.</param>
        /// <returns>The glyph sub texture data.</returns>
        GlyphMetrics this[int index] { get; }
    }
}
