// <copyright file="FreeTypeService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Content.Exceptions;
using Content.Fonts;
using FreeTypeSharp;
using Graphics;
using FreeType;
using Velaptor.Services;

/// <summary>
/// Provides extensions to <c>FreeType</c> library operations to help simplify working with <c>FreeType</c>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Waiting for changes from GitHub issue")]
internal sealed class FreeTypeService : IFreeTypeService
{
    private readonly IFreeTypeInvoker freeTypeInvoker;
    private readonly ISystemDisplayService sysDisplayService;
    private readonly IPlatform platform;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeTypeService"/> class.
    /// </summary>
    /// <param name="freeTypeInvoker">Makes calls to the native <c>FreeType</c> library.</param>
    /// <param name="sysDisplayService">Provides information about the system display.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public FreeTypeService(
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

        var glyphBitmapData = facePtr.GetGlyphBitmapData();
        var width = facePtr.GetGlyphBitmapWidth();
        var height = facePtr.GetGlyphBitmapHeight();

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

            metric = metric with
            {
                Ascender = facePtr.GetMetricAscender(),
                Descender = facePtr.GetMetricDescender(),
                Glyph = glyphKeyValue.Key,
                CharIndex = glyphKeyValue.Value,
                XMin = facePtr.GetBBoxXMin(),
                XMax = facePtr.GetBBoxXMax(),
                YMin = facePtr.GetBBoxYMin(),
                YMax = facePtr.GetBBoxYMax(),
                GlyphWidth = facePtr.GetMetricWidth(),
                GlyphHeight = facePtr.GetMetricHeight(),
                HorizontalAdvance = facePtr.GetMetricHoriAdvance(),
                HoriBearingX = facePtr.GetMetricHoriBearingX(),
                HoriBearingY = facePtr.GetMetricHoriBearingY(),
            };

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
        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        var face = CreateFontFace(fontFilePath);

        /* Style Values
            0 = regular
            1 = italic
            2 = bold
            3 = bold | italic
         */

        var result = GetFontStyle(face, fontFilePath);
        this.freeTypeInvoker.FT_Done_Face(face);

        return result;
    }

    /// <inheritdoc/>
    public FontStyle GetFontStyle(nint facePtr, string fontFilePath)
    {
        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        /* Style Values
            0 = regular
            1 = italic
            2 = bold
            3 = bold | italic
         */
        var result = facePtr.GetFontStyle();

        return result;
    }

    /// <inheritdoc/>
    public string GetFamilyName(string fontFilePath)
    {
        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        var face = CreateFontFace(fontFilePath);

        var result = GetFamilyName(face, fontFilePath);
        this.freeTypeInvoker.FT_Done_Face(face);

        return result;
    }

    /// <inheritdoc/>
    public string GetFamilyName(nint facePtr, string fontFilePath)
    {
        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("The font file does not exist", fontFilePath);
        }

        var result = facePtr.GetFontFamilyName();

        return result;
    }

    /// <inheritdoc/>
    public float GetFontScaledLineSpacing(nint facePtr, uint sizeInPoints)
    {
        SetFontSize(facePtr, sizeInPoints);

        return facePtr.GetFontLineSpacing();
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

        this.freeTypeInvoker.FT_Done_FreeType();

        this.isDisposed = true;
    }

    /// <summary>
    /// Occurs when there is a <c>FreeType</c> associated error.
    /// </summary>
    private static void FreeTypeInvoker_OnError(object? sender, FreeTypeErrorEventArgs e) => throw new FontException(e.ErrorMessage);
}
