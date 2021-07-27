// <copyright file="IFreeTypeInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1124 // Do not use regions
namespace Velaptor.NativeInterop
{
    using System;
    using FreeTypeSharp.Native;
    using Velaptor.NativeInterop.FreeType;

    // TODO: Refactor invoke calls to follow this => https://docs.microsoft.com/en-us/dotnet/standard/native-interop/best-practices

    /// <summary>
    /// Invokes calls to the FreeType library for loading and managing fonts.
    /// </summary>
    /// <remarks>
    ///     For more information and documentation, refer to the https://www.freetype.org/ website.
    /// </remarks>
    internal interface IFreeTypeInvoker : IDisposable
    {
        /// <summary>
        /// Occurs when an error has occured.
        /// </summary>
        event EventHandler<FreeTypeErrorEventArgs> OnError;

        #region Original Interop Calls

        /// <summary>
        /// Initialize a new FreeType library object. The set of modules that are registered by this function is determined at build time.
        /// </summary>
        /// <returns>A handle to a new library object.</returns>
        IntPtr FT_Init_FreeType();

        /// <summary>
        /// Return the glyph index of a given character code. This function uses the currently selected charmap to do the mapping.
        /// </summary>
        /// <param name="face">A handle to the source face object.</param>
        /// <param name="charcode">The character code.</param>
        /// <returns>The glyoph index. 0 means 'undefined character code'.</returns>
        uint FT_Get_Char_Index(IntPtr face, uint charcode);

        /// <summary>
        /// Return the kerning vector between two glyphs of the same face.
        /// </summary>
        /// <param name="face">A handle to a source face object.</param>
        /// <param name="left_glyph">The index of the left glyph in the kern pair.</param>
        /// <param name="right_glyph">The index of the right glyph in the kern pari.</param>
        /// <param name="kern_mode">See <see cref="FT_Kerning_Mode"/> for more information. Determines the scale and dimension of the returned kerning vector.</param>
        /// <returns>
        ///     The kerning vector.  This is either in font units, fractional pixels (26.6 format), or pixels for scalable formats,
        ///     and in pixels for fixed-sizes formats.
        /// </returns>
        FT_Vector FT_Get_Kerning(IntPtr face, uint left_glyph, uint right_glyph, uint kern_mode);

        /// <summary>
        /// Load a glyph into the glyph slot of a face object.
        /// </summary>
        /// <param name="face">A handle to the target face object where the glyph is loaded.</param>
        /// <param name="glyph_index">
        ///     The index of the glyph in the font file. For CID-keyed fonts (either in PS or in CFF format)
        ///     this argument specifies the CID value.
        /// </param>
        /// <param name="load_flags">
        ///     A flag indicating what to load for this glyph. The FT_LOAD_XXX constants can be used to control
        ///     the glyph loading process (e.g., whether the outline should be scaled, whether to load bitmaps
        ///     or not, whether to hint the outline, etc).
        /// </param>
        /// <returns>FreeType error code. 0 means success.</returns>
        FT_Error FT_Load_Glyph(IntPtr face, uint glyph_index, int load_flags);

        /// <summary>
        /// Load a glyph into the glyph slot of a face object, accessed by its character code.
        /// </summary>
        /// <param name="face">A handle to a target face object where the glyph is loaded.</param>
        /// <param name="char_code">The glyph's character code, according to the current charmap used in the face.</param>
        /// <param name="load_flags">
        ///     A flag indicating what to load for this glyph. The FT_LOAD_XXX constants can be used to control
        ///     the glyph loading process (e.g., whether the outline should be scaled, whether to load bitmaps
        ///     or not, whether to hint the outline, etc).
        /// </param>
        /// <returns>FreeType error code. 0 means success.</returns>
        FT_Error FT_Load_Char(IntPtr face, uint char_code, int load_flags);

        /// <summary>
        /// Call <see cref="FT.FT_Open_Face"/> to open a font by its pathname.
        /// </summary>
        /// <param name="library">A handle to the libary resource.</param>
        /// <param name="pathname">A path to the font file.</param>
        /// <param name="face_index">See <see cref="FT.FT_Open_Face"/> for a detailed description of this parameter.</param>
        /// <returns>
        ///     A handle to a new face object. If face_index is greater than or equal to zero, it must not be NULL.
        /// </returns>
        IntPtr FT_New_Face(IntPtr library, string pathname, int face_index);

        /// <summary>
        /// Convert a given glyph image to a bitmap. It does so by inspecting the glyph image format, finding the relevant renderer, and invoking it.
        /// </summary>
        /// <param name="slot">A handle to the glyph slot containing the image to convert.</param>
        /// <param name="render_mode">
        ///     The render mode used to render the glyph image into a bitmap. See FT_Render_Mode
        ///     for a list ofpossible values.
        ///
        /// <para>
        ///     If FT_RENDER_MODE_NORMAL is used, a previous call of FT_Load_Glyph with flag FT_LOAD_COLOR
        ///     makes FT_Render_Glyph provide a default blending of colored glyph layers associated with the current
        ///     glyph slot(provided the font contains such layers) instead of rendering the glyph slot's outline.
        ///     This is an experimental feature; see FT_LOAD_COLOR for more information.
        /// </para>
        /// </param>
        /// <returns>FreeType error code. 0 means success.</returns>
        FT_Error FT_Render_Glyph(IntPtr slot, FT_Render_Mode render_mode);

        /// <summary>
        /// Call <see cref="FT.FT_Request_Size"/> to request the nominal size (in points).
        /// </summary>
        /// <param name="face">A handle to a target face object.</param>
        /// <param name="char_width">The nominal width, in 26.6 fractional points.</param>
        /// <param name="char_height">The nominal height, in 26.6 fractional points.</param>
        /// <param name="horz_resolution">The horizontal resolution in dpi.</param>
        /// <param name="vert_resolution">The vertical resolution in dpi.</param>
        void FT_Set_Char_Size(IntPtr face, IntPtr char_width, IntPtr char_height, uint horz_resolution, uint vert_resolution);

        /// <summary>
        /// Discard a given face object, as well as all of its child slots and sizes.
        /// </summary>
        /// <param name="face">A handle to a target face object.</param>
        void FT_Done_Face(IntPtr face);

        /// <summary>
        /// Destroy a given glyph.
        /// </summary>
        /// <param name="glyph">A handle to the target glyph object.</param>
        void FT_Done_Glyph(IntPtr glyph);

        /// <summary>
        /// Destroy a given FreeType library object and all of its children, including resources, drivers, faces, sizes, etc.
        /// </summary>
        /// <param name="library">A handle to the target library object.</param>
        void FT_Done_FreeType(IntPtr library);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the face pointer that was created using the <see cref="FT_New_Face(IntPtr, string, int)"/> call.
        /// </summary>
        /// <returns>The pointer to the font face.</returns>
        IntPtr GetFace();

        /// <summary>
        /// Returns a value indicating if the face uses kerning between two glyphs of the same face.
        /// </summary>
        /// <returns><see langword="true"/> if the face uses kerning.</returns>
        unsafe bool FT_Has_Kerning();

        #endregion
    }
}
