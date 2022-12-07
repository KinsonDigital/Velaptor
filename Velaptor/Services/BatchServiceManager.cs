// <copyright file="BatchServiceManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.ObjectModel;
using Guards;
using OpenGL;

/// <inheritdoc />
internal sealed class BatchServiceManager : IBatchServiceManager
{
    private readonly IBatchingService<TextureBatchItem> textureBatchingService;
    private readonly IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService;
    private readonly IBatchingService<RectBatchItem> rectBatchingService;
    private readonly IBatchingService<LineBatchItem> lineBatchingService;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchServiceManager"/> class.
    /// </summary>
    /// <param name="textureBatchingService">Manages a batch of textures.</param>
    /// <param name="fontGlyphBatchingService">Manages a batch of font glyphs.</param>
    /// <param name="rectBatchingService">Manages the batch of rectangles.</param>
    /// <param name="lineBatchingService">Manages the batch of lines.</param>
    public BatchServiceManager(
        IBatchingService<TextureBatchItem> textureBatchingService,
        IBatchingService<FontGlyphBatchItem> fontGlyphBatchingService,
        IBatchingService<RectBatchItem> rectBatchingService,
        IBatchingService<LineBatchItem> lineBatchingService)
    {
        EnsureThat.ParamIsNotNull(textureBatchingService);
        EnsureThat.ParamIsNotNull(fontGlyphBatchingService);
        EnsureThat.ParamIsNotNull(rectBatchingService);
        EnsureThat.ParamIsNotNull(lineBatchingService);

        this.textureBatchingService = textureBatchingService;
        this.textureBatchingService.ReadyForRendering += TextureBatchingServiceReadyForRendering;

        this.fontGlyphBatchingService = fontGlyphBatchingService;
        this.fontGlyphBatchingService.ReadyForRendering += FontGlyphBatchingServiceReadyForRendering;

        this.rectBatchingService = rectBatchingService;
        this.rectBatchingService.ReadyForRendering += RectBatchingServiceReadyForRendering;

        this.lineBatchingService = lineBatchingService;
        this.lineBatchingService.ReadyForRendering += LineBatchingServiceReadyForRendering;
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? TextureBatchReadyForRendering;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? FontGlyphBatchReadyForRendering;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? RectBatchReadyForRendering;

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? LineBatchReadyForRendering;

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
    public ReadOnlyCollection<RectBatchItem> RectBatchItems
    {
        get => this.rectBatchingService.BatchItems;
        set => this.rectBatchingService.BatchItems = value;
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<LineBatchItem> LineBatchItems
    {
        get => this.lineBatchingService.BatchItems;
        set => this.lineBatchingService.BatchItems = value;
    }

    /// <inheritdoc/>
    public void AddTextureBatchItem(TextureBatchItem batchItem) => this.textureBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void AddFontGlyphBatchItem(FontGlyphBatchItem batchItem) => this.fontGlyphBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void AddRectBatchItem(RectBatchItem batchItem) => this.rectBatchingService.Add(batchItem);

    /// <inheritdoc/>
    public void AddLineBatchItem(LineBatchItem batchItem) => this.lineBatchingService.Add(batchItem);

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
            case BatchServiceType.Line:
                this.lineBatchingService.EmptyBatch();
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
            case BatchServiceType.Line:
                this.LineBatchReadyForRendering?.Invoke(this, EventArgs.Empty);
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
            this.lineBatchingService.ReadyForRendering -= LineBatchingServiceReadyForRendering;
        }

        this.disposed = true;
    }

    /// <summary>
    /// Invokes the texture batch filled event.
    /// </summary>
    private void TextureBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.TextureBatchReadyForRendering?.Invoke(sender, e);

    /// <summary>
    /// Invokes the font glyph batch filled event.
    /// </summary>
    private void FontGlyphBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.FontGlyphBatchReadyForRendering?.Invoke(sender, e);

    /// <summary>
    /// Invokes the rectangle batch filled event.
    /// </summary>
    private void RectBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.RectBatchReadyForRendering?.Invoke(sender, e);

    /// <summary>
    /// Invokes the line batch filled event.
    /// </summary>
    private void LineBatchingServiceReadyForRendering(object? sender, EventArgs e)
        => this.LineBatchReadyForRendering?.Invoke(sender, e);
}
