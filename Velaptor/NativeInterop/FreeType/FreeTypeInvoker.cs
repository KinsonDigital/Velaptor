// <copyright file="FreeTypeInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.FreeType;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FreeTypeSharp.Native;
using Guards;

/// <summary>
/// Invokes calls to the <c>FreeType</c> library for loading and managing fonts.
/// </summary>
/// <remarks>
///     For more information and documentation, refer to the https://www.freetype.org/ website.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the FreeType library.")]
internal sealed class FreeTypeInvoker : IFreeTypeInvoker
{
    /// <inheritdoc/>
    public event EventHandler<FreeTypeErrorEventArgs>? OnError;

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    /// <inheritdoc/>
    public FT_Vector FT_Get_Kerning(nint face, uint left_glyph, uint right_glyph, uint kern_mode)
    {
        EnsureThat.PointerIsNotNull(face);

        var error = FT.FT_Get_Kerning(face, left_glyph, right_glyph, kern_mode, out var akerning);

        if (error == FT_Error.FT_Err_Ok)
        {
            return akerning;
        }

        this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
        return default;
    }

    /// <inheritdoc/>
    public uint FT_Get_Char_Index(nint face, uint charcode)
    {
        EnsureThat.PointerIsNotNull(face);

        return FT.FT_Get_Char_Index(face, charcode);
    }

    /// <inheritdoc/>
    public FT_Error FT_Load_Glyph(nint face, uint glyph_index, int load_flags)
    {
        EnsureThat.PointerIsNotNull(face);

        return FT.FT_Load_Glyph(face, glyph_index, load_flags);
    }

    /// <inheritdoc/>
    public FT_Error FT_Load_Char(nint face, uint char_code, int load_flags)
    {
        EnsureThat.PointerIsNotNull(face);

        return FT.FT_Load_Char(face, char_code, load_flags);
    }

    /// <inheritdoc/>
    public FT_Error FT_Render_Glyph(nint slot, FT_Render_Mode render_mode)
    {
        EnsureThat.PointerIsNotNull(slot);

        return FT.FT_Render_Glyph(slot, render_mode);
    }

    /// <inheritdoc/>
    public nint FT_Init_FreeType()
    {
        var error = FT.FT_Init_FreeType(out var result);

        if (error == FT_Error.FT_Err_Ok)
        {
            return result;
        }

        this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));

        return 0;
    }

    /// <inheritdoc/>
    public nint FT_New_Face(nint library, string filepathname, int face_index)
    {
        EnsureThat.PointerIsNotNull(library);

        var error = FT.FT_New_Face(library, filepathname, face_index, out var aface);

        if (error == FT_Error.FT_Err_Ok)
        {
            return aface;
        }

        this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));

        return 0;
    }

    /// <inheritdoc/>
    public void FT_Set_Char_Size(nint face, nint char_width, nint char_height, uint horz_resolution, uint vert_resolution)
    {
        EnsureThat.PointerIsNotNull(face);
        EnsureThat.PointerIsNotNull(char_width);
        EnsureThat.PointerIsNotNull(char_height);

        var error = FT.FT_Set_Char_Size(face, char_width, char_height, horz_resolution, vert_resolution);

        if (error != FT_Error.FT_Err_Ok)
        {
            this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
        }
    }

    /// <inheritdoc/>
    public void FT_Done_Face(nint face)
    {
        EnsureThat.PointerIsNotNull(face);

        var error = FT.FT_Done_Face(face);

        if (error != FT_Error.FT_Err_Ok)
        {
            this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
        }
    }

    /// <inheritdoc/>
    public void FT_Done_Glyph(nint glyph)
    {
        EnsureThat.PointerIsNotNull(glyph);

        FT.FT_Done_Glyph(glyph);
    }

    /// <inheritdoc/>
    public void FT_Done_FreeType(nint library)
    {
        EnsureThat.PointerIsNotNull(library);

        var error = FT.FT_Done_FreeType(library);

        if (error != FT_Error.FT_Err_Ok)
        {
            this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
        }
    }

    // ReSharper restore IdentifierTypo
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Creates an error message from the standard <c>FreeType</c> message.
    /// </summary>
    /// <param name="freeTypeMsg">The error message coming from the <c>FreeType</c> library.</param>
    /// <returns>The C# friendly exception message.</returns>
    /// <remarks>
    ///     The <c>FreeType</c> error message.
    /// </remarks>
    private static string CreateErrorMessage(string freeTypeMsg)
    {
        freeTypeMsg = freeTypeMsg.RemoveAll("FT_Err");

        var result = string.Empty;

        var sections = freeTypeMsg.Split('_');

        return sections.Aggregate(result, (current, section) => current + section);
    }
}
