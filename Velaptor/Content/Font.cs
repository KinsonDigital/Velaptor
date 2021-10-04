// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;

    /// <summary>
    /// Represents a font with a particular size that
    /// can be used to render text to the screen.
    /// </summary>
    public sealed class Font : IFont
    {
        private readonly char[] availableGlyphCharacters;
        private readonly GlyphMetrics[] metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glyphs.</param>
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
            this.metrics = fontAtlasData;

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
        public bool HasKerning { get; internal init; }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc cref="IContent.IsPooled"/>
        public bool IsPooled { get; set; }

        /// <inheritdoc/>
        public ReadOnlyCollection<GlyphMetrics> Metrics => this.metrics.ToReadOnlyCollection();

        /// <inheritdoc/>
        public char[] GetAvailableGlyphCharacters() => this.availableGlyphCharacters;

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsPooled)
            {
                throw new PooledDisposalException();
            }

            if (disposing)
            {
                FontTextureAtlas.IsPooled = false;
                FontTextureAtlas.Dispose();
                IsDisposed = true;
            }
        }
    }
}
