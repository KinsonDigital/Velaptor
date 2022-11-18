// <copyright file="BatchServiceManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using Velaptor.Graphics;
using Velaptor.Guards;
using Velaptor.OpenGL;

namespace Velaptor.Services;

/// <inheritdoc />
internal sealed class BatchServiceManager : IBatchServiceManager
{
    private readonly IBatchingService<TextureBatchItem> textureBatchingService;
    private readonly IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService;
    private readonly IBatchingService<RectShape> rectBatchingService;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchServiceManager"/> class.
    /// </summary>
    /// <param name="textureBatchingService">Manages a batch of textures.</param>
    /// <param name="fontGlyphBatchingService">Manages a batch of font glyphs.</param>
    /// <param name="rectBatchingService">Manages the batch of rectangles.</param>
    public BatchServiceManager(
        IBatchingService<TextureBatchItem> textureBatchingService,
        IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService,
        IBatchingService<RectShape> rectBatchingService)
    {
        EnsureThat.ParamIsNotNull(textureBatchingService);
        EnsureThat.ParamIsNotNull(fontGlyphBatchingService);
        EnsureThat.ParamIsNotNull(rectBatchingService);

        this.textureBatchingService = textureBatchingService;
        this.textureBatchingService.ReadyForRendering += TextureBatchingServiceReadyForRendering;

        this.fontGlyphBatchingService = fontGlyphBatchingService;
        this.fontGlyphBatchingService.ReadyForRendering += FontGlyphBatchingServiceReadyForRendering;

        this.rectBatchingService = rectBatchingService;
        this.rectBatchingService.ReadyForRendering += RectBatchingServiceReadyForRendering;
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? TextureBatchReadyForRendering;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? FontGlyphBatchReadyForRendering;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? RectBatchReadyForRendering;

    /// <inheritdoc/>
    public ReadOnlyCollection<TextureBatchItem> TextureBatchItems
    {
        get => this.textureBatchingService.BatchItems;
        set => this.textureBatchingService.BatchItems = value;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<FontGlyphBatchItem> FontGlyphBatchItems
    {
        get => this.fontGlyphBatchingService.BatchItems;
        set => this.fontGlyphBatchingService.BatchItems = value;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<RectShape> RectBatchItems
    {
        get => this.rectBatchingService.BatchItems;
        set => this.rectBatchingService.BatchItems = value;
    }

    /// <inheritdoc/>
    public void AddTextureBatchItem(TextureBatchItem batchItem) => this.textureBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void AddFontGlyphBatchItem(FontGlyphBatchItem batchItem) => this.fontGlyphBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void AddRectBatchItem(RectShape batchItem) => this.rectBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void EmptyBatch(BatchServiceType serviceType)
    {
        switch (serviceType)
        {
            case BatchServiceType.Texture:
                this.textureBatchingService.EmptyBatch();
                break;
            case BatchServiceType.FontGlyph:
                this.fontGlyphBatchingService.EmptyBatch();
                break;
            case BatchServiceType.Rectangle:
                this.rectBatchingService.EmptyBatch();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(serviceType),
                    serviceType,
                    $"The enum '{nameof(BatchServiceType)}' value is invalid.");
        }
    }

    /// <inheritdoc/>
    public void EndBatch(BatchServiceType serviceType)
    {
        switch (serviceType)
        {
            case BatchServiceType.Texture:
                this.TextureBatchReadyForRendering?.Invoke(this, EventArgs.Empty);
                break;
            case BatchServiceType.FontGlyph:
                this.FontGlyphBatchReadyForRendering?.Invoke(this, EventArgs.Empty);
                break;
            case BatchServiceType.Rectangle:
                this.RectBatchReadyForRendering?.Invoke(this, EventArgs.Empty);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(serviceType),
                    serviceType,
                    $"The enum '{nameof(BatchServiceType)}' value is invalid.");
        }
    }

    /// <inheritdoc/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.textureBatchingService.ReadyForRendering -= TextureBatchingServiceReadyForRendering;
            this.fontGlyphBatchingService.ReadyForRendering -= FontGlyphBatchingServiceReadyForRendering;
            this.rectBatchingService.ReadyForRendering -= RectBatchingServiceReadyForRendering;
        }

        this.disposed = true;
    }

    /// <summary>
    /// Invoked the texture batch filled event.
    /// </summary>
    private void TextureBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.TextureBatchReadyForRendering?.Invoke(sender, e);

    /// <summary>
    /// Invoked the font glyph batch filled event.
    /// </summary>
    private void FontGlyphBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.FontGlyphBatchReadyForRendering?.Invoke(sender, e);

    /// <summary>
    /// Invoked the rectangle batch filled event.
    /// </summary>
    private void RectBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.RectBatchReadyForRendering?.Invoke(sender, e);
}
