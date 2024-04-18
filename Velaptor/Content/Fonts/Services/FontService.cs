// <copyright file="FontService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Exceptions;
using FreeTypeSharp;
using Graphics;
using NativeInterop.FreeType;
using Velaptor.Services;

/// <summary>
/// Provides extensions to <c>FreeType</c> library operations to help simplify working with <c>FreeType</c>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Waiting for changes from GitHub issue")]
internal sealed class FontService : IFontService
{
    private readonly IFreeTypeInvoker freeTypeInvoker;
    private readonly ISystemDisplayService sysDisplayService;
    private readonly IPlatform platform;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontService"/> class.
    /// </summary>
    /// <param name="freeTypeInvoker">Makes calls to the native <c>FreeType</c> library.</param>
    /// <param name="sysDisplayService">Provides information about the system display.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public FontService(
        IFreeTypeInvoker freeTypeInvoker,
        ISystemDisplayService sysDisplayService,
        IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(freeTypeInvoker);
        ArgumentNullException.ThrowIfNull(sysDisplayService);
        ArgumentNullException.ThrowIfNull(platform);

        this.freeTypeInvoker = freeTypeInvoker;
        this.sysDisplayService = sysDisplayService;
        this.platform = platform;

        this.freeTypeInvoker.OnError += FreeTypeInvoker_OnError;
    }

    /// <inheritdoc/>
    public nint CreateFontFace(string fontFilePath)
    {
        var result = this.freeTypeInvoker.FT_New_Face(fontFilePath, 0);

        if (result == nint.Zero)
        {
            throw new FontException("An invalid pointer value of zero was returned when creating a new font face.");
        }

        return result;
    }

    /// <inheritdoc/>
    public (byte[] pixelData, uint width, uint height) CreateGlyphImage(nint facePtr, uint glyphIndex)
    {
        this.freeTypeInvoker.FT_Load_Glyph(facePtr, glyphIndex, FT_LOAD.FT_LOAD_RENDER);

        uint width;
        uint height;
        byte[] glyphBitmapData;

        unsafe
        {
            width = ((FT_FaceRec_*)facePtr)->glyph->bitmap.width;
            height = ((FT_FaceRec_*)facePtr)->glyph->bitmap.rows;

            glyphBitmapData = new byte[width * height];

            var bufferPtr = (nint)((FT_FaceRec_*)facePtr)->glyph->bitmap.buffer;

            Marshal.Copy(bufferPtr, glyphBitmapData, 0, glyphBitmapData.Length);
        }

        return (glyphBitmapData, width, height);
    }

    /// <inheritdoc/>
    public Dictionary<char, GlyphMetrics> CreateGlyphMetrics(nint facePtr, Dictionary<char, uint> glyphIndices)
    {
        var result = new Dictionary<char, GlyphMetrics>();

        foreach (var glyphKeyValue in glyphIndices)
        {
            GlyphMetrics metric = default;

            this.freeTypeInvoker.FT_Load_Glyph(
                facePtr,
                glyphKeyValue.Value,
                FT_LOAD.FT_LOAD_BITMAP_METRICS_ONLY);

            unsafe
            {
                FT_FaceRec_* faceRecPtr = (FT_FaceRec_*)facePtr;

                if (Environment.Is64BitProcess)
                {
                    metric = metric with
                    {
                        Ascender = faceRecPtr->size->metrics.ascender.ToInt64() >> 6,
                        Descender = faceRecPtr->size->metrics.descender.ToInt64() >> 6,
                        Glyph = glyphKeyValue.Key,
                        CharIndex = glyphKeyValue.Value,
                        XMin = faceRecPtr->bbox.xMin.ToInt64() >> 6,
                        XMax = faceRecPtr->bbox.xMax.ToInt64() >> 6,
                        YMin = faceRecPtr->bbox.yMin.ToInt64() >> 6,
                        YMax = faceRecPtr->bbox.yMax.ToInt64() >> 6,
                        GlyphWidth = faceRecPtr->glyph->metrics.width.ToInt64() >> 6,
                        GlyphHeight = faceRecPtr->glyph->metrics.height.ToInt64() >> 6,
                        HorizontalAdvance = faceRecPtr->glyph->metrics.horiAdvance.ToInt64() >> 6,
                        HoriBearingX = faceRecPtr->glyph->metrics.horiBearingX.ToInt64() >> 6,
                        HoriBearingY = faceRecPtr->glyph->metrics.horiBearingY.ToInt64() >> 6,
                    };
                }
                else
                {
                    metric = metric with
                    {
                        Ascender = faceRecPtr->size->metrics.ascender.ToInt32() >> 6,
                        Descender = faceRecPtr->size->metrics.descender.ToInt32() >> 6,
                        Glyph = glyphKeyValue.Key,
                        CharIndex = glyphKeyValue.Value,
                        XMin = faceRecPtr->bbox.xMin.ToInt32() >> 6,
                        XMax = faceRecPtr->bbox.xMax.ToInt32() >> 6,
                        YMin = faceRecPtr->bbox.yMin.ToInt32() >> 6,
                        YMax = faceRecPtr->bbox.yMax.ToInt32() >> 6,
                        GlyphWidth = faceRecPtr->glyph->metrics.width.ToInt32() >> 6,
                        GlyphHeight = faceRecPtr->glyph->metrics.height.ToInt32() >> 6,
                        HorizontalAdvance = faceRecPtr->glyph->metrics.horiAdvance.ToInt32() >> 6,
                        HoriBearingX = faceRecPtr->glyph->metrics.horiBearingX.ToInt32() >> 6,
                        HoriBearingY = faceRecPtr->glyph->metrics.horiBearingY.ToInt32() >> 6,
                    };
                }
            }

            result.Add(glyphKeyValue.Key, metric);
        }

        return result;
    }

    /// <inheritdoc/>
    public Dictionary<char, uint> GetGlyphIndices(nint facePtr, char[]? glyphChars)
    {
        if (glyphChars is null)
        {
            return new Dictionary<char, uint>();
        }

        var result = new Dictionary<char, uint>();

        foreach (var glyphChar in glyphChars)
        {
            // Gets the glyph image and the character map index
            var charIndex = this.freeTypeInvoker.FT_Get_Char_Index(facePtr, glyphChar);

            result.Add(glyphChar, charIndex);
        }

        return result;
    }

    /// <inheritdoc/>
    public void SetFontSize(nint facePtr, uint sizeInPoints)
    {
        if (sizeInPoints <= 0)
        {
            throw new ArgumentException("The font size must be larger than 0.", nameof(sizeInPoints));
        }

        var sizeInPointsPtr = (nint)(sizeInPoints << 6);

        this.freeTypeInvoker.FT_Set_Char_Size(
            facePtr,
            sizeInPointsPtr,
            sizeInPointsPtr,
            (uint)this.sysDisplayService.MainDisplay.HorizontalDPI,
            (uint)this.sysDisplayService.MainDisplay.VerticalDPI);
    }

    /// <inheritdoc/>
    public bool HasKerning(nint facePtr) => this.freeTypeInvoker.FT_Has_Kerning(facePtr);

    /// <inheritdoc/>
    public float GetKerning(nint facePtr, uint leftGlyphIndex, uint rightGlyphIndex)
    {
        if (!HasKerning(facePtr) || leftGlyphIndex == 0 || rightGlyphIndex == 0)
        {
            return 0f;
        }

        var result = this.freeTypeInvoker.FT_Get_Kerning(
            facePtr,
            leftGlyphIndex,
            rightGlyphIndex,
            (uint)FT_Kerning_Mode_.FT_KERNING_DEFAULT);

        return this.platform.Is32BitProcess
            ? (float)(result.x.ToInt32() >> 6)
            : result.x.ToInt64() >> 6;
    }

    /// <inheritdoc/>
    public FontStyle GetFontStyle(string fontFilePath)
    {
        FontStyle result;

        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        unsafe
        {
            var face = (FT_FaceRec_*)CreateFontFace(fontFilePath);

            /* Style Values
                0 = regular
                1 = italic
                2 = bold
                3 = bold | italic
             */

            result = Environment.Is64BitProcess
                ? (FontStyle)face->style_flags.ToInt64()
                : (FontStyle)face->style_flags.ToInt32();

            this.freeTypeInvoker.FT_Done_Face((nint)face);
        }

        return result;
    }

    /// <inheritdoc/>
    public string GetFamilyName(string fontFilePath)
    {
        string? result;

        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        unsafe
        {
            var face = (FT_FaceRec_*)CreateFontFace(fontFilePath);

            result = Marshal.PtrToStringAnsi((nint)face->family_name);

            this.freeTypeInvoker.FT_Done_Face((nint)face);
        }

        return result ?? string.Empty;
    }

    /// <inheritdoc/>
    public float GetFontScaledLineSpacing(nint facePtr, uint sizeInPoints)
    {
        SetFontSize(facePtr, sizeInPoints);

        unsafe
        {
            // TODO: See if this unsafe code can be moved to the invoker somehow
            return ((FT_FaceRec_*)facePtr)->size->metrics.height.ToInt64() >> 6;
        }
    }

    /// <inheritdoc/>
    public void DisposeFace(nint facePtr) => this.freeTypeInvoker.FT_Done_Face(facePtr);

    // TODO: Verify if the DisposeFace or Dispose() methods are being invoked or used.

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.freeTypeInvoker.FT_Done_FreeType();

        this.isDisposed = true;
    }

    /// <summary>
    /// Occurs when there is a <c>FreeType</c> associated error.
    /// </summary>
    private static void FreeTypeInvoker_OnError(object? sender, FreeTypeErrorEventArgs e) => throw new FontException(e.ErrorMessage);
}
