// <copyright file="IBufferManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

/// <summary>
/// Manages GPU buffers.
/// </summary>
internal interface IBufferManager
{
    /// <summary>
    /// Sets the viewport size for a buffer that matches the given <paramref name="bufferType"/>.
    /// </summary>
    /// <param name="bufferType">The type of GPU buffer.</param>
    /// <param name="size">The size to set the buffer to.</param>
    void SetViewPortSize(VelaptorBufferType bufferType, SizeU size);

    /// <summary>
    /// Uploads the GPU buffer using the given texture <paramref name="data"/>
    /// at the given <paramref name="batchIndex"/> location.
    /// </summary>
    /// <param name="data">The texture data to upload.</param>
    /// <param name="batchIndex">The index location of the data to update.</param>
    /// <remarks>
    ///     Think of the <paramref name="batchIndex"/> as the offset/location of
    ///     the data in GPU memory. For example, if you think of the memory
    ///     being laid out like an array of data, this would be the location
    ///     of the 'chunk' of <paramref name="data"/> in the array.
    /// </remarks>
    void UploadTextureData(TextureBatchItem data, uint batchIndex);

    /// <summary>
    /// Uploads the GPU buffer using the given font glyph <paramref name="data"/>
    /// at the given <paramref name="batchIndex"/> location.
    /// </summary>
    /// <param name="data">The font data to upload.</param>
    /// <param name="batchIndex">The index location of the data to update.</param>
    /// <remarks>
    ///     Think of the <paramref name="batchIndex"/> as the offset/location of
    ///     the data in GPU memory. For example, if you think of the memory
    ///     being laid out like an array of data, this would be the location
    ///     of the 'chunk' of <paramref name="data"/> in the array.
    /// </remarks>
    void UploadFontGlyphData(FontGlyphBatchItem data, uint batchIndex);

    /// <summary>
    /// Uploads the GPU buffer using the given rectangle <paramref name="data"/>
    /// at the given <paramref name="batchIndex"/> location.
    /// </summary>
    /// <param name="data">The rectangle data to upload.</param>
    /// <param name="batchIndex">The index location of the data to update.</param>
    /// <remarks>
    ///     Think of the <paramref name="batchIndex"/> as the offset/location of
    ///     the data in GPU memory. For example, if you think of the memory
    ///     being laid out like an array of data, this would be the location
    ///     of the 'chunk' of <paramref name="data"/> in the array.
    /// </remarks>
    void UploadRectData(RectBatchItem data, uint batchIndex);

    /// <summary>
    /// Uploads the GPU buffer using the given line <paramref name="data"/>
    /// at the given <paramref name="batchIndex"/> location.
    /// </summary>
    /// <param name="data">The line data to upload.</param>
    /// <param name="batchIndex">The index location of the data to update.</param>
    /// <remarks>
    ///     Think of the <paramref name="batchIndex"/> as the offset/location of
    ///     the data in GPU memory. For example, if you think of the memory
    ///     being laid out like an array of data, this would be the location
    ///     of the 'chunk' of <paramref name="data"/> in the array.
    /// </remarks>
    void UploadLineData(LineBatchItem data, uint batchIndex);
}
