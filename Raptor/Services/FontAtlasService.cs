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
    using System.Runtime.InteropServices;
    using FreeTypeSharp.Native;
    using Raptor.Exceptions;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using NETColor = System.Drawing.Color;
    using NETPoint = System.Drawing.Point;
    using NETRectangle = System.Drawing.Rectangle;

    /* TODO List:
        ✔ 1. Check if the class consuming/injecting this service can be used by the library user.
        If so, then this class cannot be internal.  You might have to make an internal constructor
        or possibly do something different.

        ✔ 2. Add interface with IDisposble
        ✔ 3. Unregister invoker event in Dispose()
        4. Invoke dispose on invoker.  Do we want each invoker to represent a single lib and lib pointer?  Or doe
            we want a single invoker (singleton) to manage all initiated lib pointers to free type?
            * I am thinking we should have each instance of the invoker manage its own pointer
              to a single initiated free type library
        6. This service is only needed for the font atlas creation process in the FontLoaderService class.
            Look into possibly disposing of this service which in turn will dipose of the FreeTypeInvoker
            after the atlas is created.  If this is the route that is taken, make sure that the FreeType lib
            cannot be called again indirectly from the FontLoaderService class which would attempt a FreeType
            call after it has been disposed.
        6. Look into finding a way to know which monitor the window is on.  This way if there are 2 monitors
            and the window is dragged onto the 2nd monitor that has a different DPI setting, the font character
            size can be updated accordinly.  This is a possibly major issue because it would require recreation
            of the entire font atlas during runtime.  Also, if the FreeTypeInvoker has been disposed after the creation
            of the first atlas, it will need to be reinitialized.
        8. Look into this link in reguards to new unsafe class in C# 7.0 => https://ndportmann.com/system-runtime-compilerservices-unsafe/
            * This could make things faster
        9. Find a way to render an empty magenta box to the screen with the with of a space character when the attempted
            * glyph does not exist.
        10. Improve performance.  Example: font size of 54 renders a much bigger atlas and its too slow
    */

    /// <summary>
    /// Creates font atlas textures for rendering text.
    /// </summary>
    internal class FontAtlasService : IFontAtlasService
    {
        private const int AntiEdgeCroppingMargin = 3;
        private const char InvalidCharacter = '□';
        private readonly IFreeTypeInvoker freeTypeInvoker;
        private readonly IImageService imageService;
        private readonly ISystemMonitorService monitorService;
        private readonly IFile file;
        private readonly IntPtr freeTypeLibPtr;
        private IntPtr facePtr;
        private string fontFilePath = string.Empty;
        private char[]? glyphChars;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAtlasService"/> class.
        /// </summary>
        /// <param name="freeTypeInvoker">Provides low level calls to the FreeType2 library.</param>
        /// <param name="imageService">Manages image data.</param>
        /// <param name="monitorService">Provids information about the system monitors.</param>
        /// <param name="file">Performs file operations.</param>
        public FontAtlasService(
            IFreeTypeInvoker freeTypeInvoker,
            IImageService imageService,
            ISystemMonitorService monitorService,
            IFile file)
        {
            this.freeTypeInvoker = freeTypeInvoker;
            this.imageService = imageService;
            this.monitorService = monitorService;
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

            this.fontFilePath = fontFilePath;

            CreateFontFace();
            SetCharacterSize(size);

            var glyphIndices = GetGlyphIndices();

            var glyphImages = CreateGlyphImages(glyphIndices);

            var glyphMetrics = CreateGlyphMetrics(glyphIndices);

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
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.freeTypeInvoker.OnError -= FreeTypeInvoker_OnError;
                }

                // TODO: Need to figure out how to call FT_Done_Glyph() in a safe way
                // This implimentation below is causing issuess
                /*
                unsafe
                {
                    var unsafePtr = (FT_FaceRec*)this.facePtr;

                    var glyphPtr = (IntPtr)unsafePtr->glyph;

                    this.freeTypeInvoker.FT_Done_Glyph(glyphPtr);
                }
                 */

                this.freeTypeInvoker.FT_Done_Face(this.facePtr);
                this.freeTypeInvoker.FT_Done_FreeType(this.freeTypeLibPtr);

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
        private static ImageData ToImage(byte[] pixelData, int width, int height)
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
        /// Gets all of the font indices fron the font file for each glyph.
        /// </summary>
        /// <returns>The index for each glyph.</returns>
        private Dictionary<char, uint> GetGlyphIndices()
        {
            if (this.glyphChars is null)
            {
                return new Dictionary<char, uint>();
            }

            var result = new Dictionary<char, uint>();

            for (var i = 0; i < this.glyphChars.Length; i++)
            {
                var glyphChar = this.glyphChars[i];

                // Get the glyph image and the character map index
                var charIndex = this.freeTypeInvoker.FT_Get_Char_Index(this.facePtr, glyphChar);

                result.Add(glyphChar, charIndex);
            }

            return result;
        }

        /// <summary>
        /// Creates all of the glyph images for each glyph.
        /// </summary>
        /// <param name="glyphIndices">The glyph index for each glyph.</param>
        /// <returns>The glyph image data for each glyph/character.</returns>
        private Dictionary<char, ImageData> CreateGlyphImages(Dictionary<char, uint> glyphIndices)
        {
            var result = new Dictionary<char, ImageData>();

            foreach (var glyphKeyvalue in glyphIndices)
            {
                if (glyphKeyvalue.Key == ' ')
                {
                    continue;
                }

                var glyphImage = CreateGlyphImage(glyphKeyvalue.Key, glyphKeyvalue.Value);

                result.Add(glyphKeyvalue.Key, glyphImage);
            }

            return result;
        }

        /// <summary>
        /// Creates all of the glyph metrics for each glyph.
        /// </summary>
        /// <param name="glyphIndices">The glyph index for each glyph.</param>
        /// <returns>The glyph metrics for each glyph/character.</returns>
        /// <remarks>
        ///     The <paramref name="glyphMetrics"/> is the font atlas texture data that will eventually be returned.
        /// </remarks>
        private Dictionary<char, GlyphMetrics> CreateGlyphMetrics(Dictionary<char, uint> glyphIndices)
        {
            var result = new Dictionary<char, GlyphMetrics>();

            foreach (var glyphKeyValue in glyphIndices)
            {
                GlyphMetrics metric = default;

                this.freeTypeInvoker.FT_Load_Glyph(
                    this.facePtr,
                    glyphKeyValue.Value,
                    FT.FT_LOAD_BITMAP_METRICS_ONLY);

                unsafe
                {
                    var face = Marshal.PtrToStructure<FT_FaceRec>(this.facePtr);

                    metric.Ascender = face.size->metrics.ascender.ToInt32() >> 6;
                    metric.Descender = face.size->metrics.descender.ToInt32() >> 6;
                    metric.Glyph = glyphKeyValue.Key;
                    metric.CharIndex = glyphKeyValue.Value;

                    metric.XMin = face.bbox.xMin.ToInt32() >> 6;
                    metric.XMax = face.bbox.xMax.ToInt32() >> 6;
                    metric.YMin = face.bbox.yMin.ToInt32() >> 6;
                    metric.YMax = face.bbox.yMax.ToInt32() >> 6;

                    metric.GlyphWidth = face.glyph->metrics.width.ToInt32() >> 6;
                    metric.GlyphHeight = face.glyph->metrics.height.ToInt32() >> 6;
                    metric.HorizontalAdvance = face.glyph->metrics.horiAdvance.ToInt32() >> 6;
                    metric.HoriBearingX = face.glyph->metrics.horiBearingX.ToInt32() >> 6;
                    metric.HoriBearingY = face.glyph->metrics.horiBearingY.ToInt32() >> 6;
                }

                result.Add(glyphKeyValue.Key, metric);
            }

            var filteredItems = (from i in result
                                 where i.Value.HorizontalAdvance == 48
                                 select i).ToArray();

            return result;
        }

        /// <summary>
        /// Creates a new font face from the font file.
        /// </summary>
        private unsafe void CreateFontFace()
        {
            this.facePtr = this.freeTypeInvoker.FT_New_Face(this.freeTypeLibPtr, this.fontFilePath, 0);

            if (this.facePtr == IntPtr.Zero)
            {
                throw new LoadFontException("An invalid pointer value of zero was returned when creating a new font face.");
            }
        }

        /// <summary>
        /// Sets the nominal character size in points.
        /// </summary>
        /// <param name="sizeInPoints">The size in points to set the characters.</param>
        private void SetCharacterSize(int sizeInPoints)
        {
            var sizeInPointsPtr = (IntPtr)(sizeInPoints << 6);

            if (this.monitorService.MainMonitor is null)
            {
                throw new SystemDisplayException("The main system display must not be null.");
            }

            // TODO: Check if the main monitor is null and if so, throw exception
            this.freeTypeInvoker.FT_Set_Char_Size(
                this.facePtr,
                sizeInPointsPtr,
                sizeInPointsPtr,
                (uint)this.monitorService.MainMonitor.HorizontalDPI,
                (uint)this.monitorService.MainMonitor.VerticalDPI);
        }

        /// <summary>
        /// Pulls the 8-bit grayscale bitmap data for the given <paramref name="glyphChar"/>
        /// and returns it as a 32-bit RGBA image.
        /// </summary>
        /// <param name="glyphChar">The glyph character to create the image from.</param>
        /// <param name="glyphIndex">The index of the glyph in the font file.</param>
        /// <returns>The 32-bit RGBA iamge of the glyph.</returns>
        private unsafe ImageData CreateGlyphImage(char glyphChar, uint glyphIndex)
        {
            var face = Marshal.PtrToStructure<FT_FaceRec>(this.facePtr);

            this.freeTypeInvoker.FT_Load_Glyph(this.facePtr, glyphIndex, FT.FT_LOAD_RENDER);

            var width = (int)face.glyph->bitmap.width;
            var height = (int)face.glyph->bitmap.rows;

            var glyphBitmapData = new byte[width * height];
            Marshal.Copy(face.glyph->bitmap.buffer, glyphBitmapData, 0, glyphBitmapData.Length);

            var glyphImage = ToImage(glyphBitmapData, width, height);

            return glyphImage;
        }
    }
}
