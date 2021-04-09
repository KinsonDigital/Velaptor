// <copyright file="FreeTypeInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1124 // Do not use regions
#pragma warning disable SA1514 // Element documentation header should be preceded by blank line
namespace Raptor.NativeInterop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FreeTypeSharp.Native;

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
        private IntPtr libraryPtr;
        private bool isDisposed;
        private IntPtr facePtr;

        /// <summary>
        /// Finalizes an instance of the <see cref="FreeTypeInvoker"/> class.
        /// </summary>
        ~FreeTypeInvoker()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public event EventHandler<FreeTypeErrorEventArgs>? OnError;

        #region Original Interop Calls
        /// <inheritdoc/>
        public FT_Vector FT_Get_Kerning(IntPtr face, uint left_glyph, uint right_glyph, uint kern_mode)
#pragma warning restore SA1514 // Element documentation header should be preceded by blank line
#pragma warning restore SA1124 // Do not use regions
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
        public FT_Error FT_Load_Glyph(IntPtr face, uint glyph_index, int load_flags)
            => FT.FT_Load_Glyph(face, glyph_index, load_flags);

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

            this.libraryPtr = result;

            return result;
        }

        /// <inheritdoc/>
        public IntPtr FT_New_Face(IntPtr library, string filepathname, int face_index)
        {
            if (this.libraryPtr != library)
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

            this.facePtr = aface;

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
        public void FT_Done_Face(IntPtr face)
        {
            var error = FT.FT_Done_Face(face);

            if (error != FT_Error.FT_Err_Ok)
            {
                OnError?.Invoke(this, new FreeTypeErrorEventArgs(CreateErrorMessage(error.ToString())));
            }

            this.facePtr = IntPtr.Zero;
        }

        /// <inheritdoc/>
        public void FT_Done_Glyph(IntPtr glyph) => FT.FT_Done_Glyph(glyph);

        /// <inheritdoc/>
        public void FT_Done_FreeType(IntPtr library)
        {
            if (this.libraryPtr != library)
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

            this.libraryPtr = IntPtr.Zero;
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions

        #region Helper Methods
        /// <inheritdoc/>
        public IntPtr GetFace() => this.facePtr;

        /// <inheritdoc/>
        public unsafe bool FT_Has_Kerning()
        {
            if (this.facePtr == IntPtr.Zero)
            {
                // TODO: This should invoke the error callback instead
                throw new Exception("The font face has not been created yet.");
            }

            var faceRec = (FT_FaceRec*)this.facePtr;

            var result = (((int)faceRec->face_flags) & FT.FT_FACE_FLAG_KERNING) != 0;

            return result;
        }
        #endregion

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
                FT.FT_Done_Face(this.facePtr);
                FT.FT_Done_FreeType(this.libraryPtr);

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Creates n error message from the standard Free Type message.
        /// </summary>
        /// <param name="freeTypeMsg">The free type message to change.</param>
        /// <returns>The C# friendly exception message.</returns>
        /// <remarks>
        ///     The standard free type error messages come from the <see cref="FT_Error"/> enum.
        /// </remarks>
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
