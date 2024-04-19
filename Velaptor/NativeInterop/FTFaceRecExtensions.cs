// <copyright file="FTFaceRecExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable InconsistentNaming
namespace Velaptor.NativeInterop;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Content.Exceptions;
using Content.Fonts;
using FreeTypeSharp;

/// <summary>
/// Provides helper methoeds for the <see cref="FT_FaceRec_"/> struct via a pointer.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "This is a helper class that is not needed to be tested.")]
internal static class FTFaceRecExtensions
{
    private const int FaceRectShiftAmount = 6;

    /// <summary>
    /// Gets the ascender value of the font for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The ascender value.</returns>
    public static float GetMetricAscender(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->size->metrics.ascender.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->size->metrics.ascender.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the descender value of the font for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The descender value.</returns>
    public static float GetMetricDescender(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->size->metrics.descender.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->size->metrics.descender.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the glyph width for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The glyph width.</returns>
    public static float GetMetricWidth(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->glyph->metrics.width.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->glyph->metrics.width.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the glyph height for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The glyph height.</returns>
    public static float GetMetricHeight(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->glyph->metrics.height.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->glyph->metrics.height.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the glyph horizontal advance for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The glyph horizontal advance.</returns>
    public static float GetMetricHoriAdvance(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->glyph->metrics.horiAdvance.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->glyph->metrics.horiAdvance.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the glyph horizontal bearing on the X axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The glyph horizontal bearing on the X axis.</returns>
    public static float GetMetricHoriBearingX(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->glyph->metrics.horiBearingX.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->glyph->metrics.horiBearingX.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the glyph horizontal bearing on the Y axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The glyph horizontal bearing on the Y axis.</returns>
    public static float GetMetricHoriBearingY(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->glyph->metrics.horiBearingY.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->glyph->metrics.horiBearingY.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the bounding box minimum on the X axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The bounding box minimum on the X axis.</returns>
    public static float GetBBoxXMin(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->bbox.xMin.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->bbox.xMin.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the bounding box maximum on the X axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The bounding box maximum on the X axis.</returns>
    public static float GetBBoxXMax(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->bbox.xMax.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->bbox.xMax.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the bounding box minimum on the Y axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The bounding box minimum on the Y axis.</returns>
    public static float GetBBoxYMin(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->bbox.yMin.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->bbox.yMin.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the bounding box maximum on the Y axis for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The bounding box maximum on the Y axis.</returns>
    public static float GetBBoxYMax(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->bbox.yMax.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->bbox.yMax.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the font style from a font represented by the given pointer to the font <paramref name="face"/>.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The font style.</returns>
    public static FontStyle GetFontStyle(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? (FontStyle)((FT_FaceRec_*)face)->style_flags.ToInt64()
                : (FontStyle)((FT_FaceRec_*)face)->style_flags.ToInt32();
        }
    }

    /// <summary>
    /// Gets the font family name from a font represented by the given pointer to the font <paramref name="face"/>.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The font family name.</returns>
    /// <exception cref="FontException">Thrown if the font family name is null or empty.</exception>
    public static string GetFontFamilyName(this nint face)
    {
        unsafe
        {
            var familyName = (nint)((FT_FaceRec_*)face)->family_name;

            if (familyName == 0)
            {
                throw new FontException("There was an issue getting the font family name.");
            }

            return Marshal.PtrToStringAnsi(familyName) !;
        }
    }

    /// <summary>
    /// Gets the line spacing of a font that is represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The line spacing.</returns>
    public static float GetFontLineSpacing(this nint face)
    {
        unsafe
        {
            return Environment.Is64BitProcess
                ? ((FT_FaceRec_*)face)->size->metrics.height.ToInt64() >> FaceRectShiftAmount
                : ((FT_FaceRec_*)face)->size->metrics.height.ToInt32() >> FaceRectShiftAmount;
        }
    }

    /// <summary>
    /// Gets the width of a glyph bitmap for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The width of the glyph bitmap.</returns>
    public static uint GetGlyphBitmapWidth(this nint face)
    {
        unsafe
        {
            return ((FT_FaceRec_*)face)->glyph->bitmap.width;
        }
    }

    /// <summary>
    /// Gets the height of a glyph bitmap for a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The height of the glyph bitmap.</returns>
    public static uint GetGlyphBitmapHeight(this nint face)
    {
        unsafe
        {
            return ((FT_FaceRec_*)face)->glyph->bitmap.rows;
        }
    }

    /// <summary>
    /// Gets the bitmap pixel data for a glyph in a font represented by the given font <paramref name="face"/> pointer.
    /// </summary>
    /// <param name="face">A pointer to the font face.</param>
    /// <returns>The bitmap pixel data.</returns>
    /// <remarks>
    ///     The data is in 8-bit grayscale format where each byte is represented by a single RGB color component.
    ///     <br/>
    ///     The color data is only RGB and does not come with an alpha value. Every 3 bytes represents a single pixel.
    /// </remarks>
    public static byte[] GetGlyphBitmapData(this nint face)
    {
        byte[] glyphBitmapData;

        unsafe
        {
            var width = face.GetGlyphBitmapWidth();
            var height = face.GetGlyphBitmapHeight();

            glyphBitmapData = new byte[width * height];

            var bufferPtr = (nint)((FT_FaceRec_*)face)->glyph->bitmap.buffer;

            Marshal.Copy(bufferPtr, glyphBitmapData, 0, glyphBitmapData.Length);
        }

        return glyphBitmapData;
    }
}
