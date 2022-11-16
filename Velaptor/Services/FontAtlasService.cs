// <copyright file="FontAtlasService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Velaptor.Content.Fonts.Services;
using Velaptor.Exceptions;
using Velaptor.Graphics;
using Velaptor.Guards;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectangle = System.Drawing.Rectangle;

namespace Velaptor.Services;

/// <summary>
/// Creates font atlas data for rendering text.
/// </summary>
internal sealed class FontAtlasService : IFontAtlasService
{
    private readonly IFontService fontService;
    private readonly IImageService imageService;
    private readonly ISystemMonitorService monitorService;
    private readonly IFile file;
    private readonly char[] glyphChars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
        '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ', '□',
    };
    private IntPtr facePtr;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontAtlasService"/> class.
    /// </summary>
    /// <param name="fontService">Provides extensions/helpers to <c>FreeType</c> library functionality.</param>
    /// <param name="imageService">Manages image data.</param>
    /// <param name="systemMonitorService">Provides information about the system monitors.</param>
    /// <param name="file">Performs operations with files.</param>
    public FontAtlasService(
        IFontService fontService,
        IImageService imageService,
        ISystemMonitorService systemMonitorService,
        IFile file)
    {
        EnsureThat.ParamIsNotNull(fontService);
        EnsureThat.ParamIsNotNull(imageService);
        EnsureThat.ParamIsNotNull(systemMonitorService);
        EnsureThat.ParamIsNotNull(file);

        this.fontService = fontService;
        this.imageService = imageService;
        this.monitorService = systemMonitorService;
        this.file = file;
    }

    /// <inheritdoc/>
    public (ImageData atlasImage, GlyphMetrics[] atlasData) CreateFontAtlas(string fontFilePath, uint sizeInPoints)
    {
        if (string.IsNullOrEmpty(fontFilePath))
        {
            throw new ArgumentNullException(nameof(fontFilePath), "The font file path argument must not be null.");
        }

        if (this.file.Exists(fontFilePath) is false)
        {
            throw new FileNotFoundException($"The file '{fontFilePath}' does not exist.");
        }

        this.facePtr = this.fontService.CreateFontFace(fontFilePath);

        if (this.monitorService.MainMonitor is null)
        {
            throw new SystemMonitorException("The main system monitor must not be null.");
        }

        this.fontService.SetFontSize(
            this.facePtr,
            sizeInPoints);

        var glyphIndices = this.fontService.GetGlyphIndices(this.facePtr, this.glyphChars);

        var glyphImages = CreateGlyphImages(glyphIndices);

        var glyphMetrics = this.fontService.CreateGlyphMetrics(this.facePtr, glyphIndices);

        var fontAtlasMetrics = CalcAtlasMetrics(glyphImages);

        var atlasImage = new ImageData(
            new NETColor[fontAtlasMetrics.Width, fontAtlasMetrics.Height],
            fontAtlasMetrics.Width,
            fontAtlasMetrics.Height);

        glyphMetrics = SetGlyphMetricsAtlasBounds(glyphImages, glyphMetrics, fontAtlasMetrics.Columns);

        // Render each glyph image to the atlas
        foreach (var glyphImage in glyphImages)
        {
            var drawLocation = new PointF(glyphMetrics[glyphImage.Key].GlyphBounds.X, glyphMetrics[glyphImage.Key].GlyphBounds.Y);

            atlasImage = this.imageService.Draw(
                glyphImage.Value,
                atlasImage,
                new NETPoint((int)drawLocation.X, (int)drawLocation.Y));
        }

        return (atlasImage, glyphMetrics.Values.ToArray());
    }

    /// <summary>
    /// Calculates all of the font atlas metrics using the given <paramref name="glyphImages"/>.
    /// </summary>
    /// <param name="glyphImages">The glyph images that will eventually be rendered onto the font texture atlas.</param>
    /// <returns>The various metrics of the font atlas.</returns>
    private static FontAtlasMetrics CalcAtlasMetrics(Dictionary<char, ImageData> glyphImages)
    {
        FontAtlasMetrics result = default;

        const int antiEdgeCroppingMargin = 3;
        var maxGlyphWidth = glyphImages.Max(g => g.Value.Width) + antiEdgeCroppingMargin;
        var maxGlyphHeight = glyphImages.Max(g => g.Value.Height) + antiEdgeCroppingMargin;

        var possibleRowAndColumnCount = Math.Sqrt(glyphImages.Count);

        result.Rows = possibleRowAndColumnCount % 2 != 0
            ? (uint)Math.Round(possibleRowAndColumnCount + 1.0, MidpointRounding.ToZero)
            : (uint)possibleRowAndColumnCount;

        // Add an extra row for certain situations where there are couple extra glyph chars
        result.Rows++;

        result.Columns = result.Rows;

        result.Width = maxGlyphWidth * result.Columns;
        result.Height = maxGlyphHeight * result.Rows;

        return result;
    }

    /// <summary>
    /// Sets all of the atlas bounds for each glyph in the given <paramref name="glyphMetrics"/>.
    /// </summary>
    /// <param name="glyphImages">The glyph images that will eventually be rendered to the font atlas.</param>
    /// <param name="glyphMetrics">The metrics for each glyph.</param>
    /// <param name="columnCount">The number of columns in the atlas.</param>
    /// <returns>
    ///     The <paramref name="glyphMetrics"/> is the font atlas texture data that will eventually be returned.
    /// </returns>
    private static Dictionary<char, GlyphMetrics> SetGlyphMetricsAtlasBounds(Dictionary<char, ImageData> glyphImages, Dictionary<char, GlyphMetrics> glyphMetrics, uint columnCount)
    {
        const int antiEdgeCroppingMargin = 3;

        var maxGlyphWidth = glyphImages.Max(g => g.Value.Width) + antiEdgeCroppingMargin;
        var maxGlyphHeight = glyphImages.Max(g => g.Value.Height) + antiEdgeCroppingMargin;

        var cellX = 0;
        var cellY = 0;

        foreach (var glyph in glyphImages)
        {
            var xPos = cellX * maxGlyphWidth;
            var yPos = cellY * maxGlyphHeight;

            var glyphMetric = (from m in glyphMetrics
                where m.Value.Glyph == glyph.Key
                select m.Value).FirstOrDefault();

            glyphMetric.GlyphBounds = new NETRectangle((int)xPos, (int)yPos, (int)glyph.Value.Width, (int)glyph.Value.Height);
            glyphMetrics[glyph.Key] = glyphMetric;

            if (cellX >= columnCount - 1)
            {
                cellX = 0;
                cellY += 1;

                continue;
            }

            cellX += 1;
        }

        return glyphMetrics;
    }

    /// <summary>
    /// Takes the given 8-bit grayscale glyph bitmap data in raw bytes with the glyph
    /// bitmap <paramref name="width"/> and <paramref name="height"/> and returns it
    /// as a white 32-bit RGBA image.
    /// </summary>
    /// <param name="pixelData">The 8-bit grayscale glyph bitmap data.</param>
    /// <param name="width">The width of the glyph bitmap.</param>
    /// <param name="height">The height of the glyph bitmap.</param>
    /// <returns>The 32-bit RGBA glyph image data.</returns>
    private static ImageData ToImageData(byte[] pixelData, uint width, uint height)
    {
        var imageData = new NETColor[width, height];
        var iteration = 0;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                imageData[x, y] = NETColor.FromArgb(pixelData[iteration], 255, 255, 255);
                iteration++;
            }
        }

        return new ImageData(imageData, width, height);
    }

    /// <summary>
    /// Creates all of the glyph images for each glyph.
    /// </summary>
    /// <param name="glyphIndices">The glyph index for each glyph.</param>
    /// <returns>The glyph image data for each glyph/character.</returns>
    private Dictionary<char, ImageData> CreateGlyphImages(Dictionary<char, uint> glyphIndices)
    {
        var result = new Dictionary<char, ImageData>();

        foreach (var glyphKeyValue in glyphIndices)
        {
            if (glyphKeyValue.Key == ' ')
            {
                continue;
            }

            var (pixelData, width, height) = this.fontService.CreateGlyphImage(this.facePtr, glyphKeyValue.Key, glyphKeyValue.Value);

            var glyphImage = ToImageData(pixelData, width, height);

            result.Add(glyphKeyValue.Key, glyphImage);
        }

        return result;
    }
}
