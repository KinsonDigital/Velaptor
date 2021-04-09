// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Raptor.Content;

    /// <summary>
    /// Represents a font with a particular size that
    /// can be used to render text to the screen.
    /// </summary>
    public class Font : IFont, IDisposable
    {
        private readonly char[] availableGlyphCharacters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glpyhs.</param>
        /// <param name="fontAtlasData">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
        /// <param name="fontSettings">The various font settings.</param>
        /// <param name="availableGlyphChars">The list of available glyph characters for this font.</param>
        /// <param name="name">The name of the font content.</param>
        /// <param name="path">The path to the font content.</param>
        public Font(
            ITexture texture,
            GlyphMetrics[] fontAtlasData,
            FontSettings fontSettings,
            char[] availableGlyphChars,
            string name,
            string path)
        {
            FontTextureAtlas = texture;
            Metrics = fontAtlasData;

            Size = fontSettings.Size;
            Style = fontSettings.Style;
            this.availableGlyphCharacters = availableGlyphChars;
            Name = name;
            Path = path;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        /// <inheritdoc/>
        public ITexture FontTextureAtlas { get; private set; }

        /// <inheritdoc/>
        public int Size { get; private set; }

        /// <inheritdoc/>
        public FontStyle Style { get; private set; }

        /// <inheritdoc/>
        public bool HasKerning { get; internal set; }

        /// <inheritdoc/>
        public bool Unloaded { get; private set; }

        /// <inheritdoc/>
        public GlyphMetrics[] Metrics { get; }

        /// <inheritdoc/>
        public char[] GetAvailableGlyphCharacters() => this.availableGlyphCharacters;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Unloaded)
            {
                return;
            }

            if (disposing)
            {
                FontTextureAtlas.Dispose();
            }

            Unloaded = true;
        }
    }
}
