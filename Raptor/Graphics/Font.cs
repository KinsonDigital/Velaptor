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
    /// Represents a font with a particular size that can be used
    /// to render texture to the screen.
    /// </summary>
    public class Font : IFont, IDisposable
    {
        private readonly GlyphMetrics[] fontAtlasData;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains all of the glyph bitmap data.</param>
        /// <param name="fontAtlasData">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
        /// <param name="name">The name of the font content.</param>
        /// <param name="path">The path to the font content.</param>
        /// <param name="fontSettings">The various font settings.</param>
        public Font(ITexture texture, GlyphMetrics[] fontAtlasData, string name, string path, FontSettings fontSettings)
        {
            FontTextureAtlas = texture;
            this.fontAtlasData = fontAtlasData;

            Name = name;
            Path = path;
            Size = fontSettings.Size;
            Style = fontSettings.Style;
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
        public int Length => this.fontAtlasData.Length;

        /// <inheritdoc/>
        public bool Unloaded { get; private set; }

        /// <inheritdoc/>
        public GlyphMetrics this[int index] => this.fontAtlasData[index];

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public IEnumerator<GlyphMetrics> GetEnumerator() => this.fontAtlasData.ToList().GetEnumerator();

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => this.fontAtlasData.GetEnumerator();

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
