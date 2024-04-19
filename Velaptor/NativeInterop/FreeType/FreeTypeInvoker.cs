// <copyright file="FreeTypeInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.FreeType;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using ExtensionMethods;
using FreeTypeSharp;
using Guards;

/// <summary>
/// Invokes calls to the <c>FreeType</c> library for loading and managing fonts.
/// </summary>
/// <remarks>
///     For more information and documentation, refer to the https://www.freetype.org/ website.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the FreeType library.")]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Param nameing to match original library.")]
internal sealed class FreeTypeInvoker : IFreeTypeInvoker
{
    private readonly FreeTypeLibrary library = new ();
    private FreeTypeFaceFacade? facade;

    /// <inheritdoc/>
    public event EventHandler<FreeTypeErrorEventArgs>? OnError;

    /// <inheritdoc/>
    public bool FT_Has_Kerning(nint face)
    {
        unsafe
        {
            this.facade ??= new FreeTypeFaceFacade(this.library, (FT_FaceRec_*)face);

            return this.facade.HasKerningFlag;
        }
    }

    /// <inheritdoc/>
    // TODO: Change the return type to a standard dotnet vector
    public FT_Vector_ FT_Get_Kerning(nint face, uint left_glyph, uint right_glyph, FT_Kerning_Mode_ kern_mode)
    {
        EnsureThat.PointerIsNotNull(face);

        unsafe
        {
            FT_Vector_ akerning;

            var error = FT.FT_Get_Kerning((FT_FaceRec_*)face, left_glyph, right_glyph, kern_mode, &akerning);

            if (error == FT_Error.FT_Err_Ok)
            {
                return akerning;
            }

            this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
        }

        return default;
    }

    /// <inheritdoc/>
    public uint FT_Get_Char_Index(nint face, uint charcode)
    {
        EnsureThat.PointerIsNotNull(face);

        unsafe
        {
            return FT.FT_Get_Char_Index((FT_FaceRec_*)face, charcode);
        }
    }

    /// <inheritdoc/>
    public FT_Error FT_Load_Glyph(nint face, uint glyph_index, FT_LOAD load_flags)
    {
        EnsureThat.PointerIsNotNull(face);

        unsafe
        {
            return FT.FT_Load_Glyph((FT_FaceRec_*)face, glyph_index, load_flags);
        }
    }

    /// <inheritdoc/>
    public FT_Error FT_Load_Char(nint face, uint char_code, FT_LOAD load_flags)
    {
        EnsureThat.PointerIsNotNull(face);

        unsafe
        {
            return FT.FT_Load_Char((FT_FaceRec_*)face, char_code, load_flags);
        }
    }

    /// <inheritdoc/>
    public FT_Error FT_Render_Glyph(nint slot, FT_Render_Mode_ render_mode)
    {
        EnsureThat.PointerIsNotNull(slot);

        unsafe
        {
            return FT.FT_Render_Glyph((FT_GlyphSlotRec_*)slot, render_mode);
        }
    }

    /// <inheritdoc/>
    public nint FT_New_Face(string filepathname, int face_index)
    {
        unsafe
        {
            FT_FaceRec_* facePtr;
            var error = FT.FT_New_Face(this.library.Native, (byte*)Marshal.StringToHGlobalAnsi(filepathname), face_index, &facePtr);

            if (error != FT_Error.FT_Err_Ok)
            {
                this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }

            return (nint)facePtr;
        }
    }

    /// <inheritdoc/>
    public void FT_Set_Char_Size(nint face, nint char_width, nint char_height, uint horz_resolution, uint vert_resolution)
    {
        EnsureThat.PointerIsNotNull(char_width);
        EnsureThat.PointerIsNotNull(char_height);
        EnsureThat.PointerIsNotNull(face);

        unsafe
        {
            var error = FT.FT_Set_Char_Size((FT_FaceRec_*)face, char_width, char_height, horz_resolution, vert_resolution);

            if (error != FT_Error.FT_Err_Ok)
            {
                this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }
        }
    }

    /// <inheritdoc/>
    public void FT_Done_Face(nint face)
    {
        unsafe
        {
            EnsureThat.PointerIsNotNull(face);

            var error = FT.FT_Done_Face((FT_FaceRec_*)face);

            if (error != FT_Error.FT_Err_Ok)
            {
                this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }
        }
    }

    /// <inheritdoc/>
    public void FT_Done_Glyph(nint glyph)
    {
        EnsureThat.PointerIsNotNull(glyph);

        unsafe
        {
            FT.FT_Done_Glyph((FT_GlyphRec_*)glyph);
        }
    }

    /// <inheritdoc/>
    public void FT_Done_FreeType()
    {
        unsafe
        {
            var error = FT.FT_Done_FreeType(this.library.Native);

            if (error != FT_Error.FT_Err_Ok)
            {
                this.OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }
        }
    }

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
