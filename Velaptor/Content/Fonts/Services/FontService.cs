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
using FreeTypeSharp.Native;
using Graphics;
using Guards;
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
    private readonly nint libraryPtr;
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

        this.libraryPtr = this.freeTypeInvoker.FT_Init_FreeType();

        this.freeTypeInvoker.OnError += FreeTypeInvoker_OnError;
    }

    /// <inheritdoc/>
    public nint CreateFontFace(string fontFilePath)
    {
        var result = this.freeTypeInvoker.FT_New_Face(this.libraryPtr, fontFilePath, 0);

        if (result == nint.Zero)
        {
            throw new LoadFontException("An invalid pointer value of zero was returned when creating a new font face.");
        }

        return result;
    }

    /// <inheritdoc/>
    public (byte[] pixelData, uint width, uint height) CreateGlyphImage(nint facePtr, char glyphChar, uint glyphIndex)
    {
        var face = Marshal.PtrToStructure<FT_FaceRec>(facePtr);

        this.freeTypeInvoker.FT_Load_Glyph(facePtr, glyphIndex, FT.FT_LOAD_RENDER);

        uint width;
        uint height;
        byte[] glyphBitmapData;

        unsafe
        {
            width = face.glyph->bitmap.width;
            height = face.glyph->bitmap.rows;

            glyphBitmapData = new byte[width * height];
            Marshal.Copy(face.glyph->bitmap.buffer, glyphBitmapData, 0, glyphBitmapData.Length);
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
                FT.FT_LOAD_BITMAP_METRICS_ONLY);

            unsafe
            {
                var face = Marshal.PtrToStructure<FT_FaceRec>(facePtr);

                if (Environment.Is64BitProcess)
                {
                    metric.Ascender = (int)face.size->metrics.ascender.ToInt64() >> 6;
                    metric.Descender = (int)face.size->metrics.descender.ToInt64() >> 6;
                    metric.Glyph = glyphKeyValue.Key;
                    metric.CharIndex = glyphKeyValue.Value;

                    metric.XMin = (int)face.bbox.xMin.ToInt64() >> 6;
                    metric.XMax = (int)face.bbox.xMax.ToInt64() >> 6;
                    metric.YMin = (int)face.bbox.yMin.ToInt64() >> 6;
                    metric.YMax = (int)face.bbox.yMax.ToInt64() >> 6;

                    metric.GlyphWidth = (int)face.glyph->metrics.width.ToInt64() >> 6;
                    metric.GlyphHeight = (int)face.glyph->metrics.height.ToInt64() >> 6;
                    metric.HorizontalAdvance = (int)face.glyph->metrics.horiAdvance.ToInt64() >> 6;
                    metric.HoriBearingX = (int)face.glyph->metrics.horiBearingX.ToInt64() >> 6;
                    metric.HoriBearingY = (int)face.glyph->metrics.horiBearingY.ToInt64() >> 6;
                }
                else
                {
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
    public bool HasKerning(nint facePtr)
    {
        bool result;

        if (facePtr == nint.Zero)
        {
            throw new Exception("The font face has not been created yet.");
        }

        unsafe
        {
            var faceRec = (FT_FaceRec*)facePtr;

            result = ((int)faceRec->face_flags & FT.FT_FACE_FLAG_KERNING) != 0;
        }

        return result;
    }

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
            (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT);

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
            var fontFace = CreateFontFace(fontFilePath);
            var faceRec = (FT_FaceRec*)fontFace;

            /* Style Values
                0 = regular
                1 = italic
                2 = bold
                3 = bold | italic
             */
            result = Environment.Is64BitProcess
                ? (FontStyle)faceRec->style_flags.ToInt64()
                : (FontStyle)faceRec->style_flags.ToInt32();

            this.freeTypeInvoker.FT_Done_Face(fontFace);
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
            var fontFace = CreateFontFace(fontFilePath);
            var faceRec = (FT_FaceRec*)fontFace;

            result = Marshal.PtrToStringAnsi(faceRec->family_name);

            this.freeTypeInvoker.FT_Done_Face(fontFace);
        }

        return result ?? string.Empty;
    }

    /// <inheritdoc/>
    public float GetFontScaledLineSpacing(nint facePtr, uint sizeInPoints)
    {
        if (facePtr == nint.Zero)
        {
            throw new ArgumentException("The font face pointer must not be null.", nameof(facePtr));
        }

        SetFontSize(facePtr, sizeInPoints);

        var face = Marshal.PtrToStructure<FT_FaceRec>(facePtr);

        unsafe
        {
            return face.size->metrics.height.ToInt64() >> 6;
        }
    }

    /// <inheritdoc/>
    public void DisposeFace(nint facePtr) => this.freeTypeInvoker.FT_Done_Face(facePtr);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.freeTypeInvoker.FT_Done_FreeType(this.libraryPtr);

        this.isDisposed = true;
    }

    /// <summary>
    /// Occurs when there is a <c>FreeType</c> associated error.
    /// </summary>
    private void FreeTypeInvoker_OnError(object? sender, FreeTypeErrorEventArgs e) =>
        throw new NotSupportedException($"The {nameof(FontService)}.{nameof(FreeTypeInvoker_OnError)}() method is not supported yet.");
}
