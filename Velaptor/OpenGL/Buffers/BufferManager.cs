// <copyright file="BufferManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Factories;
using Velaptor.Guards;

namespace Velaptor.OpenGL.Buffers;

/// <inheritdoc/>
internal sealed class BufferManager : IBufferManager
{
    private readonly IGPUBuffer<TextureBatchItem> textureBuffer;
    private readonly IGPUBuffer<FontGlyphBatchItem> fontGlyphBuffer;
    private readonly IGPUBuffer<RectBatchItem> rectBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferManager"/> class.
    /// </summary>
    /// <param name="bufferFactory">Creates various buffers.</param>
    public BufferManager(IGPUBufferFactory bufferFactory)
    {
        EnsureThat.ParamIsNotNull(bufferFactory); // TODO: Add test for this

        this.textureBuffer = bufferFactory.CreateTextureGPUBuffer();
        this.fontGlyphBuffer = bufferFactory.CreateFontGPUBuffer();
        this.rectBuffer = bufferFactory.CreateRectGPUBuffer();
    }

    /// <inheritdoc/>
    public void SetViewPortSize(VelaptorBufferType bufferType, SizeU size)
    {
        switch (bufferType)
        {
            case VelaptorBufferType.Texture:
                this.textureBuffer.ViewPortSize = size;
                break;
            case VelaptorBufferType.Font:
                this.fontGlyphBuffer.ViewPortSize = size;
                break;
            case VelaptorBufferType.Rectangle:
                this.rectBuffer.ViewPortSize = size;
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(bufferType),
                    bufferType,
                    $"The enum '{nameof(VelaptorBufferType)}' value is invalid.");
        }
    }

    /// <inheritdoc/>
    public void UploadTextureData(TextureBatchItem data, uint batchIndex) => this.textureBuffer.UploadData(data, batchIndex);

    /// <inheritdoc/>
    public void UploadFontGlyphData(FontGlyphBatchItem data, uint batchIndex) => this.fontGlyphBuffer.UploadData(data, batchIndex);

    /// <inheritdoc/>
    public void UploadRectData(RectBatchItem data, uint batchIndex) => this.rectBuffer.UploadData(data, batchIndex);
}
