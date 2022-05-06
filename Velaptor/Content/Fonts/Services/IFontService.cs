// <copyright file="IFontService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts.Services
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides extensions to <c>FreeType</c> library operations to help simplify working with <c>FreeType</c>.
    /// </summary>
    internal interface IFontService : IDisposable
    {
        /// <summary>
        /// Creates a new font face from a font file at the given <paramref name="fontFilePath"/>.
        /// </summary>
        /// <param name="fontFilePath">The path to the font file to create the face.</param>
        /// <returns>
        ///     The pointer to the created font face.
        /// </returns>
        IntPtr CreateFontFace(string fontFilePath);

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
        /// Gets all of the font indices from the font file for each glyph.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="glyphChars">The list of glyphs to get the indices for.</param>
        /// <returns>The index for each glyph.</returns>
        Dictionary<char, uint> GetGlyphIndices(IntPtr facePtr, char[]? glyphChars);

        /// <summary>
        /// Sets the nominal character size in points.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="sizeInPoints">The size in points used to calculate the character size.</param>
        void SetFontSize(IntPtr facePtr, uint sizeInPoints);

        /// <summary>
        /// Returns a value indicating if the face uses kerning between two glyphs of the same face.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <returns><c>true</c> if the face uses kerning.</returns>
        bool HasKerning(IntPtr facePtr);

        /// <summary>
        /// Gets the kerning value between 2 glyphs.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="leftGlyphIndex">The character index of the glyph to the left of the right glyph.</param>
        /// <param name="rightGlyphIndex">The character index of the glyph to the right of the left glyph.</param>
        /// <returns>The kerning (horizontal spacing) between the glyphs.</returns>
        /// <remarks>
        /// Refer to the URL below for more info.
        /// <para>https://freetype.org/freetype2/docs/glyphs/glyphs-4.html#section-1.</para>
        /// </remarks>
        float GetKerning(IntPtr facePtr, uint leftGlyphIndex, uint rightGlyphIndex);

        /// <summary>
        /// Gets the style of the font at the given <paramref name="fontFilePath"/>.
        /// </summary>
        /// <param name="fontFilePath">The path to the font file.</param>
        /// <returns>The style of the font.</returns>
        FontStyle GetFontStyle(string fontFilePath);

        /// <summary>
        /// Gets the name of the font family of the font at the given <paramref name="fontFilePath"/>.
        /// </summary>
        /// <param name="fontFilePath">The path to the font file.</param>
        /// <returns>The family name of the font.</returns>
        string GetFamilyName(string fontFilePath);

        /// <summary>
        /// Returns the line spacing as a scaled value.
        /// </summary>
        /// <param name="facePtr">The pointer to the font face.</param>
        /// <param name="sizeInPoints">The size in points used to calculate the line spacing.</param>
        /// <returns>The line spacing as a scaled value.</returns>
        float GetFontScaledLineSpacing(IntPtr facePtr, uint sizeInPoints);

        /// <summary>
        /// Disposes of the font face that the given <paramref name="facePtr"/> points to.
        /// </summary>
        /// <param name="facePtr">The pointer to the face to dispose of.</param>
        void DisposeFace(IntPtr facePtr);
    }
}
