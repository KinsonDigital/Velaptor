using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Velaptor.Graphics;
using Velaptor.NativeInterop.FreeType;
using Velaptor.Services;

namespace Velaptor.Content
{
    internal class FontAtlasDataCache : IItemCache<(string filePath, uint size), (ImageData, GlyphMetrics[])>
    {
        private readonly char[] availableGlyphCharacters =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };
        private readonly ConcurrentDictionary<(string path, uint size), (ImageData imgData, GlyphMetrics[] metrics)> fontAtlasData = new ();
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IFreeTypeExtensions freeTypeExtensions;
        private readonly IImageService imageService;
        private readonly IFile file;
        private readonly IntPtr freeTypeLibPtr;

        public FontAtlasDataCache(
            IFreeTypeInvoker freeTypeInvoker,
            IFreeTypeExtensions freeTypeExtensions,
            IImageService imageService,
            IFile file)
        {
            this.freeTypeInvoker = freeTypeInvoker;
            this.freeTypeExtensions = freeTypeExtensions;
            this.imageService = imageService;
            this.file = file;

            this.freeTypeLibPtr = this.freeTypeInvoker.FT_Init_FreeType();
        }

        /// <inheritdoc/>
        public int TotalCachedItems => this.fontAtlasData.Count;

        public (ImageData, GlyphMetrics[]) GetItem((string filePath, uint size) pathAndSize)
        {
            return this.fontAtlasData.GetOrAdd(pathAndSize, filePathAndSize =>
            {
                var (filePath, size) = filePathAndSize;
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentNullException(nameof(filePath), "The font file path argument must not be null.");
                }

                if (this.file.Exists(filePath) is false)
                {
                    throw new FileNotFoundException($"The file '{filePath}' does not exist.");
                }

                var facePtr = this.freeTypeExtensions.CreateFontFace(this.freeTypeLibPtr, filePath);

                this.freeTypeExtensions.SetCharacterSize(
                    facePtr,
                    (int)size);

                var glyphIndices = this.freeTypeExtensions.GetGlyphIndices(facePtr, this.availableGlyphCharacters);

                var glyphImages = CreateGlyphImages(facePtr, glyphIndices);

                var glyphMetrics = this.freeTypeExtensions.CreateGlyphMetrics(facePtr, glyphIndices);

                var fontAtlasMetrics = CalcAtlasMetrics(glyphImages);

                var atlasImage = new ImageData(
                    new Color[fontAtlasMetrics.Width, fontAtlasMetrics.Height],
                    fontAtlasMetrics.Width,
                    fontAtlasMetrics.Height);

                // OpenGL origin Y is at the bottom instead of the top.  This means
                // that the current image data which is vertically oriented correctly, needs
                // to be flipped vertically before being sent to OpenGL.
                atlasImage = this.imageService.FlipVertically(atlasImage);

                glyphMetrics = SetGlyphMetricsAtlasBounds(glyphImages, glyphMetrics, fontAtlasMetrics.Columns);

                // Render each glyph image to the atlas
                foreach (var glyphImage in glyphImages)
                {
                    var drawLocation = new PointF(glyphMetrics[glyphImage.Key].GlyphBounds.X, glyphMetrics[glyphImage.Key].GlyphBounds.Y);

                    atlasImage = this.imageService.Draw(
                        glyphImage.Value,
                        atlasImage,
                        new Point((int)drawLocation.X, (int)drawLocation.Y));
                }

                return (atlasImage, glyphMetrics.Values.ToArray());
            });
        }

        public void Unload((string, uint) cacheKey)
        {

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

                glyphMetric.GlyphBounds = new RectangleF((int)xPos, (int)yPos, (int)glyph.Value.Width, (int)glyph.Value.Height);
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
            var imageData = new Color[width, height];
            var iteration = 0;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    imageData[x, y] = Color.FromArgb(pixelData[iteration], 255, 255, 255);
                    iteration++;
                }
            }

            return new ImageData(imageData, width, height);
        }

        /// <summary>
        /// Creates all of the glyph images for each glyph.
        /// </summary>
        /// <param name="facePtr">The pointer to the face for crating the images.</param>
        /// <param name="glyphIndices">The glyph index for each glyph.</param>
        /// <returns>The glyph image data for each glyph/character.</returns>
        private Dictionary<char, ImageData> CreateGlyphImages(IntPtr facePtr, Dictionary<char, uint> glyphIndices)
        {
            var result = new Dictionary<char, ImageData>();

            foreach (var glyphKeyValue in glyphIndices)
            {
                if (glyphKeyValue.Key == ' ')
                {
                    continue;
                }

                var (pixelData, width, height) = this.freeTypeExtensions.CreateGlyphImage(facePtr, glyphKeyValue.Key, glyphKeyValue.Value);

                var glyphImage = ToImageData(pixelData, width, height);

                result.Add(glyphKeyValue.Key, glyphImage);
            }

            return result;
        }
    }
}
