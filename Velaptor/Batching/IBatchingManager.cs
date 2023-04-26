// <copyright file="IBatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using OpenGL.Batching;

/// <summary>
/// Manages batch items that are used for rendering.
/// </summary>
internal interface IBatchingManager
{
    /// <summary>
    /// Adds a texture item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item.</param>
    /// <param name="renderStamp">The date and time when the item was rendered.</param>
    void AddTextureItem(TextureBatchItem item, int layer, DateTime renderStamp);

    /// <summary>
    /// Adds a font glyph item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item.</param>
    /// <param name="renderStamp">The date and time of the when the item was rendered.</param>
    void AddFontItem(FontGlyphBatchItem item, int layer, DateTime renderStamp);

    /// <summary>
    /// Adds a rectangle item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item.</param>
    /// <param name="renderStamp">The date and time of the when the item was rendered.</param>
    void AddRectItem(ShapeBatchItem item, int layer, DateTime renderStamp);

    /// <summary>
    /// Adds a line item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item.</param>
    /// <param name="renderStamp">The date and time of the when the item was rendered.</param>
    void AddLineItem(LineBatchItem item, int layer, DateTime renderStamp);
}
