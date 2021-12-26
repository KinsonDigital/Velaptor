// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using FreeTypeSharp.Native;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.FreeType;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Represents a font with a particular size that
    /// can be used to render text to the screen.
    /// </summary>
    public sealed class Font : IFont
    {
        private const char InvalidCharacter = '□';
        private readonly char[] availableGlyphCharacters;
        private readonly GlyphMetrics[] metrics;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFreeTypeExtensions freeTypeExtensions;
        private readonly IntPtr facePtr;
        private readonly GlyphMetrics invalidGlyph;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glyphs.</param>
        /// <param name="freeTypeInvoker">Invokes native FreeType function calls.</param>
        /// <param name="freeTypeExtensions">Provides extensions/helpers to free type library functionality.</param>
        /// <param name="glyphMetrics">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
        /// <param name="fontSettings">The various font settings.</param>
        /// <param name="availableGlyphChars">The list of available glyph characters for this font.</param>
        /// <param name="name">The name of the font content.</param>
        /// <param name="path">The path to the font content.</param>
        internal Font(
            ITexture texture,
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            GlyphMetrics[] glyphMetrics,
            FontSettings fontSettings,
            char[] availableGlyphChars,
            string name,
            string path)
        {
            FontTextureAtlas = texture;
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.metrics = glyphMetrics;
            this.invalidGlyph = glyphMetrics.FirstOrDefault(m => m.Glyph == InvalidCharacter);

            this.facePtr = freeTypeInvoker.FT_Get_Face();
            Size = fontSettings.Size;
            Style = fontSettings.Style;
            this.availableGlyphCharacters = availableGlyphChars;
            Name = name;
            Path = path;
            LineSpacing = this.freeTypeExtensions.GetFontScaledLineSpacing(this.facePtr) * 64f;
            HasKerning = this.freeTypeExtensions.HasKerning(this.facePtr);
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        /// <inheritdoc/>
        public ITexture FontTextureAtlas { get; }

        /// <inheritdoc/>
        public int Size { get; private set; }

        /// <inheritdoc/>
        public FontStyle Style { get; private set; }

        /// <inheritdoc/>
        public bool HasKerning { get; private set; }

        /// <inheritdoc/>
        public float LineSpacing { get; private set; }

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
            if (string.IsNullOrEmpty(text))
            {
                return SizeF.Empty;
            }

            // TODO: Trim end of line escape sequences off the end
            // TODO: Look into caching measurements with a cache total that is maintained
            var foundGlyphs = text.Select(character
                => this.metrics.FirstOrDefault(g => g.Glyph == character)).ToList();

            if (text.Any(c => this.availableGlyphCharacters.Contains(c) is false))
            {
                foundGlyphs.Add(this.invalidGlyph);
            }

            var leftCharacterIndex = 0u;

            text = text.TrimEnd('\n');
            var lines = text.Split('\n').TrimAllEnds();

            SizeF MeasureLine(IEnumerable<GlyphMetrics> charMetrics)
            {
                var width = 0f;
                var height = 0f;

                // Total all of the space between each character,
                // except the space before the first character. Also
                // Take into account any kerning
                foreach (var currentCharacter in charMetrics)
                {
                    if (HasKerning && leftCharacterIndex != 0 && currentCharacter.CharIndex != 0)
                    {
                        var delta = this.freeTypeInvoker.FT_Get_Kerning(
                            this.facePtr,
                            leftCharacterIndex,
                            currentCharacter.CharIndex,
                            (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

                        width += delta.x.ToInt32() >> 6;
                    }

                    width += currentCharacter.HorizontalAdvance;

                    height = currentCharacter.GlyphHeight > height
                        ? currentCharacter.GlyphHeight
                        : height;

                    leftCharacterIndex = currentCharacter.CharIndex;
                }

                return new SizeF(width, height);
            }

            var lineSizes = new List<SizeF>();

            var totalHeight = 0f;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineGlyphs = line.Select(c =>
                        this.availableGlyphCharacters.Contains(c)
                            ? foundGlyphs.FirstOrDefault(g => g.Glyph == c)
                            : this.invalidGlyph)
                    .ToArray();

                var verticalOffset = 0f;

                var lastLine = i == lines.Length - 1;

                if (lastLine)
                {
                    verticalOffset = lineGlyphs.Max(g => g.GlyphHeight - g.HoriBearingY);
                }

                var lineSize = MeasureLine(lineGlyphs);
                totalHeight += lineSize.Height;
                totalHeight += lastLine ? 0 : LineSpacing - lineSize.Height;
                totalHeight += verticalOffset;

                lineSizes.Add(lineSize);
            }

            var largestWidth = lineSizes.Max(l => l.Width);

            return new SizeF(largestWidth, totalHeight);
        }

        /// <summary>
        /// Returns all of the glyph metrics for the given text.
        /// </summary>
        /// <param name="text">The text to convert to glyph metrics.</param>
        /// <returns>The list of glyph metrics of the given <paramref name="text"/>.</returns>
        public GlyphMetrics[] ToGlyphMetrics(string text)
        {
            var textGlyphs = this.metrics.Where(m => text.Contains(m.Glyph)).ToList();
            textGlyphs.Add(this.invalidGlyph);

            return text.Select(character
                => (from m in textGlyphs
                    where m.Glyph == (this.availableGlyphCharacters.Contains(character)
                        ? character
                        : InvalidCharacter)
                    select m).FirstOrDefault()).ToArray();
        }

        /// <summary>
        /// Gets the kerning value between 2 glyphs.
        /// </summary>
        /// <param name="leftGlyphIndex">The character index of the glyph to the left of the right glyph.</param>
        /// <param name="rightGlyphIndex">The character index of the glyph to the right of the left glyph.</param>
        /// <returns>The kerning (horizontal spacing) between the glyphs.</returns>
        /// <remarks>
        /// Refer to the URL below for more info.
        /// <para>https://freetype.org/freetype2/docs/glyphs/glyphs-4.html#section-1.</para>
        /// </remarks>
        public float GetKerning(uint leftGlyphIndex, uint rightGlyphIndex)
        {
            if (HasKerning is false || leftGlyphIndex == 0 || rightGlyphIndex == 0)
            {
                return 0;
            }

            var delta = this.freeTypeInvoker.FT_Get_Kerning(
                this.facePtr,
                leftGlyphIndex,
                rightGlyphIndex,
                (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

            return delta.x.ToInt32() >> 6;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
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
            }

            IsDisposed = true;
        }
    }
}
