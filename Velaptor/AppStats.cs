// <copyright file="AppStats.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Records and retrieves information about the running application.
    /// </summary>
    public static class AppStats
    {
        private const string DefaultTag = "[DEFAULT]";
        private static readonly Queue<(uint frame, char glyph, uint textureId, RectangleF srcRect)> Textures = new ();
        private static readonly List<(string fontFileName, string fontSize)> LoadedFonts = new ();

        /// <summary>
        /// Gets all of the rendered textures for the last 2 frames in <c>string</c> format.
        /// </summary>
        /// <returns>The recorded frame info.</returns>
        public static string GetFontGlyphRenderingData()
        {
            var result = string.Empty;

            var largestFrame = Textures.Max(i => i.frame);

            foreach (var (frame, glyph, textureId, srcRect) in Textures)
            {
                if (frame == largestFrame)
                {
                    result += $"Frame: {frame} | Glyph: {glyph} | Texture ID: {textureId} | Source Rect: {srcRect}\n";
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all of the loaded fonts.
        /// </summary>
        /// <returns>The string result of all the loaded fonts.</returns>
        public static string GetLoadedFonts()
        {
            var result = string.Empty;

            foreach (var loadedFont in LoadedFonts)
            {
                result += $"Font File: {loadedFont.fontFileName} | Font Size: {loadedFont.fontSize}\n";
            }

            return result;
        }

        /// <summary>
        /// Records information about the textures that have been rendered for the 2 most recent frames.
        /// </summary>
        /// <param name="frame">The application frame.</param>
        /// <param name="glyph">The glyph being rendered.</param>
        /// <param name="fontAtlasTextureId">The font atlas texture ID.</param>
        /// <param name="destRect">The destination rectangle of the glyph.</param>
        internal static void RecordFontGlyphRendering(uint frame, char glyph, uint fontAtlasTextureId, RectangleF destRect)
        {
            Textures.Enqueue((frame, glyph, fontAtlasTextureId, destRect));

            var largestFrame = Textures.Max(i => i.frame);
            var secondLargest = 0u;

            // Collect all of the unique frame numbers that have been rendered
            var frames = (from texture in Textures
                orderby texture.frame descending
                select texture.frame).Distinct().ToArray();

            var distinctResult = frames.Distinct().ToArray();

            // As long as there is at least 2 frames of info recorded
            if (distinctResult.Length >= 2)
            {
                secondLargest = distinctResult.Skip(1).First();
            }

            // As long as there are frames recorded, attempt to clean up all frames
            // that are older than the last 2 recorded frames
            if (largestFrame != 0u && secondLargest != 0u)
            {
                Textures.DequeueWhile(_ => frames.Length > 2);
            }
        }

        /// <summary>
        /// Records the loaded font.
        /// </summary>
        /// <param name="fontInfo">The loaded font info.</param>
        internal static void RecordLoadedFont(string fontInfo)
        {
            if (string.IsNullOrEmpty(fontInfo) is not false || !fontInfo.Contains("|size:"))
            {
                return;
            }

            var sections = fontInfo.Split('|');

            // If there is not 2 sections and it does not contain any font meta data
            if (sections.Length != 2 || !sections[1].Contains(':'))
            {
                return;
            }

            var fontSizeSections = sections[1].Split(':');

            var containsDefaultTag = sections[0].Contains(DefaultTag);

            var fileName = containsDefaultTag
                ? $"{DefaultTag}{Path.GetFileName(sections[0])}"
                : Path.GetFileName(sections[0]);

            var dataItem = (fileName, fontSizeSections[1]);

            if (LoadedFonts.Contains(dataItem) is false)
            {
                LoadedFonts.Add(dataItem);
            }
        }

        /// <summary>
        /// Clears the given <paramref name="fontInfo"/>.
        /// </summary>
        /// <param name="fontInfo">Clears the loaded font info.</param>
        internal static void ClearLoadedFont(string fontInfo)
        {
            if (string.IsNullOrEmpty(fontInfo) is false &&
                fontInfo.Contains("|size:"))
            {
                var sections = fontInfo.Split('|');

                if (sections.Length == 2 && sections[1].Contains(':'))
                {
                    var fontSizeSections = sections[1].Split(':');

                    var dataItem = (fontSizeSections[0], fontSizeSections[1]);

                    if (LoadedFonts.Contains(dataItem) is false)
                    {
                        LoadedFonts.Remove(dataItem);
                    }
                }
            }
        }
    }
}
