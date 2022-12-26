// <copyright file="BufferManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using Factories;
using Guards;

/// <inheritdoc/>
internal sealed class BufferManager : IBufferManager
{
    private readonly IGPUBuffer<TextureBatchItem> textureBuffer;
    private readonly IGPUBuffer<FontGlyphBatchItem> fontGlyphBuffer;
    private readonly IGPUBuffer<RectBatchItem> rectBuffer;
    private readonly IGPUBuffer<LineBatchItem> lineBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferManager"/> class.
    /// </summary>
    /// <param name="bufferFactory">Creates various buffers.</param>
    public BufferManager(IGPUBufferFactory bufferFactory)
    {
        EnsureThat.ParamIsNotNull(bufferFactory);

        this.textureBuffer = bufferFactory.CreateTextureGPUBuffer();
        this.fontGlyphBuffer = bufferFactory.CreateFontGPUBuffer();
        this.rectBuffer = bufferFactory.CreateRectGPUBuffer();
        this.lineBuffer = bufferFactory.CreateLineGPUBuffer();
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
            case VelaptorBufferType.Line:
                this.lineBuffer.ViewPortSize = size;
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

    /// <inheritdoc/>
    public void UploadLineData(LineBatchItem data, uint batchIndex) => this.lineBuffer.UploadData(data, batchIndex);
}
