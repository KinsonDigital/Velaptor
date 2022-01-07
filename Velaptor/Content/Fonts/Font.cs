// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using FreeTypeSharp.Native;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.FreeType;
    using VelFontStyle = Velaptor.Content.Fonts.FontStyle;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Represents a font with a particular size that
    /// can be used to render text to the screen.
    /// </summary>
    public sealed class Font : IFont
    {
        private const char InvalidCharacter = '□';
        private readonly GlyphMetrics[] metrics;
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFreeTypeExtensions freeTypeExtensions;
        private readonly IFontStatsService fontStatsService;
        private readonly IntPtr facePtr;
        private readonly GlyphMetrics invalidGlyph;
        private readonly char[] availableGlyphCharacters =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };
        private FontStats[] fontStatData;
        private int size;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glyphs.</param>
        /// <param name="freeTypeInvoker">Invokes native FreeType function calls.</param>
        /// <param name="freeTypeExtensions">Provides extensions/helpers to free type library functionality.</param>
        /// <param name="fontStatsService">Used to gather stats about content or system fonts.</param>
        /// <param name="name">The name of the font content.</param>
        /// <param name="fontFilePath">The path to the font content.</param>
        /// <param name="size">The size to set the font to.</param>
        /// <param name="glyphMetrics">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
        // TODO: Change font size across project to uint
        internal Font(
            ITexture texture,
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            IFontStatsService fontStatsService,
            string name,
            string fontFilePath,
            int size,
            GlyphMetrics[] glyphMetrics)
        {
            FontTextureAtlas = texture;
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.fontStatsService = fontStatsService;
            this.metrics = glyphMetrics;
            this.invalidGlyph = glyphMetrics.FirstOrDefault(m => m.Glyph == InvalidCharacter);

            var libraryPtr = this.freeTypeInvoker.FT_Init_FreeType();
            this.facePtr = this.freeTypeExtensions.CreateFontFace(libraryPtr, fontFilePath);

            Size = size;
            Name = name;
            FilePath = fontFilePath;
            FamilyName = this.freeTypeExtensions.GetFamilyName(fontFilePath);

            GetFontStatData(FilePath);

            LineSpacing = this.freeTypeExtensions.GetFontScaledLineSpacing(this.facePtr, Size) * 64f;
            HasKerning = this.freeTypeExtensions.HasKerning(this.facePtr);
        }

        private void GetFontStatData(string path)
        {
            Style = this.freeTypeExtensions.GetFontStyle(path);

            // First collect all of the data from the content directory
            this.fontStatData = this.fontStatsService.GetContentStatsForFontFamily(FamilyName);

            var allStyles = new[]
            {
                FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Bold | FontStyle.Italic,
            };

            bool AllStylesFound() => this.fontStatData.Length == 4 && this.fontStatData.All(d => d.Style is FontStyle.Regular or
                    FontStyle.Bold or
                    FontStyle.Italic or
                    (FontStyle.Bold | FontStyle.Italic));

            // If all for styles have been found, then were finished
            if (AllStylesFound())
            {
                return;
            }

            // If all of the font styles were not found, attempt to find them in the systems directory
            var missingStyles = (from style in allStyles
                where this.fontStatData.Any(s => s.Style == style) is false
                select style).ToArray();

            // Try to find each missing style in the system fonts
            var systemFontFiles = this.fontStatsService.GetSystemStatsForFontFamily(FamilyName);

            var missingStylesFontStatData = (from f in systemFontFiles
                where missingStyles.Contains(f.Style)
                select f).ToArray();

            var newList = new List<FontStats>();
            newList.AddRange(missingStylesFontStatData);
            newList.AddRange(this.fontStatData);
            this.fontStatData = newList.ToArray();
        }

        private bool StyleExists(string path)
        {


            return false;
        }


        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string FilePath { get; }

        /// <inheritdoc/>
        public ITexture FontTextureAtlas { get; }

        // TODO: Need to reload and recreate the font data every time this value has changed and only if it has changed
        /// <inheritdoc/>
        public int Size
        {
            get => this.size;
            set
            {
                this.size = value;
            }
        }

        // TODO: Need to reload and recreate the font data every time this value has changed and only if it has changed
        /// <inheritdoc/>
        public VelFontStyle Style { get; private set; }

        /// <inheritdoc/>
        public string FamilyName { get; private set; }

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
