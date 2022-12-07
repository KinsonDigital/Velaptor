// <copyright file="AppStats.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Records and retrieves information about the running application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AppStats
{
    private const string DefaultTag = "[DEFAULT]";
    private static readonly Queue<(uint frame, char glyph, uint textureId, float renderSize, RectangleF srcRect)> GlyphTextures = new ();
    private static readonly List<(string textureType, string name, uint textureId)> LoadedTextures = new ();
    private static readonly List<(string fontFileName, string fontSize)> LoadedFonts = new ();

    /// <summary>
    /// Gets all of the rendered textures for the last two frames in <c>string</c> format.
    /// </summary>
    /// <returns>The recorded frame information.</returns>
    public static string GetFontGlyphRenderingData()
    {
        var result = string.Empty;

        var largestFrame = GlyphTextures.Max(i => i.frame);

        foreach (var (frame, glyph, textureId, renderSize, srcRect) in GlyphTextures)
        {
            if (frame == largestFrame)
            {
                result += $"Frame: {frame} | Glyph: {glyph} | Texture ID: {textureId} | Render Size: {renderSize} | Source Rect: {srcRect}{Environment.NewLine}";
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
            result += $"Font File: {loadedFont.fontFileName} | Font Size: {loadedFont.fontSize}{Environment.NewLine}";
        }

        return result;
    }

    /// <summary>
    /// Returns all of the loaded textures.
    /// </summary>
    /// <returns>The string result of all the loaded textures.</returns>
    public static string GetLoadedTextures()
    {
        var result = new StringBuilder();

        foreach (var loadedTexture in LoadedTextures)
        {
            result.AppendLine($"Type: {loadedTexture.textureType} | Id: {loadedTexture.textureId} | Name: {loadedTexture.name}");
        }

        return result.ToString();
    }

    /// <summary>
    /// Records information about the textures that have been rendered for the two most recent frames.
    /// </summary>
    /// <param name="frame">The application frame.</param>
    /// <param name="glyph">The glyph being rendered.</param>
    /// <param name="fontAtlasTextureId">The font atlas texture ID.</param>
    /// <param name="renderSize">The size that the glyph will be rendered at.</param>
    /// <param name="destRect">The destination rectangle of the glyph.</param>
    internal static void RecordFontGlyphRendering(
        uint frame,
        char glyph,
        uint fontAtlasTextureId,
        float renderSize,
        RectangleF destRect)
    {
        GlyphTextures.Enqueue((frame, glyph, fontAtlasTextureId, renderSize, destRect));

        var largestFrame = GlyphTextures.Max(i => i.frame);
        var secondLargest = 0u;

        // Collect all of the unique frame numbers that have been rendered
        var frames = (from texture in GlyphTextures
            orderby texture.frame descending
            select texture.frame).Distinct().ToArray();

        var distinctResult = frames.Distinct().ToArray();

        // As long as there are at least two frames of info recorded
        if (distinctResult.Length >= 2)
        {
            secondLargest = distinctResult.Skip(1).First();
        }

        // As long as there are frames recorded, attempt to clean up all frames
        // that are older than the last two recorded frames
        if (largestFrame != 0u && secondLargest != 0u)
        {
            GlyphTextures.DequeueWhile(_ => frames.Length >= 2);
        }
    }

    /// <summary>
    /// Records information about the textures that have been loaded.
    /// </summary>
    /// <param name="textureType">The type of texture.</param>
    /// <param name="name">The name of the texture.</param>
    /// <param name="textureId">The OpenGL id of the texture.</param>
    internal static void RecordLoadedTexture(string textureType, string name, uint textureId)
    {
        var type = textureType switch
        {
            "Texture" => "Texture      ",
            "Texture Atlas" => "Texture Atlas",
            _ => throw new ArgumentOutOfRangeException(
                nameof(textureType),
                textureType,
                "Unknown texture type.  Only accept 'Texture' and 'Texture Atlas'")
        };

        LoadedTextures.Add((type, name, textureId));
    }

    /// <summary>
    /// Removes the recorded record of a loaded texture that matches the given <paramref name="textureId"/>.
    /// </summary>
    /// <param name="textureId">The OpenGL id of the texture.</param>
    internal static void RemoveLoadedTexture(uint textureId)
    {
        var foundItem = LoadedTextures
            .ToArray().FirstOrDefault(i => i.textureId == textureId);

        LoadedTextures.Remove(foundItem);
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

        // If there are not two sections and it does not contain any font metadata
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
    /// <param name="fontInfo">Clears the loaded font information.</param>
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
