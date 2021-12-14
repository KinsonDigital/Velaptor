// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FreeTypeSharp.Native;
using Velaptor.NativeInterop.FreeType;

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
        private readonly IFreeTypeInvoker freeTypeInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glyphs.</param>
        /// <param name="freeTypeInvoker">Invokes native FreeType function calls.</param>
        /// <param name="fontAtlasData">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
        /// <param name="fontSettings">The various font settings.</param>
        /// <param name="availableGlyphChars">The list of available glyph characters for this font.</param>
        /// <param name="name">The name of the font content.</param>
        /// <param name="path">The path to the font content.</param>
        internal Font(
            ITexture texture,
            IFreeTypeInvoker freeTypeInvoker,
            GlyphMetrics[] fontAtlasData,
            FontSettings fontSettings,
            char[] availableGlyphChars,
            string name,
            string path)
        {
            FontTextureAtlas = texture;
            this.freeTypeInvoker = freeTypeInvoker;
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
        public float LineSpacing { get; internal set; }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc cref="IContent.IsPooled"/>
        public bool IsPooled { get; set; }

        /// <inheritdoc/>
        public ReadOnlyCollection<GlyphMetrics> Metrics => this.metrics.ToReadOnlyCollection();

        /// <inheritdoc/>
        public char[] GetAvailableGlyphCharacters() => this.availableGlyphCharacters;

        /// <inheritdoc/>
        public SizeF Measure(string text)
        {
            var foundGlyphs = new List<GlyphMetrics>();

            foreach (var character in text)
            {
                var foundGlyph = this.metrics.FirstOrDefault(g => g.Glyph == character);

                foundGlyphs.Add(foundGlyph);
            }

            // Total all of the widths of the characters in the text
            var width = 0f;

            var leftGlyphIndex = 0u;
            var facePtr = this.freeTypeInvoker.GetFace();

            // Total all of the space between each character,
            // except the space before the first character. Also
            // Take into account any kerning
            foreach (var currentGlyph in foundGlyphs)
            {
                if (HasKerning && leftGlyphIndex != 0 && currentGlyph.CharIndex != 0)
                {
                    // TODO: Check the perf for curiosity reasons
                    FT_Vector delta = this.freeTypeInvoker.FT_Get_Kerning(
                        facePtr,
                        leftGlyphIndex,
                        currentGlyph.CharIndex,
                        (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

                    width += delta.x.ToInt32() >> 6;
                }

                width += currentGlyph.HorizontalAdvance;

                leftGlyphIndex = currentGlyph.CharIndex;
            }

            var maxHeight = foundGlyphs.Max(i => i.GlyphHeight);
            var maxDecent = foundGlyphs.Max(i => i.GlyphHeight - i.HoriBearingY);
            var height = maxHeight + maxDecent;

            return new SizeF(width, height);
        }

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
