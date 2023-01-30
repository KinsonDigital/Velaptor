// <copyright file="IBatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

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
    /// <param name="layer">The layer to add the item on.</param>
    void AddTextureItem(TextureBatchItem item, int layer);

    /// <summary>
    /// Adds a font glyph item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item on.</param>
    void AddFontItem(FontGlyphBatchItem item, int layer);

    /// <summary>
    /// Adds a rectangle item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item on.</param>
    void AddRectItem(RectBatchItem item, int layer);

    /// <summary>
    /// Adds a line item to the batch.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="layer">The layer to add the item on.</param>
    void AddLineItem(LineBatchItem item, int layer);
}
