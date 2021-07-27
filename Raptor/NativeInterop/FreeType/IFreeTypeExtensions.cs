// <copyright file="IFreeTypeExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.FreeType
{
    using System;
    using System.Collections.Generic;
    using Velaptor.Graphics;

    /// <summary>
    /// Provides extensions to free type library operations to help simplify working with free type.
    /// </summary>
    internal interface IFreeTypeExtensions
    {
        /// <summary>
        /// Creates a new font face from a font file at the given <paramref name="fontFilepath"/>.
        /// </summary>
        /// <param name="freeTypeLibPtr">The pointer to the free type library.</param>
        /// <param name="fontFilePath">The path to the font file to create the face from.</param>
        /// <returns>
        ///     The pointer to the created font face.
        /// </returns>
        IntPtr CreateFontFace(IntPtr freeTypeLibPtr, string fontFilePath);

        /// <summary>
        /// Pulls the 8-bit grayscale bitmap data for the given <paramref name="glyphChar"/>.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="glyphChar">The glyph character to create the image from.</param>
        /// <param name="glyphIndex">The index of the glyph in the font file.</param>
        /// <returns>The 8-bit gray scale image pixel data with the width and height.</returns>
        (byte[] pixelData, uint width, uint height) CreateGlyphImage(IntPtr facePtr, char glyphChar, uint glyphIndex);

        /// <summary>
        /// Creates all of the glyph metrics for each glyph.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="glyphIndices">The glyph index for each glyph.</param>
        /// <returns>The glyph metrics for each glyph/character.</returns>
        Dictionary<char, GlyphMetrics> CreateGlyphMetrics(IntPtr facePtr, Dictionary<char, uint> glyphIndices);

        /// <summary>
        /// Gets all of the font indices fron the font file for each glyph.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="glyphChars">The list of glyphs to get the indices for.</param>
        /// <returns>The index for each glyph.</returns>
        Dictionary<char, uint> GetGlyphIndices(IntPtr facePtr, char[] glyphChars);

        /// <summary>
        /// Sets the nominal character size in points.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="sizeInPoints">The size in points to set the characters.</param>
        /// <param name="horiResolution">The horizontal resolution.</param>
        /// <param name="vertResolution">The vertical resolution.</param>
        void SetCharacterSize(IntPtr facePtr, int sizeInPoints, uint horiResolution, uint vertResolution);
    }
}
