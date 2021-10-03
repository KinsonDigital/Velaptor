// <copyright file="IFontAtlasService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.IO;
    using Velaptor.Graphics;

    /// <summary>
    /// Creates font atlas textures for rendering text.
    /// </summary>
    public interface IFontAtlasService : IDisposable
    {
        /// <summary>
        /// Creates a font atlas texture and atlas data that can be used for rendering.
        /// </summary>
        /// <param name="fontFilePath">The path to the font file.</param>
        /// <param name="size">The size to make the font.</param>
        /// <returns>
        /// <list type="number">
        ///     <item>
        ///         <see cref="ImageData"/>: The font atlas texture data.
        ///     </item>
        ///     <item>
        ///         <see cref="GlyphMetrics"/>: The atlas data for all of the glyphs in the font atlas.
        ///     </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="fontFilePath"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the file cannot be found.</exception>
        (ImageData atlasImage, GlyphMetrics[] atlasData) CreateFontAtlas(string fontFilePath, int size);

        /// <summary>
        /// Sets the list of available characters that will be rendered into the font texture atlas.
        /// </summary>
        /// <param name="glyphChars">The list of characters to make available.</param>
        void SetAvailableCharacters(char[] glyphChars);
    }
}
