// <copyright file="Font.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Caching;
using Exceptions;
using Services;
using Graphics;
using Guards;
using Velaptor.Services;
using VelFontStyle = FontStyle;

/// <summary>
/// Represents a font with a set size and style that can be used to render text to the screen.
/// </summary>
public sealed class Font : IFont
{
    private const char InvalidCharacter = '□';
    private readonly IFontService fontService;
    private readonly IFontStatsService fontStatsService;
    private readonly IFontAtlasService fontAtlasService;
    private readonly IItemCache<string, ITexture> textureCache;
    private readonly IntPtr facePtr;
    private readonly GlyphMetrics invalidGlyph;
    private readonly char[] availableGlyphCharacters =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
        '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
    };
    private readonly bool fontInitialized;
    private GlyphMetrics[] metrics;
    private FontStats[]? fontStats;
    private FontStyle fontStyle;
    private uint size;

    /// <summary>
    /// Initializes a new instance of the <see cref="Font"/> class.
    /// </summary>
    /// <param name="texture">The font atlas texture that contains bitmap data for all of the available glyphs.</param>
    /// <param name="fontService">Provides extensions/helpers to <c>FreeType</c> library functionality.</param>
    /// <param name="fontStatsService">Used to gather stats about content or system fonts.</param>
    /// <param name="fontAtlasService">Creates font atlas textures and glyph metric data.</param>
    /// <param name="textureCache">Creates and caches textures for later retrieval.</param>
    /// <param name="name">The name of the font content.</param>
    /// <param name="fontFilePath">The path to the font content.</param>
    /// <param name="size">The size to set the font.</param>
    /// <param name="isDefaultFont">True if the font is a default font.</param>
    /// <param name="glyphMetrics">The glyph metric data including the atlas location of all glyphs in the atlas.</param>
    internal Font(
        ITexture texture,
        IFontService fontService,
        IFontStatsService fontStatsService,
        IFontAtlasService fontAtlasService,
        IItemCache<string, ITexture> textureCache,
        string name,
        string fontFilePath,
        uint size,
        bool isDefaultFont,
        GlyphMetrics[] glyphMetrics)
    {
        EnsureThat.ParamIsNotNull(texture);
        EnsureThat.ParamIsNotNull(fontService);
        EnsureThat.ParamIsNotNull(fontStatsService);
        EnsureThat.ParamIsNotNull(fontAtlasService);
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.StringParamIsNotNullOrEmpty(name);

        FontTextureAtlas = texture;
        this.fontService = fontService;
        this.fontStatsService = fontStatsService;
        this.fontAtlasService = fontAtlasService;
        this.textureCache = textureCache;

        this.metrics = glyphMetrics;
        this.invalidGlyph = glyphMetrics.FirstOrDefault(m => m.Glyph == InvalidCharacter);

        this.facePtr = this.fontService.CreateFontFace(fontFilePath);

        this.size = size;
        Name = name;
        FilePath = fontFilePath;
        FamilyName = this.fontService.GetFamilyName(fontFilePath);
        IsDefaultFont = isDefaultFont;

        GetFontStatData(FilePath);

        HasKerning = this.fontService.HasKerning(this.facePtr);
        LineSpacing = this.fontService.GetFontScaledLineSpacing(this.facePtr, Size);

        this.fontInitialized = true;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public FontSource Source => this.fontStats?.Length <= 0
        ? FontSource.Unknown
        : (from s in this.fontStats where s.Style == this.fontStyle select s.Source).FirstOrDefault();

    /// <inheritdoc/>
    public string FilePath { get; }

    /// <inheritdoc/>
    public ITexture FontTextureAtlas { get; private set; }

    /// <inheritdoc/>
    public uint Size
    {
        get => this.size;
        set
        {
            this.size = value;

            if (this.fontInitialized && this.size > 0u)
            {
                RebuildFontAtlasTexture();
            }
        }
    }

    /// <inheritdoc/>
    public VelFontStyle Style
    {
        get => this.fontStyle;
        set
        {
            this.fontStyle = value;

            if (this.fontInitialized)
            {
                RebuildFontAtlasTexture();
            }
        }
    }

    /// <inheritdoc/>
    public bool IsDefaultFont { get; }

    /// <inheritdoc/>
    public IEnumerable<FontStyle> AvailableStylesForFamily
        => this.fontStats is null
            ? Array.Empty<VelFontStyle>().ToReadOnlyCollection()
            : this.fontStats.Select(s => s.Style).ToReadOnlyCollection();

    /// <inheritdoc/>
    public string FamilyName { get; }

    /// <inheritdoc/>
    public bool HasKerning { get; }

    /// <inheritdoc/>
    public float LineSpacing { get; private set; }

    /// <inheritdoc/>
    public ReadOnlyCollection<GlyphMetrics> Metrics => this.metrics.ToReadOnlyCollection();

    /// <inheritdoc/>
    public SizeF Measure(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return SizeF.Empty;
        }

        // TODO: Trim end of line escape sequences off the end
        var foundGlyphs = text.Select(character
            => this.metrics.FirstOrDefault(g => g.Glyph == character)).ToList();

        if (text.Any(c => this.availableGlyphCharacters.Contains(c) is false))
        {
            foundGlyphs.Add(this.invalidGlyph);
        }

        var leftCharacterIndex = 0u;

        text = text.TrimNewLineFromEnd();

        var lines = text.Split(Environment.NewLine).TrimAllEnds();

        SizeF MeasureLine(IEnumerable<GlyphMetrics> charMetrics)
        {
            var width = 0f;
            var height = 0f;

            // Total all of the space between each character, except the space before the first character.
            // Also takes into account any kerning.
            foreach (var currentCharacter in charMetrics)
            {
                width += HasKerning
                    ? this.fontService.GetKerning(this.facePtr, leftCharacterIndex, currentCharacter.CharIndex)
                    : 0;

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
    /// Gets the kerning value between two glyphs.
    /// </summary>
    /// <param name="leftGlyphIndex">The character index of the glyph to the left of the right glyph.</param>
    /// <param name="rightGlyphIndex">The character index of the glyph to the right of the left glyph.</param>
    /// <returns>The kerning (horizontal spacing) between the glyphs.</returns>
    /// <remarks>
    /// Refer to the URL below for more info.
    /// <para>https://freetype.org/freetype2/docs/glyphs/glyphs-4.html#section-1.</para>
    /// </remarks>
    public float GetKerning(uint leftGlyphIndex, uint rightGlyphIndex)
        => this.fontService.GetKerning(this.facePtr, leftGlyphIndex, rightGlyphIndex);

    /// <inheritdoc/>
    public IEnumerable<(char character, RectangleF bounds)> GetCharacterBounds(string text, Vector2 textPos)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<(char, RectangleF)>();
        }

        var textMetrics = new List<GlyphMetrics>();

        foreach (var character in text)
        {
            var foundGlyph = (from g in this.metrics
                where g.Glyph == character
                select g).FirstOrDefault();

            textMetrics.Add(foundGlyph);
        }

        var result = new List<(char character, RectangleF bounds)>();

        var leftGlyphIndex = 0u;

        foreach (var currentCharMetric in textMetrics)
        {
            textPos.X += GetKerning(leftGlyphIndex, currentCharMetric.CharIndex);

            // Calculate the height offset
            var heightOffset = currentCharMetric.GlyphHeight - currentCharMetric.HoriBearingY;

            // Adjust for characters that have a negative horizontal bearing Y
            // For example, the '_' character
            if (currentCharMetric.HoriBearingY < 0)
            {
                heightOffset += currentCharMetric.HoriBearingY;
            }

            // Create the destination rect
            RectangleF charBounds = default;
            charBounds.X = textPos.X;
            charBounds.Y = textPos.Y + heightOffset;
            charBounds.Width = currentCharMetric.GlyphBounds.Width <= 0 ? 1 : currentCharMetric.GlyphBounds.Width;
            charBounds.Height = currentCharMetric.GlyphBounds.Height <= 0 ? 1 : currentCharMetric.GlyphBounds.Height;

            result.Add((currentCharMetric.Glyph, charBounds));

            // Horizontally advance to the next glyph
            // Get the difference between the old glyph width
            // and the glyph width with the size applied
            textPos.X += currentCharMetric.HorizontalAdvance;

            leftGlyphIndex = currentCharMetric.CharIndex;
        }

        return result.ToArray();
    }

    /// <summary>
    /// Gets all of the stats for a font at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The file path to the font file.</param>
    private void GetFontStatData(string filePath)
    {
        this.fontStyle = this.fontService.GetFontStyle(filePath);

        // First collect all of the data from the content directory
        this.fontStats = this.fontStatsService.GetContentStatsForFontFamily(FamilyName);

        var allStyles = new[]
        {
            FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Bold | FontStyle.Italic,
        };

        bool AllStylesFound()
        {
            const FontStyle boldItalic = FontStyle.Bold | FontStyle.Italic;

            return this.fontStats.Length == 4 && this.fontStats.All(d =>
                d.Style is FontStyle.Regular or FontStyle.Bold or FontStyle.Italic or boldItalic);
        }

        // If all four styles have been found and finished
        if (AllStylesFound())
        {
            return;
        }

        // If all of the font styles were not found, attempt to find them in the systems directory
        var missingStyles = (from style in allStyles
            where this.fontStats.Any(s => s.Style == style) is false
            select style).ToArray();

        // Try to find each missing style in the system fonts
        var systemFontStats = this.fontStatsService.GetSystemStatsForFontFamily(FamilyName);

        var missingFontStyles = (from f in systemFontStats
            where missingStyles.Contains(f.Style)
            select f).ToArray();

        var newList = new List<FontStats>();
        newList.AddRange(missingFontStyles);
        newList.AddRange(this.fontStats);
        this.fontStats = newList.ToArray();
    }

    /// <summary>
    /// Rebuilds the font atlas texture and glyph metrics.
    /// </summary>
    /// <exception cref="LoadFontException">Thrown if the current style that is being attempted does not exist.</exception>
    private void RebuildFontAtlasTexture()
    {
        var fontFilePath = (from s in this.fontStats
            where s.Style == this.fontStyle
            select s.FontFilePath).FirstOrDefault();

        if (string.IsNullOrEmpty(fontFilePath))
        {
            throw new LoadFontException($"The font style '{this.fontStyle}' does not exist for the font family '{FamilyName}'.");
        }

        var filePathWithMetaData = $"{fontFilePath}|size:{Size}";
        FontTextureAtlas = this.textureCache.GetItem(filePathWithMetaData);

        var (_, glyphMetrics) = this.fontAtlasService.CreateFontAtlas(fontFilePath, Size);

        LineSpacing = this.fontService.GetFontScaledLineSpacing(this.facePtr, Size);

        this.metrics = glyphMetrics;
    }
}
