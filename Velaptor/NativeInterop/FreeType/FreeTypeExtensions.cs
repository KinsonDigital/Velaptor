// <copyright file="FreeTypeExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.FreeType
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using FreeTypeSharp.Native;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides extensions to free type library operations to help simplify working with free type.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class FreeTypeExtensions : IFreeTypeExtensions
    {
        private readonly IFreeTypeInvoker freeTypeInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTypeExtensions"/> class.
        /// </summary>
        /// <param name="freeTypeInvoker">Makes calls to the native FreeType library.</param>
        public FreeTypeExtensions(IFreeTypeInvoker freeTypeInvoker) => this.freeTypeInvoker = freeTypeInvoker;

        /// <inheritdoc/>
        public IntPtr CreateFontFace(IntPtr freeTypeLibPtr, string fontFilePath)
        {
            var result = this.freeTypeInvoker.FT_New_Face(freeTypeLibPtr, fontFilePath, 0);

            if (result == IntPtr.Zero)
            {
                throw new LoadFontException("An invalid pointer value of zero was returned when creating a new font face.");
            }

            return result;
        }

        /// <inheritdoc/>
        public (byte[] pixelData, uint width, uint height) CreateGlyphImage(IntPtr facePtr, char glyphChar, uint glyphIndex)
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
        public Dictionary<char, GlyphMetrics> CreateGlyphMetrics(IntPtr facePtr, Dictionary<char, uint> glyphIndices)
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

                    // TODO: Create helper method that can be used to simplify this 64 bit logic
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
        public Dictionary<char, uint> GetGlyphIndices(IntPtr facePtr, char[] glyphChars)
        {
            if (glyphChars is null)
            {
                return new Dictionary<char, uint>();
            }

            var result = new Dictionary<char, uint>();

            for (var i = 0; i < glyphChars.Length; i++)
            {
                var glyphChar = glyphChars[i];

                // Get the glyph image and the character map index
                var charIndex = this.freeTypeInvoker.FT_Get_Char_Index(facePtr, glyphChar);

                result.Add(glyphChar, charIndex);
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetCharacterSize(IntPtr facePtr, int sizeInPoints, uint horiResolution, uint vertResolution)
        {
            var sizeInPointsPtr = (IntPtr)(sizeInPoints << 6);

            this.freeTypeInvoker.FT_Set_Char_Size(
                facePtr,
                sizeInPointsPtr,
                sizeInPointsPtr,
                horiResolution,
                vertResolution);
        }

        /// <inheritdoc/>
        public bool HasKerning(IntPtr facePtr)
        {
            bool result;

            if (facePtr == IntPtr.Zero)
            {
                // TODO: This should invoke the error callback instead
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
        public FontStyle GetFontStyle(string fontFilePath)
        {
            FontStyle result;

            if (File.Exists(fontFilePath) is false)
            {
                throw new FileNotFoundException("The font file does not exist", fontFilePath);
            }

            unsafe
            {
                var freeTypeLibPtr = this.freeTypeInvoker.FT_Init_FreeType();
                var fontFace = CreateFontFace(freeTypeLibPtr, fontFilePath);
                var faceRec = (FT_FaceRec*)fontFace;

                /* Style Values
                    0 = regular
                    1 = italic
                    2 = bold
                    3 = bold & italic
                 */
                result = Environment.Is64BitProcess
                    ? (FontStyle)faceRec->style_flags.ToInt64()
                    : (FontStyle)faceRec->style_flags.ToInt32();

                this.freeTypeInvoker.FT_Done_Face(fontFace);
            }

            return result;
        }

        /// <inheritdoc/>
        public float GetFontScaledLineSpacing(IntPtr facePtr)
        {
            var face = Marshal.PtrToStructure<FT_FaceRec>(facePtr);

            unsafe
            {
                return (face.size->metrics.height.ToInt64() >> 6) / 64f;
            }
        }
    }
}
