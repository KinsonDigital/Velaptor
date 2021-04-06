// <copyright file="FreeTypeInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.NativeInterop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FreeTypeSharp.Native;

    // Refer to => https://github.com/ryancheung/FreeTypeSharp/issues/11 for cleanup questions being answered

    /*TODO List:
        1. Need to get the interop code implemented in here to avoid needing to use the FreeTypeSharp library
            * This will involve using PInvoke/Interop as well as bringing over some of the structs from FreeTypeSharp
        2. The invoker keeps track of the pointers library internally.  For every function that needs a library
            pointer as a param to do something, check if the library ptr does not and throw exception if it doesn't
        3. After implemented and working, convert all the pointers to SafeHandle types instead.  This will require some research
    */

    /// <summary>
    /// Invokes calls to the FreeType library for loading and managing fonts.
    /// </summary>
    /// <link
    /// <remarks>
    ///     For more information and documentation, refer to the https://www.freetype.org/ website.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    internal class FreeTypeInvoker : IFreeTypeInvoker
    {
        private readonly List<IntPtr> libraryPtrs = new List<IntPtr>();
        private bool isDisposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="FreeTypeInvoker"/> class.
        /// </summary>
        ~FreeTypeInvoker()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public event EventHandler<FreeTypeErrorEventArgs>? OnError;

        /// <inheritdoc/>
        public FT_Vector FT_Get_Kerning(IntPtr face, uint left_glyph, uint right_glyph, uint kern_mode)
        {
            var error = FT.FT_Get_Kerning(face, left_glyph, right_glyph, kern_mode, out FT_Vector akerning);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
                return default;
            }

            return akerning;
        }

        /// <inheritdoc/>
        public uint FT_Get_Char_Index(IntPtr face, uint charcode) => FT.FT_Get_Char_Index(face, charcode);

        /// <inheritdoc/>
        public FT_Error FT_Load_Glyph(IntPtr face, uint glyph_index, int laod_flags)
            => FT.FT_Load_Glyph(face, glyph_index, laod_flags);

        /// <inheritdoc/>
        public FT_Error FT_Load_Char(IntPtr face, uint char_code, int load_flags)
            => FT.FT_Load_Char(face, char_code, load_flags);

        /// <inheritdoc/>
        public FT_Error FT_Render_Glyph(IntPtr slot, FT_Render_Mode render_mode) => FT.FT_Render_Glyph(slot, render_mode);

        /// <inheritdoc/>
        public IntPtr FT_Init_FreeType()
        {
            var error = FT.FT_Init_FreeType(out IntPtr result);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
                return IntPtr.Zero;
            }

            this.libraryPtrs.Add(result);

            return result;
        }

        /// <inheritdoc/>
        public IntPtr FT_New_Face(IntPtr library, string filepathname, int face_index)
        {
            if (this.libraryPtrs.Contains(library) is false)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs($"The library pointer does not exist.  Have you called '{nameof(FT_Init_FreeType)}'?"));
                return IntPtr.Zero;
            }

            var error = FT.FT_New_Face(library, filepathname, face_index, out IntPtr aface);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
                return IntPtr.Zero;
            }

            return aface;
        }

        /// <inheritdoc/>
        public void FT_Set_Char_Size(IntPtr face, IntPtr char_width, IntPtr char_height, uint horz_resolution, uint vert_resolution)
        {
            var error = FT.FT_Set_Char_Size(face, char_width, char_height, horz_resolution, vert_resolution);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }
        }

        /// <inheritdoc/>
        public unsafe bool FT_Has_Kerning(IntPtr face)
        {
            var faceRec = (FT_FaceRec*)face;

            var result = (((int)faceRec->face_flags) & FT.FT_FACE_FLAG_KERNING) != 0;

            return result;
        }

        /// <inheritdoc/>
        public void FT_Done_Face(IntPtr face)
        {
            var error = FT.FT_Done_Face(face);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }
        }

        /// <inheritdoc/>
        public void FT_Done_Glyph(IntPtr glyph) => FT.FT_Done_Glyph(glyph);

        /// <inheritdoc/>
        public void FT_Done_FreeType(IntPtr library)
        {
            if (this.libraryPtrs.Contains(library) is false)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs($"The library pointer does not exist.  Have you called '{nameof(FT_Init_FreeType)}'?"));
                return;
            }

            var error = FT.FT_Done_FreeType(library);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
                return;
            }

            this.libraryPtrs.Remove(library);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of manged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                foreach (var libPtr in this.libraryPtrs)
                {
                    if (libPtr != IntPtr.Zero)
                    {
                        FT.FT_Done_FreeType(libPtr);
                    }
                }

                this.isDisposed = true;
            }
        }

        private static string CreateErrorMessage(string freeTypeMsg)
        {
            freeTypeMsg = freeTypeMsg.Replace("FT_Err", string.Empty);

            var result = string.Empty;

            var sections = freeTypeMsg.Split('_');

            foreach (var section in sections)
            {
                result += section;
            }

            return result;
        }
    }
}
