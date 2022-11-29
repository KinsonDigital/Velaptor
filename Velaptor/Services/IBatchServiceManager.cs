// <copyright file="IBatchServiceManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.ObjectModel;
using OpenGL;

/// <summary>
/// Manages batching service operations.
/// </summary>
internal interface IBatchServiceManager : IDisposable
{
    /// <summary>
    /// Invoked when the texture batch has been filled.
    /// </summary>
    event EventHandler<EventArgs>? TextureBatchReadyForRendering;

    /// <summary>
    /// Invoked when the font glyph batch has been filled.
    /// </summary>
    event EventHandler<EventArgs>? FontGlyphBatchReadyForRendering;

    /// <summary>
    /// Invoked when the rectangle batch has been filled.
    /// </summary>
    event EventHandler<EventArgs>? RectBatchReadyForRendering;

    /// <summary>
    /// Gets or sets the list of texture batch items.
    /// </summary>
    ReadOnlyCollection<TextureBatchItem> TextureBatchItems { get; set; }

    /// <summary>
    /// Gets or sets the list of font glyph batch items.
    /// </summary>
    ReadOnlyCollection<FontGlyphBatchItem> FontGlyphBatchItems { get; set; }

    /// <summary>
    /// Gets or sets the list of rectangle batch items.
    /// </summary>
    ReadOnlyCollection<RectBatchItem> RectBatchItems { get; set; }

    /// <summary>
    /// Adds the given texture <paramref name="batchItem"/> to the batch.
    /// </summary>
    /// <param name="batchItem">The texture batch item to add.</param>
    void AddTextureBatchItem(TextureBatchItem batchItem);

    /// <summary>
    /// Adds the given font glyph <paramref name="batchItem"/> to the batch.
    /// </summary>
    /// <param name="batchItem">The font glyph batch item to add.</param>
    void AddFontGlyphBatchItem(FontGlyphBatchItem batchItem);

    /// <summary>
    /// Adds the given rectangle <paramref name="batchItem"/> to the batch.
    /// </summary>
    /// <param name="batchItem">The rectangle batch item to add.</param>
    void AddRectBatchItem(RectBatchItem batchItem);

    /// <summary>
    /// Empties the batching service that matches the given <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of batching service.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="serviceType"/> has an invalid value.
    /// </exception>
    void EmptyBatch(BatchServiceType serviceType);

    /// <summary>
    /// Ends the batch for the given <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The type of batching service.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <paramref name="serviceType"/> has an invalid value.
    /// </exception>
    void EndBatch(BatchServiceType serviceType);
}
