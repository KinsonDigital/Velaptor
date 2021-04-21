// <copyright file="FontAtlasService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using NETColor = System.Drawing.Color;
    using NETPoint = System.Drawing.Point;
    using NETRectangle = System.Drawing.Rectangle;

    /// <summary>
    /// Creates font atlas textures for rendering text.
    /// </summary>
    internal class FontAtlasService : IFontAtlasService
    {
        private const char InvalidCharacter = '□';
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFreeTypeExtensions freeTypeExtensions;
        private readonly IImageService imageService;
        private readonly ISystemMonitorService monitorService;
        private readonly IFile file;
        private readonly IntPtr freeTypeLibPtr;
        private IntPtr facePtr;
        private char[]? glyphChars;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAtlasService"/> class.
        /// </summary>
        /// <param name="freeTypeInvoker">Provides low level calls to the FreeType2 library.</param>
        /// <param name="freeTypeExtensions">Provides extensions/helpers to free type library functionality.</param>
        /// <param name="imageService">Manages image data.</param>
        /// <param name="systemMonitorService">Provides information about the system monitor.</param>
        /// <param name="file">Performs file operations.</param>
        public FontAtlasService(
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            IImageService imageService,
            ISystemMonitorService systemMonitorService,
            IFile file)
        {
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.imageService = imageService;
            this.monitorService = systemMonitorService;
            this.file = file;

            this.freeTypeLibPtr = this.freeTypeInvoker.FT_Init_FreeType();

            this.freeTypeInvoker.OnError += FreeTypeInvoker_OnError;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FontAtlasService"/> class.
        /// </summary>
        ~FontAtlasService() => Dispose(false);

        /// <inheritdoc/>
        public (ImageData atlasImage, GlyphMetrics[] atlasData) CreateFontAtlas(string fontFilePath, int size)
        {
            if (this.glyphChars is null)
            {
                throw new InvalidOperationException("The available glyph characters must be set first before creating a font texture atlas.");
            }

            if (string.IsNullOrEmpty(fontFilePath))
            {
                throw new ArgumentNullException(nameof(fontFilePath), "The font file path argument must not be null.");
            }

            if (this.file.Exists(fontFilePath) is false)
            {
                throw new FileNotFoundException($"The file '{fontFilePath}' does not exist.");
            }

            this.facePtr = this.freeTypeExtensions.CreateFontFace(this.freeTypeLibPtr, fontFilePath);

            if (this.monitorService.MainMonitor is null)
            {
                throw new SystemDisplayException("The main system display must not be null.");
            }

            this.freeTypeExtensions.SetCharacterSize(
                this.facePtr,
                size,
                (uint)this.monitorService.MainMonitor.HorizontalDPI,
                (uint)this.monitorService.MainMonitor.VerticalDPI);

            var glyphIndices = this.freeTypeExtensions.GetGlyphIndices(this.facePtr, this.glyphChars);

            var glyphImages = CreateGlyphImages(glyphIndices);

            var glyphMetrics = this.freeTypeExtensions.CreateGlyphMetrics(this.facePtr, glyphIndices);

            var fontAtlasMetrics = CalcAtlasMetrics(glyphImages);

            ImageData atlasImage = default;
            atlasImage.Pixels = new NETColor[fontAtlasMetrics.Width, fontAtlasMetrics.Height];
            atlasImage.Width = fontAtlasMetrics.Width;
            atlasImage.Height = fontAtlasMetrics.Height;

            glyphMetrics = SetGlyphMetricsAtlasBounds(glyphImages, glyphMetrics, fontAtlasMetrics.Columns);

            // Render each glyph image to the atlas
            foreach (var glyphImage in glyphImages)
            {
                var drawLocation = new NETPoint(glyphMetrics[glyphImage.Key].AtlasBounds.X, glyphMetrics[glyphImage.Key].AtlasBounds.Y);

                atlasImage = this.imageService.Draw(glyphImage.Value, atlasImage, drawLocation);
            }

            return (atlasImage, glyphMetrics.Values.ToArray());
        }

        /// <inheritdoc/>
        public void SetAvailableCharacters(char[] glyphChars)
        {
            // Make sure to add the '□' character to represent missing characters
            // This will be rendered in place of characters that do not exist
            var currentList = glyphChars.ToList();

            if (currentList.Contains(InvalidCharacter) is false)
            {
                currentList.Add(InvalidCharacter);
            }

            this.glyphChars = currentList.ToArray();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.freeTypeInvoker.OnError -= FreeTypeInvoker_OnError;
                    this.freeTypeInvoker.Dispose();
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Calculates all of the font atlas metrics using the given <paramref name="glyphImages"/>.
        /// </summary>
        /// <param name="glyphImages">The glyph images that will eventually be rendered onto the font texture atlas.</param>
        /// <returns>The various metrics of the font atlas.</returns>
        private static FontAtlasMetrics CalcAtlasMetrics(Dictionary<char, ImageData> glyphImages)
        {
            FontAtlasMetrics result = default;

            const int AntiEdgeCroppingMargin = 3;
            var maxGlyphWidth = glyphImages.Max(g => g.Value.Width) + AntiEdgeCroppingMargin;
            var maxGlyphHeight = glyphImages.Max(g => g.Value.Height) + AntiEdgeCroppingMargin;

            var possibleRowAndColumnCount = Math.Sqrt(glyphImages.Count);

            result.Rows = possibleRowAndColumnCount % 2 != 0
                ? (int)Math.Round(possibleRowAndColumnCount + 1.0, MidpointRounding.ToZero)
                : (int)possibleRowAndColumnCount;

            // Add an extra row for certain situations where there are couple extra glyph chars
            result.Rows++;

            result.Columns = (int)possibleRowAndColumnCount;

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
        private static Dictionary<char, GlyphMetrics> SetGlyphMetricsAtlasBounds(Dictionary<char, ImageData> glyphImages, Dictionary<char, GlyphMetrics> glyphMetrics, int columnCount)
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

                glyphMetric.AtlasBounds = new NETRectangle(xPos, yPos, glyph.Value.Width, glyph.Value.Height);
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
        private static ImageData ToImageData(byte[] pixelData, int width, int height)
        {
            ImageData image = default;
            image.Pixels = new NETColor[width, height];
            image.Width = width;
            image.Height = height;

            var iteration = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    image.Pixels[x, y] = NETColor.FromArgb(pixelData[iteration], 255, 255, 255);
                    iteration++;
                }
            }

            return image;
        }

        /// <summary>
        /// Occurs when there is a free type associated error.
        /// </summary>
        private void FreeTypeInvoker_OnError(object? sender, FreeTypeErrorEventArgs e)
        {
            // TODO: Throw custom free type exception here
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

                var (pixelData, width, height) = this.freeTypeExtensions.CreateGlyphImage(this.facePtr, glyphKeyValue.Key, glyphKeyValue.Value);

                var glyphImage = ToImageData(pixelData, width, height);

                result.Add(glyphKeyValue.Key, glyphImage);
            }

            return result;
        }
    }
}
